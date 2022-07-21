// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.IFailFastHandler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>
  /// Interface for a fast failing (Circuit breaker) ads handler
  /// </summary>
  internal interface IFailFastHandler
  {
    /// <summary>
    /// Guards the <see cref="T:TwinCAT.Ads.IFailFastHandler" /> from state changes that allow unintended communication.
    /// </summary>
    AdsErrorCode Guard();

    /// <summary>
    /// Trips to the internal <see cref="T:TwinCAT.Ads.Internal.LostCommunicationState" /> (Open circuit) state.
    /// </summary>
    /// <param name="errorCode">The causing error.</param>
    void Trip(AdsErrorCode errorCode);

    /// <summary>Indicates that the communication has been succeeded.</summary>
    void Succeed();

    /// <summary>Gets the current state of the Fail Fast handler.</summary>
    /// <value>Current state.</value>
    IFailFastHandlerState CurrentState { get; }

    /// <summary>
    /// Gets a value indicating whether interceptor state is active
    /// </summary>
    /// <value><c>true</c> if communication state is active; otherwise, <c>false</c>.</value>
    bool IsActive { get; }

    /// <summary>
    /// Gets a value indicating whether interceptor state is 'ready to connect'
    /// </summary>
    /// <value><c>true</c> if this instance is reconnecting; otherwise, <c>false</c>.</value>
    bool IsReconnecting { get; }

    /// <summary>
    /// Gets a value indicating whether the interceptor is in open / lost state
    /// </summary>
    /// <value><c>true</c> if communication is lost / open; otherwise, <c>false</c>.</value>
    bool IsLost { get; }

    /// <summary>
    /// Resets the <see cref="T:TwinCAT.Ads.IFailFastHandler" />
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    void Reset();
  }
}
