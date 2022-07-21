// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AdsDataTypeStringParser
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>DataType String Parser class.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class AdsDataTypeStringParser
  {
    internal static bool TryParseArray(
      string typeName,
      [NotNullWhen(true)] out AdsDataTypeArrayInfo[]? dims,
      [NotNullWhen(true)] out string? baseType)
    {
      DimensionCollection dims1 = (DimensionCollection) null;
      bool array = DataTypeStringParser.TryParseArray(typeName, out dims1, out baseType);
      dims = !array ? (AdsDataTypeArrayInfo[]) null : DimensionConverter.ToAdsDataTypeArrayInfo((IList<IDimension>) dims1);
      return array;
    }
  }
}
