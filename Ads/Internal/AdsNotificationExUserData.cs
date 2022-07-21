// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsNotificationExUserData
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class AdsNotificationExUserData.</summary>
  internal class AdsNotificationExUserData
  {
    public readonly object? userData;
    public readonly Type type;
    public readonly int[]? args;

    public AdsNotificationExUserData(Type type, int[]? args, object? userData)
    {
      this.type = type;
      this.args = args;
      this.userData = userData;
    }
  }
}
