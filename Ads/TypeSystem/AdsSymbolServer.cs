// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AdsSymbolServer
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Client Side Symbol Server object</summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.ISymbolServer" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AdsSymbolServer : ISymbolServer
  {
    private AdsSessionBase _session;
    private IAdsSymbolLoader? _loader;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolServer" /> class.
    /// </summary>
    /// <param name="session">The session.</param>
    internal AdsSymbolServer(AdsSessionBase session) => this._session = session != null ? session : throw new ArgumentNullException(nameof (session));

    /// <summary>Creates the symbol loader</summary>
    private ISymbolLoader OnCreateLoader()
    {
      if (!this._session.IsConnected)
        throw new SessionNotConnectedException("Cannot create symbol loader!", (ISession) this._session);
      this._loader = (IAdsSymbolLoader) new AdsSymbolLoader((IAdsConnection) this._session.Connection, this._session.Settings.SymbolLoader, (IAccessorRawValue) SymbolLoaderFactory.createValueAccessor((IAdsConnection) this._session.Connection, this._session.Settings.SymbolLoader), (ISession) this._session);
      return (ISymbolLoader) this._loader;
    }

    /// <summary>Creates the loader.</summary>
    private ISymbolLoader createLoader()
    {
      if (this._loader == null)
        this._loader = (IAdsSymbolLoader) this.OnCreateLoader();
      return (ISymbolLoader) this._loader;
    }

    /// <summary>Gets the symbols asynchronously</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'GetDataTypes' operation. The <see cref="T:TwinCAT.TypeSystem.ResultSymbols" /> parameter contains the data types
    /// (<see cref="P:TwinCAT.TypeSystem.ResultSymbols`1.Symbols" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <seealso cref="P:TwinCAT.Ads.TypeSystem.AdsSymbolServer.Symbols" />
    public Task<ResultSymbols> GetSymbolsAsync(CancellationToken cancel) => ((ISymbolServer) this.createLoader()).GetSymbolsAsync(cancel);

    /// <summary>Gets the data types asynchronously.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'GetDataTypes' operation. The <see cref="T:TwinCAT.TypeSystem.ResultDataTypes" /> parameter contains the data types
    /// (<see cref="P:TwinCAT.TypeSystem.ResultDataTypes.DataTypes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <seealso cref="P:TwinCAT.Ads.TypeSystem.AdsSymbolServer.DataTypes" />
    public Task<ResultDataTypes> GetDataTypesAsync(CancellationToken cancel) => ((ISymbolServer) this.createLoader()).GetDataTypesAsync(cancel);

    public AdsErrorCode TryGetSymbols(out ISymbolCollection<ISymbol>? symbols) => ((ISymbolServer) this.createLoader()).TryGetSymbols(ref symbols);

    public AdsErrorCode TryGetDataTypes(out IDataTypeCollection<IDataType>? dataTypes) => ((ISymbolServer) this.createLoader()).TryGetDataTypes(ref dataTypes);

    /// <summary>Gets the data types</summary>
    /// <remarks>
    /// This property reads the DataTypes synchronously, if the data is not available yet. For performance reasons, the asynchronous
    /// counterpart <see cref="M:TwinCAT.Ads.TypeSystem.AdsSymbolServer.GetDataTypesAsync(System.Threading.CancellationToken)" /> should be preferred for the first call.
    /// </remarks>
    /// <value>The data types.</value>
    /// <seealso cref="M:TwinCAT.Ads.TypeSystem.AdsSymbolServer.GetDataTypesAsync(System.Threading.CancellationToken)" />
    public IDataTypeCollection<IDataType> DataTypes
    {
      get
      {
        try
        {
          return ((ISymbolServer) this.createLoader()).DataTypes;
        }
        catch (Exception ex)
        {
          AdsModule.Trace.TraceError(ex);
          throw;
        }
      }
    }

    /// <summary>Gets the symbols.</summary>
    /// <value>The symbols.</value>
    /// <remarks>This property reads the Symbol information synchronously, if the data is not available yet. For performance reasons, the asynchronous
    /// counterpart <see cref="M:TwinCAT.Ads.TypeSystem.AdsSymbolServer.GetSymbolsAsync(System.Threading.CancellationToken)" /> should be preferred for the first call.</remarks>
    /// <seealso cref="M:TwinCAT.Ads.TypeSystem.AdsSymbolServer.GetSymbolsAsync(System.Threading.CancellationToken)" />
    public ISymbolCollection<ISymbol> Symbols
    {
      get
      {
        try
        {
          return ((ISymbolServer) this.createLoader()).Symbols;
        }
        catch (Exception ex)
        {
          AdsModule.Trace.TraceError(ex);
          throw;
        }
      }
    }

    /// <summary>Gets the default value encoding.</summary>
    /// <value>The default value encoding.</value>
    public Encoding DefaultValueEncoding => this._loader != null ? ((ISymbolServer) this._loader).DefaultValueEncoding : StringMarshaler.DefaultEncoding;
  }
}
