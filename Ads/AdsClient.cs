// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsClient
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.Ads.Server;
using TwinCAT.Ads.SumCommand;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.Ads.ValueAccess;
using TwinCAT.Ams;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>ADS Client / ADS Communication object.</summary>
  /// <remarks>
  /// The class <see cref="T:TwinCAT.Ads.AdsClient" /> enables synchronous/asynchronous access to data of an ADS Device.
  /// </remarks>
  /// <example>
  /// The following sample shows how to instantiate and use the <see cref="T:TwinCAT.Ads.AdsClient" /> class.
  /// <code language="C#" title="AdsClient Demo (async)" source="..\..\Samples\Sample.Ads.AdsClientCore\AdsClientAsync.cs" region="CODE_SAMPLE" />
  /// <code language="C#" title="AdsClient Demo (sync)" source="..\..\Samples\Sample.Ads.AdsClientCore\AdsClient.cs" region="CODE_SAMPLE" />
  /// <code language="C#" title="Argument Parser" source="..\..\Samples\Sample.Ads.AdsClientCore\ArgParser.cs" region="CODE_SAMPLE" />
  /// The following sample shows how to call (Remote Procedures / Methods) within the PLC directly from the <see cref="T:TwinCAT.Ads.AdsClient" /> class.
  /// <code language="C#" title="RPC Call Example (async)" source="..\..\Samples\Sample.Ads.AdsClientCore\RpcCallAsync.cs" region="CODE_SAMPLE_RPCCALL" />
  /// <code language="C#" title="RPC Call Example (sync)" source="..\..\Samples\Sample.Ads.AdsClientCore\RpcCall.cs" region="CODE_SAMPLE_RPCCALL" />
  /// </example>
  [DebuggerDisplay("ID = { _id }, TargetAddress = {_targetAddress}, ClientAddress = { ClientAddress}, ConnectionState = {ConnectionState}, Transport = {Protocol}")]
  public sealed class AdsClient : 
    IAdsDisposableConnection,
    IAdsConnectAddress,
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
    IRouterNotificationProvider,
    IDisposable,
    IAdsHandleTableProvider,
    ITcAdsRaw,
    IInterceptedClient,
    IClientNotificationReceiver,
    INotificationReceiver,
    IStateChangedReceiver,
    ISymbolVersionChangedReceiver,
    IRouterNotificationReceiver,
    INotificationProvider,
    IAdsSymbolTableProvider,
    ILoggerProvider
  {
    private ILogger? _logger;
    /// <summary>Private AdsServer.</summary>
    private AdsClientServer? _server;
    /// <summary>The disposed indicator.</summary>
    private bool _disposed;
    /// <summary>The notification receiver</summary>
    private NotificationReceiverBase? _notificationReceiver;
    /// <summary>The symbol table</summary>
    private HandleTable? _symbolTable;
    /// <summary>List of Handle bags.</summary>
    private List<IDisposableHandleBag> _handleBags = new List<IDisposableHandleBag>();
    /// <summary>
    /// Static identifier counter of this <see cref="T:TwinCAT.Ads.AdsClient" />.
    /// </summary>
    private static int s_id;
    /// <summary>
    /// <see cref="T:TwinCAT.Ads.AdsClient" /> identifier
    /// </summary>
    private int _id = ++AdsClient.s_id;
    /// <summary>
    /// The actual Target <see cref="T:TwinCAT.Ads.AmsAddress" />.
    /// </summary>
    private AmsAddress _target = AmsAddress.Empty;
    /// <summary>
    /// Indicates that the <see cref="T:TwinCAT.Ads.AdsClient" /> is connected.
    /// </summary>
    private bool _isConnected;
    /// <summary>Router notification event handler delegate</summary>
    private EventHandler<AmsRouterNotificationEventArgs>? _amsRouterNotificationEventHandlerDelegate;
    /// <summary>ADS State changed handler delegate</summary>
    private EventHandler<AdsStateChangedEventArgs>? _adsStateChangedEventHandlerDelegate;
    /// <summary>StateChangedNotification registered indicator.</summary>
    private bool _stateChangedNotificationRegistered;
    /// <summary>
    /// Delegate for <see cref="E:TwinCAT.Ads.AdsClient.AdsSymbolVersionChanged" /> events.
    /// </summary>
    private EventHandler<AdsSymbolVersionChangedEventArgs>? _symbolVersionChangedDelegate;
    private bool _symbolVersionChangedNotificationRegistered;
    /// <summary>
    /// The actual <see cref="T:TwinCAT.Ads.AmsRouterState" />
    /// </summary>
    private AmsRouterState _routerState;
    private AnyTypeMarshaler _anyTypeMarshaller = new AnyTypeMarshaler();
    private static int DEFAULT_TIMEOUT = 5000;
    /// <summary>Cached timeout</summary>
    private int _timeout = AdsClient.DEFAULT_TIMEOUT;
    /// <summary>The session object.</summary>
    private ISession? _session;
    /// <summary>The interceptors</summary>
    private CommunicationInterceptors? _interceptors;
    private Encoding? _symbolEncoding;
    private ISymbolInfoTable? _symbolInfoTable;

    /// <summary>Gets the logger inteface.</summary>
    /// <value>The logger.</value>
    public ILogger? Logger => this._logger;

    public AdsClient(ISession? session, AdsClientSettings settings, ILogger? logger)
    {
      if (settings == null)
        throw new ArgumentNullException(nameof (settings));
      this._logger = logger;
      this._session = session;
      this._interceptors = settings.Interceptors;
      this._timeout = settings.Timeout;
      LogProviderExtension.LogInformation((ILoggerProvider) this, "AdsClient created: ID:{0}, Timeout: {1} ms", new object[2]
      {
        (object) this._id,
        (object) this._timeout
      });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsClient" /> class.
    /// </summary>
    /// <param name="session">The session.</param>
    /// <param name="logger">The logger.</param>
    /// <exclude />
    public AdsClient(ISession session, ILogger? logger)
      : this(session, AdsClientSettings.Default, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsClient" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <exclude />
    public AdsClient(ILogger? logger)
      : this((ISession) null, AdsClientSettings.Default, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsClient" /> class.
    /// </summary>
    public AdsClient()
      : this((ISession) null, AdsClientSettings.Default, (ILogger) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.AdsClient" /> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public AdsClient(AdsClientSettings settings)
      : this((ISession) null, settings, (ILogger) null)
    {
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.AdsClient" /> class.
    /// </summary>
    ~AdsClient() => this.Dispose(false);

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed => this._disposed;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (!this._disposed)
        this.Dispose(true);
      this._disposed = true;
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.Disconnect();
    }

    /// <summary>Gets the symbol table.</summary>
    /// <returns>SymbolTable.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    IHandleTable? IAdsHandleTableProvider.GetHandleTable() => (IHandleTable) this._symbolTable;

    /// <summary>Creates a handle bag from symbol paths.</summary>
    /// <param name="instancePath">A list of symbol paths.</param>
    /// <returns>A handle bag that can be disposed.</returns>
    IDisposableHandleBag<string> IAdsHandleTableProvider.CreateHandleBag(
      string[] instancePath)
    {
      IDisposableHandleBag<string> variableHandleBag = HandleBagFactory.CreateVariableHandleBag((IAdsConnection) this, instancePath);
      this._handleBags.Add((IDisposableHandleBag) variableHandleBag);
      return variableHandleBag;
    }

    /// <summary>
    /// Creates a notification handle bag form the specified symbols.
    /// </summary>
    /// <param name="symbols">The symbols.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">The user data.</param>
    /// <returns>A handle bag that can be disposed.</returns>
    IDisposableHandleBag<ISymbol> IAdsHandleTableProvider.CreateNotificationHandleBag(
      ISymbol[] symbols,
      NotificationSettings settings,
      object[]? userData)
    {
      IDisposableHandleBag<ISymbol> notificationHandleBag = HandleBagFactory.CreateNotificationHandleBag((IAdsConnection) this, symbols, settings, userData);
      this._handleBags.Add((IDisposableHandleBag) notificationHandleBag);
      return notificationHandleBag;
    }

    /// <summary>Creates the notification ex handle bag.</summary>
    /// <param name="symbols">The symbols.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">The user data.</param>
    /// <returns>IDisposableHandleBag.</returns>
    IDisposableHandleBag<AnySymbolSpecifier> IAdsHandleTableProvider.CreateNotificationExHandleBag(
      IList<AnySymbolSpecifier> symbols,
      NotificationSettings settings,
      object[]? userData)
    {
      IDisposableHandleBag<AnySymbolSpecifier> notificationExHandleBag = HandleBagFactory.CreateNotificationExHandleBag((IAdsConnection) this, symbols, settings, userData);
      this._handleBags.Add((IDisposableHandleBag) notificationExHandleBag);
      return notificationExHandleBag;
    }

    /// <summary>
    /// Unregisters the handle bag from this <see cref="T:TwinCAT.Ads.IAdsHandleTableProvider" />.
    /// </summary>
    /// <param name="bag">The handle bag.</param>
    void IAdsHandleTableProvider.UnregisterHandleBag(IDisposableHandleBag bag)
    {
      this._handleBags.Remove(bag);
      ((IDisposable) bag).Dispose();
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.Ads.AdsClient" /> Identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public int Id => this._id;

    /// <summary>
    /// Gets the Name of the <see cref="T:TwinCAT.Ads.AdsClient" /> object.
    /// </summary>
    /// <value>The name.</value>
    internal string Name => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "AdsClient_{0}", (object) this.Id);

    /// <summary>Occurs when the connection state has been changed.</summary>
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Called when the <see cref="P:TwinCAT.Ads.AdsClient.ConnectionState" /> of the <see cref="T:TwinCAT.Ads.AdsClient" /> has changed.
    /// </summary>
    /// <param name="newState">The new state.</param>
    /// <param name="oldState">The old state.</param>
    private void OnConnectionStateChanged(ConnectionState newState, ConnectionState oldState)
    {
      ConnectionStateChangedReason stateChangedReason = (ConnectionStateChangedReason) 0;
      if (newState == 1)
      {
        stateChangedReason = (ConnectionStateChangedReason) 2;
        if (this._interceptors != null)
          this._interceptors.Disconnect((Func<AdsErrorCode>) (() => (AdsErrorCode) 0));
      }
      else if (newState == 2)
      {
        stateChangedReason = (ConnectionStateChangedReason) 1;
        if (this._interceptors != null)
          this._interceptors.Connect((Func<AdsErrorCode>) (() => (AdsErrorCode) 0));
      }
      LogProviderExtension.LogInformation((ILoggerProvider) this, "{0} Connection state changed, NewState: {1}, OldState: {2}", new object[3]
      {
        (object) this.Name,
        (object) newState,
        (object) oldState
      });
      if (this.ConnectionStateChanged == null)
        return;
      this.ConnectionStateChanged((object) this, new ConnectionStateChangedEventArgs(stateChangedReason, newState, oldState, (Exception) null));
    }

    /// <summary>
    /// Connects to the target address as an asynchronous operation.
    /// </summary>
    /// <returns>Returns a task object that represents the <see cref="M:TwinCAT.Ads.AdsClient.ConnectAndWaitAsync(TwinCAT.Ads.AmsAddress,System.Threading.CancellationToken)" /> operation which returns an <see cref="T:TwinCAT.Ads.AdsErrorCode" />" as result..</returns>
    /// <remarks>The connection is hold until a cancel is requested, which means the method returns after cancelling/disconnecting.
    /// </remarks>
    /// <exclude />
    [Obsolete("Use the ConnectAndWaitAsync method instead", true)]
    public Task ConnectAsync(AmsAddress address, CancellationToken cancel) => this.ConnectAndWaitAsync(address, cancel);

    /// <summary>
    /// Connects to the target address and waits until the <see cref="T:TwinCAT.Ads.AdsClient" /> is disconnected asynchronously.
    /// </summary>
    /// <param name="address">The target address.</param>
    /// <param name="cancel">Cancellation Token.</param>
    /// <returns>Returns a task object that represents the <see cref="M:TwinCAT.Ads.AdsClient.ConnectAndWaitAsync(TwinCAT.Ads.AmsAddress,System.Threading.CancellationToken)" /> operation as result.</returns>
    /// <remarks>This method is used for scenarios, where the <see cref="T:TwinCAT.Ads.AdsClient" /> disconnects from other code asynchronously.
    /// When this method returns, the connection is already terminated and only additional cleanup code should be processed.
    /// </remarks>
    public async Task ConnectAndWaitAsync(AmsAddress address, CancellationToken cancel)
    {
      AdsClient adsClient = this;
      adsClient._target = address;
      if (adsClient._disposed)
        throw new ObjectDisposedException(adsClient.ToString());
      if (adsClient.IsConnected)
        adsClient.Disconnect();
      adsClient._symbolTable = new HandleTable((IAdsReadWrite) adsClient, adsClient._symbolEncoding);
      adsClient._notificationReceiver = (NotificationReceiverBase) new NotificationReceiver((INotificationProvider) adsClient, (IHandleTable) adsClient._symbolTable, (IClientNotificationReceiver) adsClient);
      LogProviderExtension.LogDebug((ILoggerProvider) adsClient, "ConnectAndWaitAsync (Before), Client:{0}, Address:  {1}", new object[2]
      {
        (object) adsClient.Name,
        (object) address
      });
      adsClient._server = new AdsClientServer(adsClient._notificationReceiver, (IRouterNotificationReceiver) adsClient, adsClient._logger);
      adsClient._server.ServerConnectionStateChanged += new EventHandler<ServerConnectionStateChangedEventArgs>(adsClient._server_ServerConnectionStateChanged);
      AdsErrorCode adsErrorCode = await adsClient._server.ConnectServerAndWaitAsync(cancel).ConfigureAwait(false);
      LogProviderExtension.LogDebug((ILoggerProvider) adsClient, "ConnectAndWaitAsync (After), Client:{0}, Address:  {1}, ErrorCode: {2}", new object[3]
      {
        (object) adsClient.Name,
        (object) address,
        (object) adsErrorCode
      });
      adsClient._isConnected = true;
      adsClient.OnConnected();
    }

    private void _server_ServerConnectionStateChanged(
      object? sender,
      ServerConnectionStateChangedEventArgs e)
    {
    }

    /// <summary>
    /// Connect this ADS server to the local ADS router.
    /// <exception cref="T:TwinCAT.Ads.Server.AdsServerException">Thrown if the connect call fails.</exception>
    /// </summary>
    /// <returns>System.UInt32.</returns>
    /// <exception cref="T:System.Exception">Target not specified!</exception>
    /// <exclude />
    private uint ConnectServer() => !AmsAddress.op_Equality(this._target, (AmsAddress) null) && this._server != null ? this._server.ConnectServer() : throw new Exception("Target not specified!");

    /// <summary>Connects the target</summary>
    /// <param name="address">The address.</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public void Connect(AmsAddress address)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.ToString());
      if (this.IsConnected)
        this.Disconnect();
      this._symbolTable = new HandleTable((IAdsReadWrite) this, this._symbolEncoding);
      this._notificationReceiver = (NotificationReceiverBase) new NotificationReceiver((INotificationProvider) this, (IHandleTable) this._symbolTable, (IClientNotificationReceiver) this);
      this._server = new AdsClientServer(this._notificationReceiver, (IRouterNotificationReceiver) this, this._logger);
      this._server.ServerConnectionStateChanged += new EventHandler<ServerConnectionStateChangedEventArgs>(this._server_ServerConnectionStateChanged);
      this._target = address;
      LogProviderExtension.LogDebug((ILoggerProvider) this, "Connect (Before), Client:{0}, Address: {1}", new object[2]
      {
        (object) this.Name,
        (object) address
      });
      this._server.ConnectServer();
      this._isConnected = true;
      this.OnConnected();
    }

    /// <summary>
    /// Gets a value indicating whether the local ADS port was opened successfully. It
    /// does not indicate if the target port is available. Use the method ReadState to
    /// determine if the target port is available.
    /// </summary>
    /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
    public bool IsConnected => this._isConnected;

    /// <summary>Connects to the target ADS Device.</summary>
    /// <param name="netId">The AmsNetId of the target device.</param>
    /// <param name="port">The Ams Port number on the target device to connect to.</param>
    public void Connect(AmsNetId netId, int port) => this.Connect(new AmsAddress(netId, port));

    /// <summary>Connects to the local target ADS Device.</summary>
    /// <param name="port">The port number of the local ADS target device to connect to.</param>
    public void Connect(int port) => this.Connect(AmsNetId.Local, port);

    /// <summary>Connects to the local target ADS Device.</summary>
    /// <param name="port">The port number of the local ADS target device to connect to.</param>
    public void Connect(AmsPort port) => this.Connect((int) port);

    /// <summary>Connects to the target ADS Device.</summary>
    /// <param name="netId">The <see cref="T:TwinCAT.Ads.AmsNetId" /> of the ADS target device specified as string.</param>
    /// <param name="port">The port number of the ADS target device.</param>
    public void Connect(AmsNetId netId, AmsPort port) => this.Connect(netId, (int) port);

    /// <summary>Connects to the target ADS Device.</summary>
    /// <param name="netId">The <see cref="T:TwinCAT.Ads.AmsNetId" /> of the ADS target device specified as string.</param>
    /// <param name="port">The port number of the ADS target device.</param>
    public void Connect(string netId, int port) => this.Connect(AmsNetId.Parse(netId), port);

    /// <summary>
    /// Handler function that is called, when the <see cref="T:TwinCAT.Ads.AdsClient" /> is connected.
    /// </summary>
    private void OnConnected()
    {
      LogProviderExtension.LogInformation((ILoggerProvider) this, "OnConnected, Client:{0}, Address: {1}, ClientAddress: {2}", new object[3]
      {
        (object) this.Name,
        (object) this.Address,
        (object) this.ClientAddress
      });
      this.AddEventNotifications();
      this.OnConnectionStateChanged((ConnectionState) 2, (ConnectionState) 1);
    }

    /// <summary>Adds the event notifications.</summary>
    private AdsErrorCode AddEventNotifications()
    {
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) 0;
      if (this._notificationReceiver == null)
        ((AdsErrorCode) 1864).ThrowOnError();
      if (this._adsStateChangedEventHandlerDelegate != null)
      {
        AdsErrorCode adsErrorCode2 = this.registerStateChangedNotification(this._timeout);
        throw AdsErrorException.Create(ResMan.GetString("AdsStateChangedRegistrationFailed_Message"), adsErrorCode2);
      }
      if (this._symbolVersionChangedDelegate != null)
      {
        AdsErrorCode adsErrorCode3 = this.registerSymbolVersionChangedNotification(this._timeout);
        throw AdsErrorException.Create(ResMan.GetString("SymbolVersionChangedRegistrationFailed_Message"), adsErrorCode3);
      }
      return adsErrorCode1;
    }

    /// <summary>
    /// Disconnects this <see cref="T:TwinCAT.Ads.AdsClient" /> from the local ADS router.
    /// </summary>
    /// <returns><c>true</c> if disconnected, <c>false</c> otherwise.</returns>
    public bool Disconnect() => this.OnDisconnect();

    /// <summary>
    /// Called when the <see cref="T:TwinCAT.Ads.AdsClient" /> is about to be disconnected.
    /// </summary>
    /// <returns><c>true</c> if disconnected, <c>false</c> otherwise.</returns>
    private bool OnDisconnect()
    {
      bool isConnected = this._isConnected;
      if (this._symbolInfoTable is IDisposable symbolInfoTable)
        symbolInfoTable.Dispose();
      this._symbolInfoTable = (ISymbolInfoTable) null;
      if (this._notificationReceiver != null)
      {
        this._notificationReceiver.Dispose();
        this._notificationReceiver = (NotificationReceiverBase) null;
        this._symbolVersionChangedNotificationRegistered = false;
        this._stateChangedNotificationRegistered = false;
      }
      if (this._server != null)
        this._server.Dispose();
      this._server = (AdsClientServer) null;
      if (this._symbolTable != null)
      {
        this._symbolTable.Dispose();
        this._symbolTable = (HandleTable) null;
      }
      bool flag = true;
      this._isConnected = false;
      LogProviderExtension.LogInformation((ILoggerProvider) this, "OnDisconnect: Client:{0}, Address: {1}", new object[2]
      {
        (object) this.Name,
        (object) this.Address
      });
      if (flag & isConnected)
        this.OnConnectionStateChanged((ConnectionState) 1, (ConnectionState) 2);
      return true;
    }

    /// <summary>
    /// Gets the target <see cref="T:TwinCAT.Ads.AmsAddress" /> of of the established ADS connection (Destination side).
    /// </summary>
    /// <value>The address.</value>
    public AmsAddress Address => this._target;

    /// <summary>
    /// Get the client <see cref="T:TwinCAT.Ads.AmsAddress" /> (Source side).
    /// </summary>
    /// <value>The client address.</value>
    public AmsAddress ClientAddress
    {
      get
      {
        AmsAddress clientAddress = AmsAddress.Empty;
        if (this._server != null)
          clientAddress = this._server.ServerAddress;
        return clientAddress;
      }
    }

    private static void ThrowIfFailed(Func<AdsErrorCode> action, string errorMessage)
    {
      AdsErrorCode adsErrorCode = action();
      if (adsErrorCode == null)
        return;
      if (errorMessage != null)
        throw new AdsErrorException(errorMessage, adsErrorCode);
      adsErrorCode.ThrowOnError();
    }

    /// <summary>
    /// Throws an <see cref="T:TwinCAT.Ads.AdsErrorException" /> with the specified errorMessage, if the return value of the Function indicates an error.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException"></exception>
    private static void ThrowIfFailed(Func<ResultAds> action, string errorMessage)
    {
      ResultAds resultAds = action();
      if (resultAds.ErrorCode == null)
        return;
      if (errorMessage != null)
        throw new AdsErrorException(errorMessage, resultAds.ErrorCode);
      resultAds.ErrorCode.ThrowOnError();
    }

    /// <summary>
    /// Occurs when Notifications are send (bundled notifications)
    /// </summary>
    /// <remarks>As an optimization, this event receives all ADS Notifications that occurred at one
    /// point in time together. As consequence, the overhead of handler code is reduced, what can be important
    /// if notifications are triggered in a high frequency and the event has to be synchronized to the UI thread
    /// context. Because multiple notifications are bound together, less thread synchronization is necessary.
    /// The <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> and <see cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" /> events shouldn't be used when SumNotifications are registered, because they
    /// have an performance side effect to this <see cref="E:TwinCAT.Ads.AdsClient.AdsSumNotification" /> event. The full performance is reached only, when all notifications are handled
    /// on this event.
    /// </remarks>
    /// <example>
    /// Example of receiving <see cref="E:TwinCAT.Ads.AdsClient.AdsSumNotification" /> events.
    /// <code source="..\..\Samples\TwinCAT.ADS.NET_Samples\03_ADS.NET_EventReading\Form1.cs" region="CODE_SAMPLE_SUMNOTIFICATIONS_ASYNC" removeRegionMarkers="true" language="csharp" title="Trigger on changed values by ADS Notifications" />
    /// </example>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotification" />
    public event EventHandler<AdsSumNotificationEventArgs>? AdsSumNotification;

    /// <summary>
    /// Occurs when Notification Unregistrations / Invalidates are received from the AdsServer
    /// </summary>
    /// <remarks>Some ADS servers are sending 0-size notifications, when the Notification handle is not valid anymore.
    /// If received, this event will be triggered, to notify any consumers to invalidate the notification handles.
    /// One example for these sort of invalidation is, if ADS Notifications are already registered at the PLC ADS Server, and the PLC Control downloads a new program. All registered
    /// notification handles are invalidated!</remarks>
    public event EventHandler<AdsNotificationsInvalidatedEventArgs>? AdsNotificationsInvalidated;

    /// <summary>
    /// Occurs when the ADS device sends a notification to the client.
    /// </summary>
    /// <remarks>The Event Argument contains the raw data value of the notification, not marshaled to .NET types.</remarks>
    /// <example>
    /// Example of receiving <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> events.
    /// <code source="..\..\Samples\TwinCAT.ADS.NET_Samples\03_ADS.NET_EventReading\Form1.cs" region="CODE_SAMPLE_NOTIFICATIONS_ASYNC" removeRegionMarkers="true" language="csharp" title="Trigger on changed values by ADS Notifications" />
    /// </example>
    public event EventHandler<AdsNotificationEventArgs>? AdsNotification;

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
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotification" />
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" />
    public event EventHandler<AdsNotificationErrorEventArgs>? AdsNotificationError;

    /// <summary>
    /// Occurs when the ADS devices sends a notification to the client.
    /// </summary>
    /// <remarks>The Notification event arguments marshals the data value automatically to the specified .NET Type with ANY_TYPE marshallers.
    /// </remarks>
    /// <example>
    /// Example of receiving <see cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" /> events.
    /// <code source="..\..\Samples\TwinCAT.ADS.NET_Samples\14_ADS.NET_ReadWriteAnyType\Form1.cs" region="CODE_READWRITEANYNOT_ASYNC" removeRegionMarkers="true" language="csharp" title="Trigger on changed values by ADS Notifications" />
    /// </example>
    public event EventHandler<AdsNotificationExEventArgs>? AdsNotificationEx;

    /// <summary>
    /// Occurs when the state of the local Router has changed.
    /// </summary>
    public event EventHandler<AmsRouterNotificationEventArgs>? RouterStateChanged
    {
      add
      {
        if (this.IsDisposed)
          throw new ObjectDisposedException(this.Name);
        this._amsRouterNotificationEventHandlerDelegate += value;
      }
      remove => this._amsRouterNotificationEventHandlerDelegate -= value;
    }

    /// <summary>Occurs when the ADS state changes.</summary>
    /// <remarks>This works only for ports that support Notifications (e.g. Port 851 but not Port 10000).
    /// 
    /// </remarks>
    public event EventHandler<AdsStateChangedEventArgs>? AdsStateChanged
    {
      add
      {
        if (this.IsDisposed)
          throw new ObjectDisposedException(this.Name);
        if (this.IsConnected && this._adsStateChangedEventHandlerDelegate == null)
        {
          AdsErrorCode adsErrorCode = this.registerStateChangedNotification(this._timeout);
          if (AdsErrorCodeExtensions.Failed(adsErrorCode))
          {
            AdsErrorException adsErrorException = AdsErrorException.Create(ResMan.GetString("AdsStateChangedRegistrationFailed_Message"), adsErrorCode);
            ((INotificationReceiver) this).OnNotificationError((Exception) adsErrorException);
            throw adsErrorException;
          }
        }
        this._adsStateChangedEventHandlerDelegate += value;
      }
      remove
      {
        if (this.IsDisposed)
          throw new ObjectDisposedException(this.Name);
        this._adsStateChangedEventHandlerDelegate -= value;
        if (!this.IsConnected || this._adsStateChangedEventHandlerDelegate != null || !this._stateChangedNotificationRegistered)
          return;
        this.unregisterStateChangedNotification(this._timeout);
      }
    }

    private AdsErrorCode registerStateChangedNotification(int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) -1;
      if (!this._stateChangedNotificationRegistered)
      {
        if (this._notificationReceiver != null)
          adsErrorCode = this._notificationReceiver.RegisterStateChangedNotification(NotificationSettings.ImmediatelyOnChange, timeout);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
          this._stateChangedNotificationRegistered = true;
      }
      return adsErrorCode;
    }

    /// <summary>Unregisters the state changed notification.</summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    private AdsErrorCode unregisterStateChangedNotification(int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) -1;
      if (this._stateChangedNotificationRegistered && this._notificationReceiver != null)
      {
        adsErrorCode = this._notificationReceiver.UnregisterStateChangedNotification(timeout);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
          this._stateChangedNotificationRegistered = false;
      }
      return adsErrorCode;
    }

    /// <summary>
    /// Registers for <see cref="E:TwinCAT.Ads.AdsClient.AdsStateChanged" /> events as an asynchronous operation.
    /// </summary>
    /// <param name="handler">The handler function to be registered for AdsStateChanged calls.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'RegisterAdsStateChanged' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the state
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public async Task<ResultAds> RegisterAdsStateChangedAsync(
      EventHandler<AdsStateChangedEventArgs> handler,
      CancellationToken cancel)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      ResultAds resultAds = ResultAds.CreateError((AdsErrorCode) 1);
      this._adsStateChangedEventHandlerDelegate += handler;
      if (this.IsConnected && !this._stateChangedNotificationRegistered)
      {
        if (this._notificationReceiver != null)
          resultAds = await this._notificationReceiver.RegisterStateChangedNotificationAsync(NotificationSettings.ImmediatelyOnChange, cancel).ConfigureAwait(false);
        if (resultAds.Succeeded)
          this._stateChangedNotificationRegistered = true;
      }
      else
        resultAds = ResultAds.CreateError((AdsErrorCode) 0);
      return resultAds;
    }

    /// <summary>
    /// unregister ads state changed as an asynchronous operation.
    /// </summary>
    /// <param name="handler">The handler function to be unregistered.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'UnregisterAdsStateChanged' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the state
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public async Task<ResultAds> UnregisterAdsStateChangedAsync(
      EventHandler<AdsStateChangedEventArgs> handler,
      CancellationToken cancel)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      this._adsStateChangedEventHandlerDelegate -= handler;
      ResultAds resultAds = ResultAds.CreateError((AdsErrorCode) 1);
      if (this._adsStateChangedEventHandlerDelegate == null && this.IsConnected && this._stateChangedNotificationRegistered)
      {
        if (this._notificationReceiver != null)
          resultAds = await this._notificationReceiver.UnregisterStateChangedNotificationAsync(cancel).ConfigureAwait(false);
        if (resultAds.Succeeded)
          this._stateChangedNotificationRegistered = false;
      }
      else
        resultAds = ResultAds.CreateError((AdsErrorCode) 0);
      return resultAds;
    }

    /// <summary>
    /// Occurs when the symbol version has been changed changes.
    /// </summary>
    /// <remarks>This is the case when the connected ADS server restarts. This invalidates all actual opened
    /// symbol handles.
    /// The SymbolVersion counter doesn't trigger, when an online change is made on the PLC (ports 801, ..., 851 ...)</remarks>
    public event EventHandler<AdsSymbolVersionChangedEventArgs>? AdsSymbolVersionChanged
    {
      add
      {
        if (this.IsDisposed)
          throw new ObjectDisposedException(this.Name);
        if (this._symbolVersionChangedDelegate == null && this._notificationReceiver != null)
        {
          AdsErrorCode adsErrorCode = this._notificationReceiver.RegisterSymbolVersionChangedNotification(NotificationSettings.ImmediatelyOnChange, this._timeout);
          if (AdsErrorCodeExtensions.Failed(adsErrorCode))
          {
            AdsErrorException adsErrorException = AdsErrorException.Create(ResMan.GetString("SymbolVersionChangedRegistrationFailed_Message"), adsErrorCode);
            ((INotificationReceiver) this).OnNotificationError((Exception) adsErrorException);
            throw adsErrorException;
          }
        }
        this._symbolVersionChangedDelegate += value;
      }
      remove
      {
        if (this.IsDisposed)
          throw new ObjectDisposedException(this.Name);
        this._symbolVersionChangedDelegate -= value;
        if (this._symbolVersionChangedDelegate != null || this._notificationReceiver == null)
          return;
        this._notificationReceiver.UnregisterSymbolVersionChangedNotification(this._timeout);
      }
    }

    /// <summary>
    /// Registers for the <see cref="E:TwinCAT.Ads.AdsClient.AdsSymbolVersionChanged" /> event.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    private AdsErrorCode registerSymbolVersionChangedNotification(int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) -1;
      if (!this._symbolVersionChangedNotificationRegistered)
      {
        adsErrorCode = this._notificationReceiver.RegisterSymbolVersionChangedNotification(NotificationSettings.ImmediatelyOnChange, timeout);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
          this._symbolVersionChangedNotificationRegistered = true;
      }
      return adsErrorCode;
    }

    /// <summary>
    /// Unregisters from the <see cref="E:TwinCAT.Ads.AdsClient.AdsSymbolVersionChanged" /> event.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    private AdsErrorCode unregisterSymbolVersionChangedNotification(int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) -1;
      if (this._symbolVersionChangedNotificationRegistered && this._notificationReceiver != null)
      {
        adsErrorCode = this._notificationReceiver.UnregisterSymbolVersionChangedNotification(timeout);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
          this._symbolVersionChangedNotificationRegistered = false;
      }
      return adsErrorCode;
    }

    /// <summary>
    /// Registers for an <see cref="E:TwinCAT.Ads.AdsClient.AdsSymbolVersionChanged" /> event as an asynchronous operation.
    /// </summary>
    /// <param name="handler">The handler function to register.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'RegisterSymbolVersionChanged' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public async Task<ResultAds> RegisterSymbolVersionChangedAsync(
      EventHandler<AdsSymbolVersionChangedEventArgs> handler,
      CancellationToken cancel)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      this._symbolVersionChangedDelegate += handler;
      ResultAds resultAds = ResultAds.CreateError((AdsErrorCode) 1);
      if (this.IsConnected && !this._symbolVersionChangedNotificationRegistered)
      {
        if (this._notificationReceiver != null)
          resultAds = await this._notificationReceiver.RegisterSymbolVersionChangedNotificationAsync(NotificationSettings.ImmediatelyOnChange, cancel).ConfigureAwait(false);
        if (resultAds.Succeeded)
          this._symbolVersionChangedNotificationRegistered = true;
      }
      else
        resultAds = ResultAds.CreateError((AdsErrorCode) 0);
      return resultAds;
    }

    /// <summary>
    /// Unregisters from an <see cref="E:TwinCAT.Ads.AdsClient.AdsSymbolVersionChanged" /> event as an asynchronous operation.
    /// </summary>
    /// <param name="handler">The handler function to unregister.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'UnregisterSymbolVersionChangedAsync' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    public async Task<ResultAds> UnregisterSymbolVersionChangedAsync(
      EventHandler<AdsSymbolVersionChangedEventArgs> handler,
      CancellationToken cancel)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      this._symbolVersionChangedDelegate -= handler;
      ResultAds resultAds = ResultAds.CreateError((AdsErrorCode) 1);
      if (this._symbolVersionChangedDelegate == null && this.IsConnected && this._symbolVersionChangedNotificationRegistered)
      {
        if (this._notificationReceiver != null)
          resultAds = await this._notificationReceiver.UnregisterSymbolVersionChangedNotificationAsync(cancel).ConfigureAwait(false);
        this._symbolVersionChangedNotificationRegistered = false;
      }
      else
        resultAds = ResultAds.CreateError((AdsErrorCode) 0);
      return resultAds;
    }

    /// <summary>Handler Function for a Router Notification.</summary>
    /// <param name="state">The route state.</param>
    void IRouterNotificationReceiver.OnRouterNotification(
      AmsRouterState state)
    {
      if (this._amsRouterNotificationEventHandlerDelegate != null)
      {
        try
        {
          this._amsRouterNotificationEventHandlerDelegate((object) this, new AmsRouterNotificationEventArgs(state));
        }
        catch (Exception ex)
        {
          AdsModule.Trace.TraceWarning(ex);
        }
      }
      AmsRouterState routerState = this._routerState;
      this._routerState = state;
      AdsModule.TraceSession.TraceInformation("RouterState changed to '{0}'", new object[1]
      {
        (object) state
      });
      switch (state - 1)
      {
        case 1:
          AdsException error;
          if (routerState != 1 || this.TryResurrect(out error))
            break;
          AdsModule.TraceSession.TraceWarning((Exception) error);
          break;
      }
    }

    /// <summary>
    /// Handles the <see cref="E:SymbolVersionChanged" /> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="T:TwinCAT.Ads.AdsSymbolVersionChangedEventArgs" /> instance containing the event data.</param>
    void ISymbolVersionChangedReceiver.OnSymbolVersionChanged(
      AdsSymbolVersionChangedEventArgs eventArgs)
    {
      if (this._symbolVersionChangedDelegate == null)
        return;
      try
      {
        this._symbolVersionChangedDelegate((object) this, eventArgs);
      }
      catch (Exception ex)
      {
        AdsModule.Trace.TraceError(ex);
      }
    }

    /// <summary>
    /// Handles the <see cref="E:AdsStateChanged" /> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="T:TwinCAT.Ads.AdsStateChangedEventArgs" /> instance containing the event data.</param>
    void IStateChangedReceiver.OnAdsStateChanged(
      AdsStateChangedEventArgs eventArgs)
    {
      if (this._adsStateChangedEventHandlerDelegate == null)
        return;
      this._adsStateChangedEventHandlerDelegate((object) this, eventArgs);
    }

    /// <summary>Handler function for Notification errors.</summary>
    /// <param name="timeStamp">The time stamp.</param>
    /// <param name="notifications">The notifications.</param>
    void INotificationReceiver.OnNotificationError(
      DateTimeOffset timeStamp,
      IList<Notification> notifications)
    {
      if (this.AdsNotificationError == null)
        return;
      foreach (Notification notification in (IEnumerable<Notification>) notifications)
      {
        AdsModule.Trace.TraceError("Notification error Handle: {0}", new object[1]
        {
          (object) notification.Handle
        });
        this.AdsNotificationError((object) this, new AdsNotificationErrorEventArgs((Exception) new AdsInvalidNotificationException(notification.Handle, timeStamp)));
      }
    }

    /// <summary>Handler function for Notification errors.</summary>
    /// <param name="e">The e.</param>
    void INotificationReceiver.OnNotificationError(Exception e)
    {
      AdsModule.Trace.TraceError(e);
      if (this.AdsNotificationError == null)
        return;
      this.AdsNotificationError((object) this, new AdsNotificationErrorEventArgs(e));
    }

    /// <summary>Handler function Raw Notifications</summary>
    /// <param name="timeStamp">The time stamp.</param>
    /// <param name="notifications"></param>
    void INotificationReceiver.OnNotification(
      DateTimeOffset timeStamp,
      IList<Notification> notifications)
    {
      bool flag1 = this.AdsNotification != null;
      bool flag2 = this.AdsNotificationEx != null;
      this.OnAdsSumNotifications(timeStamp, notifications);
      if (!(flag1 | flag2))
        return;
      foreach (Notification notification in (IEnumerable<Notification>) notifications)
      {
        bool flag3 = false;
        if (this.AdsNotificationEx != null)
        {
          this.OnAdsNotificationEx(notification);
          flag3 = true;
        }
        if (this.AdsNotification != null)
        {
          this.OnAdsNotification(notification);
          flag3 = true;
        }
        if (!flag3)
          LogProviderExtension.LogWarning((ILoggerProvider) this, "Notification event not registered!", Array.Empty<object>());
      }
    }

    void INotificationReceiver.OnInvalidateHandles(
      DateTimeOffset timeStamp,
      IList<Notification> notifications)
    {
      this.OnAdsNotificationsInvalidated(timeStamp, notifications);
    }

    /// <summary>Called when [ads notification].</summary>
    /// <param name="notification">The notification.</param>
    private void OnAdsNotification(Notification notification)
    {
      if (this.AdsNotification == null)
        return;
      this.AdsNotification((object) this, new AdsNotificationEventArgs(notification));
    }

    /// <summary>Called when [ads notification ex].</summary>
    /// <param name="notification">The notification.</param>
    private void OnAdsNotificationEx(Notification notification)
    {
      if (this.AdsNotificationEx == null || notification.UserData == null || !(notification.UserData.GetType() == typeof (AdsNotificationExUserData)))
        return;
      AdsNotificationExUserData userData = (AdsNotificationExUserData) notification.UserData;
      object obj;
      this._anyTypeMarshaller.Unmarshal(userData.type, userData.args, notification.Data.Span, StringMarshaler.DefaultEncoding, out obj);
      this.AdsNotificationEx((object) this, new AdsNotificationExEventArgs(new Notification(notification.Handle, notification.TimeStamp, userData.userData, notification.Data), obj));
    }

    /// <summary>Called when [ads sum notifications].</summary>
    /// <param name="timeStamp">The time stamp.</param>
    /// <param name="notifications">The notifications.</param>
    private void OnAdsSumNotifications(DateTimeOffset timeStamp, IList<Notification> notifications)
    {
      if (this.AdsSumNotification == null)
        return;
      this.AdsSumNotification((object) this, new AdsSumNotificationEventArgs(timeStamp, notifications));
    }

    private void OnAdsNotificationsInvalidated(
      DateTimeOffset timeStamp,
      IList<Notification> notifications)
    {
      if (this.AdsNotificationsInvalidated == null)
        return;
      this.AdsNotificationsInvalidated((object) this, new AdsNotificationsInvalidatedEventArgs(timeStamp, notifications));
    }

    /// <summary>
    /// Gets a value indicating whether the ADS client is connected to a ADS Server on the local
    /// computer.
    /// </summary>
    /// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public bool IsLocal => AmsAddress.op_Inequality(this._target, (AmsAddress) null) && this._target.NetId.IsLocal;

    /// <summary>
    /// Sets the timeout for the ads communication. Unit is in ms.
    /// </summary>
    public int Timeout
    {
      get => this._timeout;
      set
      {
        if (this.IsDisposed || this._timeout == value)
          return;
        this.OnSetTimout(value);
      }
    }

    /// <summary>Sets the Timeout internally.</summary>
    /// <param name="value">The value.</param>
    private void OnSetTimout(int value)
    {
      this._timeout = value;
      LogProviderExtension.LogInformation((ILoggerProvider) this, "AdsClient: {0}. Timeout set to '{1}' ms", new object[2]
      {
        (object) this.Name,
        (object) this._timeout
      });
    }

    /// <summary>
    /// Gets the session that initiated this <see cref="T:TwinCAT.IConnection" />
    /// </summary>
    /// <value>The session or NULL</value>
    /// <remarks>The Session can be null on standalone connections.</remarks>
    public ISession? Session => this._session;

    /// <summary>
    /// Gets the current Connection state of the <see cref="T:TwinCAT.IConnectionStateProvider" />
    /// </summary>
    /// <value>The state of the connection.</value>
    /// <Exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConnectionState ConnectionState => this.IsConnected ? (ConnectionState) 2 : (ConnectionState) 1;

    /// <summary>Gets the interceptors.</summary>
    /// <value>The interceptors.</value>
    /// <exclude />
    public CommunicationInterceptors? Interceptors => this._interceptors;

    /// <summary>
    /// Reads the identification and version number of an ADS server.
    /// </summary>
    /// <returns>DeviceInfo struct containing the name of the device and the version information.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public DeviceInfo ReadDeviceInfo()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultDeviceInfo result = this.ReadDeviceInfoSync();
      ((ResultAds) result).ThrowOnError();
      return result.DeviceInfo;
    }

    /// <summary>
    /// Reads the identification and version number of an ADS server.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadDeviceState' operation. The <see cref="T:TwinCAT.Ads.ResultDeviceInfo" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultDeviceInfo.DeviceInfo" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultDeviceInfo> ReadDeviceInfoAsync(CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._server.RequestReadDeviceInfoAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
          result = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.ReadDeviceInfoRequestAsync(this._target, id, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultDeviceInfo>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout, cancel);
    }

    /// <summary>
    /// Reads the identification and version number of an ADS server.
    /// </summary>
    /// <returns>A task that represents the asynchronous 'ReadDeviceState' operation. The <see cref="T:TwinCAT.Ads.ResultDeviceInfo" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultDeviceInfo.DeviceInfo" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    private ResultDeviceInfo ReadDeviceInfoSync()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._server.RequestReadDeviceInfoSync((Func<uint, AdsErrorCode>) (id =>
      {
        AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
        if (this._interceptors != null)
          adsErrorCode = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(adsErrorCode) ? this._server.ReadDeviceInfoRequestSync(this._target, id) : adsErrorCode;
      }), (Action<ResultDeviceInfo>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout);
    }

    /// <summary>Read data asynchronously.</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultReadBytes&gt;.</returns>
    private Task<ResultReadBytes> ReadAsync(
      uint indexGroup,
      uint indexOffset,
      int length,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._server.RequestReadBytesAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
          result = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.ReadRequestAsync(this._target, id, indexGroup, indexOffset, length, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultReadBytes>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout, cancel);
    }

    /// <summary>Read/Writes as asynchronous operation.</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readLength">Length of the read.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultReadWriteBytes&gt;.</returns>
    private async Task<ResultReadWriteBytes> ReadWriteAsync(
      uint indexGroup,
      uint indexOffset,
      int readLength,
      ReadOnlyMemory<byte> writeBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultReadBytes resultReadBytes = await this._server.RequestReadBytesAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
          result = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.ReadWriteRequestAsync(this._target, id, indexGroup, indexOffset, readLength, writeBuffer, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultReadBytes>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout, cancel).ConfigureAwait(false);
      return new ResultReadWriteBytes(((ResultAds) resultReadBytes).ErrorCode, resultReadBytes.Data, ((ResultAds) resultReadBytes).InvokeId);
    }

    /// <summary>Read/Writes as asynchronous operation.</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readLength">Length of the read.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>Task&lt;ResultReadWriteBytes&gt;.</returns>
    private ResultReadWriteBytes ReadWriteSync(
      uint indexGroup,
      uint indexOffset,
      int readLength,
      ReadOnlyMemory<byte> writeBuffer)
    {
      ResultReadBytes readBytesSync = this._server.RequestAndReceiveReadBytesSync((Func<uint, AdsErrorCode>) (id =>
      {
        AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
        if (this._interceptors != null)
          adsErrorCode = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(adsErrorCode) ? this._server.ReadWriteRequestSync(this._target, (uint) this.Id, indexGroup, indexOffset, readLength, writeBuffer.Span) : adsErrorCode;
      }), (Action<ResultReadBytes>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout);
      return new ResultReadWriteBytes(((ResultAds) readBytesSync).ErrorCode, readBytesSync.Data, ((ResultAds) readBytesSync).InvokeId);
    }

    /// <summary>
    /// Adds a device notification as an asynchronous operation.
    /// </summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
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
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.IAdsNotifications.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" /> should always
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
      return await this._notificationReceiver.AddDeviceNotificationAsync(indexGroup, indexOffset, dataSize, settings, userData, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> event.
    /// </summary>
    /// <param name="symbolPath">Symbol / Instance path of the ADS variable.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <returns>The notification handle.</returns>
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotification" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    public uint AddDeviceNotification(
      string symbolPath,
      int dataSize,
      NotificationSettings settings,
      object? userData)
    {
      uint notificationHandle = 0;
      this.TryAddDeviceNotification(symbolPath, dataSize, settings, userData, out notificationHandle).ThrowOnError();
      return notificationHandle;
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> event.
    /// </summary>
    /// <param name="symbolPath">The symbol/instance path of the ADS variable.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="notificationHandle">The notification handle.</param>
    /// <returns>The ADS ErrorCode.</returns>
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.TryDeleteDeviceNotification(System.UInt32)" /> should always
    /// be called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotification" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryDeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    public AdsErrorCode TryAddDeviceNotification(
      string symbolPath,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      out uint notificationHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      bool flag = false;
      uint clientHandle = 0;
      notificationHandle = 0U;
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) -1;
      AdsErrorCode adsErrorCode2;
      while (true)
      {
        adsErrorCode2 = this._symbolTable.TryCreateVariableHandle(symbolPath, this._timeout, out clientHandle);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
        {
          adsErrorCode2 = this.TryAddDeviceNotification(61445U, clientHandle, dataSize, settings, userData, out notificationHandle);
          if (adsErrorCode2 == 1809 && !flag)
          {
            adsErrorCode1 = this._symbolTable.Resurrect(clientHandle);
            flag = true;
          }
          else
            break;
        }
        else
          break;
      }
      return adsErrorCode2;
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="symbolPath">Symbol/Instance path of the ADS variable.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <param name="args">Additional arguments (for 'AnyType')</param>
    /// <param name="notificationHandle">The notification handle</param>
    /// <returns>The ADS error code.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.TryDeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    public AdsErrorCode TryAddDeviceNotificationEx(
      string symbolPath,
      NotificationSettings settings,
      object? userData,
      Type type,
      int[]? args,
      out uint notificationHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      bool flag = false;
      uint clientHandle = 0;
      notificationHandle = 0U;
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) -1;
      AdsErrorCode adsErrorCode2;
      while (true)
      {
        adsErrorCode2 = this._symbolTable.TryCreateVariableHandle(symbolPath, this._timeout, out clientHandle);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode2))
        {
          adsErrorCode2 = this.TryAddDeviceNotificationEx(61445U, clientHandle, settings, userData, type, args, out notificationHandle);
          if (adsErrorCode2 == 1809 && !flag)
          {
            adsErrorCode1 = this._symbolTable.Resurrect(clientHandle);
            flag = true;
          }
          else
            break;
        }
        else
          break;
      }
      return adsErrorCode2;
    }

    /// <summary>
    /// Connects a variable to the ADS client asynchronously. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="symbolPath">The symbol/instance path of the ADS variable.</param>
    /// <param name="settings">The notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <param name="args">Additional arguments (for 'AnyType')</param>
    /// <param name="cancel">The Cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'AddDeviceNotification' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> type parameter contains the created handle
    /// (<see cref="P:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" /> should always
    /// be called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" />
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
      bool repeated = false;
      ResultHandle empty = ResultHandle.Empty;
      ResultHandle resultHandle;
      while (true)
      {
        uint handle = 0;
        ConfiguredTaskAwaitable<ResultHandle> configuredTaskAwaitable = this._symbolTable.CreateVariableHandleAsync(symbolPath, cancel).ConfigureAwait(false);
        resultHandle = await configuredTaskAwaitable;
        if (((ResultAds) resultHandle).Succeeded)
        {
          handle = resultHandle.Handle;
          configuredTaskAwaitable = this.AddDeviceNotificationExAsync(61445U, resultHandle.Handle, settings, userData, type, args, cancel).ConfigureAwait(false);
          resultHandle = await configuredTaskAwaitable;
          if (((ResultAds) resultHandle).ErrorCode == 1809 && !repeated)
          {
            ResultAds resultAds = await this._symbolTable.ResurrectAsync(handle, cancel).ConfigureAwait(false);
            repeated = true;
          }
          else
            break;
        }
        else
          break;
      }
      return resultHandle;
    }

    /// <summary>
    /// Connects a variable to the ADS client asynchronously. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> event.
    /// </summary>
    /// <param name="symbolPath">The symbol/instance path of the ADS variable.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="cancel">The Cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'AddDeviceNotification' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> type parameter contains the created handle
    /// (<see cref="P:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" /> should always
    /// be called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotification" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
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
      bool repeated = false;
      ResultHandle empty = ResultHandle.Empty;
      ResultHandle resultHandle;
      while (true)
      {
        uint handle = 0;
        resultHandle = await this._symbolTable.CreateVariableHandleAsync(symbolPath, cancel).ConfigureAwait(false);
        if (((ResultAds) resultHandle).Succeeded)
        {
          handle = resultHandle.Handle;
          resultHandle = await this.AddDeviceNotificationAsync(61445U, resultHandle.Handle, dataSize, settings, userData, cancel).ConfigureAwait(false);
          if (((ResultAds) resultHandle).ErrorCode == 1809 && !repeated)
          {
            ResultAds resultAds = await this._symbolTable.ResurrectAsync(handle, cancel).ConfigureAwait(false);
            repeated = true;
          }
          else
            break;
        }
        else
          break;
      }
      return resultHandle;
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
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotification" />
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> as value.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" /> should always
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
      uint handle = 0;
      this.TryAddDeviceNotification(indexGroup, indexOffset, dataSize, settings, userData, out handle).ThrowOnError();
      return handle;
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="symbolPath">Symbol/Instance path of the ADS variable.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <returns>The notification handle.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" />
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
      uint notificationHandle = 0;
      this.TryAddDeviceNotificationEx(symbolPath, settings, userData, type, (int[]) null, out notificationHandle).ThrowOnError();
      return notificationHandle;
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="symbolPath">Symbol/Instance path of the ADS variable.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <param name="args">Additional arguments (for 'AnyType')</param>
    /// <returns>The notification handle.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" />
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
      uint notificationHandle = 0;
      this.TryAddDeviceNotificationEx(symbolPath, settings, userData, type, args, out notificationHandle).ThrowOnError();
      return notificationHandle;
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="type">Type of the object stored in the event argument ('AnyType')</param>
    /// <returns>The notification handle.</returns>
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" />
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationError" />
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
      return this.AddDeviceNotificationEx(indexGroup, indexOffset, settings, userData, type, (int[]) null);
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
    /// <remarks>Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    public uint AddDeviceNotificationEx(
      uint indexGroup,
      uint indexOffset,
      NotificationSettings settings,
      object? userData,
      Type type,
      int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      uint handle = 0;
      this.TryAddDeviceNotificationEx(indexGroup, indexOffset, settings, userData, type, args, out handle).ThrowOnError();
      return handle;
    }

    /// <summary>Deletes a registered notification.</summary>
    /// <remarks>This is the complementary method to <see cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" /> overloads and should be called when the
    /// notification is not needed anymore the free TwinCAT realtime resources.</remarks>
    /// /// 
    ///             <param name="notificationHandle">Notification handle.</param>
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    public void DeleteDeviceNotification(uint notificationHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      this.TryDeleteDeviceNotification(notificationHandle).ThrowOnError();
    }

    /// <summary>Writes the state asynchronously</summary>
    /// <param name="adsState">State of the ads.</param>
    /// <param name="deviceState">State of the device.</param>
    /// <param name="writeData">The write buffer.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the
    /// <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultAds> WriteControlAsync(
      AdsState adsState,
      ushort deviceState,
      ReadOnlyMemory<byte> writeData,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      StateInfo info = new StateInfo(adsState, (short) deviceState);
      return this._server.RequestAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
        {
          result = this._interceptors.BeforeWriteState(info);
          if (AdsErrorCodeExtensions.Succeeded(result))
            result = this._interceptors.BeforeCommunicate();
        }
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.WriteControlRequestAsync(this._target, id, adsState, deviceState, writeData, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultAds>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, r);
        this._interceptors.AfterWriteState(info, r);
      }), this._timeout, cancel);
    }

    /// <summary>Write Control (synchronous)</summary>
    /// <param name="adsState">AdsState.</param>
    /// <param name="deviceState">DeviceState</param>
    /// <param name="writeData">Write data</param>
    /// <returns>ResultAds.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    private ResultAds WriteControlSync(
      AdsState adsState,
      ushort deviceState,
      ReadOnlyMemory<byte> writeData)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      StateInfo info = new StateInfo(adsState, (short) deviceState);
      return this._server.RequestAndReceiveSync((Func<uint, AdsErrorCode>) (id =>
      {
        AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
        if (this._interceptors != null)
        {
          adsErrorCode = this._interceptors.BeforeWriteState(info);
          if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
            adsErrorCode = this._interceptors.BeforeCommunicate();
        }
        return AdsErrorCodeExtensions.Succeeded(adsErrorCode) ? this._server.WriteControlRequestSync(this._target, id, adsState, deviceState, writeData.Span) : adsErrorCode;
      }), (Action<ResultAds>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, r);
        this._interceptors.AfterWriteState(info, r);
      }), this._timeout);
    }

    /// <summary>
    /// Closes this <see cref="T:TwinCAT.Ads.AdsClient" />
    /// </summary>
    public void Close() => this.Dispose();

    /// <summary>
    /// (Re)Connects the <see cref="T:TwinCAT.IConnection" /> when disconnected.
    /// </summary>
    /// <returns><c>true</c> if succeeded, <c>false</c> otherwise.</returns>
    bool IConnection.Connect()
    {
      if (AmsAddress.op_Equality(this._target, (AmsAddress) null))
        throw new ClientNotConnectedException();
      this.Connect(this._target);
      return true;
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
      StringMarshaler stringMarshaler = new StringMarshaler(encoding, (StringConvertMode) 0);
      byte[] array = new byte[stringMarshaler.MarshalSize(encoding, len)];
      int readBytes = 0;
      this.TryRead(indexGroup, indexOffset, array.AsMemory<byte>(), out readBytes).ThrowOnError();
      string str;
      stringMarshaler.Unmarshal((ReadOnlySpan<byte>) array.AsSpan<byte>(0, readBytes), encoding, ref str);
      return str;
    }

    /// <summary>
    /// Reads a string from a specified address asynchronously.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="len">The string length to be read.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
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
      StringMarshaler marshaler = new StringMarshaler(encoding, (StringConvertMode) 0);
      byte[] data = new byte[marshaler.MarshalSize(encoding, len)];
      ResultRead resultRead = await this.ReadAsync(indexGroup, indexOffset, data.AsMemory<byte>(), cancel).ConfigureAwait(false);
      ResultAnyValue resultAnyValue1;
      if (((ResultAds) resultRead).Succeeded)
      {
        string str;
        marshaler.Unmarshal((ReadOnlySpan<byte>) data.AsSpan<byte>(0, resultRead.ReadBytes), encoding, ref str);
        resultAnyValue1 = new ResultAnyValue(((ResultAds) resultRead).ErrorCode, (object) str, ((ResultAds) resultRead).InvokeId);
      }
      else
        resultAnyValue1 = new ResultAnyValue(((ResultAds) resultRead).ErrorCode, (object) null, ((ResultAds) resultRead).InvokeId);
      ResultAnyValue resultAnyValue2 = resultAnyValue1;
      marshaler = (StringMarshaler) null;
      data = (byte[]) null;
      return resultAnyValue2;
    }

    /// <summary>Reads a string from the specified symbol/variable.</summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="len">The length.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>The string value.</returns>
    public string ReadAnyString(uint variableHandle, int len, Encoding encoding)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      StringMarshaler stringMarshaler = new StringMarshaler(encoding, (StringConvertMode) 1);
      byte[] array = new byte[stringMarshaler.MarshalSize(encoding, len)];
      int readBytes = 0;
      this.TryRead(variableHandle, array.AsMemory<byte>(), out readBytes).ThrowOnError();
      string str;
      stringMarshaler.Unmarshal((ReadOnlySpan<byte>) array.AsSpan<byte>(0, readBytes), encoding, ref str);
      return str;
    }

    /// <summary>
    /// Reads a string asynchronously from the specified symbol/variable
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="len">The length.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read string
    /// (<see cref="P:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
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
      StringMarshaler conv = new StringMarshaler(encoding, (StringConvertMode) 1);
      byte[] data = new byte[conv.MarshalSize(encoding, len)];
      ResultRead resultRead = await this.ReadAsync(variableHandle, data.AsMemory<byte>(), cancel).ConfigureAwait(false);
      if (!((ResultAds) resultRead).Succeeded)
        return new ResultAnyValue(((ResultAds) resultRead).ErrorCode, (object) null, ((ResultAds) resultRead).InvokeId);
      string str;
      conv.Unmarshal((ReadOnlySpan<byte>) data.AsSpan<byte>(0, resultRead.ReadBytes), encoding, ref str);
      return new ResultAnyValue(((ResultAds) resultRead).ErrorCode, (object) str, ((ResultAds) resultRead).InvokeId);
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length.</param>
    /// <param name="encoding">The encoding.</param>
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!</remarks>
    /// <exclude />
    [Obsolete("This method is potentially unsafe!")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void WriteAnyString(
      uint indexGroup,
      uint indexOffset,
      string value,
      int length,
      Encoding encoding)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.Length > length)
        value = value.Substring(0, length);
      byte[] array = new byte[StringMarshaler.Default.MarshalSize(value)];
      StringMarshaler.Default.Marshal(value, array.AsSpan<byte>());
      this.TryWrite(indexGroup, indexOffset, (ReadOnlyMemory<byte>) array.AsMemory<byte>()).ThrowOnError();
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
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
    public Task<ResultWrite> WriteAnyStringAsync(
      uint indexGroup,
      uint indexOffset,
      string value,
      int length,
      Encoding encoding,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.Length > length)
        value = value.Substring(0, length);
      byte[] array = new byte[StringMarshaler.Default.MarshalSize(value)];
      StringMarshaler.Default.Marshal(value, array.AsSpan<byte>());
      return this.WriteAsync(indexGroup, indexOffset, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel);
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length of the string to write</param>
    /// <param name="encoding">The encoding.</param>
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!
    /// The String is written with the specified encoding.</remarks>
    /// <exclude />
    public void WriteAnyString(uint variableHandle, string value, int length, Encoding encoding)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.Length > length)
        value = value.Substring(0, length);
      StringMarshaler stringMarshaler = new StringMarshaler(encoding, (StringConvertMode) 1);
      int length1 = stringMarshaler.MarshalSize(value);
      byte[] array = new byte[length1];
      stringMarshaler.Marshal(value, length1, array.AsSpan<byte>());
      this.TryWrite(variableHandle, (ReadOnlyMemory<byte>) array.AsMemory<byte>()).ThrowOnError();
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length of the string to write</param>
    /// <param name="encoding">The encoding.</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!
    /// The String is written with the specified encoding.</remarks>
    public void WriteAnyString(string symbolPath, string value, int length, Encoding encoding)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.Length > length)
        value = value.Substring(0, length);
      StringMarshaler stringMarshaler = new StringMarshaler(encoding, (StringConvertMode) 1);
      int length1 = stringMarshaler.MarshalSize(value);
      byte[] array = new byte[length1];
      stringMarshaler.Marshal(value, length1, array.AsSpan<byte>());
      this.TryWriteValue<byte[]>(symbolPath, array).ThrowOnError();
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
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
    public Task<ResultWrite> WriteAnyStringAsync(
      uint variableHandle,
      string value,
      int length,
      Encoding encoding,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.Length > length)
        value = value.Substring(0, length);
      StringMarshaler stringMarshaler = new StringMarshaler(encoding, (StringConvertMode) 1);
      int length1 = stringMarshaler.MarshalSize(value);
      byte[] array = new byte[length1];
      stringMarshaler.Marshal(value, length1, array.AsSpan<byte>());
      return this.WriteAsync(variableHandle, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel);
    }

    /// <summary>Writes the string (Potentially unsafe!)</summary>
    /// <param name="symbolPath">The symbol path.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length of the string to write</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultWrite&gt;.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>ATTENTION: Potentially this method is unsafe because following data can be overwritten
    /// after the string symbol. Please be sure to specify the string length lower than the string size
    /// reserved within the process image!
    /// The String is written with the specified encoding.</remarks>
    public Task<ResultWrite> WriteAnyStringAsync(
      string symbolPath,
      string value,
      int length,
      Encoding encoding,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.Length > length)
        value = value.Substring(0, length);
      return this.WriteValueAsync<string>(symbolPath, value, cancel);
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
      byte[] array = new byte[this._anyTypeMarshaller.MarshalSize((object) type, this.DefaultValueEncoding)];
      int readBytes = 0;
      this.TryRead(61445U, variableHandle, array.AsMemory<byte>(), out readBytes).ThrowOnError();
      object obj;
      this._anyTypeMarshaller.Unmarshal(type, (int[]) null, (ReadOnlySpan<byte>) array.AsSpan<byte>(), this.DefaultValueEncoding, out obj);
      return obj != null ? obj : throw new MarshalException();
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T">The type of the value to read.</typeparam>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <returns>The value of the read symbol.</returns>
    public T ReadAny<T>(uint variableHandle) => (T) this.ReadAny(variableHandle, typeof (T));

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
      ResultAnyValue resultAnyValue = await this.ReadAnyAsync(variableHandle, typeof (T), cancel).ConfigureAwait(false);
      return new ResultValue<T>(((ResultAds) resultAnyValue).ErrorCode, (T) ((ResultValue<object>) resultAnyValue).Value);
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
      ResultAnyValue resultAnyValue = await this.ReadAnyAsync(variableHandle, typeof (T), args, cancel).ConfigureAwait(false);
      return new ResultValue<T>(((ResultAds) resultAnyValue).ErrorCode, (T) ((ResultValue<object>) resultAnyValue).Value);
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
      ResultAnyValue resultAnyValue = await this.ReadAnyAsync(indexGroup, indexOffset, typeof (T), cancel).ConfigureAwait(false);
      return new ResultValue<T>(((ResultAds) resultAnyValue).ErrorCode, (T) ((ResultValue<object>) resultAnyValue).Value);
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
      ResultAnyValue resultAnyValue = await this.ReadAnyAsync(indexGroup, indexOffset, typeof (T), args, cancel).ConfigureAwait(false);
      return new ResultValue<T>(((ResultAds) resultAnyValue).ErrorCode, (T) ((ResultValue<object>) resultAnyValue).Value);
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
      byte[] array = new byte[this._anyTypeMarshaller.MarshalSize(type, args, this.DefaultValueEncoding)];
      int readBytes = 0;
      this.TryRead(61445U, variableHandle, array.AsMemory<byte>(), out readBytes).ThrowOnError();
      object obj = (object) null;
      this._anyTypeMarshaller.Unmarshal(type, args, (ReadOnlySpan<byte>) array.AsSpan<byte>(), StringMarshaler.DefaultEncoding, out obj);
      return obj != null ? obj : throw new MarshalException();
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
    public T ReadAny<T>(uint indexGroup, uint indexOffset, int[]? args) => (T) this.ReadAny(indexGroup, indexOffset, typeof (T), args);

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
    public T ReadAny<T>(uint variableHandle, int[]? args) => (T) this.ReadAny(variableHandle, typeof (T), args);

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object to be read.</typeparam>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <returns>The read value.</returns>
    public T ReadAny<T>(uint indexGroup, uint indexOffset) => (T) this.ReadAny(indexGroup, indexOffset, typeof (T), (int[]) null);

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
      byte[] array = new byte[this._anyTypeMarshaller.MarshalSize((object) type, this.DefaultValueEncoding)];
      int readBytes = 0;
      this.TryRead(indexGroup, indexOffset, array.AsMemory<byte>(), out readBytes).ThrowOnError();
      object obj = (object) null;
      this._anyTypeMarshaller.Unmarshal(type, (int[]) null, (ReadOnlySpan<byte>) array.AsSpan<byte>(), StringMarshaler.DefaultEncoding, out obj);
      return obj != null ? obj : throw new MarshalException();
    }

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to an object.
    /// </summary>
    /// <param name="indexGroup">Index group of the ADS variable.</param>
    /// <param name="indexOffset">Index offset of the ADS variable.</param>
    /// <param name="type">Type of the object to be read.</param>
    /// <param name="args">Additional arguments.</param>
    /// <returns>The read value.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
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
    public object ReadAny(uint indexGroup, uint indexOffset, Type type, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (args == null)
        return this.ReadAny(indexGroup, indexOffset, type);
      byte[] array = new byte[this._anyTypeMarshaller.MarshalSize(type, args, StringMarshaler.DefaultEncoding)];
      int readBytes = 0;
      this.TryRead(indexGroup, indexOffset, array.AsMemory<byte>(), out readBytes).ThrowOnError();
      object obj;
      this._anyTypeMarshaller.Unmarshal(type, args, (ReadOnlySpan<byte>) array.AsSpan<byte>(), StringMarshaler.DefaultEncoding, out obj);
      return obj;
    }

    /// <summary>Writes an object synchronously to an ADS device.</summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    public void WriteAny(uint variableHandle, object value) => this.WriteAny(variableHandle, value, (int[]) null);

    /// <summary>Writes an object synchronously to an ADS device.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    public void WriteAny(uint indexGroup, uint indexOffset, object value) => this.WriteAny(indexGroup, indexOffset, value, (int[]) null);

    /// <summary>
    /// Writes an object synchronously to an ADS device.
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="args">Additional arguments.</param>
    /// <remarks>
    /// <list type="table">
    /// <listheader><term>Type of value Parameter</term><description>Necessary Arguments (args)</description></listheader>
    /// <item><term>string</term><description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description></item>
    /// <item><term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    /// <item><term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public void WriteAny(uint variableHandle, object value, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      byte[] array = new byte[this._anyTypeMarshaller.MarshalSize(value, args, StringMarshaler.DefaultEncoding)];
      this._anyTypeMarshaller.Marshal(value, args, StringMarshaler.DefaultEncoding, array.AsSpan<byte>());
      this.TryWrite(variableHandle, (ReadOnlyMemory<byte>) array.AsMemory<byte>()).ThrowOnError();
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
    /// </list>
    /// </remarks>
    /// <returns>A task that represents the asynchronous task operation. The result parameter <see cref="T:TwinCAT.Ads.ResultWrite" /> of the write operation contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" />.
    /// </returns>
    public Task<ResultWrite> WriteAnyAsync(
      uint variableHandle,
      object value,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      byte[] array = new byte[this._anyTypeMarshaller.MarshalSize(value, args, this.DefaultValueEncoding)];
      this._anyTypeMarshaller.Marshal(value, args, this.DefaultValueEncoding, array.AsSpan<byte>());
      return this.WriteAsync(variableHandle, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel);
    }

    /// <summary>
    /// Writes an object synchronously to an ADS device.
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous task operation. The result parameter <see cref="T:TwinCAT.Ads.ResultWrite" /> of the write operation contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" />.
    /// </returns>
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

    /// <summary>Writes an object synchronously to an ADS device.</summary>
    /// <remarks>
    /// If the Type of the object to be written is a string type, the first element of parameter args
    /// specifies the number of characters of the string.
    /// </remarks>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="value">Object to write to the ADS device.</param>
    /// <param name="args">Additional arguments.</param>
    /// <remarks>
    /// <list type="table">
    /// <listheader><term>Type of value Parameter</term><description>Necessary Arguments (args)</description></listheader>
    /// <item><term>string</term><description>args[0]: Number of characters in the string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</description></item>
    /// <item><term>string[]</term>args[0]: Number of characters in each string typed as <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />.</item>
    /// <item><term>Array</term>args: Dimensions of Array as int[]</item>
    /// string :
    /// string[] :
    /// Array : args Dimensions of Array
    /// </list>
    /// </remarks>
    public void WriteAny(uint indexGroup, uint indexOffset, object value, int[]? args)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      byte[] array = new byte[this._anyTypeMarshaller.MarshalSize(value, args, StringMarshaler.DefaultEncoding)];
      this._anyTypeMarshaller.Marshal(value, args, StringMarshaler.DefaultEncoding, array.AsSpan<byte>());
      this.TryWrite(indexGroup, indexOffset, (ReadOnlyMemory<byte>) array.AsMemory<byte>()).ThrowOnError();
    }

    /// <summary>
    /// Determines the Symbol handle by its instance path synchronously.
    /// </summary>
    /// <param name="symbolPath">SymbolName / InstancePath.</param>
    /// <returns>The symbols/variable handle</returns>
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandle(System.String)" /> is the <see cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandle(System.UInt32)" /></remarks>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandle(System.UInt32)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandleAsync(System.String,System.Threading.CancellationToken)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryCreateVariableHandle(System.String,System.UInt32@)" />
    public uint CreateVariableHandle(string symbolPath)
    {
      uint variableHandle = 0;
      this.TryCreateVariableHandle(symbolPath, out variableHandle).ThrowOnError();
      return variableHandle;
    }

    /// <summary>
    /// Releases the specified symbol/variable handle synchronously.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    /// <returns>The ADS error code.</returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandle(System.String)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryDeleteVariableHandle(System.UInt32)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.AdsClient.TryDeleteVariableHandle(System.UInt32)" /> is the <see cref="M:TwinCAT.Ads.AdsClient.TryCreateVariableHandle(System.String,System.UInt32@)" /></remarks>
    public void DeleteVariableHandle(uint variableHandle) => this.TryDeleteVariableHandle(variableHandle).ThrowOnError();

    /// <summary>
    /// Reads data synchronously from an ADS device and writes to the specified <paramref name="readBuffer" />.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    /// <param name="readBuffer">The read buffer / data</param>
    /// <returns>Number of successfully returned data bytes.</returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryRead(System.UInt32,System.Memory{System.Byte},System.Int32@)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.ReadAsync(System.UInt32,System.Memory{System.Byte},System.Threading.CancellationToken)" />
    public int Read(uint variableHandle, Memory<byte> readBuffer)
    {
      int readBytes = 0;
      this.TryRead(variableHandle, readBuffer, out readBytes).ThrowOnError();
      return readBytes;
    }

    /// <summary>
    /// Reads the value synchronously data of the symbol, that is represented by the variable handle into the <paramref name="readBuffer" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="readBuffer">The read buffer/data</param>
    /// <param name="readBytes">Number of read bytes.</param>
    /// <returns>The ADS error code.</returns>
    public AdsErrorCode TryRead(
      uint variableHandle,
      Memory<byte> readBuffer,
      out int readBytes)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this.TryRead(61445U, variableHandle, readBuffer, out readBytes);
    }

    /// <summary>
    /// Writes data synchronously to an ADS device and then Reads data from that target.
    /// </summary>
    /// <param name="variableHandle">Variable handle.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>Number of successfully returned data bytes.</returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryReadWrite(System.UInt32,System.Memory{System.Byte},System.ReadOnlyMemory{System.Byte},System.Int32@)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.ReadWriteAsync(System.UInt32,System.Memory{System.Byte},System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" />
    public int ReadWrite(
      uint variableHandle,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      int readBytes = 0;
      this.TryReadWrite(variableHandle, readBuffer, writeBuffer, out readBytes).ThrowOnError();
      return readBytes;
    }

    /// <summary>
    /// Determines the Symbol handle by its instance path asynchronously.
    /// </summary>
    /// <param name="symbolPath">SymbolName / InstancePath.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'CreateVariableHandle' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> parameter contains the variable handle
    /// (<see cref="P:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryCreateVariableHandle(System.String,System.UInt32@)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandle(System.String)" />
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandleAsync(System.String,System.Threading.CancellationToken)" /> is the <see cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" /></remarks>
    public Task<ResultHandle> CreateVariableHandleAsync(
      string symbolPath,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._symbolTable.CreateVariableHandleAsync(symbolPath, cancel);
    }

    /// <summary>
    /// Releases the specified symbol/variable handle asynchronously.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'DeleteVariableHandle' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandleAsync(System.String,System.Threading.CancellationToken)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryDeleteVariableHandle(System.UInt32)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandle(System.UInt32)" />
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" /> is the <see cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandleAsync(System.String,System.Threading.CancellationToken)" /></remarks>
    public Task<ResultAds> DeleteVariableHandleAsync(
      uint variableHandle,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._symbolTable.DeleteVariableHandleAsync(variableHandle, cancel);
    }

    /// <summary>
    /// Releases the specified symbol/variable handle synchronously.
    /// </summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    /// <returns>The ADS error code.</returns>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryCreateVariableHandle(System.String,System.UInt32@)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandleAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteVariableHandle(System.UInt32)" />
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.AdsClient.TryDeleteVariableHandle(System.UInt32)" /> is the <see cref="M:TwinCAT.Ads.AdsClient.TryCreateVariableHandle(System.String,System.UInt32@)" /></remarks>
    public AdsErrorCode TryDeleteVariableHandle(uint variableHandle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._symbolTable.TryDeleteVariableHandle(variableHandle, this._timeout);
    }

    /// <summary>
    /// ReadWrites value data synchronously to/from the symbol represented by the <paramref name="variableHandle" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="readBuffer">The read buffer / read data.</param>
    /// <param name="writeBuffer">The write buffer / write data.</param>
    /// <param name="readBytes">Number of read bytes.</param>
    /// <returns>The ADS error code.</returns>
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
      return this.TryReadWrite(61445U, variableHandle, readBuffer, writeBuffer, out readBytes);
    }

    /// <summary>Writes data synchronously to an ADS device.</summary>
    /// <param name="variableHandle">Handle of the ADS variable</param>
    /// <param name="writeBuffer">The write buffer / value to be written</param>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryWrite(System.UInt32,System.ReadOnlyMemory{System.Byte})" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.WriteAsync(System.UInt32,System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" />
    public void Write(uint variableHandle, ReadOnlyMemory<byte> writeBuffer) => this.TryWrite(variableHandle, writeBuffer).ThrowOnError();

    /// <summary>
    /// Reads data synchronously from an ADS device and writes it to the given <paramref name="readBuffer" />
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">Memory location, where to read the data.</param>
    /// <returns>Number of successfully returned (read) data bytes.</returns>
    public int Read(uint indexGroup, uint indexOffset, Memory<byte> readBuffer)
    {
      int readBytes = 0;
      this.TryRead(indexGroup, indexOffset, readBuffer, out readBytes).ThrowOnError();
      return readBytes;
    }

    /// <summary>
    /// Triggers a 'Write' call to the ADS device at the specified address.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    public void Write(uint indexGroup, uint indexOffset) => this.TryWrite(indexGroup, indexOffset, (ReadOnlyMemory<byte>) Memory<byte>.Empty).ThrowOnError();

    /// <summary>Writes data synchronously to an ADS device.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="writeBuffer">The data to write.</param>
    public void Write(uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeBuffer) => this.TryWrite(indexGroup, indexOffset, writeBuffer).ThrowOnError();

    /// <summary>Writes data synchronously to an ADS device.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="writeBuffer">The data buffer to be written.</param>
    /// <returns>The ADS error code.</returns>
    public AdsErrorCode TryWrite(
      uint indexGroup,
      uint indexOffset,
      ReadOnlyMemory<byte> writeBuffer)
    {
      return ((ResultAds) this.WriteSync(indexGroup, indexOffset, writeBuffer)).ErrorCode;
    }

    /// <summary>
    /// Reads the ADS status and the device status from an ADS server.
    /// </summary>
    /// <returns>The ADS statue and device status.</returns>
    /// <remarks>Not all ADS Servers support the State ADS Request.</remarks>
    public StateInfo ReadState()
    {
      StateInfo stateInfo;
      this.TryReadState(out stateInfo).ThrowOnError();
      return stateInfo;
    }

    /// <summary>
    /// Reads the ADS status and the device status from an ADS server. Unlike the ReadState method this method does not call an exception on failure. Instead an AdsErrorCode is returned.
    /// If the return value is equal to AdsErrorCode.NoError the call was successful.
    /// </summary>
    /// <param name="stateInfo">The ADS statue and device status.</param>
    /// <returns><see cref="T:TwinCAT.Ads.AdsErrorCode" /> of the ADS read state call. Check for <see cref="F:TwinCAT.Ads.AdsErrorCode.NoError" /> to see if call was successful.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>Not all ADS Servers support the State ADS Request</remarks>
    public AdsErrorCode TryReadState(out StateInfo stateInfo)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultReadDeviceState resultReadDeviceState = this.ReadStateSync();
      stateInfo = resultReadDeviceState.State;
      return ((ResultAds) resultReadDeviceState).ErrorCode;
    }

    /// <summary>
    /// Reads the ADS status and the device status from an ADS server.
    /// </summary>
    /// <param name="cancel">The cancellation token</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultReadDeviceState" /> parameter contains the state
    /// (<see cref="P:TwinCAT.Ads.ResultReadDeviceState.State" />) as long as the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <remarks>Not all ADS Servers support the State ADS Request</remarks>
    public Task<ResultReadDeviceState> ReadStateAsync(
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        return Task.FromResult<ResultReadDeviceState>(new ResultReadDeviceState((AdsErrorCode) 1864, new StateInfo(), 0U));
      Func<CancellationToken, Task<ResultReadDeviceState>> action = (Func<CancellationToken, Task<ResultReadDeviceState>>) (c => this._server.RequestReadDeviceStateAsync((Func<uint, Task<AdsErrorCode>>) (id => this._server.ReadDeviceStateRequestAsync(this._target, id, c)), (Action<ResultReadDeviceState>) null, this._timeout, c));
      if (cancel.IsCancellationRequested)
        return Task.FromResult<ResultReadDeviceState>(ResultReadDeviceState.CreateError((AdsErrorCode) 1878, 0U));
      return this._interceptors != null ? this._interceptors.CommunicateReadStateAsync(action, cancel) : action(cancel);
    }

    /// <summary>
    /// Reads the ADS status and the device status from an ADS server (synchronous)
    /// </summary>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultReadDeviceState" /> parameter contains the state
    /// (<see cref="P:TwinCAT.Ads.ResultReadDeviceState.State" />) as long as the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    private ResultReadDeviceState ReadStateSync()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        return new ResultReadDeviceState((AdsErrorCode) 1864, new StateInfo(), 0U);
      Func<ResultReadDeviceState> action = (Func<ResultReadDeviceState>) (() => this._server.RequestReadDeviceState((Func<uint, AdsErrorCode>) (id => this._server.ReadDeviceStateRequestSync(this._target, id)), (Action<ResultReadDeviceState>) null, this._timeout));
      return this._interceptors != null ? this._interceptors.CommunicateReadState(action) : action();
    }

    /// <summary>
    /// Changes the ADS status and device status of the ADS server asynchronously.
    /// </summary>
    /// <param name="adsState">The ADS state.</param>
    /// <param name="deviceState">The device state.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'WriteControl' operation. The <see cref="T:TwinCAT.Ads.ResultAds" /> parameter contains the state
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultAds> WriteControlAsync(
      AdsState adsState,
      ushort deviceState,
      CancellationToken cancel)
    {
      byte[] array = new byte[1];
      return this.WriteControlAsync(adsState, deviceState, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel);
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    public void WriteControl(StateInfo stateInfo)
    {
      byte[] writeBuffer = new byte[1];
      this.TryWriteControl(stateInfo, (ReadOnlyMemory<byte>) writeBuffer).ThrowOnError();
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    public void WriteControl(StateInfo stateInfo, ReadOnlyMemory<byte> writeBuffer) => this.TryWriteControl(stateInfo, writeBuffer).ThrowOnError();

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteControl(StateInfo stateInfo) => this.TryWriteControl(stateInfo, (ReadOnlyMemory<byte>) new byte[1].AsMemory<byte>());

    /// <summary>
    /// Determines the Symbol handle by its instance path synchronously.
    /// </summary>
    /// <param name="symbolPath">SymbolName / InstancePath.</param>
    /// <param name="variableHandle">The symbols handle.</param>
    /// <returns>The ADS error code.</returns>
    /// <remarks>It is a good practice to release all variable handles after use to regain internal resources in the TwinCAT subsystem. The composite method to this
    /// <see cref="M:TwinCAT.Ads.AdsClient.TryCreateVariableHandle(System.String,System.UInt32@)" /> is the <see cref="M:TwinCAT.Ads.AdsClient.TryDeleteVariableHandle(System.UInt32)" /></remarks>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.TryDeleteVariableHandle(System.UInt32)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandleAsync(System.String,System.Threading.CancellationToken)" />
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.CreateVariableHandle(System.String)" />
    public AdsErrorCode TryCreateVariableHandle(
      string symbolPath,
      out uint variableHandle)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._symbolTable.TryCreateVariableHandle(symbolPath, this._timeout, out variableHandle);
    }

    /// <summary>
    /// Called when before the <see cref="T:TwinCAT.Ads.AdsClient" /> is disconnected.
    /// </summary>
    private void OnBeforeDisconnect()
    {
      if (this._interceptors == null)
        return;
      this._interceptors.BeforeDisconnect((Func<AdsErrorCode>) (() => (AdsErrorCode) 0));
    }

    /// <summary>
    /// Sets additional <see cref="T:TwinCAT.Ads.CommunicationInterceptor">Communication Interceptors.</see>.
    /// </summary>
    /// <param name="interceptors">The interceptors.</param>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exclude />
    public void SetCommunicationInterceptor(CommunicationInterceptors interceptors)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      this._interceptors = interceptors;
    }

    /// <summary>
    /// Injects an <see cref="T:TwinCAT.Ads.AdsErrorCode" /> to the <see cref="T:TwinCAT.Ads.Internal.IInterceptedClient" />.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>The accepted <see cref="T:TwinCAT.Ads.AdsErrorCode" />.</returns>
    AdsErrorCode IAdsInjectAcceptor.InjectError(AdsErrorCode error)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      if (this._interceptors != null)
        adsErrorCode = this._interceptors.Communicate(false, (Func<bool, ResultAds>) (resurrect => ResultAds.CreateError(error)));
      return adsErrorCode;
    }

    public bool TryResurrect([NotNullWhen(false)] out AdsException? error)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      AdsErrorCode errorCode = ((IAdsResurrectHandles) this._symbolTable).Resurrect();
      if (AdsErrorCodeExtensions.Succeeded(errorCode))
      {
        errorCode = ((IAdsResurrectHandles) this._notificationReceiver).Resurrect();
        if (AdsErrorCodeExtensions.Succeeded(errorCode))
        {
          IFailFastHandler failFastHandler = (IFailFastHandler) null;
          if (this._interceptors != null)
            failFastHandler = (IFailFastHandler) this._interceptors.CombinedInterceptors.FirstOrDefault<ICommunicationInterceptor>((Func<ICommunicationInterceptor, bool>) (item => item is IFailFastHandler));
          if (failFastHandler != null)
          {
            failFastHandler.Reset();
            error = (AdsException) null;
          }
          if (this._interceptors != null)
            this._interceptors.Communicate(true, (Func<bool, ResultAds>) (resurrect => ResultAds.CreateSuccess()));
          error = (AdsException) null;
          return true;
        }
      }
      error = (AdsException) new AdsErrorException("Cannot resurrect", errorCode);
      return false;
    }

    /// <summary>Gets the access method for the specified symbol.</summary>
    /// <param name="symbol">The symbol.</param>
    private AdsClient.AccessMethods getAccessMethod(ISymbol symbol)
    {
      AdsClient.AccessMethods accessMethod = AdsClient.AccessMethods.Mask_All;
      if (!(symbol is IAdsSymbol iadsSymbol))
        accessMethod = AdsClient.AccessMethods.Mask_Symbolic;
      else if (((IProcessImageAddress) iadsSymbol).IndexGroup == 61462U || ((IProcessImageAddress) iadsSymbol).IndexGroup == 61467U)
        accessMethod = AdsClient.AccessMethods.Mask_Symbolic;
      else if (((IProcessImageAddress) iadsSymbol).IndexGroup == 61460U || ((IProcessImageAddress) iadsSymbol).IndexGroup == 61466U)
        accessMethod = AdsClient.AccessMethods.Mask_Symbolic;
      else if (((IProcessImageAddress) iadsSymbol).IndexGroup == 61465U)
        accessMethod = AdsClient.AccessMethods.Mask_Symbolic;
      return accessMethod;
    }

    /// <summary>
    /// Reads the value of a symbol asynchronously and returns it as an object. Strings and all primitive data types (UInt32, Int32, Bool etc.) are supported.
    /// Arrays and structures cannot be read.
    /// </summary>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> as customized task object.</returns>
    public async Task<ResultAnyValue> ReadValueAsync(
      ISymbol symbol,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (((IProcessImageAddress) symbol).IsVirtual)
        return new ResultAnyValue((AdsErrorCode) 1796, (object) null, 0U);
      IDataType dataType = ((IInstance) symbol).DataType;
      if (dataType == null)
        throw new CannotResolveDataTypeException((IInstance) symbol);
      bool flag1 = (this.getAccessMethod(symbol) & AdsClient.AccessMethods.ValueByHandle) > AdsClient.AccessMethods.None;
      bool flag2 = (this.getAccessMethod(symbol) & AdsClient.AccessMethods.ValueByName) > AdsClient.AccessMethods.None;
      PrimitiveTypeMarshaler converter = PrimitiveTypeMarshaler.CreateFrom(dataType);
      Type managedType;
      if (!PrimitiveTypeMarshaler.TryGetManagedType(dataType, out managedType))
        managedType = typeof (byte[]);
      ResultAnyValue resultAnyValue;
      if (flag2)
      {
        resultAnyValue = await this.ReadValueAsync(((IInstance) symbol).InstancePath, managedType, cancel).ConfigureAwait(false);
      }
      else
      {
        IAdsSymbol iadsSymbol = (IAdsSymbol) symbol;
        byte[] buffer = new byte[((IBitSize) symbol).ByteSize];
        ResultRead resultRead = await this.ReadAsync(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, buffer.AsMemory<byte>(), cancel).ConfigureAwait(false);
        object val;
        if (symbol.Category == 4)
        {
          IList<IDimensionCollection> dimLengths = (IList<IDimensionCollection>) null;
          AnyArrayConverter.TryGetJaggedDimensions((IArrayType) dataType, out dimLengths);
          converter.UnmarshalArray(new AnyTypeSpecifier(managedType, dimLengths), this.DefaultValueEncoding, (ReadOnlySpan<byte>) buffer, out val);
        }
        else
          converter.Unmarshal(managedType, (ReadOnlySpan<byte>) buffer, StringMarshaler.DefaultEncoding, out val);
        resultAnyValue = new ResultAnyValue(((ResultAds) resultRead).ErrorCode, val, ((ResultAds) resultRead).InvokeId);
        buffer = (byte[]) null;
      }
      return resultAnyValue;
    }

    /// <summary>
    /// Reads the value of a symbol and returns it as an object.
    /// </summary>
    /// <remarks>
    /// Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'. Structs are not supported.
    /// </remarks>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <returns>The value of the symbol as an object.</returns>
    public object ReadValue(ISymbol symbol)
    {
      object obj;
      this.TryReadValue(symbol, out obj).ThrowOnError();
      return obj;
    }

    /// <summary>
    /// Reads the value of a symbol and returns it as an object.
    /// </summary>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <param name="value">The value.</param>
    /// <returns>The ADS Error Code</returns>
    /// <remarks>Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'. Structs are not supported.</remarks>
    public AdsErrorCode TryReadValue(ISymbol symbol, out object? value)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (((IProcessImageAddress) symbol).IsVirtual)
      {
        value = (object) null;
        return (AdsErrorCode) 1796;
      }
      IDataType dataType = ((IInstance) symbol).DataType;
      if (dataType == null)
        throw new CannotResolveDataTypeException((IInstance) symbol);
      bool flag1 = (this.getAccessMethod(symbol) & AdsClient.AccessMethods.ValueByHandle) > AdsClient.AccessMethods.None;
      bool flag2 = (this.getAccessMethod(symbol) & AdsClient.AccessMethods.ValueByName) > AdsClient.AccessMethods.None;
      Type managed;
      if (!PrimitiveTypeMarshaler.TryGetManagedType(dataType, out managed))
        managed = typeof (byte[]);
      PrimitiveTypeMarshaler from = PrimitiveTypeMarshaler.CreateFrom(dataType);
      AdsErrorCode adsErrorCode;
      if (flag2)
      {
        adsErrorCode = this.TryReadValue(((IInstance) symbol).InstancePath, managed, out value);
      }
      else
      {
        IAdsSymbol iadsSymbol = (IAdsSymbol) symbol;
        int readBytes = 0;
        byte[] numArray = new byte[((IBitSize) symbol).ByteSize];
        adsErrorCode = this.TryRead(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, numArray.AsMemory<byte>(), out readBytes);
        if (symbol.Category == 4)
        {
          IList<IDimensionCollection> dimLengths = (IList<IDimensionCollection>) null;
          AnyArrayConverter.TryGetJaggedDimensions((IArrayType) dataType, out dimLengths);
          AnyTypeSpecifier typeSpec = new AnyTypeSpecifier(managed, dimLengths);
          from.UnmarshalArray(typeSpec, this.DefaultValueEncoding, (ReadOnlySpan<byte>) numArray, out value);
        }
        else
          from.Unmarshal(managed, (ReadOnlySpan<byte>) numArray, StringMarshaler.DefaultEncoding, out value);
      }
      return adsErrorCode;
    }

    /// <summary>Clears the internal symbol cache.</summary>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <remarks>Previously stored symbol information is cleared. As a consequence the symbol information must be obtained from the ADS server again if accessed, which
    /// which needs an extra ADS round trip.</remarks>
    public void CleanupSymbolTable()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ISymbolInfoTable isymbolInfoTable = (ISymbolInfoTable) null;
      if (!AdsErrorCodeExtensions.Succeeded(((IAdsSymbolTableProvider) this).TryGetSymbolTable(ref isymbolInfoTable)))
        return;
      isymbolInfoTable.CleanupCache();
    }

    /// <summary>
    /// Reads the value of a symbol and returns the value as object. The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="type">Managed type of the ADS symbol.</param>
    /// <returns>Value of the symbol</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    public object ReadValue(string name, Type type)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      object obj = (object) null;
      this.TryReadValue(name, type, out obj).ThrowOnError();
      return obj;
    }

    /// <summary>
    /// Reads the value of a symbol and returns the value as object.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="type">Managed type of the ADS symbol.</param>
    /// <param name="value">The read value of the Symbol.</param>
    /// <returns>The <see cref="T:TwinCAT.Ads.AdsErrorCode" />.</returns>
    /// <remarks>The parameter type must have the same layout as the ADS symbol.</remarks>
    public AdsErrorCode TryReadValue(string name, Type type, out object? value)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ISymbolInfoTable isymbolInfoTable = (ISymbolInfoTable) null;
      AdsErrorCode adsErrorCode = ((IAdsSymbolTableProvider) this).TryGetSymbolTable(ref isymbolInfoTable);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
        adsErrorCode = isymbolInfoTable.TryReadValue(name, type, ref value);
      else
        value = (object) null;
      return adsErrorCode;
    }

    /// <summary>Reads the value of a symbol asynchronously.</summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="type">Managed type of the ADS symbol.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="F:TwinCAT.Ads.ResultAnyValue.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <remarks>The parameter type must have the same layout as the ADS symbol.</remarks>
    public async Task<ResultAnyValue> ReadValueAsync(
      string name,
      Type type,
      CancellationToken cancel)
    {
      AdsClient adsClient = this;
      if (adsClient._disposed)
        throw new ObjectDisposedException(adsClient.Name);
      if (!adsClient.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<ISymbolInfoTable> resultValue = await ((IAdsSymbolTableProvider) adsClient).GetSymbolTableAsync(cancel).ConfigureAwait(false);
      return ((ResultAds) resultValue).Succeeded ? await resultValue.Value.ReadValueAsync(name, type, cancel).ConfigureAwait(false) : new ResultAnyValue(((ResultAds) resultValue).ErrorCode, (object) null, ((ResultAds) resultValue).InvokeId);
    }

    AdsErrorCode IAdsSymbolTableProvider.TryGetSymbolTable(
      out ISymbolInfoTable? table)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      AdsErrorCode symbolTable = (AdsErrorCode) 0;
      if (this._symbolInfoTable == null)
      {
        SymbolUploadInfo symbolInfo = (SymbolUploadInfo) null;
        symbolTable = SymbolLoaderFactory.TryReadSymbolUploadInfo((IAdsConnection) this, out symbolInfo);
        if (AdsErrorCodeExtensions.Succeeded(symbolTable))
        {
          this.SetSymbolEncoding(symbolInfo.StringEncoding);
          this._symbolInfoTable = (ISymbolInfoTable) new SymbolInfoTable((IAdsConnection) this, SymbolLoaderFactory.createValueAccessor((IAdsConnection) this, new SymbolLoaderSettings((SymbolsLoadMode) 0, ValueAccessMode.IndexGroupOffset)), symbolInfo);
        }
      }
      table = this._symbolInfoTable;
      return symbolTable;
    }

    /// <summary>Gets the symbol table asynchronously.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>SymbolInfoTable.</returns>
    async Task<ResultValue<ISymbolInfoTable>> IAdsSymbolTableProvider.GetSymbolTableAsync(
      CancellationToken cancel)
    {
      AdsClient connection = this;
      if (connection._disposed)
        throw new ObjectDisposedException(connection.Name);
      if (!connection.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<ISymbolInfoTable> symbolTableAsync;
      if (connection._symbolInfoTable == null)
      {
        ResultValue<SymbolUploadInfo> resultValue = await SymbolLoaderFactory.readSymbolUploadInfoAsync((IAdsConnection) connection, cancel).ConfigureAwait(false);
        if (((ResultAds) resultValue).Succeeded)
        {
          SymbolUploadInfo uploadInfo = resultValue.Value;
          SymbolLoaderSettings settings = new SymbolLoaderSettings((SymbolsLoadMode) 0, ValueAccessMode.IndexGroupOffset);
          connection._symbolInfoTable = (ISymbolInfoTable) new SymbolInfoTable((IAdsConnection) connection, SymbolLoaderFactory.createValueAccessor((IAdsConnection) connection, settings), uploadInfo);
          symbolTableAsync = new ResultValue<ISymbolInfoTable>((AdsErrorCode) 0, connection._symbolInfoTable);
        }
        else
          symbolTableAsync = new ResultValue<ISymbolInfoTable>(((ResultAds) resultValue).ErrorCode, (ISymbolInfoTable) null);
      }
      else
        symbolTableAsync = new ResultValue<ISymbolInfoTable>((AdsErrorCode) 0, connection._symbolInfoTable);
      return symbolTableAsync;
    }

    /// <summary>
    /// Call this method to obtain information about the individual symbols (variables) in ADS devices.
    /// </summary>
    /// <param name="name">Name of the symbol.</param>
    /// <returns>A IAdsSymbol2 containing the requested symbol information or null if symbol could not
    /// be found.</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException">Thrown when the ADS call fails.</exception>
    public IAdsSymbol ReadSymbol(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      IAdsSymbol symbol = (IAdsSymbol) null;
      this.TryReadSymbol(name, out symbol).ThrowOnError();
      return symbol;
    }

    /// <summary>
    /// Call this method to obtain information about the individual symbols (variables) in ADS devices.
    /// </summary>
    /// <param name="name">Name of the symbol.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous 'ReadSymbolInfo' operation. The <see cref="T:TwinCAT.Ads.ResultValue`1" /> parameter contains the read value
    /// (<see cref="F:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public async Task<ResultValue<IAdsSymbol>> ReadSymbolAsync(
      string name,
      CancellationToken cancel)
    {
      AdsClient adsClient = this;
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (adsClient.IsDisposed)
        throw new ObjectDisposedException(adsClient.Name);
      if (!adsClient.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<ISymbolInfoTable> resultValue = await ((IAdsSymbolTableProvider) adsClient).GetSymbolTableAsync(cancel).ConfigureAwait(false);
      return ((ResultAds) resultValue).Succeeded ? await resultValue.Value.ReadSymbolAsync(name, true, cancel).ConfigureAwait(false) : new ResultValue<IAdsSymbol>(((ResultAds) resultValue).ErrorCode, (IAdsSymbol) null);
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
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      IDataType dataType = (IDataType) null;
      this.TryReadDataType(typeName, out dataType).ThrowOnError();
      return dataType;
    }

    public AdsErrorCode TryReadDataType(string typeName, out IDataType? dataType)
    {
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentOutOfRangeException(nameof (typeName));
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ISymbolInfoTable isymbolInfoTable = (ISymbolInfoTable) null;
      dataType = (IDataType) null;
      AdsErrorCode adsErrorCode = ((IAdsSymbolTableProvider) this).TryGetSymbolTable(ref isymbolInfoTable);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
        adsErrorCode = isymbolInfoTable.TryReadType(typeName, true, ref dataType);
      return adsErrorCode;
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
      AdsClient adsClient = this;
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentOutOfRangeException(nameof (typeName));
      if (adsClient.IsDisposed)
        throw new ObjectDisposedException(adsClient.Name);
      if (!adsClient.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<ISymbolInfoTable> resultValue = await ((IAdsSymbolTableProvider) adsClient).GetSymbolTableAsync(cancel).ConfigureAwait(false);
      return ((ResultAds) resultValue).Succeeded ? await resultValue.Value.ReadTypeAsync(typeName, true, cancel).ConfigureAwait(false) : new ResultValue<IDataType>(((ResultAds) resultValue).ErrorCode, (IDataType) null);
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.  Array and structures are not supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'WriteSymbol' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public Task<ResultWrite> WriteValueAsync(
      ISymbol symbol,
      object val,
      CancellationToken cancel)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (((IProcessImageAddress) symbol).IsVirtual)
        return Task.FromResult<ResultWrite>(ResultWrite.CreateError((AdsErrorCode) 1796));
      IDataType dataType = ((IInstance) symbol).DataType;
      if (dataType == null)
        throw new CannotResolveDataTypeException((IInstance) symbol);
      bool flag1 = (this.getAccessMethod(symbol) & AdsClient.AccessMethods.ValueByHandle) > AdsClient.AccessMethods.None;
      bool flag2 = (this.getAccessMethod(symbol) & AdsClient.AccessMethods.ValueByName) > AdsClient.AccessMethods.None;
      Type managed;
      if (!PrimitiveTypeMarshaler.TryGetManagedType(dataType, out managed))
        managed = val.GetType();
      PrimitiveTypeMarshaler from = PrimitiveTypeMarshaler.CreateFrom(dataType);
      object val1 = val;
      if (val != null && managed != val.GetType())
        val1 = PrimitiveTypeMarshaler.Convert(val, managed);
      if (flag2)
        return this.WriteSymbolAsync(((IInstance) symbol).InstancePath, val1, cancel);
      IAdsSymbol iadsSymbol = (IAdsSymbol) symbol;
      byte[] array = new byte[from.MarshalSize(val1, this.DefaultValueEncoding)];
      from.Marshal(val1, this.DefaultValueEncoding, array.AsSpan<byte>());
      return this.WriteAsync(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel);
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.  Array and structures are not supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    public void WriteValue(ISymbol symbol, object val) => this.TryWriteValue(symbol, val).ThrowOnError();

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    public void WriteValue(string name, object value)
    {
      if (this.IsDisposed)
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
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous 'WriteSymbol' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public async Task<ResultWrite> WriteSymbolAsync(
      string name,
      object value,
      CancellationToken cancel)
    {
      AdsClient adsClient = this;
      if (adsClient.IsDisposed)
        throw new ObjectDisposedException(adsClient.Name);
      if (!adsClient.IsConnected)
        throw new ClientNotConnectedException();
      ResultValue<ISymbolInfoTable> resultValue = await ((IAdsSymbolTableProvider) adsClient).GetSymbolTableAsync(cancel).ConfigureAwait(false);
      return ((ResultAds) resultValue).Succeeded ? await resultValue.Value.WriteValueAsync(name, value, cancel).ConfigureAwait(false) : ResultWrite.CreateError(((ResultAds) resultValue).ErrorCode);
    }

    /// <summary>
    /// Reads value data from the specified IndexGroup/IndexOffset to the specified memory location.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="memory">The memory location</param>
    /// <param name="readBytes">The read bytes.</param>
    /// <returns>TwinCAT.Ads.AdsErrorCode.</returns>
    public AdsErrorCode TryRead(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> memory,
      out int readBytes)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultReadBytes readBytesSync = this._server.RequestAndReceiveReadBytesSync((Func<uint, AdsErrorCode>) (id =>
      {
        AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
        if (this._interceptors != null)
          adsErrorCode = this._interceptors.BeforeCommunicate();
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
          adsErrorCode = this._server.ReadRequestSync(this._target, id, indexGroup, indexOffset, memory.Length);
        return adsErrorCode;
      }), (Action<ResultReadBytes>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout);
      readBytes = ((ResultRead) readBytesSync).ReadBytes;
      if (((ResultAds) readBytesSync).Succeeded && ((ResultRead) readBytesSync).ReadBytes > 0)
      {
        int length = ((ResultRead) readBytesSync).ReadBytes > memory.Length ? memory.Length : ((ResultRead) readBytesSync).ReadBytes;
        readBytesSync.Data.CopyTo(memory.Slice(0, length));
      }
      return ((ResultAds) readBytesSync).ErrorCode;
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
      ResultReadBytes resultReadBytes = await this._server.RequestReadBytesAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
          result = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.ReadRequestAsync(this._target, id, indexGroup, indexOffset, readBuffer.Length, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultReadBytes>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout, cancel).ConfigureAwait(false);
      if (((ResultAds) resultReadBytes).ErrorCode == null && ((ResultRead) resultReadBytes).ReadBytes > 0)
      {
        int length = ((ResultRead) resultReadBytes).ReadBytes > readBuffer.Length ? readBuffer.Length : ((ResultRead) resultReadBytes).ReadBytes;
        resultReadBytes.Data.CopyTo(readBuffer.Slice(0, length));
      }
      return (ResultRead) resultReadBytes;
    }

    /// <summary>
    /// Read/Writes data asynchronously to/from the specified <paramref name="writeBuffer" />, <paramref name="readBuffer" /></summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadWrite' operation. The <see cref="T:TwinCAT.Ads.ResultReadWrite" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="F:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
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
      ResultReadBytes resultReadBytes = await this._server.RequestReadBytesAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
          result = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.ReadWriteRequestAsync(this._target, id, indexGroup, indexOffset, readBuffer.Length, writeBuffer, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultReadBytes>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout, cancel).ConfigureAwait(false);
      resultReadBytes.Data.CopyTo(readBuffer);
      return new ResultReadWrite(((ResultAds) resultReadBytes).ErrorCode, ((ResultRead) resultReadBytes).ReadBytes, ((ResultAds) resultReadBytes).InvokeId);
    }

    /// <summary>
    /// Read/Writes data  to/from the specified <paramref name="writeBuffer" />, <paramref name="readBuffer" />
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>A task that represents the asynchronous 'ReadWrite' operation. The <see cref="T:TwinCAT.Ads.ResultReadWrite" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="F:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    private ResultReadWrite ReadWriteSync(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultReadBytes readBytesSync = this._server.RequestAndReceiveReadBytesSync((Func<uint, AdsErrorCode>) (id =>
      {
        AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
        if (this._interceptors != null)
          adsErrorCode = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(adsErrorCode) ? this._server.ReadWriteRequestSync(this._target, id, indexGroup, indexOffset, readBuffer.Length, writeBuffer.Span) : adsErrorCode;
      }), (Action<ResultReadBytes>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout);
      readBytesSync.Data.CopyTo(readBuffer);
      return new ResultReadWrite(((ResultAds) readBytesSync).ErrorCode, ((ResultRead) readBytesSync).ReadBytes, ((ResultAds) readBytesSync).InvokeId);
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
    public AdsErrorCode TryReadWrite(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer,
      out int readBytes)
    {
      ResultReadWrite resultReadWrite = this.ReadWriteSync(indexGroup, indexOffset, readBuffer, writeBuffer);
      readBytes = ((ResultRead) resultReadWrite).ReadBytes;
      return ((ResultAds) resultReadWrite).ErrorCode;
    }

    /// <summary>
    /// Writes data synchronously to an ADS device and then Reads data from this device into the <paramref name="readBuffer" /></summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="readBuffer">The read buffer.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>Number of successfully returned (read) data bytes.</returns>
    public int ReadWrite(
      uint indexGroup,
      uint indexOffset,
      Memory<byte> readBuffer,
      ReadOnlyMemory<byte> writeBuffer)
    {
      int readBytes = 0;
      this.TryReadWrite(indexGroup, indexOffset, readBuffer, writeBuffer, out readBytes).ThrowOnError();
      return readBytes;
    }

    /// <summary>
    /// Triggers a write call at the specified IndexGroup/IndexOffset asynchronously.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadWrite' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public Task<ResultWrite> WriteAsync(
      uint indexGroup,
      uint indexOffset,
      CancellationToken cancel)
    {
      return this.WriteAsync(indexGroup, indexOffset, (ReadOnlyMemory<byte>) Memory<byte>.Empty, cancel);
    }

    /// <summary>
    /// Writes the data / Value asynchronously into the specified <paramref name="writeBuffer" />.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'Write' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
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
      ResultAds resultAds = await this._server.RequestAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
          result = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.WriteRequestAsync(this._target, id, indexGroup, indexOffset, writeBuffer, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultAds>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, r);
      }), this._timeout, cancel).ConfigureAwait(false);
      return new ResultWrite(resultAds.ErrorCode, resultAds.InvokeId);
    }

    /// <summary>
    /// Writes the data / Value into the specified <paramref name="writeBuffer" />.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>A task that represents the asynchronous 'Write' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    private ResultWrite WriteSync(
      uint indexGroup,
      uint indexOffset,
      ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ResultAds sync = this._server.RequestAndReceiveSync((Func<uint, AdsErrorCode>) (id =>
      {
        AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
        if (this._interceptors != null)
          adsErrorCode = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(adsErrorCode) ? this._server.WriteRequestSync(this._target, id, indexGroup, indexOffset, writeBuffer.Span) : adsErrorCode;
      }), (Action<ResultAds>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, r);
      }), this._timeout);
      return new ResultWrite(sync.ErrorCode, sync.InvokeId);
    }

    /// <summary>
    /// Reads the value of an Anytype (Primitive type) asynchronously.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="type">The type as AnyType (primitive types).</param>
    /// <param name="args">The type arguments (AnyType)</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultValue`1.Value" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
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
      this._anyTypeMarshaller.CanMarshal(type, args);
      int length = this._anyTypeMarshaller.MarshalSize(type, args, StringMarshaler.DefaultEncoding);
      ResultReadBytes resultReadBytes = await this.ReadAsync(indexGroup, indexOffset, length, cancel).ConfigureAwait(false);
      object obj;
      if (((ResultAds) resultReadBytes).Succeeded)
        this._anyTypeMarshaller.Unmarshal(type, args, resultReadBytes.Data.Span, this.DefaultValueEncoding, out obj);
      return new ResultAnyValue(((ResultAds) resultReadBytes).ErrorCode, obj, ((ResultAds) resultReadBytes).InvokeId);
    }

    /// <summary>
    /// Reads the value of an Anytype (Primitive type) asynchronously.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="type">The type as AnyType (primitive types).</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultValue`1.Value" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultAnyValue> ReadAnyAsync(
      uint indexGroup,
      uint indexOffset,
      Type type,
      CancellationToken cancel)
    {
      return this.ReadAnyAsync(indexGroup, indexOffset, type, (int[]) null, cancel);
    }

    /// <summary>
    /// Reads the value of an Anytype (Primitive type) asynchronously.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="type">The type as AnyType (primitive types).</param>
    /// <param name="args">The type arguments (AnyType)</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultValue`1.Value" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
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
      this._anyTypeMarshaller.CanMarshal(type, args);
      int length = this._anyTypeMarshaller.MarshalSize(type, args, this.DefaultValueEncoding);
      ResultReadBytes resultReadBytes = await this.ReadAsync(61445U, variableHandle, length, cancel).ConfigureAwait(false);
      ResultAnyValue resultAnyValue;
      if (((ResultAds) resultReadBytes).Succeeded)
      {
        object obj = (object) null;
        this._anyTypeMarshaller.Unmarshal(type, args, resultReadBytes.Data.Span, this.DefaultValueEncoding, out obj);
        resultAnyValue = new ResultAnyValue(((ResultAds) resultReadBytes).ErrorCode, obj, ((ResultAds) resultReadBytes).InvokeId);
      }
      else
        resultAnyValue = new ResultAnyValue(((ResultAds) resultReadBytes).ErrorCode, (object) null, ((ResultAds) resultReadBytes).InvokeId);
      return resultAnyValue;
    }

    /// <summary>
    /// Reads the value of an Anytype (Primitive type) asynchronously.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="type">The type as AnyType (primitive types).</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the value
    /// <see cref="P:TwinCAT.Ads.ResultValue`1.Value" /> and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultAnyValue> ReadAnyAsync(
      uint variableHandle,
      Type type,
      CancellationToken cancel)
    {
      return this.ReadAnyAsync(variableHandle, type, (int[]) null, cancel);
    }

    /// <summary>
    /// Write the value of an Anytype (Primitive type) asynchronously.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="value">The value.</param>
    /// <param name="args">The type arguments (AnyType)</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the value
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultWrite> WriteAnyAsync(
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
      byte[] array = new byte[this._anyTypeMarshaller.MarshalSize(value, args, this.DefaultValueEncoding)];
      this._anyTypeMarshaller.Marshal(value, args, this.DefaultValueEncoding, array.AsSpan<byte>());
      return this.WriteAsync(indexGroup, indexOffset, (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel);
    }

    /// <summary>
    /// Write the value of an Anytype (Primitive type) asynchronously.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="value">The value.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'ReadState' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the value
    /// the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> of the ADS communication after execution.</returns>
    public Task<ResultWrite> WriteAnyAsync(
      uint indexGroup,
      uint indexOffset,
      object value,
      CancellationToken cancel)
    {
      return this.WriteAnyAsync(indexGroup, indexOffset, value, (int[]) null, cancel);
    }

    /// <summary>
    /// Reads the value data of the symbol asynchronously into the <paramref name="readBuffer" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="readBuffer">The read buffer/data.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultRead" /> parameter contains the total number of bytes read into the buffer
    /// (<see cref="P:TwinCAT.Ads.ResultRead.ReadBytes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution..
    /// </returns>
    public async Task<ResultRead> ReadAsync(
      uint variableHandle,
      Memory<byte> readBuffer,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return await this.ReadAsync(61445U, variableHandle, readBuffer, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes the value data synchronously that is represented in the <paramref name="writeBuffer" /> to the symbol with the specified <paramref name="variableHandle" />.
    /// </summary>
    /// <param name="variableHandle">The variable handle.</param>
    /// <param name="writeBuffer">The write buffer / value.</param>
    /// <returns>The ADS error code.</returns>
    public AdsErrorCode TryWrite(uint variableHandle, ReadOnlyMemory<byte> writeBuffer)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this.TryWrite(61445U, variableHandle, writeBuffer);
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
      return await this.WriteAsync(61445U, variableHandle, writeBuffer, cancel).ConfigureAwait(false);
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
      return await this.ReadWriteAsync(61445U, variableHandle, readBuffer, writeBuffer, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" /> event.
    /// </summary>
    /// <param name="indexGroup">The index group number of the requested ADS service.</param>
    /// <param name="indexOffset">The index offset number of the requested ADS service.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="handle">The notification handle.</param>
    /// <returns>The ADS error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.IAdsNotifications.TryDeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" />
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <remarks>
    /// The <para>dataSize</para> Parameter defines the amount of bytes, that will be attached to the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotification" /> as value.
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
      return this._notificationReceiver.TryAddDeviceNotification(indexGroup, indexOffset, dataSize, settings, userData, this._timeout, out handle);
    }

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="userData">This object can be used to store user specific data (tag data)</param>
    /// <param name="anyType">Type of the object stored in the event argument ('AnyType')</param>
    /// <param name="args">The 'AnyType' arguments.</param>
    /// <param name="handle">The notification handle.</param>
    /// <returns>The ADS Error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="M:TwinCAT.Ads.IAdsNotifications.DeleteDeviceNotification(System.UInt32)" />
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotificationEx" />
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    /// <remarks>If type is a string type, the first element of the parameter args specifies the number of characters of the string.
    /// If type is an array type, the number of elements for each dimension has to be specified in the parameter args.
    /// Only primitive types (AnyType) are supported by this method.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.IAdsNotifications.DeleteDeviceNotification(System.UInt32)" /> should always
    /// called when the notification is not used anymore.</remarks>
    public AdsErrorCode TryAddDeviceNotificationEx(
      uint indexGroup,
      uint indexOffset,
      NotificationSettings settings,
      object? userData,
      Type anyType,
      int[]? args,
      out uint handle)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this._anyTypeMarshaller.CanMarshal(anyType, args))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot marshal the type '{0}' as 'ANY' type!", (object) anyType), nameof (anyType));
      int dataSize = this._anyTypeMarshaller.MarshalSize(anyType, args, this.DefaultValueEncoding);
      AdsNotificationExUserData userData1 = new AdsNotificationExUserData(anyType, args, userData);
      return this._notificationReceiver.TryAddDeviceNotification(indexGroup, indexOffset, dataSize, settings, (object) userData1, this._timeout, out handle);
    }

    /// <summary>
    /// Connects a variable to the ADS client asynchronously. The ADS client will be notified by the <see cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" /> event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="anyType">Type of the object stored in the event argument, only Primitive 'AnyTypes' allowed.</param>
    /// <param name="args">Additional arguments (for 'AnyType')</param>
    /// <param name="cancel">The Cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'AddDeviceNotification' operation. The <see cref="T:TwinCAT.Ads.ResultHandle" /> type parameter contains the created handle
    /// (<see cref="P:TwinCAT.Ads.ResultHandle.Handle" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <remarks>If type is a string type, the first element of the parameter args specifies the number of characters of the string.
    /// If type is an array type, the number of elements for each dimension has to be specified in the parameter args.
    /// Only primitive types (AnyType) are supported by this method.
    /// Because notifications allocate TwinCAT system resources, a complementary call to <see cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" /> should always
    /// called when the notification is not used anymore.</remarks>
    /// <seealso cref="M:TwinCAT.Ads.AdsClient.DeleteDeviceNotificationAsync(System.UInt32,System.Threading.CancellationToken)" />
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationEx" />
    /// <seealso cref="E:TwinCAT.Ads.AdsClient.AdsNotificationError" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotificationEx" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationExAsync" />
    public Task<ResultHandle> AddDeviceNotificationExAsync(
      uint indexGroup,
      uint indexOffset,
      NotificationSettings settings,
      object? userData,
      Type anyType,
      int[]? args,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (!this._anyTypeMarshaller.CanMarshal(anyType, args))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot marshal the type '{0}' as 'ANY' type!", (object) anyType), nameof (anyType));
      int dataSize = this._anyTypeMarshaller.MarshalSize(anyType, args, this.DefaultValueEncoding);
      AdsNotificationExUserData userData1 = new AdsNotificationExUserData(anyType, args, userData);
      return this._notificationReceiver.AddDeviceNotificationAsync(indexGroup, indexOffset, dataSize, settings, (object) userData1, cancel);
    }

    /// <summary>Deletes a registered notification.</summary>
    /// <param name="notificationHandle">Notification handle.</param>
    /// <returns>The ADS error code.</returns>
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <remarks>This is the complementary method to <see cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" /> overloads and should be called when the
    /// notification is not needed anymore the free TwinCAT realtime resources.</remarks>
    public AdsErrorCode TryDeleteDeviceNotification(uint notificationHandle) => this.TryDeleteDeviceNotification(notificationHandle, this._timeout);

    /// <summary>Deletes a registered notification.</summary>
    /// <param name="notificationHandle">Notification handle.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The ADS error code.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <remarks>This is the complementary method to <see cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" /> overloads and should be called when the
    /// notification is not needed anymore the free TwinCAT realtime resources.</remarks>
    public AdsErrorCode TryDeleteDeviceNotification(
      uint notificationHandle,
      int timeout)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._notificationReceiver.TryDeleteDeviceNotification(notificationHandle, timeout);
    }

    /// <summary>Deletes a registered notification asynchronously.</summary>
    /// <param name="notificationHandle">Notification handle.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'DeleteDeviceNotification' operation. The <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property contains the
    /// ADS error code after execution.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" />
    /// <seealso cref="E:TwinCAT.Ads.IAdsNotifications.AdsNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.TryAddDeviceNotification" />
    /// <seealso cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotification" />
    /// <remarks>This is the complementary method to <see cref="O:TwinCAT.Ads.IAdsNotifications.AddDeviceNotificationAsync" /> overloads and should be called when the
    /// notification is not needed anymore the free TwinCAT realtime resources.</remarks>
    public Task<ResultAds> DeleteDeviceNotificationAsync(
      uint notificationHandle,
      CancellationToken cancel)
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      return this._notificationReceiver.DeleteDeviceNotificationAsync(notificationHandle, cancel);
    }

    /// <summary>Adds a DeviceNotification asynchronously.</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="dataLength">Length of the data.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="notificationHandler">The notification handler.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultHandle&gt;.</returns>
    Task<ResultHandle> INotificationProvider.RegisterNotificationInternalAsync(
      uint indexGroup,
      uint indexOffset,
      int dataLength,
      NotificationSettings settings,
      Action<AmsAddress, Dictionary<DateTimeOffset, NotificationQueueElement[]>> notificationHandler,
      CancellationToken cancel)
    {
      return this._server.RequestHandleAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
          result = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.AddDeviceNotificationRequestAsync(this._target, id, indexGroup, indexOffset, dataLength, settings, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultHandle>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, (ResultAds) r);
      }), this._timeout, cancel);
    }

    /// <summary>Deletes a Device Notification.</summary>
    /// <param name="handle">The Notification handle.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultAds&gt;.</returns>
    Task<ResultAds> INotificationProvider.UnregisterNotificationInternalAsync(
      uint handle,
      CancellationToken cancel)
    {
      return this._server.RequestAsync((Func<uint, Task<AdsErrorCode>>) (id =>
      {
        AdsErrorCode result = (AdsErrorCode) 0;
        if (this._interceptors != null)
          result = this._interceptors.BeforeCommunicate();
        return AdsErrorCodeExtensions.Succeeded(result) ? this._server.DeleteDeviceNotificationRequestAsync(this._target, id, handle, cancel) : Task.FromResult<AdsErrorCode>(result);
      }), (Action<ResultAds>) (r =>
      {
        if (this._interceptors == null)
          return;
        this._interceptors.AfterCommunicate(false, r);
      }), this._timeout, cancel);
    }

    /// <summary>Removes / Deletes a Device Notification.</summary>
    /// <param name="handle">The handle.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode INotificationProvider.UnregisterNotificationInternal(
      uint handle,
      int timeout)
    {
      return this.TryDeleteDeviceNotification(handle, timeout);
    }

    AdsErrorCode INotificationProvider.UnregisterNotificationInternal(
      uint[] handles,
      out AdsErrorCode[]? subResults)
    {
      return new SumReleaseHandles((IAdsConnection) this, handles).TryReleaseHandles(out subResults);
    }

    /// <summary>
    /// Changes the ADS status and the device status of an ADS server.
    /// </summary>
    /// <param name="stateInfo">New ADS status and device status.</param>
    /// <param name="writeBuffer">The write buffer.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteControl(
      StateInfo stateInfo,
      ReadOnlyMemory<byte> writeBuffer)
    {
      return this.WriteControlSync(((StateInfo) ref stateInfo).AdsState, (ushort) ((StateInfo) ref stateInfo).DeviceState, writeBuffer).ErrorCode;
    }

    /// <summary>
    /// Injection of an SymbolVersionChanged event (just for Testing purposes)
    /// </summary>
    /// <exclude />
    void IAdsInjectAcceptor.InjectSymbolVersionChanged()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ((ISymbolVersionChangedReceiver) this).OnSymbolVersionChanged(new AdsSymbolVersionChangedEventArgs(byte.MaxValue));
    }

    public AdsErrorCode TryReadSymbol(string name, out IAdsSymbol? symbol)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentOutOfRangeException(nameof (name));
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ISymbolInfoTable isymbolInfoTable = (ISymbolInfoTable) null;
      symbol = (IAdsSymbol) null;
      AdsErrorCode adsErrorCode = ((IAdsSymbolTableProvider) this).TryGetSymbolTable(ref isymbolInfoTable);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
        adsErrorCode = isymbolInfoTable.TryReadSymbol(name, true, ref symbol);
      return adsErrorCode;
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.  Array and structures are not supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteValue(ISymbol symbol, object? val)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (((IProcessImageAddress) symbol).IsVirtual)
        return (AdsErrorCode) 1796;
      IDataType dataType = ((IInstance) symbol).DataType;
      if (dataType == null)
        throw new CannotResolveDataTypeException((IInstance) symbol);
      if (val == null)
        throw new ArgumentNullException(nameof (val));
      bool flag1 = (this.getAccessMethod(symbol) & AdsClient.AccessMethods.ValueByHandle) > AdsClient.AccessMethods.None;
      bool flag2 = (this.getAccessMethod(symbol) & AdsClient.AccessMethods.ValueByName) > AdsClient.AccessMethods.None;
      Type managed;
      if (!PrimitiveTypeMarshaler.TryGetManagedType(dataType, out managed))
        managed = val.GetType();
      PrimitiveTypeMarshaler from = PrimitiveTypeMarshaler.CreateFrom(dataType);
      object val1 = val;
      if (val != null && managed != val.GetType())
        val1 = PrimitiveTypeMarshaler.Convert(val, managed);
      if (flag2)
        return this.TryWriteValue(((IInstance) symbol).InstancePath, val1);
      IAdsSymbol iadsSymbol = (IAdsSymbol) symbol;
      Type type = val?.GetType();
      if ((object) type == null)
        type = typeof (object);
      byte[] array = new byte[from.MarshalSize(val1, this.DefaultValueEncoding)];
      from.Marshal(val1, this.DefaultValueEncoding, array.AsSpan<byte>());
      return this.TryWrite(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, (ReadOnlyMemory<byte>) array.AsMemory<byte>());
    }

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteValue(string name, object value)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      ISymbolInfoTable isymbolInfoTable = (ISymbolInfoTable) null;
      AdsErrorCode adsErrorCode = ((IAdsSymbolTableProvider) this).TryGetSymbolTable(ref isymbolInfoTable);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
        adsErrorCode = isymbolInfoTable.TryWriteValue(name, value);
      return adsErrorCode;
    }

    /// <summary>Sets the default encoding.</summary>
    /// <param name="encoding">The encoding.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SetSymbolEncoding(Encoding encoding) => this._symbolEncoding = encoding;

    /// <summary>Gets the default value encoding.</summary>
    /// <value>The default value encoding.</value>
    public Encoding DefaultValueEncoding => StringMarshaler.DefaultEncoding;

    /// <summary>Gets the symbol encoding.</summary>
    /// <value>The symbol encoding.</value>
    public Encoding SymbolEncoding => this._symbolEncoding ?? this.DefaultValueEncoding;

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
    public object? InvokeRpcMethod(string symbolPath, string methodName, object[]? inParameters) => this.InvokeRpcMethod(symbolPath, methodName, inParameters, out object[] _);

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
      object retValue = (object) null;
      this.TryInvokeRpcMethod(symbolPath, methodName, inParameters, out outParameters, out retValue).ThrowOnError();
      return retValue;
    }

    public object? InvokeRpcMethod(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters)
    {
      object retValue = (object) null;
      this.TryInvokeRpcMethod(symbolPath, methodName, inParameters, out outParameters, out retValue).ThrowOnError();
      return retValue;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public object? InvokeRpcMethod(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier? retSpecifier)
    {
      object retValue = (object) null;
      object[] outParameters = (object[]) null;
      this.TryInvokeRpcMethod(symbolPath, methodName, inParameters, (AnyTypeSpecifier[]) null, retSpecifier, out outParameters, out retValue).ThrowOnError();
      return retValue;
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
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (string.IsNullOrEmpty(symbolPath))
        throw new ArgumentOutOfRangeException(nameof (symbolPath));
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentOutOfRangeException(nameof (methodName));
      retValue = (object) null;
      outParameters = (object[]) null;
      IAdsSymbol symbol1 = (IAdsSymbol) null;
      AdsErrorCode adsErrorCode = this.TryReadSymbol(symbolPath, out symbol1);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
      {
        if (!(symbol1 is IStructInstance symbol2))
          throw new RpcMethodNotSupportedException(methodName, (ISymbol) symbol1);
        IRpcMethod rpcMethod;
        if (!((IRpcCallableInstance) symbol2).RpcMethods.TryGetMethod(methodName, ref rpcMethod))
          throw new ArgumentOutOfRangeException(nameof (methodName), "Method not found!");
        adsErrorCode = this.TryInvokeRpcMethod((IRpcCallableInstance) symbol2, rpcMethod, inParameters, outSpecifiers, retSpecifier, out outParameters, out retValue);
      }
      return adsErrorCode;
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
      return this.TryInvokeRpcMethod(symbolPath, methodName, inParameters, (AnyTypeSpecifier[]) null, (AnyTypeSpecifier) null, out outParameters, out retValue);
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
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (string.IsNullOrEmpty(symbolPath))
        throw new ArgumentOutOfRangeException(nameof (symbolPath));
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentOutOfRangeException(nameof (methodName));
      if (inParameters == null)
        throw new ArgumentNullException(nameof (inParameters));
      ResultRpcMethod result = ResultRpcMethod.Empty;
      ResultValue<IAdsSymbol> resultValue = await this.ReadSymbolAsync(symbolPath, cancel).ConfigureAwait(false);
      ((ResultAds) result).SetError(((ResultAds) resultValue).ErrorCode);
      if (((ResultAds) resultValue).Succeeded)
      {
        if (!(resultValue.Value is IStructInstance symbol))
          throw new RpcMethodNotSupportedException(methodName, (ISymbol) resultValue.Value);
        IRpcMethod rpcMethod = (IRpcMethod) null;
        if (!((IRpcCallableInstance) symbol).RpcMethods.TryGetMethod(methodName, ref rpcMethod))
          throw new RpcMethodNotSupportedException(methodName, (ISymbol) resultValue.Value);
        result = await this.InvokeRpcMethodAsync((IRpcCallableInstance) symbol, rpcMethod, inParameters, cancel).ConfigureAwait(false);
      }
      ResultRpcMethod resultRpcMethod = result;
      result = (ResultRpcMethod) null;
      return resultRpcMethod;
    }

    public async Task<ResultRpcMethod> InvokeRpcMethodAsync(
      string symbolPath,
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      CancellationToken cancel)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (string.IsNullOrEmpty(symbolPath))
        throw new ArgumentOutOfRangeException(nameof (symbolPath));
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentOutOfRangeException(nameof (methodName));
      if (inParameters == null)
        throw new ArgumentNullException(nameof (inParameters));
      ResultRpcMethod result = ResultRpcMethod.Empty;
      ResultValue<IAdsSymbol> resultValue = await this.ReadSymbolAsync(symbolPath, cancel).ConfigureAwait(false);
      ((ResultAds) result).SetError(((ResultAds) resultValue).ErrorCode);
      if (((ResultAds) resultValue).Succeeded)
      {
        if (!(resultValue.Value is IStructInstance symbol))
          throw new RpcMethodNotSupportedException(methodName, (ISymbol) resultValue.Value);
        IRpcMethod rpcMethod = (IRpcMethod) null;
        if (!((IRpcCallableInstance) symbol).RpcMethods.TryGetMethod(methodName, ref rpcMethod))
          throw new RpcMethodNotSupportedException(methodName, (ISymbol) resultValue.Value);
        result = await this.InvokeRpcMethodAsync((IRpcCallableInstance) symbol, rpcMethod, inParameters, outSpecifiers, retSpecifier, cancel).ConfigureAwait(false);
      }
      ResultRpcMethod resultRpcMethod = result;
      result = (ResultRpcMethod) null;
      return resultRpcMethod;
    }

    /// <summary>invoke RPC method as an asynchronous operation.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="rpcMethod">The RPC method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultRpcMethod&gt;.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:TwinCAT.ClientNotConnectedException"></exception>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    /// <exception cref="T:System.ArgumentNullException">rpcMethod</exception>
    public Task<ResultRpcMethod> InvokeRpcMethodAsync(
      IRpcCallableInstance symbol,
      IRpcMethod rpcMethod,
      object[]? inParameters,
      CancellationToken cancel)
    {
      return this.InvokeRpcMethodAsync(symbol, rpcMethod, inParameters, (AnyTypeSpecifier[]) null, (AnyTypeSpecifier) null, cancel);
    }

    public async Task<ResultRpcMethod> InvokeRpcMethodAsync(
      IRpcCallableInstance symbol,
      IRpcMethod rpcMethod,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpec,
      AnyTypeSpecifier? returnSpec,
      CancellationToken cancel)
    {
      AdsClient adsClient = this;
      if (adsClient.IsDisposed)
        throw new ObjectDisposedException(adsClient.Name);
      if (!adsClient.IsConnected)
        throw new ClientNotConnectedException();
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (rpcMethod == null)
        throw new ArgumentNullException(nameof (rpcMethod));
      ResultValue<ISymbolInfoTable> resultValue = await ((IAdsSymbolTableProvider) adsClient).GetSymbolTableAsync(cancel).ConfigureAwait(false);
      return ((ResultAds) resultValue).Succeeded ? await resultValue.Value.InvokeRpcMethodAsync(symbol, rpcMethod, inParameters, outSpec, returnSpec, cancel).ConfigureAwait(false) : new ResultRpcMethod(((ResultAds) resultValue).ErrorCode, (object) null, (object[]) null, ((ResultAds) resultValue).InvokeId);
    }

    public AdsErrorCode TryInvokeRpcMethod(
      IRpcCallableInstance symbol,
      IRpcMethod rpcMethod,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpec,
      AnyTypeSpecifier? returnSpec,
      out object[]? outParameters,
      out object? returnValue)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        throw new ClientNotConnectedException();
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (rpcMethod == null)
        throw new ArgumentNullException(nameof (rpcMethod));
      ISymbolInfoTable isymbolInfoTable = (ISymbolInfoTable) null;
      AdsErrorCode adsErrorCode = ((IAdsSymbolTableProvider) this).TryGetSymbolTable(ref isymbolInfoTable);
      outParameters = (object[]) null;
      returnValue = (object) null;
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
        adsErrorCode = isymbolInfoTable.TryInvokeRpcMethod(symbol, rpcMethod, inParameters, outSpec, returnSpec, ref outParameters, ref returnValue);
      return adsErrorCode;
    }

    /// <summary>
    /// Reads the value of a symbol and returns it as an object.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <returns>The value of the symbol.</returns>
    /// <remarks>Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'.</remarks>
    [return: NotNull]
    public T ReadValue<T>(ISymbol symbol) where T : notnull => PrimitiveTypeMarshaler.Convert<T>(this.ReadValue(symbol));

    /// <summary>
    /// Reads the value of a symbol and returns it as an object.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <param name="value">The value.</param>
    /// <returns>The ADS Error Code</returns>
    /// <remarks>Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'.</remarks>
    public AdsErrorCode TryReadValue<T>(ISymbol symbol, [AllowNull] out T value)
    {
      object sourceValue = (object) null;
      AdsErrorCode adsErrorCode = this.TryReadValue(symbol, out sourceValue);
      value = !AdsErrorCodeExtensions.Succeeded(adsErrorCode) ? default (T) : PrimitiveTypeMarshaler.Convert<T>(sourceValue);
      return adsErrorCode;
    }

    /// <summary>
    /// Reads the value of a symbol asynchronously and returns it as an object.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="symbol">The symbol that should be read.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="F:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <remarks>Supported types are limited to 'AnyTypes', what includes all primitive types (UInt32, Int32, Bool etc.), strings, and Arrays that are
    /// compositions of 'AnyTypes'.</remarks>
    public async Task<ResultValue<TValue>> ReadValueAsync<TValue>(
      ISymbol symbol,
      CancellationToken cancel)
    {
      ResultAnyValue resultAnyValue = await this.ReadValueAsync(symbol, cancel).ConfigureAwait(false);
      return !((ResultAds) resultAnyValue).Succeeded ? ResultValue<TValue>.CreateError(((ResultAds) resultAnyValue).ErrorCode) : ResultValue<TValue>.CreateSuccess(PrimitiveTypeMarshaler.Convert<TValue>(((ResultValue<object>) resultAnyValue).Value));
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
      T obj;
      this.TryReadValue<T>(name, out obj).ThrowOnError();
      return obj;
    }

    /// <summary>
    /// Reads the value of a symbol and returns the value as object.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">The read value of the Symbol.</param>
    /// <returns>The <see cref="T:TwinCAT.Ads.AdsErrorCode" />.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <remarks>The parameter type must have the same layout as the ADS symbol.</remarks>
    public AdsErrorCode TryReadValue<T>(string name, [AllowNull] out T value)
    {
      object sourceValue;
      AdsErrorCode adsErrorCode = this.TryReadValue(name, typeof (T), out sourceValue);
      value = !AdsErrorCodeExtensions.Succeeded(adsErrorCode) ? default (T) : PrimitiveTypeMarshaler.Convert<T>(sourceValue);
      return adsErrorCode;
    }

    /// <summary>Reads the value of a symbol asynchronously.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous read operation. The <see cref="T:TwinCAT.Ads.ResultAnyValue" /> parameter contains the read value
    /// (<see cref="F:TwinCAT.Ads.ResultValue`1.Value" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <remarks>The parameter type must have the same layout as the ADS symbol.</remarks>
    public async Task<ResultValue<TValue>> ReadValueAsync<TValue>(
      string name,
      CancellationToken cancel)
    {
      ResultAnyValue resultAnyValue = await this.ReadValueAsync(name, typeof (TValue), cancel).ConfigureAwait(false);
      return !((ResultAds) resultAnyValue).Succeeded ? ResultValue<TValue>.CreateError(((ResultAds) resultAnyValue).ErrorCode) : ResultValue<TValue>.CreateSuccess((TValue) ((ResultValue<object>) resultAnyValue).Value);
    }

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    public void WriteValue<T>(ISymbol symbol, T val) where T : notnull => this.WriteValue(symbol, (object) val);

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteValue<T>(ISymbol symbol, [DisallowNull] T val) where T : notnull => this.TryWriteValue(symbol, (object) val);

    /// <summary>
    /// Writes a value to the symbol. Strings and all primitive data types(UInt32, Int32, Bool etc.) are supported.
    /// If a string is passed as parameter, the method attempts to parse the string according to the ADS data type of the symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="symbol">The symbol the value is written to.</param>
    /// <param name="val">The value to write.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'WriteSymbol' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public Task<ResultWrite> WriteValueAsync<T>(
      ISymbol symbol,
      [DisallowNull] T val,
      CancellationToken cancel)
      where T : notnull
    {
      return this.WriteValueAsync(symbol, (object) val, cancel);
    }

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <typeparam name="T">the value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    public void WriteValue<T>(string name, [DisallowNull] T value) where T : notnull => this.WriteValue(name, (object) value);

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryWriteValue<T>(string name, [DisallowNull] T value) where T : notnull => this.TryWriteValue(name, (object) value);

    /// <summary>
    /// Writes the passed object value to the specified ADS symbol.The parameter type must have the same
    /// layout as the ADS symbol.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the ADS symbol.</param>
    /// <param name="value">Object holding the value to be written to the ADS symbol</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns>A task that represents the asynchronous 'WriteSymbol' operation. The <see cref="T:TwinCAT.Ads.ResultWrite" /> parameter contains the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public Task<ResultWrite> WriteValueAsync<T>(
      string name,
      [DisallowNull] T value,
      CancellationToken cancel)
      where T : notnull
    {
      return this.WriteSymbolAsync(name, (object) value, cancel);
    }

    /// <summary>Enum SumAccessMode</summary>
    /// <exclude />
    [Flags]
    internal enum AccessMethods : uint
    {
      /// <summary>Access by IndexGroup / IndexOffset</summary>
      IndexGroupIndexOffset = 1,
      /// <summary>Accesses a value by handle</summary>
      ValueByHandle = 2,
      /// <summary>Access a value by name</summary>
      ValueByName = 4,
      /// <summary>Acquire handle by name</summary>
      AcquireHandleByName = 16, // 0x00000010
      /// <summary>Release handle</summary>
      ReleaseHandle = 32, // 0x00000020
      /// <summary>None / Uninitialized</summary>
      None = 0,
      /// <summary>All Access methods are allowed</summary>
      Mask_All = ReleaseHandle | AcquireHandleByName | ValueByName | ValueByHandle | IndexGroupIndexOffset, // 0x00000037
      /// <summary>
      /// Only Symbolic access is allowed (No Processimage IndexGroup/IndexOffset)
      /// </summary>
      Mask_Symbolic = ReleaseHandle | AcquireHandleByName | ValueByName | ValueByHandle, // 0x00000036
    }
  }
}
