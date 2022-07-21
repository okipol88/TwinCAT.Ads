// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ValueAccess.AdsValueAccessor
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.ValueAccess
{
  /// <summary>Value accessor class (accessing ADS Symbol values)</summary>
  internal class AdsValueAccessor : AdsValueAccessorBase, IDisposable
  {
    private DynamicValueMarshaler _converter = new DynamicValueMarshaler();
    /// <summary>
    /// Indicates that the <see cref="T:TwinCAT.Ads.ValueAccess.AdsValueAccessor" /> is disposed.
    ///  </summary>
    protected bool disposed;
    private object _syncNotification = new object();
    /// <summary>The address (cached)</summary>
    private AmsAddress _address;
    private ValueAccessMode _accessMethod = ValueAccessMode.Symbolic;
    private AdsNotificationCache _notificationTable = new AdsNotificationCache();
    /// <summary>AdsStream for notification (dynamically resized)</summary>
    private MemoryStream _notificationStream = new MemoryStream();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.ValueAccess.AdsValueAccessor" /> class.
    /// </summary>
    /// <param name="connection">The Connection.</param>
    /// <param name="accessMethod">The access method.</param>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="defaultSettings">The default settings.</param>
    /// <exception cref="T:System.ArgumentNullException">adsClient</exception>
    /// <exception cref="T:System.ObjectDisposedException">AdsClient</exception>
    internal AdsValueAccessor(
      IAdsConnection connection,
      ValueAccessMode accessMethod,
      IAccessorValueFactory valueFactory,
      NotificationSettings defaultSettings)
      : base(valueFactory, (IConnection) connection, defaultSettings)
    {
      this._address = !AmsAddress.op_Equality(connection.Address, (AmsAddress) null) ? connection.Address : throw new ClientNotConnectedException();
      this._accessMethod = accessMethod;
      this.DefaultNotificationSettings = (INotificationSettings) defaultSettings;
      ((IAdsNotifications) connection).AdsNotification += new EventHandler<AdsNotificationEventArgs>(this.adsClient_AdsNotification);
      ((IAdsNotifications) connection).AdsNotificationError += new EventHandler<AdsNotificationErrorEventArgs>(this.adsClient_AdsNotificationError);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.ValueAccess.AdsValueAccessor" /> class.
    /// </summary>
    ~AdsValueAccessor() => this.Dispose(false);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (this.disposed)
        return;
      this.Dispose(true);
      this.disposed = true;
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      IAdsConnection connection = this.Connection;
      if (connection != null)
      {
        ((IAdsNotifications) connection).AdsNotification -= new EventHandler<AdsNotificationEventArgs>(this.adsClient_AdsNotification);
        ((IAdsNotifications) connection).AdsNotificationError -= new EventHandler<AdsNotificationErrorEventArgs>(this.adsClient_AdsNotificationError);
      }
      this._notificationStream.Dispose();
    }

    /// <summary>
    /// Handles the AdsNotification event of the adsClient control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:TwinCAT.Ads.AdsNotificationEventArgs" /> instance containing the event data.</param>
    private void adsClient_AdsNotification(object? sender, AdsNotificationEventArgs e)
    {
      uint handle = e.Handle;
      if (!(e.UserData is ISymbol userData))
        return;
      this.onAdsNotification(userData, e);
    }

    /// <summary>
    /// Handles the AdsNotificationError event of the adsClient control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:TwinCAT.Ads.AdsNotificationErrorEventArgs" /> instance containing the event data.</param>
    private void adsClient_AdsNotificationError(object? sender, AdsNotificationErrorEventArgs e) => AdsModule.Trace.TraceError(e.Exception);

    /// <summary>Handler function for the AdsNotification</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="args">The <see cref="T:TwinCAT.Ads.AdsNotificationEventArgs" /> instance containing the event data.</param>
    private void onAdsNotification(ISymbol symbol, AdsNotificationEventArgs args)
    {
      DateTimeOffset timeStamp = args.TimeStamp;
      DataType dataType = (DataType) ((IInstance) symbol).DataType;
      object syncNotification = this._syncNotification;
      bool lockTaken = false;
      byte[] array;
      try
      {
        Monitor.Enter(syncNotification, ref lockTaken);
        int byteSize = ((IBitSize) symbol).ByteSize;
        array = args.Data.ToArray();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(syncNotification);
      }
      SymbolNotificationTypes notificationType = this._notificationTable.GetNotificationType(symbol);
      if ((notificationType & 2) == 2)
        this.OnRawValueChanged(symbol, (ReadOnlyMemory<byte>) array, timeStamp);
      if ((notificationType & 1) != 1)
        return;
      this.OnValueChanged(symbol, (ReadOnlySpan<byte>) array, timeStamp);
    }

    /// <summary>Gets the ADS Connection</summary>
    /// <value>The client.</value>
    public IAdsConnection? Connection => base.Connection as IAdsConnection;

    /// <summary>Reads a value from the specified ADS address</summary>
    /// <param name="symbol">The address.</param>
    /// <param name="value">Raw value</param>
    /// <param name="readTime">The read time snapshot.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exclude />
    public override int TryReadRaw(
      ISymbol symbol,
      Memory<byte> value,
      out DateTimeOffset? readTime)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      IAdsSymbol symbol1 = (IAdsSymbol) Symbol.Unwrap(symbol);
      IProcessImageAddress iprocessImageAddress = (IProcessImageAddress) symbol1;
      ValueAccessMode valueAccessMode = this.calcAccessMethod((ISymbol) symbol1);
      int read = 0;
      readTime = new DateTimeOffset?(DateTimeOffset.MinValue);
      if (((IProcessImageAddress) symbol1).IsVirtual)
        return 1796;
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      using (new AdsConnectionRestore((AdsValueAccessorBase) this))
      {
        adsErrorCode = valueAccessMode != ValueAccessMode.IndexGroupOffset ? this.TryReadSymbolic(symbol, value, out read) : ((IAdsReadWrite) this.Connection).TryRead(iprocessImageAddress.IndexGroup, iprocessImageAddress.IndexOffset, value, ref read);
        if (adsErrorCode == null)
          readTime = new DateTimeOffset?(DateTimeOffset.Now);
        else
          value = (Memory<byte>) (byte[]) null;
      }
      return (int) adsErrorCode;
    }

    /// <summary>Tries to read the value by symbol</summary>
    /// <param name="address">The Symbol.</param>
    /// <param name="value">The Memory location</param>
    /// <param name="read">Number of read bytes.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ArgumentNullException">address</exception>
    /// <exception cref="T:TwinCAT.AdsException">Connection not established!</exception>
    private AdsErrorCode TryReadSymbolic(
      ISymbol address,
      Memory<byte> value,
      out int read)
    {
      if (address == null)
        throw new ArgumentNullException(nameof (address));
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) 0;
      using (new AdsConnectionRestore((AdsValueAccessorBase) this))
      {
        uint num = 0;
        adsErrorCode1 = ((IAdsHandle) this.Connection).TryCreateVariableHandle(((IInstance) address).InstancePath, ref num);
        read = 0;
        if (adsErrorCode1 == null)
        {
          try
          {
            adsErrorCode1 = ((IAdsHandle) this.Connection).TryRead(num, value, ref read);
          }
          finally
          {
            AdsErrorCode adsErrorCode2 = ((IAdsHandle) this.Connection).TryDeleteVariableHandle(num);
            if (adsErrorCode1 == null)
              adsErrorCode1 = adsErrorCode2;
          }
        }
      }
      return adsErrorCode1;
    }

    /// <summary>Read a Symbol value asynchronously as bytes .</summary>
    /// <param name="symbol">The symbol instance.</param>
    /// <param name="destination">The destination / value memory.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadRaw' operation. The <see cref="T:TwinCAT.ValueAccess.ResultReadRawAccess" /> result contains the
    /// (<see cref="P:TwinCAT.ValueAccess.ResultReadValueAccess`1.Value" />) and the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" /> after execution.
    /// </returns>
    public override async Task<ResultReadRawAccess> ReadRawAsync(
      ISymbol symbol,
      Memory<byte> destination,
      CancellationToken cancel)
    {
      AdsValueAccessor accessor = this;
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (accessor.Connection == null)
        throw new AdsException("Connection not established!");
      IAdsSymbol symbol1 = (IAdsSymbol) Symbol.Unwrap(symbol);
      IProcessImageAddress iprocessImageAddress = (IProcessImageAddress) symbol1;
      ValueAccessMode valueAccessMode = accessor.calcAccessMethod((ISymbol) symbol1);
      DateTimeOffset minValue = DateTimeOffset.MinValue;
      if (((IProcessImageAddress) symbol1).IsVirtual)
        return new ResultReadRawAccess(1796, 0U);
      ResultReadRawAccess resultReadRawAccess;
      using (new AdsConnectionRestore((AdsValueAccessorBase) accessor))
      {
        if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
        {
          ResultRead resultRead = await ((IAdsReadWrite) accessor.Connection).ReadAsync(iprocessImageAddress.IndexGroup, iprocessImageAddress.IndexOffset, destination, cancel).ConfigureAwait(false);
          resultReadRawAccess = !((ResultAds) resultRead).Succeeded ? new ResultReadRawAccess((int) ((ResultAds) resultRead).ErrorCode, ((ResultAds) resultRead).InvokeId) : new ResultReadRawAccess(destination, (int) ((ResultAds) resultRead).ErrorCode, DateTimeOffset.Now, ((ResultAds) resultRead).InvokeId);
        }
        else
          resultReadRawAccess = await accessor.ReadSymbolicRawAsync(symbol, destination, cancel).ConfigureAwait(false);
      }
      return resultReadRawAccess;
    }

    /// <summary>Reads the value by Symbol.</summary>
    /// <param name="address">The address.</param>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ArgumentNullException">address</exception>
    /// <exception cref="T:TwinCAT.AdsException">Connection not established!</exception>
    private async Task<ResultReadRawAccess> ReadSymbolicRawAsync(
      ISymbol address,
      Memory<byte> value,
      CancellationToken cancel)
    {
      AdsValueAccessor accessor = this;
      if (address == null)
        throw new ArgumentNullException(nameof (address));
      if (accessor.Connection == null)
        throw new AdsException("Connection not established!");
      ResultReadRawAccess result = ResultReadRawAccess.Empty;
      using (new AdsConnectionRestore((AdsValueAccessorBase) accessor))
      {
        ResultHandle resultHandle = await ((IAdsHandle) accessor.Connection).CreateVariableHandleAsync(((IInstance) address).InstancePath, cancel).ConfigureAwait(false);
        if (((ResultAds) resultHandle).Succeeded)
        {
          try
          {
            ResultRead resultRead = await ((IAdsHandle) accessor.Connection).ReadAsync(resultHandle.Handle, value, cancel).ConfigureAwait(false);
            result = new ResultReadRawAccess(value, (int) ((ResultAds) resultRead).ErrorCode, ((ResultAds) resultRead).InvokeId);
          }
          finally
          {
            ResultAds resultAds = await ((IAdsHandle) accessor.Connection).DeleteVariableHandleAsync(resultHandle.Handle, cancel).ConfigureAwait(false);
            if (((ResultAccess) result).ErrorCode == 0 && resultAds.Failed && resultAds.ErrorCode != 1808)
              ((ResultAccess) result).SetError((int) resultAds.ErrorCode);
          }
        }
        else
          result = new ResultReadRawAccess((int) ((ResultAds) resultHandle).ErrorCode, ((ResultAds) resultHandle).InvokeId);
        resultHandle = (ResultHandle) null;
      }
      ResultReadRawAccess resultReadRawAccess = result;
      result = (ResultReadRawAccess) null;
      return resultReadRawAccess;
    }

    /// <summary>Writes the raw memory data to the symbol.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="data">The data.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ArgumentNullException">address</exception>
    /// <exception cref="T:TwinCAT.AdsException">Connection not established!</exception>
    private AdsErrorCode TryWriteSymbolic(ISymbol symbol, ReadOnlyMemory<byte> data)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      uint num = 0;
      AdsErrorCode adsErrorCode;
      using (new AdsConnectionRestore((AdsValueAccessorBase) this))
      {
        adsErrorCode = ((IAdsHandle) this.Connection).TryCreateVariableHandle(((IInstance) symbol).InstancePath, ref num);
        if (adsErrorCode == null)
        {
          try
          {
            adsErrorCode = ((IAdsHandle) this.Connection).TryWrite(num, data);
          }
          finally
          {
            adsErrorCode = ((IAdsHandle) this.Connection).TryDeleteVariableHandle(num);
          }
        }
      }
      return adsErrorCode;
    }

    /// <summary>Writes the raw memory data to the specified symbol.</summary>
    /// <param name="symbol">The address.</param>
    /// <param name="data">The data.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultWriteAccess&gt;.</returns>
    /// <exception cref="T:System.ArgumentNullException">address</exception>
    /// <exception cref="T:TwinCAT.AdsException">Connection not established!</exception>
    private async Task<ResultWriteAccess> WriteSymbolicAsync(
      ISymbol symbol,
      ReadOnlyMemory<byte> data,
      CancellationToken cancel)
    {
      AdsValueAccessor accessor = this;
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (accessor.Connection == null)
        throw new AdsException("Connection not established!");
      ResultWriteAccess result = ResultWriteAccess.Empty;
      using (new AdsConnectionRestore((AdsValueAccessorBase) accessor))
      {
        ResultHandle resultHandle = await ((IAdsHandle) accessor.Connection).CreateVariableHandleAsync(((IInstance) symbol).InstancePath, cancel).ConfigureAwait(false);
        if (((ResultAds) resultHandle).Succeeded)
        {
          try
          {
            ResultWrite resultWrite = await ((IAdsHandle) accessor.Connection).WriteAsync(resultHandle.Handle, data, cancel).ConfigureAwait(false);
            result = new ResultWriteAccess((int) ((ResultAds) resultWrite).ErrorCode, DateTimeOffset.Now, ((ResultAds) resultWrite).InvokeId);
          }
          finally
          {
            ResultAds resultAds = await ((IAdsHandle) accessor.Connection).DeleteVariableHandleAsync(resultHandle.Handle, cancel).ConfigureAwait(false);
            if (resultAds.Failed)
              result = new ResultWriteAccess((int) resultAds.ErrorCode, DateTimeOffset.MinValue, resultAds.InvokeId);
          }
        }
        else
          result = new ResultWriteAccess(1808, DateTimeOffset.MinValue, ((ResultAds) resultHandle).InvokeId);
        resultHandle = (ResultHandle) null;
      }
      ResultWriteAccess resultWriteAccess = result;
      result = (ResultWriteAccess) null;
      return resultWriteAccess;
    }

    public override int TryInvokeRpcMethod(
      IInstance instance,
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outParametersSpec,
      AnyTypeSpecifier? retSpec,
      out object[]? outParameters,
      out object? returnValue,
      out DateTimeOffset? timeStamp)
    {
      if (instance == null)
        throw new ArgumentNullException(nameof (instance));
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      timeStamp = new DateTimeOffset?(DateTimeOffset.MinValue);
      IRpcCallableInstance callableInstance = (IRpcCallableInstance) instance;
      AdsErrorCode adsErrorCode;
      using (new AdsConnectionRestore((AdsValueAccessorBase) this))
      {
        adsErrorCode = ((IAdsRpcInvoke) this.Connection).TryInvokeRpcMethod(callableInstance, method, inParameters, outParametersSpec, retSpec, ref outParameters, ref returnValue);
        if (adsErrorCode == null)
          timeStamp = new DateTimeOffset?(DateTimeOffset.Now);
      }
      return (int) adsErrorCode;
    }

    public override async Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      IInstance instance,
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outParametersSpec,
      AnyTypeSpecifier? retSpec,
      CancellationToken cancel)
    {
      AdsValueAccessor accessor = this;
      if (instance == null)
        throw new ArgumentNullException(nameof (instance));
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (accessor.Connection == null)
        throw new AdsException("Connection not established!");
      DateTimeOffset invokeTime = DateTimeOffset.MinValue;
      IRpcCallableInstance callableInstance = (IRpcCallableInstance) instance;
      using (new AdsConnectionRestore((AdsValueAccessorBase) accessor))
      {
        ResultRpcMethod resultRpcMethod = await ((IAdsRpcInvoke) accessor.Connection).InvokeRpcMethodAsync(callableInstance, method, inParameters, outParametersSpec, retSpec, cancel).ConfigureAwait(false);
        if (!((ResultAds) resultRpcMethod).Succeeded)
          return new ResultRpcMethodAccess((object) null, (object[]) null, (int) ((ResultAds) resultRpcMethod).ErrorCode, invokeTime, ((ResultAds) resultRpcMethod).InvokeId);
        invokeTime = DateTimeOffset.Now;
        return new ResultRpcMethodAccess(resultRpcMethod.ReturnValue, resultRpcMethod.OutValues, (int) ((ResultAds) resultRpcMethod).ErrorCode, invokeTime, ((ResultAds) resultRpcMethod).InvokeId);
      }
    }

    /// <summary>Reads an array element value as bytes.</summary>
    /// <param name="arrayInstance">The array instance.</param>
    /// <param name="indices">The indices specify which element to read.</param>
    /// <param name="destination">The destination / value memory.</param>
    /// <param name="timeStamp">The read time snapshot</param>
    /// <returns>Error code. 0 represents succeed.</returns>
    public override int TryReadArrayElementRaw(
      IArrayInstance arrayInstance,
      int[] indices,
      Memory<byte> destination,
      out DateTimeOffset? timeStamp)
    {
      if (arrayInstance == null)
        throw new ArgumentNullException(nameof (arrayInstance));
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (!(((IInstance) arrayInstance).DataType is IArrayType dataType))
        throw new ArgumentException("Type of array not found!", nameof (arrayInstance));
      ValueAccessMode valueAccessMode = this.calcAccessMethod((ISymbol) arrayInstance);
      timeStamp = new DateTimeOffset?(DateTimeOffset.MinValue);
      AdsErrorCode adsErrorCode;
      if (indices != null && indices.Length != 0)
      {
        IDataType elementType = dataType.ElementType;
        ArrayType.CheckIndices(indices, dataType, false);
        int elementOffset = ArrayType.GetElementOffset(indices, dataType);
        int read = 0;
        if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
        {
          IAdsSymbol iadsSymbol = (IAdsSymbol) Symbol.Unwrap((ISymbol) arrayInstance);
          adsErrorCode = ((IAdsReadWrite) this.Connection).TryRead(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset + (uint) elementOffset, destination, ref read);
        }
        else
          adsErrorCode = this.TryReadSymbolic((ISymbol) arrayInstance, destination, out read);
      }
      else
      {
        destination = (Memory<byte>) new byte[((IDataType) dataType).GetValueMarshalSize()];
        int read = 0;
        if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
        {
          IAdsSymbol iadsSymbol = (IAdsSymbol) Symbol.Unwrap((ISymbol) arrayInstance);
          adsErrorCode = ((IAdsReadWrite) this.Connection).TryRead(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, destination, ref read);
        }
        else
          adsErrorCode = this.TryReadSymbolic((ISymbol) arrayInstance, destination, out read);
      }
      if (adsErrorCode == null)
        timeStamp = new DateTimeOffset?(DateTimeOffset.Now);
      else
        destination = (Memory<byte>) (byte[]) null;
      return (int) adsErrorCode;
    }

    /// <summary>
    /// Reads the array element value as bytes asynchronously.
    /// </summary>
    /// <param name="arrayInstance">The array instance.</param>
    /// <param name="indices">The indices, which specify the element to read.</param>
    /// <param name="destination">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadArrayElementRaw' operation. The <see cref="T:TwinCAT.ValueAccess.ResultReadRawAccess" /> result contains the
    /// (<see cref="P:TwinCAT.ValueAccess.ResultReadValueAccess`1.Value" />) and the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" /> after execution.</returns>
    public override async Task<ResultReadRawAccess> ReadArrayElementRawAsync(
      IArrayInstance arrayInstance,
      int[] indices,
      Memory<byte> destination,
      CancellationToken cancel)
    {
      AdsValueAccessor adsValueAccessor = this;
      if (arrayInstance == null)
        throw new ArgumentNullException(nameof (arrayInstance));
      ResultReadRawAccess empty = ResultReadRawAccess.Empty;
      if (!(((IInstance) arrayInstance).DataType is IArrayType dataType))
        throw new ArgumentException("DataType not found!", nameof (arrayInstance));
      ValueAccessMode valueAccessMode = adsValueAccessor.calcAccessMethod((ISymbol) arrayInstance);
      ResultReadRawAccess resultReadRawAccess;
      if (indices != null && indices.Length != 0)
      {
        IDataType elementType = dataType.ElementType;
        ArrayType.CheckIndices(indices, dataType, false);
        int elementOffset = ArrayType.GetElementOffset(indices, dataType);
        if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
        {
          IAdsSymbol iadsSymbol = (IAdsSymbol) Symbol.Unwrap((ISymbol) arrayInstance);
          ResultRead resultRead = await ((IAdsReadWrite) adsValueAccessor.Connection).ReadAsync(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset + (uint) elementOffset, destination, cancel).ConfigureAwait(false);
          resultReadRawAccess = !((ResultAds) resultRead).Succeeded ? new ResultReadRawAccess((Memory<byte>) (byte[]) null, (int) ((ResultAds) resultRead).ErrorCode, DateTimeOffset.MinValue, ((ResultAds) resultRead).InvokeId) : new ResultReadRawAccess(destination, (int) ((ResultAds) resultRead).ErrorCode, DateTimeOffset.Now, ((ResultAds) resultRead).InvokeId);
        }
        else
          resultReadRawAccess = await adsValueAccessor.ReadRawAsync((ISymbol) arrayInstance, destination, cancel).ConfigureAwait(false);
      }
      else
      {
        destination = (Memory<byte>) new byte[((IDataType) dataType).GetValueMarshalSize()];
        if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
        {
          IAdsSymbol iadsSymbol = (IAdsSymbol) Symbol.Unwrap((ISymbol) arrayInstance);
          ResultRead resultRead = await ((IAdsReadWrite) adsValueAccessor.Connection).ReadAsync(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, destination, cancel).ConfigureAwait(false);
          resultReadRawAccess = !((ResultAds) resultRead).Succeeded ? new ResultReadRawAccess((Memory<byte>) (byte[]) null, (int) ((ResultAds) resultRead).ErrorCode, DateTimeOffset.MinValue, ((ResultAds) resultRead).InvokeId) : new ResultReadRawAccess(destination, (int) ((ResultAds) resultRead).ErrorCode, DateTimeOffset.Now, ((ResultAds) resultRead).InvokeId);
        }
        else
          resultReadRawAccess = await adsValueAccessor.ReadRawAsync((ISymbol) arrayInstance, destination, cancel).ConfigureAwait(false);
      }
      return resultReadRawAccess;
    }

    /// <summary>
    /// Writes the symbol value from source memory location to the ADS Device.
    /// </summary>
    /// <param name="symbol">The symbol instance.</param>
    /// <param name="value">The source memory location.</param>
    /// <param name="timeStamp">The write timestamp.</param>
    /// <returns>Error code. 0 represents succeed.</returns>
    public override int TryWriteRaw(
      ISymbol symbol,
      ReadOnlyMemory<byte> value,
      out DateTimeOffset? timeStamp)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      IAdsSymbol symbol1 = (IAdsSymbol) Symbol.Unwrap(symbol);
      ValueAccessMode valueAccessMode = this.calcAccessMethod((ISymbol) symbol1);
      int byteSize = ((IBitSize) symbol1).ByteSize;
      timeStamp = new DateTimeOffset?(DateTimeOffset.MinValue);
      if (((IProcessImageAddress) symbol1).IsVirtual)
        return 1796;
      AdsErrorCode adsErrorCode;
      if (value.Length > byteSize)
      {
        adsErrorCode = (AdsErrorCode) 1797;
      }
      else
      {
        using (new AdsConnectionRestore((AdsValueAccessorBase) this))
        {
          adsErrorCode = valueAccessMode != ValueAccessMode.IndexGroupOffset ? this.TryWriteSymbolic((ISymbol) symbol1, value) : ((IAdsReadWrite) this.Connection).TryWrite(((IProcessImageAddress) symbol1).IndexGroup, ((IProcessImageAddress) symbol1).IndexOffset, value);
          if (adsErrorCode == null)
            timeStamp = new DateTimeOffset?(DateTimeOffset.Now);
        }
      }
      return (int) adsErrorCode;
    }

    /// <summary>
    /// Writes the symbol value asynchronously from source memory location to the ADS Device.
    /// </summary>
    /// <param name="symbol">The symbol instance.</param>
    /// <param name="sourceData">The source value from memory location.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'WriteRaw' operation. The <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" /> result contains the
    /// the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" /> after execution.</returns>
    public override async Task<ResultWriteAccess> WriteRawAsync(
      ISymbol symbol,
      ReadOnlyMemory<byte> sourceData,
      CancellationToken cancel)
    {
      AdsValueAccessor accessor = this;
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (accessor.Connection == null)
        throw new AdsException("Connection not established!");
      ResultWriteAccess resultWriteAccess = ResultWriteAccess.Empty;
      IAdsSymbol symbol1 = (IAdsSymbol) Symbol.Unwrap(symbol);
      ValueAccessMode valueAccessMode = accessor.calcAccessMethod((ISymbol) symbol1);
      int byteSize = ((IBitSize) symbol1).ByteSize;
      if (((IProcessImageAddress) symbol1).IsVirtual)
        return new ResultWriteAccess(1796, DateTimeOffset.MinValue, 0U);
      if (sourceData.Length > byteSize)
      {
        resultWriteAccess = new ResultWriteAccess(1797, DateTimeOffset.MinValue, 0U);
      }
      else
      {
        using (new AdsConnectionRestore((AdsValueAccessorBase) accessor))
        {
          if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
          {
            ResultAds resultAds = (ResultAds) await ((IAdsReadWrite) accessor.Connection).WriteAsync(((IProcessImageAddress) symbol1).IndexGroup, ((IProcessImageAddress) symbol1).IndexOffset, sourceData, cancel).ConfigureAwait(false);
            resultWriteAccess = new ResultWriteAccess((int) resultAds.ErrorCode, DateTimeOffset.Now, resultAds.InvokeId);
          }
          else
            resultWriteAccess = await accessor.WriteSymbolicAsync((ISymbol) symbol1, sourceData, cancel).ConfigureAwait(false);
        }
      }
      return resultWriteAccess;
    }

    /// <summary>
    /// Writes an array element value from raw memory asynchronously to the ADS Device.
    /// </summary>
    /// <param name="arrayInstance">The array instance.</param>
    /// <param name="indices">The indices of the array element.</param>
    /// <param name="sourceData">The data to write to the ADS Device./&gt;.</param>
    /// <param name="timeStamp">Write time / timestamp</param>
    /// <returns>Error code. 0 represents succeed.</returns>
    public override int TryWriteArrayElementRaw(
      IArrayInstance arrayInstance,
      int[] indices,
      ReadOnlyMemory<byte> sourceData,
      out DateTimeOffset? timeStamp)
    {
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      if (arrayInstance == null)
        throw new ArgumentNullException(nameof (arrayInstance));
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      IArrayType iarrayType = !(((IInstance) arrayInstance).DataType is IResolvableType dataType) ? ((IInstance) arrayInstance).DataType as IArrayType : dataType.ResolveType((DataTypeResolveStrategy) 1) as IArrayType;
      if (iarrayType == null)
        throw new CannotResolveDataTypeException((IInstance) arrayInstance);
      ValueAccessMode valueAccessMode = this.calcAccessMethod((ISymbol) arrayInstance);
      timeStamp = new DateTimeOffset?(DateTimeOffset.MinValue);
      if (indices != null && indices.Length != 0)
      {
        IDataType elementType = iarrayType.ElementType;
        if (elementType == null)
          throw new CannotResolveDataTypeException((IInstance) arrayInstance);
        ArrayType.CheckIndices(indices, iarrayType, false);
        int elementOffset = ArrayType.GetElementOffset(indices, iarrayType);
        elementType.GetValueMarshalSize();
        if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
        {
          IAdsSymbol iadsSymbol = (IAdsSymbol) Symbol.Unwrap((ISymbol) arrayInstance);
          adsErrorCode = ((IAdsReadWrite) this.Connection).TryWrite(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset + (uint) elementOffset, sourceData);
        }
        else
          adsErrorCode = this.TryWriteSymbolic((ISymbol) arrayInstance, sourceData);
      }
      else
      {
        int valueMarshalSize = ((ISymbol) arrayInstance).GetValueMarshalSize();
        IAdsSymbol iadsSymbol = (IAdsSymbol) Symbol.Unwrap((ISymbol) arrayInstance);
        if (sourceData.Length != valueMarshalSize)
          throw new ArgumentException("Value array size mismatch!", nameof (sourceData));
        if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
          adsErrorCode = ((IAdsReadWrite) this.Connection).TryWrite(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, sourceData);
      }
      if (adsErrorCode != null)
        timeStamp = new DateTimeOffset?(DateTimeOffset.Now);
      return (int) adsErrorCode;
    }

    /// <summary>
    /// Writes an array element value from raw memory asynchronously to the ADS Device.
    /// </summary>
    /// <param name="arrayInstance">The array instance.</param>
    /// <param name="indices">The indices of the array element.</param>
    /// <param name="sourceData">The element value to write in raw memory format.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'WriteRaw' operation. The <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" /> result contains the
    /// the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" /> after execution.</returns>
    public override async Task<ResultWriteAccess> WriteArrayElementRawAsync(
      IArrayInstance arrayInstance,
      int[] indices,
      ReadOnlyMemory<byte> sourceData,
      CancellationToken cancel)
    {
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      if (arrayInstance == null)
        throw new ArgumentNullException(nameof (arrayInstance));
      IArrayType iarrayType = !(((IInstance) arrayInstance).DataType is IResolvableType dataType) ? ((IInstance) arrayInstance).DataType as IArrayType : dataType.ResolveType((DataTypeResolveStrategy) 1) as IArrayType;
      if (iarrayType == null)
        throw new CannotResolveDataTypeException((IInstance) arrayInstance);
      ValueAccessMode valueAccessMode = this.calcAccessMethod((ISymbol) arrayInstance);
      DateTimeOffset minValue = DateTimeOffset.MinValue;
      ResultWriteAccess resultWriteAccess;
      if (indices != null && indices.Length != 0)
      {
        IDataType elementType = iarrayType.ElementType;
        if (elementType == null)
          throw new CannotResolveDataTypeException((IInstance) arrayInstance);
        ArrayType.CheckIndices(indices, iarrayType, false);
        int elementOffset = ArrayType.GetElementOffset(indices, iarrayType);
        elementType.GetValueMarshalSize();
        if (valueAccessMode == ValueAccessMode.IndexGroupOffset)
        {
          IAdsSymbol iadsSymbol = (IAdsSymbol) Symbol.Unwrap((ISymbol) arrayInstance);
          ResultWrite resultWrite = await ((IAdsReadWrite) this.Connection).WriteAsync(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset + (uint) elementOffset, sourceData, cancel).ConfigureAwait(false);
          resultWriteAccess = new ResultWriteAccess((int) ((ResultAds) resultWrite).ErrorCode, ((ResultAds) resultWrite).InvokeId);
        }
        else
          resultWriteAccess = await this.WriteSymbolicAsync((ISymbol) arrayInstance, sourceData, cancel).ConfigureAwait(false);
      }
      else
      {
        int valueMarshalSize = ((ISymbol) arrayInstance).GetValueMarshalSize();
        IAdsSymbol iadsSymbol = (IAdsSymbol) Symbol.Unwrap((ISymbol) arrayInstance);
        if (sourceData.Length != valueMarshalSize)
          throw new ArgumentException("Value array size mismatch!", nameof (sourceData));
        if (valueAccessMode != ValueAccessMode.IndexGroupOffset)
          throw new NotImplementedException();
        ResultWrite resultWrite = await ((IAdsReadWrite) this.Connection).WriteAsync(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, sourceData, cancel).ConfigureAwait(false);
        resultWriteAccess = new ResultWriteAccess((int) ((ResultAds) resultWrite).ErrorCode, ((ResultAds) resultWrite).InvokeId);
      }
      return resultWriteAccess;
    }

    /// <summary>Calculates the access method.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>ValueAccessMode.</returns>
    private ValueAccessMode calcAccessMethod(ISymbol symbol) => this._accessMethod == ValueAccessMode.IndexGroupOffsetPreferred && (((IInstance) symbol).IsReference || ((IInstance) symbol).IsStatic) ? ValueAccessMode.Symbolic : this.calcAccessMethodByAddress((IProcessImageAddress) symbol);

    /// <summary>
    /// Calculates the access method dependent on Symbol Type and <see cref="T:TwinCAT.Ads.ValueAccess.ValueAccessMode" /> setting.
    /// </summary>
    /// <param name="symbolAddress">The array symbol.</param>
    /// <returns>ValueAccessMethod.</returns>
    private ValueAccessMode calcAccessMethodByAddress(
      IProcessImageAddress symbolAddress)
    {
      ValueAccessMode accessMethod = this._accessMethod;
      switch (this._accessMethod)
      {
        case ValueAccessMode.IndexGroupOffset:
          return ValueAccessMode.IndexGroupOffset;
        case ValueAccessMode.Symbolic:
          return ValueAccessMode.Symbolic;
        case ValueAccessMode.IndexGroupOffsetPreferred:
          switch ((IndexGroupSymbolAccess) (int) symbolAddress.IndexGroup - 61460)
          {
            case 0:
            case 2:
            case 5:
            case 6:
            case 7:
              return ValueAccessMode.Symbolic;
            default:
              return ValueAccessMode.IndexGroupOffset;
          }
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' not supported", (object) this._accessMethod.ToString()));
      }
    }

    /// <summary>
    /// Registers a Notification on the <see cref="T:TwinCAT.TypeSystem.ISymbol" />.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">Type of Notification (Value, Raw or Both)</param>
    /// <param name="settings">The settings.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// symbol
    /// or
    /// settings
    /// </exception>
    /// <exclude />
    /// <remarks>Only one Notification is allowed on the symbol. On case of double announcement, we set the Notification parameters
    /// to the higher priority.</remarks>
    public override void OnRegisterNotification(
      ISymbol symbol,
      SymbolNotificationTypes type,
      INotificationSettings settings)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (settings == null)
        throw new ArgumentNullException(nameof (settings));
      NotificationSettings settings1 = (NotificationSettings) null;
      if (this._notificationTable.TryGetRegisteredNotificationSettings(symbol, out settings1))
      {
        if (settings1.CompareTo(settings) < 0)
        {
          this.UnregisterNotification(symbol, type);
          this.RegisterNotification(symbol, type, (NotificationSettings) settings);
        }
        else
          this.RegisterNotification(symbol, type, settings1);
      }
      else
        this.RegisterNotification(symbol, type, (NotificationSettings) settings);
    }

    /// <summary>
    /// Unregisters a Notification from the <see cref="T:TwinCAT.TypeSystem.ISymbol" />.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">Type of Notification (Value, Raw or Both)</param>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    /// <exclude />
    public override void OnUnregisterNotification(ISymbol symbol, SymbolNotificationTypes type)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      this.UnregisterNotification(symbol, type);
    }

    /// <summary>Resizes the notification stream.</summary>
    /// <param name="length">The requested length of the stream in bytes.</param>
    private void resizeNotificationStream(int length)
    {
      long length1 = this._notificationStream.Length;
      long num1 = Math.Max(length1, 1024L);
      if (length1 < (long) length)
      {
        while (num1 < (long) length)
          num1 *= 2L;
        object syncNotification = this._syncNotification;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncNotification, ref lockTaken);
          this._notificationStream.SetLength(num1);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncNotification);
        }
      }
      else
      {
        if (length1 <= 1024L || (long) length >= length1 * 2L)
          return;
        long num2 = length1;
        do
        {
          num2 /= 2L;
        }
        while (num2 > 1024L && (long) length < num2 * 2L);
        object syncNotification = this._syncNotification;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncNotification, ref lockTaken);
          this._notificationStream.SetLength(num2);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncNotification);
        }
      }
    }

    /// <summary>Registers the notification.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">The type.</param>
    /// <param name="settings">The settings.</param>
    /// <exception cref="T:System.ArgumentException">Symbol size exceeds 64K for notification!</exception>
    private void RegisterNotification(
      ISymbol symbol,
      SymbolNotificationTypes type,
      NotificationSettings settings)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      int valueMarshalSize = symbol.GetValueMarshalSize();
      int largestSymbolSize = this._notificationTable.GetLargestSymbolSize();
      this.resizeNotificationStream(Math.Max(valueMarshalSize, largestSymbolSize));
      if (!this._notificationTable.Contains(symbol))
      {
        IAdsSymbol symbol1 = (IAdsSymbol) Symbol.Unwrap(symbol);
        ValueAccessMode valueAccessMode = this.calcAccessMethod((ISymbol) symbol1);
        using (new AdsConnectionRestore((AdsValueAccessorBase) this))
        {
          uint handle;
          if (valueAccessMode != ValueAccessMode.IndexGroupOffset)
          {
            if (valueAccessMode != ValueAccessMode.Symbolic)
              throw new NotSupportedException("Value access mode not supported!");
            handle = ((IAdsNotifications) this.Connection).AddDeviceNotification(((IInstance) symbol).InstancePath, valueMarshalSize, settings, (object) symbol);
          }
          else
            handle = ((IAdsNotifications) this.Connection).AddDeviceNotification(((IProcessImageAddress) symbol1).IndexGroup, ((IProcessImageAddress) symbol1).IndexOffset, valueMarshalSize, settings, (object) symbol);
          this._notificationTable.Add(symbol, handle, type, settings);
        }
      }
      else
        this._notificationTable.Update(symbol, type, settings);
    }

    /// <summary>Unregisters the notification.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if handle is removed, <c>false</c> otherwise.</returns>
    private bool UnregisterNotification(ISymbol symbol, SymbolNotificationTypes type)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this.Connection == null)
        throw new AdsException("Connection not established!");
      bool flag = false;
      uint handle = 0;
      if (this._notificationTable.TryGetNotificationHandle(symbol, out handle))
      {
        flag = this._notificationTable.Remove(symbol, type);
        if (flag)
        {
          using (new AdsConnectionRestore((AdsValueAccessorBase) this))
          {
            try
            {
              ((IAdsNotifications) this.Connection).DeleteDeviceNotification(handle);
            }
            catch (AdsErrorException ex)
            {
              AdsModule.Trace.TraceError((Exception) ex);
              flag = false;
            }
          }
        }
      }
      this.resizeNotificationStream(this._notificationTable.GetLargestSymbolSize());
      return flag;
    }

    /// <summary>Gets or sets the value access Method</summary>
    /// <value>The access method.</value>
    public ValueAccessMode AccessMethod
    {
      get => this._accessMethod;
      set => this._accessMethod = value;
    }

    /// <summary>
    /// Tries to read the value of the symbol and returns the value as instance of the specified type.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueType">The value type.</param>
    /// <param name="value">The value.</param>
    /// <param name="utcReadTime">The UTC read time.</param>
    /// <returns>AdsErrorCode.</returns>
    public override int TryReadAnyValue(
      ISymbol symbol,
      Type valueType,
      out object? value,
      out DateTimeOffset? utcReadTime)
    {
      byte[] numArray = new byte[((IBitSize) symbol).ByteSize];
      AdsErrorCode adsErrorCode = (AdsErrorCode) this.TryReadRaw(symbol, numArray.AsMemory<byte>(), out utcReadTime);
      value = (object) null;
      if (adsErrorCode == null)
        this._converter.Unmarshal((IAttributedInstance) symbol, (ReadOnlySpan<byte>) numArray, valueType, out value);
      return (int) adsErrorCode;
    }

    public override async Task<ResultReadValueAccess> ReadAnyValueAsync(
      ISymbol symbol,
      Type valueType,
      CancellationToken cancel)
    {
      AdsValueAccessor adsValueAccessor = this;
      byte[] bValue = new byte[((IBitSize) symbol).ByteSize];
      ResultReadRawAccess resultReadRawAccess = await adsValueAccessor.ReadRawAsync(symbol, bValue.AsMemory<byte>(), cancel).ConfigureAwait(false);
      if (!((ResultAccess) resultReadRawAccess).Succeeded)
        return new ResultReadValueAccess(((ResultAccess) resultReadRawAccess).ErrorCode, ((ResultAccess) resultReadRawAccess).InvokeId);
      object obj;
      adsValueAccessor._converter.Unmarshal((IAttributedInstance) symbol, (ReadOnlySpan<byte>) bValue, valueType, out obj);
      return new ResultReadValueAccess(obj, ((ResultAccess) resultReadRawAccess).ErrorCode, ((ResultAccess) resultReadRawAccess).DateTime, ((ResultAccess) resultReadRawAccess).InvokeId);
    }

    /// <summary>
    /// Tries to read the value of the symbol and updates the referenced value object with that data
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueObject">The value object.</param>
    /// <param name="utcReadTime">The UTC read time.</param>
    public override int TryUpdateAnyValue(
      ISymbol symbol,
      ref object valueObject,
      out DateTimeOffset? utcReadTime)
    {
      byte[] numArray = new byte[((IBitSize) symbol).ByteSize];
      AdsErrorCode adsErrorCode = (AdsErrorCode) this.TryReadRaw(symbol, numArray.AsMemory<byte>(), out utcReadTime);
      if (adsErrorCode == null)
        this._converter.InitializeInstanceValue(symbol, ref valueObject, (ReadOnlySpan<byte>) numArray);
      return (int) adsErrorCode;
    }

    public override async Task<ResultReadValueAccess> UpdateAnyValueAsync(
      ISymbol symbol,
      object valueObject,
      CancellationToken cancel)
    {
      AdsValueAccessor adsValueAccessor = this;
      byte[] bValue = new byte[((IBitSize) symbol).ByteSize];
      ResultReadRawAccess resultReadRawAccess = await adsValueAccessor.ReadRawAsync(symbol, bValue.AsMemory<byte>(), cancel).ConfigureAwait(false);
      if (((ResultAccess) resultReadRawAccess).Succeeded)
        adsValueAccessor._converter.InitializeInstanceValue(symbol, ref valueObject, (ReadOnlySpan<byte>) bValue);
      ResultReadValueAccess resultReadValueAccess = new ResultReadValueAccess(valueObject, ((ResultAccess) resultReadRawAccess).ErrorCode, ((ResultAccess) resultReadRawAccess).InvokeId);
      bValue = (byte[]) null;
      return resultReadValueAccess;
    }

    /// <summary>
    /// Tries to write the data within the value object as the symbol value.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueObject">The value object.</param>
    /// <param name="utcWriteTime">The UTC write time.</param>
    /// <returns>AdsErrorCode.</returns>
    public override int TryWriteAnyValue(
      ISymbol symbol,
      object valueObject,
      out DateTimeOffset? utcWriteTime)
    {
      byte[] numArray = this._converter.Marshal((IAttributedInstance) symbol, valueObject);
      return (int) (AdsErrorCode) ((IAccessorRawValue) this).TryWriteRaw(symbol, (ReadOnlyMemory<byte>) numArray, ref utcWriteTime);
    }

    public override async Task<ResultWriteAccess> WriteAnyValueAsync(
      ISymbol symbol,
      object valueObject,
      CancellationToken cancel)
    {
      AdsValueAccessor adsValueAccessor = this;
      byte[] numArray = adsValueAccessor._converter.Marshal((IAttributedInstance) symbol, valueObject);
      return await ((IAccessorRawValue) adsValueAccessor).WriteRawAsync(symbol, (ReadOnlyMemory<byte>) numArray, cancel).ConfigureAwait(false);
    }
  }
}
