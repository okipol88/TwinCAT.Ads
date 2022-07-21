// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SessionSettings
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Session settings class</summary>
  public class SessionSettings : IAdsSessionSettings, ISessionSettings
  {
    private static TimeSpan s_defaultCommunicationTimeout = TimeSpan.FromSeconds(5.0);
    private static TimeSpan s_defaultResurrectionTime = TimeSpan.FromSeconds(21.0);
    private TimeSpan _resurrectionTime = SessionSettings.DefaultResurrectionTime;
    private SymbolLoaderSettings? _loaderSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SessionSettings" /> class.
    /// </summary>
    public SessionSettings(int timeout) => this.Timeout = timeout;

    /// <summary>Gets the ADS timeout in milliseconds.</summary>
    /// <value>The timeout.</value>
    public int Timeout { get; internal set; }

    /// <summary>Gets the default Settings (Synchronized).</summary>
    /// <remarks>
    /// The following defaults are set here:
    /// <list type="table">
    /// <listheader><term>Setting</term><description>Description</description></listheader>
    ///     <item><term>Communication Timeout (<see cref="P:TwinCAT.Ads.SessionSettings.Timeout" />)</term><description>Default communication timeout (<see cref="P:TwinCAT.Ads.SessionSettings.DefaultCommunicationTimeout" />, default 5s)</description></item>
    ///     <item><term>Resurrection Timeout (<see cref="P:TwinCAT.Ads.SessionSettings.ResurrectionTime" />)</term><description>Default communication timeout (<see cref="P:TwinCAT.Ads.SessionSettings.DefaultResurrectionTime" />, default 21s)</description></item>
    ///     <item><term>Dynamic SymbolLoader settings <see cref="P:TwinCAT.Ads.SessionSettings.SymbolLoader" /></term><description>Synchronized mode activated (<see cref="P:TwinCAT.SymbolLoaderSettings.DefaultDynamic" />)</description></item>
    /// </list>
    /// </remarks>
    /// <value>The default settings.</value>
    public static SessionSettings Default => new SessionSettings((int) SessionSettings.DefaultCommunicationTimeout.TotalMilliseconds);

    /// <summary>
    /// Gets a Settings object that configures the AdsSession for FastWriteThrough
    /// </summary>
    /// <value>Session settings for a fast write through (with 200 ms Timeout).</value>
    /// <remarks>The settings typically can be used for polling clients, where the "FailFast"
    /// feature will be bypassed. That means, that communication fails doesn't trigger
    /// the FailFast interceptor and every Request will go out via ADS.
    /// This has the Drawback that communication Timeouts are longer and subsequent timeouts
    /// block the ADS mailbox (with the danger of overflows). So use this setting with care
    /// for specific purposes and should not be used for standard communication.
    /// <list type="bullet">
    /// <item><description>No Resurrection time and therefore:</description></item>
    /// <item><description>No <see cref="T:TwinCAT.Ads.IFailFastHandler">FailFastHandler</see> active.</description></item>
    /// <item><description>Default communication timeout 200ms.</description></item>
    /// <item><description>Not synchronized Notifications.</description></item>
    /// </list>
    /// </remarks>
    public static SessionSettings FastWriteThrough => new SessionSettings(200)
    {
      ResurrectionTime = TimeSpan.Zero
    };

    /// <summary>The default communication timeout (5 Seconds)</summary>
    public static TimeSpan DefaultCommunicationTimeout => SessionSettings.s_defaultCommunicationTimeout;

    /// <summary>The default resurrection time (21 Seconds)</summary>
    public static TimeSpan DefaultResurrectionTime => SessionSettings.s_defaultResurrectionTime;

    /// <summary>
    /// Gets or sets the resurrection time (Default: <see cref="P:TwinCAT.Ads.SessionSettings.DefaultResurrectionTime" />)
    /// </summary>
    /// <value>The resurrection time.</value>
    /// <remarks>The resurrection time is the time after a lost connection <see cref="F:TwinCAT.ConnectionState.Lost" />
    /// can be 'resurrected'. This time is set to 21 Seconds by default (a value greater than the standard
    /// Ethernet connection timeout of 20s). The reason for this timeout is not to flood the ADS mailbox
    /// with requests that cannot be handled by the ethernet infrastructure.
    /// As long this Timespan is not expired after a recognized <see cref="F:TwinCAT.ConnectionState.Lost" />, no further
    /// data communication is done, and requests are immediately ('FailFast') answered by communication
    /// exceptions.
    /// <b>Change this value only for edge cases.</b>
    /// </remarks>
    public TimeSpan ResurrectionTime
    {
      get => this._resurrectionTime;
      set => this._resurrectionTime = value;
    }

    /// <summary>Gets or sets the symbol loader settings</summary>
    /// <value>The symbol loader.</value>
    public SymbolLoaderSettings SymbolLoader
    {
      get
      {
        if (this._loaderSettings == null)
          this._loaderSettings = SymbolLoaderSettings.DefaultDynamic;
        return this._loaderSettings;
      }
      set => this._loaderSettings = value;
    }
  }
}
