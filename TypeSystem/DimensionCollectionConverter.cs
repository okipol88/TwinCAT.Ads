// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DimensionCollectionConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.Runtime.CompilerServices;


#nullable enable
namespace TwinCAT.TypeSystem
{
  internal static class DimensionCollectionConverter
  {
    internal static string DimensionsToString(IDimensionCollection dimensions)
    {
      string[] strArray1 = new string[((ICollection<IDimension>) dimensions).Count];
      for (int index1 = 0; index1 < ((ICollection<IDimension>) dimensions).Count; ++index1)
      {
        string[] strArray2 = strArray1;
        int index2 = index1;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
        interpolatedStringHandler.AppendLiteral("[");
        interpolatedStringHandler.AppendFormatted<int>(dimensions.LowerBounds[index1]);
        interpolatedStringHandler.AppendLiteral("..");
        interpolatedStringHandler.AppendFormatted<int>(dimensions.UpperBounds[index1]);
        interpolatedStringHandler.AppendLiteral("]");
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        strArray2[index2] = stringAndClear;
      }
      return string.Join(string.Empty, strArray1);
    }
  }
}
