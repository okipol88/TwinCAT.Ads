﻿// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.TypeMembersConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Linq;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  internal static class TypeMembersConverter
  {
    internal static AdsFieldEntry[] ToFieldEntryArray(IMemberCollection members) => ((IEnumerable<IMember>) members).Select<IMember, AdsFieldEntry>((Func<IMember, AdsFieldEntry>) (m => new AdsFieldEntry(m))).ToArray<AdsFieldEntry>();
  }
}
