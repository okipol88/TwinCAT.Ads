// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.IInterceptedClient
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Interface for an communication client / <see cref="T:TwinCAT.IConnection" /> that accepts <see cref="T:TwinCAT.Ads.ICommunicationInterceptor">Interceptors.</see>
  /// </summary>
  internal interface IInterceptedClient
  {
    /// <summary>Gets the communication interceptors.</summary>
    /// <value>The interceptors.</value>
    CommunicationInterceptors? Interceptors { get; }

    /// <summary>
    /// Sets additional <see cref="T:TwinCAT.Ads.CommunicationInterceptor">Communication Interceptors</see>.
    /// </summary>
    /// <param name="interceptors">The interceptors.</param>
    void SetCommunicationInterceptor(CommunicationInterceptors interceptors);

    bool TryResurrect([NotNullWhen(false)] out AdsException? error);
  }
}
