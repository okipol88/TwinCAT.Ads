// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumSymbolRead
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Symbolic ADS Sum read access (automatic marshalling)</summary>
  /// <remarks>The <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolRead" /> implements symbolic read access with automatic (dynamic) value marshalling.
  /// The advantage of the symbolic access is (in contrast to the handle access classes <see cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" />,<see cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" />)
  /// that all type information is available when using this ADS Sum Command. The disadvantage is, that the Symbolic information must be loaded beforehand, see examples.
  /// The <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" /> defaults to an InstancPath/SymbolName read (<see cref="F:TwinCAT.Ads.SumCommand.SumAccessMode.ValueByName" />).
  /// </remarks>
  /// <example>
  /// <code language="C#" title="Usage of SumSymbolRead/SumSymbolWrite with AdsSession" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMCOMMANDBYSESSION" />
  /// </example>
  /// <example>
  /// <code language="C#" title="Usage of SumSymbolRead/SumSymbolWrite with AdsClient" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMCOMMANDBYCLIENT" />
  /// </example>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumAnyTypeRead" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" />
  public class SumSymbolRead : SumSymbolReadBase, ISumRead, ISumCommand, ISumRead2<ISymbol>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolRead" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbols">The symbols to read</param>
    public SumSymbolRead(IAdsConnection connection, IList<ISymbol> symbols)
      : this(connection, symbols, (SumAccessMode) 2)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolRead" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbols">The symbols to read</param>
    /// <param name="accessMode">The access mode.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SumSymbolRead(
      IAdsConnection connection,
      IList<ISymbol> symbols,
      SumAccessMode accessMode)
      : base(connection, symbols, accessMode)
    {
    }

    public AdsErrorCode TryRead(out object[]? values, out AdsErrorCode[]? returnCodes)
    {
      IList<SumDataEntity> sumEntityInfos = this.CreateSumEntityInfos();
      IList<ReadOnlyMemory<byte>> readData;
      AdsErrorCode adsErrorCode = this.OnRead(sumEntityInfos, out values, out returnCodes, out readData);
      if (adsErrorCode == null)
      {
        ResultValue2<ISymbol, object>[] results = this.createResults(sumEntityInfos, readData, returnCodes);
        values = ((IEnumerable<ResultValue2<ISymbol, object>>) results).Select<ResultValue2<ISymbol, object>, object>((Func<ResultValue2<ISymbol, object>, object>) (r => ((ResultValue<object>) r).Value)).ToArray<object>();
      }
      return adsErrorCode;
    }

    /// <summary>Reads Symbol values as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'Read' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumValues" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumSymbolRead.Read" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumSymbolRead.TryRead(System.Object[]@,TwinCAT.Ads.AdsErrorCode[]@)" />
    public async Task<ResultSumValues> ReadAsync(CancellationToken cancel)
    {
      SumSymbolRead sumSymbolRead = this;
      object[] values = (object[]) null;
      IList<SumDataEntity> infoList = sumSymbolRead.CreateSumEntityInfos();
      ResultSumReadRaw resultSumReadRaw = await sumSymbolRead.OnReadAsync(infoList, cancel);
      if (((ResultAds) resultSumReadRaw).Succeeded)
        values = ((IEnumerable<ResultValue2<ISymbol, object>>) sumSymbolRead.createResults(infoList, resultSumReadRaw.ReadBlocks, ((ResultSumCommand) resultSumReadRaw).SubErrors)).Select<ResultValue2<ISymbol, object>, object>((Func<ResultValue2<ISymbol, object>, object>) (r => ((ResultValue<object>) r).Value)).ToArray<object>();
      ResultSumValues resultSumValues = new ResultSumValues(((ResultAds) resultSumReadRaw).ErrorCode, ((ResultSumCommand) resultSumReadRaw).SubErrors, values);
      values = (object[]) null;
      infoList = (IList<SumDataEntity>) null;
      return resultSumValues;
    }

    /// <summary>Reads the Values.</summary>
    /// <returns>System.Object[].</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.ISumRead2`1.Read2Async(System.Threading.CancellationToken)" />
    /// <remarks>The return values are automatically marshalled to their appropriate .NET types.</remarks>
    public ResultSumValues2<ISymbol> Read2()
    {
      object[] values = (object[]) null;
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      IList<SumDataEntity> sumEntityInfos = this.CreateSumEntityInfos();
      IList<ReadOnlyMemory<byte>> readData;
      AdsErrorCode adsErrorCode = this.OnRead(sumEntityInfos, out values, out returnCodes, out readData);
      if (!AdsErrorCodeExtensions.Succeeded(adsErrorCode))
        return new ResultSumValues2<ISymbol>(adsErrorCode, (ResultValue2<ISymbol, object>[]) null);
      ResultValue2<ISymbol, object>[] results = this.createResults(sumEntityInfos, readData, returnCodes);
      return new ResultSumValues2<ISymbol>(adsErrorCode, results);
    }

    /// <summary>Read2 as an asynchronous operation.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A Task&lt;ResultSumValues2`1&gt; representing the asynchronous operation.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.ISumRead2`1.Read2" />
    public async Task<ResultSumValues2<ISymbol>> Read2Async(
      CancellationToken cancel)
    {
      SumSymbolRead sumSymbolRead = this;
      IList<SumDataEntity> infoList = sumSymbolRead.CreateSumEntityInfos();
      ResultSumReadRaw resultSumReadRaw = await sumSymbolRead.OnReadAsync(infoList, cancel).ConfigureAwait(false);
      ResultValue2<ISymbol, object>[] results = sumSymbolRead.createResults(infoList, resultSumReadRaw.ReadBlocks, ((ResultSumCommand) resultSumReadRaw).SubErrors);
      ResultSumValues2<ISymbol> resultSumValues2 = new ResultSumValues2<ISymbol>(((ResultAds) resultSumReadRaw).ErrorCode, results);
      infoList = (IList<SumDataEntity>) null;
      return resultSumValues2;
    }

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
      IAccessorValue valueAccessor = this.ValueAccessor as IAccessorValue;
      for (int index = 0; index < ((ICollection<ISymbol>) this.Symbols).Count; ++index)
      {
        ISymbol symbol = this.Symbols[index];
        results[index] = (int) subErrors[index] != 0 ? new ResultValue2<ISymbol, object>(symbol, (AdsErrorCode) (int) subErrors[index], (object) null) : (valueAccessor == null ? new ResultValue2<ISymbol, object>(symbol, (AdsErrorCode) (int) subErrors[index], (object) readBlocks[index].ToArray()) : new ResultValue2<ISymbol, object>(symbol, (AdsErrorCode) (int) subErrors[index], ((IAccessorRawValue) valueAccessor).ValueFactory.CreateValue(symbol, readBlocks[index].Span, DateTimeOffset.Now)));
      }
      return results;
    }

    /// <summary>Reads the Values.</summary>
    /// <remarks>The return values are automatically marshaled to their appropriate .NET types.
    /// </remarks>
    /// <returns>System.Object[].</returns>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumSymbolRead failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumSymbolRead.TryRead(System.Object[]@,TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumSymbolRead.ReadAsync(System.Threading.CancellationToken)" />
    public object[] Read()
    {
      object[] values = (object[]) null;
      this.TryRead(out values, out AdsErrorCode[] _);
      if (this.Failed)
        throw new AdsSumCommandException("SumSymbolRead failed!", (ISumCommand) this);
      return values;
    }
  }
}
