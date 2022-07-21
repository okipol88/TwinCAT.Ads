// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumHandleWrite
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
  /// Write any (primitive) values by Handle SumCommandBase.
  /// </summary>
  /// <remarks>This is an ADS Sum Command to access values by handle information. It is always used in combination with <seealso cref="T:TwinCAT.Ads.SumCommand.SumCreateHandles" /> and <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />.
  /// By design (and in contrast to the symbolic access in <see cref="T:TwinCAT.Ads.SumCommand.SumRead" />, <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolWrite" />) this access method can act only with ADS ANY Type (Primitive) values (disadvantage).
  /// The Advantage is, that no symbolic information must be loaded before accessing the values, see samples:
  /// </remarks>
  /// <example>
  /// <code language="C#" title="Usage of Sum commands with handles (CreateHandles, Read, Write, ReleaseHandles)" source="..\..\Samples\Sample.Ads.AdsClientCore\SumCommandSymbols.cs" region="CODE_SAMPLE_SUMCOMMANDBYHANDLE" />
  /// </example>
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.ISumCommand" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumReleaseHandles" />
  /// <seealso cref="T:TwinCAT.Ads.SumCommand.SumHandleRead" />
  public class SumHandleWrite : SumWrite
  {
    private PrimitiveTypeMarshaler _converter = PrimitiveTypeMarshaler.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="handleTypeDict">The handle type dictionary.</param>
    /// <exclude />
    public SumHandleWrite(IAdsConnection connection, IDictionary<uint, Type> handleTypeDict)
      : base(connection, (SumAccessMode) 1)
    {
      if (handleTypeDict == null)
        throw new ArgumentNullException(nameof (handleTypeDict));
      this.sumEntities = (IList<SumDataEntity>) new List<SumDataEntity>();
      foreach (KeyValuePair<uint, Type> keyValuePair in (IEnumerable<KeyValuePair<uint, Type>>) handleTypeDict)
        this.sumEntities.Add((SumDataEntity) new HandleSumWriteAnyEntity(keyValuePair.Key, keyValuePair.Value, this._converter));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumHandleWrite" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="serverHandles">The handles.</param>
    /// <param name="valueTypes">The value types (ANY/Primitive .NET types only)</param>
    public SumHandleWrite(IAdsConnection connection, uint[] serverHandles, Type[] valueTypes)
      : base(connection, (SumAccessMode) 1)
    {
      if (serverHandles == null)
        throw new ArgumentNullException(nameof (serverHandles));
      if (valueTypes == null)
        throw new ArgumentNullException(nameof (valueTypes));
      this.sumEntities = (IList<SumDataEntity>) new List<SumDataEntity>();
      for (int index = 0; index < serverHandles.Length; ++index)
        this.sumEntities.Add((SumDataEntity) new HandleSumWriteAnyEntity(serverHandles[index], valueTypes[index], this._converter));
    }

    public AdsErrorCode TryWrite(object[] values, out AdsErrorCode[]? returnCodes)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (values.Length != this.sumEntities.Count)
        throw new ArgumentException("Value count mismatch!");
      byte[] numArray = new byte[((IEnumerable<object>) values).Select<object, int>((Func<object, int>) (e => this._converter.MarshalSize(e))).Sum()];
      int start = 0;
      for (int index = 0; index < this.sumEntities.Count; ++index)
      {
        this.sumEntities[index].SetWriteLength(this._converter.MarshalSize(values[index]));
        start += this._converter.Marshal(values[index], numArray.AsSpan<byte>(start));
      }
      return this.TryWriteRaw((ReadOnlyMemory<byte>) numArray, out returnCodes);
    }

    /// <summary>Write the values asynchronously.</summary>
    /// <param name="values">The values.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task that represents the 'ReadWriteRaw' operation and returns a <see cref="T:TwinCAT.Ads.SumCommand.ResultSumCommand" />. The overall error return code
    /// is contained in the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> property.</returns>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumHandleWrite.Write(System.Object[])" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumHandleWrite.TryWrite(System.Object[],TwinCAT.Ads.AdsErrorCode[]@)" />
    public Task<ResultSumCommand> WriteAsync(
      object[] values,
      CancellationToken cancel)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (values.Length != this.sumEntities.Count)
        throw new ArgumentException("Value count mismatch!");
      byte[] numArray = new byte[((IEnumerable<object>) values).Select<object, int>((Func<object, int>) (v => this._converter.MarshalSize(v))).Sum()];
      int start = 0;
      for (int index = 0; index < this.sumEntities.Count; ++index)
      {
        this.sumEntities[index].SetWriteLength(this._converter.MarshalSize(values[index]));
        start += this._converter.Marshal(values[index], numArray.AsSpan<byte>(start));
      }
      return this.WriteRawAsync((ReadOnlyMemory<byte>) numArray, cancel);
    }

    /// <summary>Writes the values to the Symbols.</summary>
    /// <param name="values">The Values (Any primitive types only):</param>
    /// <exception cref="T:TwinCAT.Ads.AdsSumCommandException">SumAnyWriteByHandleCommand failed!</exception>
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumHandleWrite.TryWrite(System.Object[],TwinCAT.Ads.AdsErrorCode[]@)" />
    /// <seealso cref="M:TwinCAT.Ads.SumCommand.SumHandleWrite.WriteAsync(System.Object[],System.Threading.CancellationToken)" />
    public void Write(object[] values)
    {
      AdsErrorCode[] returnCodes = (AdsErrorCode[]) null;
      this.TryWrite(values, out returnCodes);
      if (this.Failed)
        throw new AdsSumCommandException("SumHandleWrite failed!", (ISumCommand) this);
    }
  }
}
