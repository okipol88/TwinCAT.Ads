// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumCommandBase
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Abstract base class for ADS Sum Commands.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class SumCommandBase : ISumCommand
  {
    /// <summary>The connection used for communication.</summary>
    /// <remarks>This can be the <seealso cref="T:TwinCAT.Ads.AdsConnection" /> or <see cref="T:TwinCAT.Ads.AdsClient" /> object.</remarks>
    private IAdsConnection connection;
    /// <summary>
    /// List of single entities that build up the SumCommandBase
    /// </summary>
    protected IList<SumDataEntity> sumEntities = (IList<SumDataEntity>) new List<SumDataEntity>();
    /// <summary>Sum Access Mode</summary>
    protected SumAccessMode mode;
    /// <summary>Sum Command Mode</summary>
    private SumCommandMode commandMode;
    /// <summary>IndexGroup Used by the SumCommandBase</summary>
    private AdsReservedIndexGroup ig = (AdsReservedIndexGroup) 61568;
    /// <summary>SumCommand main result.</summary>
    private AdsErrorCode result;
    /// <summary>Sum Command sub results.</summary>
    private AdsErrorCode[]? subResults;
    /// <summary>
    /// Indicates, that the <see cref="N:TwinCAT.Ads.SumCommand" /> has executed already one time.
    /// </summary>
    private bool executed;

    /// <summary>The connection used for communication.</summary>
    /// <remarks>This can be the <seealso cref="T:TwinCAT.Ads.AdsConnection" /> or <see cref="T:TwinCAT.Ads.AdsClient" /> object.</remarks>
    public IAdsConnection Connection => this.connection;

    /// <summary>
    /// Calculates the length of the complete read Stream (all Read data)
    /// </summary>
    /// <returns>System.Int32.</returns>
    protected abstract int calcReadLength();

    /// <summary>
    /// Calculates the length of the complete write stream (all written data)
    /// </summary>
    /// <returns>System.Int32.</returns>
    protected abstract int calcWriteLength();

    /// <summary>
    /// Marshals the header information for a single Sum entity object (for a sum read)
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="bytes">The byte size of the date to Read or Write</param>
    /// <param name="writer">The writer.</param>
    /// <returns>System.Int32.</returns>
    protected int MarshalSumReadHeader(
      uint indexGroup,
      uint indexOffset,
      int bytes,
      Span<byte> writer)
    {
      if (writer == (Span<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (writer));
      int start1 = 0;
      BinaryPrimitives.WriteUInt32LittleEndian(writer.Slice(start1), indexGroup);
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(writer.Slice(start2), indexOffset);
      int start3 = start2 + 4;
      BinaryPrimitives.WriteInt32LittleEndian(writer.Slice(start3), bytes);
      return start3 + 4;
    }

    /// <summary>
    /// Marshals the header information for a single Sum entity object (for a sum write)
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="bytes">The byte size of the date to Read or Write</param>
    /// <param name="writer">The writer.</param>
    /// <returns>System.Int32.</returns>
    protected int MarshalSumWriteHeader(
      uint indexGroup,
      uint indexOffset,
      int bytes,
      Span<byte> writer)
    {
      return this.MarshalSumReadHeader(indexGroup, indexOffset, bytes, writer);
    }

    /// <summary>
    /// Marshals the header information for a single Sum entity object (for a sum read/write)
    /// </summary>
    /// <param name="indexGroup">The index group.</param>
    /// <param name="indexOffset">The index offset.</param>
    /// <param name="readBytes">The read bytes.</param>
    /// <param name="writeBytes">The write bytes.</param>
    /// <param name="writer">The writer.</param>
    /// <returns>System.Int32.</returns>
    protected int MarshalSumReadWriteHeader(
      uint indexGroup,
      uint indexOffset,
      int readBytes,
      int writeBytes,
      Span<byte> writer)
    {
      if (writer == (Span<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (writer));
      int start1 = 0;
      BinaryPrimitives.WriteUInt32LittleEndian(writer.Slice(start1, 4), indexGroup);
      int start2 = start1 + 4;
      BinaryPrimitives.WriteUInt32LittleEndian(writer.Slice(start2, 4), indexOffset);
      int start3 = start2 + 4;
      BinaryPrimitives.WriteInt32LittleEndian(writer.Slice(start3, 4), readBytes);
      int start4 = start3 + 4;
      BinaryPrimitives.WriteInt32LittleEndian(writer.Slice(start4, 4), writeBytes);
      return start4 + 4;
    }

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.Ads.AdsErrorCode" /> of the main SumCommandBase ADS Request
    /// </summary>
    /// <value>The result.</value>
    public AdsErrorCode Result
    {
      get => this.result;
      protected set => this.result = value;
    }

    /// <summary>Gets the sub results of the single Sub Requests.</summary>
    /// <value>The sub results.</value>
    public AdsErrorCode[] SubResults
    {
      get => this.subResults != null ? this.subResults : Array.Empty<AdsErrorCode>();
      protected set => this.subResults = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Ads.SumCommand.ISumCommand" /> was already executed.
    /// </summary>
    /// <value><c>true</c> if executed; otherwise, <c>false</c>.</value>
    public bool Executed
    {
      get => this.executed;
      protected set => this.executed = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Ads.SumCommand.ISumCommand" /> is succeeded.
    /// </summary>
    /// <value><c>true</c> if succeeded; otherwise, <c>false</c>.</value>
    public bool Succeeded
    {
      get
      {
        if (!this.Executed || this.result != null)
          return false;
        foreach (int subResult in this.SubResults)
        {
          if ((AdsErrorCode) subResult != null)
            return false;
        }
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.Ads.SumCommand.ISumCommand" /> faled.
    /// </summary>
    /// <value><c>true</c> if failed; otherwise, <c>false</c>.</value>
    public bool Failed
    {
      get
      {
        if (this.Executed)
        {
          if (this.result != null)
            return true;
          foreach (int subResult in this.SubResults)
          {
            if ((AdsErrorCode) subResult != null)
              return true;
          }
        }
        return false;
      }
    }

    /// <summary>
    /// Gets the first SubError that is not <see cref="F:TwinCAT.Ads.AdsErrorCode.NoError" />
    /// </summary>
    /// <value>The first subError.</value>
    public AdsErrorCode FirstSubError => this.subResults == null ? (AdsErrorCode) 0 : ((IEnumerable<AdsErrorCode>) this.subResults).FirstOrDefault<AdsErrorCode>((Func<AdsErrorCode, bool>) (s => s > 0));

    /// <summary>
    /// Gets the Overall (combined error) from SumCommand AND SubCommands.
    /// </summary>
    /// <remarks>This will return the ErrorCode of the Sumcommand (if failed) or the first failed subcommand.</remarks>
    /// <value>The combined error or <see cref="F:TwinCAT.Ads.AdsErrorCode.NoError" /></value>
    public AdsErrorCode OverallError
    {
      get
      {
        if (!this.executed)
          return (AdsErrorCode) 0;
        return AdsErrorCodeExtensions.Succeeded(this.result) ? this.FirstSubError : this.result;
      }
    }

    /// <summary>
    /// Gets a value indicating, whether the overall SumCommand succeeded (including all subcommands)
    /// </summary>
    /// <value><c>true</c> if [sub results succeeded]; otherwise, <c>false</c>.</value>
    public bool OverallSucceeded => this.executed && this.result == null && this.FirstSubError == 0;

    /// <summary>
    /// Gets a value indicating, whether the overall Sumcommand failed (checking all subcommand results).
    /// </summary>
    /// <value><c>true</c> if [overall failed]; otherwise, <c>false</c>.</value>
    public bool OverallFailed
    {
      get
      {
        if (!this.executed)
          return false;
        return this.result != null || this.FirstSubError > 0;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="N:TwinCAT.Ads.SumCommand" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="sumEntities">The sym information.</param>
    /// <param name="accessMode">The access mode.</param>
    /// <param name="readWriteMode">The read write mode.</param>
    protected SumCommandBase(
      IAdsConnection connection,
      IList<SumDataEntity> sumEntities,
      SumCommandMode accessMode,
      SumAccessMode readWriteMode)
    {
      this.connection = connection;
      this.sumEntities = sumEntities;
      this.mode = readWriteMode;
      this.commandMode = accessMode;
      switch ((int) accessMode)
      {
        case 0:
          this.ig = (AdsReservedIndexGroup) 61568;
          break;
        case 1:
          this.ig = (AdsReservedIndexGroup) 61569;
          break;
        case 2:
          this.ig = (AdsReservedIndexGroup) 61570;
          break;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="N:TwinCAT.Ads.SumCommand" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="commandMode">The command mode.</param>
    /// <param name="readWriteMode">The access mode.</param>
    protected SumCommandBase(
      IAdsConnection connection,
      SumCommandMode commandMode,
      SumAccessMode readWriteMode)
    {
      this.connection = connection;
      this.mode = readWriteMode;
      this.commandMode = commandMode;
      switch ((int) commandMode)
      {
        case 0:
          this.ig = (AdsReservedIndexGroup) 61568;
          break;
        case 1:
          this.ig = (AdsReservedIndexGroup) 61569;
          break;
        case 2:
          this.ig = (AdsReservedIndexGroup) 61570;
          break;
        case 5:
          this.ig = (AdsReservedIndexGroup) 61573;
          break;
        case 6:
          this.ig = (AdsReservedIndexGroup) 61574;
          break;
        default:
          throw new NotImplementedException();
      }
    }

    protected AdsErrorCode Execute(
      ReadOnlyMemory<byte>? writeData,
      out IList<ReadOnlyMemory<byte>>? readData,
      out int[]? readDataSizes,
      out AdsErrorCode[]? returnCodes)
    {
      this.executed = false;
      this.result = (AdsErrorCode) 0;
      readData = (IList<ReadOnlyMemory<byte>>) null;
      readDataSizes = (int[]) null;
      this.subResults = (AdsErrorCode[]) null;
      int length = this.calcReadLength();
      byte[] array = new byte[this.calcWriteLength()];
      byte[] reader = new byte[length];
      int start = 0;
      foreach (SumDataEntity sumEntity in (IEnumerable<SumDataEntity>) this.sumEntities)
        start += this.OnWriteSumEntityData(sumEntity, array.AsSpan<byte>(start));
      if (writeData.HasValue)
      {
        writeData.Value.Span.CopyTo(array.AsSpan<byte>(start));
        int num = start + writeData.Value.Length;
      }
      int num1 = 0;
      this.result = ((IAdsReadWrite) this.connection).TryReadWrite((uint) this.ig, (uint) this.sumEntities.Count, (Memory<byte>) reader, (ReadOnlyMemory<byte>) array, ref num1);
      if (this.result == null)
        this.OnReadReturnData((ReadOnlyMemory<byte>) reader, out readData, out readDataSizes, out this.subResults);
      else
        this.subResults = Enumerable.Repeat<AdsErrorCode>((AdsErrorCode) 1, this.sumEntities.Count).ToArray<AdsErrorCode>();
      this.executed = true;
      returnCodes = this.subResults;
      return this.result;
    }

    /// <summary>
    /// Asynchronously executes the <see cref="N:TwinCAT.Ads.SumCommand" />.
    /// </summary>
    /// <param name="writeData">The data to write.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'ExecuteAsync' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumReadRaw" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    protected async Task<ResultSumReadRaw> ExecuteAsync(
      ReadOnlyMemory<byte>? writeData,
      CancellationToken cancel)
    {
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      int[] readDataSizes = (int[]) null;
      this.executed = false;
      this.result = (AdsErrorCode) 0;
      this.subResults = (AdsErrorCode[]) null;
      int length1 = this.calcReadLength();
      int length2 = this.calcWriteLength();
      byte[] readBuffer = new byte[length1];
      byte[] array = new byte[length2];
      int start = 0;
      foreach (SumDataEntity sumEntity in (IEnumerable<SumDataEntity>) this.sumEntities)
        start += this.OnWriteSumEntityData(sumEntity, array.AsSpan<byte>(start));
      if (writeData.HasValue)
      {
        writeData.Value.Span.CopyTo(array.AsSpan<byte>(start));
        int num = start + writeData.Value.Length;
      }
      int num1 = 0;
      ResultReadWrite resultReadWrite = await ((IAdsReadWrite) this.connection).ReadWriteAsync((uint) this.ig, (uint) this.sumEntities.Count, readBuffer.AsMemory<byte>(), (ReadOnlyMemory<byte>) array.AsMemory<byte>(), cancel).ConfigureAwait(false);
      this.result = ((ResultAds) resultReadWrite).ErrorCode;
      if (((ResultAds) resultReadWrite).Succeeded)
        num1 = this.OnReadReturnData((ReadOnlyMemory<byte>) readBuffer, out readData, out readDataSizes, out this.subResults);
      else
        this.subResults = Enumerable.Repeat<AdsErrorCode>((AdsErrorCode) 1, this.sumEntities.Count).ToArray<AdsErrorCode>();
      this.executed = true;
      ResultSumReadRaw resultSumReadRaw = new ResultSumReadRaw(this.result, this.subResults, readData);
      readData = (IList<ReadOnlyMemory<byte>>) null;
      readBuffer = (byte[]) null;
      return resultSumReadRaw;
    }

    /// <summary>Marshals (writes) a single entitiy.</summary>
    /// <param name="entity">Single entity description.</param>
    /// <param name="writeData">The data to write.</param>
    /// <returns>System.Int32.</returns>
    protected abstract int OnWriteSumEntityData(SumDataEntity entity, Span<byte> writeData);

    /// <summary>Unmarshals (reads) the result from the Reader.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="readData">The read data.</param>
    /// <param name="readDataSizes">The read data sizes.</param>
    /// <param name="returnCodes">The return codes.</param>
    protected abstract int OnReadReturnData(
      ReadOnlyMemory<byte> reader,
      out IList<ReadOnlyMemory<byte>> readData,
      out int[] readDataSizes,
      out AdsErrorCode[] returnCodes);
  }
}
