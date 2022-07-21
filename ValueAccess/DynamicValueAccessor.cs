// Decompiled with JetBrains decompiler
// Type: TwinCAT.ValueAccess.DynamicValueAccessor
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.ValueAccess
{
  /// <summary>Dynamic Value Accessor implementation class</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DynamicValueAccessor : 
    RpcNotificationAccessorBase,
    IAccessorDynamicValue,
    IAccessorValue,
    IAccessorRawValue
  {
    /// <summary>Value Access Mode</summary>
    private ValueCreationModes _mode = (ValueCreationModes) 1;
    /// <summary>The inner value accessor</summary>
    private IAccessorValue _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.DynamicValueAccessor" /> class.
    /// </summary>
    /// <param name="inner">The inner Accessor</param>
    /// <param name="factory">The factory.</param>
    /// <param name="mode">The mode.</param>
    /// <exception cref="T:System.ArgumentNullException">valueAccessor</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DynamicValueAccessor(
      IAccessorValue inner,
      IAccessorValueFactory factory,
      ValueCreationModes mode)
      : base(factory, ((IAccessorConnection) inner)?.Connection)
    {
      this._mode = mode;
      this._inner = inner;
      if (!(inner is IAccessorNotification iaccessorNotification))
        return;
      this.DefaultNotificationSettings = iaccessorNotification.DefaultNotificationSettings;
    }

    /// <summary>Writes the value to the symbol</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="value">The value (Primitive typed value or <see cref="T:TwinCAT.TypeSystem.DynamicValue" /></param>
    /// <param name="utcWriteTime">The write time snapshot.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// symbol
    /// or
    /// value
    /// </exception>
    public override void WriteValue(ISymbol symbol, object value, out DateTimeOffset? utcWriteTime)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      bool flag = value is IDynamicValue;
      int num = 0;
      if (!flag)
        this.TryWriteValue(symbol, value, out utcWriteTime);
      else
        num = this.TryWriteValue((IDynamicValue) value, out utcWriteTime);
      if (num != 0)
      {
        Exception exception = (Exception) new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Could not write Symbol '{0}' (Type: {1})! Error: {2}", (object) ((IInstance) symbol).InstancePath, (object) ((IInstance) symbol).TypeName, (object) num), symbol);
        AdsModule.Trace.TraceError(exception);
        throw exception;
      }
    }

    /// <summary>Writes the value as asynchronous operation.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'WriteValue' operation. The task result is available via the <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" /> type.</returns>
    public override Task<ResultWriteAccess> WriteValueAsync(
      ISymbol symbol,
      object value,
      CancellationToken cancel)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (!(value is IDynamicValue))
        return base.WriteValueAsync(symbol, value, cancel);
      return this.WriteValueAsync((IDynamicValue) value, cancel);
    }

    /// <summary>Writes a dynamic symbol value.</summary>
    /// <param name="value">Dynamic value (non primitive type).</param>
    /// <param name="utcWriteTime">The write time snapshot.</param>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    /// <exception cref="T:System.ArgumentException">value</exception>
    public int TryWriteValue(IDynamicValue value, out DateTimeOffset? utcWriteTime)
    {
      DynamicValue dynamicValue = value != null ? (DynamicValue) value : throw new ArgumentNullException(nameof (value));
      return this.TryWriteRaw(dynamicValue.Symbol, (ReadOnlyMemory<byte>) dynamicValue.cachedData, out utcWriteTime);
    }

    /// <summary>Writes the value asynchronously.</summary>
    /// <param name="value">The value to write</param>
    /// <param name="cancel">The cancellation token..</param>
    /// <returns>An asynchronous task object that represents the 'WriteValue' operation. The result type of the task is <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    public Task<ResultWriteAccess> WriteValueAsync(
      IDynamicValue value,
      CancellationToken cancel)
    {
      DynamicValue dynamicValue = value != null ? (DynamicValue) value : throw new ArgumentNullException(nameof (value));
      return this.WriteRawAsync(dynamicValue.Symbol, (ReadOnlyMemory<byte>) dynamicValue.cachedData, cancel);
    }

    /// <summary>
    /// Registers a Notification on the <see cref="T:TwinCAT.TypeSystem.ISymbol" />.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">Type of Notification (Value, Raw or Both)</param>
    /// <param name="settings">The settings.</param>
    /// <exclude />
    /// <remarks>Only one Notification is allowed on the symbol. On case of double announcement, we set the Notification parameters
    /// to the higher priority.</remarks>
    public override void OnRegisterNotification(
      ISymbol symbol,
      SymbolNotificationTypes type,
      INotificationSettings settings)
    {
      if (!(this._inner is IAccessorNotification inner))
        throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support INotificationAccessor", (object) this._inner), (IAccessorRawValue) this._inner);
      inner.OnRegisterNotification(symbol, type, settings);
    }

    /// <summary>
    /// Unregisters a Notification from the <see cref="T:TwinCAT.TypeSystem.ISymbol" />.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">Type of Notification (Value, Raw or Both)</param>
    /// <exclude />
    public override void OnUnregisterNotification(ISymbol symbol, SymbolNotificationTypes type)
    {
      if (!(this._inner is IAccessorNotification inner))
        throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support INotificationAccessor", (object) this._inner), (IAccessorRawValue) this._inner);
      inner.OnUnregisterNotification(symbol, type);
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
      if (this._inner is IAccessorRpc inner)
        return inner.TryInvokeRpcMethod(instance, method, inParameters, outParametersSpec, retSpec, ref outParameters, ref returnValue, ref timeStamp);
      returnValue = (object) null;
      outParameters = (object[]) null;
      timeStamp = new DateTimeOffset?(DateTimeOffset.MinValue);
      return 1793;
    }

    public override async Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      IInstance instance,
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outParametersSpec,
      AnyTypeSpecifier? retSpec,
      CancellationToken cancel)
    {
      ResultRpcMethodAccess resultRpcMethodAccess = new ResultRpcMethodAccess((object) null, (object[]) null, 1793, DateTimeOffset.MinValue, 0U);
      if (this._inner is IAccessorRpc inner)
        resultRpcMethodAccess = await inner.InvokeRpcMethodAsync(instance, method, inParameters, outParametersSpec, retSpec, cancel).ConfigureAwait(false);
      return resultRpcMethodAccess;
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
      return ((IAccessorRawValue) this._inner).TryReadArrayElementRaw(arrayInstance, indices, destination, ref timeStamp);
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
    public override Task<ResultReadRawAccess> ReadArrayElementRawAsync(
      IArrayInstance arrayInstance,
      int[] indices,
      Memory<byte> destination,
      CancellationToken cancel)
    {
      return ((IAccessorRawValue) this._inner).ReadArrayElementRawAsync(arrayInstance, indices, destination, cancel);
    }

    /// <summary>Try to read value</summary>
    /// <param name="symbolInstance">The symbol instance.</param>
    /// <param name="value">The value.</param>
    /// <param name="utcReadTime">The read time snapshot (User Time, UTC)</param>
    public override int TryReadRaw(
      ISymbol symbolInstance,
      Memory<byte> value,
      out DateTimeOffset? utcReadTime)
    {
      return ((IAccessorRawValue) this._inner).TryReadRaw(symbolInstance, value, ref utcReadTime);
    }

    /// <summary>Read a Symbol value asynchronously as bytes .</summary>
    /// <param name="symbolInstance">The symbol instance.</param>
    /// <param name="destination">The destination / value memory.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadRaw' operation. The <see cref="T:TwinCAT.ValueAccess.ResultReadRawAccess" /> result contains the
    /// (<see cref="P:TwinCAT.ValueAccess.ResultReadValueAccess`1.Value" />) and the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" /> after execution.
    /// </returns>
    public override Task<ResultReadRawAccess> ReadRawAsync(
      ISymbol symbolInstance,
      Memory<byte> destination,
      CancellationToken cancel)
    {
      return ((IAccessorRawValue) this._inner).ReadRawAsync(symbolInstance, destination, cancel);
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
      return ((IAccessorRawValue) this._inner).TryWriteArrayElementRaw(arrayInstance, indices, sourceData, ref timeStamp);
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
    public override Task<ResultWriteAccess> WriteArrayElementRawAsync(
      IArrayInstance arrayInstance,
      int[] indices,
      ReadOnlyMemory<byte> sourceData,
      CancellationToken cancel)
    {
      return ((IAccessorRawValue) this._inner).WriteArrayElementRawAsync(arrayInstance, indices, sourceData, cancel);
    }

    /// <summary>
    /// Writes the symbol value from source memory location to the ADS Device.
    /// </summary>
    /// <param name="symbolInstance">The symbol instance.</param>
    /// <param name="source">The source memory location.</param>
    /// <param name="timeStamp">The write timestamp.</param>
    /// <returns>Error code. 0 represents succeed.</returns>
    public override int TryWriteRaw(
      ISymbol symbolInstance,
      ReadOnlyMemory<byte> source,
      out DateTimeOffset? timeStamp)
    {
      return ((IAccessorRawValue) this._inner).TryWriteRaw(symbolInstance, source, ref timeStamp);
    }

    /// <summary>
    /// Writes the symbol value asynchronously from source memory location to the ADS Device.
    /// </summary>
    /// <param name="symbolInstance">The symbol instance.</param>
    /// <param name="sourceData">The source value from memory location.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'WriteRaw' operation. The <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" /> result contains the
    /// the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" /> after execution.</returns>
    public override Task<ResultWriteAccess> WriteRawAsync(
      ISymbol symbolInstance,
      ReadOnlyMemory<byte> sourceData,
      CancellationToken cancel)
    {
      return ((IAccessorRawValue) this._inner).WriteRawAsync(symbolInstance, sourceData, cancel);
    }
  }
}
