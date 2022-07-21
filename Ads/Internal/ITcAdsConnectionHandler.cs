// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.ITcAdsConnectionHandler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

namespace TwinCAT.Ads.Internal
{
  /// <summary>Interface for handling connection state changes</summary>
  internal interface ITcAdsConnectionHandler
  {
    /// <summary>Called when the Communication has been established.</summary>
    void OnConnected();

    /// <summary>Called when the communication is shutdown.</summary>
    void OnBeforeDisconnected();

    /// <summary>Called when the communication is shutdown.</summary>
    void OnDisconnected();
  }
}
