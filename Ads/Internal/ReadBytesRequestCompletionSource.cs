// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.ReadBytesRequestCompletionSource
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Task completion source for an asynchronous Read request with included bytes data.
  /// </summary>
  /// <exclude />
  /// <seealso cref="T:TwinCAT.Ads.Internal.AdsRequestCompletionSourceBase`1" />
  internal class ReadBytesRequestCompletionSource : AdsRequestCompletionSourceBase<ResultReadBytes>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.ReadBytesRequestCompletionSource" /> class.
    /// </summary>
    /// <param name="invokeId">The invoke identifier.</param>
    public ReadBytesRequestCompletionSource(uint invokeId)
      : base(invokeId)
    {
    }
  }
}
