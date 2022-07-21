// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AlignmentCalculator
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.Linq;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class calculating alignments.</summary>
  public class AlignmentCalculator
  {
    private int packMode = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AlignmentCalculator" /> class.
    /// </summary>
    /// <param name="packMode">The pack mode.</param>
    public AlignmentCalculator(int packMode) => this.packMode = packMode;

    /// <summary>Gets the Pack1 Alignment Calculator</summary>
    /// <value>The pack1.</value>
    public static AlignmentCalculator Pack1 => new AlignmentCalculator(1);

    public int GetNextOffset(IInstanceCollection<IMember> instances)
    {
      IMember imember = ((IEnumerable<IMember>) instances).LastOrDefault<IMember>();
      return imember != null ? imember.Offset + ((IBitSize) imember).Size : 0;
    }
  }
}
