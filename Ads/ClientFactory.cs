// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ClientFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.Ads
{
  internal class ClientFactory : IAdsClientFactory
  {
    public IAdsConnection Create(ISession session)
    {
      CommunicationInterceptors interceptors = this.createInterceptors(session);
      AdsClient adsClient = new AdsClient(session, ((AdsSessionBase) session).Logger);
      adsClient.SetCommunicationInterceptor(interceptors);
      adsClient.Timeout = ((AdsSessionBase) session).Settings.Timeout;
      return (IAdsConnection) adsClient;
    }

    /// <summary>Creates the interceptors.</summary>
    /// <returns>CommunicationInterceptors.</returns>
    private CommunicationInterceptors createInterceptors(ISession session) => (CommunicationInterceptors) ((IInterceptionFactory) session).CreateInterceptor();
  }
}
