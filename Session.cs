// Decompiled with JetBrains decompiler
// Type: TwinCAT.Session
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT
{
  /// <summary>Abstract Session base class.</summary>
  /// <seealso cref="T:TwinCAT.ISession" />
  /// <seealso cref="T:System.IDisposable" />
  [DebuggerDisplay("Id={Id}, Address={AddressSpecifier}, ConnectionState={ConnectionState}")]
  public abstract class Session : 
    ISession,
    IConnectionStateProvider,
    ISymbolServerProvider,
    IDisposable
  {
    /// <summary>The provider</summary>
    /// <exclude />
    private ISessionProvider provider;
    /// <summary>Session ID counter (static)</summary>
    private static int s_id;
    /// <summary>Session Identifier</summary>
    private int _id = ++Session.s_id;
    /// <summary>The (established) connection</summary>
    private IConnection? connection;
    private DateTimeOffset _sessionEstablishedAt = DateTimeOffset.MaxValue;
    /// <summary>Disposed flag.</summary>
    private bool _disposed;
    /// <summary>The symbol server</summary>
    private ISymbolServer? _symbolServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Session" /> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <exclude />
    protected Session(ISessionProvider provider) => this.provider = provider != null ? provider : throw new ArgumentNullException(nameof (provider));

    /// <summary>Gets the Session Provider</summary>
    /// <value>The provider or NULL if instantiated directly</value>
    /// <exclude />
    public ISessionProvider Provider => this.provider;

    /// <summary>Gets the Session Identifier</summary>
    /// <value>The identifier.</value>
    public int Id => this._id;

    /// <summary>Gets the (established) connection.</summary>
    /// <value>The <see cref="T:TwinCAT.IConnection" /> if connection established, or <b>null</b> if not connected.</value>
    public IConnection? Connection
    {
      get => this.connection;
      protected set => this.connection = value;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is connected.
    /// </summary>
    /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
    public bool IsConnected => !this._disposed && this.connection != null && this.connection.IsConnected;

    /// <summary>Connects the session.</summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <remarks>The <see cref="T:TwinCAT.IConnection" /> will be valid until the <see cref="T:TwinCAT.ISession" /> is disconnected via
    /// the <see cref="M:TwinCAT.Session.Disconnect" /> method or the Dispose method is called. Any possible resurrections after communication
    /// losses will be done transparently within the <see cref="T:TwinCAT.IConnection" /> so that the <see cref="T:TwinCAT.IConnection" /> instance and <see cref="T:TwinCAT.ISession" /> instance
    /// remains.</remarks>
    public IConnection Connect()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      if (!this.IsConnected)
        this.connection = this.OnConnect(this.connection != null);
      return this.connection != null ? this.connection : throw new AdsException("Connection to '" + this.AddressSpecifier + "' couldn't be established!");
    }

    /// <summary>Handler function connecting the Session.</summary>
    /// <param name="reconnect">if set to <c>true</c> [reconnect].</param>
    /// <returns>IConnection.</returns>
    protected virtual IConnection? OnConnect(bool reconnect)
    {
      if (this.connection != null && !reconnect)
        this._sessionEstablishedAt = DateTimeOffset.Now;
      return this.connection;
    }

    /// <summary>Disconnects the session from the target.</summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ObjectDisposedException"></exception>
    /// <remarks>Closes (and disposes) the underlying <see cref="T:TwinCAT.IConnection" />
    /// The <see cref="T:TwinCAT.Session" /> itself will not be Disposed and can be reconnected.</remarks>
    public bool Disconnect()
    {
      if (this._disposed)
        throw new ObjectDisposedException(this.Name);
      bool flag = false;
      if (this.IsConnected)
        flag = this.OnDisconnect();
      return flag;
    }

    /// <summary>Handler function disconnecting the session.</summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected virtual bool OnDisconnect()
    {
      if (this.connection == null)
        return false;
      this.connection.Disconnect();
      return true;
    }

    /// <summary>Gets the UTC time when the session was established.</summary>
    /// <value>The session established at.</value>
    public DateTimeOffset EstablishedAt => this._sessionEstablishedAt;

    /// <summary>
    /// Handles the <see cref="E:ConnectionStateChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="T:TwinCAT.ConnectionStateChangedEventArgs" /> instance containing the event data.</param>
    protected void OnConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
      if (this.ConnectionStateChanged == null)
        return;
      this.ConnectionStateChanged((object) this, e);
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Session" /> is disposed.
    /// </summary>
    /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
    public bool Disposed => this._disposed;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Closes this <see cref="T:TwinCAT.ISession" />
    /// </summary>
    /// <remarks>Closes also the <see cref="T:TwinCAT.IConnection" />.</remarks>
    public void Close() => this.Dispose();

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing && this.connection != null)
      {
        ((IConnectionStateProvider) this.connection).ConnectionStateChanged -= new EventHandler<ConnectionStateChangedEventArgs>(this.OnConnectionStateChanged);
        if (this.connection is IDisposable connection)
          connection.Dispose();
        this.connection = (IConnection) null;
        this._sessionEstablishedAt = DateTimeOffset.MaxValue;
        this._symbolServer = (ISymbolServer) null;
      }
      this._disposed = true;
    }

    /// <summary>
    /// Occurs when connection status of the <see cref="T:TwinCAT.IConnectionStateProvider" /> has been changed.
    /// </summary>
    /// <remarks>The Connection state changes only if the <see cref="T:TwinCAT.IConnection" /> is established / shut down
    /// or active communication is triggered by the User of the <see cref="T:TwinCAT.IConnection" /> object.
    /// </remarks>
    /// <example>
    /// The following sample shows how to keep the <see cref="P:TwinCAT.Session.ConnectionState" /> updated by triggering ADS Communication.
    /// <code language="C#" title="Trigger ConnectionState changes in WPF Applications" source="..\..\Samples\TwinCAT.ADS.NET_Samples\40_ADS.NET_WPFConnectionObserver\MainWindow.xaml.cs" region="CODE_SAMPLE" />
    /// </example>
    /// <seealso cref="P:TwinCAT.Session.ConnectionState" />
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>Gets the symbol server.</summary>
    /// <remarks>
    /// The <see cref="T:TwinCAT.Session" /> object holds and caches the symbolic information.
    /// To initially create this information, the Connection must be established.
    /// </remarks>
    /// <value>The symbol server.</value>
    public ISymbolServer SymbolServer
    {
      get
      {
        if (this._symbolServer == null)
          this._symbolServer = this.OnCreateSymbolServer();
        return this._symbolServer;
      }
    }

    /// <summary>
    /// Handler function creating the <see cref="T:TwinCAT.TypeSystem.ISymbolServer" />
    /// </summary>
    /// <returns>ISymbolServer.</returns>
    /// <exception cref="T:TwinCAT.SessionNotConnectedException">The connection is not established!</exception>
    protected abstract ISymbolServer OnCreateSymbolServer();

    /// <summary>Gets the name of the session</summary>
    /// <value>The name.</value>
    public string Name => this.GetSessionName();

    /// <summary>
    /// Gets the current Connection state of the <see cref="T:TwinCAT.Session" />
    /// </summary>
    /// <value>The state of the connection.</value>
    /// <remarks>The Connection state changes only if the <see cref="T:TwinCAT.IConnection" /> is established / shut down
    /// or active communication is triggered by the User of the <see cref="T:TwinCAT.IConnection" /> object.
    /// </remarks>
    /// <example>
    /// The following sample shows how to keep the <see cref="P:TwinCAT.Session.ConnectionState" /> updated by triggering ADS Communication.
    /// <code language="C#" title="Trigger ConnectionState changes in WPF Applications" source="..\..\Samples\TwinCAT.ADS.NET_Samples\40_ADS.NET_WPFConnectionObserver\MainWindow.xaml.cs" region="CODE_SAMPLE" />
    /// </example>
    /// <seealso cref="E:TwinCAT.Session.ConnectionStateChanged" />
    public ConnectionState ConnectionState
    {
      get
      {
        if (this._disposed)
          return (ConnectionState) 0;
        return this.IsConnected ? ((IConnectionStateProvider) this.connection).ConnectionState : (ConnectionState) 1;
      }
    }

    /// <summary>
    /// Gets the communication endpoint address string representation.
    /// </summary>
    /// <value>The address.</value>
    public string AddressSpecifier => this.OnGetAddress();

    /// <summary>Handler function getting the address of the session.</summary>
    /// <returns>System.String.</returns>
    protected abstract string OnGetAddress();

    /// <summary>Gets the name/string identifier of the session.</summary>
    /// <returns>System.String.</returns>
    protected abstract string GetSessionName();

    /// <summary>
    /// Ensures, that the <see cref="T:TwinCAT.ISession" /> is connected and returns the <see cref="T:TwinCAT.IConnection" /> object.
    /// </summary>
    /// <returns>IConnection.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <remarks>If the session is actually not connected an exception will be thrown.</remarks>
    public IConnection EnsureConnection()
    {
      IConnection connection = this.Connection;
      if (connection == null || !this.IsConnected || !connection.IsConnected)
        throw new SessionNotConnectedException((ISession) this);
      return connection;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
    public override string ToString()
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 2);
      interpolatedStringHandler.AppendLiteral("Session ID:");
      interpolatedStringHandler.AppendFormatted<int>(this.Id);
      interpolatedStringHandler.AppendLiteral(" (");
      interpolatedStringHandler.AppendFormatted(this.AddressSpecifier);
      interpolatedStringHandler.AppendLiteral(")");
      return interpolatedStringHandler.ToStringAndClear();
    }
  }
}
