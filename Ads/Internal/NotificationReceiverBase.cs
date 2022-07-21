// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.NotificationReceiverBase
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  internal abstract class NotificationReceiverBase : 
    ISyncNotificationReceiver,
    IAdsNotificationsBase,
    IAdsResurrectHandles,
    IDisposable
  {
    private Task? _errorThread;
    private ConcurrentQueue<Exception> _exceptionQueue;
    protected INotificationProvider _notificationProvider;
    /// <summary>
    /// Table Server Notification Handle --&gt; NotificationEntry
    /// </summary>
    protected ConcurrentDictionary<uint, NotificationEntry> _notificationTable;
    /// <summary>
    /// Table Client Notification Handle --&gt; Server NotificationHandle
    /// </summary>
    protected ConcurrentDictionary<uint, uint> _clientHandleTable;
    protected IHandleTable symbolTable;
    protected bool _initialized;
    protected bool _disposed;
    private IClientNotificationReceiver _notificationReceiver;
    /// <summary>Overall notification statistics.</summary>
    private ulong _notificationCount;
    /// <summary>Overall notification error statistics.</summary>
    private ulong _errorCount;
    /// <summary>Overall notification invalidates statistics.</summary>
    private ulong _unregistrationCount;
    /// <summary>Symbol Version Changed counter.</summary>
    private uint _symbolVersionChangedCount;
    /// <summary>State Changed counter.</summary>
    private uint _stateChangedCount;
    protected uint _symbolVersionServerHandle;
    protected uint _adsStateServerHandle;

    public NotificationReceiverBase(
      INotificationProvider notificationProvider,
      IHandleTable symbolTable,
      IClientNotificationReceiver notReceiver)
    {
      this.symbolTable = symbolTable;
      this._notificationProvider = notificationProvider;
      this._initialized = false;
      this._notificationTable = new ConcurrentDictionary<uint, NotificationEntry>();
      this._exceptionQueue = new ConcurrentQueue<Exception>();
      this._notificationReceiver = notReceiver;
      this._clientHandleTable = new ConcurrentDictionary<uint, uint>();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.Internal.NotificationReceiverBase" /> class.
    /// </summary>
    ~NotificationReceiverBase()
    {
      this._initialized = false;
      this.Dispose(false);
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
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
      {
        this._initialized = false;
        this.RemoveAllNotifications(true);
        if (this._adsStateServerHandle != 0U)
          this.OnAdsStateChanged(new AdsStateChangedEventArgs(StateInfo.Empty));
        this._adsStateServerHandle = 0U;
        this._symbolVersionServerHandle = 0U;
      }
      this._disposed = true;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is active/initialized
    /// </summary>
    /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
    public bool IsActive => this._initialized;

    public void OnNotification(
      AmsAddress address,
      Dictionary<DateTimeOffset, NotificationQueueElement[]> stampedNotifications)
    {
      if (AmsAddress.op_Equality(address, (AmsAddress) null) || stampedNotifications == null || stampedNotifications.Count == 0)
        return;
      foreach (KeyValuePair<DateTimeOffset, NotificationQueueElement[]> stampedNotification in stampedNotifications)
      {
        List<Notification> notificationList1 = new List<Notification>();
        List<Notification> notificationList2 = new List<Notification>();
        List<Notification> notificationList3 = new List<Notification>();
        foreach (NotificationQueueElement notificationQueueElement in stampedNotification.Value)
        {
          NotificationEntry notificationEntry1 = (NotificationEntry) null;
          if (this._notificationTable.TryGetValue(notificationQueueElement.ServerHandle, out notificationEntry1))
          {
            bool flag1 = this._adsStateServerHandle > 0U && (int) notificationQueueElement.ServerHandle == (int) this._adsStateServerHandle;
            bool flag2 = this._symbolVersionServerHandle > 0U && (int) notificationQueueElement.ServerHandle == (int) this._symbolVersionServerHandle;
            ReadOnlyMemory<byte> readOnlyMemory;
            if (notificationEntry1.CheckData(notificationQueueElement.Data))
            {
              if (flag2)
              {
                if (this._symbolVersionChangedCount > 0U)
                {
                  readOnlyMemory = notificationQueueElement.Data;
                  readOnlyMemory = readOnlyMemory.Slice(0, 1);
                  this.OnSymbolVersionChanged(new AdsSymbolVersionChangedEventArgs(readOnlyMemory.ToArray()[0]));
                }
                ++this._symbolVersionChangedCount;
              }
              else if (flag1)
              {
                ++this._stateChangedCount;
                readOnlyMemory = notificationQueueElement.Data;
                if (readOnlyMemory.Length < 4)
                {
                  this.OnAdsStateChanged(new AdsStateChangedEventArgs(StateInfo.Empty));
                }
                else
                {
                  StateInfo stateInfo;
                  ref StateInfo local = ref stateInfo;
                  readOnlyMemory = notificationQueueElement.Data;
                  readOnlyMemory = readOnlyMemory.Slice(0, 4);
                  ReadOnlySpan<byte> span = readOnlyMemory.Span;
                  // ISSUE: explicit constructor call
                  ((StateInfo) ref local).\u002Ector(span);
                  this.OnAdsStateChanged(new AdsStateChangedEventArgs(stateInfo));
                }
              }
              else
              {
                readOnlyMemory = notificationQueueElement.Data;
                if (readOnlyMemory.Length > 0)
                {
                  notificationList2.Add(new Notification(notificationEntry1.ClientHandle, stampedNotification.Key, notificationEntry1.UserData, notificationQueueElement.Data));
                }
                else
                {
                  notificationList3.Add(new Notification(notificationEntry1.ClientHandle, stampedNotification.Key, notificationEntry1.UserData, notificationQueueElement.Data));
                  NotificationEntry notificationEntry2 = (NotificationEntry) null;
                  this._notificationTable.TryRemove(notificationEntry1.ServerHandle, out notificationEntry2);
                }
              }
            }
            else
              notificationList1.Add(new Notification(notificationEntry1.ClientHandle, stampedNotification.Key, notificationEntry1.UserData, notificationQueueElement.Data));
          }
        }
        try
        {
          if (this._notificationReceiver != null)
          {
            if (notificationList2.Count > 0)
            {
              ((INotificationReceiver) this._notificationReceiver).OnNotification(stampedNotification.Key, (IList<Notification>) notificationList2);
              this._notificationCount += (ulong) notificationList2.Count;
            }
            if (notificationList1.Count > 0)
            {
              ((INotificationReceiver) this._notificationReceiver).OnNotificationError(stampedNotification.Key, (IList<Notification>) notificationList1);
              this._errorCount += (ulong) notificationList1.Count;
            }
            if (notificationList3.Count > 0)
            {
              ((INotificationReceiver) this._notificationReceiver).OnInvalidateHandles(stampedNotification.Key, (IList<Notification>) notificationList3);
              this._unregistrationCount += (ulong) notificationList3.Count;
            }
          }
        }
        catch (Exception ex)
        {
          this.OnNotificationError(ex);
        }
      }
    }

    internal AdsErrorCode RegisterSymbolVersionChangedNotification(
      NotificationSettings settings,
      int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      if (this._symbolVersionServerHandle == 0U)
      {
        uint handle = 0;
        adsErrorCode = this.TryAddDeviceNotification(61448U, 0U, 1, settings, (object) null, timeout, out handle);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
          this._symbolVersionServerHandle = this._clientHandleTable[handle];
      }
      return adsErrorCode;
    }

    internal AdsErrorCode UnregisterSymbolVersionChangedNotification(int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      if (this._symbolVersionServerHandle != 0U)
      {
        NotificationEntry notificationEntry = (NotificationEntry) null;
        if (this._notificationTable.TryGetValue(this._symbolVersionServerHandle, out notificationEntry))
        {
          adsErrorCode = this.TryDeleteDeviceNotification(notificationEntry.ClientHandle, timeout);
          this._symbolVersionServerHandle = 0U;
        }
        else
          adsErrorCode = (AdsErrorCode) 1808;
      }
      return adsErrorCode;
    }

    internal async Task<ResultAds> RegisterSymbolVersionChangedNotificationAsync(
      NotificationSettings settings,
      CancellationToken cancel)
    {
      ResultHandle resultHandle;
      if (this._symbolVersionServerHandle == 0U)
      {
        resultHandle = await this.AddDeviceNotificationAsync(61448U, 0U, 1, settings, (object) null, cancel).ConfigureAwait(false);
        if (((ResultAds) resultHandle).Succeeded)
          this._symbolVersionServerHandle = this._clientHandleTable[resultHandle.Handle];
      }
      else
      {
        NotificationEntry notificationEntry = (NotificationEntry) null;
        resultHandle = !this._notificationTable.TryGetValue(this._symbolVersionServerHandle, out notificationEntry) ? new ResultHandle((AdsErrorCode) 1812, 0U, 0U) : new ResultHandle((AdsErrorCode) 0, notificationEntry.ClientHandle, 0U);
      }
      return (ResultAds) resultHandle;
    }

    internal async Task<ResultAds> UnregisterSymbolVersionChangedNotificationAsync(
      CancellationToken cancel)
    {
      ResultAds resultAds = ResultAds.Empty;
      if (this._symbolVersionServerHandle != 0U)
      {
        NotificationEntry notificationEntry = (NotificationEntry) null;
        if (this._notificationTable.TryGetValue(this._symbolVersionServerHandle, out notificationEntry))
          resultAds = await this.DeleteDeviceNotificationAsync(notificationEntry.ClientHandle, cancel).ConfigureAwait(false);
        else
          resultAds = ResultAds.CreateError((AdsErrorCode) 1812);
        this._symbolVersionServerHandle = 0U;
      }
      return resultAds;
    }

    internal AdsErrorCode RegisterStateChangedNotification(
      NotificationSettings settings,
      int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      if (this._adsStateServerHandle == 0U)
      {
        uint handle;
        adsErrorCode = this.TryAddDeviceNotification(61696U, 0U, 4, settings, (object) null, timeout, out handle);
        if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
          this._adsStateServerHandle = this._clientHandleTable[handle];
      }
      return adsErrorCode;
    }

    internal async Task<ResultAds> RegisterStateChangedNotificationAsync(
      NotificationSettings settings,
      CancellationToken cancel)
    {
      ResultHandle resultHandle;
      if (this._adsStateServerHandle == 0U)
      {
        resultHandle = await this.AddDeviceNotificationAsync(61696U, 0U, 4, settings, (object) null, cancel).ConfigureAwait(false);
        if (((ResultAds) resultHandle).Succeeded)
          this._adsStateServerHandle = this._clientHandleTable[resultHandle.Handle];
      }
      else
      {
        NotificationEntry notificationEntry = (NotificationEntry) null;
        if (this._notificationTable.TryGetValue(this._adsStateServerHandle, out notificationEntry))
        {
          resultHandle = new ResultHandle((AdsErrorCode) 0, notificationEntry.ClientHandle, 0U);
        }
        else
        {
          resultHandle = ResultHandle.CreateError((AdsErrorCode) 1812, 0U);
          this._adsStateServerHandle = 0U;
        }
      }
      return (ResultAds) resultHandle;
    }

    internal AdsErrorCode UnregisterStateChangedNotification(int timeout)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) -1;
      if (this._adsStateServerHandle != 0U)
      {
        NotificationEntry notificationEntry = (NotificationEntry) null;
        adsErrorCode = !this._notificationTable.TryGetValue(this._adsStateServerHandle, out notificationEntry) ? (AdsErrorCode) 0 : this.TryDeleteDeviceNotification(notificationEntry.ClientHandle, timeout);
        this._adsStateServerHandle = 0U;
      }
      return adsErrorCode;
    }

    internal async Task<ResultAds> UnregisterStateChangedNotificationAsync(
      CancellationToken cancel)
    {
      ResultAds resultAds = ResultAds.Empty;
      if (this._adsStateServerHandle != 0U)
      {
        uint clientHandle = this._notificationTable[this._adsStateServerHandle].ClientHandle;
        NotificationEntry notificationEntry = (NotificationEntry) null;
        if (this._notificationTable.TryGetValue(this._adsStateServerHandle, out notificationEntry))
          resultAds = await this.DeleteDeviceNotificationAsync(notificationEntry.ClientHandle, cancel).ConfigureAwait(false);
        else
          resultAds = ResultAds.CreateSuccess();
        this._adsStateServerHandle = 0U;
      }
      return resultAds;
    }

    protected void OnSymbolVersionChanged(AdsSymbolVersionChangedEventArgs eventArgs)
    {
      this.RemoveAllNotifications(false);
      ((ISymbolVersionChangedReceiver) this._notificationReceiver).OnSymbolVersionChanged(eventArgs);
    }

    protected void OnAdsStateChanged(AdsStateChangedEventArgs eventArgs) => ((IStateChangedReceiver) this._notificationReceiver).OnAdsStateChanged(eventArgs);

    public void OnNotificationError(Exception e) => ((INotificationReceiver) this._notificationReceiver).OnNotificationError(e);

    protected abstract void RemoveAllNotifications(bool all);

    protected virtual void Init()
    {
      this._initialized = true;
      this._errorThread = Task.Factory.StartNew(new Action(this.ErrorThread), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    protected bool OnAddNotification(NotificationEntry notification)
    {
      if (notification == null)
        throw new ArgumentNullException(nameof (notification));
      bool flag = this._notificationTable.TryAdd(notification.ServerHandle, notification);
      this._clientHandleTable.TryAdd(notification.ClientHandle, notification.ServerHandle);
      return flag;
    }

    protected virtual NotificationEntry? OnDeleteNotification(
      uint clientHandle,
      CancellationToken cancel,
      out AdsErrorCode result)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref result = 0;
      uint key = 0;
      if (!this._clientHandleTable.TryRemove(clientHandle, out key))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref result = 1874;
        return (NotificationEntry) null;
      }
      NotificationEntry notificationEntry = (NotificationEntry) null;
      if (this._notificationTable.TryRemove(key, out notificationEntry))
        return notificationEntry;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref result = 1874;
      return (NotificationEntry) null;
    }

    protected abstract uint OnAddNotification(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      int timeout,
      out AdsErrorCode result);

    /// <summary>
    /// Connects a variable to the ADS client. The ADS client will be notified by the AdsNotification event.
    /// </summary>
    /// <param name="indexGroup">Contains the index group number of the requested ADS service.</param>
    /// <param name="indexOffset">Contains the index offset number of the requested ADS service.</param>
    /// <param name="dataSize">Maximum amount of data in bytes to receive with this ADS Notification.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="userData">This object can be used to store user specific data.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>The handle of the notification.</returns>
    public abstract Task<ResultHandle> AddDeviceNotificationAsync(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      CancellationToken cancel);

    /// <summary>Deletes an existing notification.</summary>
    /// <param name="notificationHandle">Handle of the notification.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;TaskResult&gt;.</returns>
    public abstract Task<ResultAds> DeleteDeviceNotificationAsync(
      uint notificationHandle,
      CancellationToken cancel);

    private void ErrorThread()
    {
    }

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
    public AdsErrorCode TryAddDeviceNotification(
      uint indexGroup,
      uint indexOffset,
      int dataSize,
      NotificationSettings settings,
      object? userData,
      int timeout,
      out uint handle)
    {
      AdsErrorCode result;
      handle = this.OnAddNotification(indexGroup, indexOffset, dataSize, settings, userData, timeout, out result);
      return result;
    }

    public AdsErrorCode TryDeleteDeviceNotification(uint clientHandle, int timeout)
    {
      AdsErrorCode result;
      this.OnDeleteNotification(clientHandle, CancellationToken.None, out result);
      return result;
    }

    public abstract AdsErrorCode Resurrect();

    public abstract AdsErrorCode Resurrect(uint clientHandle);

    public abstract Task<ResultAds> ResurrectAsync(CancellationToken cancel);

    public abstract Task<ResultAds> ResurrectAsync(
      uint clientHandle,
      CancellationToken cancel);
  }
}
