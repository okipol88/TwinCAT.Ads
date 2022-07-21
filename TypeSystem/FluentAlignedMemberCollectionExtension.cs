// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.FluentAlignedMemberCollectionExtension
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class FluentStructTypeExtension.</summary>
  /// <remarks>
  /// Fluent interface for adding members to <see cref="T:TwinCAT.Ads.TypeSystem.StructType" />s.
  /// </remarks>
  public static class FluentAlignedMemberCollectionExtension
  {
    /// <summary>
    /// Adds a member to the <see cref="T:TwinCAT.Ads.TypeSystem.StructType" />
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="member">The member.</param>
    /// <returns>StructType.</returns>
    public static AlignedMemberCollection AddAligned(
      this AlignedMemberCollection str,
      IMember member)
    {
      str.Add(member);
      return str;
    }
  }
}
