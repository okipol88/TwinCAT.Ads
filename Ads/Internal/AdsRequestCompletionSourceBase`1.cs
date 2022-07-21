// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsRequestCompletionSourceBase`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Base implementation of a AdsRequest completion source</summary>
  /// <typeparam name="T"></typeparam>
  /// <seealso cref="T:System.Threading.Tasks.TaskCompletionSource`1" />
  /// <seealso cref="T:TwinCAT.Ads.Internal.IRequestCompletionSource" />
  /// <remarks>Represents an external asynchronous operation that produces an answer to an ADS Request (as Ads TaskResult). It propagates the asynchronous result of the ADS Confirmation
  /// via the Task object to the calling code (the code that starts the Request):
  /// </remarks>
  internal class AdsRequestCompletionSourceBase<T> : 
    TaskCompletionSource<T>,
    IRequestCompletionSource
    where T : ResultAds
  {
    private uint _id;

    /// <summary>Gets the Ads InvokeID</summary>
    /// <value>The invoke identifier.</value>
    /// <remarks>This is the Identifier the Completion source is triggering its confirmations on.</remarks>
    public uint InvokeId => this._id;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsRequestCompletionSourceBase`1" /> class.
    /// </summary>
    /// <param name="invokeId">The invoke identifier.</param>
    protected AdsRequestCompletionSourceBase(uint invokeId) => this._id = invokeId;
  }
}
