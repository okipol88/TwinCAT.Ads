// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsSysServState
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Runtime.InteropServices;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal class AdsSysServState
  {
    internal AdsState adsState;
    internal ushort deviceState;
    internal ushort restartIndex;
    internal byte version;
    internal byte revision;
    internal ushort build;
    internal byte platform;
    internal byte osType;
    internal ushort[] reserved = new ushort[2];
  }
}
