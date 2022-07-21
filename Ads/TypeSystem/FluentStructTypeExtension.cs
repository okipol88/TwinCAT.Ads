// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.FluentStructTypeExtension
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class FluentStructTypeExtension.</summary>
  /// <remarks>
  /// Fluent interface for adding members to <see cref="T:TwinCAT.Ads.TypeSystem.StructType" />s.
  /// </remarks>
  public static class FluentStructTypeExtension
  {
    /// <summary>
    /// Adds a member to the <see cref="T:TwinCAT.Ads.TypeSystem.StructType" />
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="member">The member.</param>
    /// <returns>StructType.</returns>
    public static StructType AddAligned(this StructType str, IMember member)
    {
      str.AddAligned(member);
      return str;
    }

    /// <summary>Adds a RpcMethod.</summary>
    /// <param name="str">The struct.</param>
    /// <param name="method">The method.</param>
    /// <returns>RpcStructType.</returns>
    public static StructType AddMethod(this StructType str, IRpcMethod method)
    {
      str.AddMethod(method);
      return str;
    }
  }
}
