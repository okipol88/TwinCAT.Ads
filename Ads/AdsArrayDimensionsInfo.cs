// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsArrayDimensionsInfo
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Class AdsArrayDimensionsInfo.</summary>
  internal class AdsArrayDimensionsInfo
  {
    /// <summary>The dimension information.</summary>
    private AdsDataTypeArrayInfo[] _dims;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsArrayDimensionsInfo" /> class.
    /// </summary>
    /// <param name="dims">The dims.</param>
    /// <exception cref="T:System.ArgumentNullException">dims</exception>
    internal AdsArrayDimensionsInfo(AdsDataTypeArrayInfo[] dims) => this._dims = dims != null ? dims : throw new ArgumentNullException(nameof (dims));

    /// <summary>Get the number of Elements over all Dimensions</summary>
    /// <value>The elements.</value>
    internal int Elements => AdsArrayDimensionsInfo.GetArrayElementCount(this._dims);

    /// <summary>Gets the number of elements over all dimensions</summary>
    /// <param name="arrayInfo">The array information.</param>
    /// <returns>System.Int32.</returns>
    internal static int GetArrayElementCount(AdsDataTypeArrayInfo[] arrayInfo)
    {
      if (arrayInfo == null)
        throw new ArgumentNullException(nameof (arrayInfo));
      int arrayElementCount = 0;
      if (arrayInfo.Length != 0)
      {
        arrayElementCount = arrayInfo[0].Elements;
        for (int index = 1; index < arrayInfo.Length; ++index)
          arrayElementCount *= arrayInfo[index].Elements;
      }
      return arrayElementCount;
    }

    /// <summary>Gets the lower bounds.</summary>
    /// <value>The lower bounds.</value>
    internal int[] LowerBounds
    {
      get
      {
        int[] lowerBounds = new int[this._dims.Length];
        for (int index = 0; index < this._dims.Length; ++index)
          lowerBounds[index] = this._dims[index].LowerBound;
        return lowerBounds;
      }
    }

    /// <summary>Gets the upper bounds.</summary>
    /// <value>The upper bounds.</value>
    internal int[] UpperBounds
    {
      get
      {
        int[] upperBounds = new int[this._dims.Length];
        for (int index = 0; index < this._dims.Length; ++index)
          upperBounds[index] = this._dims[index].LowerBound + this._dims[index].Elements - 1;
        return upperBounds;
      }
    }

    /// <summary>Gets the dimension elements.</summary>
    /// <value>The dimension elements.</value>
    internal int[] DimensionElements
    {
      get
      {
        int[] dimensionElements = new int[this._dims.Length];
        for (int index = 0; index < this._dims.Length; ++index)
          dimensionElements[index] = this._dims[index].Elements;
        return dimensionElements;
      }
    }
  }
}
