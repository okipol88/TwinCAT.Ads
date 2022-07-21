// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumDataEntity
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;

namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Class SumDataEntityInfo.</summary>
  /// <remarks>The DataEntity describes a single data entity that is part of the Sum Command.</remarks>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class SumDataEntity
  {
    /// <summary>Entity read length</summary>
    protected int readLength;
    /// <summary>Entity write length</summary>
    protected int writeLength;

    /// <summary>
    /// Read length of the data entity in the context of the sum command
    /// </summary>
    public int ReadLength => this.readLength;

    /// <summary>
    /// Gets the Write length of the data entity in the context of the sum command.
    /// </summary>
    /// <value>The length of the write.</value>
    public int WriteLength => this.writeLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumDataEntity" /> class.
    /// </summary>
    /// <param name="readLength">Length of the read.</param>
    /// <param name="writeLength">Length of the write.</param>
    protected SumDataEntity(int readLength, int writeLength)
    {
      this.readLength = readLength;
      this.writeLength = writeLength;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumDataEntity" /> class.
    /// </summary>
    protected SumDataEntity()
    {
    }

    /// <summary>Sets the length of the write.</summary>
    /// <param name="length">The length.</param>
    /// <remarks>This is used in the case, when the size of the data is not known upfront (e.g. strings), that is only
    /// available during runtime.</remarks>
    internal void SetWriteLength(int length) => this.writeLength = length;
  }
}
