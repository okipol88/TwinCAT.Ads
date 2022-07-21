// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.IgIoSumEntity
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;

namespace TwinCAT.Ads.SumCommand
{
  /// <summary>SumDataEntity with IndexGroup IndexOffset access.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IgIoSumEntity : SumDataEntity
  {
    /// <summary>The index group</summary>
    public readonly uint IndexGroup;
    /// <summary>The index offset</summary>
    public readonly uint IndexOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.IgIoSumEntity" /> class.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readLength">Length of the read.</param>
    /// <param name="writeLength">Length of the write.</param>
    public IgIoSumEntity(uint indexGroup, uint indexOffset, int readLength, int writeLength)
      : base(readLength, writeLength)
    {
      this.IndexGroup = indexGroup;
      this.IndexOffset = indexOffset;
    }
  }
}
