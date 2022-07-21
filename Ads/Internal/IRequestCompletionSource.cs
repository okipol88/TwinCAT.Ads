// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.IRequestCompletionSource
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Interface Representing an AdsRequest completion source
  /// </summary>
  internal interface IRequestCompletionSource
  {
    /// <summary>Gets the Ads InvokeID</summary>
    /// <value>The invoke identifier.</value>
    /// <remarks>This is the Identifier the Completion source is triggering its confirmations on.
    /// </remarks>
    uint InvokeId { get; }
  }
}
