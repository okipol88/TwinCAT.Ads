// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumSymbolWrite
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>Class for ADS Sum symbolic Write Access.</summary>
  /// <remarks>The <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" /> implements symbolic write access with automatic (dynamic) value marshalling.
  /// The advantage of the symbolic access is (in contrast to the handle access classes <see cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" />,<see cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" />)
  /// that all type information is available when using this ADS Sum Command. The disadvantage is, that the Symbolic information must be loaded beforehand, see examples.
  /// The <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" /> defaults to an IndexGroup/IndexOffset read (<see cref="F:TwinCAT.Ads.SumCommand.SumAccessMode.IndexGroupIndexOffset" />), because the the write access by InstancePath/SymbolName
  /// is not supported. The disadvantage of this mode is, that dereferencing References/Pointers don't work and their values cannot be resolved.
  /// An option is to use the <see cref="F:TwinCAT.Ads.SumCommand.SumAccessMode.ValueByHandle" />, but that means to use more ADS roundtrips because the handles have to be requested first!
  /// </remarks>
  /// <example>
  /// <code language="C#" title="Usage of SumRead/SumSymbolWrite with AdsSession" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMCOMMANDBYSESSION" />
  /// </example>
  /// <example>
  /// <code language="C#" title="Usage of SumRead/SumSymbolWrite with AdsClient" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMCOMMANDBYCLIENT" />
  /// </example>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" />
  public class SumSymbolWrite : SumSymbolCommand<SumWrite>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbols">The symbols to read</param>
    public SumSymbolWrite(IAdsConnection connection, IList<ISymbol> symbols)
      : this(connection, symbols, (SumAccessMode) 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbols">The symbols to read</param>
    /// <param name="accessMode">Sum access mode.</param>
    /// <exclude />
    /// <remarks>A WriteSymbolByName is not implemented by TwinCAT. Therefore, actually only by IndexGroupIndexOffset is possible. Use WriteByHandle instead.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private SumSymbolWrite(
      IAdsConnection connection,
      IList<ISymbol> symbols,
      SumAccessMode accessMode)
      : base(connection, symbols, accessMode)
    {
    }

    /// <summary>Creates the information list.</summary>
    /// <returns>IList&lt;SumSymbolInfo&gt;.</returns>
    /// <exclude />
    protected override IList<SumDataEntity> CreateSumEntityInfos()
    {
      List<SumDataEntity> sumEntityInfos = new List<SumDataEntity>();
      foreach (Symbol unwrappedSymbol in (IEnumerable<ISymbol>) this.UnwrappedSymbols)
      {
        int valueMarshalSize = ((ISymbol) unwrappedSymbol).GetValueMarshalSize();
        SumAccessMode accessMode = this.AccessMode;
        SumDataEntity sumDataEntity;
        if (accessMode != null)
        {
          if (accessMode == 2)
          {
            sumDataEntity = (SumDataEntity) new InstancePathSumWriteEntity(unwrappedSymbol.InstancePath, valueMarshalSize);
          }
          else
          {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
            interpolatedStringHandler.AppendLiteral("SumAccessMode '");
            interpolatedStringHandler.AppendFormatted<SumAccessMode>(this.AccessMode);
            interpolatedStringHandler.AppendLiteral("' is not supported!");
            throw new NotSupportedException(interpolatedStringHandler.ToStringAndClear());
          }
        }
        else
          sumDataEntity = (SumDataEntity) new IgIoSumWriteEntity(unwrappedSymbol.IndexGroup, unwrappedSymbol.IndexOffset, valueMarshalSize);
        sumEntityInfos.Add(sumDataEntity);
      }
      return (IList<SumDataEntity>) sumEntityInfos;
    }

    public AdsErrorCode TryWrite(object[] values, out AdsErrorCode[]? returnCodes)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      IList<SumDataEntity> sumEntityInfos = this.CreateSumEntityInfos();
      this.innerCommand = (ISumCommand) new SumWrite(this.Connection, sumEntityInfos, this.accessMode);
      byte[] numArray = new byte[this.calcEntityWriteLength(sumEntityInfos, values)];
      IList<ISymbol> unwrappedSymbols = this.UnwrappedSymbols;
      InstanceValueMarshaler instanceValueMarshaler = new InstanceValueMarshaler();
      int start = 0;
      for (int index = 0; index < ((ICollection<ISymbol>) unwrappedSymbols).Count; ++index)
      {
        SumDataEntity sumDataEntity = sumEntityInfos[index];
        ISymbol symbol = unwrappedSymbols[index];
        symbol.GetValueMarshalSize();
        start += instanceValueMarshaler.Marshal((IAttributedInstance) symbol, values[index], numArray.AsSpan<byte>(start));
      }
      return ((SumWrite) this.innerCommand).TryWriteRaw((ReadOnlyMemory<byte>) numArray, out returnCodes);
    }

    private int calcEntityWriteLength(IList<SumDataEntity> infoList, object[] val)
    {
      int num = 0;
      IList<ISymbol> unwrappedSymbols = this.UnwrappedSymbols;
      InstanceValueMarshaler instanceValueMarshaler = new InstanceValueMarshaler();
      for (int index = 0; index < ((ICollection<ISymbol>) unwrappedSymbols).Count; ++index)
      {
        SumDataEntity info = infoList[index];
        ISymbol symbol = unwrappedSymbols[index];
        symbol.GetValueMarshalSize();
        num += instanceValueMarshaler.MarshalSize((IInstance) symbol);
      }
      return num;
    }

    /// <summary>Reads the symbol values asynchronously.</summary>
    /// <param name="values">The values.</param>
    /// <param name="cancel">The cancellation token</param>
    /// <returns>An asynchronous task that represents the 'Write' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumCommand" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumSymbolWrite.Write(System.Object[])" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumSymbolWrite.TryWrite(System.Object[],TwinCAT.Ads.AdsErrorCode[]@)" />
    public Task<ResultSumCommand> WriteAsync(
      object[] values,
      CancellationToken cancel)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      IList<SumDataEntity> sumEntityInfos = this.CreateSumEntityInfos();
      this.innerCommand = (ISumCommand) new SumWrite(this.Connection, sumEntityInfos, this.AccessMode);
      byte[] numArray = new byte[((IEnumerable<ISymbol>) this.UnwrappedSymbols).Select<ISymbol, int>((Func<ISymbol, int>) (s => s.GetValueMarshalSize())).Sum()];
      IList<ISymbol> unwrappedSymbols = this.UnwrappedSymbols;
      InstanceValueMarshaler instanceValueMarshaler = new InstanceValueMarshaler();
      int start = 0;
      for (int index = 0; index < ((ICollection<ISymbol>) unwrappedSymbols).Count; ++index)
      {
        SumDataEntity sumDataEntity = sumEntityInfos[index];
        ISymbol symbol = unwrappedSymbols[index];
        symbol.GetValueMarshalSize();
        start += instanceValueMarshaler.Marshal((IAttributedInstance) symbol, values[index], numArray.AsSpan<byte>(start));
      }
      return ((SumWrite) this.innerCommand).WriteRawAsync((ReadOnlyMemory<byte>) numArray, cancel);
    }

    /// <summary>Writes the specified values.</summary>
    /// <remarks>The values will be marshalled automatically to their appropriate ADS types.
    /// </remarks>
    /// <param name="values">The values.</param>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumSymbolWrite failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumSymbolWrite.TryWrite(System.Object[],TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumSymbolWrite.WriteAsync(System.Object[],System.Threading.CancellationToken)" />
    public void Write(object[] values)
    {
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      this.TryWrite(values, out returnCodes);
      if (this.Failed)
        throw new AdsSumCommandException("SumSymbolWrite failed!", (ISumCommand) this);
    }
  }
}
