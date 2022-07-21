// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.NotificationQueueElement
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;

namespace TwinCAT.Ads.Internal
{
  /// <summary>Class NotificationQueueElement.</summary>
  internal class NotificationQueueElement
  {
    /// <summary>The server handle</summary>
    private uint _serverHandle;
    /// <summary>The time stamp</summary>
    private DateTimeOffset _timeStamp;
    /// <summary>The Notification data.</summary>
    private ReadOnlyMemory<byte> _data;

    /// <summary>Gets the server handle.</summary>
    /// <value>The server handle.</value>
    public uint ServerHandle => this._serverHandle;

    /// <summary>Gets the time stamp of the Notification</summary>
    /// <value>The time stamp.</value>
    public DateTimeOffset TimeStamp => this._timeStamp;

    /// <summary>Gets the Notification data</summary>
    /// <value>The data.</value>
    public ReadOnlyMemory<byte> Data => this._data;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.NotificationQueueElement" /> class.
    /// </summary>
    /// <param name="serverHandle">The server handle.</param>
    /// <param name="timeStamp">The time stamp.</param>
    /// <param name="notificationData">The notification data.</param>
    internal NotificationQueueElement(
      uint serverHandle,
      DateTimeOffset timeStamp,
      ReadOnlyMemory<byte> notificationData)
    {
      this._serverHandle = serverHandle;
      this._timeStamp = timeStamp;
      this._data = notificationData;
    }
  }
}
