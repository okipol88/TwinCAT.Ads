// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.SumCommand.SumSymbolCommand`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.ComponentModel;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.SumCommand
{
  /// <summary>
  /// Base class that implements SumAccess for a set of <see cref="T:TwinCAT.TypeSystem.ISymbol" /> objects. (abstract)
  /// </summary>
  /// <remarks>This class gives the choice between different <see cref="P:TwinCAT.Ads.SumCommand.SumSymbolCommand`1.AccessMode">Access modes, but depend on the <see cref="T:TwinCAT.TypeSystem.ISymbol" /> object.</see></remarks>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class SumSymbolCommand<T> : SumCommandWrapper<T> where T : class, ISumCommand
  {
    /// <summary>The connection</summary>
    private IAdsConnection _connection;
    /// <summary>The symbols</summary>
    private IList<ISymbol> symbols;
    /// <summary>The access mode</summary>
    protected SumAccessMode accessMode;

    /// <summary>Gets the connection object.</summary>
    /// <returns>IAdsConnection.</returns>
    protected override IAdsConnection OnGetConnection() => this._connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.SumCommand.SumSymbolCommand`1" /> class.
    /// </summary>
    /// <param name="connection">The ADS Connection.</param>
    /// <param name="symbols">The symbols to access.</param>
    /// <param name="accessMode">The access mode.</param>
    protected SumSymbolCommand(
      IAdsConnection connection,
      IList<ISymbol> symbols,
      SumAccessMode accessMode)
    {
      this.accessMode = accessMode;
      this._connection = connection;
      this.symbols = symbols;
    }

    /// <summary>The symbols</summary>
    protected IList<ISymbol> Symbols => this.symbols;

    /// <summary>Gets the access mode.</summary>
    /// <value>The access mode.</value>
    public SumAccessMode AccessMode => this.accessMode;

    /// <summary>Creates the information list.</summary>
    /// <returns>IList&lt;SumSymbolInfo&gt;.</returns>
    protected abstract IList<SumDataEntity> CreateSumEntityInfos();

    /// <summary>
    /// Gets the unwrapped symbols (Decoupled from DynamicSymbols)
    /// </summary>
    /// <value>The unwrapped symbols.</value>
    protected IList<ISymbol> UnwrappedSymbols
    {
      get
      {
        List<ISymbol> unwrappedSymbols = new List<ISymbol>();
        for (int index = 0; index < ((ICollection<ISymbol>) this.symbols).Count; ++index)
        {
          if (this.symbols[index] is IDynamicSymbol symbol)
            unwrappedSymbols.Add((ISymbol) symbol.Unwrap());
          else
            unwrappedSymbols.Add(this.symbols[index]);
        }
        return (IList<ISymbol>) unwrappedSymbols;
      }
    }

    /// <summary>Gets the value accessor.</summary>
    /// <value>The value accessor.</value>
    protected IAccessorRawValue? ValueAccessor
    {
      get
      {
        IList<ISymbol> unwrappedSymbols = this.UnwrappedSymbols;
        return unwrappedSymbols != null && ((ICollection<ISymbol>) unwrappedSymbols).Count > 0 && unwrappedSymbols[0] is IValueSymbol ivalueSymbol ? ((IValueRawSymbol) ivalueSymbol).ValueAccessor : (IAccessorRawValue) null;
      }
    }
  }
}
