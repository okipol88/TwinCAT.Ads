// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.DataTypeFlagConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

namespace TwinCAT.Ads.Internal
{
  /// <summary>Class DataTypeFlagConverter.</summary>
  internal static class DataTypeFlagConverter
  {
    /// <summary>
    /// Converts <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeFlags" /> to <see cref="T:TwinCAT.Ads.Internal.AdsSymbolFlags" />
    /// </summary>
    /// <param name="dataTypeFlags">The sub item flags.</param>
    /// <returns>AdsSymbolFlags.</returns>
    internal static AdsSymbolFlags Convert(AdsDataTypeFlags dataTypeFlags)
    {
      AdsSymbolFlags adsSymbolFlags = (AdsSymbolFlags) 0;
      if ((dataTypeFlags & 4) == 4)
        adsSymbolFlags = (AdsSymbolFlags) (adsSymbolFlags | 4);
      if ((dataTypeFlags & 1024) == 1024)
        adsSymbolFlags = (AdsSymbolFlags) (adsSymbolFlags | 16);
      if ((dataTypeFlags & 32) == 32)
        adsSymbolFlags = (AdsSymbolFlags) (adsSymbolFlags | 2);
      if ((dataTypeFlags & 128) == 128)
        adsSymbolFlags = (AdsSymbolFlags) (adsSymbolFlags | 8);
      return adsSymbolFlags;
    }
  }
}
