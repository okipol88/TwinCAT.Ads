// Decompiled with JetBrains decompiler
// Type: TwinCAT.AdsCommunicationStatistics
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using TwinCAT.Ads;


#nullable enable
namespace TwinCAT
{
  /// <summary>ADS Communication statistics</summary>
  /// <remarks>
  /// These statistics can be used for communication diagnosis.
  /// They contain Error/Succeed counts as well as Resurrection infos.
  /// </remarks>
  public class AdsCommunicationStatistics
  {
    private AdsSessionBase _session;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.AdsCommunicationStatistics" /> class.
    /// </summary>
    /// <param name="session">The session.</param>
    internal AdsCommunicationStatistics(AdsSessionBase session) => this._session = session != null ? session : throw new ArgumentNullException(nameof (session));

    /// <summary>
    /// Gets the total cycles/requests done so far by this session.
    /// </summary>
    /// <value>The total cycles / requests</value>
    public int TotalCycles => this._session.ConnectionObserver != null ? this._session.ConnectionObserver.TotalCycles : 0;

    /// <summary>Gets the total number of negative ADS responses.</summary>
    /// <value>The total number of negative ADS responses.</value>
    /// <remarks>This number includes all communication/tripping errors and succeeded negative ADS responses.</remarks>
    public int TotalErrors => this._session.ConnectionObserver != null ? this._session.ConnectionObserver.TotalErrors : 0;

