// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.LostCommunicationState
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// State object for a lost communication state (circuit open, auto reset errors active)
  /// </summary>
  internal class LostCommunicationState : FailFastHandlerState
  {
    /// <summary>Indicates the time the communication was lost</summary>
    private readonly DateTimeOffset _lostTime;
    private readonly AdsErrorCode _error;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.FailFastHandlerState" /> class.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="causingError">The error.</param>
    public LostCommunicationState(TimeSpan timeout, AdsErrorCode causingError)
      : base(timeout)
    {
      this._lostTime = DateTimeOffset.Now;
      this._error = causingError;
      AdsModule.TraceSession.TraceInformation("FailFastHandlerInterceptor --> Lost (Caused by '{0}')", new object[1]
      {
        (object) causingError
      });
    }

    /// <summary>Called when communication should be guarded.</summary>
    /// <exception cref="T:System.InvalidOperationException">The circuit is currently open.</exception>
    protected override AdsErrorCode OnGuard() => this._error;

    /// <summary>
    /// Handler function for transitioning into the next state (if possible)
    /// </summary>
    /// <returns>IFailFastHandlerState.</returns>
    protected override IFailFastHandlerState OnNextState() => DateTimeOffset.Now - this._lostTime >= this.timeout ? (IFailFastHandlerState) new ReconnectingCommunicationState(this.timeout) : (IFailFastHandlerState) this;
  }
}
