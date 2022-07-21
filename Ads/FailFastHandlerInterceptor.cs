// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.FailFastHandlerInterceptor
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Fail Fast handler for ADS communication (Circuit breaker)
  /// </summary>
  /// <remarks>If a target is not available it will throw Timeout exceptions after a Default time of 5 seconds.
  /// To prevent hanging applications and bring more robustness into the communication (less consumption of ADS Mailbox memory), a second try to call the target
  /// should fail fast - not waiting for the Timeout. Only after a dedicated reconnection timeout timespan, real communication
  /// should be retried.
  /// The <see cref="T:TwinCAT.Ads.FailFastHandlerInterceptor" /> is responsible for implementing this behavior.
  /// </remarks>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FailFastHandlerInterceptor : 
    CommunicationInterceptor,
    IFailFastHandler,
    IPreventRejected
  {
    /// <summary>Synchronizer object.</summary>
    private object _synchronizer = new object();
    private static TimeSpan s_defaultTimeout;
    /// <summary>
    /// Value indicating whether Connection refused errors (AdsErrorCode.WSA_ConnRefused) trigger internal error conditions
    /// </summary>
    private bool _preventRejectedConnection;
    /// <summary>The actual Fail fast timeout</summary>
    private TimeSpan _timeout;
    /// <summary>State of the internal state machine.</summary>
    private IFailFastHandlerState _state;
    /// <summary>
    /// The tripping errors (Error codes that a classified as communication errors
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>ErrorCode</term>
    /// <term>Description</term>&gt;
    /// </listheader>
    /// <item>
    /// <term><see cref="F:TwinCAT.Ads.AdsErrorCode.WSA_ConnRefused" /></term>
    /// <term></term>
    /// </item>
    /// <item>
    /// <term><see cref="F:TwinCAT.Ads.AdsErrorCode.PortDisabled" /></term>
    /// <term></term>
    /// </item>
    /// <item>
    /// <term><see cref="F:TwinCAT.Ads.AdsErrorCode.PortNotConnected" /></term>
    /// <term></term>
    /// </item>
    /// <item>
    /// <term><see cref="F:TwinCAT.Ads.AdsErrorCode.ClientSyncTimeOut" /></term>
    /// <term></term>
    /// </item>
    /// <item>
    /// <term><see cref="F:TwinCAT.Ads.AdsErrorCode.TargetMachineNotFound" /></term>
    /// <term></term>
    /// </item>
    /// <item>
    /// <term><see cref="F:TwinCAT.Ads.AdsErrorCode.TargetPortNotFound" /></term>
    /// <term></term>
    /// </item>
    /// <item>
    /// <term><see cref="F:TwinCAT.Ads.AdsErrorCode.ClientPortNotOpen" /></term>
    /// <term></term>
    /// </item>
    /// </list>
    /// 
    /// The <see cref="F:TwinCAT.Ads.AdsErrorCode.WSA_ConnRefused" /> can be temporarily switched off by the
    /// <see cref="T:TwinCAT.Ads.Internal.IPreventRejected" /> interface.</remarks>
    public static AdsErrorCode[] TrippingErrors;
    /// <summary>The causing error for the trip.</summary>
    private AdsErrorCode _trippedError;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.FailFastHandlerInterceptor" /> class with the Default FailFast timeout of 21 s
    /// </summary>
    public FailFastHandlerInterceptor()
      : this(FailFastHandlerInterceptor.s_defaultTimeout)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.FailFastHandlerInterceptor" /> class.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    public FailFastHandlerInterceptor(TimeSpan timeout)
      : base(nameof (FailFastHandlerInterceptor))
    {
      this._state = (IFailFastHandlerState) new ActiveCommunicationState(timeout);
      this._timeout = timeout;
    }

    /// <summary>
    /// Gets or sets a value indicating whether Connection refused errors (AdsErrorCode.WSA_ConnRefused) trigger internal error conditions
    /// </summary>
    /// <value><c>true</c> if connection refused is prevented; otherwise, <c>false</c>.</value>
    bool IPreventRejected.PreventRejectedConnection
    {
      set => this._preventRejectedConnection = value;
      get => this._preventRejectedConnection;
    }

    /// <summary>Gets the actual FailFast Timeout</summary>
    /// <value>The timeout.</value>
    public TimeSpan Timeout => this._timeout;

    /// <summary>Gets the current state of the Fail Fast handler.</summary>
    /// <value>Current state.</value>
    public IFailFastHandlerState CurrentState => this._state;

    /// <summary>
    /// Handler function called before the connection is established.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    protected override AdsErrorCode OnBeforeConnect()
    {
      this.Reset();
      return base.OnBeforeConnect();
    }

    /// <summary>Resets this instance.</summary>
    public void Reset() => this._state = (IFailFastHandlerState) new ActiveCommunicationState(this._timeout);

    /// <summary>
    /// Handler function called after the connection has been established.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    protected override void OnAfterConnect(AdsErrorCode errorCode) => base.OnAfterConnect(errorCode);

    /// <summary>
    /// Handler function called after the disconnection of the communication channel.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    protected override void OnAfterDisconnect(AdsErrorCode errorCode) => base.OnAfterDisconnect(errorCode);

    /// <summary>
    /// Handler function called before the communication action occurs.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    protected override AdsErrorCode OnBeforeCommunicate() => this.Guard();

    /// <summary>Handler function called after communication</summary>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="result">The ADS communication result object.</param>
    protected override void OnAfterCommunicate(bool resurrect, ResultAds result)
    {
      if (FailFastHandlerInterceptor.IsTrippingError(result.ErrorCode, this._preventRejectedConnection))
        this.Trip(result.ErrorCode);
      else
        this.Succeed();
      base.OnAfterCommunicate(resurrect, result);
    }

    /// <summary>
    /// Determines whether an error is tripped by the <see cref="T:TwinCAT.Ads.AdsErrorCode" />.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="preventRejectedConnectionError">if set to <c>true</c> [prevent rejected connection error].</param>
    /// <returns><c>true</c> if the error code is an tripping error.</returns>
    internal static bool IsTrippingError(
      AdsErrorCode errorCode,
      bool preventRejectedConnectionError)
    {
      return (!preventRejectedConnectionError || 10061 != errorCode) && ((IEnumerable<AdsErrorCode>) FailFastHandlerInterceptor.TrippingErrors).Contains<AdsErrorCode>(errorCode);
    }

    /// <summary>
    /// Guards the <see cref="T:TwinCAT.Ads.IFailFastHandler" /> from state changes that allow unintended communication.
    /// </summary>
    public AdsErrorCode Guard()
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      object synchronizer = this._synchronizer;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(synchronizer, ref lockTaken);
        this._state = this._state.NextState();
        adsErrorCode = this._state.Guard();
        this._state = this._state.NextState();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(synchronizer);
      }
      return adsErrorCode;
    }

    /// <summary>Causing error for the Trip</summary>
    /// <value>The tripped error.</value>
    public AdsErrorCode TrippedError
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._trippedError;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether interceptor state is active
    /// </summary>
    /// <value><c>true</c> if communication state is active; otherwise, <c>false</c>.</value>
    public bool IsActive => this._state is ActiveCommunicationState;

    /// <summary>
    /// Gets a value indicating whether interceptor state is 'ready to connect'
    /// </summary>
    /// <value><c>true</c> if this instance is reconnecting; otherwise, <c>false</c>.</value>
    public bool IsReconnecting => this._state is ReconnectingCommunicationState;

    /// <summary>
    /// Gets a value indicating whether the interceptor is in open / lost state
    /// </summary>
    /// <value><c>true</c> if communication is lost / open; otherwise, <c>false</c>.</value>
    public bool IsLost => this._state is LostCommunicationState;

    /// <summary>
    /// Trips the <see cref="T:TwinCAT.Ads.FailFastHandlerInterceptor" /> with the specified error.
    /// </summary>
    /// <param name="error">The error.</param>
    public void Trip(AdsErrorCode error)
    {
      object synchronizer = this._synchronizer;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(synchronizer, ref lockTaken);
        this._trippedError = error;
        this._state = this._state.NextState();
        this._state.Trip(error);
        this._state = this._state.NextState();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(synchronizer);
      }
    }

    /// <summary>Succeeds this instance.</summary>
    public void Succeed()
    {
      object synchronizer = this._synchronizer;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(synchronizer, ref lockTaken);
        this._state = this._state.NextState();
        this._state.Succeed();
        this._state = this._state.NextState();
        this._trippedError = (AdsErrorCode) 0;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(synchronizer);
      }
    }

    static FailFastHandlerInterceptor()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
