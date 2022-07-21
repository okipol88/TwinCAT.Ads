// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.IgIoSumReadEntity
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;

namespace TwinCAT.Ads.SumCommand
{
  /// <summary>SumDataEntity IndexGroup IndexOffset Read access.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IgIoSumReadEntity : IgIoSumEntity
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.IgIoSumReadEntity" /> class.
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readLength">Length of the read.</param>
    public IgIoSumReadEntity(uint indexGroup, uint indexOffset, int readLength)
      : base(indexGroup, indexOffset, readLength, 0)
    {
    }
  }
}
