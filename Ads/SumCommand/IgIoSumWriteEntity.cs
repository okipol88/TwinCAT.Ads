// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.IgIoSumWriteEntity
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;

namespace TwinCAT.Ads.SumCommand
{
  /// <summary>SumDataEntity IndexGroup IndexOffset Write access.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IgIoSumWriteEntity : IgIoSumEntity
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.IgIoSumWriteEntity" /> class.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="writeLength">Length of the write.</param>
    public IgIoSumWriteEntity(uint indexGroup, uint indexOffset, int writeLength)
      : base(indexGroup, indexOffset, 0, writeLength)
    {
    }
  }
}
