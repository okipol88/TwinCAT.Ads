// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsConnection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>ADS Connection class</summary>
  /// <remarks>The ADS Connection class represents an ADS Point-to-Point Connection between client and server. It is established by using the
  /// Connect method of the <see cref="T:TwinCAT.Ads.AdsSession" /> object.
  /// An ADS Connection can have different <see cref="P:TwinCAT.Ads.AdsConnection.ConnectionState">ConnectionStates</see>, which represent the state of the logical ADS connection.</remarks>
  /// <seealso cref="T:TwinCAT.Ads.AdsSession" />
  /// <seealso cref="T:TwinCAT.Ads.IAdsConnection" />
  /// <seealso cref="T:System.IDisposable" />
  [DebuggerDisplay("Id = {Id} : Address: {Address}, State: {ConnectionState}")]
  public sealed class AdsConnection : 
    IAdsConnection,
    IConnection,
    IConnectionStateProvider,
    IAdsNotifications,
    IAdsSymbolicAccess,
    IAdsAnyAccess,
    IAdsHandle,
    IAdsReadWrite2,
    IAdsReadWrite,
    IAdsStateProvider,
    IAdsStateControl,
    IAdsSymbolChangedProvider,
    IAdsRpcInvoke,
    IAdsInjectAcceptor,
    IAdsReadWriteTimeoutAccess,
    IAdsStateControlTimeout,
    IAdsSymbolLoaderFactory,
    IAdsHandleTableProvider,
    IDisposable
  {
    private static int s_id;
    /// <summary>The session</summary>
    private AdsSessionBase _session;
    /// <summary>Connection ID</summary>
    private int _id = ++AdsConnection.s_id;
    /// <summary>Internal used client.</summary>
    private IAdsConnection _client;
    /// <summary>Handler that is called before Read/Write</summary>
    private Action? _beforeAccessDelegate;
    /// <summary>Handler that is called after Read/Write</summary>
    private Action? _afterAccessDelegate;
    private bool _resurrecting;
    /// <summary>The actual connection state</summary>
    private ConnectionState _connectionState = (ConnectionState) 1;
    /// <summary>The timestamp of the connection loss</summary>
    private DateTimeOffset? _lostTime;
    /// <summary>
    /// Indicates that the <see cref="T:TwinCAT.Ads.AdsConnection" /> is disposed.
    /// </summary>
    private bool _disposed;
    /// <summary>The connection establish time</summary>
    private DateTimeOffset? _connectionEstablishTime;
    /// <summary>
    /// The UTC time when tha last active/resurrected Connection was established
    /// </summary>
    private DateTimeOffset? _connectionActiveSince;
    private int _resurrectingTryCount;
    /// <summary>The number of resurrections.</summary>
    private int _resurrections;
    private int _connectionLostCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsConnection" /> class.
    /// </summary>
    /// <param name="session">The session.</param>
    /// <param name="factory">The client factory</param>
    internal AdsConnection(AdsSessionBase session, IAdsClientFactory factory)
    {
      this._session = session != null ? session : throw new ArgumentNullException(nameof (session));
      this._client = factory.Create((ISession) session);
      this.createResurrectionHandler();
      IConnectionStateObserver connectionObserver = this.ConnectionObserver;
      if (connectionObserver != null)
      {
        ((ConnectionStateInterceptor) connectionObserver).AdsStateChanged += new EventHandler<AdsStateChangedEventArgs2>(this.ConnectionObserver_AdsStateChanged);
        ((IConnectionStateProvider) connectionObserver).ConnectionStateChanged += new EventHandler<ConnectionStateChangedEventArgs>(this.ConnectionObserver_ConnectionStateChanged);
      }
      this._connectionEstablishTime = new DateTimeOffset?(DateTimeOffset.Now);
    }

    /// <summary>
    /// Gets the Session object of the <see cref="T:TwinCAT.Ads.AdsConnection" /> object.
    /// </summary>
    /// <value>The client.</value>
    public ISession Session => (ISession) this._session;

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.Ads.AdsConnection" /> identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public int Id => this._id;

    /// <summary>Gets the default value encoding.</summary>
    /// <value>The default value encoding.</value>
    public Encoding DefaultValueEncoding => ((IConnection) this._client).DefaultValueEncoding;

    /// <summary>
    /// Gets the used <see cref="T:TwinCAT.Ads.AdsClient" /> of the <see cref="T:TwinCAT.Ads.AdsConnection" /> object.
    /// </summary>
    /// <value>The client.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IAdsConnection Client => this._client;

    /// <summary>Creates the interceptor delegates/handlers.</summary>
    /// <remarks>
    /// This implementation resurrects when ConnectionState is Lost,
    /// which means the connection IsConnected.
    /// </remarks>
    private void createResurrectionHandler()
    {
      this._beforeAccessDelegate = (Action) (() =>
      {
        if (this._client == null || this._connectionState != 3)
          return;
        DateTimeOffset now = DateTimeOffset.Now;
        TimeSpan resurrectionTime = this._session.Settings.ResurrectionTime;
        DateTimeOffset dateTimeOffset = now;
        DateTimeOffset? lostTime = this._lostTime;
        TimeSpan? nullable = lostTime.HasValue ? new TimeSpan?(dateTimeOffset - lostTime.GetValueOrDefault()) : new TimeSpan?();
        if ((nullable.HasValue ? (resurrectionTime < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          return;
        this.OnResurrect();
      });
      this._afterAccessDelegate = (Action) null;
    }

    /// <summary>Gets the access wait time.</summary>
    /// <remarks>
    /// Gets the Wait Time until the next communication try will be done.
    /// This time is calculated as follows:
    ///     ResurrectionTime - (DateTime.Now - ConnectionLostTime)
    /// </remarks>
    /// <value>The access wait time.</value>
    /// <seealso cref="P:TwinCAT.Ads.AdsConnection.ConnectionLostTime" />
    /// <seealso cref="P:TwinCAT.Ads.SessionSettings.ResurrectionTime" />
    public TimeSpan AccessWaitTime
    {
      get
      {
        if (this._disposed)
          return TimeSpan.MaxValue;
        TimeSpan accessWaitTime = TimeSpan.Zero;
        if (this._lostTime.HasValue)
        {
          accessWaitTime = this._session.Settings.ResurrectionTime - (DateTimeOffset.Now - this._lostTime.Value);
          if (accessWaitTime < TimeSpan.Zero)
            accessWaitTime = TimeSpan.Zero;
        }
        return accessWaitTime;
      }
    }

    /// <summary>
    /// Handler that is called before the Connection access (enables the Resurrection)
    /// </summary>
    /// <remarks>
    /// Calling this is important for the Connection Resurrection Handler!
    /// Resurrection is only tried when the Connection is neither Disposed nor
    /// Disconnected
    /// </remarks>
    private void BeforeAccess()
    {
      if (this._beforeAccessDelegate == null)
        return;
      this._beforeAccessDelegate();
    }

    /// <summary>Handler that is called after Connection access.</summary>
    private void AfterAccess()
    {
      if (this._afterAccessDelegate == null)
        return;
      this._afterAccessDelegate();
    }

    /// <summary>
    /// Resurrection handler of the <see cref="T:TwinCAT.Ads.AdsConnection" />.
    /// </summary>
    private void OnResurrect()
    {
      AdsModule.Trace.TraceInformation("Resurrect");
      this.TryResurrect(out AdsException _);
    }

    /// <summary>
    /// Resurrects the <see cref="T:TwinCAT.Ads.AdsConnection" />
    /// </summary>
    /// <exception cref="T:TwinCAT.AdsException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.TryResurrect(TwinCAT.AdsException@)" />
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Resurrect()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.ToString());
      if (!this.TryResurrect(out AdsException _))
        throw new AdsException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryResurrect([NotNullWhen(false)] out AdsException? error)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.ToString());
      if (!this.IsLost)
      {
        error = (AdsException) null;
        return true;
      }
      bool flag;
      try
      {
        this._resurrecting = true;
        ++this._resurrectingTryCount;
        flag = ((IInterceptedClient) this._client).TryResurrect(out error);
        if (!flag)
        {
          AdsModule.TraceSession.TraceError("Resurrection failed!", (Exception) error);
          this.OnLost();
        }
        ++this._resurrections;
      }
      finally
      {
        this._resurrecting = false;
      }
      return flag;
    }

    /// <summary>
    /// (Re)Connects the <see cref="T:TwinCAT.IConnection" /> when disconnected.
    /// </summary>
    /// <returns><c>true</c> if the <see cref="T:TwinCAT.Ads.AdsConnection" /> is reconnected, <c>false</c> otherwise.</returns>
    public bool Connect()
    {
      if (this.IsConnected)
        return false;
      this.OnConnect();
      return true;
    }

    /// <summary>Connect handler.</summary>
    private void OnConnect()
    {
      IConnection client1 = (IConnection) this._client;
      if (this._client is IAdsConnectAddress client2)
        client2.Connect(this._session.Address);
      else
        client1.Connect();
      this._connectionActiveSince = new DateTimeOffset?(DateTimeOffset.Now);
      this._lostTime = new DateTimeOffset?();
    }

    private CommunicationInterceptors? Interceptors => this.Client is IInterceptedClient client ? client.Interceptors : (CommunicationInterceptors) null;

    /// <summary>
    /// Gets the current <see cref="P:TwinCAT.Ads.AdsConnection.ConnectionState" />
    /// </summary>
    /// <value>The state.</value>
    public ConnectionState State => this._connectionState;

    /// <summary>
    /// Handles the ConnectionStateChanged event of the ConnectionObserver control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:TwinCAT.SessionConnectionStateChangedEventArgs" /> instance containing the event data.</param>
    private void ConnectionObserver_ConnectionStateChanged(
      object? sender,
      ConnectionStateChangedEventArgs e)
    {
      if (this._disposed)
        return;
      this._connectionState = e.NewState;
      if (e.NewState == 2)
        this.OnConnected();
      else if (e.NewState == 1)
        this.OnDisconnected();
      else if (e.NewState == 3)
        this.OnLost();
      this.OnConnectionStatusChanged(e);
    }

    /// <summary>
    /// Gets the current Connection state of the <see cref="T:TwinCAT.Ads.AdsConnection" />
    /// </summary>
    /// <value>The state of the connection.</value>
    /// <remarks>The Connection state changes only if the <see cref="T:TwinCAT.IConnection" /> is established / shut down
    /// or active communication is triggered by the User of the <see cref="T:TwinCAT.IConnection" /> object.
    /// </remarks>
    /// <example>
    /// The following sample shows how to keep the <see cref="P:TwinCAT.Ads.AdsConnection.ConnectionState" /> updated by triggering ADS Communication.
    /// <code language="C#" title="Trigger ConnectionState changes in WPF Applications" source="..\..\Samples\TwinCAT.ADS.NET_Samples\40_ADS.NET_WPFConnectionObserver\MainWindow.xaml.cs" region="CODE_SAMPLE" />
    /// </example>
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.ConnectionStateChanged" />
    public ConnectionState ConnectionState
    {
      get
      {
        if (this._disposed)
          return (ConnectionState) 0;
        IConnectionStateObserver connectionObserver = this.ConnectionObserver;
        return connectionObserver == null ? (ConnectionState) 0 : ((IConnectionStateProvider) connectionObserver).ConnectionState;
      }
    }

    /// <summary>
    /// Occurs when connection status of the <see cref="T:TwinCAT.Ads.AdsConnection" /> has been changed.
    /// </summary>
    /// <remarks>The Connection state changes only if the <see cref="T:TwinCAT.IConnection" /> is established / shut down
    /// or active communication is triggered by the User of the <see cref="T:TwinCAT.IConnection" /> object.
    /// </remarks>
    /// <example>
    /// The following sample shows how to keep the <see cref="P:TwinCAT.Ads.AdsConnection.ConnectionState" /> updated by triggering ADS Communication.
    /// <code language="C#" title="Trigger ConnectionState changes in WPF Applications" source="..\..\Samples\TwinCAT.ADS.NET_Samples\40_ADS.NET_WPFConnectionObserver\MainWindow.xaml.cs" region="CODE_SAMPLE" />
    /// </example>
    /// <seealso cref="P:TwinCAT.Ads.AdsConnection.ConnectionState" />
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Handles the <see cref="E:ConnectionStatusChanged" /> event.
    /// </summary>
    /// <param name="args">The <see cref="T:TwinCAT.ConnectionStateChangedEventArgs" /> instance containing the event data.</param>
    private void OnConnectionStatusChanged(ConnectionStateChangedEventArgs args)
    {
      if (this._disposed || this.ConnectionStateChanged == null)
        return;
      this.ConnectionStateChanged((object) this, (ConnectionStateChangedEventArgs) new SessionConnectionStateChangedEventArgs(args.Reason, args.NewState, args.OldState, this.Session, (IConnection) this, args.Exception));
    }

    /// <summary>Occurs when Device state has been changed.</summary>
    internal event EventHandler<AdsStateChangedEventArgs2>? DeviceStateChanged;

    /// <summary>
    /// Handles the <see cref="E:AdsStateChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="T:TwinCAT.Ads.AdsStateChangedEventArgs" /> instance containing the event data.</param>
    private void OnAdsStateChanged(AdsStateChangedEventArgs2 e)
    {
      if (this._disposed || this.DeviceStateChanged == null)
        return;
      this.DeviceStateChanged((object) this, e);
    }

    /// <summary>Gets the connection observer.</summary>
    /// <value>The connection observer.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal IConnectionStateObserver? ConnectionObserver
    {
      get
      {
        if (this._disposed)
          return (IConnectionStateObserver) null;
        CommunicationInterceptors interceptors = this.Interceptors;
        return interceptors == null ? (IConnectionStateObserver) null : (IConnectionStateObserver) interceptors.Lookup(typeof (ConnectionStateInterceptor));
      }
    }

    /// <summary>Gets the ads state observer.</summary>
    /// <value>The ads state observer.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IAdsStateObserver? AdsStateObserver
    {
      get
      {
        if (this._disposed)
          return (IAdsStateObserver) null;
        CommunicationInterceptors interceptors = this.Interceptors;
        return interceptors == null ? (IAdsStateObserver) null : (IAdsStateObserver) interceptors.Lookup(typeof (ConnectionStateInterceptor));
      }
    }

    /// <summary>
    /// Called when the <see cref="T:TwinCAT.Ads.AdsConnection" /> is established.
    /// </summary>
    private void OnConnected()
    {
      AdsModule.Trace.TraceInformation("Connection established");
      this._lostTime = new DateTimeOffset?();
    }

    /// <summary>
    /// Called when the <see cref="T:TwinCAT.Ads.AdsConnection" /> is closed.
    /// </summary>
    private void OnDisconnected()
    {
      AdsModule.Trace.TraceInformation("Connection closed");
      this._lostTime = new DateTimeOffset?();
      this._connectionActiveSince = new DateTimeOffset?();
      this._connectionEstablishTime = new DateTimeOffset?();
    }

    /// <summary>Gets the connection lost time.</summary>
    /// <value>The connection lost time.</value>
    public DateTimeOffset? ConnectionLostTime => this._lostTime;

    /// <summary>Called when the connection has been lost.</summary>
    private void OnLost()
    {
      AdsModule.TraceSession.TraceInformation("Connection lost");
      this._lostTime = new DateTimeOffset?(DateTimeOffset.Now);
      this._connectionActiveSince = new DateTimeOffset?();
      ++this._connectionLostCount;
    }

    /// <summary>
    /// Gets a value indicating whether the communication is in lost / open state
    /// </summary>
    /// <value><c>true</c> if this instance is lost; otherwise, <c>false</c>.</value>
    public bool IsLost
    {
      get
      {
        if (this._disposed || !this.IsConnected)
          return false;
        CommunicationInterceptors interceptors = this.Interceptors;
        if (interceptors == null)
          return false;
        IFailFastHandler failFastHandler = interceptors.Find<IFailFastHandler>();
        return failFastHandler != null && failFastHandler.IsLost;
      }
    }

    /// <summary>
    /// Gets a value indicating whether communication is in active state
    /// </summary>
    /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
    public bool IsActive
    {
      get
      {
        if (this._disposed || !this.IsConnected)
          return false;
        CommunicationInterceptors interceptors = this.Interceptors;
        if (interceptors == null)
          return true;
        IFailFastHandler failFastHandler = interceptors.Find<IFailFastHandler>();
        return failFastHandler == null || failFastHandler.IsActive;
      }
    }

    /// <summary>
    /// Gets a value indicating whether communication is ready for reconnecting
    /// </summary>
    /// <value><c>true</c> if this instance is reconnecting; otherwise, <c>false</c>.</value>
    public bool IsReconnecting
    {
      get
      {
        if (this._disposed || !this.IsConnected)
          return false;
        CommunicationInterceptors interceptors = this.Interceptors;
        if (interceptors == null)
          return false;
        IFailFastHandler failFastHandler = interceptors.Find<IFailFastHandler>();
        return failFastHandler != null && failFastHandler.IsReconnecting;
      }
    }

    /// <summary>
    /// Handles the AdsStateChanged event of the ConnectionObserver control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:TwinCAT.Ads.AdsStateChangedEventArgs" /> instance containing the event data.</param>
    private void ConnectionObserver_AdsStateChanged(object? sender, AdsStateChangedEventArgs2 e)
    {
      if (this._disposed)
        return;
      this.OnAdsStateChanged(e);
    }

    /// <summary>
    /// Error injection only for Unit-Test purposes to simulate error conditions
    /// </summary>
    /// <param name="error">The error.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public AdsErrorCode InjectError(AdsErrorCode error)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return ((IAdsInjectAcceptor) this.Client).InjectError(error);
    }

    /// <summary>
    /// Injection of an SymbolVersionChanged event (just for Testing purposes)
    /// </summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public void InjectSymbolVersionChanged()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ((IAdsInjectAcceptor) this.Client).InjectSymbolVersionChanged();
    }

    /// <summary>
    /// Get the <see cref="T:TwinCAT.Ads.AmsAddress" /> of the ADS client.
    /// </summary>
    /// <value>The client address.</value>
    public AmsAddress ClientAddress => this._disposed ? AmsAddress.Empty : this._session.Address;

    /// <summary>
    /// Gets a value indicating whether the local ADS port was opened successfully. It
    /// does not indicate if the target port is available. Use the method ReadState to
    /// determine if the target port is available.
    /// </summary>
    /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
    public bool IsConnected
    {
      get
      {
        if (this._disposed)
          return false;
        return this._connectionState == 2 || this._connectionState == 3;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the ADS client is connected to a ADS Server on the local
    /// computer.
    /// </summary>
    /// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
    public bool IsLocal => this._disposed || this._session.Address.NetId.IsLocal;

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.Ads.AmsAddress" /> of the ADS server.
    /// </summary>
    /// <value>The server address.</value>
    public AmsAddress Address => this._session.Address;

    /// <summary>
    /// Occurs when Notifications are send (bundled notifications)
    /// </summary>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <example>
    /// Example of receiving <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsSumNotification" /> events.
    /// <code source="..\..\Samples\TwinCAT.ADS.NET_Samples\03_ADS.NET_EventReading\Form1.cs" region="CODE_SAMPLE_SUMNOTIFICATIONS_ASYNC" removeRegionMarkers="true" language="csharp" title="Trigger on changed values by ADS Notifications" /></example>
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" />
    /// <remarks>As an optimization, this event receives all ADS Notifications that occurred at one
    /// point in time together. As consequence, the overhead of handler code is reduced, what can be important
    /// if notifications are triggered in a high frequency and the event has to be synchronized to the UI thread
    /// context. Because multiple notifications are bound together, less thread synchronization is necessary.
    /// The <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" /> and <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotificationEx" /> events shouldn't be used when SumNotifications are registered, because they
    /// have an performance side effect to this <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsSumNotification" /> event. The full performance is reached only, when all notifications are handled
    /// on this event.</remarks>
    public event EventHandler<AdsSumNotificationEventArgs> AdsSumNotification
    {
      add
      {
        if (this._disposed)
          throw new ObjectDisposedException(this.Name);
        ((IAdsNotifications) this._client).AdsSumNotification += value;
      }
      remove
      {
        if (this._client == null)
          return;
        ((IAdsNotifications) this._client).AdsSumNotification -= value;
      }
    }

    /// <summary>
    /// Occurs when the ADS device sends a notification to the client.
    /// </summary>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <example>
    /// Example of receiving <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" /> events.
    /// <code source="..\..\Samples\TwinCAT.ADS.NET_Samples\03_ADS.NET_EventReading\Form1.cs" region="CODE_SAMPLE_NOTIFICATIONS_ASYNC" removeRegionMarkers="true" language="csharp" title="Trigger on changed values by ADS Notifications" /></example>
    /// <remarks>The Event Argument contains the raw data value of the notification, not marshalled to .NET types.</remarks>
    public event EventHandler<AdsNotificationEventArgs>? AdsNotification
    {
      add
      {
        if (this._disposed)
          throw new ObjectDisposedException(this.Name);
        ((IAdsNotifications) this._client).AdsNotification += value;
      }
      remove
      {
        if (this._client == null)
          return;
        ((IAdsNotifications) this._client).AdsNotification -= value;
      }
    }

    /// <summary>
    /// Occurs when a exception has occurred during notification management.
    /// </summary>
    /// <remarks>
    /// The occurrence of this event can have two different reasons:
    /// <list type="number">
    /// <item>Indicates an internal error occurred during Notification management.</item>
    /// <item>The registered notification becomes invalid on the server, eg. after a PLC Download / Online Change. If the ADS Server detects that the (still registered) Notification Sender is getting invalid, it sends
    /// an error notification so that the client will be informed about detached notifications. The event arguments contains the <see cref="T:TwinCAT.Ads.AdsInvalidNotificationException" /> which describes the invalid notification handle
    /// by its <see cref="P:TwinCAT.Ads.AdsInvalidNotificationException.Handle" /> property.</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:TwinCAT.Ads.AdsInvalidNotificationException" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" />
    public event EventHandler<AdsNotificationErrorEventArgs>? AdsNotificationError
    {
      add
      {
        if (this._disposed)
          throw new ObjectDisposedException(this.Name);
        ((IAdsNotifications) this._client).AdsNotificationError += value;
      }
      remove
      {
        if (this._client == null)
          return;
        ((IAdsNotifications) this._client).AdsNotificationError -= value;
      }
    }

    /// <summary>
    /// Occurs when the ADS devices sends a notification to the client.
    /// </summary>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <example>
    /// Example of receiving <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotificationEx" /> events.
    /// <code source="..\..\Samples\TwinCAT.ADS.NET_Samples\14_ADS.NET_ReadWriteAnyType\Form1.cs" region="CODE_READWRITEANYNOT_ASYNC" removeRegionMarkers="true" language="csharp" title="Trigger on changed values by ADS Notifications" /></example>
    /// <remarks>The Notification event arguments marshals the data value automatically to the specified .NET Type with ANY_TYPE marshallers.</remarks>
    public event EventHandler<AdsNotificationExEventArgs>? AdsNotificationEx
    {
      add
      {
        if (this._disposed)
          throw new ObjectDisposedException(this.Name);
        ((IAdsNotifications) this._client).AdsNotificationEx += value;
      }
      remove
      {
        if (this._client == null)
          return;
        ((IAdsNotifications) this._client).AdsNotificationEx -= value;
      }
    }

    /// <summary>Occurs when ADS State has been changed.</summary>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <remarks>This event occurs asynchronously if the synchronized flag is not set.</remarks>
    public event EventHandler<AdsStateChangedEventArgs> AdsStateChanged
    {
      add
      {
        if (this._disposed)
          throw new ObjectDisposedException(this.Name);
        ((IAdsStateProvider) this._client).AdsStateChanged += value;
      }
      remove
      {
        if (this._client == null)
          return;
        ((IAdsStateProvider) this._client).AdsStateChanged -= value;
      }
    }

    /// <summary>
    /// Registers for <see cref="E:TwinCAT.Ads.AdsConnection.AdsStateChanged" /> events as an asynchronous operation.
    /// </summary>
    /// <param name="handler">The handler function to be registered for AdsStateChanged calls.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'RegisterAdsStateChanged' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the state
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultAds> RegisterAdsStateChangedAsync(
      EventHandler<AdsStateChangedEventArgs> handler,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      return ((IAdsStateProvider) this._client).RegisterAdsStateChangedAsync(handler, cancel);
    }

    /// <summary>
    /// Registers for <see cref="E:TwinCAT.Ads.AdsConnection.AdsStateChanged" /> events as an asynchronous operation.
    /// </summary>
    /// <param name="handler">The handler function to be unregistered.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'UnregisterAdsStateChanged' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the state
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultAds> UnregisterAdsStateChangedAsync(
      EventHandler<AdsStateChangedEventArgs> handler,
      CancellationToken cancel)
    {
      return ((IAdsStateProvider) this._client).UnregisterAdsStateChangedAsync(handler, cancel);
    }

    /// <summary>Occurs when the symbol version has been changed.</summary>
    /// <remarks>This is the case when the connected ADS server restarts. This invalidates all actual opened
    /// symbol handles.
    /// The SymbolVersion counter doesn't trigger, when an online change is made on the PLC (ports 801, ..., 851 ...)</remarks>
    public event EventHandler<AdsSymbolVersionChangedEventArgs> AdsSymbolVersionChanged
    {
      add
      {
        if (this._disposed)
          throw new ObjectDisposedException(this.Name);
        ((IAdsSymbolChangedProvider) this._client).AdsSymbolVersionChanged += value;
      }
      remove
      {
        if (this._client == null)
          return;
        ((IAdsSymbolChangedProvider) this._client).AdsSymbolVersionChanged -= value;
      }
    }

    /// <summary>Registers the symbol version changed asynchronously.</summary>
    /// <param name="handler">The handler function to register.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'RegisterSymbolVersionChanged' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public Task<ResultAds> RegisterSymbolVersionChangedAsync(
      EventHandler<AdsSymbolVersionChangedEventArgs> handler,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      return ((IAdsSymbolChangedProvider) this._client).RegisterSymbolVersionChangedAsync(handler, cancel);
    }

    /// <summary>Unregisters the symbol version changed asynchronous.</summary>
    /// <param name="handler">The handler function to unregister.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'UnregisterSymbolVersionChangedAsync' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public Task<ResultAds> UnregisterSymbolVersionChangedAsync(
      EventHandler<AdsSymbolVersionChangedEventArgs> handler,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      return ((IAdsSymbolChangedProvider) this._client).UnregisterSymbolVersionChangedAsync(handler, cancel);
    }

    /// <summary>Occurs when [router state changed].</summary>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public event EventHandler<AmsRouterNotificationEventArgs> RouterStateChanged
    {
      add
      {
        if (this._disposed)
          throw new ObjectDisposedException(this.Name);
        ((IRouterNotificationProvider) this._client).RouterStateChanged += value;
      }
      remove
      {
        if (this._client == null)
          return;
        ((IRouterNotificationProvider) this._client).RouterStateChanged -= value;
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> event.
    /// </summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <returns>The notification handle.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    public uint AddDeviceNotification(
      string variableName,
      int dataSize,
      NotificationSettings settings,
      object? userData)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).AddDeviceNotification(variableName, dataSize, settings, userData);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the AdsNotification event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <returns>The notification handle.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    public uint AddDeviceNotification(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object? userData)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).AddDeviceNotification(indexGroup, indexOffset, dataSize, settings, userData);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> event.
    /// </summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="handle">The notification handle.</param>
    /// <returns>The ADS ErrorCode.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.TryDeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.TryDeleteDeviceNotification(System.UInt32)" /> should always
    /// be called when the notification is not used anymore.</remarks>
    public AdsErrorCode TryAddDeviceNotification(
      string variableName,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      out uint handle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).TryAddDeviceNotification(variableName, dataSize, settings, userData, ref handle);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the AdsNotification event.
    /// </summary>
    /// <param name="symbolPath">The symbol path..</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="type">Type of the object stored in the event argument.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="handle">The handle.</param>
    /// <returns>The handle of the notification.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public AdsErrorCode TryAddDeviceNotificationEx(
      string symbolPath,
      NotificationSettings settings,
      object? userData,
      Type type,
      int[]? args,
      out uint handle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).TryAddDeviceNotificationEx(symbolPath, settings, userData, type, args, ref handle);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="symbolPath">Symbol/Instance path of the ADS variable.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <returns>The notification handle.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    public uint AddDeviceNotificationEx(
      string symbolPath,
      NotificationSettings settings,
      object? userData,
      Type type)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).AddDeviceNotificationEx(symbolPath, settings, userData, type);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <returns>The notification handle.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    public uint AddDeviceNotificationEx(
      uint indexGroup,
      uint indexOffset,
      NotificationSettings settings,
      object? userData,
      Type type)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).AddDeviceNotificationEx(indexGroup, indexOffset, settings, userData, type);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="symbolPath">Symbol/Instance path of the ADS variable.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <param name="args">Additional arguments (for 'AnyType')</param>
    /// <returns>The notification handle.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    public uint AddDeviceNotificationEx(
      string symbolPath,
      NotificationSettings settings,
      object? userData,
      Type type,
      int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).AddDeviceNotificationEx(symbolPath, settings, userData, type, args);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the AdsNotification event.
    /// If type is a string type, the first element of the parameter args specifies the number of characters of the string.
    /// If type is an array type, the number of elements for each dimension has to be specified in the parameter args.
    /// Only primitive ('AnyType') types are allowed for the parameter <paramref name="type" />.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="type">Type of the object stored in the event argument.</param>
    /// <param name="args">Additional arguments for 'AnyType' types.</param>
    /// <returns>The notification handle.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    public uint AddDeviceNotificationEx(
      uint indexGroup,
      uint indexOffset,
      NotificationSettings settings,
      object? userData,
      Type type,
      int[] args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).AddDeviceNotificationEx(indexGroup, indexOffset, settings, userData, type, args);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Generates a unique handle for an ADS variable.</summary>
    /// <param name="variableName">Name of the ADS variable</param>
    /// <returns>The handle of the ADS Variable.</returns>
    public uint CreateVariableHandle(string variableName)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      uint variableHandle;
      try
      {
        ((IAdsHandle) this._client).TryCreateVariableHandle(variableName, ref variableHandle).ThrowOnError();
      }
      finally
      {
        this.AfterAccess();
      }
      return variableHandle;
    }

    /// <summary>Deletes an existing notification.</summary>
    /// <param name="notificationHandle">Handle of the notification.</param>
    public void DeleteDeviceNotification(uint notificationHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ((IAdsNotifications) this._client).TryDeleteDeviceNotification(notificationHandle).ThrowOnError();
    }

    /// <summary>Releases the handle of a ADS variable again.</summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    public void DeleteVariableHandle(uint variableHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      IHandleTable handleTable = ((IAdsHandleTableProvider) this).GetHandleTable();
      if (handleTable == null)
        return;
      handleTable.TryDeleteVariableHandle(variableHandle, this.Timeout).ThrowOnError();
    }

    /// <summary>
    /// Releases the specified symbol/variable handle synchronously.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    /// <returns>The ADS error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.TryCreateVariableHandle(System.String,System.UInt32@)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteVariableHandle(System.UInt32)" />
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.AdsConnection.TryDeleteVariableHandle(System.UInt32)" /> is the <see cref="M:TwinCAT.Ads.AdsConnection.TryCreateVariableHandle(System.String,System.UInt32@)" /></remarks>
    public AdsErrorCode TryDeleteVariableHandle(uint variableHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return ((IAdsHandleTableProvider) this).GetHandleTable().TryDeleteVariableHandle(variableHandle, this.Timeout);
    }

    /// <summary>
    /// Reads the value from the symbol that is represented by the handle.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public int Read(uint variableHandle, Memory<byte> buffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsHandle) this._client).Read(variableHandle, buffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to the given <paramref name="readBuffer" />
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">Memory location, where to read the data.</param>
    /// <returns>Number of successfully returned (read) data bytes.</returns>
    public int Read(uint indexGroup, uint indexOffset, Memory<byte> readBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsReadWrite2) this._client).Read(indexGroup, indexOffset, readBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <returns>The read object.</returns>
    public object ReadAny(uint variableHandle, Type type)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAny(variableHandle, type);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data asynchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    /// <remarks>
    /// As object types only primitive types are supported.
    /// If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// <list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public async Task<ResultAnyValue> ReadAnyAsync(
      uint indexGroup,
      uint indexOffset,
      Type type,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultAnyValue resultAnyValue;
      try
      {
        resultAnyValue = await ((IAdsAnyAccess) this._client).ReadAnyAsync(indexGroup, indexOffset, type, args, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAnyValue;
    }

    /// <summary>
    /// Reads data asynchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    public Task<ResultAnyValue> ReadAnyAsync(
      uint indexGroup,
      uint indexOffset,
      Type type,
      CancellationToken cancel)
    {
      return this.ReadAnyAsync(indexGroup, indexOffset, type, (int[]) null, cancel);
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    /// <remarks>
    /// As object types only primitive types are supported.
    /// If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// <list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public async Task<ResultAnyValue> ReadAnyAsync(
      uint variableHandle,
      Type type,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultAnyValue resultAnyValue;
      try
      {
        resultAnyValue = await ((IAdsAnyAccess) this._client).ReadAnyAsync(variableHandle, type, args, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAnyValue;
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <remarks>As object types only primitive types are supported.</remarks>
    /// <param name="variableHandle">The variable/symbol handle.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    public Task<ResultAnyValue> ReadAnyAsync(
      uint variableHandle,
      Type type,
      CancellationToken cancel)
    {
      return this.ReadAnyAsync(variableHandle, type, (int[]) null, cancel);
    }

    /// <summary>write any as an asynchronous operation.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous task operation. The result parameter <see cref="T:TwinCAT.Ads.ResultWrite" /> of the write operation contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" />.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultWrite> WriteAnyAsync(
      uint indexGroup,
      uint indexOffset,
      object value,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultWrite resultWrite;
      try
      {
        resultWrite = await ((IAdsAnyAccess) this._client).WriteAnyAsync(indexGroup, indexOffset, value, args, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>
    /// Writes an object asynchronously to an ADS device.
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultWrite&gt;.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks><list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list></remarks>
    public async Task<ResultWrite> WriteAnyAsync(
      uint variableHandle,
      object value,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultWrite resultWrite;
      try
      {
        resultWrite = await ((IAdsAnyAccess) this._client).WriteAnyAsync(variableHandle, value, args, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>
    /// Writes an object synchronously to an ADS device.
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous task operation. The result parameter <see cref="T:TwinCAT.Ads.ResultWrite" /> of the write operation contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" />.</returns>
    /// <remarks><list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list></remarks>
    public Task<ResultWrite> WriteAnyAsync(
      uint variableHandle,
      object value,
      CancellationToken cancel)
    {
      return this.WriteAnyAsync(variableHandle, value, (int[]) null, cancel);
    }

    /// <summary>
    /// Writes an object asynchronously to an ADS device.
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous task operation. The result parameter <see cref="T:TwinCAT.Ads.ResultWrite" /> of the write operation contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" />.
    /// </returns>
    public Task<ResultWrite> WriteAnyAsync(
      uint indexGroup,
      uint indexOffset,
      object value,
      CancellationToken cancel)
    {
      return this.WriteAnyAsync(indexGroup, indexOffset, value, (int[]) null, cancel);
    }

    /// <summary>
    /// Determines the Symbol handle by its instance path asynchronously.
    /// </summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'CreateVariableHandle' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> parameter contains the variable handle
    /// (<see cref="P:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.TryCreateVariableHandle(System.String,System.UInt32@)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.CreateVariableHandle(System.String)" />
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.AdsConnection.CreateVariableHandleAsync(System.String,System.Threading.CancellationToken)" /> is the <see cref="M:TwinCAT.Ads.AdsConnection.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" /></remarks>
    public async Task<ResultHandle> CreateVariableHandleAsync(
      string variableName,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultHandle variableHandleAsync;
      try
      {
        variableHandleAsync = await ((IAdsHandle) this._client).CreateVariableHandleAsync(variableName, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return variableHandleAsync;
    }

    /// <summary>
    /// Releases the handle of a ADS variable again (asynchronously)
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the
    /// <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.CreateVariableHandleAsync(System.String,System.Threading.CancellationToken)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.TryDeleteVariableHandle(System.UInt32)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteVariableHandle(System.UInt32)" />
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.IAdsHandle.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" /> is the <see cref="M:TwinCAT.Ads.IAdsHandle.CreateVariableHandleAsync(System.String,System.Threading.CancellationToken)" /></remarks>
    public async Task<ResultAds> DeleteVariableHandleAsync(
      uint variableHandle,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultAds resultAds;
      try
      {
        resultAds = await ((IAdsHandle) this._client).DeleteVariableHandleAsync(variableHandle, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAds;
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <param name="args">Additional arguments.</param>
    /// <returns>The read value.</returns>
    /// <remarks>
    /// As object types only primitive types are supported.
    /// If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// <list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public object ReadAny(uint variableHandle, Type type, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAny(variableHandle, type, args);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <returns>The read value.</returns>
    public object ReadAny(uint indexGroup, uint indexOffset, Type type)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAny(indexGroup, indexOffset, type);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <param name="args">Additional arguments.</param>
    /// <returns>The read value.</returns>
    /// <remarks>
    /// As object types only primitive types are supported.
    /// If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// <list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public object ReadAny(uint indexGroup, uint indexOffset, Type type, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAny(indexGroup, indexOffset, type, args);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The Value of the data marshalled to the specified <paramref name="type" />.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// The <paramref name="type" /> is limited to Primitive types ('AnyType').</remarks>
    public object ReadAny(uint indexGroup, uint indexOffset, Type type, int[] args, int timeout)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.ReadAny(indexGroup, indexOffset, type, args);
    }

    /// <summary>Reads as string from a specified address.</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="len">The string length to be read.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public string ReadAnyString(uint indexGroup, uint indexOffset, int len, Encoding encoding)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAnyString(indexGroup, indexOffset, len, encoding);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>read any string as an asynchronous operation.</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="len">The string length to be read.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="F:TwinCAT.Ads.ResultAnyValue.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultAnyValue> ReadAnyStringAsync(
      uint indexGroup,
      uint indexOffset,
      int len,
      Encoding encoding,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultAnyValue resultAnyValue;
      try
      {
        resultAnyValue = await ((IAdsAnyAccess) this._client).ReadAnyStringAsync(indexGroup, indexOffset, len, encoding, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAnyValue;
    }

    /// <summary>Reads a string from the specified symbol/variable.</summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="len">The length.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>The string value.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public string ReadAnyString(uint variableHandle, int len, Encoding encoding)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAnyString(variableHandle, len, encoding);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads a string asynchronously from the specified symbol/variable
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="len">The length.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read string
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultAnyValue> ReadAnyStringAsync(
      uint variableHandle,
      int len,
      Encoding encoding,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultAnyValue resultAnyValue;
      try
      {
        resultAnyValue = await ((IAdsAnyAccess) this._client).ReadAnyStringAsync(variableHandle, len, encoding, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAnyValue;
    }

    /// <summary>
    /// Reads the identification and version number of an ADS server.
    /// </summary>
    /// <returns>DeviceInfo struct containing the name of the device and the version information.</returns>
    public DeviceInfo ReadDeviceInfo()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return this._client.ReadDeviceInfo();
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the identification and version number of an ADS server.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadDeviceState' operation. The <see cref="T:TwinCAT.Ads.ResultDeviceInfo" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultDeviceInfo.DeviceInfo" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultDeviceInfo> ReadDeviceInfoAsync(
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultDeviceInfo resultDeviceInfo;
      try
      {
        resultDeviceInfo = await this._client.ReadDeviceInfoAsync(cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultDeviceInfo;
    }

    /// <summary>
    /// Reads the ADS status and the device status from an ADS server.
    /// </summary>
    /// <returns>The ADS statue and device status.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>Not all ADS Servers support the State ADS Request.</remarks>
    public StateInfo ReadState()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsStateProvider) this._client).ReadState();
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the value of a symbol and returns it as an object. Strings and all primitive datatypes(UInt32, Int32, Bool etc.) are supported.
    /// Arrays and structures cannot be read.
    /// </summary>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <returns>The value of the symbol as an object.</returns>
    public object ReadValue(ISymbol symbol)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).ReadValue(symbol);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the value of a symbol and returns it as an object. Strings and all primitive datatypes (UInt32, Int32, Bool etc.) are supported.
    /// Arrays and structures cannot be read.
    /// </summary>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>The value of the symbol as an object.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultAnyValue> ReadValueAsync(
      ISymbol symbol,
      CancellationToken cancel)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultAnyValue resultAnyValue;
      try
      {
        resultAnyValue = await ((IAdsSymbolicAccess) this._client).ReadValueAsync(symbol, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAnyValue;
    }

    /// <summary>
    /// Reads the value of a symbol and returns the value as object.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="type">Managed type of the ADS symbol.</param>
    /// <param name="value">The read value of the Symbol.</param>
    /// <returns>The <see cref="T:TwinCAT.Ads.AdsErrorCode" />.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>The parameter type must have the same layout as the ADS symbol.</remarks>
    public AdsErrorCode TryReadValue(string name, Type type, out object? value)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryReadValue(name, type, ref value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the value of a symbol and returns the value as object. The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <param name="value">The value.</param>
    /// <returns>Value of the symbol</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'. Structs are not supported.</remarks>
    public AdsErrorCode TryReadValue(ISymbol symbol, out object? value)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryReadValue(symbol, ref value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the value of a symbol and returns the value as object. The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="type">Managed type of the ADS symbol.</param>
    /// <returns>Value of the symbol</returns>
    public object ReadValue(string name, Type type)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      object obj = (object) null;
      this.TryReadValue(name, type, out obj).ThrowOnError();
      return obj;
    }

    /// <summary>Reads the value of a symbol asynchronously.</summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="type">Managed type of the ADS symbol.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>The parameter type must have the same layout as the ADS symbol.</remarks>
    public async Task<ResultAnyValue> ReadValueAsync(
      string name,
      Type type,
      CancellationToken cancel)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultAnyValue resultAnyValue;
      try
      {
        resultAnyValue = await ((IAdsSymbolicAccess) this._client).ReadValueAsync(name, type, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAnyValue;
    }

    /// <summary>
    /// Call this method to obtain information about the individual symbols (variables) in ADS devices.
    /// </summary>
    /// <param name="name">Name of the symbol.</param>
    /// <returns>A <see cref="T:TwinCAT.Ads.TypeSystem.IAdsSymbol" /> containing the requested symbol information or null if symbol could not
    /// be found.</returns>
    public IAdsSymbol ReadSymbol(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).ReadSymbol(name);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Call this method to obtain information about the individual symbols (variables) in ADS devices.
    /// </summary>
    /// <param name="name">Name of the symbol.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous 'ReadSymbolInfo' operation. The <see cref="T:TwinCAT.Ads.ResultValue`1" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    public async Task<ResultValue<IAdsSymbol>> ReadSymbolAsync(
      string name,
      CancellationToken cancel)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultValue<IAdsSymbol> resultValue;
      try
      {
        resultValue = await ((IAdsSymbolicAccess) this._client).ReadSymbolAsync(name, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultValue;
    }

    /// <summary>
    /// Call this method to obtain information about the specified data type.
    /// </summary>
    /// <param name="typeName">Name of the data type (without namespace)</param>
    /// <returns>An <IDataType></IDataType> containing the requested type.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">typeName</exception>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.IAdsSymbolicAccess.TryReadDataType(System.String,TwinCAT.TypeSystem.IDataType@)" />
    /// <seealso cref="M:TwinCAT.Ads.IAdsSymbolicAccess.ReadDataTypeAsync(System.String,System.Threading.CancellationToken)" />
    public IDataType ReadDataType(string typeName)
    {
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentOutOfRangeException(nameof (typeName));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).ReadDataType(typeName);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    public AdsErrorCode TryReadDataType(string typeName, out IDataType? dataType)
    {
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentOutOfRangeException(nameof (typeName));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryReadDataType(typeName, ref dataType);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>read data type as an asynchronous operation.</summary>
    /// <param name="typeName">Name of the data type.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous 'ReadDataType' operation. The <see cref="T:TwinCAT.Ads.ResultValue`1" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">typeName</exception>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.IAdsSymbolicAccess.ReadDataType(System.String)" />
    /// <seealso cref="M:TwinCAT.Ads.IAdsSymbolicAccess.TryReadDataType(System.String,TwinCAT.TypeSystem.IDataType@)" />
    public async Task<ResultValue<IDataType>> ReadDataTypeAsync(
      string typeName,
      CancellationToken cancel)
    {
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentOutOfRangeException(nameof (typeName));
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultValue<IDataType> resultValue;
      try
      {
        resultValue = await ((IAdsSymbolicAccess) this._client).ReadDataTypeAsync(typeName, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultValue;
    }

    /// <summary>
    /// Writes data synchronously to an ADS device and then Reads data from this device into the <paramref name="readBuffer" />
    /// </summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>Number of successfully returned (read) data bytes.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public int ReadWrite(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsReadWrite2) this._client).ReadWrite(indexGroup, indexOffset, readBuffer, writeBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes data synchronously to an ADS device and then Reads data from that target.
    /// </summary>
    /// <param name="variableHandle">Variable handle.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>Number of successfully returned data bytes.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.TryReadWrite(System.UInt32,System.Memory{System.Byte},System.ReadOnlyMemory{System.Byte},System.Int32@)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.ReadWriteAsync(System.UInt32,System.Memory{System.Byte},System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" />
    public int ReadWrite(
      uint variableHandle,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsHandle) this._client).ReadWrite(variableHandle, readBuffer, writeBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// ReadWrites value data asynchronously to/from the symbol represented by the <paramref name="variableHandle" />.
    /// </summary>
    /// <param name="variableHandle">Variable handle.</param>
    /// <param name="readBuffer">The read data / value</param>
    /// <param name="writeBuffer">The write data / value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadWrite' operation. The <see cref="T:TwinCAT.Ads.ResultReadWrite" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="P:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultReadWrite> ReadWriteAsync(
      uint variableHandle,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultReadWrite resultReadWrite = ResultReadWrite.Empty;
      this.BeforeAccess();
      try
      {
        resultReadWrite = await ((IAdsHandle) this._client).ReadWriteAsync(variableHandle, readBuffer, writeBuffer, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultReadWrite;
    }

    /// <summary>
    /// Reads the ADS status and the device status from an ADS server. Unlike the ReadState method this method does not call an exception on failure. Instead an AdsErrorCode is returned.
    /// If the return value is equal to AdsErrorCode.NoError the call was successful.
    /// </summary>
    /// <param name="stateInfo">The ADS statue and device status.</param>
    /// <returns>AdsErrorCode of the ads read state call. Check for AdsErrorCode.NoError to see if call was successful.</returns>
    public AdsErrorCode TryReadState(out StateInfo stateInfo)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsStateProvider) this._client).TryReadState(ref stateInfo);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Read the ADS State asynchronously</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultReadDeviceState" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultReadDeviceState.State" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>Not all ADS Servers support the State ADS Request</remarks>
    public async Task<ResultReadDeviceState> ReadStateAsync(
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultReadDeviceState resultReadDeviceState;
      try
      {
        resultReadDeviceState = await ((IAdsStateProvider) this._client).ReadStateAsync(cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultReadDeviceState;
    }

    /// <summary>
    /// Reads the ADS status and the device status from an ADS server.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The ADS statue and device status.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public StateInfo ReadState(int timeout)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.ReadState();
    }

    /// <summary>
    /// Reads the ADS status and the device status from an ADS server. Unlike the ReadState method this method does not call an exception on failure. Instead an AdsErrorCode is returned.
    /// If the return value is equal to AdsErrorCode.NoError the call was successful.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="stateInfo">The ADS statue and device status.</param>
    /// <returns>AdsErrorCode of the ads read state call. Check for AdsErrorCode.NoError to see if call was successful.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryReadState(int timeout, out StateInfo stateInfo)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.TryReadState(out stateInfo);
    }

    /// <summary>Writes data synchronously to an ADS device.</summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    /// <param name="writeBuffer">The write buffer / value to be written</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.IAdsHandle.TryWrite(System.UInt32,System.ReadOnlyMemory{System.Byte})" />
    /// <seealso cref="M:TwinCAT.Ads.IAdsHandle.WriteAsync(System.UInt32,System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" />
    public void Write(uint variableHandle, ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsHandle) this._client).Write(variableHandle, writeBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Trigger Client Method/Command.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <remarks>This method is used to trigger Client Methods/Commands without parameters.</remarks>
    public void Write(uint indexGroup, uint indexOffset)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        byte[] array = Array.Empty<byte>();
        ((IAdsReadWrite2) this._client).Write(indexGroup, indexOffset, (ReadOnlyMemory<byte>) array.AsMemory<byte>());
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Writes data synchronously to an ADS device.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="writeBuffer">The data to write.</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public void Write(uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsReadWrite2) this._client).Write(indexGroup, indexOffset, writeBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to the given stream.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="readBytes">The number of read bytes.</param>
    /// <returns>The ADS error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryRead(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      out int readBytes)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsReadWrite) this._client).TryRead(indexGroup, indexOffset, readBuffer, ref readBytes);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes data synchronously to an ADS device and reads data from that device.
    /// </summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="readBytes">The read bytes.</param>
    /// <returns>The ADS Error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryReadWrite(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer,
      out int readBytes)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsReadWrite) this._client).TryReadWrite(indexGroup, indexOffset, readBuffer, writeBuffer, ref readBytes);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Writes data synchronously to an ADS device.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="writeBuffer">The data buffer to be written.</param>
    /// <returns>The ADS error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryWrite(
      uint indexGroup,
      uint indexOffset,
      ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsReadWrite) this._client).TryWrite(indexGroup, indexOffset, writeBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to the given stream.
    /// </summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>Number of successfully returned data bytes.</returns>
    public int Read(uint indexGroup, uint indexOffset, Memory<byte> readBuffer, int timeout)
    {
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.Read(indexGroup, indexOffset, readBuffer);
    }

    /// <summary>
    /// Writes data synchronously to an ADS device and then Reads data from this device.
    /// </summary>
    /// <param name="indexGroup">Thhe index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>Number of successfully returned data bytes.</returns>
    public int ReadWrite(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer,
      int timeout)
    {
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.ReadWrite(indexGroup, indexOffset, readBuffer, writeBuffer);
    }

    /// <summary>Trigger Client Method/Command.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="timeout">The timeout.</param>
    /// <remarks>This method is used to trigger Client Methods/Commands without parameters.</remarks>
    public void Write(uint indexGroup, uint indexOffset, int timeout)
    {
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        this.Write(indexGroup, indexOffset);
    }

    /// <summary>Writes data synchronously to an ADS device.</summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="timeout">The timeout.</param>
    public void Write(
      uint indexGroup,
      uint indexOffset,
      ReadOnlyMemory<byte> writeBuffer,
      int timeout)
    {
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        this.Write(indexGroup, indexOffset, writeBuffer);
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to the given stream.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="readBytes">The read bytes.</param>
    /// <returns>The ADS Error code.</returns>
    public AdsErrorCode TryRead(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      int timeout,
      out int readBytes)
    {
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.TryRead(indexGroup, indexOffset, readBuffer, out readBytes);
    }

    /// <summary>
    /// Writes data synchronously to an ADS device and then Reads data from this device.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">The read stream.</param>
    /// <param name="writeBuffer">The write stream.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="readBytes">The read bytes.</param>
    /// <returns>The ADS Error code.</returns>
    public AdsErrorCode TryReadWrite(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer,
      int timeout,
      out int readBytes)
    {
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.TryReadWrite(indexGroup, indexOffset, readBuffer, writeBuffer, out readBytes);
    }

    /// <summary>Writes data synchronously to an ADS device.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The ADS Error code.</returns>
    public AdsErrorCode TryWrite(
      uint indexGroup,
      uint indexOffset,
      ReadOnlyMemory<byte> writeBuffer,
      int timeout)
    {
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.TryWrite(indexGroup, indexOffset, writeBuffer);
    }

    /// <summary>Writes an object synchronously to an ADS device.</summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    public void WriteAny(uint variableHandle, object value)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsAnyAccess) this._client).WriteAny(variableHandle, value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes an object synchronously to an ADS device.
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="args">Additional arguments.</param>
    public void WriteAny(uint variableHandle, object value, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsAnyAccess) this._client).WriteAny(variableHandle, value, args);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Writes an object synchronously to an ADS device.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    public void WriteAny(uint indexGroup, uint indexOffset, object value)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsAnyAccess) this._client).WriteAny(indexGroup, indexOffset, value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes an object synchronously to an ADS device.
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="args">Additional arguments.</param>
    public void WriteAny(uint indexGroup, uint indexOffset, object value, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsAnyAccess) this._client).WriteAny(indexGroup, indexOffset, value, args);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes an object synchronously to an ADS device.
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public void WriteAny(
      uint indexGroup,
      uint indexOffset,
      object value,
      int[] args,
      int timeout)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        this.WriteAny(indexGroup, indexOffset, value, args);
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    public void WriteControl(StateInfo stateInfo)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsStateControl) this._client).WriteControl(stateInfo);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public void WriteControl(StateInfo stateInfo, int timeout)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        this.WriteControl(stateInfo);
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public void WriteControl(StateInfo stateInfo, ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsStateControl) this._client).WriteControl(stateInfo, writeBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public void WriteControl(StateInfo stateInfo, ReadOnlyMemory<byte> writeBuffer, int timeout)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        this.WriteControl(stateInfo, writeBuffer);
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryWriteControl(
      StateInfo stateInfo,
      ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsStateControl) this._client).TryWriteControl(stateInfo, writeBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public AdsErrorCode TryWriteControl(StateInfo stateInfo)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsStateControl) this._client).TryWriteControl(stateInfo);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Changes the ADS status and device status of the ADS server asynchronously.
    /// </summary>
    /// <param name="state">The ADS state.</param>
    /// <param name="deviceState">The device state.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'WriteControl' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the state
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultAds> WriteControlAsync(
      AdsState state,
      ushort deviceState,
      CancellationToken cancel)
    {
      return this.WriteControlAsync(state, deviceState, (ReadOnlyMemory<byte>) Memory<byte>.Empty, cancel);
    }

    /// <summary>
    /// Writes the <see cref="T:TwinCAT.Ads.AdsState" /> and device state to the ADS device.
    /// </summary>
    /// <param name="adsState">State of the ads.</param>
    /// <param name="deviceState">State of the device.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the
    /// <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public async Task<ResultAds> WriteControlAsync(
      AdsState adsState,
      ushort deviceState,
      ReadOnlyMemory<byte> writeBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultAds resultAds;
      try
      {
        resultAds = await ((IAdsStateControl) this._client).WriteControlAsync(adsState, deviceState, writeBuffer, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAds;
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryWriteControl(
      StateInfo stateInfo,
      ReadOnlyMemory<byte> writeBuffer,
      int timeout)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.TryWriteControl(stateInfo, writeBuffer);
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public AdsErrorCode TryWriteControl(StateInfo stateInfo, int timeout)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      using (new AdsTimeoutSetter((IConnection) this, timeout))
        return this.TryWriteControl(stateInfo);
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.  Array and structures are not supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    public void WriteValue(ISymbol symbol, object val)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsSymbolicAccess) this._client).WriteValue(symbol, val);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.  Array and structures are not supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>Task.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultWrite> WriteValueAsync(
      ISymbol symbol,
      object val,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultWrite resultWrite;
      try
      {
        resultWrite = await ((IAdsSymbolicAccess) this._client).WriteValueAsync(symbol, val, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public void WriteValue(string name, object value)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.TryWriteValue(name, value).ThrowOnError();
    }

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryWriteValue(string name, object value)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryWriteValue(name, value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous 'WriteSymbol' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultWrite> WriteSymbolAsync(
      string name,
      object value,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultWrite resultWrite;
      try
      {
        resultWrite = await ((IAdsSymbolicAccess) this._client).WriteValueAsync<object>(name, value, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Ads.AdsConnection" /> is disposed.
    /// </summary>
    /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
    public bool Disposed => this._disposed;

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
      {
        if (this._client != null && this._client is IDisposable client)
          client.Dispose();
        this._lostTime = new DateTimeOffset?(DateTimeOffset.Now);
      }
      this._disposed = true;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Disconnects this <see cref="T:TwinCAT.IConnection" />.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public bool Disconnect()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (this._client == null)
        return false;
      bool flag = ((IConnection) this._client).Disconnect();
      this._lostTime = new DateTimeOffset?(DateTimeOffset.Now);
      return flag;
    }

    /// <summary>
    /// Closes the <see cref="T:TwinCAT.Ads.AdsConnection" />
    /// </summary>
    public void Close() => this.Dispose();

    /// <summary>
    /// Reads the value synchronously data of the symbol, that is represented by the variable handle into the <paramref name="readBuffer" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="readBuffer">The read buffer/data</param>
    /// <param name="readBytes">Number of read bytes.</param>
    /// <returns>The ADS error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryRead(
      uint variableHandle,
      Memory<byte> readBuffer,
      out int readBytes)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsHandle) this._client).TryRead(variableHandle, readBuffer, ref readBytes);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// ReadWrites value data synchronously to/from the symbol represented by the <paramref name="variableHandle" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="readBuffer">The read buffer / read data.</param>
    /// <param name="writeBuffer">The write buffer / write data.</param>
    /// <param name="readBytes">Number of read bytes.</param>
    /// <returns>The ADS error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryReadWrite(
      uint variableHandle,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer,
      out int readBytes)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsHandle) this._client).TryReadWrite(variableHandle, readBuffer, writeBuffer, ref readBytes);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes the value data synchronously that is represented in the <paramref name="writeBuffer" /> to the symbol with the specified <paramref name="variableHandle" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="writeBuffer">The write buffer / value.</param>
    /// <returns>The ADS error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public AdsErrorCode TryWrite(uint variableHandle, ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsHandle) this._client).TryWrite(variableHandle, writeBuffer);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.IAdsSymbolLoader">Symbol loader</see> with the specified mode.
    /// </summary>
    /// <param name="session">The session (for session orientated loads/symbols). Can be NULL if not present.</param>
    /// <param name="settings">The settings.</param>
    /// <returns>The <see cref="T:TwinCAT.Ads.TypeSystem.IAdsSymbolLoader" /> interface of the Symbol loader.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <example>
    /// The following sample shows how to create a dynamic version of the SymbolLoader V2. The dynamic symbol loader makes use of the Dynamic Language Runtime (DLR) of the .NET Framework.
    /// That means Structures, Arrays and Enumeration types and instances are generated 'on-the-fly' during symbol Browsing. These created dynamic objects are a one to one representation
    /// of the Symbol Server target objects (e.g the IEC61131 types on the PLC).
    /// Dynamic language features are only available from .NET4 upwards.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE" />
    /// The following sample shows how to create a static (non dynamic) version of the SymbolLoader V2.
    /// The static symbol loader in version 2 is a nearly code compatible version of the Dynamic Loader, only the dynamic creation of objects is not available. The reason for supporting
    /// this mode is that .NET Framework Versions lower than Version 4.0 (CLR2) doesn't support the Dynamic Language Runtime (DLR).
    /// The SymbolLoader V2 static object is supported from .NET 2.0 on.
    /// <code language="C#" title="Virtual Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE" /></example>
    /// <example>
    /// The SymbolLoader V2 static object is supported from .NET 2.0 on.
    /// <code language="C#" title="Flat Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2Flat.cs" region="CODE_SAMPLE" /></example>
    /// <example>
    ///   <code language="C#" title="Argument Parser" source="..\..\Samples\Sample.Ads.AdsClientCore\ArgParser.cs" region="CODE_SAMPLE" />
    ///   <code language="C#" title="Dumping Symbols" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolDump.cs" region="CODE_SAMPLE" />
    /// </example>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) with Virtual Symbols
    /// <code language="C#" title="RPC Call in Virtual Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) with Dynamic Symbols.
    /// <code language="C#" title="RPC Call in Dynamic Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2DynamicTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    /// <seealso cref="T:TwinCAT.Ads.TypeSystem.SymbolLoaderFactory" />
    /// <remarks>The Symbol Loader (V2) supports the following <see cref="T:TwinCAT.SymbolsLoadMode">modes</see>.
    /// <list type="Table">
    /// <listheader>
    /// <term></term><description></description>
    /// </listheader>
    /// <item>
    /// <term><see cref="F:TwinCAT.SymbolsLoadMode.Flat" /></term>
    /// <description>The flat mode organizes the Symbols in a flat list. At the beginning this List caches only the root symbol objects, which can be enumerated.
    /// To access the sub elements like structure fields or array elements use the <see cref="P:TwinCAT.TypeSystem.ISymbol.SubSymbols" /> collection. The property get
    /// accessor generates the subsymbols lazy on the fly (performance optimized) and stores them internally as weak reference (memory optimized).
    /// This mode is available in all .NET versions.</description>
    /// </item>
    /// <item>
    /// <term><see cref="F:TwinCAT.SymbolsLoadMode.VirtualTree" /></term>
    /// <description>On top of the behaviour of the <see cref="F:TwinCAT.SymbolsLoadMode.Flat" />, the virtual tree mode organizes the Symbols hierarchically with parent-child relationships.
    /// That eases the access to the hierarchical structure but needs slightly more preprocessing of the data.
    /// This mode is available in all .NET Versions.
    /// </description>
    /// </item>
    /// <item>
    /// <term><see cref="F:TwinCAT.SymbolsLoadMode.DynamicTree" /></term>
    /// <description>The Dynamic tree mode organizes the Symbols hierarchically and (dynamically) creates struct members,
    /// array elements and enum fields on the fly. 'Dynamically' means here not only lazy creation like in <see cref="F:TwinCAT.SymbolsLoadMode.Flat" />, but furthermore
    /// real creation of type safe .NET complex types/instances as represetantives of the TwinCAT Symbol objects/types. This feature is only available on platforms that support the Dynamic
    /// Language Runtime (DLR); actually all .NET Framework Version larger than 4.0.
    /// </description>
    /// </item>
    /// </list>
    /// Virtual instances means, that all Symbols are ordered within a tree structure. For that symbol nodes that are not located on a fixed address, a Virtual Symbol will be created.
    /// Setting the virtualInstance parameter to 'false' means, that the located symbols will be returned in a flattened list.</remarks>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use TwinCAT.Ads.TypeSystem.SymbolLoaderFactory.Create instead for creating a SymbolLoader object!")]
    public IAdsSymbolLoader? CreateSymbolLoader(
      ISession session,
      ISymbolLoaderSettings settings)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return (IAdsSymbolLoader) SymbolLoaderFactory.Create((IConnection) this, settings);
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length.</param>
    /// <param name="encoding">The encoding.</param>
    /// <exclude />
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!</remarks>
    [Obsolete("This method is potentially unsafe!")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void WriteAnyString(
      uint indexGroup,
      uint indexOffset,
      string value,
      int length,
      Encoding encoding)
    {
      this.BeforeAccess();
      try
      {
        ((IAdsAnyAccess) this._client).WriteAnyString(indexGroup, indexOffset, value, length, encoding);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>write any string as an asynchronous operation.</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultWrite&gt;.</returns>
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!</remarks>
    [Obsolete("This method is potentially unsafe!")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<ResultWrite> WriteAnyStringAsync(
      uint indexGroup,
      uint indexOffset,
      string value,
      int length,
      Encoding encoding,
      CancellationToken cancel)
    {
      this.BeforeAccess();
      ResultWrite resultWrite;
      try
      {
        resultWrite = await ((IAdsAnyAccess) this._client).WriteAnyStringAsync(indexGroup, indexOffset, value, length, encoding, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length of the string to write</param>
    /// <param name="encoding">The encoding.</param>
    /// <exclude />
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!
    /// The String is written with the specified encoding.</remarks>
    public void WriteAnyString(uint variableHandle, string value, int length, Encoding encoding)
    {
      this.BeforeAccess();
      try
      {
        ((IAdsAnyAccess) this._client).WriteAnyString(variableHandle, value, length, encoding);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length of the string to write</param>
    /// <param name="encoding">The encoding.</param>
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!
    /// The String is written with the specified encoding.</remarks>
    public void WriteAnyString(string symbolPath, string value, int length, Encoding encoding)
    {
      this.BeforeAccess();
      try
      {
        ((IAdsAnyAccess) this._client).WriteAnyString(symbolPath, value, length, encoding);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>write any string as an asynchronous operation.</summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length of the string to write</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultWrite&gt;.</returns>
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!
    /// The String is written with the specified encoding.</remarks>
    public async Task<ResultWrite> WriteAnyStringAsync(
      uint variableHandle,
      string value,
      int length,
      Encoding encoding,
      CancellationToken cancel)
    {
      this.BeforeAccess();
      ResultWrite resultWrite;
      try
      {
        resultWrite = await ((IAdsAnyAccess) this._client).WriteAnyStringAsync(variableHandle, value, length, encoding, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>write any string as an asynchronous operation.</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length of the string to write</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultWrite&gt;.</returns>
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!
    /// The String is written with the specified encoding.</remarks>
    public async Task<ResultWrite> WriteAnyStringAsync(
      string symbolPath,
      string value,
      int length,
      Encoding encoding,
      CancellationToken cancel)
    {
      this.BeforeAccess();
      ResultWrite resultWrite;
      try
      {
        resultWrite = await ((IAdsAnyAccess) this._client).WriteAnyStringAsync(symbolPath, value, length, encoding, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>Read (determine) the Symbol handle by its name/path</summary>
    /// <param name="symbolName">SymbolName / Path.</param>
    /// <param name="variableHandle">The handle.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryCreateVariableHandle(
      string symbolName,
      out uint variableHandle)
    {
      this.BeforeAccess();
      try
      {
        return ((IAdsHandle) this._client).TryCreateVariableHandle(symbolName, ref variableHandle);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the data asynchronously from specified IndexGroup/IndexOffset
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readBuffer">The read buffer, memory area where the data is written.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultRead" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="P:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution..
    /// </returns>
    public async Task<ResultRead> ReadAsync(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultRead resultRead = ResultRead.Empty;
      this.BeforeAccess();
      try
      {
        resultRead = await ((IAdsReadWrite) this._client).ReadAsync(indexGroup, indexOffset, readBuffer, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultRead;
    }

    /// <summary>
    /// Read/Writes data asynchronously to/from the specified <paramref name="writeBuffer" />, <paramref name="readBuffer" />
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadWrite' operation. The <see cref="T:TwinCAT.Ads.ResultReadWrite" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="P:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    public async Task<ResultReadWrite> ReadWriteAsync(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultReadWrite resultReadWrite = ResultReadWrite.Empty;
      this.BeforeAccess();
      try
      {
        resultReadWrite = await ((IAdsReadWrite) this._client).ReadWriteAsync(indexGroup, indexOffset, readBuffer, writeBuffer, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultReadWrite;
    }

    /// <summary>
    /// Triggers a write call at the specified IndexGroup/IndexOffset asynchronously.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadWrite' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public async Task<ResultWrite> WriteAsync(
      uint indexGroup,
      uint indexOffset,
      CancellationToken cancel)
    {
      byte[] array = Array.Empty<byte>();
      return await this.WriteAsync(indexGroup, indexOffset, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes the data / Value asynchronously into the specified <paramref name="writeBuffer" />.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'Write' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public async Task<ResultWrite> WriteAsync(
      uint indexGroup,
      uint indexOffset,
      ReadOnlyMemory<byte> writeBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultWrite resultWrite = ResultWrite.Empty;
      this.BeforeAccess();
      try
      {
        resultWrite = await ((IAdsReadWrite) this._client).WriteAsync(indexGroup, indexOffset, writeBuffer, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>
    /// Reads the value data of the symbol asynchronously into the <paramref name="readBuffer" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="readBuffer">The buffer.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultRead" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="P:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution..</returns>
    public async Task<ResultRead> ReadAsync(
      uint variableHandle,
      Memory<byte> readBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultRead resultRead = ResultRead.Empty;
      this.BeforeAccess();
      try
      {
        resultRead = await ((IAdsHandle) this._client).ReadAsync(variableHandle, readBuffer, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultRead;
    }

    /// <summary>
    /// Writes the value data asynchronously that is represented by the <paramref name="writeBuffer" /> to the symbol specified by the
    /// <paramref name="variableHandle" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="writeBuffer">The write buffer/value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    public async Task<ResultWrite> WriteAsync(
      uint variableHandle,
      ReadOnlyMemory<byte> writeBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultWrite resultWrite = ResultWrite.Empty;
      this.BeforeAccess();
      try
      {
        resultWrite = await ((IAdsHandle) this._client).WriteAsync(variableHandle, writeBuffer, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>Gets the symbol table.</summary>
    /// <returns>SymbolTable.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    IHandleTable? IAdsHandleTableProvider.GetHandleTable() => this._client != null ? ((IAdsHandleTableProvider) this._client).GetHandleTable() : (IHandleTable) null;

    /// <summary>Creates a handle bag from symbol paths.</summary>
    /// <param name="instancePath">A list of symbol paths.</param>
    /// <returns>A handle bag that can be disposed.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    IDisposableHandleBag<string> IAdsHandleTableProvider.CreateHandleBag(
      string[] instancePath)
    {
      return ((IAdsHandleTableProvider) this._client).CreateHandleBag(instancePath);
    }

    /// <summary>
    /// Creates a notification handle bag form the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">The user data.</param>
    /// <returns>A handle bag that can be disposed.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    IDisposableHandleBag<ISymbol> IAdsHandleTableProvider.CreateNotificationHandleBag(
      ISymbol[] symbols,
      NotificationSettings settings,
      object[]? userData)
    {
      return ((IAdsHandleTableProvider) this._client).CreateNotificationHandleBag(symbols, settings, userData);
    }

    /// <summary>Creates the notification ex handle bag.</summary>
    /// <param name="symbols">The symbols</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">The user data.</param>
    /// <returns>IDisposableHandleBag.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    IDisposableHandleBag<AnySymbolSpecifier> IAdsHandleTableProvider.CreateNotificationExHandleBag(
      IList<AnySymbolSpecifier> symbols,
      NotificationSettings settings,
      object[]? userData)
    {
      return ((IAdsHandleTableProvider) this._client).CreateNotificationExHandleBag(symbols, settings, userData);
    }

    /// <summary>
    /// Unregisters the handle bag from this <see cref="T:TwinCAT.Ads.IAdsHandleTableProvider" />.
    /// </summary>
    /// <param name="bag">The handle bag.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IAdsHandleTableProvider.UnregisterHandleBag(IDisposableHandleBag bag) => ((IAdsHandleTableProvider) this._client).UnregisterHandleBag(bag);

    /// <summary>
    /// Connects a variable to the ADS client asynchronously. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> event.
    /// </summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="cancel">The Cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'AddDeviceNotification' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> type parameter contains the created handle
    /// (<see cref="F:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" /> should always
    /// be called when the notification is not used anymore.</remarks>
    public async Task<ResultHandle> AddDeviceNotificationAsync(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultHandle resultHandle = ResultHandle.Empty;
      this.BeforeAccess();
      try
      {
        resultHandle = await ((IAdsNotifications) this._client).AddDeviceNotificationAsync(indexGroup, indexOffset, dataSize, settings, userData, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultHandle;
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> event.
    /// </summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="handle">The notification handle.</param>
    /// <returns>The ADS error code.</returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.TryDeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications..AddDeviceNotificationAsync" />
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.IAdsNotifications.TryDeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    public AdsErrorCode TryAddDeviceNotification(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      out uint handle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).TryAddDeviceNotification(indexGroup, indexOffset, dataSize, settings, userData, ref handle);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <param name="args">The 'AnyType' arguments.</param>
    /// <param name="handle">The notification handle.</param>
    /// <returns>The ADS Error code.</returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <remarks>If type is a string type, the first element of the parameter args specifies the number of characters of the string.
    /// If type is an array type, the number of elements for each dimension has to be specified in the parameter args.
    /// Only primitive types (AnyType) are supported by this method.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    public AdsErrorCode TryAddDeviceNotificationEx(
      uint indexGroup,
      uint indexOffset,
      NotificationSettings settings,
      object? userData,
      Type type,
      int[]? args,
      out uint handle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).TryAddDeviceNotificationEx(indexGroup, indexOffset, settings, userData, type, args, ref handle);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Connects a variable to the ADS client asynchronously. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="type">Type of the object stored in the event argument, only Primitive 'AnyTypes' allowed.</param>
    /// <param name="args">Additional arguments (for 'AnyType')</param>
    /// <param name="cancel">The Cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'AddDeviceNotification' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> type parameter contains the created handle
    /// (<see cref="F:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <remarks>If type is a string type, the first element of the parameter args specifies the number of characters of the string.
    /// If type is an array type, the number of elements for each dimension has to be specified in the parameter args.
    /// Only primitive types (AnyType) are supported by this method.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" /> should always
    /// called when the notification is not used anymore.</remarks>
    public async Task<ResultHandle> AddDeviceNotificationExAsync(
      uint indexGroup,
      uint indexOffset,
      NotificationSettings settings,
      object? userData,
      Type type,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultHandle resultHandle = ResultHandle.Empty;
      this.BeforeAccess();
      try
      {
        resultHandle = await ((IAdsNotifications) this._client).AddDeviceNotificationExAsync(indexGroup, indexOffset, settings, userData, type, args, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultHandle;
    }

    /// <summary>
    /// Connects a variable to the ADS client asynchronously. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="symbolPath">The symbol/instance path of the ADS variable.</param>
    /// <param name="settings">The notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <param name="args">Additional arguments (for 'AnyType')</param>
    /// <param name="cancel">The Cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'AddDeviceNotification' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> type parameter contains the created handle
    /// (<see cref="P:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" /> should always
    /// be called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotificationEx" />
    /// <seealso cref="M:TwinCAT.Ads.AdsConnection.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    public async Task<ResultHandle> AddDeviceNotificationExAsync(
      string symbolPath,
      NotificationSettings settings,
      object? userData,
      Type type,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultHandle resultHandle = ResultHandle.Empty;
      this.BeforeAccess();
      try
      {
        resultHandle = await ((IAdsNotifications) this._client).AddDeviceNotificationExAsync(symbolPath, settings, userData, type, args, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultHandle;
    }

    /// <summary>Deletes a registered notification.</summary>
    /// <param name="notificationHandle">Notification handle.</param>
    /// <returns>The ADS error code.</returns>
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <remarks>This is the complementary method to <see cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" /> overloads and should be called when the
    /// notification is not needed anymore the free TwinCAT realtime resources.</remarks>
    public AdsErrorCode TryDeleteDeviceNotification(uint notificationHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsNotifications) this._client).TryDeleteDeviceNotification(notificationHandle);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Deletes a registered notification asynchronously.</summary>
    /// <param name="notificationHandle">Notification handle.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'DeleteDeviceNotification' operation. The <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property contains the
    /// ADS error code after execution.</returns>
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <remarks>This is the complementary method to <see cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" /> overloads and should be called when the
    /// notification is not needed anymore the free TwinCAT realtime resources.</remarks>
    public async Task<ResultAds> DeleteDeviceNotificationAsync(
      uint notificationHandle,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultAds resultAds = ResultAds.Empty;
      this.BeforeAccess();
      try
      {
        resultAds = await ((IAdsNotifications) this._client).DeleteDeviceNotificationAsync(notificationHandle, CancellationToken.None).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultAds;
    }

    /// <summary>
    /// Connects a variable to the ADS client asynchronously. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" /> event.
    /// </summary>
    /// <param name="symbolPath">The symbol/instance path of the ADS variable.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="cancel">The Cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'AddDeviceNotification' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> type parameter contains the created handle
    /// (<see cref="F:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" />
    /// <seealso cref="M:TwinCAT.Ads.IAdsNotifications.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsConnection.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.IAdsNotifications.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" /> should always
    /// be called when the notification is not used anymore.</remarks>
    public async Task<ResultHandle> AddDeviceNotificationAsync(
      string symbolPath,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultHandle empty = ResultHandle.Empty;
      this.BeforeAccess();
      ResultHandle resultHandle;
      try
      {
        resultHandle = await ((IAdsNotifications) this._client).AddDeviceNotificationAsync(symbolPath, dataSize, settings, userData, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultHandle;
    }

    public AdsErrorCode TryReadSymbol(string symbolPath, out IAdsSymbol? symbol)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryReadSymbol(symbolPath, ref symbol);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.  Array and structures are not supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteValue(ISymbol symbol, object val)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryWriteValue(symbol, val);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T">The type of the value to read.</typeparam>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <returns>The value of the read symbol.</returns>
    public T ReadAny<T>(uint variableHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAny<T>(variableHandle);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Reads data synchronously from an ADS device.</summary>
    /// <remarks>As object types only primitive types are supported.</remarks>
    /// <typeparam name="T">The Type of the value to be read.</typeparam>
    /// <param name="variableHandle">The variable/symbol handle.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    public async Task<ResultValue<T>> ReadAnyAsync<T>(
      uint variableHandle,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<T> resultValue = ResultValue<T>.Empty;
      this.BeforeAccess();
      try
      {
        resultValue = await ((IAdsAnyAccess) this._client).ReadAnyAsync<T>(variableHandle, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultValue;
    }

    /// <summary>
    /// Reads data asynchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T">Type of the object to be read</typeparam>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultValue`1" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    /// <remarks>
    /// As object types only primitive types are supported.
    /// If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// <list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public async Task<ResultValue<T>> ReadAnyAsync<T>(
      uint variableHandle,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<T> resultValue = ResultValue<T>.Empty;
      this.BeforeAccess();
      try
      {
        resultValue = await ((IAdsAnyAccess) this._client).ReadAnyAsync<T>(variableHandle, args, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultValue;
    }

    /// <summary>
    /// Reads data asynchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>The asynchronous result.</returns>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultValue`1" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    public async Task<ResultValue<T>> ReadAnyAsync<T>(
      uint indexGroup,
      uint indexOffset,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<T> resultValue = ResultValue<T>.Empty;
      this.BeforeAccess();
      try
      {
        resultValue = await ((IAdsAnyAccess) this._client).ReadAnyAsync<T>(indexGroup, indexOffset, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultValue;
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object to be read.</typeparam>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="args">Additional arguments.</param>
    /// <returns>The read value.</returns>
    /// <remarks>
    /// As object types only primitive types are supported.
    /// If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// <list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public T ReadAny<T>(uint indexGroup, uint indexOffset, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAny<T>(indexGroup, indexOffset, args);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data asynchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="args">Additional arguments.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultValue`1" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    /// <remarks>
    /// As object types only primitive types are supported.
    /// If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// <list type="table">
    ///   <listheader>
    ///     <term>Type of value Parameter</term>
    ///     <description>Necessary Arguments (args)</description>
    ///   </listheader>
    ///   <item>
    ///     <term>string</term>
    ///     <description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description>
    ///   </item>
    ///   <item>
    ///     <term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    ///   <item>
    ///     <term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public async Task<ResultValue<T>> ReadAnyAsync<T>(
      uint indexGroup,
      uint indexOffset,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<T> resultValue = ResultValue<T>.Empty;
      this.BeforeAccess();
      try
      {
        resultValue = await ((IAdsAnyAccess) this._client).ReadAnyAsync<T>(indexGroup, indexOffset, args, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultValue;
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T">The type of the value to read.</typeparam>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="args">Additional arguments.</param>
    /// <returns>The value of the read symbol.</returns>
    /// <remarks>As object types only primitive types are supported.
    /// If the Type of the object to be read is a string type, the first element of
    /// the parameter args specifies the number of characters of the string.
    /// If the Type of the object to be read is an array type, the number of elements
    /// for each dimension has to be specified in the parameter args.
    /// <list type="table"><listheader><term>Type of value Parameter</term><description>Necessary Arguments (args)</description></listheader><item><term>string</term><description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description></item><item><term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item><item><term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list></remarks>
    public T ReadAny<T>(uint variableHandle, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAny<T>(variableHandle, args);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object to be read.</typeparam>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <returns>The read value.</returns>
    public T ReadAny<T>(uint indexGroup, uint indexOffset)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsAnyAccess) this._client).ReadAny<T>(indexGroup, indexOffset);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Clears the internal symbol cache.</summary>
    /// <remarks>Previously stored symbol information is cleared. As a consequence the symbol information must be obtained from the ADS server again if accessed, which
    /// which needs an extra ADS round trip.</remarks>
    public void CleanupSymbolTable()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      ((IAdsSymbolicAccess) this._client).CleanupSymbolTable();
    }

    /// <summary>
    /// Reads the value of a symbol and returns it as an object.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <returns>The value of the symbol.</returns>
    /// <remarks>Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'.</remarks>
    public T ReadValue<T>(ISymbol symbol) where T : notnull
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).ReadValue<T>(symbol);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the value of a symbol and returns it as an object.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <param name="value">The value.</param>
    /// <returns>The ADS Error Code</returns>
    /// <remarks>Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'.</remarks>
    public AdsErrorCode TryReadValue<T>(ISymbol symbol, out T value)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryReadValue<T>(symbol, ref value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the value of a symbol asynchronously and returns it as an object.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <remarks>Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'.</remarks>
    public async Task<ResultValue<T>> ReadValueAsync<T>(
      ISymbol symbol,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<T> resultValue = ResultValue<T>.Empty;
      this.BeforeAccess();
      try
      {
        resultValue = await ((IAdsSymbolicAccess) this._client).ReadValueAsync<T>(symbol, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultValue;
    }

    /// <summary>
    /// Reads the value of a symbol and returns the value. The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <returns>Value of the symbol</returns>
    public T ReadValue<T>(string name) where T : notnull
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).ReadValue<T>(name);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Reads the value of a symbol and returns the value as object.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">The read value of the Symbol.</param>
    /// <returns>The <see cref="T:TwinCAT.Ads.AdsErrorCode" />.</returns>
    /// <remarks>The parameter type must have the same layout as the ADS symbol.</remarks>
    public AdsErrorCode TryReadValue<T>(string name, out T value)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryReadValue<T>(name, ref value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Reads the value of a symbol asynchronously.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <remarks>The parameter type must have the same layout as the ADS symbol.</remarks>
    public async Task<ResultValue<T>> ReadValueAsync<T>(
      string name,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<T> resultValue = ResultValue<T>.Empty;
      this.BeforeAccess();
      try
      {
        resultValue = await ((IAdsSymbolicAccess) this._client).ReadValueAsync<T>(name, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultValue;
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    public void WriteValue<T>(ISymbol symbol, T val) where T : notnull
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsSymbolicAccess) this._client).WriteValue<T>(symbol, val);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteValue<T>(ISymbol symbol, T val) where T : notnull
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryWriteValue<T>(symbol, val);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'WriteSymbol' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public async Task<ResultWrite> WriteValueAsync<T>(
      ISymbol symbol,
      T val,
      CancellationToken cancel)
      where T : notnull
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultWrite resultWrite = ResultWrite.Empty;
      this.BeforeAccess();
      try
      {
        resultWrite = await ((IAdsSymbolicAccess) this._client).WriteValueAsync<T>(symbol, val, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <typeparam name="T">the value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    public void WriteValue<T>(string name, T value) where T : notnull
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        ((IAdsSymbolicAccess) this._client).WriteValue<T>(name, value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteValue<T>(string name, T value) where T : notnull
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsSymbolicAccess) this._client).TryWriteValue<T>(name, value);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous 'WriteSymbol' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public async Task<ResultWrite> WriteValueAsync<T>(
      string name,
      T value,
      CancellationToken cancel)
      where T : notnull
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultWrite resultWrite = ResultWrite.Empty;
      this.BeforeAccess();
      try
      {
        resultWrite = await ((IAdsSymbolicAccess) this._client).WriteValueAsync<T>(name, value, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultWrite;
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The input parameters or NULL</param>
    /// <returns>The return value of the Method (as object).</returns>
    /// <remarks>This method only supports primitive data types as <paramref name="inParameters" />. Any available outparameters will be ignored.
    /// Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public object? InvokeRpcMethod(string symbolPath, string methodName, object[]? inParameters)
    {
      object[] outParameters = (object[]) null;
      return this.InvokeRpcMethod(symbolPath, methodName, inParameters, out outParameters);
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The input parameters or NULL</param>
    /// <param name="outParameters">The output parameters.</param>
    /// <returns>The return value of the Method (as object).</returns>
    /// <remarks>
    /// Because this overload doesn't provide any <see cref="T:TwinCAT.TypeSystem.AnyTypeSpecifier" /> specifications, only primitive datatypes will be correctly marshalled
    /// by this method. Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public object? InvokeRpcMethod(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      out object[]? outParameters)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        object obj = (object) null;
        ((IAdsRpcInvoke) this._client).TryInvokeRpcMethod(symbolPath, methodName, inParameters, ref outParameters, ref obj).ThrowOnError();
        return obj;
      }
      finally
      {
        this.AfterAccess();
      }
    }

    public object? InvokeRpcMethod(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier? retSpecifier)
    {
      object[] outParameters = (object[]) null;
      return this.InvokeRpcMethod(symbolPath, methodName, inParameters, (AnyTypeSpecifier[]) null, retSpecifier, out outParameters);
    }

    public object? InvokeRpcMethod(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        object obj = (object) null;
        ((IAdsRpcInvoke) this._client).TryInvokeRpcMethod(symbolPath, methodName, inParameters, outSpecifiers, retSpecifier, ref outParameters, ref obj).ThrowOnError();
        return obj;
      }
      finally
      {
        this.AfterAccess();
      }
    }

    public AdsErrorCode TryInvokeRpcMethod(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters,
      out object? retValue)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsRpcInvoke) this._client).TryInvokeRpcMethod(symbolPath, methodName, inParameters, outSpecifiers, retSpecifier, ref outParameters, ref retValue);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    public Task<ResultRpcMethod> InvokeRpcMethodAsync(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsRpcInvoke) this._client).InvokeRpcMethodAsync(symbolPath, methodName, inParameters, outSpecifiers, retSpecifier, cancel);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    public AdsErrorCode TryInvokeRpcMethod(
      IRpcCallableInstance symbol,
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters,
      out object? retValue)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsRpcInvoke) this._client).TryInvokeRpcMethod(symbol, method, inParameters, outSpecifiers, retSpecifier, ref outParameters, ref retValue);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    public async Task<ResultRpcMethod> InvokeRpcMethodAsync(
      IRpcCallableInstance symbol,
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultRpcMethod resultRpcMethod;
      try
      {
        resultRpcMethod = await ((IAdsRpcInvoke) this._client).InvokeRpcMethodAsync(symbol, method, inParameters, outSpecifiers, retSpecifier, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultRpcMethod;
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="retValue">The return value of the RPC method as object.</param>
    /// <returns>The ADS Error Code.</returns>
    /// <remarks>
    /// Because this overload doesn't provide any <see cref="T:TwinCAT.TypeSystem.AnyTypeSpecifier" /> specifications, only primitive datatypes will be correctly marshalled
    /// by this method. Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public AdsErrorCode TryInvokeRpcMethod(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      out object? retValue)
    {
      object[] outParameters = (object[]) null;
      return this.TryInvokeRpcMethod(symbolPath, methodName, inParameters, out outParameters, out retValue);
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="retValue">The return value of the RPC method as object.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <returns>The ADS Error Code.</returns>
    /// <remarks>
    /// Because this overload doesn't provide any <see cref="T:TwinCAT.TypeSystem.AnyTypeSpecifier" /> specifications, only primitive datatypes will be correctly marshalled
    /// by this method. Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public AdsErrorCode TryInvokeRpcMethod(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      out object[]? outParameters,
      out object? retValue)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      try
      {
        return ((IAdsRpcInvoke) this._client).TryInvokeRpcMethod(symbolPath, methodName, inParameters, ref outParameters, ref retValue);
      }
      finally
      {
        this.AfterAccess();
      }
    }

    /// <summary>Invokes the specified RPC Method asynchronously</summary>
    /// <param name="symbolPath">The symbol/Instance path of the symbol.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="cancel">The cancellation token</param>
    /// <returns>A task that represents the asynchronous 'InvokeRpcMethod' operation. The <see cref="T:TwinCAT.Ads.ResultRpcMethod" /> results contains
    /// the return value together with the output parameters.
    /// </returns>
    /// <remarks>
    /// Because this overload doesn't provide any <see cref="T:TwinCAT.TypeSystem.AnyTypeSpecifier" /> specifications, only primitive datatypes will be correctly marshalled
    /// by this method. Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public async Task<ResultRpcMethod> InvokeRpcMethodAsync(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.BeforeAccess();
      ResultRpcMethod resultRpcMethod;
      try
      {
        resultRpcMethod = await ((IAdsRpcInvoke) this._client).InvokeRpcMethodAsync(symbolPath, methodName, inParameters, cancel).ConfigureAwait(false);
      }
      finally
      {
        this.AfterAccess();
      }
      return resultRpcMethod;
    }

    /// <summary>
    /// Gets the name of this <see cref="T:TwinCAT.Ads.AdsConnection" />.
    /// </summary>
    /// <value>The name.</value>
    public string Name => string.Format((IFormatProvider) null, "Connection {0} (ID: {1})", (object) ((object) this._session.Address).ToString(), (object) this._id);

    /// <summary>
    /// Gets the UTC time when the Connection was originally established.
    /// </summary>
    /// <value>The connection established at.</value>
    public DateTimeOffset? ConnectionEstablishedAt => this._connectionEstablishTime;

    /// <summary>
    /// Gets the UTC time when tha last active/resurrected Connection was established
    /// </summary>
    /// <value>The active since.</value>
    public DateTimeOffset? ActiveSince => this._connectionActiveSince;

    /// <summary>
    /// Gets the number of tries to resurrect the <see cref="T:TwinCAT.Ads.AdsConnection" />.
    /// </summary>
    /// <value>The number of tried resurrections of the <see cref="T:TwinCAT.IConnection" />.</value>
    /// <exclude />
    [Obsolete("Use AdsConnection.TotalResurrectionTries")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int ResurrectingTries => this.TotalResurrectingTries;

    /// <summary>
    /// Gets the number of tries to resurrect the <see cref="T:TwinCAT.Ads.AdsConnection" />.
    /// </summary>
    /// <value>The number of tried resurrections of the <see cref="T:TwinCAT.IConnection" />.</value>
    public int TotalResurrectingTries => this._resurrectingTryCount;

    /// <summary>
    /// Gets the number of succeeded connection resurrections.
    /// </summary>
    /// <value>The resurrection count.</value>
    /// <exclude />
    [Obsolete("Use AdsConnection.TotalResurrections")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Resurrections => this.TotalResurrections;

    /// <summary>
    /// Gets the number of succeeded connection resurrections.
    /// </summary>
    /// <value>The resurrection count.</value>
    public int TotalResurrections => this._resurrections;

    /// <summary>Gets the connection lost count.</summary>
    /// <value>The connection lost count.</value>
    /// <exclude />
    [Obsolete("Use AdsConnection.TotalConnectionLosses")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int ConnectionLostCount => this.TotalConnectionLosses;

    /// <summary>Gets the connection lost count.</summary>
    /// <value>The connection lost count.</value>
    public int TotalConnectionLosses => this._connectionLostCount;

    /// <summary>Gets the timeout (in milliseconds)</summary>
    /// <value>The timeout.</value>
    public int Timeout
    {
      get => this._session.Settings.Timeout;
      set
      {
        this._session.Settings.Timeout = value;
        if (this._client == null)
          return;
        ((IConnection) this._client).Timeout = value;
      }
    }
  }
}
