// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumInstancePathAnyTypeRead
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// SumRead Command that uses the instancePath as symbol ID / Address and returns objects as Values (ANY Type marshaling)
  /// </summary>
  /// <example>
  ///   <code language="C#" title="Usage of the SumInstancePathAnyTypeRead SumCommand" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMSYMBOLANYTYPEREAD" />
  /// </example>
  /// <remarks>The return value results will be produced by the <see cref="T:TwinCAT.TypeSystem.AnyTypeMarshaler" /> and must be predefined following the ANY_TYPE concept.
  /// Therefore a list of specifiers must be specified for the <see cref="T:TwinCAT.Ads.SumCommand.SumInstancePathAnyTypeRead" /> constructor.
  /// The internal single value reads are done by <see cref="F:TwinCAT.Ads.IndexGroupSymbolAccess.ValueByName" /> / <see cref="F:TwinCAT.Ads.SumCommand.SumAccessMode.ValueByName" />.
  /// </remarks>
  public class SumInstancePathAnyTypeRead : SumInstancePathCommand<SumReadWrite, string>
  {
    /// <summary>The used type marshaler (ANY_TYPE).</summary>
    private AnyTypeMarshaler _marshaler = new AnyTypeMarshaler();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumRead" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="typeSpecs">List of InstancePath / ANY_TYPE specifiers to specify the types of the return values.</param>
    public SumInstancePathAnyTypeRead(
      IAdsConnection connection,
      IList<(string instancePath, Type type, int[]? args)> typeSpecs)
      : base(connection, typeSpecs)
    {
    }

    private byte[] marshalInstancePathEntities(IList<SumDataEntity> infoList)
    {
      this.innerCommand = (ISumCommand) new SumReadWrite(this.Connection, infoList, (SumAccessMode) 2);
      byte[] array = new byte[infoList.Select<SumDataEntity, int>((Func<SumDataEntity, int>) (i => i.WriteLength)).Sum()];
      int start = 0;
      foreach (InstancePathSumEntity info in (IEnumerable<SumDataEntity>) infoList)
        start += info.Marshal(array.AsSpan<byte>(start));
      return array;
    }

    /// <summary>Creates the information list.</summary>
    /// <returns>IList&lt;SumSymbolInfo&gt;.</returns>
    /// <exclude />
    protected override IList<SumDataEntity> CreateSumEntityInfos()
    {
      List<SumDataEntity> sumEntityInfos = new List<SumDataEntity>();
      foreach ((string instancePath, Type type, int[] args) instancePath in (IEnumerable<(string instancePath, Type type, int[] args)>) this.instancePaths)
      {
        int valueLength = this._marshaler.MarshalSize(instancePath.type, instancePath.args, StringMarshaler.DefaultEncoding);
        InstancePathSumReadEntity pathSumReadEntity = new InstancePathSumReadEntity(instancePath.instancePath, valueLength);
        sumEntityInfos.Add((SumDataEntity) pathSumReadEntity);
      }
      return (IList<SumDataEntity>) sumEntityInfos;
    }

    /// <summary>
    /// Reads all the values as an asynchronous operation in one ADS roundtrip and returns an result object containing all the SumCommand SubResults.
    /// </summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A Task&lt;ResultSumValues2&gt; representing the asynchronous operation.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.ISumRead2.Read" />
    /// <seealso cref="!:TryRead(out object[], out AdsErrorCode[])" />
    public async Task<ResultSumTypedValues<string, object>> ReadAsync(
      CancellationToken cancel)
    {
      SumInstancePathAnyTypeRead instancePathAnyTypeRead = this;
      IList<SumDataEntity> infoList = instancePathAnyTypeRead.CreateSumEntityInfos();
      byte[] writeData = instancePathAnyTypeRead.marshalInstancePathEntities(infoList);
      ResultSumReadRaw resultSumReadRaw = await ((SumReadWrite) instancePathAnyTypeRead.innerCommand).ReadWriteRawAsync((ReadOnlyMemory<byte>) writeData, cancel).ConfigureAwait(false);
      ResultReadValueAccess2<string, object>[] results = instancePathAnyTypeRead.createResults(infoList, ((ResultAds) resultSumReadRaw).ErrorCode, ((ResultAds) resultSumReadRaw).InvokeId, resultSumReadRaw.ReadBlocks, ((ResultSumCommand) resultSumReadRaw).SubErrors);
      ResultSumTypedValues<string, object> resultSumTypedValues = new ResultSumTypedValues<string, object>(((ResultAds) resultSumReadRaw).ErrorCode, results);
      infoList = (IList<SumDataEntity>) null;
      return resultSumTypedValues;
    }

    /// <summary>
    /// Reads all the values in one ADS roundtrip and returns an result object containing all the SumCommand SubResults.
    /// </summary>
    /// <returns>System.Object[].</returns>
    /// <seealso cref="!:TryRead(out object[], out AdsErrorCode[])" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.ISumRead2.ReadAsync(System.Threading.CancellationToken)" />
    /// <remarks>The return values are automatically marshalled to their appropriate .NET types.</remarks>
    public ResultSumTypedValues<string, object> Read() => AsyncHelper.RunSync<ResultSumTypedValues<string, object>>((Func<Task<ResultSumTypedValues<string, object>>>) (() => this.ReadAsync(CancellationToken.None)));

    private ResultReadValueAccess2<string, object>[] createResults(
      IList<SumDataEntity> infoList,
      AdsErrorCode sumResult,
      uint invokeId,
      IList<ReadOnlyMemory<byte>>? readBlocks,
      AdsErrorCode[]? subErrors)
    {
      ResultReadValueAccess2<string, object>[] results = new ResultReadValueAccess2<string, object>[infoList.Count];
      for (int index = 0; index < this.instancePaths.Count; ++index)
      {
        (string instancePath, Type type, int[] args) instancePath = this.instancePaths[index];
        if (subErrors != null)
        {
          if ((int) subErrors[index] == 0)
          {
            object obj = (object) null;
            this._marshaler.Unmarshal(instancePath.type, instancePath.args, readBlocks[index].Span, StringMarshaler.DefaultEncoding, out obj);
            results[index] = new ResultReadValueAccess2<string, object>(instancePath.instancePath, obj, (int) subErrors[index], invokeId);
          }
          else
            results[index] = new ResultReadValueAccess2<string, object>(instancePath.instancePath, (object) null, (int) subErrors[index], invokeId);
        }
        else
          results[index] = new ResultReadValueAccess2<string, object>(instancePath.instancePath, (object) null, 1, invokeId);
      }
      return results;
    }
  }
}
