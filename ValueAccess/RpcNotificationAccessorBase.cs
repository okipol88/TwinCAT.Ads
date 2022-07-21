// Decompiled with JetBrains decompiler
// Type: TwinCAT.ValueAccess.RpcNotificationAccessorBase
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.ValueAccess
{
  /// <summary>
  /// Abstract base class for Accessing Values with the RawValue, Value, Rpc and Notification concept.
  /// </summary>
  /// <seealso cref="T:TwinCAT.ValueAccess.ValueAccessor" />
  /// <seealso cref="T:TwinCAT.ValueAccess.IAccessorNotification" />
  /// <seealso cref="T:TwinCAT.ValueAccess.IAccessorRpc" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class RpcNotificationAccessorBase : 
    ValueAccessor,
    IAccessorNotification,
    IAccessorRpc
  {
    /// <summary>Default notification settings.</summary>
    private INotificationSettings _notificationSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.RpcNotificationAccessorBase" /> class.
    /// </summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="connection">The connection.</param>
    protected RpcNotificationAccessorBase(
      IAccessorValueFactory valueFactory,
      IConnection connection)
      : this(valueFactory, connection, (INotificationSettings) NotificationSettings.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.RpcNotificationAccessorBase" /> class.
    /// </summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="connection">The connection.</param>
    /// <param name="defaultSettings">The default settings.</param>
    protected RpcNotificationAccessorBase(
      IAccessorValueFactory valueFactory,
      IConnection connection,
      INotificationSettings defaultSettings)
      : base(valueFactory, connection)
    {
      this._notificationSettings = defaultSettings;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.RpcNotificationAccessorBase" /> class.
    /// </summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="session">The session.</param>
    protected RpcNotificationAccessorBase(IAccessorValueFactory valueFactory, ISession session)
      : base(valueFactory, session)
    {
      this._notificationSettings = (INotificationSettings) NotificationSettings.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.RpcNotificationAccessorBase" /> class.
    /// </summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="session">The session.</param>
    /// <param name="defaultSettings">The default settings.</param>
    protected RpcNotificationAccessorBase(
      IAccessorValueFactory valueFactory,
      ISession session,
      INotificationSettings defaultSettings)
      : base(valueFactory, session)
    {
      this._notificationSettings = defaultSettings;
    }

    /// <summary>
    /// Registers a Notification on the <see cref="T:TwinCAT.TypeSystem.ISymbol" />.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">Type of Notification (Value, Raw or Both)</param>
    /// <param name="settings">The settings.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// symbol
    /// or
    /// settings
    /// </exception>
    /// <exclude />
    /// <remarks>Only one Notification is allowed on the symbol. On case of double announcement, we set the Notification parameters
    /// to the higher priority.</remarks>
    public abstract void OnRegisterNotification(
      ISymbol symbol,
      SymbolNotificationTypes type,
      INotificationSettings settings);

    /// <summary>
    /// Unregisters a Notification from the <see cref="T:TwinCAT.TypeSystem.ISymbol" />.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">Type of Notification (Value, Raw or Both)</param>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    /// <exclude />
    public abstract void OnUnregisterNotification(ISymbol symbol, SymbolNotificationTypes type);

    /// <summary>
    /// Gets / Sets the NotificationSettings that are used for Notification Defaults.
    /// </summary>
    /// <value>The default notification settings.</value>
    public INotificationSettings DefaultNotificationSettings
    {
      get => this._notificationSettings;
      set => this._notificationSettings = value;
    }

    public bool TryGetNotificationSettings(ISymbol symbol, [NotNullWhen(true)] out INotificationSettings? settings)
    {
      IValueSymbol ivalueSymbol = (IValueSymbol) symbol;
      if (ivalueSymbol != null)
      {
        settings = ivalueSymbol.NotificationSettings;
        return true;
      }
      settings = (INotificationSettings) null;
      return false;
    }

    public abstract int TryInvokeRpcMethod(
      IInstance instance,
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outParametersSpec,
      AnyTypeSpecifier? retSpec,
      out object[]? outParameters,
      out object? returnValue,
      out DateTimeOffset? timeStamp);

    public abstract Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      IInstance instance,
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outParametersSpec,
      AnyTypeSpecifier? retSpec,
      CancellationToken cancel);
  }
}
