// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.SymbolFactoryServices
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Class SymbolFactoriesContainer (for internal use only)
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SymbolFactoryServices : ISymbolFactoryServices
  {
    /// <summary>Type Binder</summary>
    private IBinder _binder;
    /// <summary>Symbol Factory</summary>
    private ISymbolFactory _symbolFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.SymbolFactoryValueServices" /> class (for internal use only).
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <param name="factory">The loader.</param>
    /// <exception cref="T:System.ArgumentNullException">binder
    /// or
    /// loader
    /// or
    /// accessor</exception>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SymbolFactoryServices(IBinder binder, ISymbolFactory factory)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      this._binder = binder;
      this._symbolFactory = factory;
    }

    /// <summary>Gets the type binder.</summary>
    /// <value>The type binder.</value>
    public IBinder Binder => this._binder;

    /// <summary>Gets the symbol factory.</summary>
    /// <value>The symbol factory.</value>
    public ISymbolFactory SymbolFactory => this._symbolFactory;
  }
}
