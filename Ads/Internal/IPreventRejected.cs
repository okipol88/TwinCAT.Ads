// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.IPreventRejected
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Interface for prevnting error states on Connection Error
  /// </summary>
  /// <remarks>
  /// This is used only internally detect the Connection state without triggering internal error conditions.
  /// </remarks>
  /// <seealso cref="F:TwinCAT.Ads.AdsErrorCode.WSA_ConnRefused" />
  /// <seealso cref="F:TwinCAT.Ads.FailFastHandlerInterceptor.TrippingErrors" />
  internal interface IPreventRejected
  {
    /// <summary>
    /// Gets or sets a value indicating whether Connection refused errors <see cref="F:TwinCAT.Ads.AdsErrorCode.WSA_ConnRefused" />) trigger internal error conditions
    /// </summary>
    /// <value><c>true</c> if connection refused is prevented; otherwise, <c>false</c>.</value>
    bool PreventRejectedConnection { get; set; }
  }
}
