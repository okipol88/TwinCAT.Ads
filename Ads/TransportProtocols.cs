// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TransportProtocols
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;

namespace TwinCAT.Ads
{
  /// <summary>Enum ADS TransportProtocol</summary>
  [Flags]
  public enum TransportProtocols
  {
    /// <summary>None / Uninitialized</summary>
    None = 0,
    /// <summary>ADS via Router</summary>
    Router = 1,
    /// <summary>ADS via TCP/IP (without router)</summary>
    TcpIp = 2,
    /// <summary>
    /// Indicates that <see cref="F:TwinCAT.Ads.TransportProtocols.Router" /> and <see cref="F:TwinCAT.Ads.TransportProtocols.TcpIp" /> are appropriate (for establishing connections)
    /// </summary>
    All = TcpIp | Router, // 0x00000003
  }
}
