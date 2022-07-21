﻿// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.SymbolUploadFlags
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;

namespace TwinCAT.Ads.Internal
{
  /// <summary>Enum SymbolUploadFlags</summary>
  /// <exclude />
  [Flags]
  public enum SymbolUploadFlags : uint
  {
    /// <summary>None / Unititialized</summary>
    None = 0,
    /// <summary>Target is 64 Bit Platform</summary>
    Is64BitPlatform = 1,
    /// <summary>Symbol Server includes Base types.</summary>
    IncludesBaseTypes = 2,
  }
}
