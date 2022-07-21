// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.INotificationProvider
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Low level interface for Notification management</summary>
  internal interface INotificationProvider
  {
    /// <summary>Adds a DeviceNotification asynchronously.</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="dataLength">Length of the data.</param>
    /// <param name="settings">The Notification settings.</param>
    /// <param name="notificationHandler">The notification handler.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultHandle&gt;.</returns>
    Task<ResultHandle> RegisterNotificationInternalAsync(
      uint indexGroup,
      uint indexOffset,
      int dataLength,
      NotificationSettings settings,
      Action<AmsAddress, Dictionary<DateTimeOffset, NotificationQueueElement[]>> notificationHandler,
      CancellationToken cancel);

    /// <summary>Deletes a Device Notification.</summary>
    /// <param name="handle">The Notification handle.</param>
    /// <param name="token">The Notification token.</param>
    /// <returns>Task&lt;ResultAds&gt;.</returns>
    Task<ResultAds> UnregisterNotificationInternalAsync(
      uint handle,
      CancellationToken token);

    /// <summary>Removes / Deletes a Device Notification.</summary>
    /// <param name="handle">The handle.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode UnregisterNotificationInternal(uint handle, int timeout);

    AdsErrorCode UnregisterNotificationInternal(
      uint[] handles,
      out AdsErrorCode[]? subResults);
  }
}
