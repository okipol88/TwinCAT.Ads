// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.ActiveCommunicationState
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>State object for an active communication state.</summary>
  internal class ActiveCommunicationState : FailFastHandlerState
  {
    /// <summary>Trip indicator</summary>
    private bool _tripped;
    private AdsErrorCode _error;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.FailFastHandlerState" /> class.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    public ActiveCommunicationState(TimeSpan timeout)
      : base(timeout)
    {
      this._tripped = false;
      AdsModule.TraceSession.TraceInformation("FailFastHandlerInterceptor --> Active");
    }

    /// <summary>
    /// Handler function for transitioning into the next state (if possible)
    /// </summary>
    /// <returns>IFailFastHandlerState.</returns>
    protected override IFailFastHandlerState OnNextState() => this._tripped ? (IFailFastHandlerState) new LostCommunicationState(this.timeout, this._error) : (IFailFastHandlerState) this;

    /// <summary>
    /// Handler function for a tripped communication error condition.
    /// </summary>
    protected override void OnTrip(AdsErrorCode error)
    {
      this._tripped = true;
      this._error = error;
    }
  }
}
