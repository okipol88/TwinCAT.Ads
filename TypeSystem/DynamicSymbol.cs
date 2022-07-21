// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicSymbol
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Dynamic <see cref="T:TwinCAT.TypeSystem.ISymbol">Symbol</see> object.
  /// </summary>
  /// <remarks>The <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" /> adds dynamic run time behaviour to the <see cref="T:TwinCAT.TypeSystem.ISymbol" />/<see cref="T:TwinCAT.TypeSystem.IValueSymbol" />.
  /// That means e.g. for StructSymbols that .NET Properties are defined and dispatched at runtime
  /// to the structs fields like they are defined in TwinCAT / ADS Types.
  /// Indexed access to Array Symbols is another example where the dynamic runtime support takes place.
  /// </remarks>
  /// <example>
  /// Sample for the dynamic resolution of Symbols:
  /// <code language="C#" title="Dynamic Symbol access" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE_SIMPLEDYNAMIC" />
  /// </example>
  /// <seealso cref="T:TwinCAT.TypeSystem.IDynamicSymbol" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IValueSymbol" />
  /// <seealso cref="T:TwinCAT.TypeSystem.ISymbol" />
  /// <seealso cref="T:System.Dynamic.DynamicObject" />
  [DebuggerDisplay("Path = { InstancePath }, Type = {TypeName}, Size = {Size}, Category = {Category}")]
  public class DynamicSymbol : 
    DynamicObject,
    IDynamicSymbol,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    ISymbolFactoryServicesProvider,
    IValueSymbol,
    IValueRawSymbol,
    IHierarchicalSymbol,
    IValueAccessorProvider,
    ISymbolValueChangeNotify,
    IBinderProvider,
    IContextMaskProvider,
    IInstanceInternal,
    ISymbolInternal
  {
    /// <summary>
    /// Indicates, that the aggregates symbols is an IProcessImageAddress (and most probably IAdsSymbol)
    /// </summary>
    private bool allowIGIOAccess;
    /// <summary>
    /// Static symbol object wrapped by this <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />
    /// </summary>
    /// <exclude />
    private IValueSymbol symbol;
    /// <summary>
    /// The normalized name of this <seealso cref="T:TwinCAT.TypeSystem.DynamicSymbol" />.
    /// </summary>
    private string? normalizedName;
    /// <summary>RawValueChanged delegate</summary>
    private EventHandler<RawValueChangedEventArgs>? _rawValueChanged;
    /// <summary>Synchronization object</summary>
    protected object syncObject = new object();
    /// <summary>ValueChanged delegate.</summary>
    private EventHandler<ValueChangedEventArgs>? _valueChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicValue" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    internal DynamicSymbol(IValueSymbol symbol)
    {
      this.symbol = symbol != null ? symbol : throw new ArgumentNullException(nameof (symbol));
      this.allowIGIOAccess = symbol is IProcessImageAddress;
    }

    /// <summary>
    /// Indicates, that the aggregates symbols is an IProcessImageAddress (and most probably IAdsSymbol)
    /// </summary>
    protected bool AllowIGIOAccess => this.allowIGIOAccess;

    /// <summary>Equals</summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj != null && !(this.GetType() != obj.GetType()) && ((object) this.symbol).Equals((object) ((DynamicSymbol) obj).symbol);

    /// <summary>Operator==</summary>
    /// <param name="o1">The o1.</param>
    /// <param name="o2">The o2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(DynamicSymbol? o1, DynamicSymbol? o2)
    {
      if (!object.Equals((object) o1, (object) null))
        return o1.Equals((object) o2);
      return object.Equals((object) o2, (object) null);
    }

    /// <summary>Implements the != operator.</summary>
    /// <param name="o1">The o1.</param>
    /// <param name="o2">The o2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(DynamicSymbol? o1, DynamicSymbol? o2) => !(o1 == o2);

    /// <summary>Gets the HashCode of the Address</summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => 91 * 10 + ((object) this.symbol).GetHashCode();

    /// <summary>Sets a new instance name.</summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <remarks>This can only used as long the Instance is not added to a collection using the type binder classes.</remarks>
    /// <exclude />
    void IInstanceInternal.SetInstanceName(string instanceName) => this.OnSetInstanceName(instanceName);

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    public override bool TryGetMember(GetMemberBinder binder, [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      if (this.symbol is IProcessImageAddress symbol)
      {
        if (binder.Name == "IndexGroup")
        {
          result = (object) symbol.IndexGroup;
          return true;
        }
        if (binder.Name == "IndexOffset")
        {
          result = (object) symbol.IndexOffset;
          return true;
        }
      }
      return base.TryGetMember(binder, out result);
    }

    /// <summary>Returns the enumeration of all dynamic member names.</summary>
    /// <returns>A sequence that contains dynamic member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames()
    {
      IProcessImageAddress symbol = this.symbol as IProcessImageAddress;
      List<string> dynamicMemberNames = new List<string>(base.GetDynamicMemberNames());
      if (symbol != null)
      {
        dynamicMemberNames.Add("IndexGroup");
        dynamicMemberNames.Add("IndexOffset");
      }
      return (IEnumerable<string>) dynamicMemberNames;
    }

    /// <summary>Sets a new InstanceName InstancePath</summary>
    /// <param name="instanceName">Instance name.</param>
    protected virtual void OnSetInstanceName(string instanceName) => ((IInstanceInternal) this.symbol).SetInstanceName(instanceName);

    /// <summary>
    /// Gets the inner symbol of this <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />
    /// </summary>
    /// <value>The inner symbol.</value>
    public IValueSymbol _InnerSymbol => this.symbol;

    /// <summary>
    /// Gets a value indicating whether this instance has value (only the non VirtualInstances, what means the Symbols with locations).
    /// </summary>
    /// <value><c>true</c> if this instance has value; otherwise, <c>false</c>.</value>
    public bool HasValue => ((IValueRawSymbol) this.symbol).HasValue;

    /// <summary>Gets the notification settings.</summary>
    /// <value>The notification settings.</value>
    public INotificationSettings? NotificationSettings
    {
      get => this.symbol.NotificationSettings;
      set => this.symbol.NotificationSettings = value;
    }

    /// <summary>Gets the category.</summary>
    /// <value>The category.</value>
    public DataTypeCategory Category => ((ISymbol) this.symbol).Category;

    /// <summary>Gets the parent Symbol</summary>
    /// <value>The parent.</value>
    public ISymbol? Parent => ((ISymbol) this.symbol)?.Parent;

    /// <summary>Sets the parent of the Symbol</summary>
    /// <param name="symbol">The symbol.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SetParent(ISymbol symbol)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if ((object) (symbol as DynamicSymbol) == null)
        throw new ArgumentException("Symbol is not dynamic!", nameof (symbol));
      ((IHierarchicalSymbol) this.symbol).SetParent(symbol);
    }

    /// <summary>
    /// Gets the SubSymbols of the <see cref="T:TwinCAT.TypeSystem.ISymbol" />
    /// </summary>
    /// <value></value>
    /// <remarks>Only used for Array and Struct instances. Otherwise empty</remarks>
    public ISymbolCollection<ISymbol> SubSymbols
    {
      get
      {
        ISymbolCollection<ISymbol> symbols = (ISymbolCollection<ISymbol>) null;
        ISymbolInternal isymbolInternal = (ISymbolInternal) this;
        if (isymbolInternal != null)
          symbols = isymbolInternal.SubSymbolsInternal;
        if (symbols == null)
          symbols = (ISymbolCollection<ISymbol>) new SymbolCollection();
        return (ISymbolCollection<ISymbol>) new ReadOnlySymbolCollection((IInstanceCollection<ISymbol>) symbols);
      }
    }

    /// <summary>Gets the SubSymbols Collection (internal variant)</summary>
    /// <value>The sub symbols internal.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ISymbolCollection<ISymbol> SubSymbolsInternal => !(this.symbol is ISymbolInternal symbol) ? (ISymbolCollection<ISymbol>) new SymbolCollection() : (symbol.SubSymbolsCreated ? symbol.SubSymbolsInternal : symbol.CreateSubSymbols((ISymbol) this));

    /// <summary>
    /// Gets a value indicating whether [sub symbols created].
    /// </summary>
    /// <value><c>true</c> if [sub symbols created]; otherwise, <c>false</c>.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool SubSymbolsCreated => this.symbol is ISymbolInternal symbol && symbol.SubSymbolsCreated;

    /// <summary>Creates the sub symbols.</summary>
    /// <param name="parent">The parent.</param>
    /// <returns>SymbolCollection.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ISymbolCollection<ISymbol> CreateSubSymbols(ISymbol parent) => this.symbol is ISymbolInternal symbol ? symbol.CreateSubSymbols(parent) : (ISymbolCollection<ISymbol>) new ReadOnlySymbolCollection((IInstanceCollection<ISymbol>) new SymbolCollection());

    /// <summary>
    /// Gets the normalized instance name (fixed name for dynamic property access that doesn't contain invalid characters),
    /// </summary>
    /// <value>The normalized instance name (can be the same like <see cref="P:TwinCAT.TypeSystem.IInstance.InstanceName" /></value>
    /// <seealso cref="P:TwinCAT.TypeSystem.IInstance.InstanceName" />
    public string NormalizedName
    {
      get
      {
        if (this.normalizedName == null)
        {
          ISymbolFactory symbolFactory = ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory;
          if (!symbolFactory.HasInvalidCharacters)
          {
            this.normalizedName = this.InstanceName;
          }
          else
          {
            char[] invalidCharacters = symbolFactory.InvalidCharacters;
            char[] charArray = this.InstanceName.ToCharArray();
            for (int index1 = 0; index1 < charArray.Length; ++index1)
            {
              for (int index2 = 0; index2 < invalidCharacters.Length; ++index2)
              {
                if ((int) charArray[index1] == (int) invalidCharacters[index2])
                {
                  charArray[index1] = '_';
                  break;
                }
              }
            }
            this.normalizedName = new string(charArray);
          }
        }
        return this.normalizedName;
      }
    }

    /// <summary>
    /// Reads the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <returns>System.Byte[].</returns>
    /// <value>The raw value.</value>
    public byte[] ReadRawValue() => this.OnReadRawValue(-1);

    /// <summary>Read raw value as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>System.Byte[].</returns>
    /// <value>The raw value.</value>
    public Task<ResultReadRawAccess> ReadRawValueAsync(
      CancellationToken cancel)
    {
      return this.OnReadRawValueAsync(cancel);
    }

    /// <summary>Reads the Symbols raw value</summary>
    /// <param name="timeout">The timeout in ms.</param>
    /// <returns>System.Byte[].</returns>
    /// <value>The raw value in bytes.</value>
    /// <remarks>A negative timeout indicates that the Default Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public byte[] ReadRawValue(int timeout) => this.OnReadRawValue(timeout);

    /// <summary>Handler function for reading Raw symbol value.</summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>System.Byte[].</returns>
    protected virtual byte[] OnReadRawValue(int timeout) => ((IValueRawSymbol) this.symbol).ReadRawValue(timeout);

    /// <summary>
    /// Handler function reading the raw value of the <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultReadRawAccess&gt;.</returns>
    protected virtual Task<ResultReadRawAccess> OnReadRawValueAsync(
      CancellationToken cancel)
    {
      return ((IValueRawSymbol) this.symbol).ReadRawValueAsync(cancel);
    }

    /// <summary>
    /// Writes the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <param name="rawValue">The value as byte array.</param>
    public void WriteRawValue(byte[] rawValue) => this.OnWriteRawValue(rawValue, -1);

    /// <summary>
    /// Writes the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <param name="rawValue">The value as byte array.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultRead" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="P:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution..
    /// </returns>
    public Task<ResultWriteAccess> WriteRawValueAsync(
      byte[] rawValue,
      CancellationToken cancel)
    {
      return this.OnWriteRawValueAsync(rawValue, cancel);
    }

    /// <summary>
    /// Writes the raw value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> (Ads Read / Write)
    /// </summary>
    /// <param name="rawValue">The value as byte array.</param>
    /// <param name="timeout">The timeout.</param>
    /// <value>The value.</value>
    /// <remarks>A negative timeout indicates that the Default Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public void WriteRawValue(byte[] rawValue, int timeout) => this.OnWriteRawValue(rawValue, timeout);

    /// <summary>Handler function for reading symbols raw value.</summary>
    /// <param name="rawValue">The value as byte array.</param>
    /// <param name="timeout">The timeout.</param>
    protected virtual void OnWriteRawValue(byte[] rawValue, int timeout) => ((IValueRawSymbol) this.symbol).WriteRawValue(rawValue, timeout);

    /// <summary>
    /// Handler function for writing the raw <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" /> value.
    /// </summary>
    /// <param name="rawValue">The raw value to write.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task object that represents the 'OnWriteRawValue' operation and returns a <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" /> as result.</returns>
    protected virtual Task<ResultWriteAccess> OnWriteRawValueAsync(
      byte[] rawValue,
      CancellationToken cancel)
    {
      return ((IValueRawSymbol) this.symbol).WriteRawValueAsync(rawValue, cancel);
    }

    /// <summary>
    /// Occurs when the RawValue of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> has changed.
    /// </summary>
    public event EventHandler<RawValueChangedEventArgs>? RawValueChanged
    {
      add
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          bool flag = false;
          if (this._rawValueChanged == null || this._rawValueChanged.GetInvocationList().Length == 0)
            flag = true;
          this._rawValueChanged += value;
          if (!flag || !(this.ValueAccessor is IAccessorNotification valueAccessor))
            return;
          valueAccessor.OnRegisterNotification((ISymbol) this, (SymbolNotificationTypes) 2, this.NotificationSettings);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
      remove
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          this._rawValueChanged -= value;
          if (this._rawValueChanged != null && this._rawValueChanged.GetInvocationList().Length != 0 || !(this.ValueAccessor is IAccessorNotification valueAccessor))
            return;
          valueAccessor.OnUnregisterNotification((ISymbol) this, (SymbolNotificationTypes) 2);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
    }

    /// <summary>
    /// Occurs when the (Primitive) value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> has changed.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code language="C#" title="Use Dynamic Notifications" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2Notifications.cs" region="NOTIFICATION_SAMPLE" />
    /// </example>
    /// </remarks>
    public event EventHandler<ValueChangedEventArgs>? ValueChanged
    {
      add
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          bool flag = false;
          if (this._valueChanged == null || this._valueChanged.GetInvocationList().Length == 0)
            flag = true;
          this._valueChanged += value;
          if (!flag)
            return;
          ((IAccessorNotification) this.ValueAccessor)?.OnRegisterNotification((ISymbol) this, (SymbolNotificationTypes) 1, this.NotificationSettings);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
      remove
      {
        object syncObject = this.syncObject;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(syncObject, ref lockTaken);
          this._valueChanged -= value;
          if (this._valueChanged != null && this._valueChanged.GetInvocationList().Length != 0)
            return;
          ((IAccessorNotification) this.ValueAccessor)?.OnUnregisterNotification((ISymbol) this, (SymbolNotificationTypes) 1);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(syncObject);
        }
      }
    }

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />.
    /// </summary>
    /// <returns>System.Object.</returns>
    /// <remarks>
    /// <example>
    /// <code language="C#" title="Dynamic Read access" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE" />
    /// </example>
    /// </remarks>
    public object ReadValue() => this.OnReadValue(-1);

    /// <summary>
    /// Reads the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" /> asynchronously.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A tasks that represents the asynchronous 'ReadValue' operation. The read result is stored in the <see cref="T:TwinCAT.ValueAccess.ResultReadValueAccess" /> return value and contains
    /// the <see cref="P:TwinCAT.ValueAccess.ResultReadValueAccess`1.Value" /> and the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" />.</returns>
    /// <remarks>Calling on primitive types, a call of this method will return the primitive value.
    /// On complex types (structures and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: the raw byte Array will be returned,
    /// in dynamic mode: A Value will be created on the fly.</remarks>
    public Task<ResultReadValueAccess> ReadValueAsync(
      CancellationToken cancel)
    {
      return this.OnReadValueAsync(cancel);
    }

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />.
    /// </summary>
    /// <param name="timeout">The timeout in ms.</param>
    /// <returns>System.Object.</returns>
    /// <value>The value.</value>
    /// <remarks>Calling on primitive types, a call of this method will return the primitive value.
    /// On complex types (structures and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: the raw byte Array will be returned,
    /// in dynamic mode: A Value will be created on the fly.
    /// A negative timeout indicates that the Default Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public object ReadValue(int timeout) => this.OnReadValue(timeout);

    /// <summary>
    /// Reads the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <param name="timeout">The timeout in ms.</param>
    /// <param name="value">The symbol value.</param>
    /// <returns>The error code.</returns>
    /// <remarks>Calling on primitive types, a call of this method will return the primitive value.
    /// On complex types (structures and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: the raw byte Array will be returned,
    /// in dynamic mode: A Value will be created on the fly.
    /// A negative timeout indicates that the Default Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    public int TryReadValue(int timeout, out object? value) => this.OnTryReadValue(timeout, out value);

    /// <summary>Handler function for the</summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual object OnReadValue(int timeout)
    {
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      IAccessorConnection iaccessorConnection = valueAccessor != null ? valueAccessor as IAccessorConnection : throw new CannotAccessValueException((ISymbol) this.symbol);
      DateTimeOffset? nullable;
      if (timeout < 0 || iaccessorConnection == null)
        return valueAccessor.ReadValue((ISymbol) this.symbol, ref nullable);
      using (new AdsTimeoutSetter(iaccessorConnection.Connection, timeout))
        return valueAccessor.ReadValue((ISymbol) this.symbol, ref nullable);
    }

    /// <summary>Handler function for the</summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="value">The value.</param>
    /// <returns>The error Code.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual int OnTryReadValue(int timeout, out object? value)
    {
      if (!this.HasValue)
      {
        value = (object) null;
        return 1796;
      }
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      IAccessorConnection iaccessorConnection = valueAccessor != null ? valueAccessor as IAccessorConnection : throw new CannotAccessValueException((ISymbol) this.symbol);
      DateTimeOffset? nullable;
      if (timeout < 0 || iaccessorConnection == null)
        return valueAccessor.TryReadValue((ISymbol) this.symbol, ref value, ref nullable);
      using (new AdsTimeoutSetter(iaccessorConnection?.Connection, timeout))
        return valueAccessor.TryReadValue((ISymbol) this.symbol, ref value, ref nullable);
    }

    /// <summary>
    /// Handler function reading the <see cref="T:TwinCAT.TypeSystem.DynamicSymbol">DynamicSymbols</see> value asynchronously.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task returning the <see cref="T:TwinCAT.ValueAccess.ResultReadValueAccess" /> as result.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual Task<ResultReadValueAccess> OnReadValueAsync(
      CancellationToken cancel)
    {
      if (!this.HasValue)
        return Task.FromResult<ResultReadValueAccess>(new ResultReadValueAccess(1796, 0U));
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      IAccessorConnection iaccessorConnection = valueAccessor as IAccessorConnection;
      if (valueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this.symbol);
      return valueAccessor.ReadValueAsync((ISymbol) this.symbol, cancel);
    }

    /// <summary>
    /// Writes the specified value to the <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <remarks>
    /// <example>
    /// <code language="C#" title="Dynamic Write access" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE" />
    /// </example>
    /// </remarks>
    public void WriteValue(object value) => this.OnWriteValue(value, -1);

    /// <summary>
    /// Writes the specified value to the <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout in ms.</param>
    /// <remarks>Calling on primitive types, a call of this method will directly write this Value.
    /// On complex types (structs and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: Only byte Arrays (of correct size) can be written)
    /// in dynamic mode: A Value that represents the value will be accepted also. A negative timeout indicates that the Default
    /// Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    /// <example>
    /// <code language="C#" title="Dynamic Write access" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE" />
    /// </example>
    public void WriteValue(object value, int timeout) => this.OnWriteValue(value, timeout);

    /// <summary>
    /// Writes the specified value to the <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout in ms.</param>
    /// <returns>The error code.</returns>
    /// <remarks>Calling on primitive types, a call of this method will directly write this Value.
    /// On complex types (structs and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: Only byte Arrays (of correct size) can be written)
    /// in dynamic mode: A Value that represents the value will be accepted also. A negative timeout indicates that the Default
    /// Timeout for the communication will be used.
    /// 0 means timeout is switched off.</remarks>
    /// <example>
    /// <code language="C#" title="Dynamic Write access" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE" />
    /// </example>
    public int TryWriteValue(object value, int timeout) => this.OnTryWriteValue(value, timeout);

    /// <summary>
    /// Writes the Value of the <see cref="T:TwinCAT.TypeSystem.IValueSymbol" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A tasks that represents the asynchronous 'ReadValue' operation. The read result is stored in the <see cref="T:TwinCAT.ValueAccess.ResultWriteAccess" /> return value and contains
    /// the <see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" />.</returns>
    /// <remarks>Calling on primitive types, a call of this method will directly write this Value.
    /// On complex types (structs and arrays) it depends on the <see cref="T:TwinCAT.TypeSystem.ISymbolLoader" /> settings what will happen.
    /// In non dynamic modes: Only byte Arrays (of correct size) can be written)
    /// in dynamic mode: A Value that represents the value will be accepted also.</remarks>
    public Task<ResultWriteAccess> WriteValueAsync(
      object value,
      CancellationToken cancel)
    {
      return this.OnWriteValueAsync(value, cancel);
    }

    /// <summary>Handler Function for writing value.</summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual void OnWriteValue(object value, int timeout)
    {
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      IAccessorConnection iaccessorConnection = valueAccessor != null ? valueAccessor as IAccessorConnection : throw new CannotAccessValueException((ISymbol) this.symbol);
      DateTimeOffset? nullable;
      if (timeout >= 0 && iaccessorConnection != null)
      {
        using (new AdsTimeoutSetter(iaccessorConnection?.Connection, timeout))
          valueAccessor.WriteValue((ISymbol) this.symbol, value, ref nullable);
      }
      else
        valueAccessor.WriteValue((ISymbol) this.symbol, value, ref nullable);
    }

    /// <summary>Handler Function for writing value.</summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual int OnTryWriteValue(object value, int timeout)
    {
      if (!this.HasValue)
        return 1796;
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      IAccessorConnection iaccessorConnection = valueAccessor != null ? valueAccessor as IAccessorConnection : throw new CannotAccessValueException((ISymbol) this.symbol);
      DateTimeOffset? nullable;
      if (timeout < 0 || iaccessorConnection == null)
        return valueAccessor.TryWriteValue((ISymbol) this.symbol, value, ref nullable);
      using (new AdsTimeoutSetter(iaccessorConnection?.Connection, timeout))
        return valueAccessor.TryWriteValue((ISymbol) this.symbol, value, ref nullable);
    }

    /// <summary>Handler Function for writing value.</summary>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;WriteValueResult&gt;.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    protected virtual Task<ResultWriteAccess> OnWriteValueAsync(
      object value,
      CancellationToken cancel)
    {
      if (!this.HasValue)
        return Task.FromResult<ResultWriteAccess>(new ResultWriteAccess(1796, 0U));
      IAccessorValue valueAccessor = (IAccessorValue) this.ValueAccessor;
      IAccessorConnection iaccessorConnection = valueAccessor != null ? valueAccessor as IAccessorConnection : throw new CannotAccessValueException((ISymbol) this.symbol);
      return valueAccessor.WriteValueAsync((ISymbol) this.symbol, value, cancel);
    }

    /// <summary>Gets the access rights.</summary>
    /// <value>The access rights.</value>
    public SymbolAccessRights AccessRights => this.symbol.AccessRights;

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IDataType" /> of the <see cref="T:TwinCAT.TypeSystem.IInstance" />.
    /// </summary>
    /// <value>The type of the data.</value>
    public IDataType? DataType => ((IInstance) this.symbol).DataType;

    /// <summary>
    /// Gets the name of the <see cref="T:TwinCAT.TypeSystem.IDataType">DataType</see> that is used for this <see cref="T:TwinCAT.TypeSystem.IInstance" />.
    /// </summary>
    /// <value>The name of the type.</value>
    public string TypeName => ((IInstance) this.symbol).TypeName;

    /// <summary>Gets the name of the instance (without periods (.)</summary>
    /// <value>The name of the instance.</value>
    public string InstanceName => ((IInstance) this.symbol).InstanceName;

    /// <summary>
    /// Gets the relative / absolute access path to the instance (with periods (.))
    /// </summary>
    /// <value>The instance path.</value>
    /// <remarks>If this path is relative or absolute depends on the context. <see cref="T:TwinCAT.TypeSystem.IMember" /> are using relative paths, <see cref="T:TwinCAT.TypeSystem.ISymbol" />s are using absolute ones.</remarks>
    public string InstancePath => ((IInstance) this.symbol).InstancePath;

    /// <summary>
    /// Gets the size of the <see cref="T:TwinCAT.TypeSystem.IDataType" /> in bits.
    /// </summary>
    /// <value>The size of the bit.</value>
    public int BitSize => ((IBitSize) this.symbol).BitSize;

    /// <summary>
    /// Gets a value indicating whether this Symbol is acontainer type.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is container type; otherwise, <c>false</c>.
    /// </value>
    public bool IsContainerType => this.DataType != null ? this.DataType.IsContainer : PrimitiveTypeMarshaler.IsContainerType(this.Category);

    /// <summary>
    /// Gets a value indicating whether this instance is a primitive type.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is primitive type; otherwise, <c>false</c>.
    /// </value>
    public bool IsPrimitiveType => this.DataType != null ? this.DataType.IsPrimitive : PrimitiveTypeMarshaler.IsPrimitiveType(this.Category);

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.ISymbol" /> is persistent.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is persistent; otherwise, <c>false</c>.
    /// </value>
    public bool IsPersistent => ((ISymbol) this.symbol).IsPersistent;

    /// <summary>
    /// Gets a value indicating whether this instance is static.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is static; otherwise, <c>false</c>.
    /// </value>
    public bool IsStatic => ((IInstance) this.symbol).IsStatic;

    /// <summary>Indicates that this instance is read only.</summary>
    /// <remarks>
    /// Actually, this Flag is restricted to TcCOM-Objects readonly Parameters. Within the PLC this is used for the ApplicationName and
    /// ProjectName of PLC instances.
    /// Write-Access on these Modules will create an <see cref="F:TwinCAT.Ads.AdsErrorCode.DeviceAccessDenied" /> error.
    /// </remarks>
    public bool IsReadOnly => ((ISymbol) this.symbol).IsReadOnly;

    /// <summary>
    /// Gets the size of the <see cref="T:TwinCAT.TypeSystem.IInstance" /> in bytes.
    /// </summary>
    /// <value>
    /// The size of the <see cref="T:TwinCAT.TypeSystem.IInstance" /> in bytes.
    /// </value>
    public int Size => ((IBitSize) this.symbol).Size;

    /// <summary>
    /// Gets the (aligned) size of of the Type/Instance in Bytes
    /// </summary>
    /// <value>The size of the byte.</value>
    public int ByteSize => ((IBitSize) this.symbol).ByteSize;

    /// <summary>
    /// Indicates that the Size of the Object is Byte aligned (BitSize % 8 == 0)
    /// </summary>
    /// <value><c>true</c> if this instance is byte aligned; otherwise, <c>false</c>.</value>
    public bool IsByteAligned => ((IBitSize) this.symbol).IsByteAligned;

    /// <summary>
    /// Gets a value indicating whether this instance is not basing on a full DataType but instead of some sort of bit mapping
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is bit mapping; otherwise, <c>false</c>.
    /// </value>
    public bool IsBitType => ((IBitSize) this.symbol).IsBitType;

    /// <summary>Gets the value loader.</summary>
    /// <value>The value loader.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ISymbolFactoryServices FactoryServices => ((ISymbolFactoryServicesProvider) this.symbol).FactoryServices;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => ((object) this.symbol).ToString();

    /// <summary>Gets the Symbol Attributes</summary>
    /// <value>The attributes.</value>
    public ITypeAttributeCollection Attributes => ((IAttributedInstance) this.symbol).Attributes;

    /// <summary>
    /// Indicates that the <see cref="T:TwinCAT.TypeSystem.IInstance" /> represents a Reference type (REFERENCE TO)
    /// </summary>
    /// <value><c>true</c> if is ReferenceTo, otherwise <c>false</c>.</value>
    public bool IsReference
    {
      get
      {
        bool isReference = false;
        if (this.DataType != null)
          return this.DataType.IsReference;
        if (!(this is IVirtualStructInstance))
          isReference = DataTypeStringParser.IsReference(this.TypeName);
        return isReference;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether an ancestor is a reference
    /// </summary>
    /// <value><c>true</c> if this instance is ancestor is reference; otherwise, <c>false</c>.</value>
    internal bool HasReferenceAncestor
    {
      get
      {
        for (DynamicSymbol dynamicSymbol = this; dynamicSymbol.Parent != null; dynamicSymbol = (DynamicSymbol) dynamicSymbol.Parent)
        {
          if (((IInstance) dynamicSymbol.Parent).IsReference)
            return true;
        }
        return false;
      }
    }

    /// <summary>
    /// Indicates that the <see cref="T:TwinCAT.TypeSystem.IInstance" /> represents a Pointer type (Pointer TO)
    /// </summary>
    /// <value><c>true</c> if is ReferenceTo, otherwise <c>false</c>.</value>
    public bool IsPointer
    {
      get
      {
        bool isPointer = false;
        if (this.DataType != null)
          isPointer = this.DataType.IsPointer;
        else if (!(this is IVirtualStructInstance))
          isPointer = DataTypeStringParser.IsPointer(this.TypeName);
        return isPointer;
      }
    }

    /// <summary>
    /// Gets the comment of the <see cref="T:TwinCAT.TypeSystem.IInstance" />
    /// </summary>
    /// <value>The comment.</value>
    public string Comment => ((IInstance) this.symbol).Comment;

    /// <summary>Gets the data type binder.</summary>
    /// <value>The data type binder.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IBinder? Binder => this.symbol is IBinderProvider symbol ? symbol.Binder : (IBinder) null;

    /// <summary>Gets the context mask.</summary>
    /// <value>The context mask.</value>
    byte IContextMaskProvider.ContextMask => this.symbol is IAdsSymbol symbol ? ((IContextMaskProvider) symbol).ContextMask : (byte) 0;

    /// <summary>Gets the value accessor.</summary>
    /// <value>The value accessor.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IAccessorRawValue? ValueAccessor => !(this.symbol is IValueAccessorProvider symbol) ? (IAccessorRawValue) null : symbol.ValueAccessor;

    /// <summary>
    /// Gets the connection bound to this <see cref="T:TwinCAT.TypeSystem.DynamicSymbol" />
    /// </summary>
    /// <value>The connection.</value>
    public IConnection? Connection => this.ValueAccessor is IAccessorConnection valueAccessor ? valueAccessor.Connection : (IConnection) null;

    /// <summary>
    /// Gets a value indicating whether this instance is recursive.
    /// </summary>
    /// <value><c>true</c> if this instance is recursive; otherwise, <c>false</c>.</value>
    public bool IsRecursive => ((ISymbol) this.symbol).IsRecursive;

    /// <summary>Gets the value encoding.</summary>
    /// <value>The value encoding.</value>
    public Encoding ValueEncoding => ((IAttributedInstance) this._InnerSymbol).ValueEncoding;

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see> into a new created instance of the managed type
    /// </summary>
    /// <param name="managedType">The tp.</param>
    /// <returns>Read value (System.Object).</returns>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.WriteAnyValue(System.Object)" />
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.UpdateAnyValue(System.Object@)" />
    public object ReadAnyValue(Type managedType) => this.OnReadAnyValue(managedType);

    /// <summary>Handler function for reading ADS 'Any' Values.</summary>
    /// <param name="managedType">Managed type to read.</param>
    /// <returns>System.Object.</returns>
    protected virtual object OnReadAnyValue(Type managedType)
    {
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!(this.ValueAccessor is IAccessorValueAny valueAccessor))
        throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support IValueAnyAccessor", (object) this.ValueAccessor), this.ValueAccessor);
      object obj;
      DateTimeOffset? nullable;
      int num = valueAccessor.TryReadAnyValue((ISymbol) this, managedType, ref obj, ref nullable);
      if (num != 0)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot read (any) of Symbol '{0}'! Error: {1}", (object) this.InstancePath, (object) num), (ISymbol) this.symbol);
      return obj;
    }

    /// <summary>
    /// Reads the value of this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see> into the specified managed value.
    /// </summary>
    /// <param name="valueObject">The managed object.</param>
    /// <returns>Read value (System.Object).</returns>
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.ReadAnyValue(System.Type)" />
    /// <seealso cref="M:TwinCAT.TypeSystem.IValueAnySymbol.WriteAnyValue(System.Object)" />
    public void UpdateAnyValue(ref object valueObject) => valueObject = this.OnUpdateAnyValue(valueObject);

    /// <summary>Called when [update any value].</summary>
    /// <param name="valueObject">The value object.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException"></exception>
    private object OnUpdateAnyValue(object valueObject)
    {
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!(this.ValueAccessor is IAccessorValueAny valueAccessor))
        throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support IValueAnyAccessor", (object) this.ValueAccessor), this.ValueAccessor);
      DateTimeOffset? nullable;
      int num = valueAccessor.TryUpdateAnyValue((ISymbol) this, ref valueObject, ref nullable);
      if (num != 0)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot read (any) of Symbol '{0}'! Error: {1}", (object) this.InstancePath, (object) num), (ISymbol) this);
      return valueObject;
    }

    /// <summary>
    /// Writes the value represented by the managed value to this <see cref="T:TwinCAT.TypeSystem.IValueSymbol">Value</see>
    /// </summary>
    /// <param name="managedValue">The managed value.</param>
    /// <seealso cref="M:TwinCAT.TypeSystem.DynamicSymbol.ReadAnyValue(System.Type)" />
    /// <seealso cref="M:TwinCAT.TypeSystem.DynamicSymbol.UpdateAnyValue(System.Object@)" />
    public void WriteAnyValue(object managedValue) => this.OnWriteAnyValue(managedValue);

    /// <summary>Called when [write any value].</summary>
    /// <param name="managedValue">The managed value.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotAccessVirtualSymbolException"></exception>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException"></exception>
    private void OnWriteAnyValue(object managedValue)
    {
      if (!this.HasValue)
        throw new CannotAccessVirtualSymbolException((ISymbol) this);
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException((ISymbol) this);
      if (!(this.ValueAccessor is IAccessorValueAny valueAccessor))
        throw new ValueAccessorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Accessor '{0}' doesn't support IValueAnyAccessor", (object) this.ValueAccessor), this.ValueAccessor);
      DateTimeOffset? nullable;
      int num = valueAccessor.TryWriteAnyValue((ISymbol) this, managedValue, ref nullable);
      if (num != 0)
        throw new SymbolException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot write (any) of Symbol '{0}'! Error: {1}", (object) this.InstancePath, (object) num), (ISymbol) this);
    }

    /// <summary>
    /// Called when the RawValue of the symbol has been changed.
    /// </summary>
    /// <param name="args">The arguments.</param>
    void ISymbolValueChangeNotify.OnRawValueChanged(
      RawValueChangedEventArgs args)
    {
      if (this._rawValueChanged == null)
        return;
      this._rawValueChanged((object) this, args);
    }

    /// <summary>Called when the Value of the symbol has been changed.</summary>
    /// <param name="args">The arguments.</param>
    void ISymbolValueChangeNotify.OnValueChanged(ValueChangedEventArgs args)
    {
      if (this._valueChanged == null)
        return;
      this._valueChanged((object) this, args);
    }

    /// <summary>
    /// Unwraps the DynamicSymbol to its static version (only for internal purposes)
    /// </summary>
    /// <returns>IValueSymbol.</returns>
    /// <value>The unwrap.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IValueSymbol Unwrap() => this.symbol;
  }
}
