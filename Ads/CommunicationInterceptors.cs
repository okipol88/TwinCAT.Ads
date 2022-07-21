// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.CommunicationInterceptors
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Class CommunicationInterceptors is used for combinations (collections) of <see cref="T:TwinCAT.Ads.ICommunicationInterceptor" /> objects.
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CommunicationInterceptors : CommunicationInterceptor
  {
    /// <summary>Internal list of interceptors.</summary>
    private List<ICommunicationInterceptHandler> _list;
    private static int _id;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.CommunicationInterceptors" /> class.
    /// </summary>
    public CommunicationInterceptors()
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
      interpolatedStringHandler.AppendLiteral("CombinedInterceptor_");
      interpolatedStringHandler.AppendFormatted<int>(++CommunicationInterceptors._id);
      // ISSUE: explicit constructor call
      base.\u002Ector(interpolatedStringHandler.ToStringAndClear());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.CommunicationInterceptors" /> class.
    /// </summary>
    /// <param name="interceptors">The interceptors.</param>
    public CommunicationInterceptors(
      IEnumerable<ICommunicationInterceptHandler> interceptors)
      : this()
    {
      this._list.AddRange(interceptors);
    }

    /// <summary>Gets the internal (combined) Interceptors</summary>
    /// <value>The combined interceptors.</value>
    internal ReadOnlyCollection<ICommunicationInterceptor> CombinedInterceptors => new List<ICommunicationInterceptor>(this._list.Cast<ICommunicationInterceptor>()).AsReadOnly();

    /// <summary>Finds the communication interceptor by base type.</summary>
    /// <returns>IEnumerable&lt;ICommunicationInterceptor&gt;.</returns>
    public T? Find<T>() where T : class => (T) this._list.FirstOrDefault<ICommunicationInterceptHandler>((Func<ICommunicationInterceptHandler, bool>) (interceptor => typeof (T).IsAssignableFrom(interceptor.GetType())));

    /// <summary>Finds the communication interceptor by id</summary>
    /// <param name="id">The identifier.</param>
    /// <returns>ICommunicationInterceptor.</returns>
    public ICommunicationInterceptor? Find(string id) => this._list.Cast<ICommunicationInterceptor>().FirstOrDefault<ICommunicationInterceptor>((Func<ICommunicationInterceptor, bool>) (interceptor => StringComparer.OrdinalIgnoreCase.Compare(interceptor.ID, id) == 0));

    /// <summary>
    /// Combines the specified interceptor with the current <see cref="T:TwinCAT.Ads.CommunicationInterceptors" />.
    /// </summary>
    /// <param name="interceptor">The interceptor.</param>
    /// <returns>ICommunicationInterceptor.</returns>
    public ICommunicationInterceptor Combine(
      ICommunicationInterceptor interceptor)
    {
      this._list.Add((ICommunicationInterceptHandler) interceptor);
      return (ICommunicationInterceptor) this;
    }

    /// <summary>
    /// Handler function called before the communication action occurs.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    protected override AdsErrorCode OnBeforeCommunicate()
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
      {
        adsErrorCode = interceptHandler.BeforeCommunicate();
        if (adsErrorCode != null)
          break;
      }
      return adsErrorCode;
    }

    /// <summary>Handler function called after communication</summary>
    /// <param name="resurrect">Resurrection flag.</param>
    /// <param name="errorCode">The error code.</param>
    protected override void OnAfterCommunicate(bool resurrect, ResultAds errorCode)
    {
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
        interceptHandler.AfterCommunicate(resurrect, errorCode);
    }

    /// <summary>
    /// Handler function called before the connection is established.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    protected override AdsErrorCode OnBeforeConnect()
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
      {
        adsErrorCode = interceptHandler.BeforeConnect();
        if (adsErrorCode != null)
          break;
      }
      return adsErrorCode;
    }

    /// <summary>
    /// Handler function called after the connection has been established.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    protected override void OnAfterConnect(AdsErrorCode errorCode)
    {
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
        interceptHandler.AfterConnect(errorCode);
    }

    /// <summary>
    /// Handler function called before the communication channel is shut down.
    /// </summary>
    /// <returns>AdsErrorCode.</returns>
    protected override AdsErrorCode OnBeforeDisconnect()
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
      {
        adsErrorCode = interceptHandler.BeforeDisconnect();
        if (adsErrorCode != null)
          break;
      }
      return adsErrorCode;
    }

    /// <summary>
    /// Handler function called after the disconnection of the communication channel.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    protected override void OnAfterDisconnect(AdsErrorCode errorCode)
    {
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
        interceptHandler.AfterDisconnect(errorCode);
    }

    /// <summary>Handler function called before an ADS State is read.</summary>
    /// <returns>AdsErrorCode.</returns>
    protected override AdsErrorCode OnBeforeReadState()
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
      {
        adsErrorCode = interceptHandler.BeforeReadState();
        if (adsErrorCode != null)
          break;
      }
      return adsErrorCode;
    }

    /// <summary>Handler function called after the ADS state is read.</summary>
    /// <param name="result">The result.</param>
    protected override void OnAfterReadState(ResultReadDeviceState result)
    {
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
        interceptHandler.AfterReadState(result);
    }

    /// <summary>
    /// Handler function called before the ADS state is written.
    /// </summary>
    /// <param name="adsState">State of the ads.</param>
    /// <returns>AdsErrorCode.</returns>
    protected override AdsErrorCode OnBeforeWriteState(StateInfo adsState)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
      {
        adsErrorCode = interceptHandler.BeforeWriteState(adsState);
        if (adsErrorCode != null)
          break;
      }
      return adsErrorCode;
    }

    /// <summary>
    /// Handler function called after an ADS state is written.
    /// </summary>
    /// <param name="adsState">ADS state.</param>
    /// <param name="result">The result.</param>
    protected override void OnAfterWriteState(StateInfo adsState, ResultAds result)
    {
      foreach (ICommunicationInterceptHandler interceptHandler in this._list)
        interceptHandler.AfterWriteState(adsState, result);
    }

    /// <summary>Lookups the specified interceptor type.</summary>
    /// <param name="interceptorType">Type of the interceptor.</param>
    /// <returns>ICommunicationInterceptor.</returns>
    public ICommunicationInterceptor? Lookup(Type interceptorType) => (ICommunicationInterceptor) this._list.Find((Predicate<ICommunicationInterceptHandler>) (element => element.GetType().IsAssignableFrom(interceptorType)));
  }
}
