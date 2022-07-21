// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.SymbolInfoTable
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.SumCommand;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using TwinCAT.TypeSystem.Generic;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Internal Symbol Info table.</summary>
  /// <exclude />
  internal class SymbolInfoTable : AdsSymbolProvider, ISymbolInfoTable, IDisposable
  {
    private SymbolicAnyTypeMarshaler _marshaller;
    private bool _symbolVersionChangedSupported = true;
    private SymbolUploadInfo _symbolInfo;
    private StringMarshaler _symbolMarshaler;
    private IAccessorValue _accessor;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.SymbolInfoTable" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="accessor">The accessor.</param>
    /// <param name="uploadInfo">The upload information.</param>
    /// <exception cref="T:System.ArgumentNullException">accessor</exception>
    /// <exception cref="T:System.ArgumentNullException">adsClient
    /// or
    /// symbolEncoding</exception>
    internal SymbolInfoTable(
      IAdsConnection connection,
      IAccessorValue accessor,
      SymbolUploadInfo uploadInfo)
      : base(connection, ((object) connection.Address).ToString())
    {
      if (accessor == null)
        throw new ArgumentNullException(nameof (accessor));
      this._symbolInfo = uploadInfo;
      this._symbolMarshaler = new StringMarshaler(uploadInfo.StringEncoding, (StringConvertMode) 2);
      ISymbolFactory isymbolFactory = (ISymbolFactory) new SymbolFactory(true);
      AdsOnDemandBinder binder = new AdsOnDemandBinder(connection.Address, (IInternalSymbolProvider) this, isymbolFactory, false);
      binder.SetPlatformPointerSize(uploadInfo.TargetPointerSize);
      this.FactoryServices = (ISymbolFactoryServices) new SymbolFactoryValueServices((IBinder) binder, isymbolFactory, (IAccessorRawValue) accessor, ((IConnection) connection).Session);
      isymbolFactory.Initialize(this.FactoryServices);
      this._accessor = accessor;
      this._marshaller = new SymbolicAnyTypeMarshaler();
      bool containsBaseTypes = this._symbolInfo.ContainsBaseTypes;
      AdsModule.Trace.TraceInformation("BaseTypes in DataType Stream: {0}", new object[1]
      {
        (object) containsBaseTypes
      });
      this.SetSymbols(new SymbolCollection<ISymbol>((InstanceCollectionMode) 1));
      try
      {
        ((IAdsSymbolChangedProvider) this.Connection).AdsSymbolVersionChanged += new EventHandler<AdsSymbolVersionChangedEventArgs>(this.Connection_AdsSymbolVersionChanged);
      }
      catch (AdsErrorException ex)
      {
        if (ex.ErrorCode != 1793)
          throw ex;
        this._symbolVersionChangedSupported = false;
      }
    }

    protected override void Init()
    {
      bool containsBaseTypes = this._symbolInfo.ContainsBaseTypes;
      AdsModule.Trace.TraceInformation("BaseTypes in DataType Stream: {0}", new object[1]
      {
        (object) containsBaseTypes
      });
      base.Init();
      this.SetSymbols(new SymbolCollection<ISymbol>((InstanceCollectionMode) 1));
    }

    ~SymbolInfoTable() => this.Dispose(false);

    /// <summary>Disposes this instance.</summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (this._disposed || !disposing)
        return;
      ((IAdsSymbolChangedProvider) this.Connection).AdsSymbolVersionChanged -= new EventHandler<AdsSymbolVersionChangedEventArgs>(this.Connection_AdsSymbolVersionChanged);
      this.CleanupCache();
      this._disposed = true;
    }

    /// <summary>
    /// Cleanup the <see cref="T:TwinCAT.Ads.Internal.SymbolInfoTable" />
    /// </summary>
    /// <remarks>
    /// Because the <see cref="T:TwinCAT.Ads.Internal.SymbolInfoTable" /> holds 'unmanaged' resources in form
    /// of Symbol handles that must be unregistered, the Cleanup is called by the
    /// dispose method.
    /// </remarks>
    public void CleanupCache()
    {
      int chunkSize = 100;
      List<uint> theList = new List<uint>();
      object syncObject = this.syncObject;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(syncObject, ref lockTaken);
        foreach (IAdsSymbol symbol in (IEnumerable<ISymbol>) this.Symbols)
        {
          if (((IProcessImageAddress) symbol).IndexGroup == 61445U)
            theList.Add(((IProcessImageAddress) symbol).IndexOffset);
        }
        this.Reset();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(syncObject);
      }
      foreach (List<uint> uintList in theList.Chunk<uint>(chunkSize))
      {
        SumReleaseHandles sumReleaseHandles = new SumReleaseHandles(this.Connection, uintList.ToArray());
        sumReleaseHandles.ReleaseHandles();
        AdsErrorCode result = sumReleaseHandles.Result;
      }
      this.Init();
    }

    /// <summary>Writes the symbol.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultWrite&gt;.</returns>
    /// <exception cref="T:System.NotSupportedException">Type of symbol not supported
    /// or
    /// Type of symbol not supported</exception>
    public async Task<ResultWrite> WriteValueAsync(
      string name,
      object value,
      CancellationToken cancel)
    {
      SymbolInfoTable symbolInfoTable = this;
      ResultWrite ret = ResultWrite.Empty;
      // ISSUE: explicit non-virtual call
      ResultValue<IAdsSymbol> resultValue1 = await __nonvirtual (symbolInfoTable.ReadSymbolAsync(name, true, cancel)).ConfigureAwait(false);
      ((ResultAds) ret).SetError(((ResultAds) resultValue1).ErrorCode);
      if (((ResultAds) ret).Succeeded)
      {
        IAdsSymbol adsSymbol = resultValue1.Value;
        if (((IInstance) adsSymbol).DataType == null)
        {
          // ISSUE: explicit non-virtual call
          ResultValue<IDataType> resultValue2 = await __nonvirtual (symbolInfoTable.ReadTypeAsync(((IInstance) adsSymbol).TypeName, true, cancel)).ConfigureAwait(false);
          IDataType idataType = ((ResultAds) resultValue2).Succeeded ? resultValue2.Value : throw new CannotResolveDataTypeException(((IInstance) adsSymbol).TypeName);
        }
        ConfiguredTaskAwaitable<ResultHandle> configuredTaskAwaitable;
        if (((IProcessImageAddress) adsSymbol).IndexGroup != 61445U)
        {
          configuredTaskAwaitable = symbolInfoTable.readSymbolHandleAsync(((IInstance) adsSymbol).InstancePath, cancel).ConfigureAwait(false);
          ResultHandle resultHandle = await configuredTaskAwaitable;
          ((ResultAds) ret).SetError(((ResultAds) resultHandle).ErrorCode);
          if (((ResultAds) ret).Succeeded)
            ((IAdsSymbolInternal) adsSymbol).SetAddress(61445U, resultHandle.Handle);
        }
        if (((ResultAds) ret).Succeeded)
        {
          if (((IProcessImageAddress) adsSymbol).IndexGroup != 61445U)
          {
            configuredTaskAwaitable = ((IAdsHandle) symbolInfoTable.Connection).CreateVariableHandleAsync(name, cancel).ConfigureAwait(false);
            ResultHandle resultHandle = await configuredTaskAwaitable;
            ((ResultAds) ret).SetError(((ResultAds) resultHandle).ErrorCode);
            if (((ResultAds) ret).Succeeded)
              ((IAdsSymbolInternal) adsSymbol).SetAddress(61445U, resultHandle.Handle);
          }
          if (((ResultAds) ret).Succeeded)
          {
            try
            {
              byte[] data = new byte[((ISymbol) adsSymbol).GetValueMarshalSize()];
              bool flag = await symbolInfoTable.onDemandResolveAsync(adsSymbol, cancel).ConfigureAwait(false);
              int marshalledBytes = 0;
              if (symbolInfoTable._marshaller.TryMarshal((IAttributedInstance) adsSymbol, value, data.AsSpan<byte>(), out marshalledBytes))
              {
                bool bRetry = true;
                for (int i = 0; i < 2 & bRetry; ++i)
                {
                  bRetry = false;
                  ((ResultAds) ret).SetError((AdsErrorCode) ((ResultAccess) await ((IAccessorRawValue) symbolInfoTable._accessor).WriteRawAsync((ISymbol) adsSymbol, (ReadOnlyMemory<byte>) data.AsMemory<byte>(), cancel).ConfigureAwait(false)).ErrorCode);
                  AdsErrorCode errorCode = ((ResultAds) ret).ErrorCode;
                  if (errorCode != null && (errorCode == 1795 || errorCode - 1808 <= 1) && ((IProcessImageAddress) adsSymbol).IndexGroup == 61445U)
                  {
                    byte[] bytes = BitConverter.GetBytes(((IProcessImageAddress) adsSymbol).IndexOffset);
                    ((ResultAds) ret).SetError(((ResultAds) await ((IAdsReadWrite) symbolInfoTable.Connection).WriteAsync(61446U, 0U, (ReadOnlyMemory<byte>) bytes.AsMemory<byte>(), cancel).ConfigureAwait(false)).ErrorCode);
                    configuredTaskAwaitable = ((IAdsHandle) symbolInfoTable.Connection).CreateVariableHandleAsync(name, cancel).ConfigureAwait(false);
                    ResultHandle resultHandle = await configuredTaskAwaitable;
                    ((ResultAds) ret).SetError(((ResultAds) resultHandle).ErrorCode);
                    if (((ResultAds) ret).Succeeded)
                    {
                      ((IAdsSymbolInternal) adsSymbol).SetAddress(61445U, resultHandle.Handle);
                      bRetry = true;
                    }
                  }
                }
              }
              else
                ((ResultAds) ret).SetError((AdsErrorCode) 1797);
              data = (byte[]) null;
            }
            catch (AdsErrorException ex)
            {
              ((ResultAds) ret).SetError(ex.ErrorCode);
            }
          }
        }
        adsSymbol = (IAdsSymbol) null;
      }
      ResultWrite resultWrite = ret;
      ret = (ResultWrite) null;
      return resultWrite;
    }

    public AdsErrorCode TryReadType(string name, bool lookup, out IDataType? type)
    {
      type = !string.IsNullOrEmpty(name) ? this.readTypeFirst(name, lookup) : throw new ArgumentNullException(nameof (name));
      AdsErrorCode adsErrorCode;
      if (type == null)
      {
        byte[] numArray = new byte[this._symbolMarshaler.MarshalSize(name)];
        this._symbolMarshaler.Marshal(name, (Span<byte>) numArray);
        byte[] span = new byte[(int) ushort.MaxValue];
        int num = 0;
        adsErrorCode = ((IAdsReadWrite) this.Connection).TryReadWrite(61457U, 0U, (Memory<byte>) span, (ReadOnlyMemory<byte>) numArray, ref num);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
        {
          AdsDataTypeEntry entry = new AdsDataTypeEntry(true, (IStringMarshaler) this._symbolMarshaler, (ReadOnlySpan<byte>) span);
          type = (IDataType) AdsSymbolParser.ParseType(entry, this._symbolInfo.StringEncoding, this.FactoryServices);
          object syncObject = this.syncObject;
          bool lockTaken = false;
          try
          {
            Monitor.Enter(syncObject, ref lockTaken);
            this.FactoryServices.Binder.RegisterType(type);
            this.FactoryServices.Binder.OnTypeGenerated(type);
          }
          finally
          {
            if (lockTaken)
              Monitor.Exit(syncObject);
          }
        }
      }
      else
        adsErrorCode = (AdsErrorCode) 0;
      if (type == null)
        this.OnResolveError(name);
      return adsErrorCode;
    }

    private IDataType? readTypeFirst(string name, bool lookup)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      IDataType idataType1 = (IDataType) null;
      if (lookup)
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          if (this.DataTypes.TryGetType(name, ref idataType1))
          {
            if (((IBitSize) idataType1).Size <= 0)
            {
              IDataType idataType2 = ((IResolvableType) idataType1).ResolveType((DataTypeResolveStrategy) 1);
              if (idataType2.Category == 13)
              {
                int platformPointerSize = ((IDataTypeResolver) this.FactoryServices.Binder).PlatformPointerSize;
                switch (platformPointerSize)
                {
                  case 4:
                  case 8:
                    Type managedType = typeof (ulong);
                    if (platformPointerSize == 4)
                      managedType = typeof (uint);
                    if (((IBitSize) idataType2).Size <= 0)
                      ((DataType) idataType2).SetSize(platformPointerSize, managedType);
                    ((DataType) idataType1).SetSize(platformPointerSize, managedType);
                    break;
                }
              }
            }
          }
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
      if (idataType1 == null)
      {
        int length = 0;
        Encoding encoding = (Encoding) null;
        string referencedType = (string) null;
        string baseType = (string) null;
        DimensionCollection dims = (DimensionCollection) null;
        if (DataTypeStringParser.TryParseString(name, out length, out encoding))
          idataType1 = encoding != Encoding.Unicode ? (IDataType) new StringType(length, encoding) : (IDataType) new WStringType(length);
        else if (DataTypeStringParser.TryParsePointer(name, out referencedType))
          idataType1 = (IDataType) new PointerType(referencedType, this.Binder.PlatformPointerSize);
        else if (DataTypeStringParser.TryParseArray(name, out dims, out baseType))
        {
          IDataType elementType = this.readTypeFirst(baseType, lookup);
          if (elementType != null)
            idataType1 = (IDataType) new ArrayType(name, elementType, dims, (AdsDataTypeFlags) 1);
        }
        if (idataType1 != null)
        {
          object syncObject = this.syncObject;
          bool lockTaken = false;
          try
          {
            Monitor.Enter(syncObject, ref lockTaken);
            this.FactoryServices.Binder.RegisterType(idataType1);
            this.FactoryServices.Binder.OnTypeGenerated(idataType1);
          }
          finally
          {
            if (lockTaken)
              Monitor.Exit(syncObject);
          }
        }
      }
      return idataType1;
    }

    /// <summary>
    /// Reads a <see cref="T:TwinCAT.TypeSystem.IDataType" /> asynchronously.
    /// </summary>
    /// <param name="name">The type name.</param>
    /// <param name="lookup">if set to true, the operation first looks in the cached DataTypes table, otherwise an ADS GetDataTypeByName is forced./&gt;.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadType' operation. The <see cref="T:TwinCAT.Ads.ResultValue`1" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultValue`1.Value" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <seealso cref="M:TwinCAT.Ads.Internal.SymbolInfoTable.TryReadType(System.String,System.Boolean,TwinCAT.TypeSystem.IDataType@)" />
    public async Task<ResultValue<IDataType>> ReadTypeAsync(
      string name,
      bool lookup,
      CancellationToken cancel)
    {
      SymbolInfoTable symbolInfoTable = this;
      IDataType type = symbolInfoTable.readTypeFirst(name, lookup);
      ResultValue<IDataType> empty1 = ResultValue<IDataType>.Empty;
      ResultValue<IDataType> resultValue1;
      if (type == null)
      {
        ResultReadWrite empty2 = ResultReadWrite.Empty;
        byte[] numArray = new byte[symbolInfoTable._symbolMarshaler.MarshalSize(name)];
        symbolInfoTable._symbolMarshaler.Marshal(name, (Span<byte>) numArray);
        byte[] rdBuffer = new byte[(int) ushort.MaxValue];
        ResultReadWrite resultReadWrite = await ((IAdsReadWrite) symbolInfoTable.Connection).ReadWriteAsync(61457U, 0U, (Memory<byte>) rdBuffer, (ReadOnlyMemory<byte>) numArray, cancel).ConfigureAwait(false);
        if (((ResultAds) resultReadWrite).ErrorCode == null)
        {
          // ISSUE: explicit non-virtual call
          type = (IDataType) AdsSymbolParser.ParseType(new AdsDataTypeEntry(true, (IStringMarshaler) symbolInfoTable._symbolMarshaler, (ReadOnlySpan<byte>) rdBuffer), symbolInfoTable._symbolInfo.StringEncoding, __nonvirtual (symbolInfoTable.FactoryServices));
          object syncObject = symbolInfoTable.syncObject;
          bool lockTaken = false;
          try
          {
            Monitor.Enter(syncObject, ref lockTaken);
            // ISSUE: explicit non-virtual call
            __nonvirtual (symbolInfoTable.FactoryServices).Binder.RegisterType(type);
            // ISSUE: explicit non-virtual call
            __nonvirtual (symbolInfoTable.FactoryServices).Binder.OnTypeGenerated(type);
          }
          finally
          {
            if (lockTaken)
              Monitor.Exit(syncObject);
          }
        }
        resultValue1 = new ResultValue<IDataType>(((ResultAds) resultReadWrite).ErrorCode, type);
        rdBuffer = (byte[]) null;
      }
      else
        resultValue1 = new ResultValue<IDataType>((AdsErrorCode) 0, type);
      if (type == null)
        symbolInfoTable.OnResolveError(name);
      ResultValue<IDataType> resultValue2 = resultValue1;
      type = (IDataType) null;
      return resultValue2;
    }

    /// <summary>Updates the symbol handle for Symbolic access</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <remarks></remarks>
    private async Task<ResultHandle> readSymbolHandleAsync(
      string symbolPath,
      CancellationToken cancel)
    {
      SymbolInfoTable symbolInfoTable = this;
      byte[] readBytes = new byte[4];
      byte[] array = new byte[symbolInfoTable._symbolMarshaler.MarshalSize(symbolPath)];
      symbolInfoTable._symbolMarshaler.Marshal(symbolPath, array.AsSpan<byte>());
      ResultReadWrite resultReadWrite = await ((IAdsReadWrite) symbolInfoTable.Connection).ReadWriteAsync(61443U, 0U, readBytes.AsMemory<byte>(), (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel).ConfigureAwait(false);
      ResultHandle resultHandle = !((ResultAds) resultReadWrite).Succeeded ? ResultHandle.CreateError(((ResultAds) resultReadWrite).ErrorCode) : ResultHandle.CreateSuccess(BitConverter.ToUInt32(readBytes, 0), ((ResultAds) resultReadWrite).InvokeId);
      readBytes = (byte[]) null;
      return resultHandle;
    }

    /// <summary>Updates the symbol handle for Symbolic access</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="handle">The handle.</param>
    /// <returns>AdsErrorCode.</returns>
    private AdsErrorCode readSymbolHandleSync(string symbolPath, out uint handle)
    {
      handle = 0U;
      byte[] array1 = new byte[4];
      byte[] array2 = new byte[this._symbolMarshaler.MarshalSize(symbolPath)];
      this._symbolMarshaler.Marshal(symbolPath, array2.AsSpan<byte>());
      int num;
      AdsErrorCode adsErrorCode = ((IAdsReadWrite) this.Connection).TryReadWrite(61443U, 0U, array1.AsMemory<byte>(), (ReadOnlyMemory<byte>) array2.AsMemory<byte>(), ref num);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
        handle = BitConverter.ToUInt32(array1, 0);
      return adsErrorCode;
    }

    /// <summary>Writes the symbol.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="T:System.NotSupportedException">
    /// Type of symbol not supported
    /// or
    /// Type of symbol not supported
    /// </exception>
    public AdsErrorCode TryWriteValue(string name, object value)
    {
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) 0;
      IAdsSymbol symbol = (IAdsSymbol) null;
      AdsErrorCode adsErrorCode2 = this.TryReadSymbol(name, true, out symbol);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
      {
        IDataType type = ((IInstance) symbol).DataType;
        if (type == null)
        {
          adsErrorCode2 = this.TryReadType(((IInstance) symbol).TypeName, true, out type);
          if (AdsErrorCodeExtensions.Failed(adsErrorCode2))
            throw new CannotResolveDataTypeException(((IInstance) symbol).TypeName);
        }
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
        {
          if (((IProcessImageAddress) symbol).IndexGroup != 61445U)
          {
            uint handle = 0;
            adsErrorCode2 = this.readSymbolHandleSync(((IInstance) symbol).InstancePath, out handle);
            if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
              ((IAdsSymbolInternal) symbol).SetAddress(61445U, handle);
          }
          if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
          {
            if (((IProcessImageAddress) symbol).IndexGroup != 61445U)
            {
              uint num = 0;
              adsErrorCode2 = ((IAdsHandle) this.Connection).TryCreateVariableHandle(name, ref num);
              if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
                ((IAdsSymbolInternal) symbol).SetAddress(61445U, num);
            }
            if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
            {
              try
              {
                byte[] array = new byte[((ISymbol) symbol).GetValueMarshalSize()];
                int marshalledBytes = 0;
                if (this._marshaller.TryMarshal((IAttributedInstance) symbol, value, array.AsSpan<byte>(), out marshalledBytes))
                {
                  bool flag = true;
                  for (int index = 0; index < 2 & flag; ++index)
                  {
                    flag = false;
                    DateTimeOffset? nullable;
                    adsErrorCode2 = (AdsErrorCode) ((IAccessorRawValue) this._accessor).TryWriteRaw((ISymbol) symbol, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), ref nullable);
                    if (adsErrorCode2 != null && (adsErrorCode2 == 1795 || adsErrorCode2 - 1808 <= 1) && ((IProcessImageAddress) symbol).IndexGroup == 61445U)
                    {
                      uint num = 0;
                      adsErrorCode1 = ((IAdsReadWrite) this.Connection).TryWrite(61446U, 0U, (ReadOnlyMemory<byte>) BitConverter.GetBytes(((IProcessImageAddress) symbol).IndexOffset).AsMemory<byte>());
                      adsErrorCode2 = ((IAdsHandle) this.Connection).TryCreateVariableHandle(name, ref num);
                      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
                      {
                        ((IAdsSymbolInternal) symbol).SetAddress(61445U, num);
                        flag = true;
                      }
                    }
                  }
                }
                else
                  adsErrorCode2 = (AdsErrorCode) 1797;
              }
              catch (AdsErrorException ex)
              {
                adsErrorCode2 = ex.ErrorCode;
              }
            }
          }
        }
      }
      return adsErrorCode2;
    }

    /// <summary>Reads the symbol.</summary>
    /// <param name="symbolPath">The Symbol path.</param>
    /// <param name="managedType">Managed type</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:System.NotSupportedException">Type of symbol not supported
    /// or
    /// Type of symbol not supported</exception>
    public AdsErrorCode TryReadValue(
      string symbolPath,
      Type managedType,
      out object? value)
    {
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) 0;
      value = (object) null;
      IAdsSymbol symbol = (IAdsSymbol) null;
      uint handle = 0;
      AdsErrorCode adsErrorCode2 = this.TryReadSymbol(symbolPath, true, out symbol);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
      {
        IDataType type = ((IInstance) symbol).DataType;
        if (type == null)
        {
          adsErrorCode2 = this.TryReadType(((IInstance) symbol).TypeName, true, out type);
          if (AdsErrorCodeExtensions.Failed(adsErrorCode2))
            throw new CannotResolveDataTypeException(((IInstance) symbol).TypeName);
        }
        if (((IProcessImageAddress) symbol).IndexGroup != 61445U)
        {
          adsErrorCode2 = this.readSymbolHandleSync(((IInstance) symbol).InstancePath, out handle);
          if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
            ((IAdsSymbolInternal) symbol).SetAddress(61445U, handle);
        }
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
        {
          byte[] array = new byte[((ISymbol) symbol).GetValueMarshalSize()];
          DateTimeOffset? nullable;
          adsErrorCode2 = (AdsErrorCode) ((IAccessorRawValue) this._accessor).TryReadRaw((ISymbol) symbol, array.AsMemory<byte>(), ref nullable);
          if ((adsErrorCode2 == 1795 || adsErrorCode2 - 1808 <= 1) && ((IProcessImageAddress) symbol).IndexGroup == 61445U)
          {
            adsErrorCode1 = ((IAdsHandle) this.Connection).TryDeleteVariableHandle(((IProcessImageAddress) symbol).IndexOffset);
            adsErrorCode2 = ((IAdsHandle) this.Connection).TryCreateVariableHandle(symbolPath, ref handle);
            if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
            {
              ((IAdsSymbolInternal) symbol).SetAddress(61445U, handle);
              adsErrorCode2 = (AdsErrorCode) ((IAccessorRawValue) this._accessor).TryReadRaw((ISymbol) symbol, array.AsMemory<byte>(), ref nullable);
            }
          }
          if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
            this._marshaller.Unmarshal((IAttributedInstance) symbol, (ReadOnlySpan<byte>) array.AsSpan<byte>(), managedType, out value);
        }
      }
      return adsErrorCode2;
    }

    /// <summary>Reads the symbol.</summary>
    /// <param name="symbolPath">The Symbol path.</param>
    /// <param name="managedType">Managed type</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:System.NotSupportedException">Type of symbol not supported
    /// or
    /// Type of symbol not supported</exception>
    public async Task<ResultAnyValue> ReadValueAsync(
      string symbolPath,
      Type managedType,
      CancellationToken cancel)
    {
      SymbolInfoTable symbolInfoTable = this;
      ResultAnyValue ret = ResultAnyValue.Empty;
      // ISSUE: explicit non-virtual call
      ResultValue<IAdsSymbol> resultValue1 = await __nonvirtual (symbolInfoTable.ReadSymbolAsync(symbolPath, true, cancel)).ConfigureAwait(false);
      ((ResultAds) ret).SetError(((ResultAds) resultValue1).ErrorCode);
      if (((ResultAds) ret).Succeeded)
      {
        IAdsSymbol adsSymbol = resultValue1.Value;
        if (((IInstance) adsSymbol).DataType == null)
        {
          // ISSUE: explicit non-virtual call
          ResultValue<IDataType> resultValue2 = await __nonvirtual (symbolInfoTable.ReadTypeAsync(((IInstance) adsSymbol).TypeName, true, cancel)).ConfigureAwait(false);
          IDataType idataType = ((ResultAds) resultValue2).Succeeded ? resultValue2.Value : throw new CannotResolveDataTypeException(((IInstance) adsSymbol).TypeName);
        }
        ConfiguredTaskAwaitable<ResultHandle> configuredTaskAwaitable;
        if (((IProcessImageAddress) adsSymbol).IndexGroup != 61445U)
        {
          configuredTaskAwaitable = symbolInfoTable.readSymbolHandleAsync(((IInstance) adsSymbol).InstancePath, cancel).ConfigureAwait(false);
          ResultHandle resultHandle = await configuredTaskAwaitable;
          ((ResultAds) ret).SetError(((ResultAds) resultHandle).ErrorCode);
          if (((ResultAds) resultHandle).Succeeded)
            ((IAdsSymbolInternal) adsSymbol).SetAddress(61445U, resultHandle.Handle);
        }
        if (((ResultAds) ret).Succeeded)
        {
          byte[] data = new byte[((ISymbol) adsSymbol).GetValueMarshalSize()];
          ResultReadRawAccess result = await ((IAccessorRawValue) symbolInfoTable._accessor).ReadRawAsync((ISymbol) adsSymbol, data.AsMemory<byte>(), cancel).ConfigureAwait(false);
          ((ResultAds) ret).SetError((AdsErrorCode) ((ResultAccess) result).ErrorCode);
          AdsErrorCode errorCode = ((ResultAds) ret).ErrorCode;
          if ((errorCode == 1795 || errorCode - 1808 <= 1) && ((IProcessImageAddress) adsSymbol).IndexGroup == 61445U)
          {
            ResultAds resultAds = await ((IAdsHandle) symbolInfoTable.Connection).DeleteVariableHandleAsync(((IProcessImageAddress) adsSymbol).IndexOffset, cancel).ConfigureAwait(false);
            configuredTaskAwaitable = ((IAdsHandle) symbolInfoTable.Connection).CreateVariableHandleAsync(symbolPath, cancel).ConfigureAwait(false);
            ResultHandle resultHandle = await configuredTaskAwaitable;
            ((ResultAds) ret).SetError(((ResultAds) resultHandle).ErrorCode);
            if (((ResultAds) ret).Succeeded)
            {
              ((IAdsSymbolInternal) adsSymbol).SetAddress(61445U, resultHandle.Handle);
              result = await ((IAccessorRawValue) symbolInfoTable._accessor).ReadRawAsync((ISymbol) adsSymbol, data.AsMemory<byte>(), cancel).ConfigureAwait(false);
              ((ResultAds) ret).SetError((AdsErrorCode) ((ResultAccess) result).ErrorCode);
            }
          }
          if (((ResultAds) ret).Succeeded && ((ResultAccess) result).Succeeded)
          {
            bool flag = await symbolInfoTable.onDemandResolveAsync(adsSymbol, cancel).ConfigureAwait(false);
            object obj;
            symbolInfoTable._marshaller.Unmarshal((IAttributedInstance) adsSymbol, (ReadOnlySpan<byte>) data.AsSpan<byte>(), managedType, out obj);
            ret = new ResultAnyValue((AdsErrorCode) 0, obj, ((ResultAds) ret).InvokeId);
          }
          data = (byte[]) null;
          result = (ResultReadRawAccess) null;
        }
        adsSymbol = (IAdsSymbol) null;
      }
      ResultAnyValue resultAnyValue = ret;
      ret = (ResultAnyValue) null;
      return resultAnyValue;
    }

    /// <summary>Resolve Type Binding asynchronously OnDemand.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <remarks>
    /// Tries to resolve the type (parses it if not found and adds it to the resolver if necessary).
    /// </remarks>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    private async Task<bool> onDemandResolveAsync(IAdsSymbol symbol, CancellationToken cancel)
    {
      SymbolInfoTable symbolInfoTable = this;
      return !(symbol is IBindable2 ibindable2) || ibindable2.IsBindingResolved(true) || await ibindable2.ResolveWithBinderAsync(true, (IBinder) symbolInfoTable.Binder, cancel).ConfigureAwait(false);
    }

    private AdsErrorCode TryOnDemandResolve(string name, out IDataType? type)
    {
      AdsErrorCode adsErrorCode = ((ISymbolInfoTable) this.Provider).TryReadType(name, true, ref type);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
      {
        DataTypeCategory category = type.Category;
      }
      return adsErrorCode;
    }

    /// <summary>Resolves the type asynchronous.</summary>
    /// <param name="name">The name.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultValue&lt;IDataType&gt;&gt;.</returns>
    private async Task<ResultValue<IDataType>> OnDemandResolveAsync(
      string name,
      CancellationToken cancel)
    {
      ResultValue<IDataType> resultValue = await ((ISymbolInfoTable) this.Provider).ReadTypeAsync(name, true, cancel).ConfigureAwait(false);
      if (((ResultAds) resultValue).Succeeded)
      {
        DataTypeCategory category = resultValue.Value.Category;
      }
      return resultValue;
    }

    /// <summary>Get Symbol</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="bLookup">if set to <c>true</c> then this method looks first in its internal cache, otherwise it directly does an ADS roundtrip.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>TcAdsSymbol.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">name</exception>
    public async Task<ResultValue<IAdsSymbol>> ReadSymbolAsync(
      string symbolPath,
      bool bLookup,
      CancellationToken cancel)
    {
      SymbolInfoTable symbolInfoTable = this;
      if (string.IsNullOrEmpty(symbolPath))
        throw new ArgumentOutOfRangeException(nameof (symbolPath));
      ResultValue<IAdsSymbol> result = ResultValue<IAdsSymbol>.Empty;
      if (bLookup)
      {
        ISymbol isymbol = (ISymbol) null;
        // ISSUE: reference to a compiler-generated method
        if (((IInstanceCollection<ISymbol>) symbolInfoTable.\u003C\u003En__0()).TryGetInstance(symbolPath, ref isymbol))
        {
          result = new ResultValue<IAdsSymbol>((AdsErrorCode) 0, (IAdsSymbol) isymbol);
          return result;
        }
      }
      byte[] numArray = new byte[symbolInfoTable._symbolMarshaler.MarshalSize(symbolPath)];
      symbolInfoTable._symbolMarshaler.Marshal(symbolPath, (Span<byte>) numArray);
      byte[] readBuffer = new byte[(int) ushort.MaxValue];
      ResultReadWrite resultReadWrite = await ((IAdsReadWrite) symbolInfoTable.Connection).ReadWriteAsync(61449U, 0U, (Memory<byte>) readBuffer, (ReadOnlyMemory<byte>) numArray, cancel).ConfigureAwait(false);
      if (AdsErrorCodeExtensions.Succeeded(((ResultAds) resultReadWrite).ErrorCode))
      {
        // ISSUE: explicit non-virtual call
        ResultValue<ISymbol> resultValue = await AdsSymbolParser.ParseSymbolAsync((ReadOnlySpan<byte>) readBuffer.AsSpan<byte>(), symbolInfoTable._symbolMarshaler, __nonvirtual (symbolInfoTable.FactoryServices), cancel).ConfigureAwait(false);
        result = new ResultValue<IAdsSymbol>(((ResultAds) resultValue).ErrorCode, (IAdsSymbol) resultValue.Value);
      }
      else
        ((ResultAds) result).SetError(((ResultAds) resultReadWrite).ErrorCode);
      return result;
    }

    public AdsErrorCode TryReadSymbol(
      string symbolPath,
      bool bLookup,
      out IAdsSymbol? symbol)
    {
      if (string.IsNullOrEmpty(symbolPath))
        throw new ArgumentOutOfRangeException(nameof (symbolPath));
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) 0;
      symbol = (IAdsSymbol) null;
      if (bLookup)
      {
        ISymbol isymbol = (ISymbol) null;
        if (((IInstanceCollection<ISymbol>) this.Symbols).TryGetInstance(symbolPath, ref isymbol))
        {
          symbol = (IAdsSymbol) isymbol;
          return adsErrorCode1;
        }
      }
      byte[] array1 = new byte[StringMarshaler.Default.MarshalSize(symbolPath)];
      StringMarshaler.Default.Marshal(symbolPath, array1.AsSpan<byte>());
      byte[] array2 = new byte[(int) ushort.MaxValue];
      int num = 0;
      AdsErrorCode adsErrorCode2 = ((IAdsReadWrite) this.Connection).TryReadWrite(61449U, 0U, array2.AsMemory<byte>(), (ReadOnlyMemory<byte>) array1.AsMemory<byte>(), ref num);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
      {
        ISymbol symbol1 = (ISymbol) null;
        if (AdsSymbolParser.ParseSymbol((ReadOnlySpan<byte>) array2.AsSpan<byte>(), this._symbolMarshaler, this.FactoryServices, out symbol1).valid)
          symbol = (IAdsSymbol) symbol1;
        else
          adsErrorCode2 = (AdsErrorCode) 1;
      }
      return adsErrorCode2;
    }

    private static void EnsureParametersValid(IRpcMethod method, object[]? inParameters)
    {
      int count1 = ((ICollection<IRpcMethodParameter>) method.InParameters).Count;
      int count2 = ((ICollection<IRpcMethodParameter>) method.OutParameters).Count;
      if (count1 > 0)
      {
        if (inParameters == null || inParameters.Length != count1)
          throw new ArgumentOutOfRangeException(nameof (inParameters), "Wrong number of In-Parameters!");
      }
      else if (inParameters != null && inParameters.Length != 0)
        throw new ArgumentOutOfRangeException(nameof (inParameters), "Wrong number of In-Parameters!");
    }

    public async Task<ResultRpcMethod> InvokeRpcMethodAsync(
      IRpcCallableInstance symbol,
      IRpcMethod rpcMethod,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      CancellationToken cancel)
    {
      SymbolInfoTable symbolInfoTable = this;
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (rpcMethod == null)
        throw new ArgumentNullException(nameof (rpcMethod));
      SymbolInfoTable.EnsureParametersValid(rpcMethod, inParameters);
      ResultRpcMethod result = ResultRpcMethod.Empty;
      bool flag;
      string symbolMethodPath = RpcInvokeParser.GetRpcMethodAccessPath(symbol, rpcMethod, ref flag);
      IDataType idataType1 = (IDataType) null;
      ResultValue<IDataType> resultValue1 = new ResultValue<IDataType>((AdsErrorCode) 0, (IDataType) null);
      if (!rpcMethod.IsVoid)
      {
        // ISSUE: explicit non-virtual call
        ResultValue<IDataType> resultValue2 = await __nonvirtual (symbolInfoTable.ReadTypeAsync(rpcMethod.ReturnType, true, cancel)).ConfigureAwait(false);
        ((ResultAds) result).SetError(((ResultAds) resultValue2).ErrorCode);
        if (((ResultAds) resultValue2).Succeeded)
          idataType1 = resultValue2.Value;
      }
      if (((ResultAds) result).Failed)
        return result;
      List<IDataType> parameterTypes = new List<IDataType>();
      foreach (RpcMethodParameter parameter in (IEnumerable<IRpcMethodParameter>) rpcMethod.Parameters)
      {
        RpcMethodParameter para = parameter;
        // ISSUE: explicit non-virtual call
        ResultValue<IDataType> resultValue3 = await __nonvirtual (symbolInfoTable.ReadTypeAsync(para.TypeName, true, cancel)).ConfigureAwait(false);
        ((ResultAds) result).SetError(((ResultAds) resultValue3).ErrorCode);
        if (((ResultAds) resultValue3).Failed)
          return result;
        IDataType idataType2 = resultValue3.Value;
        if (idataType2 == null)
        {
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 2);
          interpolatedStringHandler.AppendLiteral("The dataType '");
          interpolatedStringHandler.AppendFormatted(para.TypeName);
          interpolatedStringHandler.AppendLiteral("' of RPC parameter '");
          interpolatedStringHandler.AppendFormatted(para.Name);
          interpolatedStringHandler.AppendLiteral("' is not supported!");
          throw new DataTypeException(interpolatedStringHandler.ToStringAndClear(), para.TypeName);
        }
        parameterTypes.Add(idataType2);
        para = (RpcMethodParameter) null;
      }
      // ISSUE: explicit non-virtual call
      SymbolRpcMarshaler marshaller = new SymbolRpcMarshaler((IDataTypeResolver) __nonvirtual (symbolInfoTable.FactoryServices).Binder);
      try
      {
        object[] allValues = new object[((ICollection<IRpcMethodParameter>) rpcMethod.Parameters).Count];
        symbolInfoTable.SetRpcInParameterValues(rpcMethod, inParameters, allValues);
        int inMarshallingSize = marshaller.GetInMarshallingSize(rpcMethod, allValues);
        int readSize = marshaller.GetOutMarshallingSize(rpcMethod, outSpecifiers, retSpecifier, allValues);
        byte[] writeBuffer = new byte[inMarshallingSize];
        byte[] readBuffer = new byte[readSize];
        marshaller.MarshalInParameters(rpcMethod, allValues, writeBuffer.AsSpan<byte>());
        int num = 0;
        ResultHandle resultHandle = await ((IAdsHandle) symbolInfoTable.Connection).CreateVariableHandleAsync(symbolMethodPath, cancel).ConfigureAwait(false);
        uint varHandle = resultHandle.Handle;
        ((ResultAds) result).SetError(((ResultAds) resultHandle).ErrorCode);
        if (((ResultAds) resultHandle).ErrorCode == null)
        {
          try
          {
            ResultReadWrite resultReadWrite = await ((IAdsHandle) symbolInfoTable.Connection).ReadWriteAsync(varHandle, (Memory<byte>) readBuffer, (ReadOnlyMemory<byte>) writeBuffer, cancel).ConfigureAwait(false);
            ((ResultAds) result).SetError(((ResultAds) resultReadWrite).ErrorCode);
            if (((ResultAds) resultReadWrite).Succeeded)
            {
              if (readSize > 0)
              {
                object returnValue = (object) null;
                num = marshaller.UnmarshalRpcMethod(rpcMethod, outSpecifiers, retSpecifier, allValues, (ReadOnlySpan<byte>) readBuffer, out returnValue);
                List<object> objectList = new List<object>();
                for (int index = 0; index < ((ICollection<IRpcMethodParameter>) rpcMethod.Parameters).Count; ++index)
                {
                  if (((Enum) (object) ((IList<IRpcMethodParameter>) rpcMethod.Parameters)[index].ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 2))
                    objectList.Add(allValues[index]);
                }
                object[] array = objectList.ToArray();
                result = new ResultRpcMethod(((ResultAds) resultReadWrite).ErrorCode, returnValue, array, ((ResultAds) resultReadWrite).InvokeId);
              }
            }
          }
          finally
          {
            ResultAds resultAds = await ((IAdsHandle) symbolInfoTable.Connection).DeleteVariableHandleAsync(varHandle, cancel).ConfigureAwait(false);
          }
        }
        allValues = (object[]) null;
        writeBuffer = (byte[]) null;
        readBuffer = (byte[]) null;
      }
      catch (MarshalException ex)
      {
        throw new RpcInvokeException((IInterfaceInstance) symbol, rpcMethod.Name, (Exception) ex);
      }
      return result;
    }

    private void SetRpcInParameterValues(
      IRpcMethod method,
      object[]? inParameterValues,
      object[] allParameterValues)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (((ICollection<IRpcMethodParameter>) method.Parameters).Count != allParameterValues.Length)
        throw new ArgumentOutOfRangeException(nameof (allParameterValues));
      int num1 = ((IEnumerable<IRpcMethodParameter>) method.Parameters).Where<IRpcMethodParameter>((Func<IRpcMethodParameter, bool>) (p => ((Enum) (object) p.ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 1))).Count<IRpcMethodParameter>();
      if (inParameterValues != null)
      {
        if (num1 != inParameterValues.Length)
          throw new ArgumentOutOfRangeException(nameof (inParameterValues));
      }
      else if (num1 != 0)
        throw new ArgumentOutOfRangeException(nameof (inParameterValues));
      int num2 = 0;
      if (inParameterValues == null)
        return;
      for (int index = 0; index < ((ICollection<IRpcMethodParameter>) method.Parameters).Count; ++index)
      {
        if (((Enum) (object) ((IList<IRpcMethodParameter>) method.Parameters)[index].ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 1))
          allParameterValues[index] = inParameterValues[num2++];
      }
    }

    public AdsErrorCode TryInvokeRpcMethod(
      IRpcCallableInstance symbol,
      IRpcMethod rpcMethod,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters,
      out object? retValue)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (rpcMethod == null)
        throw new ArgumentNullException(nameof (rpcMethod));
      SymbolInfoTable.EnsureParametersValid(rpcMethod, inParameters);
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) 0;
      retValue = (object) null;
      outParameters = (object[]) null;
      bool flag;
      string methodAccessPath = RpcInvokeParser.GetRpcMethodAccessPath(symbol, rpcMethod, ref flag);
      IDataType type1 = (IDataType) null;
      if (!rpcMethod.IsVoid)
        adsErrorCode1 = this.TryReadType(rpcMethod.ReturnType, true, out type1);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode1))
      {
        List<IDataType> idataTypeList = new List<IDataType>();
        foreach (RpcMethodParameter parameter in (IEnumerable<IRpcMethodParameter>) rpcMethod.Parameters)
        {
          IDataType type2 = (IDataType) null;
          AdsErrorCode adsErrorCode2 = this.TryReadType(parameter.TypeName, true, out type2);
          if (AdsErrorCodeExtensions.Failed(adsErrorCode2))
            return adsErrorCode2;
          if (type2 == null)
            throw new DataTypeException("Not supported!", parameter.TypeName);
          idataTypeList.Add(type2);
        }
        SymbolRpcMarshaler symbolRpcMarshaler = new SymbolRpcMarshaler((IDataTypeResolver) this.FactoryServices.Binder);
        try
        {
          object[] objArray = new object[((ICollection<IRpcMethodParameter>) rpcMethod.Parameters).Count];
          this.SetRpcInParameterValues(rpcMethod, inParameters, objArray);
          int inMarshallingSize = symbolRpcMarshaler.GetInMarshallingSize(rpcMethod, objArray);
          int outMarshallingSize = symbolRpcMarshaler.GetOutMarshallingSize(rpcMethod, outSpecifiers, retSpecifier, objArray);
          byte[] array = new byte[inMarshallingSize];
          byte[] source = new byte[outMarshallingSize];
          symbolRpcMarshaler.MarshalInParameters(rpcMethod, objArray, array.AsSpan<byte>());
          int num1 = 0;
          uint num2;
          adsErrorCode1 = ((IAdsHandle) this.Connection).TryCreateVariableHandle(methodAccessPath, ref num2);
          if (AdsErrorCodeExtensions.Succeeded(adsErrorCode1))
          {
            try
            {
              int num3 = 0;
              adsErrorCode1 = ((IAdsHandle) this.Connection).TryReadWrite(num2, (Memory<byte>) source, (ReadOnlyMemory<byte>) array, ref num3);
              if (AdsErrorCodeExtensions.Succeeded(adsErrorCode1))
              {
                if (outMarshallingSize > 0)
                {
                  num1 = symbolRpcMarshaler.UnmarshalRpcMethod(rpcMethod, outSpecifiers, retSpecifier, objArray, (ReadOnlySpan<byte>) source, out retValue);
                  List<object> objectList = new List<object>();
                  for (int index = 0; index < ((ICollection<IRpcMethodParameter>) rpcMethod.Parameters).Count; ++index)
                  {
                    if (((Enum) (object) ((IList<IRpcMethodParameter>) rpcMethod.Parameters)[index].ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 2))
                      objectList.Add(objArray[index]);
                  }
                  outParameters = objectList.ToArray();
                }
              }
            }
            finally
            {
              ((IAdsHandle) this.Connection).TryDeleteVariableHandle(num2);
            }
          }
        }
        catch (MarshalException ex)
        {
          throw new RpcInvokeException((IInterfaceInstance) symbol, rpcMethod.Name, (Exception) ex);
        }
      }
      return adsErrorCode1;
    }

    /// <summary>Loads the data.</summary>
    protected override AdsErrorCode TryReadTypesSymbols() => (AdsErrorCode) 0;

    /// <summary>Loads the types.</summary>
    /// <param name="timeout">The timeout.</param>
    protected override AdsErrorCode TryReadTypes(TimeSpan timeout) => (AdsErrorCode) 0;

    /// <summary>Gets the symbols asynchronously</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'GetDataTypes' operation. The <see cref="T:TwinCAT.TypeSystem.ResultSymbols" /> parameter contains the data types
    /// (<see cref="P:TwinCAT.TypeSystem.ResultSymbols`1.Symbols" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public override Task<ResultSymbols> GetSymbolsAsync(CancellationToken cancel) => throw new NotImplementedException();

    /// <summary>Gets the data types asynchronously.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'GetDataTypes' operation. The <see cref="T:TwinCAT.TypeSystem.ResultDataTypes" /> parameter contains the data types
    /// (<see cref="P:TwinCAT.TypeSystem.ResultDataTypes.DataTypes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <seealso cref="P:TwinCAT.TypeSystem.ISymbolServer.DataTypes" />
    public override Task<ResultDataTypes> GetDataTypesAsync(
      CancellationToken cancel)
    {
      throw new NotImplementedException();
    }

    /// <summary>Called when [types generated].</summary>
    /// <param name="types">The types.</param>
    private void OnTypesGenerated(DataTypeCollection types)
    {
      if (this.TypesGenerated == null)
        return;
      this.TypesGenerated((object) this, new DataTypeEventArgs((IEnumerable<IDataType>) types));
    }

    /// <summary>Called when [type generated].</summary>
    /// <param name="type">The type.</param>
    private void OnTypeGenerated(IDataType type)
    {
      if (this.TypesGenerated == null)
        return;
      DataTypeCollection dataTypeCollection = new DataTypeCollection();
      dataTypeCollection.Add(type);
      this.TypesGenerated((object) this, new DataTypeEventArgs((IEnumerable<IDataType>) dataTypeCollection));
    }

    /// <summary>Occurs when a new type was generated.</summary>
    public event EventHandler<DataTypeEventArgs>? TypesGenerated;

    /// <summary>Called when the data type resolution fails</summary>
    /// <param name="typeName">Name of the type.</param>
    private void OnResolveError(string typeName)
    {
      if (this.TypeResolveError == null)
        return;
      this.TypeResolveError((object) this, new DataTypeNameEventArgs(typeName));
    }

    /// <summary>Occurs when the datatype resolution fails</summary>
    public event EventHandler<DataTypeNameEventArgs>? TypeResolveError;

    private void Connection_AdsSymbolVersionChanged(
      object? sender,
      AdsSymbolVersionChangedEventArgs e)
    {
      this.OnSymbolVersionChanged();
    }

    private void OnSymbolVersionChanged()
    {
    }
  }
}
