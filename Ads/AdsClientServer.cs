// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.AdsClientServer
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.Ads.Server;
using TwinCAT.Ams;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Server implementation for the <see cref="T:TwinCAT.Ads.AdsClient" />.
  /// Implements the <see cref="T:TwinCAT.Ads.Server.AdsServer" />
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.Server.AdsServer" />
  /// <exclude />
  internal class AdsClientServer : AdsServer
  {
    /// <summary>The notification receiver</summary>
    private NotificationReceiverBase _notificationReceiver;
    private IRouterNotificationReceiver _routerNotifications;
    /// <summary>The notification scheduler</summary>
    private EventLoopScheduler? _notificationScheduler;
    private static int s_invokeId;
    /// <summary>
    /// Invoke Dictionary InvokeID --&gt;&gt; IRequestCompletionSource
    /// </summary>
    private ConcurrentDictionary<uint, IRequestCompletionSource> _invokeIdDict = new ConcurrentDictionary<uint, IRequestCompletionSource>();
    /// <summary>
    /// Invoke Dictionary InvokeID --&gt;&gt; ManuelResetEventSlim
    /// </summary>
    private ConcurrentDictionary<uint, ManualResetEventSlim> _invokeIdSync = new ConcurrentDictionary<uint, ManualResetEventSlim>();
    /// <summary>Default Confirmation Timeout (2 min)</summary>
    private static int s_confirmationTimeout = 120000;

    internal AdsClientServer(
      NotificationReceiverBase notRec,
      IRouterNotificationReceiver routerNotifications,
      ILogger? logger)
      : base("AdsClient", logger)
    {
      this._notificationReceiver = notRec;
      this._routerNotifications = routerNotifications;
    }

    /// <summary>
    /// Handler function that is called, when the <see cref="T:TwinCAT.Ads.Server.AdsServer" /> is connected.
    /// </summary>
    protected virtual void OnConnected()
    {
      this._notificationScheduler = new EventLoopScheduler((Func<ThreadStart, Thread>) (ts =>
      {
        Thread thread = new Thread(ts);
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
        interpolatedStringHandler.AppendLiteral("$NotificationLoop_");
        interpolatedStringHandler.AppendFormatted<int?>(this.ServerAddress?.Port);
        thread.Name = interpolatedStringHandler.ToStringAndClear();
        thread.IsBackground = true;
        return thread;
      }));
      base.OnConnected();
    }

    /// <summary>
    /// Called when the <see cref="T:TwinCAT.Ads.Server.AdsServer" /> is about to be disconnected.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected virtual bool OnDisconnect()
    {
      if (this._notificationScheduler != null)
        this._notificationScheduler.Dispose();
      this._notificationScheduler = (EventLoopScheduler) null;
      return base.OnDisconnect();
    }

    protected virtual void OnServerConnectionStateChanged(
      object? sender,
      ServerConnectionStateChangedEventArgs e)
    {
      base.OnServerConnectionStateChanged(sender, e);
    }

    protected virtual Task<ResultAds> OnDeviceNotificationAsync(
      AmsAddress sender,
      NotificationSamplesStamp[] stampHeaders,
      CancellationToken cancel)
    {
      if (!this.IsDisconnecting && this.IsConnected && stampHeaders.Length != 0)
      {
        Dictionary<DateTimeOffset, NotificationQueueElement[]> orgElements = new Dictionary<DateTimeOffset, NotificationQueueElement[]>();
        for (int index1 = 0; index1 < stampHeaders.Length; ++index1)
        {
          NotificationSamplesStamp stampHeader = stampHeaders[index1];
          DateTimeOffset timeStamp = stampHeader.TimeStamp;
          NotificationQueueElement[] notificationQueueElementArray = new NotificationQueueElement[stampHeader.SamplesCount];
          for (int index2 = 0; index2 < stampHeader.SamplesCount; ++index2)
          {
            NotificationDataSample notificationSample = stampHeader.NotificationSamples[index2];
            uint notificationHandle = notificationSample.NotificationHandle;
            byte[] array = notificationSample.SampleData.ToArray();
            int sampleSize = notificationSample.SampleSize;
            notificationQueueElementArray[index2] = new NotificationQueueElement(notificationHandle, timeStamp, (ReadOnlyMemory<byte>) array.AsMemory<byte>());
          }
          orgElements.Add(timeStamp, notificationQueueElementArray);
        }
        if (!this.IsDisconnecting && this.IsConnected && this._notificationScheduler != null)
          this._notificationScheduler.Schedule(new Action(sendNotification));

        void sendNotification() => this._notificationReceiver.OnNotification(sender, orgElements);
      }
      return Task.FromResult<ResultAds>(ResultAds.CreateSuccess());
    }

    /// <summary>Request as an asynchronous operation.</summary>
    /// <typeparam name="S">The Request completion source.</typeparam>
    /// <typeparam name="R">The communication result (ADS Result)</typeparam>
    /// <param name="createCompletionSource">The create completion source.</param>
    /// <param name="request">The request.</param>
    /// <param name="createDefaultResult">The create default result.</param>
    /// <param name="confirmResult">Called when the Result is ready (after received Ads confirmation) to confirm the result.</param>
    /// <param name="timeout">The timeout in ms.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;R&gt;.</returns>
    internal async Task<R> RequestAsync<S, R>(
      Func<uint, S> createCompletionSource,
      Func<uint, Task<AdsErrorCode>> request,
      Func<AdsErrorCode, uint, R> createDefaultResult,
      Action<R>? confirmResult,
      int timeout,
      CancellationToken cancel)
      where S : TaskCompletionSource<R>, IRequestCompletionSource
      where R : ResultAds
    {
      AdsClientServer adsClientServer1 = this;
      int managedThreadId = Thread.CurrentThread.ManagedThreadId;
      uint invokeId = (uint) Interlocked.Increment(ref AdsClientServer.s_invokeId);
      S taskCompletionSource = createCompletionSource(invokeId);
      adsClientServer1._invokeIdDict[invokeId] = (IRequestCompletionSource) taskCompletionSource;
      bool timedOut = false;
      R adsResult;
      try
      {
        AdsErrorCode adsErrorCode = await request(invokeId).ConfigureAwait(false);
        if (adsErrorCode == null)
        {
          if (timeout > 0)
          {
            Task delay = Task.Delay(timeout, cancel);
            if (await Task.WhenAny((Task) taskCompletionSource.Task, delay).ConfigureAwait(false) != delay)
              LogProviderExtension.LogDebug((ILoggerProvider) adsClientServer1, "RequestAsync: InvokeID: '{0}' succeeded!", new object[1]
              {
                (object) invokeId
              });
            else if (cancel.IsCancellationRequested)
            {
              LogProviderExtension.LogInformation((ILoggerProvider) adsClientServer1, "RequestAsync: InvokeID: '{0}' cancellation triggered!", new object[1]
              {
                (object) invokeId
              });
              adsResult = createDefaultResult((AdsErrorCode) 1878, invokeId);
              taskCompletionSource.TrySetResult(adsResult);
            }
            else
            {
              timedOut = true;
              LogProviderExtension.LogWarning((ILoggerProvider) adsClientServer1, "RequestAsync: InvokeID: '{0}' Timeout after '{1} ms' waiting triggered!", new object[2]
              {
                (object) invokeId,
                (object) timeout
              });
              adsResult = createDefaultResult((AdsErrorCode) 1861, invokeId);
              taskCompletionSource.TrySetResult(adsResult);
            }
            delay = (Task) null;
          }
          else
          {
            Task task = (Task) taskCompletionSource.Task;
          }
          LogProviderExtension.LogTrace((ILoggerProvider) adsClientServer1, "RequestAsync: Waiting for Request InvokeID: {0} confirmation (Timeout: '{1}')", new object[2]
          {
            (object) invokeId,
            (object) timeout
          });
          adsResult = await taskCompletionSource.Task.ConfigureAwait(false);
        }
        else
        {
          adsResult = createDefaultResult(adsErrorCode, invokeId);
          LogProviderExtension.LogWarning((ILoggerProvider) adsClientServer1, "RequestAsync: InvokeID: '{0}' failed with error: {1}!", new object[2]
          {
            (object) invokeId,
            (object) ((ResultAds) (object) adsResult).ErrorCode
          });
          taskCompletionSource.TrySetResult(adsResult);
        }
      }
      catch (OperationCanceledException ex)
      {
        LogProviderExtension.LogDebug((ILoggerProvider) adsClientServer1, "RequestAsync: Cancellation received for Request InvokeID '{0}'", new object[1]
        {
          (object) invokeId
        });
        timedOut = true;
        adsResult = createDefaultResult((AdsErrorCode) 1878, invokeId);
        taskCompletionSource.TrySetResult(adsResult);
      }
      finally
      {
        IRequestCompletionSource completionSource = (IRequestCompletionSource) null;
        if (!adsClientServer1._invokeIdDict.TryRemove(invokeId, out completionSource))
          LogProviderExtension.LogError((ILoggerProvider) adsClientServer1, "RequestAsync: InvokeId '{0}' not found!", new object[1]
          {
            (object) invokeId
          });
        else if (timedOut)
        {
          AdsClientServer adsClientServer2 = adsClientServer1;
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 4);
          interpolatedStringHandler.AppendLiteral("RequestAsync: Removing Timed out InvokeID '");
          interpolatedStringHandler.AppendFormatted<int>(0);
          interpolatedStringHandler.AppendLiteral("' (");
          interpolatedStringHandler.AppendFormatted<int>(1);
          interpolatedStringHandler.AppendLiteral(", Timeout: ");
          interpolatedStringHandler.AppendFormatted<int>(2);
          interpolatedStringHandler.AppendLiteral(", Outstanding: ");
          interpolatedStringHandler.AppendFormatted<int>(3);
          interpolatedStringHandler.AppendLiteral(")");
          string stringAndClear = interpolatedStringHandler.ToStringAndClear();
          object[] objArray = new object[4]
          {
            (object) invokeId,
            (object) completionSource,
            (object) timeout,
            (object) adsClientServer1._invokeIdDict.Count
          };
          LogProviderExtension.LogWarning((ILoggerProvider) adsClientServer2, stringAndClear, objArray);
        }
      }
      if (confirmResult != null)
        confirmResult(adsResult);
      await Task.Yield();
      R r = adsResult;
      adsResult = default (R);
      taskCompletionSource = default (S);
      return r;
    }

    /// <summary>
    /// Called when an ADS Read Write confirmation is received.
    /// Overwrite this method in derived classes to react on ADS Read Write confirmations.
    /// </summary>
    /// <param name="target">The target address.</param>
    /// <param name="invokeId">The invoke id provided by this server during the corresponding request</param>
    /// <param name="result">The ADS error code provided by the sender</param>
    /// <param name="name">The name.</param>
    /// <param name="version">The version.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous <see cref="M:TwinCAT.Ads.AdsClientServer.OnReadWriteConfirmationAsync(TwinCAT.Ads.AmsAddress,System.UInt32,TwinCAT.Ads.AdsErrorCode,System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" /> operation. The <see cref="T:System.Threading.Tasks.Task`1" /> parameter contains the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> as
    /// <see cref="P:System.Threading.Tasks.Task`1.Result" />.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual async Task<AdsErrorCode> OnReadDeviceInfoConfirmationAsync(
      AmsAddress target,
      uint invokeId,
      AdsErrorCode result,
      string name,
      AdsVersion version,
      CancellationToken cancel)
    {
      AdsClientServer adsClientServer = this;
      AdsErrorCode adsErrorCode1 = (AdsErrorCode) -1;
      IRequestCompletionSource tcs = (IRequestCompletionSource) null;
      if (adsClientServer._invokeIdDict.TryGetValue(invokeId, out tcs))
      {
        // ISSUE: reference to a compiler-generated method
        adsErrorCode1 = await adsClientServer.\u003C\u003En__0(target, invokeId, result, name, version, cancel).ConfigureAwait(false);
        ((TaskCompletionSource<ResultDeviceInfo>) tcs).SetResult(new ResultDeviceInfo(result, name, version, invokeId));
      }
      else
        LogProviderExtension.LogDebug((ILoggerProvider) adsClientServer, "OnReadDeviceInfoConfirmationAsync(InvokeID: {0}) ignored, because timed out!", new object[1]
        {
          (object) invokeId
        });
      AdsErrorCode adsErrorCode2 = adsErrorCode1;
      tcs = (IRequestCompletionSource) null;
      return adsErrorCode2;
    }

    /// <summary>
    /// Sending an Request synchronously and waiting for the result.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="createCompletionSource">The create completion source.</param>
    /// <param name="request">The request.</param>
    /// <param name="createDefaultResult">The create default result.</param>
    /// <param name="confirmResult">Called when the Result is ready (after received Ads confirmation) to confirm the result.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>Task&lt;R&gt;.</returns>
    internal R RequestAndReceiveSync<S, R>(
      Func<uint, S> createCompletionSource,
      Func<uint, AdsErrorCode> request,
      Func<AdsErrorCode, uint, R> createDefaultResult,
      Action<R>? confirmResult,
      int timeout)
      where S : TaskCompletionSource<R>, IRequestCompletionSource
      where R : ResultAds
    {
      if (Thread.CurrentThread.ManagedThreadId == this.AmsServer.InternalImplementation.FrameReceiverThreadId)
        throw new AdsErrorException("Cannot run AmsRequest on ReceiveLoop Thread!", (AdsErrorCode) 1872);
      ManualResetEventSlim manualResetEventSlim1 = new ManualResetEventSlim(false);
      uint key = (uint) Interlocked.Increment(ref AdsClientServer.s_invokeId);
      S s = createCompletionSource(key);
      this._invokeIdDict[key] = (IRequestCompletionSource) s;
      this._invokeIdSync[key] = manualResetEventSlim1;
      bool flag1 = false;
      if (timeout <= 0)
        timeout = AdsClientServer.s_confirmationTimeout;
      R result;
      try
      {
        AdsErrorCode adsErrorCode = request(key);
        if (adsErrorCode == null)
        {
          TimeSpan timeout1 = DateTimeOffset.Now + TimeSpan.FromMilliseconds((double) timeout) - DateTimeOffset.Now;
          bool flag2 = false;
          bool flag3 = false;
          flag1 = timeout1 <= TimeSpan.Zero;
          while (!flag2 && !flag1)
          {
            flag1 = !manualResetEventSlim1.Wait(timeout1);
            flag2 = s.Task.IsCompleted;
            flag3 = s.Task.IsCanceled;
          }
          if (flag2)
          {
            result = s.Task.Result;
            if (!flag3)
              LogProviderExtension.LogDebug((ILoggerProvider) this, "RequestAndReceiveSync: InvokeID:'{0}' succeeded!", new object[1]
              {
                (object) key
              });
            else
              LogProviderExtension.LogDebug((ILoggerProvider) this, "RequestAndReceiveSync: InvokeId:'{0}', Cancelled or failed!", new object[1]
              {
                (object) key
              });
          }
          else
          {
            flag1 = true;
            LogProviderExtension.LogDebug((ILoggerProvider) this, "RequestAndReceiveSync: InvokeID: '{0}' Timeout after '{1} ms' waiting triggered!", new object[2]
            {
              (object) key,
              (object) timeout
            });
            result = createDefaultResult((AdsErrorCode) 1861, key);
            s.TrySetResult(result);
          }
        }
        else
        {
          result = createDefaultResult(adsErrorCode, key);
          LogProviderExtension.LogWarning((ILoggerProvider) this, "RequestAndReceiveSync: InvokeID: '{0}' failed with error: {1}!", new object[2]
          {
            (object) key,
            (object) ((ResultAds) (object) result).ErrorCode
          });
          s.TrySetResult(result);
        }
      }
      catch (OperationCanceledException ex)
      {
        LogProviderExtension.LogDebug((ILoggerProvider) this, "RequestAndReceiveSync: Cancellation received for Request InvokeID '{0}'", new object[1]
        {
          (object) key
        });
        flag1 = true;
        result = createDefaultResult((AdsErrorCode) 1878, key);
        s.TrySetResult(result);
      }
      finally
      {
        IRequestCompletionSource completionSource = (IRequestCompletionSource) null;
        manualResetEventSlim1.Dispose();
        bool flag4 = this._invokeIdDict.TryRemove(key, out completionSource);
        ManualResetEventSlim manualResetEventSlim2 = (ManualResetEventSlim) null;
        if (this._invokeIdSync.TryRemove(key, out manualResetEventSlim2))
          manualResetEventSlim2.Dispose();
        if (!flag4)
          LogProviderExtension.LogError((ILoggerProvider) this, "RequestAndReceiveSync: InvokeId '{0}' not found!", new object[1]
          {
            (object) key
          });
        else if (flag1)
          LogProviderExtension.LogWarning((ILoggerProvider) this, "RequestAndReceiveSync: Removing Timed out InvokeID '{0}' ({1}, Timeout: {2})", new object[3]
          {
            (object) key,
            (object) completionSource,
            (object) timeout
          });
      }
      if (confirmResult != null)
        confirmResult(result);
      return result;
    }

    /// <summary>
    /// Sends the AdsRequest for a ReadState operation asynchronously
    /// </summary>
    /// <param name="readStateRequest">The inner request / operation.</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The Request timeout.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;TaskResultReadState&gt;.</returns>
    internal async Task<ResultReadDeviceState> RequestReadDeviceStateAsync(
      Func<uint, Task<AdsErrorCode>> readStateRequest,
      Action<ResultReadDeviceState>? confirmResult,
      int timeout,
      CancellationToken cancel)
    {
      Func<uint, ReadDeviceStateRequestCompletionSource> createCompletionSource = (Func<uint, ReadDeviceStateRequestCompletionSource>) (id => new ReadDeviceStateRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultReadDeviceState> createDefaultResult = (Func<AdsErrorCode, uint, ResultReadDeviceState>) ((error, id) => new ResultReadDeviceState(error, new StateInfo(), id));
      return await this.RequestAsync<ReadDeviceStateRequestCompletionSource, ResultReadDeviceState>(createCompletionSource, readStateRequest, createDefaultResult, confirmResult, timeout, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the AdsRequest for a ReadDeviceState operation (synchronous)
    /// </summary>
    /// <param name="timeout">The Request timeout.</param>
    /// <param name="readStateRequest">The inner request / operation.</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <returns>Task&lt;TaskResultReadState&gt;.</returns>
    internal ResultReadDeviceState RequestReadDeviceState(
      Func<uint, AdsErrorCode> readStateRequest,
      Action<ResultReadDeviceState>? confirmResult,
      int timeout)
    {
      Func<uint, ReadDeviceStateRequestCompletionSource> createCompletionSource = (Func<uint, ReadDeviceStateRequestCompletionSource>) (id => new ReadDeviceStateRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultReadDeviceState> createDefaultResult = (Func<AdsErrorCode, uint, ResultReadDeviceState>) ((error, id) => new ResultReadDeviceState(error, new StateInfo(), id));
      return this.RequestAndReceiveSync<ReadDeviceStateRequestCompletionSource, ResultReadDeviceState>(createCompletionSource, readStateRequest, createDefaultResult, confirmResult, timeout);
    }

    /// <summary>
    /// Sends the AdsRequest for a ReadDeviceInfo operation (asynchronously)
    /// </summary>
    /// <param name="readStateRequest">The inner request / operation.</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The Request timeout.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;TaskResultReadState&gt;.</returns>
    internal async Task<ResultDeviceInfo> RequestReadDeviceInfoAsync(
      Func<uint, Task<AdsErrorCode>> readStateRequest,
      Action<ResultDeviceInfo> confirmResult,
      int timeout,
      CancellationToken cancel)
    {
      Func<uint, ReadDeviceInfoRequestCompletionSource> createCompletionSource = (Func<uint, ReadDeviceInfoRequestCompletionSource>) (id => new ReadDeviceInfoRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultDeviceInfo> createDefaultResult = (Func<AdsErrorCode, uint, ResultDeviceInfo>) ((error, id) => new ResultDeviceInfo(error, string.Empty, AdsVersion.Empty, id));
      return await this.RequestAsync<ReadDeviceInfoRequestCompletionSource, ResultDeviceInfo>(createCompletionSource, readStateRequest, createDefaultResult, confirmResult, timeout, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the AdsRequest for a ReadDeviceInfo operation (Synchronously)
    /// </summary>
    /// <param name="readStateRequest">The inner request / operation.</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The Request timeout.</param>
    /// <returns>Task&lt;TaskResultReadState&gt;.</returns>
    internal ResultDeviceInfo RequestReadDeviceInfoSync(
      Func<uint, AdsErrorCode> readStateRequest,
      Action<ResultDeviceInfo> confirmResult,
      int timeout)
    {
      Func<uint, ReadDeviceInfoRequestCompletionSource> createCompletionSource = (Func<uint, ReadDeviceInfoRequestCompletionSource>) (id => new ReadDeviceInfoRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultDeviceInfo> createDefaultResult = (Func<AdsErrorCode, uint, ResultDeviceInfo>) ((error, id) => new ResultDeviceInfo(error, string.Empty, AdsVersion.Empty, id));
      return this.RequestAndReceiveSync<ReadDeviceInfoRequestCompletionSource, ResultDeviceInfo>(createCompletionSource, readStateRequest, createDefaultResult, confirmResult, timeout);
    }

    /// <summary>Sends an asynchronous read request</summary>
    /// <param name="readRequest">The inner request operation.</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;TaskResultRead&gt;.</returns>
    private async Task<ResultRead> RequestReadAsync(
      Func<uint, Task<AdsErrorCode>> readRequest,
      Action<ResultRead> confirmResult,
      int timeout,
      CancellationToken cancel)
    {
      Func<uint, ReadRequestCompletionSource> createCompletionSource = (Func<uint, ReadRequestCompletionSource>) (id => new ReadRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultRead> createDefaultResult = (Func<AdsErrorCode, uint, ResultRead>) ((error, id) => new ResultRead(error, 0, id));
      return await this.RequestAsync<ReadRequestCompletionSource, ResultRead>(createCompletionSource, readRequest, createDefaultResult, confirmResult, timeout, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an asynchronous AdsRequest for a Simple Ads Call (without return value, just AdsErrorCode)
    /// </summary>
    /// <param name="request">The inner request/operation.</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The Request timeout.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;TaskResult&gt;.</returns>
    internal async Task<ResultAds> RequestAsync(
      Func<uint, Task<AdsErrorCode>> request,
      Action<ResultAds> confirmResult,
      int timeout,
      CancellationToken cancel)
    {
      Func<uint, AdsRequestCompletionSource> createCompletionSource = (Func<uint, AdsRequestCompletionSource>) (id => new AdsRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultAds> createDefaultResult = (Func<AdsErrorCode, uint, ResultAds>) ((error, id) => ResultAds.CreateError(error, id));
      return await this.RequestAsync<AdsRequestCompletionSource, ResultAds>(createCompletionSource, request, createDefaultResult, confirmResult, timeout, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an Request for a simple Ads Call (without return value, just AdsErrorCode) and waits for the response (synchronous)
    /// </summary>
    /// <param name="request">The inner request/operation.</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The Request timeout.</param>
    /// <returns>Task&lt;TaskResult&gt;.</returns>
    internal ResultAds RequestAndReceiveSync(
      Func<uint, AdsErrorCode> request,
      Action<ResultAds> confirmResult,
      int timeout)
    {
      Func<uint, AdsRequestCompletionSource> createCompletionSource = (Func<uint, AdsRequestCompletionSource>) (id => new AdsRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultAds> createDefaultResult = (Func<AdsErrorCode, uint, ResultAds>) ((error, id) => ResultAds.CreateError(error, id));
      return this.RequestAndReceiveSync<AdsRequestCompletionSource, ResultAds>(createCompletionSource, request, createDefaultResult, confirmResult, timeout);
    }

    /// <summary>
    /// Sends the AdsRequest asynchronously for operations that return data (Read, ReadWrite requests)
    /// </summary>
    /// <param name="readRequest">The inner read / readWrite operation</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>Task&lt;TaskResultReadBytes&gt;.</returns>
    internal async Task<ResultReadBytes> RequestReadBytesAsync(
      Func<uint, Task<AdsErrorCode>> readRequest,
      Action<ResultReadBytes> confirmResult,
      int timeout,
      CancellationToken cancel)
    {
      Func<uint, ReadBytesRequestCompletionSource> createCompletionSource = (Func<uint, ReadBytesRequestCompletionSource>) (id => new ReadBytesRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultReadBytes> createDefaultResult = (Func<AdsErrorCode, uint, ResultReadBytes>) ((error, id) => new ResultReadBytes(error, (ReadOnlyMemory<byte>) Memory<byte>.Empty, id));
      return await this.RequestAsync<ReadBytesRequestCompletionSource, ResultReadBytes>(createCompletionSource, readRequest, createDefaultResult, confirmResult, timeout, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the AdsRequest and receives synchronously return data (Read, ReadWrite requests)
    /// </summary>
    /// <param name="readRequest">The inner read / readWrite operation</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>Task&lt;TaskResultReadBytes&gt;.</returns>
    internal ResultReadBytes RequestAndReceiveReadBytesSync(
      Func<uint, AdsErrorCode> readRequest,
      Action<ResultReadBytes> confirmResult,
      int timeout)
    {
      Func<uint, ReadBytesRequestCompletionSource> createCompletionSource = (Func<uint, ReadBytesRequestCompletionSource>) (id => new ReadBytesRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultReadBytes> createDefaultResult = (Func<AdsErrorCode, uint, ResultReadBytes>) ((error, id) => new ResultReadBytes(error, (ReadOnlyMemory<byte>) Memory<byte>.Empty, id));
      return this.RequestAndReceiveSync<ReadBytesRequestCompletionSource, ResultReadBytes>(createCompletionSource, readRequest, createDefaultResult, confirmResult, timeout);
    }

    /// <summary>
    /// Sends the AdsRequest for an operation that returns ADS handles (asynchronously)
    /// </summary>
    /// <param name="handleRequest">The handle request operation</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;TaskResultHandle&gt;.</returns>
    internal async Task<ResultHandle> RequestHandleAsync(
      Func<uint, Task<AdsErrorCode>> handleRequest,
      Action<ResultHandle> confirmResult,
      int timeout,
      CancellationToken cancel)
    {
      Func<uint, HandleRequestCompletionSource> createCompletionSource = (Func<uint, HandleRequestCompletionSource>) (id => new HandleRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultHandle> createDefaultResult = (Func<AdsErrorCode, uint, ResultHandle>) ((error, id) => new ResultHandle(error, 0U, id));
      return await this.RequestAsync<HandleRequestCompletionSource, ResultHandle>(createCompletionSource, handleRequest, createDefaultResult, confirmResult, timeout, cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the AdsRequest and receive synchronously for an operation that returns ADS handles.
    /// </summary>
    /// <param name="handleRequest">The handle request operation</param>
    /// <param name="confirmResult">The confirm result.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>Task&lt;TaskResultHandle&gt;.</returns>
    internal ResultHandle RequestAndReceiveHandleSync(
      Func<uint, AdsErrorCode> handleRequest,
      Action<ResultHandle> confirmResult,
      int timeout)
    {
      Func<uint, HandleRequestCompletionSource> createCompletionSource = (Func<uint, HandleRequestCompletionSource>) (id => new HandleRequestCompletionSource(id));
      Func<AdsErrorCode, uint, ResultHandle> createDefaultResult = (Func<AdsErrorCode, uint, ResultHandle>) ((error, id) => new ResultHandle(error, 0U, id));
      return this.RequestAndReceiveSync<HandleRequestCompletionSource, ResultHandle>(createCompletionSource, handleRequest, createDefaultResult, confirmResult, timeout);
    }

    /// <summary>
    /// Confirms the outstanding request with the specified invokeId
    /// </summary>
    /// <typeparam name="S">The type of the TaskCompletionSource"/&gt;</typeparam>
    /// <typeparam name="R">The <see cref="T:TwinCAT.Ads.ResultAds" /> type.</typeparam>
    /// <param name="sourceAddress">Source address of the confirmation.</param>
    /// <param name="invokeId">Source address of the confirmation.</param>
    /// <param name="result">The TaskResult</param>
    /// <returns><c>true</c> if the the request is confirmed, otherwise false (Timeout???)</returns>
    private bool ConfirmRequest<S, R>(AmsAddress sourceAddress, uint invokeId, R result)
      where S : TaskCompletionSource<R>, IRequestCompletionSource
      where R : ResultAds
    {
      if (sourceAddress == null)
        throw new ArgumentNullException(nameof (sourceAddress));
      bool flag = false;
      IRequestCompletionSource completionSource = (IRequestCompletionSource) null;
      if (this._invokeIdDict.TryGetValue(invokeId, out completionSource))
      {
        ((S) completionSource).SetResult(result);
        ManualResetEventSlim manualResetEventSlim = (ManualResetEventSlim) null;
        if (this._invokeIdSync.TryGetValue(invokeId, out manualResetEventSlim))
          manualResetEventSlim.Set();
        flag = true;
      }
      else
        LogProviderExtension.LogWarning((ILoggerProvider) this, "ConfirmRequest: Confirmation for InvokeID: '{0}' ignored, already removed because it was to late?", new object[1]
        {
          (object) invokeId
        });
      return flag;
    }

    /// <summary>
    /// Confirms the outstanding Read Bytes request with the specified invokeID.
    /// </summary>
    /// <param name="sourceAddress">The source Address.</param>
    /// <param name="invokeId">The invoke identifier.</param>
    /// <param name="result">The result.</param>
    private bool ConfirmRequest(AmsAddress sourceAddress, uint invokeId, ResultReadBytes result) => this.ConfirmRequest<ReadBytesRequestCompletionSource, ResultReadBytes>(sourceAddress, invokeId, result);

    /// <summary>
    /// Confirms the outstanding ReadState-Request with the specified invoke id.
    /// </summary>
    /// <param name="sourceAddress">The source address.</param>
    /// <param name="invokeId">The invoke identifier.</param>
    /// <param name="result">The result.</param>
    private bool ConfirmRequest(
      AmsAddress sourceAddress,
      uint invokeId,
      ResultReadDeviceState result)
    {
      return this.ConfirmRequest<ReadDeviceStateRequestCompletionSource, ResultReadDeviceState>(sourceAddress, invokeId, result);
    }

    /// <summary>
    /// Confirms the outstanding Handle-Request with the specified invoke id.
    /// </summary>
    /// <param name="sourceAddress">The source address.</param>
    /// <param name="invokeId">The invoke identifier.</param>
    /// <param name="result">The result.</param>
    private bool ConfirmRequest(AmsAddress sourceAddress, uint invokeId, ResultHandle result) => this.ConfirmRequest<HandleRequestCompletionSource, ResultHandle>(sourceAddress, invokeId, result);

    /// <summary>
    /// Confirms the outstanding Request with the specified invoke id.
    /// </summary>
    /// <param name="sourceAddress">The source address.</param>
    /// <param name="invokeId">The invoke identifier.</param>
    /// <param name="result">The result.</param>
    private bool ConfirmRequest(AmsAddress sourceAddress, uint invokeId, ResultAds result) => this.ConfirmRequest<AdsRequestCompletionSource, ResultAds>(sourceAddress, invokeId, result);

    /// <summary>
    /// Called when an ADS Read State confirmation is received.
    /// Overwrite this method in derived classes to react on ADS Read State confirmations.
    /// </summary>
    /// <param name="target">The sender's AMS address</param>
    /// <param name="invokeId">The invoke id provided by this server during the corresponding request</param>
    /// <param name="result">The ADS error code provided by the sender</param>
    /// <param name="adsState">The ADS state of the sender</param>
    /// <param name="deviceState">The device state of the sender</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous <see cref="M:TwinCAT.Ads.AdsClientServer.OnReadDeviceStateConfirmationAsync(TwinCAT.Ads.AmsAddress,System.UInt32,TwinCAT.Ads.AdsErrorCode,TwinCAT.Ads.AdsState,System.UInt16,System.Threading.CancellationToken)" /> operation. The <see cref="T:System.Threading.Tasks.Task`1" /> parameter contains the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> as
    /// <see cref="P:System.Threading.Tasks.Task`1.Result" />.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual async Task<AdsErrorCode> OnReadDeviceStateConfirmationAsync(
      AmsAddress target,
      uint invokeId,
      AdsErrorCode result,
      AdsState adsState,
      ushort deviceState,
      CancellationToken cancel)
    {
      ResultReadDeviceState resultState = new ResultReadDeviceState(result, new StateInfo(adsState, (short) deviceState), invokeId);
      AdsErrorCode adsErrorCode1 = await base.OnReadDeviceStateConfirmationAsync(target, invokeId, result, adsState, deviceState, cancel).ConfigureAwait(false);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode1))
        this.ConfirmRequest(target, invokeId, resultState);
      AdsErrorCode adsErrorCode2 = adsErrorCode1;
      resultState = (ResultReadDeviceState) null;
      return adsErrorCode2;
    }

    /// <summary>
    /// Called when an ADS Read confirmation is received.
    /// Overwrite this method in derived classes to react on ADS Read confirmations.
    /// </summary>
    /// <param name="sender">The sender's AMS address</param>
    /// <param name="invokeId">The invoke id provided by this server during the corresponding request</param>
    /// <param name="errorCode">The ADS error code provided by the sender</param>
    /// <param name="readData">The read data buffer</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous <see cref="M:TwinCAT.Ads.AdsClientServer.OnReadConfirmationAsync(TwinCAT.Ads.AmsAddress,System.UInt32,TwinCAT.Ads.AdsErrorCode,System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" /> operation. The <see cref="T:System.Threading.Tasks.Task`1" /> parameter contains the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> as
    /// <see cref="P:System.Threading.Tasks.Task`1.Result" />.
    /// </returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual async Task<AdsErrorCode> OnReadConfirmationAsync(
      AmsAddress sender,
      uint invokeId,
      AdsErrorCode errorCode,
      ReadOnlyMemory<byte> readData,
      CancellationToken cancel)
    {
      AdsErrorCode adsErrorCode = await base.OnReadConfirmationAsync(sender, invokeId, errorCode, readData, cancel).ConfigureAwait(false);
      ResultReadBytes result = new ResultReadBytes(errorCode, readData, invokeId);
      this.ConfirmRequest(sender, invokeId, result);
      return ((ResultAds) result).ErrorCode;
    }

    /// <summary>Called when an ADS Write confirmation is received.</summary>
    /// <remarks>
    /// Overwrite this method in derived classes to react on ADS Write confirmations.
    /// </remarks>
    /// <param name="target">The sender's AMS address</param>
    /// <param name="invokeId">The invoke id provided by this server during the corresponding request</param>
    /// <param name="errorCode">The ADS error code provided by the sender</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous <see cref="M:TwinCAT.Ads.AdsClientServer.OnWriteConfirmationAsync(TwinCAT.Ads.AmsAddress,System.UInt32,TwinCAT.Ads.AdsErrorCode,System.Threading.CancellationToken)" /> operation. The <see cref="T:System.Threading.Tasks.Task`1" /> parameter contains the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> as
    /// <see cref="P:System.Threading.Tasks.Task`1.Result" />.</returns>
    protected virtual async Task<AdsErrorCode> OnWriteConfirmationAsync(
      AmsAddress target,
      uint invokeId,
      AdsErrorCode errorCode,
      CancellationToken cancel)
    {
      ResultAds result = ResultAds.CreateError(errorCode);
      AdsErrorCode adsErrorCode1 = await base.OnWriteConfirmationAsync(target, invokeId, errorCode, cancel).ConfigureAwait(false);
      this.ConfirmRequest(target, invokeId, result);
      AdsErrorCode adsErrorCode2 = adsErrorCode1;
      result = (ResultAds) null;
      return adsErrorCode2;
    }

    /// <summary>
    /// Called when an ADS Read Write confirmation is received.
    /// Overwrite this method in derived classes to react on ADS Read Write confirmations.
    /// </summary>
    /// <param name="address">The sender's AMS address</param>
    /// <param name="invokeId">The invoke id provided by this server during the corresponding request</param>
    /// <param name="errorCode">The ADS error code provided by the sender</param>
    /// <param name="readData">The read data buffer</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous <see cref="M:TwinCAT.Ads.AdsClientServer.OnReadWriteConfirmationAsync(TwinCAT.Ads.AmsAddress,System.UInt32,TwinCAT.Ads.AdsErrorCode,System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" /> operation. The <see cref="T:System.Threading.Tasks.Task`1" /> parameter contains the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> as
    /// <see cref="P:System.Threading.Tasks.Task`1.Result" />.</returns>
    protected virtual Task<AdsErrorCode> OnReadWriteConfirmationAsync(
      AmsAddress address,
      uint invokeId,
      AdsErrorCode errorCode,
      ReadOnlyMemory<byte> readData,
      CancellationToken cancel)
    {
      ResultReadBytes result = new ResultReadBytes(errorCode, readData, invokeId);
      this.ConfirmRequest(address, invokeId, result);
      return Task.FromResult<AdsErrorCode>((AdsErrorCode) 0);
    }

    /// <summary>
    /// Called when an ADS Add Device Notification confirmation is received.
    /// Overwrite this method in derived classes to react on ADS Add Device Notification confirmations.
    /// </summary>
    /// <param name="target">The sender's AMS address</param>
    /// <param name="invokeId">The invoke id provided by this server during the corresponding request</param>
    /// <param name="errorCode">The ADS error code provided by the sender</param>
    /// <param name="notificationHandle">The notification handle provided by the sender</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous <see cref="M:TwinCAT.Ads.AdsClientServer.OnAddDeviceNotificationConfirmationAsync(TwinCAT.Ads.AmsAddress,System.UInt32,TwinCAT.Ads.AdsErrorCode,System.UInt32,System.Threading.CancellationToken)" /> operation. The <see cref="T:System.Threading.Tasks.Task`1" /> parameter contains the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> as
    /// <see cref="P:System.Threading.Tasks.Task`1.Result" />.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual async Task<AdsErrorCode> OnAddDeviceNotificationConfirmationAsync(
      AmsAddress target,
      uint invokeId,
      AdsErrorCode errorCode,
      uint notificationHandle,
      CancellationToken cancel)
    {
      ResultHandle result = new ResultHandle(errorCode, notificationHandle, invokeId);
      AdsErrorCode adsErrorCode1 = await base.OnAddDeviceNotificationConfirmationAsync(target, invokeId, errorCode, notificationHandle, cancel).ConfigureAwait(false);
      this.ConfirmRequest(target, invokeId, result);
      AdsErrorCode adsErrorCode2 = adsErrorCode1;
      result = (ResultHandle) null;
      return adsErrorCode2;
    }

    /// <summary>
    /// Called when an ADS Delete Device Notification confirmation is received.
    /// Overwrite this method in derived classes to react on ADS Delete Device Notification confirmations.
    /// </summary>
    /// <param name="target">The sender's AMS address</param>
    /// <param name="invokeId">The invoke id provided by this server during the corresponding request</param>
    /// <param name="errorCode">The ADS error code provided by the sender</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous <see cref="M:TwinCAT.Ads.AdsClientServer.OnDeleteDeviceNotificationConfirmationAsync(TwinCAT.Ads.AmsAddress,System.UInt32,TwinCAT.Ads.AdsErrorCode,System.Threading.CancellationToken)" /> operation. The <see cref="T:System.Threading.Tasks.Task`1" /> parameter contains the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> as
    /// <see cref="P:System.Threading.Tasks.Task`1.Result" />.</returns>
    protected virtual async Task<AdsErrorCode> OnDeleteDeviceNotificationConfirmationAsync(
      AmsAddress target,
      uint invokeId,
      AdsErrorCode errorCode,
      CancellationToken cancel)
    {
      AdsErrorCode adsErrorCode = await base.OnDeleteDeviceNotificationConfirmationAsync(target, invokeId, errorCode, cancel).ConfigureAwait(false);
      ResultAds error = ResultAds.CreateError(errorCode);
      this.ConfirmRequest(target, invokeId, error);
      return adsErrorCode;
    }

    /// <summary>
    /// Called when an ADS Write Control confirmation is received.
    /// Overwrite this method in derived classes to react on ADS Write Control confirmations.
    /// </summary>
    /// <param name="target">The sender's AMS address</param>
    /// <param name="invokeId">The invoke id provided by this server during the corresponding request</param>
    /// <param name="errorCode">The ADS error code provided by the sender</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous <see cref="M:TwinCAT.Ads.AdsClientServer.OnWriteControlConfirmationAsync(TwinCAT.Ads.AmsAddress,System.UInt32,TwinCAT.Ads.AdsErrorCode,System.Threading.CancellationToken)" /> operation. The <see cref="T:System.Threading.Tasks.Task`1" /> parameter contains the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> as
    /// <see cref="P:System.Threading.Tasks.Task`1.Result" />.</returns>
    protected virtual async Task<AdsErrorCode> OnWriteControlConfirmationAsync(
      AmsAddress target,
      uint invokeId,
      AdsErrorCode errorCode,
      CancellationToken cancel)
    {
      AdsErrorCode adsErrorCode = await base.OnWriteControlConfirmationAsync(target, invokeId, errorCode, cancel).ConfigureAwait(false);
      ResultAds error = ResultAds.CreateError(errorCode);
      this.ConfirmRequest(target, invokeId, error);
      return adsErrorCode;
    }

    internal Task<AdsErrorCode> ReadDeviceInfoRequestAsync(
      AmsAddress target,
      uint id,
      CancellationToken cancel)
    {
      return base.ReadDeviceInfoRequestAsync(target, id, cancel);
    }

    /// <summary>
    /// Sends an ReadDeviceInfo request operation (synchronously)
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>AdsErrorCode.</returns>
    internal AdsErrorCode ReadDeviceInfoRequestSync(AmsAddress target, uint id) => base.ReadDeviceInfoRequestSync(target, id);

    /// <summary>Sends an ReadRequest asynchronously.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="length">The length of the data to read.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    internal Task<AdsErrorCode> ReadRequestAsync(
      AmsAddress target,
      uint id,
      uint indexGroup,
      uint indexOffset,
      int length,
      CancellationToken cancel)
    {
      return base.ReadRequestAsync(target, id, indexGroup, indexOffset, length, cancel);
    }

    /// <summary>Sends an ReadWriteRequest asynchronously.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readLength">Length of the read.</param>
    /// <param name="writeData">The data to write.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    internal Task<AdsErrorCode> ReadWriteRequestAsync(
      AmsAddress target,
      uint id,
      uint indexGroup,
      uint indexOffset,
      int readLength,
      ReadOnlyMemory<byte> writeData,
      CancellationToken cancel)
    {
      return base.ReadWriteRequestAsync(target, id, indexGroup, indexOffset, readLength, writeData, cancel);
    }

    /// <summary>Sends a synchronous ReadWriteRequest.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readLength">Length of the read.</param>
    /// <param name="span">The span.</param>
    /// <returns>AdsErrorCode.</returns>
    internal AdsErrorCode ReadWriteRequestSync(
      AmsAddress target,
      uint id,
      uint indexGroup,
      uint indexOffset,
      int readLength,
      ReadOnlySpan<byte> span)
    {
      return base.ReadWriteRequestSync(target, id, indexGroup, indexOffset, readLength, span);
    }

    /// <summary>Sends an asynchronous WriteControlRequest.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="adsState">State of the ads.</param>
    /// <param name="deviceState">State of the device.</param>
    /// <param name="span">The span.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    internal Task<AdsErrorCode> WriteControlRequestAsync(
      AmsAddress target,
      uint id,
      AdsState adsState,
      ushort deviceState,
      ReadOnlyMemory<byte> span,
      CancellationToken cancel)
    {
      return base.WriteControlRequestAsync(target, id, adsState, deviceState, span, cancel);
    }

    /// <summary>Sends a synchronous WriteControlRequest.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="adsState">State of the ads.</param>
    /// <param name="deviceState">State of the device.</param>
    /// <param name="span">The span.</param>
    /// <returns>AdsErrorCode.</returns>
    internal AdsErrorCode WriteControlRequestSync(
      AmsAddress target,
      uint id,
      AdsState adsState,
      ushort deviceState,
      ReadOnlySpan<byte> span)
    {
      return base.WriteControlRequestSync(target, id, adsState, deviceState, span);
    }

    /// <summary>Sends and asynchronous ReadDeviceStateRequest.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    internal Task<AdsErrorCode> ReadDeviceStateRequestAsync(
      AmsAddress target,
      uint id,
      CancellationToken cancel)
    {
      return base.ReadDeviceStateRequestAsync(target, id, cancel);
    }

    /// <summary>Sends an ReadDeviceState Request synchronously.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>AdsErrorCode.</returns>
    internal AdsErrorCode ReadDeviceStateRequestSync(AmsAddress target, uint id) => base.ReadDeviceStateRequestSync(target, id);

    /// <summary>Sends an ReadRequest synchronously.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="length">The length.</param>
    /// <returns>AdsErrorCode.</returns>
    internal AdsErrorCode ReadRequestSync(
      AmsAddress target,
      uint id,
      uint indexGroup,
      uint indexOffset,
      int length)
    {
      return this.ReadRequest(target, id, indexGroup, indexOffset, length);
    }

    /// <summary>Sends an WriteRequest asynchronously.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="span">The span.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    internal Task<AdsErrorCode> WriteRequestAsync(
      AmsAddress target,
      uint id,
      uint indexGroup,
      uint indexOffset,
      ReadOnlyMemory<byte> span,
      CancellationToken cancel)
    {
      return base.WriteRequestAsync(target, id, indexGroup, indexOffset, span, cancel);
    }

    /// <summary>Sends an WriteRequest synchronously.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="span">The span.</param>
    /// <returns>AdsErrorCode.</returns>
    internal AdsErrorCode WriteRequestSync(
      AmsAddress target,
      uint id,
      uint indexGroup,
      uint indexOffset,
      ReadOnlySpan<byte> span)
    {
      return this.WriteRequest(target, id, indexGroup, indexOffset, span);
    }

    /// <summary>Sends an AddDeviceNotificationRequest Asynchronously.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="dataLength">Length of the data.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    internal Task<AdsErrorCode> AddDeviceNotificationRequestAsync(
      AmsAddress target,
      uint id,
      uint indexGroup,
      uint indexOffset,
      int dataLength,
      NotificationSettings settings,
      CancellationToken cancel)
    {
      return base.AddDeviceNotificationRequestAsync(target, id, indexGroup, indexOffset, dataLength, settings, cancel);
    }

    /// <summary>Sends an DeleteDeviceNotification asynchronously.</summary>
    /// <param name="target">The target.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="handle">The handle.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>Task&lt;AdsErrorCode&gt;.</returns>
    internal Task<AdsErrorCode> DeleteDeviceNotificationRequestAsync(
      AmsAddress target,
      uint id,
      uint handle,
      CancellationToken cancel)
    {
      return base.DeleteDeviceNotificationRequestAsync(target, id, handle, cancel);
    }

    /// <summary>Handler Function for a Router Notification.</summary>
    /// <param name="state">The route state.</param>
    public virtual void OnRouterNotification(AmsRouterState state) => this._routerNotifications.OnRouterNotification(state);
  }
}
