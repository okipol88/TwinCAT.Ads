// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.IAdsErrorInjector
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Interface IAdsErrorInjector (for unit testing purposes)
  /// </summary>
  /// <exclude />
  internal interface IAdsErrorInjector
  {
    /// <summary>Injects an error (for unit testing purposes)</summary>
    /// <param name="error">The error.</param>
    /// <param name="throwAdsException">if set to <c>true</c> [throw ads exception].</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode InjectError(AdsErrorCode error, bool throwAdsException);

    /// <summary>
    /// Injection of an SymbolVersionChanged event (just for Unit-Testing purposes)
    /// </summary>
    void InjectSymbolVersionChanged();
  }
}
