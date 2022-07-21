// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsSessionBase
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using TwinCAT.Ads.Internal;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Abstract base class for ADS Sessions.</summary>
  public abstract class AdsSessionBase : 
    Session,
    IAdsSession,
    ISession,
    IConnectionStateProvider,
    ISymbolServerProvider,
    IInterceptionFactory
  {
    private IAdsClientFactory _clientFactory;
    /// <summary>The address</summary>
    private AmsAddress _address;
    /// <summary>The Session Settings settings</summary>
    private SessionSettings _settings;
    /// <summary>The logger interface</summary>
    private ILogger? _logger;
    /// <summary>Interceptor collection</summary>
    private CommunicationInterceptors? _interceptor;
    /// <summary>Fail fast handler</summary>
    private FailFastHandlerInterceptor? _failFastHandlerInterceptor;
    /// <summary>Connection observer</summary>
    private ConnectionStateInterceptor? _connectionStateObserver;
    /// <summary>The session owner</summary>
    private object? _owner;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsSessionBase" /> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="factory">The client factory</param>
    /// <param name="logger">The logger.</param>
    /// <param name="owner">The session owner</param>
    /// <exception cref="T:System.ArgumentNullException">address</exception>
    protected AdsSessionBase(
      AmsAddress address,
      SessionSettings settings,
      IAdsClientFactory factory,
      ILogger? logger,
      object? owner)
      : base((ISessionProvider) AdsSessionProvider.Self)
    {
      if (AmsAddress.op_Equality(address, (AmsAddress) null))
        throw new ArgumentNullException(nameof (address));
      this._logger = logger;
      this._owner = owner;
      this._address = address.Clone();
      this._settings = settings;
      this._clientFactory = factory;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.AdsSessionBase" /> class.
    /// </summary>
    ~AdsSessionBase() => this.Dispose(false);

    /// <summary>
    /// Gets the target address of the <see cref="T:TwinCAT.Ads.AdsSessionBase" />
    /// </summary>
    /// <value>The address.</value>
    public AmsAddress Address => this._address;

    /// <summary>Handler function getting the address of the session.</summary>
    /// <returns>System.String.</returns>
    protected override string OnGetAddress() => ((object) this._address).ToString();

    /// <summary>Gets the settings of the Session/Connection.</summary>
    /// <value>The settings.</value>
    public SessionSettings Settings => this._settings;

    /// <summary>Gets the logger interface or null.</summary>
    /// <value>The logger.</value>
    public ILogger? Logger => this._logger;

    /// <summary>Gets the connection.</summary>
    /// <value>The connection.</value>
    public AdsConnection? Connection
    {
      get => (AdsConnection) base.Connection;
      protected set => this.Connection = (IConnection) value;
    }

    /// <summary>
    /// Ensures, that the <see cref="T:TwinCAT.ISession" /> is connected and returns the <see cref="T:TwinCAT.IConnection" /> object.
    /// </summary>
    /// <returns>IConnection.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <remarks>If the session is actually not connected an exception will be thrown.</remarks>
    public AdsConnection EnsureConnection()
    {
      AdsConnection connection = this.Connection;
      if (connection == null || !this.IsConnected || !connection.IsConnected)
        throw new SessionNotConnectedException((ISession) this);
      return connection;
    }

    /// <summary>Handler function connecting the Session.</summary>
    /// <returns>IConnection.</returns>
    protected override IConnection? OnConnect(bool reconnect)
    {
      if (this.Connection != null)
      {
        this.Connection.Connect();
      }
      else
      {
        AdsConnection adsConnection = new AdsConnection(this, this._clientFactory);
        adsConnection.ConnectionStateChanged += new EventHandler<ConnectionStateChangedEventArgs>(((Session) this).OnConnectionStateChanged);
        adsConnection.Connect();
        adsConnection.RouterStateChanged += new EventHandler<AmsRouterNotificationEventArgs>(this.OnRouterStateChanged);
        this.Connection = adsConnection;
      }
      return base.OnConnect(reconnect);
    }

    /// <summary>Called when [disconnect].</summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected override bool OnDisconnect()
    {
      AdsConnection connection = this.Connection;
      return base.OnDisconnect();
    }

    /// <summary>
    /// Handles the AmsRouterNotification event of the _connection control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:TwinCAT.Ads.AmsRouterNotificationEventArgs" /> instance containing the event data.</param>
    private void OnRouterStateChanged(object? sender, AmsRouterNotificationEventArgs e)
    {
      AmsRouterState state = e.State;
    }

    /// <summary>Gets the name/string identifier of the session.</summary>
    /// <returns>System.String.</returns>
    protected override string GetSessionName()
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
      interpolatedStringHandler.AppendLiteral("Session ");
      interpolatedStringHandler.AppendFormatted<AmsNetId>(this._address.NetId);
      interpolatedStringHandler.AppendLiteral(":");
      interpolatedStringHandler.AppendFormatted<int>(this._address.Port);
      return interpolatedStringHandler.ToStringAndClear();
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        AdsConnection connection = this.Connection;
        if (connection != null)
          connection.RouterStateChanged -= new EventHandler<AmsRouterNotificationEventArgs>(this.OnRouterStateChanged);
        this._failFastHandlerInterceptor = (FailFastHandlerInterceptor) null;
        this._connectionStateObserver = (ConnectionStateInterceptor) null;
        this._interceptor = (CommunicationInterceptors) null;
      }
      base.Dispose(disposing);
    }

    /// <summary>Gets the Connection observer</summary>
    /// <value>Connection observer.</value>
    internal ConnectionStateInterceptor? ConnectionObserver => this._connectionStateObserver;

    /// <summary>Creates the interceptor.</summary>
    /// <returns>ICommunicationInterceptor.</returns>
    ICommunicationInterceptor IInterceptionFactory.CreateInterceptor()
    {
      if (this._interceptor == null)
      {
        CommunicationInterceptors communicationInterceptors = new CommunicationInterceptors();
        if (this._settings.ResurrectionTime > TimeSpan.Zero)
        {
          this._failFastHandlerInterceptor = new FailFastHandlerInterceptor(this._settings.ResurrectionTime);
          communicationInterceptors.Combine((ICommunicationInterceptor) this._failFastHandlerInterceptor);
        }
        this._connectionStateObserver = new ConnectionStateInterceptor((IAdsSession) this);
        communicationInterceptors.Combine((ICommunicationInterceptor) this._connectionStateObserver);
        this._interceptor = communicationInterceptors;
      }
      return (ICommunicationInterceptor) this._interceptor;
    }

    /// <summary>Handler function creating the symbol server object.</summary>
    /// <returns>ISymbolServer.</returns>
    /// <exception cref="T:TwinCAT.SessionNotConnectedException">The connection is not established!</exception>
    protected override ISymbolServer OnCreateSymbolServer() => (ISymbolServer) new AdsSymbolServer(this);

    /// <summary>Gets the Communication / Session statistics.</summary>
    /// <value>The communication / Session statistics.</value>
    public AdsCommunicationStatistics Statistics => new AdsCommunicationStatistics(this);

    /// <summary>Gets the NetId of the Session</summary>
    /// <value>The net identifier.</value>
    public AmsNetId NetId => this._address.NetId;

    /// <summary>Gets the Ams Port of the Session</summary>
    /// <value>The port.</value>
    public int Port => this._address.Port;

    /// <summary>Gets the Session owner.</summary>
    /// <value>The owner or NULL</value>
    public object? Owner => this._owner;
  }
}
