// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AdsSymbolLoader
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
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
  public sealed class AdsSymbolLoader : 
    AdsSymbolProvider,
    IAdsSymbolLoader,
    ISymbolLoader,
    ISymbolProvider,
    ISymbolServer,
    ITypeBinderEvents,
    IDisposable,
    IDynamicSymbolLoader
  {
    /// <summary>Disposed flag</summary>
    private bool _disposed;
    /// <summary>The Loader settings</summary>
    private SymbolLoaderSettings _settings = SymbolLoaderSettings.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolLoader" /> class.
    /// </summary>
    /// <param name="connection">The Connection.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="accessor">The accessor.</param>
    /// <param name="session">The session.</param>
    /// <exception cref="T:System.ArgumentNullException">settings
    /// or
    /// accessor
    /// or
    /// symbolsInfo</exception>
    internal AdsSymbolLoader(
      IAdsConnection connection,
      SymbolLoaderSettings settings,
      IAccessorRawValue accessor,
      ISession session)
      : base(connection)
    {
      if (settings == null)
        throw new ArgumentNullException(nameof (settings));
      if (accessor == null)
        throw new ArgumentNullException(nameof (accessor));
      ISymbolFactory isymbolFactory = settings.SymbolsLoadMode != 2 ? (ISymbolFactory) new SymbolFactory(settings.NonCachedArrayElements) : (ISymbolFactory) new DynamicSymbolFactory((ISymbolFactory) new SymbolFactory(settings.NonCachedArrayElements), settings.NonCachedArrayElements);
      this._settings = settings;
      this.FactoryServices = (ISymbolFactoryServices) new SymbolFactoryValueServices((IBinder) new AdsBinder(connection.Address, (IInternalSymbolProvider) this, isymbolFactory, this.UseVirtualInstances), isymbolFactory, accessor, session);
      isymbolFactory.Initialize(this.FactoryServices);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolLoader" /> class.
    /// </summary>
    ~AdsSymbolLoader() => this.Dispose(false);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (this._disposed)
        return;
      this.Dispose(true);
      this._disposed = true;
      GC.SuppressFinalize((object) this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      ((IDisposable) this.Accessor).Dispose();
    }

    /// <summary>Gets actual the Symbol loader settings</summary>
    /// <value>The access method.</value>
    public ISymbolLoaderSettings Settings => (ISymbolLoaderSettings) this._settings;

    private async Task<ResultAds> readSymbolsAsync(CancellationToken cancel)
    {
      AdsSymbolLoader adsSymbolLoader = this;
      ResultAds.CreateError((AdsErrorCode) 1);
      SymbolsLoadMode symbolsLoadMode = adsSymbolLoader._settings.SymbolsLoadMode;
      if (symbolsLoadMode != null)
      {
        if (symbolsLoadMode - 1 > 1)
          throw new NotSupportedException();
        adsSymbolLoader.SetSymbols((SymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 2));
      }
      else
        adsSymbolLoader.SetSymbols((SymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 1));
      int timeout = ((IConnection) adsSymbolLoader.Connection).Timeout;
      ResultValue<SymbolUploadInfo> resultValue = await adsSymbolLoader.ReadUploadInfoAsync(cancel).ConfigureAwait(false);
      ResultAds resultAds = (ResultAds) resultValue;
      SymbolUploadInfo uploadInfo = resultValue.Value;
      if (uploadInfo != null && uploadInfo.SymbolsBlockSize > 0)
      {
        StringMarshaler symbolsMarshaler = new StringMarshaler(uploadInfo.StringEncoding, (StringConvertMode) 1);
        int num = 65536;
        int symbolCount = 0;
        if (uploadInfo.SymbolsBlockSize < num)
        {
          byte[] symbolBuffer = new byte[uploadInfo.SymbolsBlockSize];
          resultAds = (ResultAds) await ((IAdsReadWrite) adsSymbolLoader.Connection).ReadAsync(61451U, 0U, symbolBuffer.AsMemory<byte>(), cancel).ConfigureAwait(false);
          if (resultAds.Succeeded)
            symbolCount = AdsSymbolParser.ParseSymbols((ReadOnlySpan<byte>) symbolBuffer, symbolsMarshaler, adsSymbolLoader.FactoryServices);
          symbolBuffer = (byte[]) null;
        }
        else
          resultAds = (ResultAds) await CommunicationRetry.BlockwiseReadResizeAsync((Func<Memory<byte>, CancellationToken, Task<ResultRead>>) (async (m, c) => await ((IAdsReadWrite) this.Connection).ReadAsync(61451U, (uint) (int.MinValue | symbolCount), m, c).ConfigureAwait(false)), (Func<bool>) (() => symbolCount >= uploadInfo.SymbolCount), 65536, (Func<ReadOnlyMemory<byte>, AdsErrorCode>) (m =>
          {
            symbolCount += AdsSymbolParser.ParseSymbols(m.Span, symbolsMarshaler, this.FactoryServices);
            return (AdsErrorCode) 0;
          }), cancel).ConfigureAwait(false);
        if (resultAds.Failed)
          adsSymbolLoader.SetSymbols((SymbolCollection<ISymbol>) null);
      }
      return resultAds;
    }

    /// <summary>Loads the symbols.</summary>
    /// <param name="timeout">The timeout.</param>
    private AdsErrorCode tryReadSymbols(TimeSpan timeout)
    {
      SymbolsLoadMode symbolsLoadMode = this._settings.SymbolsLoadMode;
      if (symbolsLoadMode != null)
      {
        if (symbolsLoadMode - 1 > 1)
          throw new NotSupportedException();
        this.SetSymbols((SymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 2));
      }
      else
        this.SetSymbols((SymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 1));
      int timeout1 = ((IConnection) this.Connection).Timeout;
      SymbolUploadInfo info = (SymbolUploadInfo) null;
      AdsErrorCode adsErrorCode1 = this.TryReadUploadInfo(out info);
      if (info != null && info.SymbolsBlockSize > 0)
      {
        byte[] numArray = new byte[info.SymbolsBlockSize];
        ((IConnection) this.Connection).Timeout = (int) timeout.TotalMilliseconds;
        try
        {
          int num1 = 65536;
          if (info.SymbolsBlockSize < num1)
          {
            int num2;
            adsErrorCode1 = ((IAdsReadWrite) this.Connection).TryRead(61451U, 0U, (Memory<byte>) numArray, ref num2);
            if (adsErrorCode1 != null)
              return adsErrorCode1;
          }
          else
          {
            uint num3 = 0;
            int start1 = 0;
            int num4 = 0;
            do
            {
              int num5 = 0;
              AdsErrorCode adsErrorCode2;
              do
              {
                int length = start1 + num1 <= numArray.Length ? num1 : numArray.Length - start1;
                adsErrorCode2 = ((IAdsReadWrite) this.Connection).TryRead(61451U, 2147483648U | num3, numArray.AsMemory<byte>(start1, length), ref num5);
                if (adsErrorCode2 != null)
                {
                  if (adsErrorCode2 != 1797)
                    return adsErrorCode2;
                  num1 *= 4;
                }
              }
              while (adsErrorCode2 != null);
              num1 = 65536;
              int start2 = start1;
              num4 = 0;
              for (int index = BinaryPrimitives.ReadInt32LittleEndian((ReadOnlySpan<byte>) numArray.AsSpan<byte>(start1, 4)); index > 0 && (long) num3 < (long) info.SymbolCount; index = BinaryPrimitives.ReadInt32LittleEndian((ReadOnlySpan<byte>) numArray.AsSpan<byte>(start2, 4)))
              {
                start2 += index;
                ++num3;
                if (start2 >= start1 + num1 || start2 >= numArray.Length)
                  break;
              }
              start1 = start2;
            }
            while ((long) num3 < (long) info.SymbolCount && start1 < numArray.Length);
            adsErrorCode1 = (AdsErrorCode) 0;
          }
          AdsSymbolParser.ParseSymbols((ReadOnlySpan<byte>) numArray, new StringMarshaler(info.StringEncoding, (StringConvertMode) 1), this.FactoryServices);
        }
        catch (Exception ex)
        {
          this.SetSymbols((SymbolCollection<ISymbol>) null);
          AdsModule.Trace.TraceError(ex);
          throw;
        }
        finally
        {
          ((IConnection) this.Connection).Timeout = timeout1;
        }
      }
      return adsErrorCode1;
    }

    private async Task<ResultAds> readTypesAsync(CancellationToken cancel)
    {
      AdsSymbolLoader adsSymbolLoader = this;
      adsSymbolLoader.InternalNamespaces.Clear();
      ResultValue<SymbolUploadInfo> resultValue = await adsSymbolLoader.ReadUploadInfoAsync(cancel).ConfigureAwait(false);
      if (((ResultAds) resultValue).Failed)
        return (ResultAds) resultValue;
      Namespace @namespace = new Namespace(adsSymbolLoader.RootNamespaceName);
      adsSymbolLoader.InternalNamespaces.Add((INamespace<IDataType>) @namespace);
      int targetPointerSize = resultValue.Value.TargetPointerSize;
      if (targetPointerSize > 0)
        adsSymbolLoader.SetPlatformPointerSize(targetPointerSize);
      SymbolUploadInfo uploadInfo = resultValue.Value;
      try
      {
        bool baseTypesContained = uploadInfo.ContainsBaseTypes;
        AdsModule.Trace.TraceInformation("BaseTypes in DataType Stream: {0}", new object[1]
        {
          (object) baseTypesContained
        });
        if (!baseTypesContained)
          adsSymbolLoader.RegisterBuildInTypes();
        int num = 65536;
        if (uploadInfo.DataTypesBlockSize > 0)
        {
          int datatypeCount = 0;
          StringMarshaler symbolMarshaler = new StringMarshaler(uploadInfo.StringEncoding, (StringConvertMode) 1);
          if (uploadInfo.DataTypeCount > 0)
          {
            if (uploadInfo.DataTypesBlockSize < num)
            {
              byte[] dataTypeBuffer = new byte[uploadInfo.DataTypesBlockSize];
              ResultRead resultRead = await ((IAdsReadWrite) adsSymbolLoader.Connection).ReadAsync(61454U, 0U, (Memory<byte>) dataTypeBuffer, cancel).ConfigureAwait(false);
              if (((ResultAds) resultRead).Failed)
                return (ResultAds) resultRead;
              datatypeCount = AdsSymbolParser.ParseTypes((ReadOnlySpan<byte>) dataTypeBuffer.AsSpan<byte>(0, resultRead.ReadBytes), symbolMarshaler, adsSymbolLoader.FactoryServices, baseTypesContained, adsSymbolLoader.InternalBuildInTypes);
              dataTypeBuffer = (byte[]) null;
            }
            else
            {
              ResultRead resultRead1 = await CommunicationRetry.BlockwiseReadAsync((Func<Memory<byte>, CancellationToken, Task<ResultRead>>) ((m, c) => ((IAdsReadWrite) this.Connection).ReadAsync(61454U, (uint) (int.MinValue | datatypeCount), m, c)), (Func<bool>) (() => datatypeCount >= uploadInfo.DataTypeCount), 65536, (Func<ReadOnlyMemory<byte>, AdsErrorCode>) (m =>
              {
                datatypeCount += AdsSymbolParser.ParseTypes(m.Span, symbolMarshaler, this.FactoryServices, baseTypesContained, this.InternalBuildInTypes);
                return (AdsErrorCode) 0;
              }), cancel).ConfigureAwait(false);
            }
          }
        }
        adsSymbolLoader.expandDataTypes();
        if (uploadInfo.ContainsBaseTypes)
          AdsModule.Trace.TraceInformation("Aligning Base types");
      }
      catch (Exception ex)
      {
        adsSymbolLoader.InternalNamespaces.Clear();
        adsSymbolLoader.SetBuildInTypes((DataTypeCollection<IDataType>) null);
        AdsModule.Trace.TraceError(ex);
        return ResultAds.CreateError((AdsErrorCode) 1);
      }
      return ResultAds.CreateError((AdsErrorCode) 0);
    }

    /// <summary>Loads the data types.</summary>
    /// <param name="timeout">The timeout.</param>
    protected override AdsErrorCode TryReadTypes(TimeSpan timeout)
    {
      this.InternalNamespaces.Clear();
      SymbolUploadInfo uploadInfo = (SymbolUploadInfo) null;
      AdsErrorCode adsErrorCode = this.TryReadUploadInfo(out uploadInfo);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode))
      {
        this.InternalNamespaces.Add((INamespace<IDataType>) new Namespace(this.RootNamespaceName));
        int targetPointerSize = uploadInfo.TargetPointerSize;
        if (targetPointerSize > 0)
          this.SetPlatformPointerSize(targetPointerSize);
        try
        {
          bool baseTypesContained = uploadInfo.ContainsBaseTypes;
          AdsModule.Trace.TraceInformation("BaseTypes in DataType Stream: {0}", new object[1]
          {
            (object) baseTypesContained
          });
          if (!baseTypesContained)
            this.RegisterBuildInTypes();
          int num1 = 65536;
          if (uploadInfo.DataTypesBlockSize > 0)
          {
            int datatypeCount = 0;
            StringMarshaler symbolMarshaler = new StringMarshaler(uploadInfo.StringEncoding, (StringConvertMode) 1);
            if (uploadInfo.DataTypeCount > 0)
            {
              if (uploadInfo.DataTypesBlockSize < num1)
              {
                byte[] array = new byte[uploadInfo.DataTypesBlockSize];
                int length = ((IAdsReadWrite2) this.Connection).Read(61454U, 0U, (Memory<byte>) array);
                datatypeCount = AdsSymbolParser.ParseTypes((ReadOnlySpan<byte>) array.AsSpan<byte>(0, length), symbolMarshaler, this.FactoryServices, baseTypesContained, this.InternalBuildInTypes);
              }
              else
                CommunicationRetry.BlockwiseReadResize((Func<Memory<byte>, ResultRead>) (m =>
                {
                  int num2 = 0;
                  return new ResultRead(((IAdsReadWrite) this.Connection).TryRead(61454U, (uint) (int.MinValue | datatypeCount), m, ref num2), num2, 0U);
                }), (Func<bool>) (() => datatypeCount >= uploadInfo.DataTypeCount), 65536, (Func<ReadOnlyMemory<byte>, AdsErrorCode>) (m =>
                {
                  datatypeCount += AdsSymbolParser.ParseTypes(m.Span, symbolMarshaler, this.FactoryServices, baseTypesContained, this.InternalBuildInTypes);
                  return (AdsErrorCode) 0;
                }));
            }
          }
          this.expandDataTypes();
          if (uploadInfo.ContainsBaseTypes)
            AdsModule.Trace.TraceInformation("Aligning Base types");
        }
        catch (Exception ex)
        {
          this.InternalNamespaces.Clear();
          this.SetBuildInTypes((DataTypeCollection<IDataType>) null);
          AdsModule.Trace.TraceError(ex);
          throw;
        }
      }
      return adsErrorCode;
    }

    /// <summary>Expands the so far unresolved datatypes.</summary>
    /// <remarks>Some datatypes must be generated, because they are not available from the Watch server.
    /// This must be done, before the DataTypes collection is accessed, because otherwise the collection can be changed
    /// during enumeration
    /// </remarks>
    private void expandDataTypes()
    {
      DataTypeCollection<IDataType> dataTypeCollection = this.InternalNamespaces.AllTypesInternal.Clone();
      int count1 = dataTypeCollection.Count;
      foreach (IDataType type in dataTypeCollection)
        this.expandDataType(type);
      int count2 = this.InternalNamespaces.Count;
      AdsModule.Trace.TraceInformation("{0} datatypes expanded!", new object[1]
      {
        (object) (count2 - count1)
      });
    }

    private void expandDataType(IDataType type)
    {
      DataTypeCategory dataTypeCategory = type != null ? type.Category : throw new ArgumentNullException(nameof (type));
      if (dataTypeCategory <= 4)
      {
        if (dataTypeCategory != 2)
        {
          if (dataTypeCategory != 4)
            return;
          IDataType elementType = ((IArrayType) type).ElementType;
          if (elementType == null)
            return;
          this.expandDataType(elementType);
        }
        else
        {
          AliasType aliasType = (AliasType) type;
          DataType baseType = (DataType) aliasType.BaseType;
          if (baseType != null)
            this.expandDataType((IDataType) baseType);
          if (aliasType.Size > 0 || baseType == null || !(baseType.ManagedType != (Type) null))
            return;
          aliasType.SetSize(baseType.Size, baseType.ManagedType);
        }
      }
      else if (dataTypeCategory != 13)
      {
        if (dataTypeCategory != 15)
          return;
        IDataType referencedType = ((IReferenceType) type).ReferencedType;
        if (referencedType == null)
          return;
        this.expandDataType(referencedType);
      }
      else
      {
        IDataType referencedType = ((IPointerType) type).ReferencedType;
        if (referencedType == null)
          return;
        this.expandDataType(referencedType);
      }
    }

    /// <summary>Gets the symbols asynchronously</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'GetDataTypes' operation. The <see cref="T:TwinCAT.TypeSystem.ResultSymbols" /> parameter contains the data types
    /// (<see cref="P:TwinCAT.TypeSystem.ResultSymbols`1.Symbols" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    public override async Task<ResultSymbols> GetSymbolsAsync(
      CancellationToken cancel)
    {
      ResultAds resultAds = await this.readTypesSymbolsAsync(cancel).ConfigureAwait(false);
      // ISSUE: reference to a compiler-generated method
      return !resultAds.Succeeded ? ResultSymbols.CreateError(resultAds.ErrorCode) : ResultSymbols.CreateSuccess(this.\u003C\u003En__0());
    }

    /// <summary>Gets the symbols asynchronously</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'GetDataTypes' operation. The <see cref="T:TwinCAT.TypeSystem.ResultDynamicSymbols" /> parameter contains the data types
    /// (<see cref="P:TwinCAT.TypeSystem.ResultSymbols`1.Symbols" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.
    /// </returns>
    public async Task<ResultDynamicSymbols> GetDynamicSymbolsAsync(
      CancellationToken cancel)
    {
      AdsSymbolLoader adsSymbolLoader = this;
      return new ResultDynamicSymbols((await adsSymbolLoader.readTypesSymbolsAsync(cancel).ConfigureAwait(false)).ErrorCode, (IDynamicSymbolsCollection) new DynamicSymbolsCollection((IEnumerable<ISymbol>) ((ISymbolServer) adsSymbolLoader).Symbols));
    }

    /// <summary>Gets the data types asynchronously.</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous 'GetDataTypes' operation. The <see cref="T:TwinCAT.TypeSystem.ResultDataTypes" /> parameter contains the data types
    /// (<see cref="P:TwinCAT.TypeSystem.ResultDataTypes.DataTypes" />) and the <see cref="P:TwinCAT.Ads.ResultAds.ErrorCode" /> after execution.</returns>
    /// <seealso cref="P:TwinCAT.TypeSystem.ISymbolServer.DataTypes" />
    public override async Task<ResultDataTypes> GetDataTypesAsync(
      CancellationToken cancel)
    {
      AdsSymbolLoader adsSymbolLoader = this;
      return new ResultDataTypes((await adsSymbolLoader.readTypesSymbolsAsync(cancel).ConfigureAwait(false)).ErrorCode, (IDataTypeCollection<IDataType>) new ReadOnlyDataTypeCollection(adsSymbolLoader.InternalNamespaces.AllTypesInternal));
    }

    /// <summary>
    /// Indicates that Virtual (created StructInstances) are used.
    /// </summary>
    /// <value>The use virtual instances.</value>
    internal bool UseVirtualInstances
    {
      get
      {
        SymbolsLoadMode symbolsLoadMode = this._settings.SymbolsLoadMode;
        return symbolsLoadMode != null && symbolsLoadMode - 1 <= 1;
      }
    }

    /// <summary>Gets the dynamic Symbols</summary>
    /// <value>The dynamic symbols (when activated)</value>
    /// <remarks>
    /// The Dynamic Symbols can only be returned if the <see cref="F:TwinCAT.SymbolsLoadMode.DynamicTree" /> is active.
    /// </remarks>
    public IDynamicSymbolsCollection SymbolsDynamic
    {
      get
      {
        if (this._settings.SymbolsLoadMode != 2)
          return (IDynamicSymbolsCollection) DynamicSymbolsCollection.Empty;
        this.TryReadTypesSymbols();
        return (IDynamicSymbolsCollection) new DynamicSymbolsCollection((IEnumerable<ISymbol>) ((ISymbolServer) this).Symbols);
      }
    }

    /// <summary>
    /// Loads the data types and symbols into the <see cref="T:TwinCAT.Ads.TypeSystem.AdsSymbolLoader" />
    /// </summary>
    protected override AdsErrorCode TryReadTypesSymbols()
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      if (this.InternalNamespaces.Count == 0)
        adsErrorCode = this.TryReadTypes(this.Timeout);
      if (AdsErrorCodeExtensions.Succeeded(adsErrorCode) && this.InternalSymbols == null)
        adsErrorCode = this.tryReadSymbols(this.Timeout);
      return adsErrorCode;
    }

    private async Task<ResultAds> readTypesSymbolsAsync(CancellationToken cancel)
    {
      AdsSymbolLoader adsSymbolLoader = this;
      ResultAds resultAds = ResultAds.CreateError((AdsErrorCode) 0);
      ConfiguredTaskAwaitable<ResultAds> configuredTaskAwaitable;
      if (adsSymbolLoader.InternalNamespaces.Count == 0)
      {
        configuredTaskAwaitable = adsSymbolLoader.readTypesAsync(cancel).ConfigureAwait(false);
        resultAds = await configuredTaskAwaitable;
        if (resultAds.Failed)
          return resultAds;
      }
      if (adsSymbolLoader.InternalSymbols == null)
      {
        configuredTaskAwaitable = adsSymbolLoader.readSymbolsAsync(cancel).ConfigureAwait(false);
        resultAds = await configuredTaskAwaitable;
      }
      return resultAds;
    }

    /// <summary>
    /// Gets or sets the NotificationSettings that are used for Notification Defaults.
    /// </summary>
    /// <value>The default notification settings.</value>
    public INotificationSettings DefaultNotificationSettings
    {
      get => ((IAccessorNotification) this.Accessor).DefaultNotificationSettings;
      set => ((IAccessorNotification) this.Accessor).DefaultNotificationSettings = value;
    }

    /// <summary>
    /// Gets the (byte) size of Pointers on the attached platform system.
    /// </summary>
    /// <value>The size of the platform pointer.</value>
    public int PlatformPointerSize => ((IDataTypeResolver) this.FactoryServices.Binder).PlatformPointerSize;

    /// <summary>Occurs when new types are generated internally</summary>
    public event EventHandler<DataTypeEventArgs> TypesGenerated
    {
      add => this.Binder.TypesGenerated += value;
      remove => this.Binder.TypesGenerated -= value;
    }

    /// <summary>Occurs when a typename cannot be resolved.</summary>
    public event EventHandler<DataTypeNameEventArgs> TypeResolveError
    {
      add => this.Binder.TypeResolveError += value;
      remove => this.Binder.TypeResolveError -= value;
    }
  }
}
