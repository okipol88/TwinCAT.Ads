// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.PCCHType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Class PCCHType. This class cannot be inherited.
  /// Implements the <see cref="T:TwinCAT.Ads.TypeSystem.PointerType" />
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.PointerType" />
  public sealed class PCCHType : PointerType
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PCCHType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    internal PCCHType(AdsDataTypeEntry entry)
      : base(entry, "BYTE")
    {
    }

    /// <summary>Creates this instance.</summary>
    /// <returns>PCCHType.</returns>
    public static PCCHType Create() => new PCCHType();

    private PCCHType()
      : base("PCCH", "BYTE", IntPtr.Size)
    {
    }
  }
}
