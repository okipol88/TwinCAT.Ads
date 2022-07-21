// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.MarshallingHelperExtension
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class MarshallingHelperExtension.</summary>
  /// <exclude />
  internal static class MarshallingHelperExtension
  {
    /// <summary>Gets the marshal size of the symbol.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>System.Int32.</returns>
    internal static int GetValueMarshalSize(this ISymbol symbol) => symbol.Category != 15 ? ((IBitSize) symbol).ByteSize : ((IReferenceInstance) symbol).ResolvedByteSize;

    /// <summary>
    /// Gets the marshal siize of the <see cref="T:TwinCAT.TypeSystem.IDataType" />
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.Int32.</returns>
    internal static int GetValueMarshalSize(this IDataType type) => type.Category != 15 ? ((IBitSize) type).ByteSize : ((IReferenceType) type).ResolvedByteSize;
  }
}
