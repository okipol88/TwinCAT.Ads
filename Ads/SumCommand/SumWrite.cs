// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumWrite
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
  /// <summary>ADS Sum Write Command.</summary>
  /// <remarks>Write data is raw array of byte[].</remarks>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SumWrite : SumCommandBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumWrite" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="sumEntities">The sym information.</param>
    /// <param name="readWriteMode">The read write mode.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SumWrite(
      IAdsConnection connection,
      IList<SumDataEntity> sumEntities,
      SumAccessMode readWriteMode)
      : base(connection, sumEntities, (SumCommandMode) 1, readWriteMode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumWrite" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="readWriteMode">The read write mode.</param>
    protected SumWrite(IAdsConnection connection, SumAccessMode readWriteMode)
      : base(connection, (SumCommandMode) 1, readWriteMode)
    {
    }

    /// <summary>Calculates the length of the read Stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcReadLength() => 4 * this.sumEntities.Count;

    /// <summary>Calculates the length of the write stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcWriteLength()
    {
      int num = this.sumEntities.Count * 12;
      foreach (SumDataEntity sumEntity in (IEnumerable<SumDataEntity>) this.sumEntities)
        num += sumEntity.WriteLength;
      return num;
    }

    public AdsErrorCode TryWriteRaw(
      ReadOnlyMemory<byte> writeData,
      out AdsErrorCode[]? returnCodes)
    {
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      int[] readDataSizes = (int[]) null;
      return this.Execute(new ReadOnlyMemory<byte>?(writeData), out readData, out readDataSizes, out returnCodes);
    }

    /// <summary>Tries to write the data in raw list of byte arrays.</summary>
    /// <param name="writeData">The write data.</param>
    /// <param name="result">The result.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumWrite.WriteRaw(System.ReadOnlyMemory{System.Byte})" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumWrite.WriteRawAsync(System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" />
    public AdsErrorCode TryWriteRaw(
      ReadOnlyMemory<byte> writeData,
      out ResultSumCommand result)
    {
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      int[] readDataSizes = (int[]) null;
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      AdsErrorCode adsErrorCode = this.Execute(new ReadOnlyMemory<byte>?(writeData), out readData, out readDataSizes, out returnCodes);
      result = new ResultSumCommand(adsErrorCode, returnCodes);
      return adsErrorCode;
    }

    /// <summary>Tries to write the data in raw list of byte arrays.</summary>
    /// <param name="writeData">The write data.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'WriteRaw' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumCommand" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumWrite.WriteRaw(System.ReadOnlyMemory{System.Byte})" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumWrite.TryWriteRaw(System.ReadOnlyMemory{System.Byte},TwinCAT.Ads.AdsErrorCode[]@)" />
    public async Task<ResultSumCommand> WriteRawAsync(
      ReadOnlyMemory<byte> writeData,
      CancellationToken cancel)
    {
      return (ResultSumCommand) await this.ExecuteAsync(new ReadOnlyMemory<byte>?(writeData), cancel).ConfigureAwait(false);
    }

    /// <summary>Writes the data in form of raw list of byte arrays.</summary>
    /// <param name="writeData">The write data.</param>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumWriteCommand failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumWrite.TryWriteRaw(System.ReadOnlyMemory{System.Byte},TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumWrite.WriteRawAsync(System.ReadOnlyMemory{System.Byte},System.Threading.CancellationToken)" />
    public void WriteRaw(ReadOnlyMemory<byte> writeData)
    {
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      this.TryWriteRaw(writeData, out returnCodes);
      if (this.Failed)
        throw new AdsSumCommandException("SumWriteCommand failed!", (ISumCommand) this);
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
        return start + this.MarshalSumWriteHeader(igIoSumEntity.IndexGroup, igIoSumEntity.IndexOffset, entity.WriteLength, writer.Slice(start));
      }
      if (this.mode == 1)
      {
        HandleSumEntity handleSumEntity = (HandleSumEntity) entity;
        return start + this.MarshalSumWriteHeader(61445U, handleSumEntity.Handle, entity.WriteLength, writer.Slice(start));
      }
      if (this.mode == 2)
      {
        InstancePathSumEntity instancePathSumEntity = (InstancePathSumEntity) entity;
        return start + instancePathSumEntity.Marshal(writer.Slice(start));
      }
      if (this.mode == 4)
      {
        HandleSumEntity handleSumEntity = (HandleSumEntity) entity;
        return start + this.MarshalSumWriteHeader(61446U, handleSumEntity.Handle, entity.WriteLength, writer.Slice(start));
      }
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
      readDataSizes = new int[this.sumEntities.Count];
      returnCodes = new AdsErrorCode[this.sumEntities.Count];
      readData = (IList<ReadOnlyMemory<byte>>) new List<ReadOnlyMemory<byte>>();
      int start = 0;
      for (int index = 0; index < this.sumEntities.Count; ++index)
      {
        returnCodes[index] = (AdsErrorCode) (int) BinaryPrimitives.ReadUInt32LittleEndian(reader.Slice(start, 4).Span);
        start += 4;
      }
      return start;
    }
  }
}
