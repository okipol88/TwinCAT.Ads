// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumCreateHandles
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
  /// <summary>
  /// SumCommandBase for getting variable handles by a set of InstancePaths
  /// </summary>
  /// <example>
  /// <code language="C#" title="Usage of Sum commands with handles (CreateHandles, Read, Write, ReleaseHandles)" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMCOMMANDBYHANDLE" />
  /// </example>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" />
  public class SumCreateHandles : SumCommandWrapper<SumReadWrite>
  {
    private IAdsConnection _connection;
    private string[] _instancePaths;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumCreateHandles" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="instancePaths">The instance paths.</param>
    public SumCreateHandles(IAdsConnection connection, IList<string> instancePaths)
    {
      this._connection = connection;
      this._instancePaths = instancePaths.ToArray<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumCreateHandles" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="instancePaths">The instance paths.</param>
    public SumCreateHandles(IAdsConnection connection, string[] instancePaths)
    {
      this._connection = connection;
      this._instancePaths = instancePaths;
    }

    /// <summary>Creates the sum entity infos.</summary>
    /// <returns>IList&lt;SumDataEntityInfo&gt;.</returns>
    private IList<SumDataEntity> CreateSumEntityInfos()
    {
      List<SumDataEntity> sumEntityInfos = new List<SumDataEntity>();
      foreach (string instancePath in this._instancePaths)
      {
        InstancePathSumEntity instancePathSumEntity = (InstancePathSumEntity) new InstancePathSumReadEntity(instancePath, 4);
        sumEntityInfos.Add((SumDataEntity) instancePathSumEntity);
      }
      return (IList<SumDataEntity>) sumEntityInfos;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the TryCreateHandles(out string[] instancePaths, out uint[] handles, out AdsErrorCode[] returnCodes) overload instead!")]
    public AdsErrorCode TryCreateHandles(
      out uint[]? handles,
      out AdsErrorCode[]? returnCodes)
    {
      ISumHandleCollection handles1 = (ISumHandleCollection) null;
      AdsErrorCode handles2 = this.TryCreateHandles(out handles1);
      if (AdsErrorCodeExtensions.Succeeded(handles2))
      {
        returnCodes = new AdsErrorCode[handles1.Count];
        handles = new uint[handles1.Count];
        for (int index = 0; index < handles1.Count; ++index)
        {
          handles[index] = handles1[index].Handle;
          returnCodes[index] = handles1[index].ErrorCode;
        }
      }
      else
      {
        handles = (uint[]) null;
        returnCodes = (AdsErrorCode[]) null;
      }
      return handles2;
    }

    public AdsErrorCode TryCreateHandles(
      out string[]? instancePaths,
      out uint[]? handles,
      out AdsErrorCode[]? returnCodes)
    {
      ISumHandleCollection handles1 = (ISumHandleCollection) null;
      AdsErrorCode handles2 = this.TryCreateHandles(out handles1);
      if (AdsErrorCodeExtensions.Succeeded(handles2))
      {
        returnCodes = new AdsErrorCode[handles1.Count];
        handles = new uint[handles1.Count];
        for (int index = 0; index < handles1.Count; ++index)
        {
          handles[index] = handles1[index].Handle;
          returnCodes[index] = handles1[index].ErrorCode;
        }
        instancePaths = this._instancePaths;
      }
      else
      {
        instancePaths = (string[]) null;
        handles = (uint[]) null;
        returnCodes = (AdsErrorCode[]) null;
      }
      return handles2;
    }

    /// <summary>Tries to create the Handles.</summary>
    /// <param name="handles">The handles.</param>
    /// <returns>AdsErrorCode.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AdsErrorCode TryCreateHandles(out ISumHandleCollection? handles)
    {
      IList<SumDataEntity> sumEntityInfos = this.CreateSumEntityInfos();
      handles = (ISumHandleCollection) null;
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      IList<ReadOnlyMemory<byte>> readData = (IList<ReadOnlyMemory<byte>>) null;
      byte[] numArray = new byte[sumEntityInfos.Select<SumDataEntity, int>((Func<SumDataEntity, int>) (i => i.WriteLength)).Sum()];
      int start = 0;
      foreach (InstancePathSumEntity instancePathSumEntity in (IEnumerable<SumDataEntity>) sumEntityInfos)
        start += instancePathSumEntity.Marshal(numArray.AsSpan<byte>(start));
      this.innerCommand = (ISumCommand) new SumReadWrite(this._connection, sumEntityInfos, (SumAccessMode) 3);
      AdsErrorCode handles1 = ((SumReadWrite) this.innerCommand).TryReadWriteRaw((ReadOnlyMemory<byte>) numArray, out readData, out returnCodes);
      if (handles1 == null)
      {
        handles = (ISumHandleCollection) new SumHandleList();
        for (int index = 0; index < sumEntityInfos.Count; ++index)
        {
          uint handle = 0;
          if ((int) returnCodes[index] == 0)
            handle = BinaryPrimitives.ReadUInt32LittleEndian(readData[index].Span);
          handles.Add((SumHandleEntry) new SumHandleInstancePathEntry(this._instancePaths[index], handle, (AdsErrorCode) (int) returnCodes[index]));
        }
      }
      return handles1;
    }

    private int calcWriteDataSize(IList<SumDataEntity> infos) => infos.Select<SumDataEntity, int>((Func<SumDataEntity, int>) (i => i.WriteLength)).Sum();

    /// <summary>Create handles asynchronously.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'ReadWriteRaw' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumHandles" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumCreateHandles.CreateHandles" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumCreateHandles.TryCreateHandles(System.String[]@,System.UInt32[]@,TwinCAT.Ads.AdsErrorCode[]@)" />
    public async Task<ResultSumHandles> CreateHandlesAsync(
      CancellationToken cancel)
    {
      SumCreateHandles sumCreateHandles = this;
      IList<SumDataEntity> infos = sumCreateHandles.CreateSumEntityInfos();
      byte[] numArray = new byte[infos.Select<SumDataEntity, int>((Func<SumDataEntity, int>) (i => i.WriteLength)).Sum()];
      int start = 0;
      foreach (InstancePathSumEntity instancePathSumEntity in (IEnumerable<SumDataEntity>) infos)
        start += instancePathSumEntity.Marshal(numArray.AsSpan<byte>(start));
      sumCreateHandles.innerCommand = (ISumCommand) new SumReadWrite(sumCreateHandles._connection, infos, (SumAccessMode) 3);
      ResultSumReadRaw resultSumReadRaw = await ((SumReadWrite) sumCreateHandles.innerCommand).ReadWriteRawAsync((ReadOnlyMemory<byte>) numArray, cancel).ConfigureAwait(false);
      ISumHandleCollection source = (ISumHandleCollection) new SumHandleList();
      if (((ResultAds) resultSumReadRaw).Succeeded)
      {
        for (int index = 0; index < infos.Count; ++index)
        {
          uint handle = 0;
          if ((int) ((ResultSumCommand) resultSumReadRaw).SubErrors[index] == 0)
            handle = BinaryPrimitives.ReadUInt32LittleEndian(resultSumReadRaw.ReadBlocks[index].Span);
          source.Add((SumHandleEntry) new SumHandleInstancePathEntry(sumCreateHandles._instancePaths[index], handle, (AdsErrorCode) (int) ((ResultSumCommand) resultSumReadRaw).SubErrors[index]));
        }
      }
      else
      {
        for (int index = 0; index < infos.Count; ++index)
          source.Add((SumHandleEntry) new SumHandleInstancePathEntry(sumCreateHandles._instancePaths[index], 0U, ((ResultAds) resultSumReadRaw).ErrorCode));
      }
      uint[] array = source.Select<SumHandleEntry, uint>((Func<SumHandleEntry, uint>) (x => x.Handle)).ToArray<uint>();
      ResultSumHandles handlesAsync = (ResultSumHandles) new ResultSumHandles2(((ResultAds) resultSumReadRaw).ErrorCode, ((ResultSumCommand) resultSumReadRaw).SubErrors, array, sumCreateHandles._instancePaths);
      infos = (IList<SumDataEntity>) null;
      return handlesAsync;
    }

    /// <summary>Creates the ADS handles.</summary>
    /// <returns>System.UInt32[].</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumGetHandles failed!</exception>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumGetHandlesCommand failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumCreateHandles.TryCreateHandles(System.String[]@,System.UInt32[]@,TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumCreateHandles.CreateHandlesAsync(System.Threading.CancellationToken)" />
    public uint[] CreateHandles()
    {
      uint[] handles = (uint[]) null;
      string[] instancePaths = (string[]) null;
      this.TryCreateHandles(out instancePaths, out handles, out AdsErrorCode[] _);
      if (this.Failed)
        throw new AdsSumCommandException("SumGetHandles failed!", (ISumCommand) this);
      return handles;
    }
  }
}
