// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.PlcOpenType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.Ads.Internal;
using TwinCAT.PlcOpen;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Plc Open types.</summary>
  /// <remarks>These are <see cref="T:TwinCAT.Ads.TypeSystem.PrimitiveType">Primitive types</see> like (UXINT, XINT, XWORD, PWORD) whose size is dependant of the target platform (4 or 8 bytes).
  /// </remarks>
  /// <exclude />
  public sealed class PlcOpenType : PrimitiveType
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PlatformBoundType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">entry</exception>
    internal PlcOpenType(AdsDataTypeEntry entry)
      : base(entry, (PrimitiveTypeFlags) 48, (Type) null)
    {
      switch (entry.EntryName)
      {
        case "DATE":
          this.ManagedType = typeof (DATE);
          break;
        case "DATE_AND_TIME":
          this.ManagedType = typeof (DT);
          break;
        case "DT":
          this.ManagedType = typeof (DT);
          break;
        case "LTIME":
          this.ManagedType = typeof (LTIME);
          break;
        case "TIME":
          this.ManagedType = typeof (TIME);
          break;
        case "TIME_OF_DAY":
          this.ManagedType = typeof (TOD);
          break;
        case "TOD":
          this.ManagedType = typeof (TOD);
          break;
        default:
          throw new NotSupportedException("The type named '" + entry.EntryName + "' is not a PlcOpen DataType!");
      }
    }
  }
}
