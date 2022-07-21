// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.ReadRequestCompletionSource
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// Task completion source for an asynchronous Read request
  internal class ReadRequestCompletionSource : AdsRequestCompletionSourceBase<ResultRead>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.ReadRequestCompletionSource" /> class.
    /// </summary>
    /// <param name="invokeId">The invoke identifier.</param>
    public ReadRequestCompletionSource(uint invokeId)
      : base(invokeId)
    {
    }
  }
}
