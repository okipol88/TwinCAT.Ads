// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.CategoryConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Category converter</summary>
  /// <remarks>Converts the <see cref="T:TwinCAT.TypeSystem.DataTypeCategory" /> from <see cref="T:TwinCAT.Ads.AdsDataTypeId" />
  /// and vice versa.</remarks>
  /// <exclude />
  internal class CategoryConverter
  {
    /// <summary>
    /// Converts to <see cref="T:TwinCAT.TypeSystem.DataTypeCategory" /> from <see cref="T:TwinCAT.Ads.AdsDataTypeId" />.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>DataTypeCategory.</returns>
    internal static DataTypeCategory FromId(AdsDataTypeId id)
    {
      if (id <= 21)
      {
        if (id != null && id - 2 > 3 && id - 16 > 5)
          goto label_8;
      }
      else
      {
        if (id - 30 <= 1)
          return (DataTypeCategory) 10;
        if (id != 33)
        {
          if (id == 65)
            return (DataTypeCategory) 0;
          goto label_8;
        }
      }
      return (DataTypeCategory) 1;
label_8:
      return (DataTypeCategory) 0;
    }

    /// <summary>
    /// Converts the <see cref="T:TwinCAT.TypeSystem.DataTypeCategory" /> from <see cref="T:TwinCAT.Ads.AdsDataTypeId" /> and typename.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>DataTypeCategory.</returns>
    internal static DataTypeCategory FromId(AdsDataTypeId id, string typeName)
    {
      DataTypeCategory dataTypeCategory = CategoryConverter.FromId(id);
      if (!string.IsNullOrEmpty(typeName) && dataTypeCategory == null)
      {
        if (DataTypeStringParser.IsPointer(typeName))
          dataTypeCategory = (DataTypeCategory) 13;
        else if (DataTypeStringParser.IsReference(typeName))
          dataTypeCategory = (DataTypeCategory) 15;
        else if (DataTypeStringParser.IsArray(typeName))
          dataTypeCategory = (DataTypeCategory) 4;
        else if (DataTypeStringParser.IsSubRange(typeName))
          dataTypeCategory = (DataTypeCategory) 9;
        else if (DataTypeStringParser.IsIntegralType(typeName))
          dataTypeCategory = (DataTypeCategory) 1;
        else if (DataTypeStringParser.IsString(typeName))
          dataTypeCategory = (DataTypeCategory) 10;
      }
      return dataTypeCategory;
    }

    /// <summary>
    /// Converts the <see cref="T:TwinCAT.TypeSystem.DataTypeCategory" /> to the <see cref="T:TwinCAT.Ads.AdsDataTypeId" />.
    /// </summary>
    /// <param name="cat">The cat.</param>
    /// <returns>AdsDatatypeId.</returns>
    internal static AdsDataTypeId FromCategory(DataTypeCategory cat)
    {
      switch ((int) cat)
      {
        case 0:
        case 1:
        case 2:
          return (AdsDataTypeId) 0;
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
          return (AdsDataTypeId) 65;
        case 12:
          return (AdsDataTypeId) 33;
        default:
          return (AdsDataTypeId) 0;
      }
    }
  }
}
