// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.NotificationEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class NotificationEntry.</summary>
  /// <remarks>This class represents a single Notification Value, bound with Client/Server Handle, VariableHandle, RawData and userData.
  /// </remarks>
  internal class NotificationEntry
  {
    /// <summary>The user data</summary>
    private object? _userData;
    /// <summary>The client handle</summary>
    private uint _clientHandle;
    /// <summary>The server handle</summary>
    private uint _serverHandle;
    private int _maxDataSize = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.NotificationEntry" /> class.
    /// </summary>
    /// <param name="clientHandle">The client handle.</param>
    /// <param name="serverHandle">The server handle.</param>
    /// <param name="maxSize">The maximum size of the notification (or -1 if not checked)</param>
    /// <param name="userData">The user data.</param>
    internal NotificationEntry(uint clientHandle, uint serverHandle, int maxSize, object? userData)
    {
      this._clientHandle = clientHandle;
      this._serverHandle = serverHandle;
      this._userData = userData;
      this._maxDataSize = maxSize;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.NotificationEntry" /> class.
    /// </summary>
    /// <param name="clientHandle">The client handle.</param>
    /// <param name="serverHandle">The server handle.</param>
    /// <param name="userData">The user data.</param>
    internal NotificationEntry(uint clientHandle, uint serverHandle, object userData)
      : this(clientHandle, serverHandle, -1, userData)
    {
    }

    /// <summary>Gets the user data.</summary>
    /// <value>The user data.</value>
    public object? UserData => this._userData;

    /// <summary>Gets the client handle.</summary>
    /// <value>The client handle.</value>
    public uint ClientHandle => this._clientHandle;

    /// <summary>Gets the server handle.</summary>
    /// <value>The server handle.</value>
    public uint ServerHandle => this._serverHandle;

    /// <summary>Gets the size of the notification data in  bytes.</summary>
    /// <value>The size of the notification data.</value>
    /// <remarks>
    /// In some circumstances (e.g. Broadcast search) the size of the Notifications is variable and
    /// we have to define the maximum size.
    /// </remarks>
    private int MaxDataSize => this._maxDataSize;

    public bool CheckData(ReadOnlyMemory<byte> data) => this._maxDataSize < 0 || data.Length <= this._maxDataSize;
  }
}
