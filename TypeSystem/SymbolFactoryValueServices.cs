// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.SymbolFactoryValueServices
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Class SymbolFactoriesContainer (for internal use only)
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SymbolFactoryValueServices : SymbolFactoryServices, ISymbolFactoryValueServices
  {
    /// <summary>Value accessor</summary>
    private IAccessorRawValue _valueAccessor;
    /// <summary>The session</summary>
    private ISession? _session;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public SymbolFactoryValueServices(
      IBinder binder,
      ISymbolFactory factory,
      IAccessorRawValue accessor,
      ISession? session)
      : base(binder, factory)
    {
      this._valueAccessor = accessor;
      this._session = session;
    }

    /// <summary>Gets the value accessor.</summary>
    /// <value>The value accessor.</value>
    /// <exclude />
    public IAccessorRawValue ValueAccessor => this._valueAccessor;

    /// <summary>Gets the session.</summary>
    /// <value>The session or NULL if not available</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public ISession Session => this._session;
  }
}
