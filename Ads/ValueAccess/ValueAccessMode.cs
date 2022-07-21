// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.ValueAccess.ValueAccessMode
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

namespace TwinCAT.Ads.ValueAccess
{
  /// <summary>Enum ValueAccessMethod</summary>
  /// <remarks>
  /// <list type="table">
  /// <listheader><term>Mode</term><description>Description</description></listheader>
  ///     <item><term><see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.None" /></term><description>None/Uninitialized. No Valid mode.</description></item>
  ///     <item><term><see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.IndexGroupOffset" /></term><description>Communicates over IndexGroup/IndexOffset only. This is the most direct/efficient access into the Process image. The advantage is that, the symbol
  ///     access is done via 1 ADS round trip. Disadvantages are that not all Symbols can be accessed via IG/IO (e.g. References) and IndexOffsets could be invalid after
  ///     online changes / PlcProgram downloads. Detection of these events and following invalidation of all changed symbols need to be done within the user application.</description></item>
  ///     <item><term><see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.Symbolic" /></term><description>The Symbolic-only mode is the most safe mode to use but needs more time than the <see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.IndexGroupOffset" />. It could
  ///     need up to 3 ADS round trips (create handle, access value, close handle) but is not influenced by online changes or / plcProgram downloads.</description></item>
  ///     <item><term><see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.IndexGroupOffsetPreferred" /></term><description>This is a mixed access mode. For symbols, where it is possible it uses the IndexGroup/IndexOffset. For others it chooses
  ///     the <see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.Symbolic" /> access.</description></item>
  ///     <item><term><see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.Default" /></term><description>The Default-Mode setting if no other <see cref="T:TwinCAT.Ads.ValueAccess.ValueAccessMode" /> is specified. This is set to <see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.Symbolic" />.</description></item>
  /// </list>
  /// </remarks>
  public enum ValueAccessMode
  {
    /// <summary>None / Uninitialized</summary>
    None = 0,
    /// <summary>Value access via Index Group and Offset Only</summary>
    IndexGroupOffset = 1,
    /// <summary>
    /// The Default access mode (<see cref="F:TwinCAT.Ads.ValueAccess.ValueAccessMode.Symbolic" />)
    /// </summary>
    Default = 2,
    /// <summary>Symbolic access via Instance Path only.</summary>
    Symbolic = 2,
    /// <summary>
    /// Uses IndexGroup IndexOffset Preferred (and Symbolic for Dereferenced Pointers / References)
    /// </summary>
    /// <remarks>
    /// By standard this uses IndexGroup/IndexOffset. For Symbols that are dereferenced (Pointers/Referenced) the
    /// Symbol method is chosen.
    /// </remarks>
    IndexGroupOffsetPreferred = 3,
  }
}
