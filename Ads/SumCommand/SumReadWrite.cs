// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumReadWrite
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Ads SumReadWrite Command.</summary>
  /// 
  ///             Read/Write value data is expected as already marshalled list of byte[].
  ///             <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SumReadWrite : SumCommandBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumReadWrite" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="sumEntities">The sym information.</param>
    /// <param name="readWriteMode">The read write mode.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SumReadWrite(
      IAdsConnection connection,
      IList<SumDataEntity> sumEntities,
      SumAccessMode readWriteMode)
      : base(connection, sumEntities, (SumCommandMode) 2, readWriteMode)
    {
    }

    /// <summary>Calculates the length of the read Stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcReadLength()
    {
      int num = 4 * this.sumEntities.Count + 4 * this.sumEntities.Count;
      foreach (SumDataEntity sumEntity in (IEnumerable<SumDataEntity>) this.sumEntities)
        num += sumEntity.ReadLength;
      return num;
    }

    /// <summary>Calculates the length of the write stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcWriteLength()
    {
      int num = this.sumEntities.Count * 16;
      foreach (SumDataEntity sumEntity in (IEnumerable<SumDataEntity>) this.sumEntities)
        num += sumEntity.WriteLength;
      return num;
    }

    public AdsErrorCode TryReadWriteRaw(
      ReadOnlyMemory<byte> writeData,
      out IList<ReadOnlyMemory<byte>>? readData,
      out AdsErrorCode[]? returnCodes)
    {
      int[] readDataSizes = (int[]) null;
      return this.Execute(new ReadOnlyMemory<byte>?(writeData), out readData, out readDataSizes, out returnCodes);
    }

    /// <summary>Tries the read write.</summary>
    /// <param name="writeData">The write data.</param>
    /// <param name="result">The result.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumReadWrite.ReadWriteRaw(System.ReadOnlyMemory{System.Byte})" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumReadWrite.ReadWriteRawAsync(System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" />
    public AdsErrorCode TryReadWriteRaw(
      ReadOnlyMemory<byte> writeData,
      out ResultSumReadRaw result)
    {
      int[] readDataSizes = (int[]) null;
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      AdsErrorCode adsErrorCode = this.Execute(new ReadOnlyMemory<byte>?(writeData), out readData, out readDataSizes, out returnCodes);
      result = new ResultSumReadRaw(adsErrorCode, returnCodes, readData);
      return adsErrorCode;
    }

    /// <summary>
    /// Processes the <see cref="T:TwinCAT.Ads.SumCommand.SumReadWrite" /> asynchronously.
    /// </summary>
    /// <param name="writeData">The write data.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'ReadWriteRaw' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumCommand" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumReadWrite.ReadWriteRaw(System.ReadOnlyMemory{System.Byte})" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumReadWrite.TryReadWriteRaw(System.ReadOnlyMemory{System.Byte},System.Collections.Generic.IList{System.ReadOnlyMemory{System.Byte}}@,TwinCAT.Ads.AdsErrorCode[]@)" />
    public Task<ResultSumReadRaw> ReadWriteRawAsync(
      ReadOnlyMemory<byte> writeData,
      CancellationToken cancel)
    {
      return this.ExecuteAsync(new ReadOnlyMemory<byte>?(writeData), cancel);
    }

    /// <summary>
    /// Reads/Writes the data in Raw form (as list of byte arrays)
    /// </summary>
    /// <param name="writeData">The write data.</param>
    /// <returns>IList&lt;System.Byte[]&gt;.</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumReadWriteCommand failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumReadWrite.TryReadWriteRaw(System.ReadOnlyMemory{System.Byte},System.Collections.Generic.IList{System.ReadOnlyMemory{System.Byte}}@,TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumReadWrite.ReadWriteRawAsync(System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" />
    public IList<ReadOnlyMemory<byte>> ReadWriteRaw(
      ReadOnlyMemory<byte> writeData)
    {
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      this.TryReadWriteRaw(writeData, out readData, out returnCodes);
      if (this.Failed)
        throw new AdsSumCommandException("SumReadWriteCommand failed!", (ISumCommand) this);
      return readData;
    }

    /// <summary>Marshals (writes) a single entitiy.</summary>
    /// <param name="entity">Single entity description.</param>
    /// <param name="writer">The writer.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.NotSupportedException"></exception>
    protected override int OnWriteSumEntityData(SumDataEntity entity, Span<byte> writer)
    {
      if (entity == null)
        throw new ArgumentNullException(nameof (entity));
      if (writer == (Span<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (writer));
      int start = 0;
      if (this.mode == null)
      {
        IgIoSumEntity igIoSumEntity = (IgIoSumEntity) entity;
        return start + this.MarshalSumReadWriteHeader(igIoSumEntity.IndexGroup, igIoSumEntity.IndexOffset, entity.ReadLength, entity.WriteLength, writer.Slice(start));
      }
      if (this.mode == 1)
        return start + this.MarshalSumReadWriteHeader(61445U, 0U, entity.ReadLength, entity.WriteLength, writer.Slice(start));
      if (this.mode == 3)
        return start + this.MarshalSumReadWriteHeader(61443U, 0U, entity.ReadLength, entity.WriteLength, writer.Slice(start));
      if (this.mode == 2)
        return start + this.MarshalSumReadWriteHeader(61444U, 0U, entity.ReadLength, entity.WriteLength, writer.Slice(start));
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
      interpolatedStringHandler.AppendLiteral("SumAccessMode '");
      interpolatedStringHandler.AppendFormatted<SumAccessMode>(this.mode);
      interpolatedStringHandler.AppendLiteral("' is not supported!");
      throw new NotSupportedException(interpolatedStringHandler.ToStringAndClear());
    }

    /// <summary>Unmarshals (reads) the result from the Reader.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="readData">The read data.</param>
    /// <param name="readDataSizes">The read data sizes.</param>
    /// <param name="returnCodes">The return codes.</param>
    protected override int OnReadReturnData(
      ReadOnlyMemory<byte> reader,
      out IList<ReadOnlyMemory<byte>> readData,
      out int[] readDataSizes,
      out AdsErrorCode[] returnCodes)
    {
      ReadOnlySpan<byte> span = reader.Span;
      readDataSizes = new int[this.sumEntities.Count];
      returnCodes = new AdsErrorCode[this.sumEntities.Count];
      readData = (IList<ReadOnlyMemory<byte>>) new List<ReadOnlyMemory<byte>>();
      int start1 = 0;
      for (int index = 0; index < this.sumEntities.Count; ++index)
      {
        returnCodes[index] = (AdsErrorCode) (int) BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(start1));
        int start2 = start1 + 4;
        readDataSizes[index] = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(start2));
        start1 = start2 + 4;
      }
      for (int index = 0; index < this.sumEntities.Count; ++index)
      {
        SumDataEntity sumEntity = this.sumEntities[index];
        readData.Add(reader.Slice(start1, readDataSizes[index]));
        start1 += readDataSizes[index];
      }
      return start1;
    }
  }
}
