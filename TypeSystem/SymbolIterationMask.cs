// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.SymbolIterationMask
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;

namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Mask Flagset to specify filters for <see cref="T:TwinCAT.TypeSystem.Generic.SymbolIterator`1" />.
  /// </summary>
  [Flags]
  public enum SymbolIterationMask
  {
    /// <summary>Uninitialized / None</summary>
    /// <remarks>Doesn't iterate over complex types</remarks>
    None = 0,
    /// <summary>Iterates over Subelements of Structs</summary>
    Structures = 1,
    /// <summary>Iterates over Elements of Arrays</summary>
    Arrays = 2,
    /// <summary>Iterates over Subelements of Unions</summary>
    Unions = 4,
    /// <summary>Iterates over Pointer SubElements</summary>
    Pointer = 8,
    /// <summary>Iterates over References</summary>
    References = 16, // 0x00000010
    /// <summary>Iterates over All Complex/Combined types</summary>
    All = References | Pointer | Unions | Arrays | Structures, // 0x0000001F
    /// <summary>
    /// Iterates over All Complex/Combined types excluding Array elements
    /// </summary>
    AllWithoutArrayElements = References | Pointer | Unions | Structures, // 0x0000001D
  }
}
