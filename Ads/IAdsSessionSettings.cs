// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.IAdsSessionSettings
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads
{
  /// <summary>Interface for ADS Session Settings</summary>
  /// <seealso cref="T:TwinCAT.ISessionSettings" />
  public interface IAdsSessionSettings : ISessionSettings
  {
    /// <summary>Gets the ADS timeout in milliseconds.</summary>
    /// <value>The timeout.</value>
    int Timeout { get; }

    /// <summary>Gets or sets the resurrection time.</summary>
    /// <value>The resurrection time.</value>
    TimeSpan ResurrectionTime { get; set; }

    /// <summary>Gets or sets the symbol loader settings</summary>
    /// <value>The symbol loader.</value>
    SymbolLoaderSettings SymbolLoader { get; set; }
  }
}
