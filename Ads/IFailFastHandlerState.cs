// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.IFailFastHandlerState
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// State interface for the different <see cref="T:TwinCAT.Ads.IFailFastHandler" /> states.
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IFailFastHandlerState
  {
    /// <summary>
    /// Transition to the next state of the state machine (state machine pattern).
    /// </summary>
    /// <returns>IFailFastHandlerState.</returns>
    IFailFastHandlerState NextState();

    /// <summary>
    /// Guards the <see cref="T:TwinCAT.Ads.IFailFastHandler" /> from state changes that allow unintended communication.
    /// </summary>
    AdsErrorCode Guard();

    /// <summary>
    /// Trips to the internal <see cref="T:TwinCAT.Ads.Internal.LostCommunicationState" /> (Open circuit) state.
    /// </summary>
    /// <param name="errorCode">The causing error.</param>
    void Trip(AdsErrorCode errorCode);

    /// <summary>Indicates that the communication has been succeeded.</summary>
    void Succeed();
  }
}
