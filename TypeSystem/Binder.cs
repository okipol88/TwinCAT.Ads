// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Binder
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Namespace Binder Base Implementation</summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.IBinder" />
  /// <exclude />
  public class Binder : IBinder, IDataTypeResolver, ITypeBinderEvents, ISymbolBinderEvents
  {
    /// <summary>The symbol provider</summary>
    private IInternalSymbolProvider _provider;
    /// <summary>The symbol factory</summary>
    private ISymbolFactory _symbolFactory;
    /// <summary>The platform pointer size</summary>
    private int _platformPointerSize = -1;
    private bool _useVirtualInstances;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.Binder" /> class.
    /// </summary>
    /// <param name="provider">The symbol provider.</param>
    /// <param name="symbolFactory">The symbol factory.</param>
    /// <param name="useVirtualInstances">if set to <c>true</c> [use virtual instances].</param>
    protected Binder(
      IInternalSymbolProvider provider,
      ISymbolFactory symbolFactory,
      bool useVirtualInstances)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      if (symbolFactory == null)
        throw new ArgumentNullException(nameof (symbolFactory));
      if (provider.SymbolsInternal != null && ((IInstanceCollection<ISymbol>) provider.SymbolsInternal).Mode != 2)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Symbol provider has to be in Mode: {0}", (object) (InstanceCollectionMode) 2), nameof (provider));
      this._provider = provider;
      this._useVirtualInstances = useVirtualInstances;
      this._symbolFactory = symbolFactory;
    }

    /// <summary>Gets the Symbol provider.</summary>
    /// <value>The provider.</value>
    public IInternalSymbolProvider Provider => this._provider;

    /// <summary>Gets the size of the platform pointer (in Bytes)</summary>
    /// <value>The size of the platform pointer.</value>
    public int PlatformPointerSize => this._platformPointerSize;

    /// <summary>Sets the size of the platform pointer.</summary>
    /// <param name="sz">The sz.</param>
    internal void SetPlatformPointerSize(int sz)
    {
      this._platformPointerSize = sz;
      AdsModule.Trace.TraceInformation("Platform pointer size -> {0} bytes", new object[1]
      {
        (object) sz
      });
    }

    /// <summary>
    /// Binds the specified symbol to the Resolver (Registering and linking Parents).
    /// </summary>
    /// <param name="subSymbol">The child symbol.</param>
    /// <returns>the parent Symbol where the subSymbol was bound to</returns>
    public ISymbol? Bind(IHierarchicalSymbol subSymbol)
    {
      IHierarchicalSymbol subSymbol1 = subSymbol != null ? (IHierarchicalSymbol) ((ISymbol) subSymbol).Parent : throw new ArgumentNullException(nameof (subSymbol));
      string parentPath;
      string parentName;
      if (subSymbol1 == null && this.UseVirtualInstances && SymbolParser.TryParseParentPath((IInstance) subSymbol, out parentPath, out parentName))
      {
        string indicesStr = (string) null;
        string instanceName = (string) null;
        string fullPathWithoutIndexes;
        IList<int[]> jaggedIndices;
        bool arrayElement = SymbolParser.TryParseArrayElement(parentPath, out fullPathWithoutIndexes, out instanceName, out indicesStr, out jaggedIndices, out ArrayIndexType? _);
        ISymbol symbol = (ISymbol) null;
        if (!((InstanceCollection<ISymbol>) this._provider.SymbolsInternal).TryGetInstanceHierarchically(parentPath, out symbol))
        {
          if (arrayElement)
          {
            ISymbol isymbol = (ISymbol) null;
            if (((IInstanceCollection<ISymbol>) this._provider.SymbolsInternal).TryGetInstance(fullPathWithoutIndexes, ref isymbol))
            {
              IArrayType dataType = (IArrayType) ((IInstance) isymbol).DataType;
              if (dataType != null)
              {
                for (int index = 0; index < jaggedIndices.Count; ++index)
                {
                  subSymbol1 = (IHierarchicalSymbol) this._symbolFactory.CreateArrayElement(dataType, jaggedIndices[index], isymbol);
                  this.Bind(subSymbol1);
                }
              }
              else
                AdsModule.Trace.TraceWarning("Cannot create SubElements of Symbol '" + ((IInstance) isymbol).InstancePath + "'");
            }
            else
              AdsModule.Trace.TraceWarning("Cannot bind subSymbol '{0}'", new object[1]
              {
                (object) ((IInstance) subSymbol).InstancePath
              });
          }
          else
          {
            subSymbol1 = (IHierarchicalSymbol) this._symbolFactory.CreateVirtualStruct(parentName, parentPath, (ISymbol) null);
            this.Bind(subSymbol1);
          }
        }
        else
          subSymbol1 = (IHierarchicalSymbol) symbol;
        if (subSymbol1 != null)
        {
          subSymbol.SetParent((ISymbol) subSymbol1);
          if (subSymbol1 is IVirtualStructInstance ivirtualStructInstance)
            ivirtualStructInstance.AddMember((ISymbol) subSymbol, ivirtualStructInstance);
        }
      }
      if (((ICollection<ISymbol>) this._provider.SymbolsInternal).Contains((ISymbol) subSymbol))
        ((IInstanceInternal) subSymbol).SetInstanceName(this.createUniquePathName((IInstance) subSymbol));
      try
      {
        if (((ISymbol) subSymbol).Parent == null)
          ((ICollection<ISymbol>) this._provider.SymbolsInternal).Add((ISymbol) subSymbol);
      }
      catch (ArgumentException ex)
      {
        string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot bind symbol '{0}' because of double registration. Ignoring symbol!", (object) ((IInstance) subSymbol).InstancePath);
        AdsModule.Trace.TraceWarning(str, (Exception) ex);
      }
      return (ISymbol) subSymbol1;
    }

    /// <summary>Creates an unique path name</summary>
    /// <param name="instance">The instance.</param>
    /// <returns>System.String.</returns>
    internal string createUniquePathName(IInstance instance)
    {
      int num = 0;
      string instancePath = instance.InstancePath;
      string uniquePathName;
      DefaultInterpolatedStringHandler interpolatedStringHandler;
      for (uniquePathName = instancePath; ((IInstanceCollection<ISymbol>) this._provider.SymbolsInternal).Contains(uniquePathName); uniquePathName = interpolatedStringHandler.ToStringAndClear())
      {
        ++num;
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
        interpolatedStringHandler.AppendFormatted(instancePath);
        interpolatedStringHandler.AppendLiteral("_");
        interpolatedStringHandler.AppendFormatted<int>(num);
      }
      return uniquePathName;
    }

    /// <summary>
    /// Indicates that Virtual (created StructInstances) are used.
    /// </summary>
    /// <value>The use virtual instances.</value>
    internal bool UseVirtualInstances => this._useVirtualInstances;

    /// <summary>Registers the specified type.</summary>
    /// <param name="type">The type.</param>
    public void RegisterType(IDataType type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      ((IBindable) type).Bind((IBinder) this);
      ((NamespaceCollection) this._provider.NamespacesInternal).RegisterType(type);
    }

    /// <summary>Registers the specified types.</summary>
    /// <param name="types">The type.</param>
    public void RegisterTypes(IEnumerable<IDataType> types)
    {
      if (types == null)
        throw new ArgumentNullException(nameof (types));
      foreach (IDataType type in types)
        this.RegisterType(type);
    }

    /// <summary>Unregisters/Unbinds a type</summary>
    /// <param name="type">The type.</param>
    public void UnregisterType(IDataType type) => ((ICollection<IDataType>) ((INamespaceContainer<IDataType>) this._provider.NamespacesInternal)[type.Namespace].DataTypes).Remove(type);

    /// <summary>Unregisters all types.</summary>
    public void UnregisterAll() => ((ICollection<INamespace<IDataType>>) this._provider.NamespacesInternal).Clear();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool TryResolveType(string name, [NotNullWhen(true)] out IDataType? type)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      return ((INamespaceContainer<IDataType>) this._provider.NamespacesInternal).TryGetType(name, ref type) || SymbolParser.TryParseType(name, (IBinder) this, out type);
    }

    /// <summary>
    /// Asynchronously resolves the type name to the <see cref="T:TwinCAT.TypeSystem.IDataType" /> object.
    /// </summary>
    /// <param name="name">The name of the data type.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>An asynchronous task object that represents the 'ResultType' operation and returns a <see cref="T:TwinCAT.Ads.ResultValue`1" /> as result. The result <see cref="T:TwinCAT.TypeSystem.IDataType" /> can be accessed via
    /// <see cref="P:TwinCAT.Ads.ResultValue`1.Value" />.</returns>
    public virtual Task<ResultValue<IDataType>> ResolveTypeAsync(
      string name,
      CancellationToken cancel)
    {
      IDataType type = (IDataType) null;
      return Task.FromResult<ResultValue<IDataType>>(!this.TryResolveType(name, out type) ? new ResultValue<IDataType>((AdsErrorCode) 1, (IDataType) null) : new ResultValue<IDataType>((AdsErrorCode) 0, type));
    }

    /// <summary>
    /// Handler function called when a type / types are generated
    /// </summary>
    /// <param name="type">The type.</param>
    public void OnTypeGenerated(IDataType type)
    {
      if (this.TypesGenerated == null)
        return;
      DataTypeCollection<IDataType> dataTypeCollection = new DataTypeCollection<IDataType>();
      dataTypeCollection.Add(type);
      this.TypesGenerated((object) this, new DataTypeEventArgs((IEnumerable<IDataType>) dataTypeCollection.AsReadOnly()));
    }

    /// <summary>
    /// Handler function called when a type / types are generated
    /// </summary>
    /// <param name="types">The types.</param>
    public void OnTypesGenerated(IEnumerable<IDataType> types)
    {
      if (this.TypesGenerated == null)
        return;
      this.TypesGenerated((object) this, new DataTypeEventArgs(types));
    }

    /// <summary>Handler function when a type name cannot be resolved.</summary>
    /// <param name="typeName">Name of the type.</param>
    public void OnTypeResolveError(string typeName)
    {
      if (this.TypeResolveError == null)
        return;
      this.TypeResolveError((object) this, new DataTypeNameEventArgs(typeName));
    }

    /// <summary>Occurs when new types are generated</summary>
    public event EventHandler<DataTypeEventArgs>? TypesGenerated;

    /// <summary>Occurs when a typename cannot be resolved.</summary>
    public event EventHandler<DataTypeNameEventArgs>? TypeResolveError;
  }
}