    /// <summary>Gets the UTC time of the last succeeded access.</summary>
    /// <value>The last succeeded access.</value>
    /// <exclude />
    [Obsolete("Use AdsCommunicationStatistics.LastSucceessAt")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DateTimeOffset? LastSucceededAccess => this.LastSucceededAt;

    /// <summary>
    /// Gets the Date/Time of the last succeeded ADS communication/Roundtrip.
    /// </summary>
    /// <value>The Date/Time value.</value>
    /// <remarks>A successful communication is also a negative ADS response (not  <see cref="F:TwinCAT.Ads.AdsErrorCode.NoError" />) that is not classified as communication/tripping error (<see cref="F:TwinCAT.Ads.FailFastHandlerInterceptor.TrippingErrors" />).
    /// </remarks>
    public DateTimeOffset? LastSucceededAt
    {
      get
      {
        if (!this._session.IsConnected)
          return new DateTimeOffset?();
        return this._session.ConnectionObserver?.LastSucceededAt;
      }
    }

    /// <summary>
    /// Gets the Time of the last read/write access (successfull or not)
    /// </summary>
    /// <value>The last succeeded access.</value>
    /// <exclude />
    [Obsolete("Use AdsCommunicationStatistics.LastSucceessAt")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DateTimeOffset? LastAccess => this.LastAccessAt;

    /// <summary>Gets the Time of the last read/write access</summary>
    /// <value>DateTime of the last access.</value>
    public DateTimeOffset? LastAccessAt
    {
      get
      {
        if (!this._session.IsConnected)
          return new DateTimeOffset?();
        return this._session.ConnectionObserver?.LastAccessAt;
      }
    }

    /// <summary>Gets error code of the last access.</summary>
    /// <value>The last succeeded access.</value>
    public AdsErrorCode LastErrorCode => this._session.IsConnected && this._session.ConnectionObserver != null ? this._session.ConnectionObserver.LastErrorCode : (AdsErrorCode) 0;

    /// <summary>
    /// Gets the wait time for the next access (Resurrection time) if in <see cref="F:TwinCAT.ConnectionState.Lost" />.
    /// </summary>
    /// <value>The wait time if in <see cref="F:TwinCAT.ConnectionState.Lost" /> otherwise <b>TimeSpan.Zero</b>.</value>
    public TimeSpan AccessWaitTime => this._session.IsConnected ? this._session.Connection.AccessWaitTime : TimeSpan.Zero;

    /// <summary>Gets the error count since last access (UTC)</summary>
    /// <value>The error count since last access.</value>
    /// <exclude />
    [Obsolete("Use AdsCommunicationStatistics.CommunicationErrorsSinceLastSucceeded")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int ErrorsSinceLastSucceeded => this.CommunicationErrorsSinceLastSucceeded;

    /// <summary>
    /// Gets the number of communication errors since the last successful access
    /// </summary>
    /// <value></value>
    /// <remarks>Only communication (tripping, <see cref="F:TwinCAT.Ads.FailFastHandlerInterceptor.TrippingErrors" />) errors count here. A succeeded roundtrip (non tripping)
    /// sets this value to zero.
    /// </remarks>
    public int CommunicationErrorsSinceLastSucceeded => this._session.ConnectionObserver != null ? this._session.ConnectionObserver.CommunicationErrorsSinceLastSucceeded : 0;

    /// <summary>Gets the communication error count.</summary>
    /// <value>The communication error count.</value>
    /// <remarks>The communication errors are the errors that are classified as communication tripping errors (Network communication problems e.g. device not reachable, <see cref="F:TwinCAT.Ads.FailFastHandlerInterceptor.TrippingErrors" />)</remarks>
    public int TotalCommunicationErrors => this._session.ConnectionObserver != null ? this._session.ConnectionObserver.TotalCommunicationErrors : 0;

    /// <summary>Gets the last tripping error time.</summary>
    /// <value>The last tripping error time.</value>
    /// <remarks>Tripping errors are errors that are preventing the ADS Session to communicate until a resurrection occurs.
    /// These are all errors that are classified that the target system could not be reached.
    /// </remarks>
    public DateTimeOffset LastCommunicationErrorAt => this._session.ConnectionObserver != null ? this._session.ConnectionObserver.LastCommunicationErrorAt : DateTimeOffset.MinValue;

    /// <summary>
    /// Get the Time of the last occurred Error (Error response codes and tripping errors)
    /// </summary>
    /// <value>The error count since last access.</value>
    public DateTimeOffset LastErrorAt => this._session.ConnectionObserver != null ? this._session.ConnectionObserver.LastErrorAt : DateTimeOffset.MinValue;

    /// <summary>
    /// Gets the number of resurrections on the <see cref="T:TwinCAT.Ads.AdsConnection" />
    /// </summary>
    /// <value>The resurrections.</value>
    /// <exclude />
    [Obsolete("Use AdsCommunicationStatistics.TotalResurrections")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int ConnectionResurrections => this.TotalResurrections;

    /// <summary>
    /// Gets the number of resurrections on the <see cref="T:TwinCAT.Ads.AdsConnection" />
    /// </summary>
    /// <value>The resurrections.</value>
    public int TotalResurrections => this._session.IsConnected ? this._session.Connection.TotalResurrections : 0;

    /// <summary>Gets the UTC time when the session was established.</summary>
    /// <value>The session established at.</value>
    public DateTimeOffset SessionEstablishedAt => this._session.EstablishedAt;

    /// <summary>
    /// Gets the UTC time when the current connection was established.
    /// </summary>
    /// <value>The connection established at.</value>
    public DateTimeOffset? ConnectionEstablishedAt => this._session.IsConnected ? this._session.Connection.ConnectionEstablishedAt : new DateTimeOffset?();

    /// <summary>Gets the DateTime of the last connection activation.</summary>
    /// <value>Connection active time.</value>
    public DateTimeOffset? ConnectionActiveSince => this._session.IsConnected ? this._session.Connection.ActiveSince : new DateTimeOffset?();

    /// <summary>Gets the connection lost count.</summary>
    /// <value>The connection lost count.</value>
    /// <exclude />
    [Obsolete("Use AdsCommunicationStatistics.TotalConnectionLosses")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int ConnectionLostCount => this.TotalConnectionLosses;

    /// <summary>Gets the connection lost count.</summary>
    /// <value>The connection lost count.</value>
    public int TotalConnectionLosses => this._session.IsConnected ? this._session.Connection.TotalConnectionLosses : 0;

    /// <summary>Gets the UTC connection lost time.</summary>
    /// <value>The connection lost time.</value>
    /// <exclude />
    [Obsolete("Use AdsCommunicationStatistics.ConnectionLostAt")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DateTimeOffset? ConnectionLostTime => this.ConnectionLostAt;

    /// <summary>Gets the connection lost time.</summary>
    /// <value>The connection lost time.</value>
    public DateTimeOffset? ConnectionLostAt
    {
      get
      {
        if (!this._session.IsConnected)
          return new DateTimeOffset?();
        return this._session.Connection?.ConnectionLostTime;
      }
    }
  }
}
