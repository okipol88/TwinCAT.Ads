// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AdsSymbolProvider
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.PlcOpen;
using TwinCAT.TypeSystem;
using TwinCAT.TypeSystem.Generic;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// The class <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolLoader" /> is responsible for downloading the list of declared variables from an ADS Server.
  /// </summary>
  /// <seealso cref="T:TwinCAT.SymbolLoaderSettings" />
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.IAdsSymbolLoader" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IInternalSymbolProvider" />
  /// <seealso cref="T:System.IDisposable" />
  /// <exclude />
  public abstract class AdsSymbolProvider : 
    IInternalSymbolProvider,
    ISymbolProvider,
    ISymbolServer,
    ISymbolFactoryServicesProvider
  {
    /// <summary>The default timeout</summary>
    internal static TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromMilliseconds(30000.0);
    /// <summary>The timeout</summary>
    private TimeSpan _timeout = AdsSymbolProvider.DEFAULT_TIMEOUT;
    /// <summary>The connection</summary>
    private IAdsConnection _connection;
    /// <summary>The symbol factor services</summary>
    private ISymbolFactoryServices? _symbolFactoryServices;
    /// <summary>Namespaces</summary>
    private NamespaceCollection _namespaces = new NamespaceCollection();
    /// <summary>Upload info.</summary>
    private SymbolUploadInfo? _symbolUploadInfo;
    /// <summary>
    /// Contains the Build-In types if types are loaded, null if not.
    /// </summary>
    private DataTypeCollection<IDataType>? _buildInTypes;
    /// <summary>The root namespace</summary>
    private string _rootNamespace = string.Empty;
    /// <summary>Synchronization object</summary>
    protected object syncObject = new object();
    /// <summary>The symbols</summary>
    private SymbolCollection<ISymbol>? _symbols;

    /// <summary>Gets the timeout.</summary>
    /// <value>The timeout.</value>
    protected TimeSpan Timeout => this._timeout;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolProvider" /> class.
    /// </summary>
    protected AdsSymbolProvider(IAdsConnection connection, string rootNamespace)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      this._connection = !AmsAddress.op_Equality(connection.Address, (AmsAddress) null) ? connection : throw new ArgumentException("No connection address!", nameof (connection));
      this._rootNamespace = rootNamespace;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolProvider" /> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <exception cref="T:System.ArgumentNullException">connection</exception>
    /// <exception cref="T:System.ArgumentException">No connection address! - connection</exception>
    protected AdsSymbolProvider(IAdsConnection connection)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      this._connection = !AmsAddress.op_Equality(connection.Address, (AmsAddress) null) ? connection : throw new ArgumentException("No connection address!", nameof (connection));
      this._rootNamespace = ((object) connection.Address).ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolLoader" /> class.
    /// </summary>
    /// <param name="connection">The Connection.</param>
    /// <param name="accessor">The accessor.</param>
    /// <param name="session">The session.</param>
    /// <param name="rootNamespace">The root namespace.</param>
    /// <exception cref="T:System.ArgumentNullException">connection</exception>
    /// <exception cref="T:System.ArgumentNullException">accessor</exception>
    /// <exception cref="T:System.ArgumentNullException">accessor</exception>
    protected AdsSymbolProvider(
      IAdsConnection connection,
      IAccessorRawValue accessor,
      ISession session,
      string rootNamespace)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (accessor == null)
        throw new ArgumentNullException(nameof (accessor));
      this._connection = !AmsAddress.op_Equality(connection.Address, (AmsAddress) null) ? connection : throw new ArgumentException("Connection Address not specified!", nameof (connection));
      this._rootNamespace = rootNamespace;
      ISymbolFactory isymbolFactory = (ISymbolFactory) new TwinCAT.Ads.TypeSystem.SymbolFactory(true);
      this._symbolFactoryServices = (ISymbolFactoryServices) new SymbolFactoryValueServices((IBinder) new AdsBinder(this._connection.Address, (IInternalSymbolProvider) this, isymbolFactory, false), isymbolFactory, accessor, session);
      isymbolFactory.Initialize(this._symbolFactoryServices);
      this._namespaces = new NamespaceCollection();
      this._symbols = (SymbolCollection<ISymbol>) null;
    }

    /// <summary>Initializes this instance.</summary>
    protected virtual void Init()
    {
      this._namespaces = new NamespaceCollection();
      this._symbols = (SymbolCollection<ISymbol>) null;
    }

    /// <summary>Registers the build in types.</summary>
    protected void RegisterBuildInTypes()
    {
      this._buildInTypes = AdsSymbolProvider.CreateBuildInTypes();
      this.Binder.RegisterTypes((IEnumerable<IDataType>) this._buildInTypes);
      this.Binder.OnTypesGenerated((IEnumerable<IDataType>) this._buildInTypes);
    }

    /// <summary>Gets the default value encoding.</summary>
    /// <value>The default value encoding.</value>
    public Encoding DefaultValueEncoding => this.Accessor.DefaultValueEncoding;

    /// <summary>The connection</summary>
    public IAdsConnection Connection => this._connection;

    /// <summary>Gets the binder.</summary>
    /// <value>The binder.</value>
    protected Binder Binder => (Binder) this._symbolFactoryServices.Binder;

    /// <summary>Gets the accessor.</summary>
    /// <value>The accessor.</value>
    /// <exclude />
    protected IAccessorRawValue Accessor => ((ISymbolFactoryValueServices) this._symbolFactoryServices).ValueAccessor;

    /// <summary>Gets the symbol factory.</summary>
    /// <value>The symbol factory.</value>
    protected ISymbolFactory SymbolFactory => this._symbolFactoryServices.SymbolFactory;

    /// <summary>Gets/Sets the SymbolFactoryServices.</summary>
    public ISymbolFactoryServices FactoryServices
    {
      get => this._symbolFactoryServices;
      protected set => this._symbolFactoryServices = value;
    }

    /// <summary>Namespaces</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected NamespaceCollection InternalNamespaces => this._namespaces;

    /// <summary>Upload info.</summary>
    protected SymbolUploadInfo? SymbolUploadInfo => this._symbolUploadInfo;

    /// <summary>Gets the Upload Info object</summary>
    /// <value>The information.</value>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use TryReadUploadInfo() instead!", false)]
    public SymbolUploadInfo? UploadInfo
    {
      get
      {
        SymbolUploadInfo info = (SymbolUploadInfo) null;
        this.TryReadUploadInfo(out info);
        return info;
      }
    }

    /// <summary>
    /// Get the <see cref="P:TwinCAT.Ads.TypeSystem.AdsSymbolProvider.SymbolUploadInfo" /> from the target system.
    /// </summary>
    /// <param name="info">The UploadSymbol information.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryReadUploadInfo(out SymbolUploadInfo? info)
    {
      CancellationToken token = CancellationToken.None;
      if (this._symbolUploadInfo != null)
      {
        info = this._symbolUploadInfo;
        return (AdsErrorCode) 0;
      }
      ResultValue<SymbolUploadInfo> resultValue = AsyncHelper.RunSync<ResultValue<SymbolUploadInfo>>((Func<Task<ResultValue<SymbolUploadInfo>>>) (() => AdsSymbolProvider.loadUploadInfoAsync(this._connection, token)));
      info = resultValue.Value;
      return ((ResultAds) resultValue).ErrorCode;
    }

    /// <summary>Gets the Upload Info object</summary>
    /// <value>The information.</value>
    /// <exclude />
    public async Task<ResultValue<SymbolUploadInfo>> ReadUploadInfoAsync(
      CancellationToken cancel)
    {
      ResultValue<SymbolUploadInfo> resultValue;
      if (this._symbolUploadInfo == null)
      {
        using (CancellationTokenSource timedCancel = new CancellationTokenSource(this._timeout))
        {
          using (CancellationTokenSource combined = CancellationTokenSource.CreateLinkedTokenSource(cancel, timedCancel.Token))
            resultValue = await AdsSymbolProvider.loadUploadInfoAsync(this._connection, combined.Token).ConfigureAwait(false);
        }
        if (((ResultAds) resultValue).Succeeded)
        {
          this._symbolUploadInfo = resultValue.Value;
          if (this._symbolUploadInfo.TargetPointerSize > 0)
            this.SetPlatformPointerSize(this._symbolUploadInfo.TargetPointerSize);
          AdsModule.Trace.TraceInformation(this._symbolUploadInfo.Dump());
        }
      }
      else
        resultValue = new ResultValue<SymbolUploadInfo>((AdsErrorCode) 0, this._symbolUploadInfo);
      return resultValue;
    }

    /// <summary>Resets the cache-contents of this loader.</summary>
    /// <remarks>Accessing the data members will force AdsReads in the following.</remarks>
    public void Reset()
    {
      this._symbolUploadInfo = (SymbolUploadInfo) null;
      this._symbols = (SymbolCollection<ISymbol>) null;
      this._namespaces = new NamespaceCollection();
    }

    /// <summary>Gets the amount of data types used in the target.</summary>
    /// <value>The data type count.</value>
    /// <remarks>This poperty can be used, before uploading all data types and symbols.</remarks>
    public int DataTypeCount
    {
      get
      {
        SymbolUploadInfo info = (SymbolUploadInfo) null;
        this.TryReadUploadInfo(out info);
        return info != null ? info.DataTypeCount : 0;
      }
    }

    /// <summary>Gets the amount of symbols used by the target.</summary>
    /// <value>The symbol count.</value>
    /// <remarks>This poperty can be used, before uploading all data types and symbols.</remarks>
    public int SymbolCount
    {
      get
      {
        SymbolUploadInfo info = (SymbolUploadInfo) null;
        this.TryReadUploadInfo(out info);
        return info != null ? info.SymbolCount : 0;
      }
    }

    /// <summary>
    /// Gets the maximal amount of Dynamic symbols of the target system.
    /// </summary>
    /// <value>The max dynamic symbols.</value>
    public int MaxDynamicSymbolCount
    {
      get
      {
        SymbolUploadInfo info = (SymbolUploadInfo) null;
        this.TryReadUploadInfo(out info);
        return info != null ? info.MaxDynamicSymbolCount : 0;
      }
    }

    /// <summary>Gets the used dynamic symbols by the target system.</summary>
    /// <value>The used dynamic symbols.</value>
    public int UsedDynamicSymbolCount
    {
      get
      {
        SymbolUploadInfo info = (SymbolUploadInfo) null;
        this.TryReadUploadInfo(out info);
        return info != null ? info.MaxDynamicSymbolCount : 0;
      }
    }

    /// <summary>
    /// Gets the default string Encoding of the Symbols and DataTypesw.
    /// </summary>
    /// <value>The string encoding.</value>
    public Encoding StringEncoding
    {
      get
      {
        SymbolUploadInfo info = (SymbolUploadInfo) null;
        this.TryReadUploadInfo(out info);
        return info != null ? info.StringEncoding : StringMarshaler.DefaultEncoding;
      }
    }

    /// <summary>Load upload info asynchronously.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultValue&lt;SymbolUploadInfo&gt;&gt;.</returns>
    /// <exception cref="T:System.ArgumentNullException">client</exception>
    /// <exception cref="T:System.ArgumentNullException">client</exception>
    internal static async Task<ResultValue<SymbolUploadInfo>> loadUploadInfoAsync(
      IAdsConnection connection,
      CancellationToken cancel)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      byte[] buffer = new byte[SymbolUploadInfo.GetMarshalSize(3)];
      ResultRead resultRead = await AdsClientExtensions.ReadWithFallbackAsync(connection, 61455U, 0U, 61452U, (Memory<byte>) buffer, cancel).ConfigureAwait(false);
      ResultValue<SymbolUploadInfo> resultValue1;
      if (((ResultAds) resultRead).Succeeded)
      {
        int version = SymbolUploadInfo.CalcVersion(resultRead.ReadBytes);
        resultValue1 = new ResultValue<SymbolUploadInfo>(((ResultAds) resultRead).ErrorCode, new SymbolUploadInfo(version, (ReadOnlySpan<byte>) buffer));
      }
      else
        resultValue1 = new ResultValue<SymbolUploadInfo>(((ResultAds) resultRead).ErrorCode, new SymbolUploadInfo());
      ResultValue<SymbolUploadInfo> resultValue2 = resultValue1;
      buffer = (byte[]) null;
      return resultValue2;
    }

    /// <summary>Load upload info synchronously.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="ret">The ret.</param>
    /// <returns>Task&lt;ResultValue&lt;SymbolUploadInfo&gt;&gt;.</returns>
    /// <exception cref="T:System.ArgumentNullException">client</exception>
    internal static AdsErrorCode loadUploadInfoSync(
      IAdsConnection connection,
      out SymbolUploadInfo ret)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      byte[] numArray = new byte[SymbolUploadInfo.GetMarshalSize(3)];
      bool flag = false;
      ResultRead resultRead = AdsClientExtensions.ReadWithFallback(connection, 61455U, 0U, numArray.AsMemory<byte>(), 61452U, ref flag);
      if (((ResultAds) resultRead).Succeeded)
      {
        int version = SymbolUploadInfo.CalcVersion(resultRead.ReadBytes);
        ret = new SymbolUploadInfo(version, (ReadOnlySpan<byte>) numArray);
      }
      else
        ret = new SymbolUploadInfo();
      return ((ResultAds) resultRead).ErrorCode;
    }

    /// <summary>Creates the build in types.</summary>
    /// <returns>DataTypeCollection&lt;IDataType&gt;.</returns>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DataTypeCollection<IDataType> CreateBuildInTypes()
    {
      DataTypeCollection<IDataType> buildInTypes = new DataTypeCollection<IDataType>();
      DataType baseType1 = (DataType) new PrimitiveType("TOD", typeof (TOD));
      DataType baseType2 = (DataType) new PrimitiveType("DT", typeof (DT));
      DataType baseType3 = (DataType) new PrimitiveType("UDINT", typeof (uint));
      buildInTypes.Add((IDataType) new BitMappingType("BIT", 1, typeof (bool)));
      buildInTypes.Add((IDataType) new BitMappingType("BIT2", 2, typeof (byte)));
      buildInTypes.Add((IDataType) new BitMappingType("BIT3", 3, typeof (byte)));
      buildInTypes.Add((IDataType) new BitMappingType("BIT4", 4, typeof (byte)));
      buildInTypes.Add((IDataType) new BitMappingType("BIT5", 5, typeof (byte)));
      buildInTypes.Add((IDataType) new BitMappingType("BIT6", 6, typeof (byte)));
      buildInTypes.Add((IDataType) new BitMappingType("BIT7", 7, typeof (byte)));
      buildInTypes.Add((IDataType) new BitMappingType("BIT8", 8, typeof (byte)));
      buildInTypes.Add((IDataType) new PrimitiveType("VOID", typeof (void)));
      buildInTypes.Add((IDataType) new PrimitiveType("SINT", typeof (sbyte)));
      buildInTypes.Add((IDataType) new PrimitiveType("USINT", (PrimitiveTypeFlags) 66, typeof (byte)));
      buildInTypes.Add((IDataType) new PrimitiveType("BYTE", (PrimitiveTypeFlags) 3, typeof (byte)));
      buildInTypes.Add((IDataType) new PrimitiveType("UINT8", (PrimitiveTypeFlags) 66, typeof (byte)));
      buildInTypes.Add((IDataType) new PrimitiveType("INT", typeof (short)));
      buildInTypes.Add((IDataType) new PrimitiveType("INT16", typeof (short)));
      buildInTypes.Add((IDataType) new PrimitiveType("UINT", typeof (ushort)));
      buildInTypes.Add((IDataType) new PrimitiveType("WORD", (PrimitiveTypeFlags) 3, typeof (ushort)));
      buildInTypes.Add((IDataType) new PrimitiveType("UINT16", typeof (ushort)));
      buildInTypes.Add((IDataType) new PrimitiveType("DINT", typeof (int)));
      buildInTypes.Add((IDataType) new PrimitiveType("INT32", typeof (int)));
      buildInTypes.Add((IDataType) baseType3);
      buildInTypes.Add((IDataType) new PrimitiveType("UINT32", typeof (uint)));
      buildInTypes.Add((IDataType) new PrimitiveType("DWORD", (PrimitiveTypeFlags) 65, typeof (uint)));
      buildInTypes.Add((IDataType) new PrimitiveType("REAL", (PrimitiveTypeFlags) 72, typeof (float)));
      buildInTypes.Add((IDataType) new PrimitiveType("FLOAT", (PrimitiveTypeFlags) 72, typeof (float)));
      buildInTypes.Add((IDataType) new PrimitiveType("LREAL", (PrimitiveTypeFlags) 72, typeof (double)));
      buildInTypes.Add((IDataType) new PrimitiveType("DOUBLE", (PrimitiveTypeFlags) 72, typeof (double)));
      buildInTypes.Add((IDataType) new PrimitiveType("BOOL", typeof (bool)));
      buildInTypes.Add((IDataType) new PrimitiveType("TIME", typeof (TIME)));
      buildInTypes.Add((IDataType) baseType1);
      buildInTypes.Add((IDataType) new PrimitiveType("DATE", typeof (DATE)));
      buildInTypes.Add((IDataType) baseType2);
      buildInTypes.Add((IDataType) new PrimitiveType("LTIME", typeof (LTIME)));
      buildInTypes.Add((IDataType) new AliasType("DATE_AND_TIME", baseType2));
      buildInTypes.Add((IDataType) new AliasType("TIME_OF_DAY", baseType1));
      buildInTypes.Add((IDataType) new PrimitiveType("LINT", typeof (long)));
      buildInTypes.Add((IDataType) new PrimitiveType("ULINT", typeof (ulong)));
      buildInTypes.Add((IDataType) new AliasType("OTCID", baseType3));
      return buildInTypes;
    }

    /// <summary>
    /// Contains the Build-In types if types are loaded, null if not.
    /// </summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected DataTypeCollection<IDataType>? InternalBuildInTypes => this._buildInTypes;

    /// <summary>Sets the build in types.</summary>
    /// <param name="types">The types.</param>
    protected void SetBuildInTypes(DataTypeCollection<IDataType>? types) => this._buildInTypes = types;

    /// <summary>Gets the build in types.</summary>
    /// <value>The build in types.</value>
    public IDataTypeCollection BuildInTypes
    {
      get
      {
        AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
        if (this._buildInTypes == null)
          adsErrorCode = this.TryReadTypes(this._timeout);
        return this._buildInTypes != null ? (IDataTypeCollection) new ReadOnlyDataTypeCollection(this._buildInTypes) : (IDataTypeCollection) ReadOnlyDataTypeCollection.Empty;
      }
    }

    /// <summary>Aligns the base types.</summary>
    /// <param name="binder">The binder.</param>
    protected void alignBaseTypes(IBinder binder)
    {
    }

    /// <summary>Gets the base Address of the accessed Process image.</summary>
    /// <value>The image base address.</value>
    public AmsAddress ImageBaseAddress => this._connection.Address;

    /// <summary>
    /// Gets the root namespace name of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolLoader" />.
    /// </summary>
    /// <value>The root namespace.</value>
    public string RootNamespaceName => this._rootNamespace;

    /// <summary>The symbols</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected SymbolCollection<ISymbol>? InternalSymbols => this._symbols;

    /// <summary>Sets the symbols.</summary>
    /// <param name="symbols">The symbols.</param>
    protected void SetSymbols(SymbolCollection<ISymbol>? symbols) => this._symbols = symbols;

    public AdsErrorCode TryGetSymbols(out ISymbolCollection<ISymbol>? symbols)
    {
      AdsErrorCode symbols1 = this.TryReadTypesSymbols();
      symbols = !AdsErrorCodeExtensions.Succeeded(symbols1) ? (ISymbolCollection<ISymbol>) null : (ISymbolCollection<ISymbol>) this._symbols;
      return symbols1;
    }

    public AdsErrorCode TryGetDataTypes(out IDataTypeCollection<IDataType>? dataTypes)
    {
      AdsErrorCode dataTypes1 = this.TryReadTypesSymbols();
      if (AdsErrorCodeExtensions.Succeeded(dataTypes1))
      {
        ReadOnlyDataTypeCollection dataTypeCollection = new ReadOnlyDataTypeCollection(this._namespaces.AllTypesInternal);
        dataTypes = (IDataTypeCollection<IDataType>) dataTypeCollection;
      }
      else
        dataTypes = (IDataTypeCollection<IDataType>) null;
      return dataTypes1;
    }

    /// <summary>Gets the symbols.</summary>
    /// <value>The symbols.</value>
    /// <remarks>This property reads the Symbol information synchronously, if the data is not available yet. For performance reasons, the asynchronous
    /// counterpart <see cref="M:TwinCAT.Ads.TypeSystem.AdsSymbolProvider.GetSymbolsAsync(System.Threading.CancellationToken)" /> should be preferred for the first call.</remarks>
    /// <seealso cref="M:TwinCAT.Ads.TypeSystem.AdsSymbolProvider.GetSymbolsAsync(System.Threading.CancellationToken)" />
    public ISymbolCollection<ISymbol> Symbols
    {
      get
      {
        ISymbolCollection<ISymbol> symbols;
        return AdsErrorCodeExtensions.Succeeded(this.TryGetSymbols(out symbols)) ? symbols : (ISymbolCollection<ISymbol>) ReadOnlySymbolCollection.Empty;
      }
    }

    /// <summary>Loads the data.</summary>
    protected abstract AdsErrorCode TryReadTypesSymbols();

    /// <summary>Gets the (root) symbols of the Symbol provider.</summary>
    /// <value>Read only collection of the Symbols</value>
    ISymbolCollection? IInternalSymbolProvider.SymbolsInternal => (ISymbolCollection) this._symbols;

    /// <summary>
    /// Get the Namespaces of DataTypes for this Symbol provider
    /// </summary>
    /// <value>ReadOnly collection of the namespaces.</value>
    /// <exclude />
    public INamespaceCollection<IDataType> Namespaces
    {
      get
      {
        if (this._namespaces.Count == 0)
          this.TryReadTypes(this._timeout);
        return (INamespaceCollection<IDataType>) this._namespaces.AsReadOnly();
      }
    }

    /// <summary>Reads the data types from the target</summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>AdsErrorCode.</returns>
    protected abstract AdsErrorCode TryReadTypes(TimeSpan timeout);

    /// <summary>
    /// Get the Namespaces of DataTypes for this Symbol provider
    /// </summary>
    /// <value>ReadOnly collection of the namespaces.</value>
    /// <exclude />
    INamespaceCollection IInternalSymbolProvider.NamespacesInternal => (INamespaceCollection) this._namespaces;

    /// <summary>
    /// Gets the root (main) namespace of the Symbol provider.
    /// </summary>
    /// <value>The root namespace.</value>
    public INamespace<IDataType>? RootNamespace
    {
      get
      {
        if (this._namespaces.Count == 0)
          this.TryReadTypes(this._timeout);
        return this._namespaces.Count > 0 ? this._namespaces[0] : (INamespace<IDataType>) null;
      }
    }

    /// <summary>Gets the data types</summary>
    /// <remarks>
    /// This property reads the DataTypes synchronously, if the data is not available yet. For performance reasons, the asynchronous
    /// counterpart <see cref="M:TwinCAT.Ads.TypeSystem.AdsSymbolProvider.GetDataTypesAsync(System.Threading.CancellationToken)" /> should be preferred for the first call.
    /// </remarks>
    /// <value>The data types.</value>
    /// <seealso cref="M:TwinCAT.Ads.TypeSystem.AdsSymbolProvider.GetDataTypesAsync(System.Threading.CancellationToken)" />
    public IDataTypeCollection<IDataType> DataTypes
    {
      get
      {
        IDataTypeCollection<IDataType> dataTypes;
        return AdsErrorCodeExtensions.Succeeded(this.TryGetDataTypes(out dataTypes)) ? dataTypes : (IDataTypeCollection<IDataType>) ReadOnlyDataTypeCollection.Empty;
      }
    }

    IDataTypeCollection? IInternalSymbolProvider.DataTypesInternal => ((INamespaceInternal<IDataType>) this.RootNamespace)?.DataTypesInternal;

    private IInternalSymbolProvider ITypeBinderProvider => (IInternalSymbolProvider) this;

    /// <summary>Gets the Symbol Provider</summary>
    /// <value>The provider.</value>
    public IInternalSymbolProvider Provider => (IInternalSymbolProvider) this;

    /// <summary>Sets the size of the platform pointer.</summary>
    /// <param name="sz">The sz.</param>
    internal void SetPlatformPointerSize(int sz)
    {
      this.Binder.SetPlatformPointerSize(sz);
      AdsModule.Trace.TraceInformation("Platform pointer size -> {0} bytes", new object[1]
      {
        (object) sz
      });
    }

    /// <summary>Gets the symbols asynchronously</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'GetDataTypes' operation. The <see cref="T:TwinCAT.TypeSystem.ResultSymbols" /> parameter contains the data types
    /// (<see cref="P:TwinCAT.TypeSystem.ResultSymbols`1.Symbols" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <seealso cref="P:TwinCAT.Ads.TypeSystem.AdsSymbolProvider.Symbols" />
    public abstract Task<ResultSymbols> GetSymbolsAsync(CancellationToken cancel);

    /// <summary>Gets the data types asynchronously.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;ResultDataTypes&gt;.</returns>
    /// <seealso cref="P:TwinCAT.TypeSystem.ISymbolServer.DataTypes" />
    public abstract Task<ResultDataTypes> GetDataTypesAsync(
      CancellationToken cancel);

    /// <summary>Resets the cached symbolic data.</summary>
    public void ResetCachedSymbolicData()
    {
      this._symbols = (SymbolCollection<ISymbol>) null;
      this._namespaces.Clear();
    }
  }
}
