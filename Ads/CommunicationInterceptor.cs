// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.CommunicationInterceptor
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
  /// <summary>
  /// Communication interceptor class (abstract base class for cross cutting communication issues).
  /// </summary>
  /// <remarks>The <see cref="T:TwinCAT.Ads.CommunicationInterceptor" /></remarks>
  ///  is used for 'cross concern' communication issues. E.g. when a client has to interact just before and after
  ///             the <see cref="T:TwinCAT.Ads.AdsClient" />
  ///  communicates with the connected AdsServer.
  ///             <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class CommunicationInterceptor : 
    ICommunicationInterceptor,
    ICommunicationInterceptHandler
  {
    /// <summary>The Identifier</summary>
    private string _id;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.CommunicationInterceptor" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    protected CommunicationInterceptor(string id) => this._id = id;

    /// <summary>
    /// Gets the identifier of the <see cref="T:TwinCAT.Ads.ICommunicationInterceptor" />
    /// </summary>
    /// <value>The identifier.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public string ID => this._id;

    /// <summary>Communicates the specified action.</summary>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="action">The action.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exception cref="T:System.ArgumentNullException">action</exception>
    public AdsErrorCode Communicate(bool resurrect, Func<bool, ResultAds> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      AdsErrorCode adsErrorCode = this.BeforeCommunicate();
      ResultAds result = adsErrorCode != null ? ResultAds.CreateError(adsErrorCode) : action(resurrect);
      this.AfterCommunicate(resurrect, result);
      return adsErrorCode;
    }

    /// <summary>Communication handler (asynchronous)</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action">The communication action to be called.</param>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task object representing the 'Communication' operation, returning a future result <typeparamref name="T" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">action</exception>
    public Task<T> CommunicateAsync<T>(
      Func<bool, CancellationToken, Task<T>> action,
      bool resurrect,
      CancellationToken cancel)
      where T : ResultAds
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      return ((Func<bool, CancellationToken, Task<T>>) (async (r, c) =>
      {
        AdsErrorCode adsErrorCode = this.BeforeCommunicate();
        T result = default (T);
        if (adsErrorCode == null)
          result = await action(r, c).ConfigureAwait(false);
        else
          ((ResultAds) (object) result).SetError(adsErrorCode);
        this.AfterCommunicate(resurrect, (ResultAds) (object) result);
        return result;
      }))(resurrect, cancel);
    }

    /// <summary>Interceptor handler for 'ReadState'</summary>
    /// <param name="action">The read state action.</param>
    /// <returns>ResultReadDeviceState.</returns>
    public ResultReadDeviceState CommunicateReadState(
      Func<ResultReadDeviceState> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      AdsErrorCode adsErrorCode = this.BeforeCommunicate() ?? this.BeforeReadState();
      ResultReadDeviceState result = adsErrorCode != null ? new ResultReadDeviceState(adsErrorCode, new StateInfo(), 0U) : action();
      this.AfterReadState(result);
      this.AfterCommunicate(false, (ResultAds) result);
      return result;
    }

    /// <summary>Asynchronous Interceptor handler for 'ReadState'</summary>
    /// <param name="action">The asynchronous action.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task object representing the 'ReadState' operation, returning a future result <see cref="T:TwinCAT.Ads.ResultReadDeviceState" /> which contains the
    /// <see cref="P:TwinCAT.Ads.ResultReadDeviceState.State" /> and the communication result <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /></returns>
    public Task<ResultReadDeviceState> CommunicateReadStateAsync(
      Func<CancellationToken, Task<ResultReadDeviceState>> action,
      CancellationToken cancel)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      return ((Func<CancellationToken, Task<ResultReadDeviceState>>) (async c =>
      {
        AdsErrorCode adsErrorCode = this.BeforeCommunicate() ?? this.BeforeReadState();
        ResultReadDeviceState result;
        if (adsErrorCode == null)
          result = await action(c).ConfigureAwait(false);
        else
          result = new ResultReadDeviceState(adsErrorCode, new StateInfo(), 0U);
        this.AfterCommunicate(false, (ResultAds) result);
        this.AfterReadState(result);
        return result;
      }))(cancel);
    }

    /// <summary>Interceptor handler for 'WriteState'</summary>
    /// <param name="action">The write state action.</param>
    /// <param name="adsState">The ADS state.</param>
    /// <returns>ResultWrite.</returns>
    public ResultWrite CommunicateWriteState(
      Func<ResultWrite> action,
      ref StateInfo adsState)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      ResultWrite result = ResultWrite.Empty;
      AdsErrorCode adsErrorCode = this.BeforeCommunicate();
      ((ResultAds) result).SetError(adsErrorCode);
      if (adsErrorCode == null)
      {
        adsErrorCode = this.BeforeWriteState(adsState);
        ((ResultAds) result).SetError(adsErrorCode);
      }
      if (adsErrorCode == null)
        result = action();
      this.AfterWriteState(adsState, (ResultAds) result);
      this.AfterCommunicate(false, (ResultAds) result);
      return result;
    }

    /// <summary>Asynchronous handler for 'WriteState'</summary>
    /// <param name="action">The action.</param>
    /// <param name="adsState">State of the ads.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task object representing the 'WriteState' operation, returning a future result <see cref="T:TwinCAT.Ads.ResultWrite" /> which contains the
    /// communication return code <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" />.</returns>
    public Task<ResultWrite> CommunicateWriteStateAsync(
      Func<CancellationToken, Task<ResultWrite>> action,
      StateInfo adsState,
      CancellationToken cancel)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      return ((Func<CancellationToken, Task<ResultWrite>>) (async c =>
      {
        ResultWrite result = ResultWrite.Empty;
        AdsErrorCode adsErrorCode = this.BeforeCommunicate();
        ((ResultAds) result).SetError(adsErrorCode);
        if (adsErrorCode == null)
        {
          adsErrorCode = this.BeforeWriteState(adsState);
          ((ResultAds) result).SetError(adsErrorCode);
        }
        if (adsErrorCode == null)
          result = await action(c).ConfigureAwait(false);
        this.AfterWriteState(adsState, (ResultAds) result);
        return result;
      }))(cancel);
    }

    /// <summary>Calls the specified connection action</summary>
    /// <param name="action">Wrapped handler function.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode Connect(Func<AdsErrorCode> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      AdsErrorCode result = this.BeforeConnect() ?? action();
      this.AfterConnect(result);
      return result;
    }

    /// <summary>
    /// Handler function for shutting the communication connection down.
    /// </summary>
    /// <param name="action">Wrapped handler function.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode BeforeDisconnect(Func<AdsErrorCode> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      AdsErrorCode adsErrorCode = action();
      ((ICommunicationInterceptHandler) this).BeforeDisconnect();
      return adsErrorCode;
    }

    /// <summary>
    /// Handler function for shutting the communication connection down.
    /// </summary>
    /// <param name="action">Wrapped handler function.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode Disconnect(Func<AdsErrorCode> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      AdsErrorCode result = action();
      this.AfterDisconnect(result);
      return result;
    }

    /// <summary>Called before communication</summary>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode BeforeCommunicate() => this.OnBeforeCommunicate();

    /// <summary>
    /// Handler function called before the communication action occurs.
    /// </summary>
    protected virtual AdsErrorCode OnBeforeCommunicate() => (AdsErrorCode) 0;

    /// <summary>Called After communication</summary>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="errorCode">The error code.</param>
    public void AfterCommunicate(bool resurrect, ResultAds errorCode) => this.OnAfterCommunicate(resurrect, errorCode);

    /// <summary>Handler function called after communication</summary>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="errorCode">The error code.</param>
    protected virtual void OnAfterCommunicate(bool resurrect, ResultAds errorCode)
    {
    }

    /// <summary>
    /// Called before the communication channel is established.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode BeforeConnect() => this.OnBeforeConnect();

    /// <summary>
    /// Handler function called before the connection is established.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    protected virtual AdsErrorCode OnBeforeConnect() => (AdsErrorCode) 0;

    /// <summary>Called after the connection has been established.</summary>
    /// <param name="errorCode">The error code.</param>
    public void AfterConnect(AdsErrorCode errorCode) => this.OnAfterConnect(errorCode);

    /// <summary>
    /// Handler function called after the connection has been established.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    protected virtual void OnAfterConnect(AdsErrorCode errorCode)
    {
    }

    /// <summary>Called before the communication channel shuts down.</summary>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode ICommunicationInterceptHandler.BeforeDisconnect() => this.OnBeforeDisconnect();

    /// <summary>
    /// Handler function called before the communication channel is shut down.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    protected virtual AdsErrorCode OnBeforeDisconnect() => (AdsErrorCode) 0;

    /// <summary>Called after the disconnect.</summary>
    /// <param name="errorCode">The error code.</param>
    public void AfterDisconnect(AdsErrorCode errorCode) => this.OnAfterDisconnect(errorCode);

    /// <summary>
    /// Handler function called after the disconnection of the communication channel.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    protected virtual void OnAfterDisconnect(AdsErrorCode errorCode)
    {
    }

    /// <summary>Called before an ADS state is written</summary>
    /// <param name="adsState">State of the ads.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode BeforeWriteState(StateInfo adsState) => this.OnBeforeWriteState(adsState);

    /// <summary>
    /// Handler function called before the ADS state is written.
    /// </summary>
    /// <param name="adsState">State of the ads.</param>
    /// <returns>AdsErrorCode.</returns>
    protected virtual AdsErrorCode OnBeforeWriteState(StateInfo adsState) => (AdsErrorCode) 0;

    /// <summary>Called after an ADS state is written.</summary>
    /// <param name="adsState">Ads state..</param>
    /// <param name="result">The result.</param>
    public void AfterWriteState(StateInfo adsState, ResultAds result) => this.OnAfterWriteState(adsState, result);

    /// <summary>
    /// Handler function called after an ADS state is written.
    /// </summary>
    /// <param name="adsState">ADS state.</param>
    /// <param name="result">The result.</param>
    protected virtual void OnAfterWriteState(StateInfo adsState, ResultAds result)
    {
    }

    /// <summary>Called before the AsdState is read.</summary>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode BeforeReadState() => this.OnBeforeReadState();

    /// <summary>Handler function called before an ADS State is read.</summary>
    /// <returns>AdsErrorCode.</returns>
    protected virtual AdsErrorCode OnBeforeReadState() => (AdsErrorCode) 0;

    /// <summary>Called after the ADS state is read.</summary>
    /// <param name="result">The result.</param>
    public void AfterReadState(ResultReadDeviceState result) => this.OnAfterReadState(result);

    /// <summary>Handler function called after the ADS state is read.</summary>
    /// <param name="result">The result.</param>
    protected virtual void OnAfterReadState(ResultReadDeviceState result)
    {
    }
  }
}
