// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsClientSettings
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Settings object for the <see cref="T:TwinCAT.Ads.AdsClient" /> class.
  /// </summary>
  /// <remarks>This <see cref="T:TwinCAT.Ads.AdsClientSettings" /> object is used to initalize the <see cref="T:TwinCAT.Ads.AdsClient" /> with application appropriate
  /// settings.
  /// Several predefined application dependant settings are available as static properties:
  /// <list type="bullet">
  /// <item><see cref="P:TwinCAT.Ads.AdsClientSettings.Default" /></item>
  /// <item><see cref="P:TwinCAT.Ads.AdsClientSettings.FastWriteThrough" /></item>
  /// <item><see cref="P:TwinCAT.Ads.AdsClientSettings.CompatibilityDefault" /></item>
  /// </list>
  /// </remarks>
  public class AdsClientSettings
  {
    /// <summary>The Protocol settings</summary>
    private TransportProtocols _protocol;
    private CommunicationInterceptors? _interceptors;
    private int _timeout = 5000;

    /// <summary>
    /// Prevents a default instance of the <see cref="T:TwinCAT.Ads.AdsClientSettings" /> class from being created.
    /// </summary>
    private AdsClientSettings()
    {
    }

    /// <summary>
    /// Creates a Default settings <see cref="T:TwinCAT.Ads.AdsClientSettings" /> object with custom timeout.
    /// </summary>
    /// <param name="timeout">The timeout of the <see cref="T:TwinCAT.Ads.AdsClient" /> in milliseconds.</param>
    public AdsClientSettings(int timeout) => AdsClientSettings.Default._timeout = timeout;

    /// <summary>
    /// Gets the default settings (Default interceptors, Timout 5000 ms)
    /// </summary>
    /// <value>The default.</value>
    /// <remarks>Creates an settings object, with specification for <see cref="F:TwinCAT.Ads.TransportProtocols.All" /> and
    /// <see cref="T:TwinCAT.Ads.FailFastHandlerInterceptor" />.
    /// <list type="bullet">
    /// <item><description><see cref="F:TwinCAT.Ads.TransportProtocols.All" /></description></item>
    /// <item><description><see cref="T:TwinCAT.Ads.FailFastHandlerInterceptor" /> is active.</description></item>
    /// <item><description>Default communication timeout 5000ms.</description></item>
    /// <item><description>Not synchronized Notifications.</description></item>
    /// </list>
    /// </remarks>
    public static AdsClientSettings Default => new AdsClientSettings()
    {
      _protocol = TransportProtocols.All,
      _interceptors = AdsClientSettings.CreateDefaultInterceptors(),
      _timeout = 5000
    };

    /// <summary>
    /// Gets a Settings object that configures the AdsClient for FastWriteThrough
    /// </summary>
    /// <value>Client settings for a fast write through (with 200 ms Timeout).</value>
    /// <remarks>The settings typically can be used for polling clients, where the "FailFast"
    /// feature will be bypassed. That means, that communication fails doesn't trigger
    /// the FailFast interceptor and every Request will go out via ADS.
    /// This has the Drawback that communication Timeouts are longer and subsequent timeouts
    /// block the ADS mailbox (with the danger of overflows). So use this setting with care
    /// for specific purposes and should not be used for standard communication.
    /// <list type="bullet">
    /// <item><description>No <see cref="T:TwinCAT.Ads.FailFastHandlerInterceptor" /> active.</description></item>
    /// <item><description>Default communicationtimeout 200ms.</description></item>
    /// <item><description>Not synchronized Notifications.</description></item>
    /// </list>
    /// </remarks>
    public static AdsClientSettings FastWriteThrough
    {
      get
      {
        AdsClientSettings fastWriteThrough = AdsClientSettings.Default;
        fastWriteThrough._protocol = TransportProtocols.All;
        fastWriteThrough._interceptors = (CommunicationInterceptors) null;
        fastWriteThrough._timeout = 200;
        return fastWriteThrough;
      }
    }

    /// <summary>Compatibility settings object</summary>
    /// <value>The settings object.</value>
    /// <remarks>The compatibility settings intitialize the AdsClient the same way
    /// as it is done in earlier versions of the TwinCAT.Ads.dll (earlier than Version 4.2)
    /// <list type="bullet">
    /// <item><description><see cref="F:TwinCAT.Ads.TransportProtocols.All" /></description></item>
    /// <item><description>No <see cref="T:TwinCAT.Ads.FailFastHandlerInterceptor" /> active.</description></item>
    /// <item><description>Default communicationtimeout 5000ms.</description></item>
    /// <item><description>Synchronized Notifications.</description></item>
    /// </list>
    /// </remarks>
    public static AdsClientSettings CompatibilityDefault
    {
      get
      {
        AdsClientSettings compatibilityDefault = AdsClientSettings.Default;
        compatibilityDefault._interceptors = (CommunicationInterceptors) null;
        compatibilityDefault._timeout = 5000;
        return compatibilityDefault;
      }
    }

    /// <summary>Gets the protocol settings</summary>
    /// <value>The protocol.</value>
    /// <exclude />
    public TransportProtocols Protocol => this._protocol;

    /// <summary>Gets the interceptors.</summary>
    /// <value>The interceptors.</value>
    /// <exclude />
    public CommunicationInterceptors? Interceptors => this._interceptors;

    /// <summary>Creates the default interceptors.</summary>
    /// <returns>CommunicationInterceptors.</returns>
    /// <remarks>The Default is to create a FailFastHandlerInterceptor, which is the default now also for a standard <see cref="T:TwinCAT.Ads.AdsClient" />
    /// </remarks>
    /// <exclude />
    private static CommunicationInterceptors CreateDefaultInterceptors()
    {
      CommunicationInterceptors defaultInterceptors = new CommunicationInterceptors();
      defaultInterceptors.Combine((ICommunicationInterceptor) new FailFastHandlerInterceptor());
      return defaultInterceptors;
    }

    /// <summary>
    /// The communication Timeout that is set initially on the <see cref="T:TwinCAT.Ads.AdsClient" />
    /// </summary>
    /// <value>The timeout.</value>
    public int Timeout => this._timeout;
  }
}
