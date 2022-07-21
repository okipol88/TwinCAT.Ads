// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumRead
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
  /// <summary>The ADS SumRead Command.</summary>
  /// <remarks>Reads the data as Raw list of byte[].</remarks>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SumRead : SumCommandBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumRead" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="sumEntities">The SumCommandBase entities.</param>
    /// <param name="readWriteMode">The read write mode.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SumRead(
      IAdsConnection connection,
      IList<SumDataEntity> sumEntities,
      SumAccessMode readWriteMode)
      : base(connection, sumEntities, (SumCommandMode) 0, readWriteMode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumRead" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="readWriteMode">The read write mode.</param>
    protected SumRead(IAdsConnection connection, SumAccessMode readWriteMode)
      : base(connection, (SumCommandMode) 0, readWriteMode)
    {
    }

    /// <summary>Calculates the length of the read Stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcReadLength()
    {
      int num = 4 * this.sumEntities.Count;
      foreach (SumDataEntity sumEntity in (IEnumerable<SumDataEntity>) this.sumEntities)
        num += sumEntity.ReadLength;
      return num;
    }

    /// <summary>Calculates the length of the write stream.</summary>
    /// <returns>System.Int32.</returns>
    protected override int calcWriteLength()
    {
      int num1 = 0;
      switch ((int) this.mode)
      {
        case 0:
        case 1:
        case 4:
          num1 = this.sumEntities.Count * 12;
          break;
        case 2:
        case 3:
          using (IEnumerator<SumDataEntity> enumerator = this.sumEntities.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              int num2 = 8 + enumerator.Current.WriteLength + 4;
              num1 += num2;
            }
            break;
          }
        default:
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
          interpolatedStringHandler.AppendLiteral("SumAccessMode '");
          interpolatedStringHandler.AppendFormatted<SumAccessMode>(this.mode);
          interpolatedStringHandler.AppendLiteral("' is not supported!");
          throw new NotSupportedException(interpolatedStringHandler.ToStringAndClear());
      }
      return num1;
    }

    public AdsErrorCode TryReadRaw(
      out IList<ReadOnlyMemory<byte>>? readData,
      out AdsErrorCode[]? returnCodes)
    {
      int[] readDataSizes = (int[]) null;
      return this.Execute(new ReadOnlyMemory<byte>?(), out readData, out readDataSizes, out returnCodes);
    }

    /// <summary>Try Read</summary>
    /// <param name="result">The result.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumRead.ReadRaw" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumRead.ReadRawAsync(System.Threading.CancellationToken)" />
    public AdsErrorCode TryReadRaw(out ResultSumReadRaw result)
    {
      int[] readDataSizes = (int[]) null;
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      AdsErrorCode adsErrorCode = this.Execute(new ReadOnlyMemory<byte>?(), out readData, out readDataSizes, out returnCodes);
      result = new ResultSumReadRaw(adsErrorCode, returnCodes, readData);
      return adsErrorCode;
    }

    /// <summary>
    /// Read the <see cref="T:TwinCAT.Ads.SumCommand.SumRead" /> data in raw bytes.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'Read' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumReadRaw" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumRead.ReadRaw" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumRead.TryReadRaw(System.Collections.Generic.IList{System.ReadOnlyMemory{System.Byte}}@,TwinCAT.Ads.AdsErrorCode[]@)" />
    public Task<ResultSumReadRaw> ReadRawAsync(CancellationToken cancel) => this.ExecuteAsync(new ReadOnlyMemory<byte>?(), cancel);

    /// <summary>Reads the values (as list of byte arrays)</summary>
    /// <returns>IList&lt;System.Byte[]&gt;.</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumRead failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumRead.TryReadRaw(System.Collections.Generic.IList{System.ReadOnlyMemory{System.Byte}}@,TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumRead.ReadRawAsync(System.Threading.CancellationToken)" />
    public IList<ReadOnlyMemory<byte>> ReadRaw()
    {
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      this.TryReadRaw(out readData, out returnCodes);
      if (this.Failed)
        throw new AdsSumCommandException("SumRead failed!", (ISumCommand) this);
      return readData;
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
      for (int index = 0; index < this.sumEntities.Count; ++index)
      {
        SumDataEntity sumEntity = this.sumEntities[index];
        readData.Add(reader.Slice(start, sumEntity.ReadLength));
        start += sumEntity.ReadLength;
      }
      return start;
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
        return start + this.MarshalSumReadHeader(igIoSumEntity.IndexGroup, igIoSumEntity.IndexOffset, entity.ReadLength, writer);
      }
      if (this.mode == 1)
      {
        HandleSumEntity handleSumEntity = (HandleSumEntity) entity;
        return start + this.MarshalSumReadHeader(61445U, handleSumEntity.Handle, entity.ReadLength, writer);
      }
      if (this.mode == 2)
      {
        InstancePathSumEntity instancePathSumEntity = (InstancePathSumEntity) entity;
        return start + instancePathSumEntity.Marshal(writer.Slice(start));
      }
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
      interpolatedStringHandler.AppendLiteral("SumAccessMode '");
      interpolatedStringHandler.AppendFormatted<SumAccessMode>(this.mode);
      interpolatedStringHandler.AppendLiteral("' is not supported!");
      throw new NotSupportedException(interpolatedStringHandler.ToStringAndClear());
    }
  }
}
