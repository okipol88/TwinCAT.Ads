// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.DimensionCollectionExtension
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.ObjectModel;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  internal static class DimensionCollectionExtension
  {
    /// <summary>
    /// Creates a new DimensionCollection from unconcrete dynamic dimensions
    /// </summary>
    /// <param name="dynamicDimensions">The dims.</param>
    /// <param name="index">Starting index within the dynamic lengths.</param>
    /// <param name="dynamicLengths">The dynamic lengths.</param>
    /// <param name="dynamicDimensionCount">The amount of patched dimensions.</param>
    /// <returns>The Patched / Real / Online Dimension collection.</returns>
    internal static DimensionCollection FillDynamicDimensions(
      this ReadOnlyDimensionCollection dynamicDimensions,
      int index,
      int[] dynamicLengths,
      out int dynamicDimensionCount)
    {
      dynamicDimensionCount = 0;
      DimensionCollection dimensionCollection = new DimensionCollection();
      foreach (Dimension dynamicDimension in (ReadOnlyCollection<IDimension>) dynamicDimensions)
      {
        if (dynamicDimension.ElementCount == 0 && dynamicLengths.Length > index)
        {
          dimensionCollection.Add((IDimension) new Dimension(dynamicDimension.LowerBound, dynamicLengths[index++]));
          ++dynamicDimensionCount;
        }
        else
          dimensionCollection.Add((IDimension) new Dimension(dynamicDimension.LowerBound, dynamicDimension.ElementCount));
      }
      return dimensionCollection;
    }
  }
}
