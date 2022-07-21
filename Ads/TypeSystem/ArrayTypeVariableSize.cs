// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.ArrayTypeVariableSize
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Variable / Dynamic / AnySize Array Type.
  /// Implements the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayType" />
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.ArrayType" />
  /// <exclude />
  internal class ArrayTypeVariableSize : ArrayType
  {
    /// <summary>Root dynamic instance.</summary>
    private IArrayInstanceVariableSize _rootDynamic;
    /// <summary>
    /// Offset within the DynamicDimLengths (necessary for jagged arrays)
    /// </summary>
    private int _dynamicParameterOffset;
    /// <summary>Number of used dynamic Dimensions</summary>
    private int _dynamicParameterCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayTypeVariableSize" /> class.
    /// </summary>
    /// <param name="type">The dynamic/original type.</param>
    /// <param name="dynamicParameterOffset">The dynamic parameter offset.</param>
    /// <param name="rootDynamic">The root dynamic instance.</param>
    internal ArrayTypeVariableSize(
      ArrayType type,
      int dynamicParameterOffset,
      IArrayInstanceVariableSize rootDynamic)
      : base(type)
    {
      this._rootDynamic = rootDynamic;
      this._dynamicParameterOffset = dynamicParameterOffset;
      this.SetDimensions(((ReadOnlyDimensionCollection) type.Dimensions).FillDynamicDimensions(dynamicParameterOffset, rootDynamic.DynamicDimLengths, out this._dynamicParameterCount));
      IDataType idataType = type.ElementType;
      if (idataType == null)
        throw new CannotResolveDataTypeException(type.ElementTypeName);
      if (idataType.Category == 4)
      {
        idataType = (IDataType) new ArrayTypeVariableSize((ArrayType) idataType, this._dynamicParameterOffset + this._dynamicParameterCount, rootDynamic);
        this.SetElementType((DataType) idataType);
      }
      this.Size = this.Dimensions.ElementCount * ((IBitSize) idataType).Size;
    }

    /// <summary>Number of used dynamic Dimensions.</summary>
    /// <value>The dynamic parameter count.</value>
    internal int DynamicParameterCount => this._dynamicParameterCount;
  }
}
