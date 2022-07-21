// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumHandleList
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Class SumHandleList.</summary>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumHandleCollection" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class SumHandleList : 
    List<SumHandleEntry>,
    ISumHandleCollection,
    IList<SumHandleEntry>,
    ICollection<SumHandleEntry>,
    IEnumerable<SumHandleEntry>,
    IEnumerable
  {
    public uint[] ValidHandles => this.Select<SumHandleEntry, uint>((Func<SumHandleEntry, uint>) (e => e.Handle)).Where<uint>((Func<uint, bool>) (h => h > 0U)).ToArray<uint>();
  }
}
