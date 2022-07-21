// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.IAdsNotificationsBase
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Interface for Notification management.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal interface IAdsNotificationsBase
  {
    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the AdsNotification event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="cancel">The cancellation token.</param>
    Task<ResultHandle> AddDeviceNotificationAsync(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object userData,
      CancellationToken cancel);

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the AdsNotification event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="handle">The handle.</param>
    /// <returns>The handle of the notification.</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsErrorException">Thrown when the ADS call fails.</exception>
    AdsErrorCode TryAddDeviceNotification(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object userData,
      int timeout,
      out uint handle);

    /// <summary>Deletes an existing notification.</summary>
    /// <param name="notificationHandle">Handle of the notification.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode TryDeleteDeviceNotification(uint notificationHandle, int timeout);

    /// <summary>Deletes an existing notification.</summary>
    /// <param name="notificationHandle">Handle of the notification.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;TaskResult&gt;.</returns>
    Task<ResultAds> DeleteDeviceNotificationAsync(
      uint notificationHandle,
      CancellationToken cancel);
  }
}
