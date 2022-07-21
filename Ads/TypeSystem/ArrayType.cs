// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.ArrayType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Represents an Array DataType</summary>
  /// <summary>DataType class</summary>
  [DebuggerDisplay("Name = {Name}, Size = {Size}, Category = {Category}, Elements = { ElementCount }")]
  public class ArrayType : DataType, IArrayType, IDataType, IBitSize, IOversamplingSupport
  {
    private bool _oversampled;
    /// <summary>The element type name</summary>
    /// <exclude />
    private string elementTypeName = string.Empty;
    /// <summary>The element type</summary>
    /// <exclude />
    private DataType? elementType;
    /// <summary>The element type id</summary>
    /// <exclude />
    private AdsDataTypeId elementTypeId;
    /// <summary>Dimension information (for arrays)</summary>
    private DimensionCollection _dimensions;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal ArrayType(AdsDataTypeEntry entry)
      : base((DataTypeCategory) 4, entry)
    {
      AdsDataTypeArrayInfo[] arrayInfos = entry.ArrayInfos;
      DimensionCollection dims1 = (DimensionCollection) null;
      DimensionCollection dims2 = (DimensionCollection) null;
      string baseType = (string) null;
      if (DataTypeStringParser.TryParseArray(this.Name, out dims1, out baseType) && DataTypeStringParser.TryParseArray(baseType, out dims2, out string _))
      {
        this._dimensions = dims1;
        this.elementTypeName = baseType;
        this.elementTypeId = entry.BaseTypeId;
      }
      else
      {
        this._dimensions = DimensionConverter.ToDimensionCollection(arrayInfos);
        this.elementTypeName = entry.TypeName;
        this.elementTypeId = entry.BaseTypeId;
      }
      this._oversampled = (entry.Flags & 16) == 16;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" /> class.
    /// </summary>
    /// <param name="typeName">ArrayType name.</param>
    /// <param name="elementType">Element Type.</param>
    /// <param name="dims">Dimension specification.</param>
    /// <param name="flags">The flags.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal ArrayType(
      string typeName,
      IDataType elementType,
      DimensionCollection dims,
      AdsDataTypeFlags flags)
      : base(typeName, (AdsDataTypeId) 65, (DataTypeCategory) 4, dims.ElementCount * ((IBitSize) elementType).ByteSize, (Type) null, flags)
    {
      this.elementType = elementType != null ? (DataType) elementType : throw new ArgumentNullException(nameof (elementType));
      this.elementTypeName = elementType.Name;
      this._dimensions = dims;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" /> class.
    /// </summary>
    /// <param name="elementType">Element Type.</param>
    /// <param name="dims">Dimension specification.</param>
    /// <exception cref="T:System.ArgumentNullException">elementType</exception>
    internal ArrayType(IDataType elementType, DimensionCollection dims)
      : base("ARRAY OF " + elementType.Name, (AdsDataTypeId) 65, (DataTypeCategory) 4, dims.ElementCount * ((IBitSize) elementType).ByteSize, (Type) null, (AdsDataTypeFlags) 0)
    {
      this.elementType = elementType != null ? (DataType) elementType : throw new ArgumentNullException(nameof (elementType));
      this.elementTypeName = elementType.Name;
      this._dimensions = dims;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" /> class.
    /// </summary>
    /// <param name="typeName">ArrayType name.</param>
    /// <param name="elementType">Element Type.</param>
    /// <param name="dims">Dimension specification.</param>
    /// <param name="flags">The flags.</param>
    internal ArrayType(
      string typeName,
      string elementType,
      DimensionCollection dims,
      AdsDataTypeFlags flags)
      : base(typeName, (AdsDataTypeId) 65, (DataTypeCategory) 4, 0, (Type) null, flags)
    {
      if (string.IsNullOrEmpty(elementType))
        throw new ArgumentNullException(nameof (elementType));
      this.elementType = (DataType) null;
      this.elementTypeName = elementType;
      this._dimensions = dims;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal ArrayType(ArrayType type)
      : this(type.Name, (IDataType) type.elementType, type._dimensions, type.Flags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" /> class.
    /// </summary>
    /// <param name="elementType">Type of the element.</param>
    /// <param name="dimensions">The dimensions.</param>
    public ArrayType(DataType elementType, IDimensionCollection dimensions)
      : base((DataTypeCategory) 4)
    {
      this.elementType = elementType;
      this.elementTypeName = elementType.Name;
      this._dimensions = new DimensionCollection((IEnumerable<IDimension>) dimensions);
      this.Name = ArrayType.CreateName(elementType.Name, dimensions);
      this.SetSize(dimensions.ElementCount * elementType.Size, (Type) null);
    }

    private static string CreateName(string elementName, IDimensionCollection dimensions) => "ARRAY " + DimensionCollectionConverter.DimensionsToString(dimensions) + " OF " + elementName;

    /// <summary>Gets the corresponding .NET Type if attached.</summary>
    /// <value>Dot net type.</value>
    public override Type? ManagedType
    {
      get
      {
        if (base.ManagedType == (Type) null)
        {
          IDataType elementType = this.ElementType;
          if (elementType != null)
          {
            Type managedType = ((DataType) elementType).ManagedType;
            if (managedType != (Type) null)
            {
              if (this.DimensionCount > 1)
                this.ManagedType = managedType.MakeArrayType(this.DimensionCount);
              else
                this.ManagedType = managedType.MakeArrayType();
            }
          }
        }
        return base.ManagedType;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this array instance describes an oversampling type.
    /// </summary>
    /// <value><c>true</c> if this instance is oversampling; otherwise, <c>false</c>.</value>
    public bool IsOversampled => this._oversampled;

    /// <summary>Gets the name of the element type.</summary>
    /// <value>The name of the element type.</value>
    public string ElementTypeName => this.elementTypeName;

    /// <summary>Sets the type of the element.</summary>
    /// <param name="elementType">Type of the element.</param>
    protected void SetElementType(DataType elementType) => this.elementType = elementType;

    /// <summary>Gets the type of the contained elements.</summary>
    /// <value>The type of the element.</value>
    public IDataType? ElementType
    {
      get
      {
        if (!this.IsBindingResolved(false))
        {
          bool flag = false;
          if (this.resolver != null)
            flag = ((IBindable2) this).ResolveWithBinder(false, this.resolver);
          int num = flag ? 1 : 0;
        }
        return (IDataType) this.elementType;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is resolved.
    /// </summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <value><c>true</c> if this instance is resolved; otherwise, <c>false</c>.</value>
    /// <exclude />
    public override bool IsBindingResolved(bool recurse)
    {
      bool flag = false;
      if (this.elementType != null)
        flag = !recurse || ((IBindable2) this.elementType).IsBindingResolved(true);
      return flag;
    }

    /// <summary>Handler function resolving the DataType binding</summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The binder.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exclude />
    protected override bool OnResolveWithBinder(bool recurse, IBinder binder)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool flag = true;
      if (!this.IsBindingResolved(recurse))
      {
        if (this.elementType == null)
        {
          IDataType idataType;
          flag = ((IDataTypeResolver) binder).TryResolveType(this.elementTypeName, ref idataType);
          this.elementType = (DataType) idataType;
        }
        if (this.elementType != null & recurse)
          flag = ((IBindable2) this.elementType).ResolveWithBinder(recurse, binder);
      }
      return flag;
    }

    /// <summary>
    /// Handler function resolving the DataType binding asynchronously.
    /// </summary>
    /// <param name="recurse">if set to this method resolves all subtypes recursivly.</param>
    /// <param name="binder">The binder.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exclude />
    protected override async Task<bool> OnResolveWithBinderAsync(
      bool recurse,
      IBinder binder,
      CancellationToken cancel)
    {
      ArrayType arrayType = this;
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool ret = true;
      if (!arrayType.IsBindingResolved(recurse))
      {
        if (arrayType.elementType == null)
        {
          ResultValue<IDataType> resultValue = await ((IDataTypeResolver) binder).ResolveTypeAsync(arrayType.elementTypeName, cancel).ConfigureAwait(false);
          arrayType.elementType = (DataType) resultValue.Value;
        }
        if (arrayType.elementType != null & recurse)
          ret = await ((IBindable2) arrayType.elementType).ResolveWithBinderAsync(recurse, binder, cancel).ConfigureAwait(false);
      }
      return ret;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is jagged.
    /// </summary>
    /// <value><c>true</c> if this instance is jagged; otherwise, <c>false</c>.</value>
    public bool IsJagged => this.JaggedLevel > 1;

    /// <summary>Gets the jagged level (Non-Jagged Array have level 1)</summary>
    /// <value>The jagged level.</value>
    public int JaggedLevel
    {
      get
      {
        int jaggedLevel = 1;
        for (IArrayType iarrayType = (IArrayType) this; iarrayType.ElementType != null && iarrayType.ElementType.Category == 4; iarrayType = (IArrayType) iarrayType.ElementType)
          ++jaggedLevel;
        return jaggedLevel;
      }
    }

    [Obsolete]
    internal AdsDataTypeId ElementTypeId => this.elementTypeId;

    /// <summary>Sets the dimensions.</summary>
    /// <param name="dims">The dims.</param>
    protected void SetDimensions(DimensionCollection dims) => this._dimensions = dims;

    /// <summary>Gets the dimensions as read only collection.</summary>
    /// <value>The dimensions.</value>
    public IDimensionCollection Dimensions => (IDimensionCollection) this._dimensions.AsReadOnly();

    /// <summary>Gets the dimension count.</summary>
    /// <value>The dimension count.</value>
    public int DimensionCount => this._dimensions.Count;

    /// <summary>Gets the element count.</summary>
    /// <value>The element count.</value>
    public int ElementCount => this._dimensions.ElementCount;

    /// <summary>Gets the byte-size of a single element of the array</summary>
    /// <value>The size of the element.</value>
    public int ElementSize
    {
      get
      {
        IDataType elementType = this.ElementType;
        return elementType == null ? this.Size / this.Dimensions.ElementCount : ((IBitSize) elementType).Size;
      }
    }

    /// <summary>
    /// Checks the dimensions of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" />
    /// </summary>
    /// <param name="indices">Indices</param>
    /// <param name="arrayType">ArrayType</param>
    /// <param name="acceptOversampled">if set to <c>true</c> [accept oversampled].</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Indices doesn't match the specified Array type</exception>
    internal static void CheckIndices(int[] indices, IArrayType arrayType, bool acceptOversampled)
    {
      bool flag = ((!(arrayType is IOversamplingSupport ioversamplingSupport) ? 0 : (ioversamplingSupport.IsOversampled ? 1 : 0)) & (acceptOversampled ? 1 : 0)) != 0;
      ArrayIndexConverter.CheckIndices(indices, arrayType.Dimensions.LowerBounds, arrayType.Dimensions.UpperBounds, false, acceptOversampled);
    }

    /// <summary>
    /// Checks the dimensions of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" />
    /// </summary>
    /// <param name="indices">Indices</param>
    /// <param name="arrayType">ArrayType</param>
    /// <param name="acceptOversampled">if set to <c>true</c> [accept oversampled].</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Indices doesn't match the specified Array type</exception>
    internal static bool TryCheckIndices(
      int[] indices,
      IArrayType arrayType,
      bool acceptOversampled)
    {
      bool flag = ((!(arrayType is IOversamplingSupport ioversamplingSupport) ? 0 : (ioversamplingSupport.IsOversampled ? 1 : 0)) & (acceptOversampled ? 1 : 0)) != 0;
      return ArrayIndexConverter.TryCheckIndices(indices, arrayType.Dimensions.LowerBounds, arrayType.Dimensions.UpperBounds, false, acceptOversampled);
    }

    /// <summary>
    /// Checks the dimensions of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" />
    /// </summary>
    /// <param name="indices">Indices</param>
    /// <param name="type">ArrayType</param>
    /// <param name="acceptOversampled">if set to <c>true</c> [accept oversampled].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal static bool AreIndicesValid(int[] indices, IArrayType type, bool acceptOversampled)
    {
      bool oversampled = ((!(type is IOversamplingSupport ioversamplingSupport) ? 0 : (ioversamplingSupport.IsOversampled ? 1 : 0)) & (acceptOversampled ? 1 : 0)) != 0;
      return ArrayIndexConverter.TryCheckIndices(indices, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, false, oversampled);
    }

    /// <summary>
    /// Checks the dimensions of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" />
    /// </summary>
    /// <param name="indices">Indices</param>
    /// <param name="acceptOversampled">if set to <c>true</c> [accept oversampled].</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Indices doesn't match the specified Array type</exception>
    internal void CheckIndices(int[] indices, bool acceptOversampled) => ArrayType.CheckIndices(indices, (IArrayType) this, acceptOversampled);

    /// <summary>
    /// Gets the element position within a flattened multidimensional array / SubSymbols List
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="type">The type.</param>
    /// <returns>The Position-Index within the Elements collection of the Array instance.</returns>
    internal static int GetElementPosition(int[] indices, IArrayType type)
    {
      ArrayType.CheckIndices(indices, type, false);
      return ArrayIndexConverter.IndicesToSubIndex(indices, type);
    }

    /// <summary>
    /// Gets the element position within a flattened multidimensional array
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <returns>
    /// The Position-Index within the Elements collection of the array
    /// </returns>
    internal int GetElementPosition(int[] indices) => ArrayType.GetElementPosition(indices, (IArrayType) this);

    internal int GetElementOffset(int[] indices) => ArrayType.GetElementOffset(indices, (IArrayType) this);

    internal static bool IsOversamplingIndex(int[] indices, IArrayType type) => ArrayIndexConverter.IsOversamplingIndex(indices, type);

    /// <summary>
    /// Gets the element offset (bits or bytes, dependent on ElementType.IsBitType)
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    internal static int GetElementOffset(int[] indices, IArrayType type)
    {
      int elementPosition = ArrayType.GetElementPosition(indices, type);
      return (type.ElementType == null ? ((IBitSize) type).Size / type.Dimensions.ElementCount : ((IBitSize) type.ElementType).Size) * elementPosition;
    }

    internal int[] GetIndicesOfPosition(int position) => ArrayIndexConverter.SubIndexToIndices(position, (IArrayType) this);

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IDataType" /> is primitive
    /// </summary>
    /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
    public override bool IsPrimitive => false;
  }
}
