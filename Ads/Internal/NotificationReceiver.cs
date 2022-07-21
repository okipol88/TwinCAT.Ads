// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.NotificationReceiver
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Summary description for NotificationMngt.</summary>
  internal class NotificationReceiver : NotificationReceiverBase
  {
    private const int DEFAULT_QUEUESIZE = 500;
    private const int QUEUEPEAK = 10000;
    private uint _curHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.NotificationReceiver" /> class.
    /// </summary>
    /// <param name="syncPort">The synchronize port.</param>
    /// <param name="symbolTable">The symbol table.</param>
    /// <param name="notReceiver">The not receiver.</param>
    public NotificationReceiver(
      INotificationProvider syncPort,
      IHandleTable symbolTable,
      IClientNotificationReceiver notReceiver)
      : base(syncPort, symbolTable, notReceiver)
    {
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing) => base.Dispose(disposing);

    protected override void Init() => base.Init();

    /// <summary>Called when [add notification].</summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="dataSize">Size of the data.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">The user data.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="result">The result.</param>
    /// <returns>System.UInt32.</returns>
    protected override uint OnAddNotification(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      int timeout,
      out AdsErrorCode result)
    {
      uint num = 0;
      if (!this._initialized)
        this.Init();
      using (CancellationTokenSource c = new CancellationTokenSource(timeout))
      {
        ResultHandle resultHandle = AsyncPump.Run<ResultHandle>((Func<Task<ResultHandle>>) (() => this._notificationProvider.RegisterNotificationInternalAsync(indexGroup, indexOffset, dataSize, settings, new Action<AmsAddress, Dictionary<DateTimeOffset, NotificationQueueElement[]>>(((NotificationReceiverBase) this).OnNotification), c.Token)));
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref result = (int) ((ResultAds) resultHandle).ErrorCode;
        uint handle = resultHandle.Handle;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        if (^(int&) ref result == 0)
        {
          num = this.GetNextClientHandle();
          this.OnAddNotification(new NotificationEntry(num, handle, dataSize, userData));
          this._clientHandleTable.TryAdd(num, handle);
        }
      }
      return num;
    }

    /// <summary>add device notification as an asynchronous operation.</summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>The handle of the notification.</returns>
    public override async Task<ResultHandle> AddDeviceNotificationAsync(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      CancellationToken cancel)
    {
      NotificationReceiver notificationReceiver = this;
      ResultHandle resultHandle = await notificationReceiver._notificationProvider.RegisterNotificationInternalAsync(indexGroup, indexOffset, dataSize, settings, new Action<AmsAddress, Dictionary<DateTimeOffset, NotificationQueueElement[]>>(((NotificationReceiverBase) notificationReceiver).OnNotification), cancel).ConfigureAwait(false);
      if (((ResultAds) resultHandle).ErrorCode == null)
      {
        uint handle = resultHandle.Handle;
        uint nextClientHandle = notificationReceiver.GetNextClientHandle();
        NotificationEntry notification = new NotificationEntry(nextClientHandle, handle, dataSize, userData);
        notificationReceiver.OnAddNotification(notification);
        resultHandle = new ResultHandle((AdsErrorCode) 0, nextClientHandle, ((ResultAds) resultHandle).InvokeId);
      }
      return resultHandle;
    }

    public override async Task<ResultAds> DeleteDeviceNotificationAsync(
      uint clientHandle,
      CancellationToken cancel)
    {
      NotificationReceiver notificationReceiver = this;
      ResultAds resultAds = ResultAds.Empty;
      AdsErrorCode result = (AdsErrorCode) 0;
      // ISSUE: reference to a compiler-generated method
      NotificationEntry notificationEntry = notificationReceiver.\u003C\u003En__0(clientHandle, cancel, out result);
      resultAds.SetError(result);
      if (notificationEntry != null && result == null)
        resultAds = await notificationReceiver._notificationProvider.UnregisterNotificationInternalAsync(notificationEntry.ServerHandle, cancel).ConfigureAwait(false);
      return resultAds;
    }

    protected override NotificationEntry? OnDeleteNotification(
      uint clientHandle,
      CancellationToken cancel,
      out AdsErrorCode result)
    {
      NotificationEntry entry = base.OnDeleteNotification(clientHandle, cancel, out result);
      if (entry != null)
        AsyncPump.Run<ResultAds>((Func<Task<ResultAds>>) (() => this._notificationProvider.UnregisterNotificationInternalAsync(entry.ServerHandle, cancel)));
      return entry;
    }

    protected override void RemoveAllNotifications(bool all)
    {
      if (this._clientHandleTable == null || this._notificationTable == null || this._notificationTable.IsEmpty)
        return;
      bool flag1 = false;
      bool flag2 = true;
      while (flag2)
      {
        if (flag1)
        {
          ICollection<uint> keys = this._notificationTable.Keys;
          if (!all)
          {
            keys.Remove(this._symbolVersionServerHandle);
            keys.Remove(this._adsStateServerHandle);
          }
          if (this._notificationProvider.UnregisterNotificationInternal(keys.ToArray<uint>(), out AdsErrorCode[] _) == 1793)
          {
            flag2 = true;
            flag1 = false;
          }
          else
            flag2 = false;
        }
        else
        {
          foreach (KeyValuePair<uint, uint> keyValuePair in this._clientHandleTable)
          {
            bool flag3 = true;
            if (!all)
              flag3 = (int) keyValuePair.Value != (int) this._symbolVersionServerHandle && (int) keyValuePair.Value != (int) this._adsStateServerHandle;
            if (flag3)
            {
              try
              {
                this._notificationProvider.UnregisterNotificationInternal(keyValuePair.Key, 2000);
              }
              catch (TaskCanceledException ex)
              {
              }
              catch (Exception ex)
              {
              }
            }
          }
          flag2 = false;
        }
      }
      if (!all)
        return;
      this._notificationTable.Clear();
      this._clientHandleTable.Clear();
    }

    private uint GetNextClientHandle()
    {
      uint curHandle = this._curHandle;
      while (this._curHandle == 0U || this._clientHandleTable.ContainsKey(this._curHandle))
      {
        ++this._curHandle;
        if ((int) this._curHandle == (int) curHandle)
          return 0;
      }
      return this._curHandle++;
    }

    public override AdsErrorCode Resurrect() => (AdsErrorCode) 0;

    public override AdsErrorCode Resurrect(uint clientHandle) => (AdsErrorCode) 0;

    public override Task<ResultAds> ResurrectAsync(CancellationToken cancel) => Task.FromResult<ResultAds>(ResultAds.CreateSuccess());

    public override Task<ResultAds> ResurrectAsync(
      uint clientHandle,
      CancellationToken cancel)
    {
      return Task.FromResult<ResultAds>(ResultAds.CreateSuccess());
    }
  }
}
