// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.IInterceptionFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Interface IInterceptionFactory</summary>
  /// <exclude />
  internal interface IInterceptionFactory
  {
    /// <summary>Creates the communication interceptor(s).</summary>
    /// <returns>ICommunicationInterceptor.</returns>
    ICommunicationInterceptor CreateInterceptor();
  }
}
