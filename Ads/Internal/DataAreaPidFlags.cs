// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.DataAreaPidFlags
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;

namespace TwinCAT.Ads.Internal
{
  /// <summary>Enum DataAreaPidFlags</summary>
  /// <exclude />
  [Flags]
  internal enum DataAreaPidFlags : uint
  {
    /// <summary>Enables Pid Addressing</summary>
    PidAddressing = 2147483648, // 0x80000000
    /// <summary>BitType Addressing</summary>
    BitTypeFlag = 1073741824, // 0x40000000
    /// <summary>
    /// Offset mask (Byte offset or Bit offset, dependent on setting <see cref="F:TwinCAT.Ads.Internal.DataAreaPidFlags.BitTypeFlag" />)
    /// </summary>
    Mask_PidOffset = 16777215, // 0x00FFFFFF
    /// <summary>Mask DataArea (e.g.</summary>
    Mask_PidAreaNo = 1056964608, // 0x3F000000
  }
}
