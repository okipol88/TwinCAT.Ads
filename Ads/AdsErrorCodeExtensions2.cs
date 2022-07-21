// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsErrorCodeExtensions2
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Extension class for <see cref="T:TwinCAT.Ads.AdsErrorCode" />.
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.AdsErrorCodeExtensions" />
  /// <exclude />
  public static class AdsErrorCodeExtensions2
  {
    /// <summary>
    /// Throws an <see cref="T:TwinCAT.Ads.AdsErrorException" /> with the specified error code.
    /// </summary>
    /// <param name="adsErrorCode">The ads error code.</param>
    /// <exclude />
    public static AdsErrorCode ThrowOnError(this AdsErrorCode adsErrorCode)
    {
      if (adsErrorCode == null)
        return (AdsErrorCode) 0;
      throw AdsErrorException.Create(adsErrorCode);
    }

    /// <summary>
    /// Throws an <see cref="T:TwinCAT.Ads.AdsErrorException" /> with the specified error code.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="adsErrorCode">The ads error code.</param>
    /// <exclude />
    public static AdsErrorCode ThrowOnError(
      this AdsErrorCode adsErrorCode,
      string message)
    {
      if (adsErrorCode == null)
        return (AdsErrorCode) 0;
      throw AdsErrorException.Create(message, adsErrorCode);
    }
  }
}
