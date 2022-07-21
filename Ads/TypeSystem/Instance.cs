// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.Instance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Instance implementation</summary>
  [DebuggerDisplay("InstancePath = { instanceName }, Type = {typeName}, Size = {size}, Category = {category}, Static = { staticAddress }")]
  public class Instance : 
    IInstance,
    IBitSize,
    ISymbolFlagProvider,
    IBinderProvider,
    IInstanceInternal,
    IResolvableInstanceType,
    IBindable2,
    IBindable
  {
    /// <summary>DataType resolver</summary>
    internal IBinder? resolver;
    /// <summary>Namespace</summary>
    /// <exclude />
    private string ns = string.Empty;
    /// <summary>The Byte size or Bit Size of the instance</summary>
    /// <exclude />
    private int size;
    /// <summary>Get the Instance Flags</summary>
    /// <exclude />
    private AdsSymbolFlags flags;
    /// <summary>Instance category</summary>
    /// <exclude />
    private AdsDataTypeId dataTypeId;
    /// <summary>Instance category</summary>
    /// <exclude />
    private DataTypeCategory category;
    /// <summary>Datatype name.</summary>
    /// <exclude />
    private string typeName = string.Empty;
    /// <summary>Resolved / Cached Data Type</summary>
    /// <exclude />
    private IDataType? resolvedDataType;
    /// <summary>Instance comment.</summary>
    /// <exclude />
    private string comment = string.Empty;
    /// <summary>Name of the instance.</summary>
    /// <exclude />
    private string instanceName = string.Empty;
    /// <summary>The static address</summary>
    /// <exclude />
    private bool staticAddress;
    /// <summary>The attributes</summary>
    private TypeAttributeCollection? attributes;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Instance" /> class.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">resolver</exception>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Instance() => this.flags = (AdsSymbolFlags) 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Instance" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Instance(AdsSymbolEntry symbol)
    {
      this.comment = symbol.Comment;
      this.instanceName = symbol.Name;
      this.size = (int) symbol.Size;
      this.typeName = Instance.AlignTypeName(symbol.TypeName);
      this.dataTypeId = symbol.DataTypeId;
      this.flags = symbol.Flags;
      if (symbol.AttributeCount <= (ushort) 0 || symbol.Attributes == null)
        return;
      this.attributes = new TypeAttributeCollection((IEnumerable<ITypeAttribute>) symbol.Attributes);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Instance" /> class.
    /// </summary>
    /// <param name="subEntry">The sub entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Instance(AdsFieldEntry subEntry)
    {
      this.instanceName = subEntry != null ? subEntry.EntryName : throw new ArgumentNullException(nameof (subEntry));
      this.size = subEntry.Size;
      this.typeName = Instance.AlignTypeName(subEntry.TypeName);
      this.dataTypeId = subEntry.BaseTypeId;
      this.flags = SubItemFlagConverter.Convert(subEntry.Flags);
      this.staticAddress = (subEntry.Flags & 131072) > 0;
      this.comment = subEntry.Comment;
      if (!subEntry.HasAttributes || subEntry.Attributes == null)
        return;
      this.attributes = new TypeAttributeCollection((IEnumerable<ITypeAttribute>) subEntry.Attributes);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Instance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="attributes">The attributes.</param>
    /// <param name="comment">The comment.</param>
    protected internal Instance(
      string instanceName,
      TwinCAT.Ads.TypeSystem.DataType type,
      AdsSymbolFlags flags,
      TypeAttributeCollection? attributes,
      string comment)
    {
      this.instanceName = instanceName;
      this.size = type.Size;
      this.category = type.Category;
      this.typeName = type.Name;
      this.dataTypeId = type.DataTypeId;
      this.flags = flags;
      this.staticAddress = ((Enum) (object) flags).HasFlag((Enum) (object) (AdsSymbolFlags) 8192);
      this.comment = comment;
      this.attributes = attributes;
      this.resolvedDataType = (IDataType) type;
    }

    /// <summary>
    /// Binds this bindable object via the specified <see cref="T:TwinCAT.TypeSystem.IBinder" />
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <exception cref="T:System.ArgumentNullException">binder</exception>
    /// <exception cref="T:System.ArgumentException"></exception>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Bind(IBinder binder)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      this.resolver = this.resolver == null ? binder : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Instance '{0}' is already bound!", (object) this.InstancePath));
      this.ns = ((ISymbolProvider) this.resolver.Provider).RootNamespaceName;
      this.OnBound(binder);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is bound.
    /// </summary>
    /// <value><c>true</c> if this instance is bound; otherwise, <c>false</c>.</value>
    /// <exclude />
    public bool IsBound => this.resolver != null;

    /// <summary>
    /// Called when he <see cref="T:TwinCAT.Ads.TypeSystem.Instance" /> is bound via its instance binder.
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnBound(IBinder binder)
    {
    }

    /// <summary>Aligns the type name</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="T:System.ArgumentException">Type name not valid!</exception>
    protected static string AlignTypeName(string typeName)
    {
      bool flag = !string.IsNullOrEmpty(typeName) ? typeName.EndsWith("(VAR_IN_OUT)", StringComparison.OrdinalIgnoreCase) : throw new ArgumentException("Type name not valid!");
      string str = typeName;
      if (flag)
      {
        int length = typeName.IndexOf(' ', StringComparison.OrdinalIgnoreCase);
        str = typeName.Substring(0, length);
      }
      return str;
    }

    /// <summary>Sets a new instance name.</summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <remarks>This can only used as long the Instance is not added to a collection using the type binder classes.</remarks>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IInstanceInternal.SetInstanceName(string instanceName) => this.OnSetInstanceName(instanceName);

    /// <summary>Sets a new InstanceName InstancePath</summary>
    /// <param name="instanceName">Instance name.</param>
    protected virtual void OnSetInstanceName(string instanceName) => this.instanceName = instanceName;

    /// <summary>Gets the data type resolver.</summary>
    /// <value>The data type resolver.</value>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IBinder? Binder => this.resolver;

    /// <summary>Gets the namespace name.</summary>
    /// <value>The namespace.</value>
    public string Namespace => this.ns;

    /// <summary>
    /// Gets the size of the <see cref="T:TwinCAT.TypeSystem.IDataType" /> in bytes or Bits dependant on <see cref="P:TwinCAT.Ads.TypeSystem.Instance.IsBitType" />
    /// </summary>
    /// <value>The size of the bit.</value>
    public int Size
    {
      get => this.OnGetSize();
      protected set => this.size = value;
    }

    /// <summary>Gets the size of the internal.</summary>
    /// <value>The size of the internal.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int InternalSize => this.size;

    /// <summary>
    /// Handler function getting the size of the <see cref="T:TwinCAT.Ads.TypeSystem.Instance" />
    /// </summary>
    /// <returns>System.Int32.</returns>
    protected virtual int OnGetSize() => this.size;

    /// <summary>Gets the instance flags.</summary>
    /// <value>The flags.</value>
    /// <exclude />
    public AdsSymbolFlags Flags
    {
      get => this.flags;
      protected set => this.flags = value;
    }

    /// <summary>
    /// Gets the (aligned) size of of the Type/Instance in Bytes
    /// </summary>
    /// <value>The size of the byte.</value>
    public int ByteSize
    {
      get
      {
        int byteSize;
        if (this.IsBitType)
        {
          byteSize = this.BitSize / 8;
          if (this.BitSize % 8 > 0)
            ++byteSize;
        }
        else
          byteSize = this.Size;
        return byteSize;
      }
    }

    /// <summary>
    /// Indicates that the Size of the Object is Byte aligned (BitSize % 8 == 0)
    /// </summary>
    /// <value><c>true</c> if this instance is byte aligned; otherwise, <c>false</c>.</value>
    public bool IsByteAligned => !this.IsBitType || this.size % 8 == 0;

    /// <summary>
    /// Gets the size of this <see cref="T:TwinCAT.Ads.TypeSystem.Instance" /> in bits.
    /// </summary>
    /// <value>The size of the bit.</value>
    public virtual int BitSize => this.IsBitType ? this.Size : this.Size * 8;

    /// <summary>Only for internal use (obsolete)</summary>
    /// <value>The datatype.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AdsDataTypeId DataTypeId
    {
      get => this.dataTypeId;
      protected set => this.dataTypeId = value;
    }

    /// <summary>
    /// Gets the the <see cref="T:TwinCAT.TypeSystem.DataTypeCategory" /> of the Instance.
    /// </summary>
    /// <value>The category.</value>
    /// <remarks>Corresponds to the <see cref="P:TwinCAT.TypeSystem.IDataType.Category" /></remarks>
    public DataTypeCategory Category
    {
      get
      {
        if (this.category == null)
          this.TryResolveType();
        return this.category;
      }
      protected set => this.category = value;
    }

    /// <summary>
    /// Gets the name of the <see cref="T:TwinCAT.TypeSystem.IDataType">DataType</see> that is used for this <see cref="T:TwinCAT.TypeSystem.IInstance" />.
    /// </summary>
    /// <value>The name of the type.</value>
    public string TypeName
    {
      get => this.typeName;
      protected set => this.typeName = value;
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IDataType" /> of the <see cref="T:TwinCAT.TypeSystem.IInstance" />.
    /// </summary>
    /// <value>The type of the data.</value>
    public IDataType? DataType
    {
      get
      {
        if (this.resolvedDataType == null)
        {
          this.TryResolveType();
          if (this.resolvedDataType != null && ((IBitSize) this.resolvedDataType).ByteSize != this.ByteSize)
          {
            if (this.resolvedDataType is ReferenceType || this.resolvedDataType is PointerType)
            {
              TwinCAT.Ads.TypeSystem.DataType resolvedDataType = (TwinCAT.Ads.TypeSystem.DataType) this.resolvedDataType;
              if (this.ByteSize == 4 || this.ByteSize == 8)
                ((TwinCAT.TypeSystem.Binder) this.resolver).SetPlatformPointerSize(this.ByteSize);
            }
            else if (this.IsBitType)
            {
              if (this.BitSize != ((IBitSize) this.resolvedDataType).BitSize)
                this.size = ((IBitSize) this.resolvedDataType).Size;
            }
            else if (this.ByteSize != ((IBitSize) this.resolvedDataType).ByteSize)
              AdsModule.Trace.TraceWarning("Mismatching Byte size Instance: {0} ({1} bytes), Type: {2} ({3} bytes)", new object[4]
              {
                (object) this.InstancePath,
                (object) this.ByteSize,
                (object) this.resolvedDataType.Name,
                (object) ((IBitSize) this.resolvedDataType).ByteSize
              });
          }
        }
        return this.resolvedDataType;
      }
      protected set => this.resolvedDataType = value;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is resolved.
    /// </summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <value><c>true</c> if this instance is resolved; otherwise, <c>false</c>.</value>
    /// <exclude />
    public bool IsBindingResolved(bool recurse)
    {
      bool flag = false;
      if (this.resolvedDataType != null)
        flag = !recurse || ((IBindable2) this.resolvedDataType).IsBindingResolved(true);
      return flag;
    }

    /// <summary>
    /// Tries to resolve the <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual bool TryResolveType()
    {
      bool flag = false;
      if (this.resolver != null)
        ((IBindable2) this).ResolveWithBinder(false, this.resolver);
      return flag;
    }

    /// <summary>
    /// Tries to resolve the Bindable Symbol/DataType asynchronously.
    /// </summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The DataType/Symbol binder to be used.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exclude />
    async Task<bool> IBindable2.ResolveWithBinderAsync(
      bool recurse,
      IBinder binder,
      CancellationToken cancel)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool resolved = false;
      if (this.resolvedDataType == null && !string.IsNullOrEmpty(this.typeName))
      {
        ResultValue<IDataType> resultValue = await ((IDataTypeResolver) binder).ResolveTypeAsync(this.typeName, cancel).ConfigureAwait(false);
        if (((ResultAds) resultValue).Succeeded && resultValue.Value != null)
        {
          resolved = true;
          this.resolvedDataType = resultValue.Value;
          DataTypeCategory category = this.category;
          this.category = this.resolvedDataType.Category;
        }
      }
      if (recurse && this.resolvedDataType != null)
        resolved = await ((IBindable2) this.resolvedDataType).ResolveWithBinderAsync(recurse, binder, cancel).ConfigureAwait(false);
      return resolved;
    }

    /// <summary>Resolves the Bindable Symbol/DataType synchronously.</summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The binder.</param>
    /// <returns><c>true</c> if the bindable can be resolved, <c>false</c> otherwise.</returns>
    /// <exclude />
    bool IBindable2.ResolveWithBinder(bool recurse, IBinder binder)
    {
      if (binder == null)
        return false;
      bool flag = false;
      if (this.resolvedDataType == null && !string.IsNullOrEmpty(this.typeName))
      {
        IDataType idataType = (IDataType) null;
        flag = ((IDataTypeResolver) binder).TryResolveType(this.typeName, ref idataType);
        if (flag)
        {
          this.resolvedDataType = idataType;
          DataTypeCategory category = this.category;
          this.category = this.resolvedDataType.Category;
        }
      }
      if (recurse && this.resolvedDataType != null)
        ((IBindable2) this.resolvedDataType).ResolveWithBinder(recurse, binder);
      return flag;
    }

    /// <summary>Gets the comment.</summary>
    /// <value>The comment.</value>
    public string Comment => this.comment;

    internal void SetComment(string comment) => this.comment = comment;

    internal void AddDerivedAttributes(IEnumerable<ITypeAttribute> derived)
    {
      if (this.attributes == null)
        this.attributes = new TypeAttributeCollection();
      this.attributes.AddRange(derived);
    }

    /// <summary>Gets the name of the instance (without periods (.)</summary>
    /// <value>The name of the instance.</value>
    public string InstanceName
    {
      get => this.instanceName;
      protected set => this.instanceName = value;
    }

    /// <summary>
    /// Gets the relative / absolute access path to the instance (with periods (.))
    /// </summary>
    /// <value>The instance path.</value>
    /// <remarks>If this path is relative or absolute depends on the context. <see cref="T:TwinCAT.TypeSystem.IMember" /> are using relative paths, <see cref="T:TwinCAT.TypeSystem.ISymbol" />s are using absolute ones.</remarks>
    public virtual string InstancePath => this.instanceName;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.InstancePath;

    /// <summary>
    /// Gets a value indicating whether this instance has a value.
    /// </summary>
    /// <value><c>true</c> if this instance has value; otherwise, <c>false</c>.</value>
    /// <remarks></remarks>
    public virtual bool HasValue => true;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IInstance" /> is static.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is static; otherwise, <c>false</c>.
    /// </value>
    public bool IsStatic
    {
      get => this.staticAddress;
      protected set => this.staticAddress = value;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is not basing on a full DataType but instead of some sort of bit mapping
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is bit mapping; otherwise, <c>false</c>.
    /// </value>
    public bool IsBitType => (this.flags & 2) == 2;

    /// <summary>Indicates that this instance is read only.</summary>
    /// <remarks>
    /// Actually, this Flag is restricted to TcCOM-Objects readonly Parameters. Within the PLC this is used for the ApplicationName and
    /// ProjectName of PLC instances.
    /// Write-Access on these Modules will create an <see cref="F:TwinCAT.Ads.AdsErrorCode.DeviceAccessDenied" /> error.
    /// </remarks>
    public bool IsReadOnly => (this.flags & 32) == 32;

    /// <summary>Indicates that this instance is persistent.</summary>
    public bool IsPersistent => (this.flags & 1) == 1;

    /// <summary>
    /// Indicates that this instance is a TcComInterfacePointer.
    /// </summary>
    public bool IsTcComInterfacePointer => (this.flags & 16) == 16;

    /// <summary>Indicates that this instance has set TypeGuid flag.</summary>
    public bool IsTypeGuid => (this.flags & 8) == 8;

    /// <summary>
    /// Gets a value indicating whether this instance is reference.
    /// </summary>
    /// <value><c>true</c> if this instance is reference; otherwise, <c>false</c>.</value>
    public bool IsReference
    {
      get
      {
        bool isReference = false;
        if (this.resolvedDataType != null)
          isReference = this.resolvedDataType.IsReference;
        else if (!(this is VirtualStructInstance) && !string.IsNullOrEmpty(this.typeName))
          isReference = DataTypeStringParser.IsReference(this.typeName);
        return isReference;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is reference.
    /// </summary>
    /// <value><c>true</c> if this instance is reference; otherwise, <c>false</c>.</value>
    public bool IsPointer
    {
      get
      {
        bool isPointer = false;
        if (this.resolvedDataType != null)
          isPointer = this.resolvedDataType.IsPointer;
        else if (!(this is VirtualStructInstance) && !string.IsNullOrEmpty(this.typeName))
          isPointer = DataTypeStringParser.IsPointer(this.typeName);
        return isPointer;
      }
    }

    /// <summary>Gets the context mask of this instance.</summary>
    /// <remarks>The Size of the internal data is 4-Bit</remarks>
    public byte ContextMask
    {
      get
      {
        // ISSUE: unable to decompile the method.
      }
    }

    /// <summary>Sets the type attributes</summary>
    /// <param name="coll">The attributes.</param>
    protected void SetAttributes(TypeAttributeCollection? coll) => this.attributes = coll;

    /// <summary>Gets the Type Attributes.</summary>
    /// <value>The attributes.</value>
    public ITypeAttributeCollection Attributes => this.attributes != null ? (ITypeAttributeCollection) this.attributes.AsReadOnly() : (ITypeAttributeCollection) new TypeAttributeCollection().AsReadOnly();

    /// <summary>Sets the context mask.</summary>
    /// <param name="contextMask">The context mask.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">contextMask</exception>
    protected void SetContextMask(byte contextMask)
    {
      if (contextMask > (byte) 15)
        throw new ArgumentOutOfRangeException(nameof (contextMask));
      AdsSymbolFlags adsSymbolFlags = (AdsSymbolFlags) ((int) (ushort) ((uint) contextMask << 8) & 3840);
      this.flags = (AdsSymbolFlags) (this.flags & 61695);
      this.flags = this.flags | adsSymbolFlags;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IDataType? ResolveType(DataTypeResolveStrategy type) => this.DataType is IResolvableType dataType ? dataType.ResolveType(type) : this.DataType;
  }
}
