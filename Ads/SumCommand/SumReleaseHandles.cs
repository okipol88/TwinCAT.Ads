// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumReleaseHandles
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Release Handles SumCommandBase.</summary>
  /// <remarks>Releases the specified ADS handles. Usually used in conjunction with the <see cref="T:TwinCAT.Ads.SumCommand.SumCreateHandles" /> and the <see cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" /> / <see cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" /> commands.
  /// </remarks>
  /// <example>
  /// <code language="C#" title="Usage of Sum commands with handles (CreateHandles, Read, Write, ReleaseHandles)" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMCOMMANDBYHANDLE" />
  /// </example>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumCreateHandles" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" />
  public class SumReleaseHandles : SumCommandWrapper<SumWrite>
  {
    /// <summary>The connection</summary>
    private IAdsConnection _connection;
    /// <summary>The handles to release.</summary>
    private uint[] _serverHandles;
    private PrimitiveTypeMarshaler _converter = PrimitiveTypeMarshaler.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="serverHandles">The handles.</param>
    public SumReleaseHandles(IAdsConnection connection, uint[] serverHandles)
    {
      this._connection = connection;
      this._serverHandles = serverHandles;
    }

    /// <summary>Creates the sum entity infos.</summary>
    /// <returns>IList&lt;SumDataEntityInfo&gt;.</returns>
    /// <exclude />
    private IList<SumDataEntity> CreateSumEntityInfos()
    {
      List<SumDataEntity> sumEntityInfos = new List<SumDataEntity>();
      foreach (uint serverHandle in this._serverHandles)
      {
        HandleSumEntity handleSumEntity = new HandleSumEntity(serverHandle, 0, 4, this._converter);
        sumEntityInfos.Add((SumDataEntity) handleSumEntity);
      }
      return (IList<SumDataEntity>) sumEntityInfos;
    }

    public AdsErrorCode TryReleaseHandles(out AdsErrorCode[]? returnCodes)
    {
      IList<SumDataEntity> sumEntityInfos = this.CreateSumEntityInfos();
      byte[] numArray = new byte[sumEntityInfos.Count * 4];
      int start = 0;
      foreach (HandleSumEntity handleSumEntity in (IEnumerable<SumDataEntity>) sumEntityInfos)
      {
        BinaryPrimitives.WriteUInt32LittleEndian(numArray.AsSpan<byte>(start, 4), handleSumEntity.Handle);
        start += 4;
      }
      this.innerCommand = (ISumCommand) new SumWrite(this._connection, sumEntityInfos, (SumAccessMode) 4);
      return ((SumWrite) this.innerCommand).TryWriteRaw((ReadOnlyMemory<byte>) numArray, out returnCodes);
    }

    /// <summary>Releases the handles.</summary>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumReleaseHandlesCommand failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumReleaseHandles.TryReleaseHandles(TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumReleaseHandles.ReleaseHandlesAsync(System.Threading.CancellationToken)" />
    public void ReleaseHandles()
    {
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      this.TryReleaseHandles(out returnCodes);
      if (this.Failed)
        throw new AdsSumCommandException("SumReleaseHandles failed!", (ISumCommand) this);
    }

    /// <summary>Releases the handles asynchronously.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'ReleaseHandles' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumCommand" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    public Task<ResultSumCommand> ReleaseHandlesAsync(CancellationToken cancel)
    {
      IList<SumDataEntity> sumEntityInfos = this.CreateSumEntityInfos();
      byte[] numArray = new byte[sumEntityInfos.Count * 4];
      int start = 0;
      foreach (HandleSumEntity handleSumEntity in (IEnumerable<SumDataEntity>) sumEntityInfos)
      {
        BinaryPrimitives.WriteUInt32LittleEndian(numArray.AsSpan<byte>(start, 4), handleSumEntity.Handle);
        start += 4;
      }
      this.innerCommand = (ISumCommand) new SumWrite(this._connection, sumEntityInfos, (SumAccessMode) 4);
      return ((SumWrite) this.innerCommand).WriteRawAsync((ReadOnlyMemory<byte>) numArray, cancel);
    }
  }
}
