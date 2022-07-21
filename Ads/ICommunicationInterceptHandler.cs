// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ICommunicationInterceptHandler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Interface ICommunicationInterceptHandler</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface ICommunicationInterceptHandler
  {
    /// <summary>Called before communication</summary>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode BeforeCommunicate();

    /// <summary>Called after communication</summary>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="result">Communication error/result.</param>
    void AfterCommunicate(bool resurrect, ResultAds result);

    /// <summary>
    /// Called before the communication channel is established.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode BeforeConnect();

    /// <summary>
    /// Called after the communication channel is established.
    /// </summary>
    /// <param name="result">The result.</param>
    void AfterConnect(AdsErrorCode result);

    /// <summary>Called before the communication channel shuts down.</summary>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode BeforeDisconnect();

    /// <summary>Called after the communication channel has shut down.</summary>
    /// <param name="result">The result.</param>
    void AfterDisconnect(AdsErrorCode result);

    /// <summary>Called before an ADS state is written</summary>
    /// <param name="adsState">State of the ads.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode BeforeWriteState(StateInfo adsState);

    /// <summary>Called after an ADS state is written.</summary>
    /// <param name="adsState">Ads state..</param>
    /// <param name="result">The result.</param>
    void AfterWriteState(StateInfo adsState, ResultAds result);

    /// <summary>Called before the AdsState is read.</summary>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode BeforeReadState();

    /// <summary>Called after the ADS state is read.</summary>
    /// <param name="result">The result.</param>
    void AfterReadState(ResultReadDeviceState result);
  }
}
