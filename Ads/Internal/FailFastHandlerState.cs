// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.FailFastHandlerState
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Base class for the different <see cref="T:TwinCAT.Ads.IFailFastHandler" /> state objects.
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class FailFastHandlerState : IFailFastHandlerState
  {
    /// <summary>
    /// Fail fast timeout (reconnection timeout / auto-reset timeout).
    /// </summary>
    protected readonly TimeSpan timeout;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.FailFastHandlerState" /> class.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    protected FailFastHandlerState(TimeSpan timeout) => this.timeout = timeout;

    /// <summary>
    /// Guards the <see cref="T:TwinCAT.Ads.IFailFastHandler" /> from state changes that allow unintended communication.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode Guard() => this.OnGuard();

    /// <summary>Called when communication should be guarded.</summary>
    protected virtual AdsErrorCode OnGuard() => (AdsErrorCode) 0;

    /// <summary>
    /// Transition to the next state of the state machine (state machine pattern).
    /// </summary>
    /// <returns>IFailFastHandlerState.</returns>
    public IFailFastHandlerState NextState() => this.OnNextState();

    /// <summary>
    /// Handler function for transitioning into the next state (if possible)
    /// </summary>
    /// <returns>IFailFastHandlerState.</returns>
    protected abstract IFailFastHandlerState OnNextState();

    /// <summary>Indicates that the communication has been succeeded.</summary>
    public void Succeed() => this.OnSucceed();

    /// <summary>Handler function for a succeeded communication.</summary>
    protected virtual void OnSucceed()
    {
    }

    /// <summary>
    /// Trips to the internal <see cref="T:TwinCAT.Ads.Internal.LostCommunicationState" /> (Open circuit) state.
    /// </summary>
    public void Trip(AdsErrorCode error) => this.OnTrip(error);

    /// <summary>
    /// Handler function for a tripped communication error condition.
    /// </summary>
    /// <param name="error">The causing error.</param>
    protected virtual void OnTrip(AdsErrorCode error)
    {
    }
  }
}
