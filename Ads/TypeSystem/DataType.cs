// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.DataType
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
  /// <summary>DataType class</summary>
  [DebuggerDisplay("Name = {Name}, Size = {Size}, Category = {Category}")]
  public class DataType : 
    IDataType,
    IBitSize,
    IManagedMappableType,
    IResolvableType,
    IBindable2,
    IBindable
  {
    private static DataType? _none = (DataType) null;
    /// <summary>The s_id counter</summary>
    private static int s_idCounter = 1;
    /// <summary>Internal ID of the DataType (non persistent)</summary>
    /// <exclude />
    private int id = DataType.s_idCounter++;
    /// <summary>The corresponding .NET type</summary>
    /// <exclude />
    private Type? _managedType;
    /// <summary>The type binder / resolver</summary>
    internal IBinder? resolver;
    /// <summary>The namespace</summary>
    /// <exclude />
    private string ns = string.Empty;
    /// <summary>Data Type Category</summary>
    /// <exclude />
    private DataTypeCategory category;
    /// <summary>Data Type category</summary>
    /// <exclude />
    private AdsDataTypeId dataTypeId;
    /// <summary>
    /// hashValue of base type / Code Offset to setter Method (typeHashValue or offsSetCode)
    /// </summary>
    /// <exclude />
    private uint typeHashValue;
    /// <summary>
    /// The size of this <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> in bytes or bits
    /// </summary>
    /// <exclude />
    private int size;
    /// <summary>
    /// The name of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" />
    /// </summary>
    /// <exclude />
    private string _typeName = string.Empty;
    /// <summary>
    /// Additional comment to the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" />
    /// </summary>
    /// <exclude />
    private string comment = string.Empty;
    /// <summary>Flag indicators set to this type.</summary>
    /// <exclude />
    protected AdsDataTypeFlags flags = (AdsDataTypeFlags) 1;
    /// <summary>The attributes</summary>
    /// <exclude />
    private TypeAttributeCollection attributes = new TypeAttributeCollection();

    internal static DataType None
    {
      get
      {
        if (DataType._none == null)
          DataType._none = new DataType();
        return DataType._none;
      }
    }

    /// <summary>
    /// Prevents a default instance of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> class from being created.
    /// </summary>
    private DataType()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> class.
    /// </summary>
    /// <param name="cat">The cat.</param>
    protected internal DataType(DataTypeCategory cat) => this.category = cat;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> class.
    /// </summary>
    /// <param name="cat">The category</param>
    /// <param name="entry">The entry.</param>
    internal DataType(DataTypeCategory cat, AdsDataTypeEntry entry)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      this.category = cat;
      this.comment = entry.Comment;
      this._typeName = entry.EntryName;
      this.flags = entry.Flags;
      this.size = entry.Size;
      this.dataTypeId = entry.BaseTypeId;
      if (this.dataTypeId == 65 && this.category == null)
        this.category = (DataTypeCategory) 16;
      else if (this.category == null)
        this.category = (DataTypeCategory) 1;
      this.typeHashValue = entry.TypeHashValue;
      if (!entry.HasAttributes)
        return;
      this.attributes = new TypeAttributeCollection((IEnumerable<ITypeAttribute>) entry.Attributes);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> class (Bytes mapping)
    /// </summary>
    /// <param name="name">Name of the Data type</param>
    /// <param name="typeId">DataType category / ID (internal style)</param>
    /// <param name="cat">Data type category</param>
    /// <param name="size">The Size of the Data Type in bytes or Bits</param>
    /// <param name="dotnetType">Associated dot net type.</param>
    internal DataType(
      string name,
      AdsDataTypeId typeId,
      DataTypeCategory cat,
      int size,
      Type? dotnetType)
      : this(name, typeId, cat, size, dotnetType, (AdsDataTypeFlags) 1)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="cat">The cat.</param>
    /// <param name="dotnetType">Type of the dotnet.</param>
    protected internal DataType(string name, DataTypeCategory cat, Type dotnetType)
    {
      this.category = cat;
      this.comment = string.Empty;
      this._typeName = name;
      this.flags = (AdsDataTypeFlags) 1;
      this.size = PrimitiveTypeMarshaler.Default.MarshalSize(dotnetType);
      PrimitiveTypeMarshaler.TryGetDataTypeId(dotnetType, out this.dataTypeId);
      this.category = cat;
      this.typeHashValue = 0U;
      this._managedType = dotnetType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> class.
    /// </summary>
    /// <param name="name">Name of the Data type</param>
    /// <param name="typeId">DataType category / ID (internal style)</param>
    /// <param name="cat">Data type category</param>
    /// <param name="size">The Size of the Data Type in bits or bytes (depending on bitMapping)</param>
    /// <param name="dotnetType">Associated dot net type.</param>
    /// <param name="flags">The flags.</param>
    internal DataType(
      string name,
      AdsDataTypeId typeId,
      DataTypeCategory cat,
      int size,
      Type? dotnetType,
      AdsDataTypeFlags flags)
      : this()
    {
      this._typeName = name;
      this.dataTypeId = typeId;
      this.category = cat;
      this.flags = (AdsDataTypeFlags) (flags | 1);
      this.size = size;
      this._managedType = dotnetType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> class (copy Constructor)
    /// </summary>
    /// <param name="copy">The copy.</param>
    protected DataType(DataType copy)
    {
      this._typeName = copy != null ? copy.Name : throw new ArgumentNullException(nameof (copy));
      this.dataTypeId = copy.dataTypeId;
      this.category = copy.Category;
      this.size = copy.size;
      this._managedType = copy._managedType;
      this.flags = copy.flags;
      this.ns = copy.ns;
      this.resolver = copy.resolver;
    }

    /// <summary>Gets the ID of the DataType</summary>
    /// <value>The id.</value>
    public int Id => this.id;

    /// <summary>Gets the corresponding .NET Type if attached.</summary>
    /// <value>Dot net type.</value>
    public virtual Type? ManagedType
    {
      get => this._managedType;
      protected set => this._managedType = value;
    }

    /// <summary>
    /// Gets the namespace string within the <see cref="T:TwinCAT.TypeSystem.IDataType" /> exists.
    /// </summary>
    /// <value>The namespace.</value>
    public string Namespace
    {
      get => this.ns;
      protected set => this.ns = value;
    }

    /// <summary>Gets the Data Type category</summary>
    /// <value>The category.</value>
    public DataTypeCategory Category
    {
      get => this.category;
      protected set => this.category = value;
    }

    /// <summary>
    /// Gets the hashValue of base type / Code Offset to setter Method (typeHashValue or offsSetCode)
    /// </summary>
    /// <value>The data type id.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public uint TypeHashValue => this.typeHashValue;

    /// <summary>
    /// Gets the DataTypeId <see cref="T:TwinCAT.Ads.AdsDataTypeId" /> (Only for internal use)
    /// </summary>
    /// <value>The data type id.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AdsDataTypeId DataTypeId
    {
      get => this.dataTypeId;
      protected set => this.dataTypeId = value;
    }

    /// <summary>
    /// Gets the Size of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> in Bytes or bits.
    /// </summary>
    /// <value>The size.</value>
    public int Size
    {
      get
      {
        if (this.size <= 0 && (this.Category == 13 || this.Category == 15) && this.resolver != null && ((IDataTypeResolver) this.resolver).PlatformPointerSize > 0)
          this.size = ((IDataTypeResolver) this.resolver).PlatformPointerSize;
        return this.size;
      }
      protected set => this.size = value;
    }

    /// <summary>
    /// Sets the size of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" />
    /// </summary>
    /// <param name="size">The size.</param>
    /// <param name="managedType">Type of the managed.</param>
    internal void SetSize(int size, Type? managedType)
    {
      this.size = size;
      this._managedType = managedType;
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
          byteSize = this.size;
        return byteSize;
      }
    }

    /// <summary>
    /// Indicates that the Size of the Object is Byte aligned (BitSize % 8 == 0)
    /// </summary>
    /// <value><c>true</c> if this instance is byte aligned; otherwise, <c>false</c>.</value>
    public bool IsByteAligned => !this.IsBitType || this.size % 8 == 0;

    /// <summary>
    /// Gets the size of the <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> in bits.
    /// </summary>
    /// <value>The size of the bit.</value>
    public int BitSize => this.IsBitType ? this.size : this.size * 8;

    /// <summary>Gets the name of the Data Type (without namespace)</summary>
    /// <value>The name.</value>
    public string Name
    {
      get => this._typeName;
      protected set => this._typeName = value;
    }

    /// <summary>
    /// Gets the full name of the <see cref="T:TwinCAT.TypeSystem.IDataType" /> (Namespace + Name)
    /// </summary>
    /// <value>The full name.</value>
    public string FullName => this.ns + "." + this._typeName;

    /// <summary>Gets the comment.</summary>
    /// <value>The comment.</value>
    public string Comment => this.comment;

    /// <summary>Resolves the type.</summary>
    /// <param name="type">The type.</param>
    /// <returns>IDataType.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IDataType ResolveType(DataTypeResolveStrategy type)
    {
      IDataType idataType1 = (IDataType) this;
      IDataType idataType2 = (IDataType) this;
      if (type == null)
      {
        IAliasType ialiasType;
        for (; idataType2 != null && idataType2.Category == 2; idataType2 = ialiasType.BaseType)
        {
          ialiasType = (IAliasType) idataType2;
          idataType1 = idataType2;
        }
        if (idataType2 == null && this.Category == 2)
          AdsModule.Trace.TraceWarning("Could not resolve Alias '{0}'. Base type: '{1}' not found!", new object[2]
          {
            (object) idataType1.Name,
            (object) ((IAliasType) idataType1).BaseTypeName
          });
      }
      else if (type == 1)
      {
        while (idataType2 != null && (idataType2.Category == 2 || idataType2.Category == 15))
        {
          if (idataType2.Category == 2)
          {
            IAliasType ialiasType = (IAliasType) idataType2;
            idataType1 = idataType2;
            idataType2 = ialiasType.BaseType;
          }
          else if (idataType2.Category == 15)
          {
            IReferenceType ireferenceType = (IReferenceType) idataType2;
            idataType1 = idataType2;
            idataType2 = ireferenceType.ReferencedType;
          }
        }
        if (idataType2 == null)
        {
          if (this.Category == 2)
            AdsModule.Trace.TraceWarning("Could not resolve Alias '{0}'. Base type '{1}' not found!", new object[2]
            {
              (object) idataType1.Name,
              (object) ((IAliasType) idataType1).BaseTypeName
            });
          else if (this.Category == 15)
            AdsModule.Trace.TraceWarning("Could not resolve Reference '{0}'. Referenced type not found!", new object[1]
            {
              (object) idataType1.Name
            });
        }
      }
      return idataType2;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
    public override string ToString() => this.Name;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is primitive
    /// </summary>
    /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
    public virtual bool IsPrimitive => PrimitiveTypeMarshaler.IsPrimitiveType(this.category);

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is a container type
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    /// <seealso cref="P:TwinCAT.TypeSystem.IDataType.Category" />
    /// <remarks>Container Types are all types that contain SubElements like
    /// <list type="bullet"><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Array" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Pointer" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Union" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Struct" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Function" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.FunctionBlock" /></description></item><item><description><see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Program" /></description></item></list>
    /// and the <see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Alias" /> and <see cref="F:TwinCAT.TypeSystem.DataTypeCategory.Reference" /> types, if they have a container type as base type.</remarks>
    public virtual bool IsContainer => PrimitiveTypeMarshaler.IsContainerType(this.category);

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is a reference type
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    /// <seealso cref="P:TwinCAT.TypeSystem.IDataType.Category" />
    /// <remarks>Reference types can be dereferenced.</remarks>
    public virtual bool IsReference => DataType.IsReferenceType(this.category);

    /// <summary>
    /// Determines whether the specified category is a reference type.
    /// </summary>
    /// <param name="cat">The data type category.</param>
    /// <returns><c>true</c> if [is reference type] [the specified cat]; otherwise, <c>false</c>.</returns>
    public static bool IsReferenceType(DataTypeCategory cat) => cat == 15;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is a pointer type
    /// </summary>
    /// <value><c>true</c> if this instance is pointer type; otherwise, <c>false</c>.</value>
    /// <seealso cref="P:TwinCAT.TypeSystem.IDataType.Category" />
    /// <remarks>Pointer types can be dereferenced with the '^' operator.</remarks>
    public virtual bool IsPointer => DataType.IsPointerType(this.category);

    /// <summary>
    /// Determines whether the specified category is a pointer type.
    /// </summary>
    /// <param name="cat">The data type category.</param>
    /// <returns><c>true</c> if [is pointer type] [the specified cat]; otherwise, <c>false</c>.</returns>
    public static bool IsPointerType(DataTypeCategory cat) => cat == 13;

    /// <summary>Gets the Flag indicators set to this type.</summary>
    /// <value>The flags.</value>
    /// <exclude />
    public AdsDataTypeFlags Flags => this.flags;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is a bit mapping Type
    /// </summary>
    /// <value><c>true</c> if this instance is bit mapping subtype; otherwise, <c>false</c>.</value>
    public bool IsBitType => (this.flags & 32) == 32;

    /// <summary>
    /// Gets the attributes of the <see cref="T:TwinCAT.TypeSystem.IDataType" />
    /// </summary>
    /// <value>The attributes.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public ITypeAttributeCollection Attributes => (ITypeAttributeCollection) this.attributes.AsReadOnly();

    /// <summary>
    /// Registers this instance at the <see cref="T:TwinCAT.TypeSystem.IBinder" />
    /// </summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Bind(IBinder binder)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      this.resolver = this.resolver == null ? binder : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is already bound to namespace!", (object) this.FullName));
      this.ns = ((ISymbolProvider) this.resolver.Provider).RootNamespaceName;
      this.OnBound(binder);
    }

    /// <summary>
    /// Called when this <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> is bound via the type binder.
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnBound(IBinder binder)
    {
    }

    /// <summary>
    /// Gets a value indicating whether this instance is resolved.
    /// </summary>
    /// <returns><c>true</c> if [is type resolved] [the specified recurse]; otherwise, <c>false</c>.</returns>
    /// <value><c>true</c> if this instance is resolved; otherwise, <c>false</c>.</value>
    /// <exclude />
    public bool IsBound => this.resolver != null;

    /// <summary>
    /// Gets a value indicating whether this instance is resolved.
    /// </summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <value><c>true</c> if this instance is resolved; otherwise, <c>false</c>.</value>
    /// <exclude />
    public virtual bool IsBindingResolved(bool recurse) => true;

    /// <summary>
    /// Tries to resolve the Bindable Symbol/DataType asynchronously.
    /// </summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The DataType/Symbol binder to be used.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exclude />
    Task<bool> IBindable2.ResolveWithBinderAsync(
      bool recurse,
      IBinder binder,
      CancellationToken cancel)
    {
      return this.OnResolveWithBinderAsync(recurse, binder, cancel);
    }

    /// <summary>
    /// Handler function resolving the DataType binding asynchronously.
    /// </summary>
    /// <param name="recurse">if set to this method resolves all subtypes recursivly.</param>
    /// <param name="binder">The binder.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exclude />
    protected virtual Task<bool> OnResolveWithBinderAsync(
      bool recurse,
      IBinder binder,
      CancellationToken cancel)
    {
      return Task.FromResult<bool>(true);
    }

    /// <summary>Resolves the Bindable Symbol/DataType synchronously.</summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The binder.</param>
    /// <returns><c>true</c> if the bindable can be resolved, <c>false</c> otherwise.</returns>
    /// <exclude />
    bool IBindable2.ResolveWithBinder(bool recurse, IBinder binder) => this.OnResolveWithBinder(recurse, binder);

    /// <summary>Handler function resolving the DataType binding</summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The binder.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exclude />
    protected virtual bool OnResolveWithBinder(bool recurse, IBinder binder) => true;
  }
}
