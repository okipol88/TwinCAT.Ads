// Decompiled with JetBrains decompiler
// Type: TwinCAT.ValueAccess.ValueAccessor
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.ValueAccess
{
  /// <summary>
  /// Value Accessor base class (implements RawValue and Value Access)
  /// </summary>
  /// <seealso cref="T:TwinCAT.ValueAccess.IAccessorRawValue" />
  /// <seealso cref="T:TwinCAT.ValueAccess.IAccessorValue" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class ValueAccessor : IAccessorValue, IAccessorRawValue, IAccessorConnection
  {
    /// <summary>The connection</summary>
    private IConnection? connection;
    /// <summary>Session object</summary>
    private ISession? session;
    /// <summary>The value factory</summary>
    private IAccessorValueFactory valueFactory;

    protected ValueAccessor(IAccessorValueFactory factory, ISession? session)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      this.session = session;
      if (session != null)
        this.connection = session.Connection;
      this.valueFactory = factory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.ValueAccessor" /> class.
    /// </summary>
    /// <param name="factory">The value factory.</param>
    /// <param name="connection">The connection.</param>
    /// <exception cref="T:System.ArgumentNullException">factory</exception>
    protected ValueAccessor(IAccessorValueFactory factory, IConnection connection)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (!connection.IsConnected)
        throw new ClientNotConnectedException();
      if (connection.Session != null)
        this.session = connection.Session;
      this.connection = connection;
      this.valueFactory = factory;
    }

    /// <summary>Gets the connection.</summary>
    /// <value>The connection.</value>
    public IConnection? Connection => this.session != null ? this.session.Connection : this.connection;

    /// <summary>Gets the session.</summary>
    /// <value>The session or NULL if not session based.</value>
    public ISession? Session => this.session;

    /// <summary>Gets the value factory.</summary>
    /// <value>The value factory.</value>
    public IAccessorValueFactory ValueFactory => this.valueFactory;

    /// <summary>Gets the default value encoding.</summary>
    /// <value>The default value encoding.</value>
    public Encoding DefaultValueEncoding => StringMarshaler.DefaultEncoding;

    /// <summary>Reads the symbol value.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="timeStamp">The read time snapshot.</param>
    /// <returns>The value object (Primitive type or DynamicValue)</returns>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    public object ReadValue(ISymbol symbol, out DateTimeOffset? timeStamp)
    {
      object obj = (object) null;
      AdsErrorCode adsErrorCode = (AdsErrorCode) this.TryReadValue(symbol, out obj, out timeStamp);
      if (adsErrorCode != null)
      {
        SymbolException symbolException = new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Could not read Symbol '{0}'! Error: {1}", (object) symbol, (object) adsErrorCode), symbol, (Exception) AdsErrorException.Create(adsErrorCode));
        AdsModule.Trace.TraceError((Exception) symbolException);
        throw symbolException;
      }
      return obj;
    }

    /// <summary>Reads an array element value as bytes.</summary>
    /// <param name="arrayInstance">The array instance.</param>
    /// <param name="indices">The indices specify which element to read.</param>
    /// <param name="destination">The destination / value memory.</param>
    /// <param name="timeStamp">The read time snapshot</param>
    /// <returns>Error code. 0 represents succeed.</returns>
    public abstract int TryReadArrayElementRaw(
      IArrayInstance arrayInstance,
      int[] indices,
      Memory<byte> destination,
      out DateTimeOffset? timeStamp);

    /// <summary>Reads a value from the specified ADS address</summary>
    /// <param name="symbol">The address.</param>
    /// <param name="value">Raw value</param>
    /// <param name="timeStamp">The read time snapshot.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exclude />
    public int TryReadValue(ISymbol symbol, out object? value, out DateTimeOffset? timeStamp)
    {
      int num1 = symbol != null ? symbol.GetValueMarshalSize() : throw new ArgumentNullException(nameof (symbol));
      if (num1 <= 0)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
        interpolatedStringHandler.AppendLiteral("Symbol '");
        interpolatedStringHandler.AppendFormatted<ISymbol>(symbol);
        interpolatedStringHandler.AppendLiteral("' is of invalid size '");
        interpolatedStringHandler.AppendFormatted<int>(num1);
        interpolatedStringHandler.AppendLiteral("'. Cannot read!");
        throw new SymbolException(interpolatedStringHandler.ToStringAndClear(), symbol);
      }
      byte[] array = new byte[symbol.GetValueMarshalSize()];
      value = (object) null;
      ISymbol symbolInstance = symbol;
      int num2 = this.TryReadRaw(symbolInstance, array.AsMemory<byte>(), out timeStamp);
      if (num2 == 0)
        value = this.valueFactory == null ? (object) array : this.valueFactory.CreateValue(symbolInstance, (ReadOnlySpan<byte>) array, timeStamp.Value);
      return num2;
    }

    /// <summary>Reads the symbol value as an asynchronous operation.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'ReadValue' operation. The value of the result can be accessed via the <see cref="T:TwinCAT.ValueAccess.ResultReadValueAccess" /> result.</returns>
    public async Task<ResultReadValueAccess> ReadValueAsync(
      ISymbol symbol,
      CancellationToken cancel)
    {
      byte[] bValue = symbol != null ? new byte[symbol.GetValueMarshalSize()] : throw new ArgumentNullException(nameof (symbol));
      object value = (object) null;
      ISymbol resolved = symbol;
      ResultReadRawAccess resultReadRawAccess = await this.ReadRawAsync(resolved, bValue.AsMemory<byte>(), cancel).ConfigureAwait(false);
      if (((ResultAccess) resultReadRawAccess).ErrorCode == 0)
        value = this.valueFactory == null ? (object) bValue : this.valueFactory.CreateValue(resolved, (ReadOnlySpan<byte>) bValue, ((ResultAccess) resultReadRawAccess).DateTime);
      ResultReadValueAccess resultReadValueAccess = new ResultReadValueAccess(value, ((ResultAccess) resultReadRawAccess).ErrorCode, ((ResultAccess) resultReadRawAccess).InvokeId);
      bValue = (byte[]) null;
      value = (object) null;
      resolved = (ISymbol) null;
      return resultReadValueAccess;
    }

    /// <summary>Try to read value</summary>
    /// <param name="symbolInstance">The symbol instance.</param>
    /// <param name="value">The value.</param>
    /// <param name="utcReadTime">The read time snapshot (User Time, UTC)</param>
    /// <returns>AdsErrorCode.</returns>
    public abstract int TryReadRaw(
      ISymbol symbolInstance,
      Memory<byte> value,
      out DateTimeOffset? utcReadTime);

    /// <summary>
    /// Writes an array element value from raw memory asynchronously to the ADS Device.
    /// </summary>
    /// <param name="arrayInstance">The array instance.</param>
    /// <param name="indices">The indices of the array element.</param>
    /// <param name="sourceData">The data to write to the ADS Device./&gt;.</param>
    /// <param name="timeStamp">Write time / timestamp</param>
    /// <returns>Error code. 0 represents succeed.</returns>
    public abstract int TryWriteArrayElementRaw(
      IArrayInstance arrayInstance,
      int[] indices,
      ReadOnlyMemory<byte> sourceData,
      out DateTimeOffset? timeStamp);

    /// <summary>Tries to write the Value</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="value">The value.</param>
    /// <param name="utcWriteTime">The UTC write time.</param>
    /// <returns>AdsErrorCode.</returns>
    public int TryWriteValue(ISymbol symbol, object value, out DateTimeOffset? utcWriteTime)
    {
      ISymbol isymbol = symbol != null ? symbol : throw new ArgumentNullException(nameof (symbol));
      InstanceValueMarshaler instanceValueMarshaler = new InstanceValueMarshaler();
      if (((IBitSize) symbol).ByteSize <= 0)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
        interpolatedStringHandler.AppendLiteral("Symbol '");
        interpolatedStringHandler.AppendFormatted<ISymbol>(symbol);
        interpolatedStringHandler.AppendLiteral("' is of invalid size '");
        interpolatedStringHandler.AppendFormatted<int>(((IBitSize) symbol).ByteSize);
        interpolatedStringHandler.AppendLiteral("'. Cannot write!");
        throw new SymbolException(interpolatedStringHandler.ToStringAndClear(), symbol);
      }
      byte[] array = new byte[((IBitSize) symbol).ByteSize];
      instanceValueMarshaler.Marshal((IAttributedInstance) isymbol, value, array.AsSpan<byte>());
      return this.TryWriteRaw(isymbol, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), out utcWriteTime);
    }

    /// <summary>Tries to write the Value</summary>
    /// <param name="symbolInstance">The address.</param>
    /// <param name="sourceValue">The value as memory.</param>
    /// <param name="utcWriteTime">The write time snapshot.</param>
    /// <returns>AdsErrorCode.</returns>
    public abstract int TryWriteRaw(
      ISymbol symbolInstance,
      ReadOnlyMemory<byte> sourceValue,
      out DateTimeOffset? utcWriteTime);

    /// <summary>Writes the value to the symbol</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="value">The value</param>
    /// <param name="utcWriteTime">The write time snapshot.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// symbol
    /// or
    /// value
    /// </exception>
    public virtual void WriteValue(ISymbol symbol, object value, out DateTimeOffset? utcWriteTime)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) ((IAccessorValue) this).TryWriteValue(symbol, value, ref utcWriteTime);
      if (adsErrorCode != null)
      {
        SymbolException symbolException = new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Could not write Symbol '{0} '! Error: {1}", (object) symbol, (object) adsErrorCode), symbol);
        AdsModule.Trace.TraceError((Exception) symbolException);
        throw symbolException;
      }
    }

    /// <summary>Writes the value as asynchronous operation.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'WriteValue' operation. The task result is available via the <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" /> type.</returns>
    public virtual Task<ResultWriteAccess> WriteValueAsync(
      ISymbol symbol,
      object value,
      CancellationToken cancel)
    {
      ISymbol isymbol = symbol != null ? symbol : throw new ArgumentNullException(nameof (symbol));
      Memory<byte> memory = (Memory<byte>) (byte[]) null;
      switch (value)
      {
        case byte[] _:
          sourceValue = ((byte[]) value).AsMemory<byte>();
          goto label_5;
        case Memory<byte> sourceValue:
label_5:
          return this.WriteRawAsync(isymbol, (ReadOnlyMemory<byte>) sourceValue, cancel);
        default:
          InstanceValueMarshaler instanceValueMarshaler = new InstanceValueMarshaler();
          byte[] array = new byte[((IBitSize) symbol).ByteSize];
          int length = instanceValueMarshaler.Marshal((IAttributedInstance) isymbol, value, array.AsSpan<byte>());
          sourceValue = array.AsMemory<byte>(0, length);
          goto label_5;
      }
    }

    /// <summary>Read the raw memory value as asynchronous operation.</summary>
    /// <param name="symbolInstance">The symbol instance.</param>
    /// <param name="destination">the memory destination.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'ReadRaw' operation. The result of the operation is represented by the type <see cref="T:TwinCAT.ValueAccess.ResultReadRawAccess" />.</returns>
    public abstract Task<ResultReadRawAccess> ReadRawAsync(
      ISymbol symbolInstance,
      Memory<byte> destination,
      CancellationToken cancel);

    /// <summary>
    /// Reads the raw memory value of an array element as asynchronous operation
    /// </summary>
    /// <param name="arrayInstance">The array instance.</param>
    /// <param name="indices">The indices of the element</param>
    /// <param name="destination">Memory location for the value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'ReadArrayElementRaw' operation. The result of the operation is represented by the type <see cref="T:TwinCAT.ValueAccess.ResultReadRawAccess" />.</returns>
    public abstract Task<ResultReadRawAccess> ReadArrayElementRawAsync(
      IArrayInstance arrayInstance,
      int[] indices,
      Memory<byte> destination,
      CancellationToken cancel);

    /// <summary>
    /// Writes the raw value to the symbol as asynchronous operation.
    /// </summary>
    /// <param name="symbolInstance">The symbol instance.</param>
    /// <param name="sourceValue">The raw source value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'WriteRAw' operation. The result of the operation is represented by the type <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" />.</returns>
    public abstract Task<ResultWriteAccess> WriteRawAsync(
      ISymbol symbolInstance,
      ReadOnlyMemory<byte> sourceValue,
      CancellationToken cancel);

    /// <summary>
    /// Writes the raw value of an array element as asynchronous operation.
    /// </summary>
    /// <param name="arrayInstance">The symbol instance.</param>
    /// <param name="indices">The indices of the array element.</param>
    /// <param name="sourceValue">The raw source element value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'WriteRAw' operation. The result of the operation is represented by the type <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" />.</returns>
    public abstract Task<ResultWriteAccess> WriteArrayElementRawAsync(
      IArrayInstance arrayInstance,
      int[] indices,
      ReadOnlyMemory<byte> sourceValue,
      CancellationToken cancel);

    /// <summary>
    /// Handler function for reading the raw value asynchronously.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="rawValue">The raw value.</param>
    /// <param name="timeStamp">The timstamp of the read operation.</param>
    protected virtual void OnRawValueChanged(
      ISymbol symbol,
      ReadOnlyMemory<byte> rawValue,
      DateTimeOffset timeStamp)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      RawValueChangedEventArgs changedEventArgs = new RawValueChangedEventArgs(symbol, rawValue, timeStamp);
      ((ISymbolValueChangeNotify) symbol).OnRawValueChanged(changedEventArgs);
    }

    /// <summary>Called when the (Primitive) Value changes</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="source">The raw source value.</param>
    /// <param name="timeStamp">The timestamp specifying the time when the value change occurred.</param>
    protected virtual void OnValueChanged(
      ISymbol symbol,
      ReadOnlySpan<byte> source,
      DateTimeOffset timeStamp)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      object obj = this.valueFactory.CreateValue(symbol, source, timeStamp);
      ValueChangedEventArgs changedEventArgs = new ValueChangedEventArgs(symbol, obj, timeStamp);
      ((ISymbolValueChangeNotify) symbol).OnValueChanged(changedEventArgs);
    }
  }
}
