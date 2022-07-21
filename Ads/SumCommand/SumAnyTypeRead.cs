// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumAnyTypeRead
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


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// Symbolic ADS Sum read access (ANY_TYPE marshalling) on already loaded symbols.
  /// </summary>
  /// <example>
  ///   <code language="C#" title="Usage of the SumAnyTypeRead SumCommand" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMSYMBOLANYTYPEREAD" />
  /// </example>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumRead2`1" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumRead" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumInstancePathAnyTypeRead" />
  /// <remarks>The <see cref="T:TwinCAT.Ads.SumCommand.SumAnyTypeRead" /> implements symbolic read with predefined value marshalling. The requested value types are specified before
  /// calling the read operation an must fit to its symbol definition.
  /// The advantage of the symbolic access is (in contrast to the handle access classes <see cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" />,<see cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" />) or the acess
  /// by instance path (<see cref="T:TwinCAT.Ads.SumCommand.SumInstancePathAnyTypeRead" />) that all type information is available when using this ADS Sum Command. The disadvantage is, that the Symbolic information must be loaded before using this SumCommand, see examples.
  /// The <see cref="T:TwinCAT.Ads.SumCommand.SumAnyTypeRead" /> defaults to an InstancePath/SymbolName read (<see cref="F:TwinCAT.Ads.IndexGroupSymbolAccess.ValueByName" /><see cref="F:TwinCAT.Ads.SumCommand.SumAccessMode.ValueByName" />).</remarks>
  public class SumAnyTypeRead : SumSymbolReadBase, ISumRead2<ISymbol>, ISumCommand
  {
    /// <summary>The ANY_TYPE Marshaler</summary>
    private AnyTypeMarshaler _marshaler = new AnyTypeMarshaler();
    private IList<(ISymbol symbol, Type type, int[]? args)> _symbolSpec;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumAnyTypeRead" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbolSpec">The symbols specifier (ANY_TYPE)</param>
    public SumAnyTypeRead(
      IAdsConnection connection,
      IList<(ISymbol symbol, Type type, int[]? args)> symbolSpec)
      : base(connection, (IList<ISymbol>) ((IEnumerable<(ISymbol, Type, int[])>) symbolSpec).Select<(ISymbol, Type, int[]), ISymbol>((Func<(ISymbol, Type, int[]), ISymbol>) (s => s.symbol)).ToList<ISymbol>(), (SumAccessMode) 2)
    {
      this._symbolSpec = symbolSpec;
    }

    /// <summary>Read as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A Task&lt;ResultSumValues2&gt; representing the asynchronous operation.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.ISumRead2.Read" />
    /// <seealso cref="!:TryRead(out object[], out AdsErrorCode[])" />
    public async Task<ResultSumValues2<ISymbol>> Read2Async(
      CancellationToken cancel)
    {
      SumAnyTypeRead sumAnyTypeRead = this;
      IList<SumDataEntity> infoList = sumAnyTypeRead.CreateSumEntityInfos();
      ResultSumReadRaw resultSumReadRaw = await sumAnyTypeRead.OnReadAsync(infoList, cancel).ConfigureAwait(false);
      ResultValue2<ISymbol, object>[] results = sumAnyTypeRead.createResults(infoList, resultSumReadRaw.ReadBlocks, ((ResultSumCommand) resultSumReadRaw).SubErrors);
      ResultSumValues2<ISymbol> resultSumValues2 = new ResultSumValues2<ISymbol>(((ResultAds) resultSumReadRaw).ErrorCode, results);
      infoList = (IList<SumDataEntity>) null;
      return resultSumValues2;
    }

    /// <summary>Reads the Values.</summary>
    /// <returns>System.Object[].</returns>
    /// <seealso cref="!:TryRead(out object[], out AdsErrorCode[])" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.ISumRead2.ReadAsync(System.Threading.CancellationToken)" />
    /// <remarks>The return values are automatically marshaled to their appropriate .NET types.</remarks>
    public ResultSumValues2<ISymbol> Read2() => AsyncHelper.RunSync<ResultSumValues2<ISymbol>>((Func<Task<ResultSumValues2<ISymbol>>>) (() => this.Read2Async(CancellationToken.None)));

    /// <summary>Creates the SumCommand results (abstract)</summary>
    /// <param name="infoList">The information list.</param>
    /// <param name="readBlocks">The read blocks.</param>
    /// <param name="subErrors">The sub errors.</param>
    /// <returns>ResultReadValueAccess2[].</returns>
    protected override ResultValue2<ISymbol, object>[] createResults(
      IList<SumDataEntity> infoList,
      IList<ReadOnlyMemory<byte>> readBlocks,
      AdsErrorCode[] subErrors)
    {
      ResultValue2<ISymbol, object>[] results = new ResultValue2<ISymbol, object>[infoList.Count];
      IList<ISymbol> unwrappedSymbols = this.UnwrappedSymbols;
      for (int index = 0; index < ((ICollection<ISymbol>) this.Symbols).Count; ++index)
      {
        (ISymbol symbol, Type type, int[] args) tuple = this._symbolSpec[index];
        object obj = (object) null;
        if ((int) subErrors[index] == 0)
        {
          this._marshaler.Unmarshal(tuple.type, tuple.args, readBlocks[index].Span, StringMarshaler.DefaultEncoding, out obj);
          results[index] = new ResultValue2<ISymbol, object>(tuple.symbol, (AdsErrorCode) (int) subErrors[index], obj);
        }
        else
          results[index] = new ResultValue2<ISymbol, object>(tuple.symbol, (AdsErrorCode) (int) subErrors[index], (object) null);
      }
      return results;
    }
  }
}
