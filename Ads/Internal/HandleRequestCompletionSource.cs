// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.HandleRequestCompletionSource
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Task completion source for an asynchronous request that returns and ADS Handle.
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.Internal.AdsRequestCompletionSourceBase`1" />
  /// <exclude />
  internal class HandleRequestCompletionSource : AdsRequestCompletionSourceBase<ResultHandle>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.HandleRequestCompletionSource" /> class.
    /// </summary>
    /// <param name="invokeId">The invoke identifier.</param>
    public HandleRequestCompletionSource(uint invokeId)
      : base(invokeId)
    {
    }
  }
}
