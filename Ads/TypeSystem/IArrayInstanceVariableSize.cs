// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.IArrayInstanceVariableSize
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Interface IAnySizeArrayInstance</summary>
  /// <exclude />
  public interface IArrayInstanceVariableSize
  {
    /// <summary>Updates the dimensions of this VariableSize Array</summary>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode TryUpdateDimensions();

    /// <summary>Reads the dynamic Dim Lengths of the AnySize Array.</summary>
    /// <param name="lengthIsValue">The dynamic dimension lengths.</param>
    /// <returns>AdsErrorCode.</returns>
    AdsErrorCode TryReadLengthIs(out int[]? lengthIsValue);

    /// <summary>Gets the Array of Dynamic Dim Lengths (cached).</summary>
    /// <value>The dynamic dim lengths.</value>
    int[] DynamicDimLengths { get; }
  }
}
