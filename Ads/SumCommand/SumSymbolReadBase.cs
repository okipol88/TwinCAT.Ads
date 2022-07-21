// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumSymbolReadBase
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
  /// <summary>Abstract base class for Symbolic ADS Sum read access</summary>
  /// <exclude />
  public abstract class SumSymbolReadBase : SumSymbolCommand<SumReadWrite>, ISumCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumRead" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbols">The symbols to read</param>
    /// <param name="accessMode">The access mode.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected SumSymbolReadBase(
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
      foreach (ISymbol unwrappedSymbol in (IEnumerable<ISymbol>) this.UnwrappedSymbols)
      {
        int valueMarshalSize = unwrappedSymbol.GetValueMarshalSize();
        SumAccessMode accessMode = this.AccessMode;
        SumDataEntity sumDataEntity;
        if (accessMode != null)
        {
          if (accessMode == 2)
          {
            sumDataEntity = (SumDataEntity) new InstancePathSumReadEntity(((IInstance) unwrappedSymbol).InstancePath, valueMarshalSize);
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
        {
          if (!(unwrappedSymbol is IAdsSymbol iadsSymbol))
          {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
            interpolatedStringHandler.AppendLiteral("Symbol '");
            interpolatedStringHandler.AppendFormatted<ISymbol>(unwrappedSymbol);
            interpolatedStringHandler.AppendLiteral("' doesn't support SumAccessMode '");
            interpolatedStringHandler.AppendFormatted<SumAccessMode>(this.AccessMode);
            interpolatedStringHandler.AppendLiteral("'!");
            throw new NotSupportedException(interpolatedStringHandler.ToStringAndClear());
          }
          sumDataEntity = (SumDataEntity) new IgIoSumReadEntity(((IProcessImageAddress) iadsSymbol).IndexGroup, ((IProcessImageAddress) iadsSymbol).IndexOffset, valueMarshalSize);
        }
        sumEntityInfos.Add(sumDataEntity);
      }
      return (IList<SumDataEntity>) sumEntityInfos;
    }

    protected AdsErrorCode OnRead(
      IList<SumDataEntity> infoList,
      out object[]? values,
      out AdsErrorCode[]? returnCodes,
      out IList<ReadOnlyMemory<byte>>? readData)
    {
      if (this.accessMode == 2)
      {
        this.innerCommand = (ISumCommand) new SumReadWrite(this.Connection, infoList, (SumAccessMode) 2);
      }
      else
      {
        if (this.accessMode != null)
          throw new NotImplementedException();
        this.innerCommand = (ISumCommand) new SumRead(this.Connection, infoList, (SumAccessMode) 0);
      }
      values = (object[]) null;
      readData = (IList<ReadOnlyMemory<byte>>) null;
      if (this.accessMode == 2)
        return ((SumReadWrite) this.innerCommand).TryReadWriteRaw((ReadOnlyMemory<byte>) this.marshalInstancePathEntities(infoList), out readData, out returnCodes);
      if (this.accessMode == null)
        return ((SumRead) this.innerCommand).TryReadRaw(out readData, out returnCodes);
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
      interpolatedStringHandler.AppendLiteral("SumAccessMode '");
      interpolatedStringHandler.AppendFormatted<SumAccessMode>(this.AccessMode);
      interpolatedStringHandler.AppendLiteral("' is not supported!");
      throw new NotSupportedException(interpolatedStringHandler.ToStringAndClear());
    }

    /// <summary>Handler function reading the data asynchronously</summary>
    /// <param name="infoList">The information list.</param>
    /// <param name="cancel">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;ResultSumReadRaw&gt; representing the asynchronous operation.</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    protected async Task<ResultSumReadRaw> OnReadAsync(
      IList<SumDataEntity> infoList,
      CancellationToken cancel)
    {
      SumSymbolReadBase sumSymbolReadBase = this;
      if (sumSymbolReadBase.accessMode == 2)
      {
        // ISSUE: explicit non-virtual call
        sumSymbolReadBase.innerCommand = (ISumCommand) new SumReadWrite(__nonvirtual (sumSymbolReadBase.Connection), infoList, (SumAccessMode) 2);
      }
      else
      {
        if (sumSymbolReadBase.accessMode != null)
          throw new NotImplementedException();
        // ISSUE: explicit non-virtual call
        sumSymbolReadBase.innerCommand = (ISumCommand) new SumRead(__nonvirtual (sumSymbolReadBase.Connection), infoList, (SumAccessMode) 0);
      }
      ResultSumReadRaw resultSumReadRaw;
      if (sumSymbolReadBase.accessMode == 2)
      {
        byte[] writeData = sumSymbolReadBase.marshalInstancePathEntities(infoList);
        resultSumReadRaw = await ((SumReadWrite) sumSymbolReadBase.innerCommand).ReadWriteRawAsync((ReadOnlyMemory<byte>) writeData, cancel).ConfigureAwait(false);
      }
      else
      {
        if (sumSymbolReadBase.accessMode != null)
          throw new NotImplementedException();
        resultSumReadRaw = await ((SumRead) sumSymbolReadBase.innerCommand).ReadRawAsync(cancel).ConfigureAwait(false);
      }
      return resultSumReadRaw;
    }

    /// <summary>Creates the SumCommand results (abstract)</summary>
    /// <param name="infoList">The information list.</param>
    /// <param name="readBlocks">The read blocks.</param>
    /// <param name="subErrors">The sub errors.</param>
    /// <returns>ResultReadValueAccess2[].</returns>
    protected abstract ResultValue2<ISymbol, object>[] createResults(
      IList<SumDataEntity> infoList,
      IList<ReadOnlyMemory<byte>> readBlocks,
      AdsErrorCode[] subErrors);

    /// <summary>Marshals the instance path entities.</summary>
    /// <param name="infoList">The information list.</param>
    /// <returns>System.Byte[].</returns>
    private byte[] marshalInstancePathEntities(IList<SumDataEntity> infoList)
    {
      byte[] array = new byte[infoList.Select<SumDataEntity, int>((Func<SumDataEntity, int>) (i => i.WriteLength)).Sum()];
      int start = 0;
      if (this.accessMode == 2)
      {
        foreach (InstancePathSumEntity info in (IEnumerable<SumDataEntity>) infoList)
          start += info.Marshal(array.AsSpan<byte>(start));
      }
      return array;
    }
  }
}
