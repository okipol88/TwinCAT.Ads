// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.DimensionConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class DimensionConverter.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DimensionConverter
  {
    /// <summary>
    /// Converts the <see cref="T:TwinCAT.TypeSystem.DimensionCollection" /> to <see cref="T:TwinCAT.Ads.AdsDataTypeArrayInfo" /> array.
    /// </summary>
    /// <returns>AdsDatatypeArrayInfo[] if is array, otherwise <c>NULL</c>.</returns>
    internal static AdsDataTypeArrayInfo[] ToAdsDataTypeArrayInfo(
      IList<IDimension> dims)
    {
      AdsDataTypeArrayInfo[] dataTypeArrayInfo = Array.Empty<AdsDataTypeArrayInfo>();
      if (((ICollection<IDimension>) dims).Count > 0)
      {
        dataTypeArrayInfo = new AdsDataTypeArrayInfo[((ICollection<IDimension>) dims).Count];
        for (int index = 0; index < ((ICollection<IDimension>) dims).Count; ++index)
        {
          IDimension dim = dims[index];
          dataTypeArrayInfo[index] = new AdsDataTypeArrayInfo(dim.LowerBound, dim.ElementCount);
        }
      }
      return dataTypeArrayInfo;
    }

    internal static DimensionCollection ToDimensionCollection(
      AdsDataTypeArrayInfo[] arrayInfos)
    {
      int num = arrayInfos != null ? arrayInfos.GetLength(0) : throw new ArgumentNullException(nameof (arrayInfos));
      DimensionCollection dimensionCollection = new DimensionCollection();
      for (int index = 0; index < num; ++index)
      {
        AdsDataTypeArrayInfo arrayInfo = arrayInfos[index];
        Dimension dimension = new Dimension(arrayInfo.LowerBound, arrayInfo.Elements);
        dimensionCollection.Add((IDimension) dimension);
      }
      return dimensionCollection;
    }
  }
}
