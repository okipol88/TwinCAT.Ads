// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ConnectionStateInterceptor
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Connection state observer (Interceptor)</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ConnectionStateInterceptor : 
    CommunicationInterceptor,
    IConnectionStateProvider,
    IConnectionStateObserver,
    IAdsStateObserver,
    IPreventRejected
  {
    /// <summary>The session</summary>
    private IAdsSession _session;
    /// <summary>Synchronizer object</summary>
    private object _synchronizer = new object();
    /// <summary>Current connection state.</summary>
    private ConnectionState _connectionState = (ConnectionState) 1;
    /// <summary>The Date/Time of the last succeeded Roundtrip</summary>
    private DateTimeOffset _lastSucceeded = DateTimeOffset.MinValue;
    /// <summary>DateTime of the last read/write access</summary>
    private DateTimeOffset _lastAccess = DateTimeOffset.MinValue;
    /// <summary>Last Error Code</summary>
    private AdsErrorCode _lastErrorCode;
    private IObservable<object>? _timerObservable;
    /// <summary>Total number of negative ADS responses.</summary>
    private int _errorCount;
    /// <summary>Error count since last acces.</summary>
    private int _communicationErrorCountSinceLastSucceeeded;
    private DateTimeOffset _lastErrorTime = DateTimeOffset.MinValue;
    private DateTimeOffset _lastCommunicationErrorTime = DateTimeOffset.MinValue;
    private int _communicationErrorCount;
    /// <summary>Number of total accesses.</summary>
    private int _totalCycles;
    /// <summary>Number of succeeded communication roundtrips</summary>
    private int _succeededCycles;
    /// <summary>
    /// Indicates that Connection refused errors (AdsErrorCode.WSA_ConnRefused) are prevented temporarily.
    /// </summary>
    private bool _preventRejectedConnection;
    /// <summary>The current ADS state.</summary>
    private StateInfo _adsState;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.ConnectionStateInterceptor" /> class.
    /// </summary>
    /// <param name="session">The session.</param>
    internal ConnectionStateInterceptor(IAdsSession session)
      : base("ConnectionStateObserverInterceptor")
    {
      this._session = session;
    }

    /// <summary>
    /// Gets the Date/Time of the last succeeded ADS communication/Roundtrip.
    /// </summary>
    /// <value>The Date/Time value.</value>
    /// <remarks>A successful communication is also a negative ADS response (not  <see cref="F:TwinCAT.Ads.AdsErrorCode.NoError" />) that is not classified as communication/tripping error (<see cref="F:TwinCAT.Ads.FailFastHandlerInterceptor.TrippingErrors" />).
    /// </remarks>
    public DateTimeOffset LastSucceededAt
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._lastSucceeded;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Gets the DateTime of the last read/write access</summary>
    public DateTimeOffset LastAccessAt
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._lastAccess;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>
    /// Gets the response ErrorCode of the last read/write access.
    /// </summary>
    public AdsErrorCode LastErrorCode
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._lastErrorCode;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Handler function called after communication</summary>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="result">The error code.</param>
    protected override void OnAfterCommunicate(bool resurrect, ResultAds result)
    {
      bool tripping = FailFastHandlerInterceptor.IsTrippingError(result.ErrorCode, this._preventRejectedConnection);
      if (!resurrect)
        this.OnHandleCounters(result.ErrorCode, tripping);
      if (tripping)
      {
        this.setState((ConnectionState) 3);
        this.setAdsState(new StateInfo());
      }
      else
        this.setState((ConnectionState) 2);
      base.OnAfterCommunicate(resurrect, result);
    }

    private void OnHandleCounters(AdsErrorCode errorCode, bool tripping)
    {
      object synchronizer = this._synchronizer;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(synchronizer, ref lockTaken);
        ++this._totalCycles;
        this._lastAccess = DateTimeOffset.Now;
        this._lastErrorCode = errorCode;
        if (AdsErrorCodeExtensions.Failed(errorCode))
        {
          ++this._errorCount;
          this._lastErrorTime = this._lastAccess;
          if (tripping)
          {
            ++this._communicationErrorCount;
            this._lastCommunicationErrorTime = this._lastAccess;
            ++this._communicationErrorCountSinceLastSucceeeded;
          }
          else
          {
            this._lastSucceeded = this._lastAccess;
            ++this._succeededCycles;
            this._communicationErrorCountSinceLastSucceeeded = 0;
          }
        }
        else
        {
          ++this._succeededCycles;
          this._lastSucceeded = this._lastAccess;
          this._communicationErrorCountSinceLastSucceeeded = 0;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(synchronizer);
      }
    }

    /// <summary>Sets the connection state.</summary>
    /// <param name="newState">The new state.</param>
    private void setState(ConnectionState newState)
    {
      ConnectionState oldState = (ConnectionState) 0;
      object synchronizer = this._synchronizer;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(synchronizer, ref lockTaken);
        oldState = this._connectionState;
        this._connectionState = newState;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(synchronizer);
      }
      if (oldState == this._connectionState)
        return;
      this.OnConnectionStatusChanged(oldState, newState);
    }

    /// <summary>
    /// Handler function called after the connection has been established.
    /// </summary>
    /// <param name="error">The error code.</param>
    protected override void OnAfterConnect(AdsErrorCode error)
    {
      if (AdsErrorCodeExtensions.Succeeded(error))
      {
        this.setState((ConnectionState) 2);
      }
      else
      {
        this.setState((ConnectionState) 3);
        this.setAdsState(new StateInfo());
      }
      this._timerObservable = Observable.Interval(TimeSpan.FromSeconds(1.0)).Select<long, object>((Func<long, object>) (i => (object) (short) i));
    }

    /// <summary>
    /// Handler function called after the disconnection of the communication channel.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    protected override void OnAfterDisconnect(AdsErrorCode errorCode)
    {
      if (AdsErrorCodeExtensions.Succeeded(errorCode))
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
        this.setAdsState(new StateInfo());
        this.setState((ConnectionState) 1);
      }
      else
      {
        this.setState((ConnectionState) 3);
        this.setAdsState(new StateInfo());
      }
      this._timerObservable = (IObservable<object>) null;
    }

    /// <summary>Gets the total number of negative ADS responses.</summary>
    /// <value>The total number of negative ADS responses.</value>
    /// <remarks>This number includes all communication/tripping errors and succeeded negative ADS responses.</remarks>
    public int TotalErrors
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._errorCount;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>
    /// Gets the number of communication errors since the last successful access
    /// </summary>
    /// <value></value>
    /// <remarks>Only communication (tripping, <see cref="F:TwinCAT.Ads.FailFastHandlerInterceptor.TrippingErrors" />) errors count here. A succeeded roundtrip (non tripping)
    /// sets this value to zero.
    /// </remarks>
    public int CommunicationErrorsSinceLastSucceeded
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._communicationErrorCountSinceLastSucceeeded;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Gets the last error (negative response) time</summary>
    /// <value>The last error time.</value>
    /// <remarks>This is the Date/Time of the last ADS request that was responded <b>not</b> with <see cref="F:TwinCAT.Ads.AdsErrorCode.NoError" />.
    /// This can mean simply a negative response.
    /// </remarks>
    /// <seealso cref="P:TwinCAT.Ads.ConnectionStateInterceptor.LastCommunicationErrorAt" />
    public DateTimeOffset LastErrorAt
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._lastErrorTime;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Gets the last communication error date/time.</summary>
    /// <value>The last communication error date/time.</value>
    /// <remarks>The communication errors are the errors that are classified as communication tripping errors (Network communication problems e.g. device not reachable, <see cref="F:TwinCAT.Ads.FailFastHandlerInterceptor.TrippingErrors" />)</remarks>
    public DateTimeOffset LastCommunicationErrorAt
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._lastCommunicationErrorTime;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Gets the communication error count.</summary>
    /// <value>The communication error count.</value>
    /// <remarks>The communication errors are the errors that are classified as communication tripping errors (Network communication problems e.g. device not reachable, <see cref="F:TwinCAT.Ads.FailFastHandlerInterceptor.TrippingErrors" />)</remarks>
    public int TotalCommunicationErrors
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._communicationErrorCount;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>
    /// Gets the current Connection state of the <see cref="T:TwinCAT.Ads.ConnectionStateInterceptor" />
    /// </summary>
    /// <value>The state of the connection.</value>
    public ConnectionState ConnectionState
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._connectionState;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Occurs when the connection state has been changed.</summary>
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>Occurs when the ads state has been changed.</summary>
    public event EventHandler<AdsStateChangedEventArgs2>? AdsStateChanged;

    /// <summary>Called when [connection status changed].</summary>
    /// <param name="oldState">The old state.</param>
    /// <param name="newState">The new state.</param>
    protected virtual void OnConnectionStatusChanged(
      ConnectionState oldState,
      ConnectionState newState)
    {
      if (this.ConnectionStateChanged == null)
        return;
      ConnectionStateChangedReason stateChangedReason = (ConnectionStateChangedReason) 4;
      if (newState == 2 && (oldState == 1 || oldState == null))
        stateChangedReason = (ConnectionStateChangedReason) 1;
      else if (newState == 2 && oldState == 3)
        stateChangedReason = (ConnectionStateChangedReason) 5;
      else if (newState == 3)
        stateChangedReason = (ConnectionStateChangedReason) 3;
      else if (newState == 1)
        stateChangedReason = (ConnectionStateChangedReason) 2;
      this.ConnectionStateChanged((object) this, (ConnectionStateChangedEventArgs) new SessionConnectionStateChangedEventArgs(stateChangedReason, newState, oldState, (ISession) this._session, ((ISession) this._session).Connection, (Exception) null));
    }

    /// <summary>
    /// Gets the total number of ADS Accesses (Succeeded or Failed)
    /// </summary>
    public int TotalCycles
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._totalCycles;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Gets the number of succeeded Accesses.</summary>
    public int TotalSucceded
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return this._succeededCycles;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Gets the age of the last succeeded access.</summary>
    /// <remarks>Returns the TimeSpan to the time with the last succeeded Access. This Information can be used to get a measure of the Quality of the current
    /// connection - at least when frequent communciation is done over the connection.
    /// </remarks>
    public TimeSpan TimeToLastSucceed
    {
      get
      {
        object synchronizer = this._synchronizer;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(synchronizer, ref lockTaken);
          return DateTimeOffset.Now - this._lastSucceeded;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(synchronizer);
        }
      }
    }

    /// <summary>Gets the current state of the connected ADS Server.</summary>
    /// <value>ADS state</value>
    public StateInfo StateInfo => this._adsState;

    /// <summary>
    /// Gets or sets a value indicating whether Connection refused errors (AdsErrorCode.WSA_ConnRefused) trigger internal error conditions
    /// </summary>
    /// <value><c>true</c> if connection refused is prevented; otherwise, <c>false</c>.</value>
    bool IPreventRejected.PreventRejectedConnection
    {
      get => this._preventRejectedConnection;
      set => this._preventRejectedConnection = value;
    }

    /// <summary>Handler function called after the ADS state is read.</summary>
    /// <param name="result">The result.</param>
    protected override void OnAfterReadState(ResultReadDeviceState result)
    {
      if (result == null)
        throw new ArgumentNullException(nameof (result));
      if (((ResultAds) result).ErrorCode != null)
        return;
      this.setAdsState(result.State);
    }

    /// <summary>
    /// Handler function called after an ADS state is written.
    /// </summary>
    /// <param name="adsState">ADS state.</param>
    /// <param name="result">The result.</param>
    protected override void OnAfterWriteState(StateInfo adsState, ResultAds result)
    {
      if (!result.Succeeded)
        return;
      this.setAdsState(adsState);
    }

    /// <summary>Sets the ADS state.</summary>
    /// <param name="adsState">Ads state.</param>
    private void setAdsState(StateInfo adsState)
    {
      StateInfo adsState1 = this._adsState;
      if (((StateInfo) ref adsState).Equals(adsState1))
        return;
      object synchronizer = this._synchronizer;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(synchronizer, ref lockTaken);
        this._adsState = adsState;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(synchronizer);
      }
      this.OnAdsStateChanged(adsState1, this._adsState);
    }

    /// <summary>Called when the ADS state has been changed.</summary>
    /// <param name="oldState">The old state.</param>
    /// <param name="newState">State of the _ads.</param>
    private void OnAdsStateChanged(StateInfo oldState, StateInfo newState)
    {
      if (this.AdsStateChanged == null)
        return;
      this.AdsStateChanged((object) this, new AdsStateChangedEventArgs2(newState, oldState, (ISession) this._session));
    }

    /// <summary>Get connection statistics</summary>
    /// <returns>ConnectionStatisticsInfo.</returns>
    public ConnectionStatisticsInfo GetConnectionStatistics()
    {
      object synchronizer = this._synchronizer;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(synchronizer, ref lockTaken);
        ConnectionStatisticsInfo connectionStatistics;
        // ISSUE: explicit constructor call
        ((ConnectionStatisticsInfo) ref connectionStatistics).\u002Ector(this._totalCycles, this._succeededCycles, this._communicationErrorCount, DateTimeOffset.Now - this._lastSucceeded);
        return connectionStatistics;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(synchronizer);
      }
    }
  }
}
