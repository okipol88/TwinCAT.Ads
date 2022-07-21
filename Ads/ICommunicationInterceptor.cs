// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ICommunicationInterceptor
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Interface for intercepting communication</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface ICommunicationInterceptor
  {
    /// <summary>
    /// Gets the identifier of the <see cref="T:TwinCAT.Ads.ICommunicationInterceptor" />
    /// </summary>
    /// <value>The identifier.</value>
    string ID { get; }

    /// <summary>Communication handler</summary>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="action">The communication action to be called.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode Communicate(bool resurrect, Func<bool, ResultAds> action);

    /// <summary>Communication handler (asynchronous)</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action">The communication action to be called.</param>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task object representing the 'Communication' operation, returning a future result <typeparamref name="T" />.</returns>
    Task<T> CommunicateAsync<T>(
      Func<bool, CancellationToken, Task<T>> action,
      bool resurrect,
      CancellationToken cancel)
      where T : ResultAds;

    /// <summary>
    /// Handler function for establishing the communication connection
    /// </summary>
    /// <param name="action">Wrapped handler function.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode Connect(Func<AdsErrorCode> action);

    /// <summary>
    /// Handler function for shutting the communication connection down.
    /// </summary>
    /// <param name="action">Wrapped handler function.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode BeforeDisconnect(Func<AdsErrorCode> action);

    /// <summary>
    /// Handler function for shutting the communication connection down.
    /// </summary>
    /// <param name="action">Wrapped handler function.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode Disconnect(Func<AdsErrorCode> action);

    /// <summary>Interceptor handler for 'ReadState'</summary>
    /// <param name="action">The read state action.</param>
    /// <returns>ResultReadDeviceState.</returns>
    ResultReadDeviceState CommunicateReadState(
      Func<ResultReadDeviceState> action);

    /// <summary>Asynchronous Interceptor handler for 'ReadState'</summary>
    /// <param name="action">The asynchronous action.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task object representing the 'ReadState' operation, returning a future result <see cref="T:TwinCAT.Ads.ResultReadDeviceState" /> which contains the
    /// <see cref="P:TwinCAT.Ads.ResultReadDeviceState.State" /> and the communication result <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /></returns>
    Task<ResultReadDeviceState> CommunicateReadStateAsync(
      Func<CancellationToken, Task<ResultReadDeviceState>> action,
      CancellationToken cancel);

    /// <summary>Interceptor handler for 'WriteState'</summary>
    /// <param name="action">The write state action.</param>
    /// <param name="adsState">The ADS state.</param>
    /// <returns>ResultWrite.</returns>
    ResultWrite CommunicateWriteState(Func<ResultWrite> action, ref StateInfo adsState);

    /// <summary>Asynchronous handler for 'WriteState'</summary>
    /// <param name="action">The action.</param>
    /// <param name="adsState">State of the ads.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task object representing the 'WriteState' operation, returning a future result <see cref="T:TwinCAT.Ads.ResultWrite" /> which contains the
    /// communication return code <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" />.</returns>
    Task<ResultWrite> CommunicateWriteStateAsync(
      Func<CancellationToken, Task<ResultWrite>> action,
      StateInfo adsState,
      CancellationToken cancel);
  }
}
