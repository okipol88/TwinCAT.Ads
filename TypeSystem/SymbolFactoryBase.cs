// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.SymbolFactoryBase
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Base implementation for <see cref="T:TwinCAT.TypeSystem.ISymbolFactory" /> interface.
  /// </summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.ISymbolFactory" />
  /// <exclude />
  public abstract class SymbolFactoryBase : ISymbolFactory
  {
    /// <summary>Symbol Factory services</summary>
    private ISymbolFactoryServices? services;
    /// <summary>
    /// Indicates that ArrayElements will be created on access and not cached.
    /// </summary>
    private bool nonCachedArrayElements;
    /// <summary>Indicates whether this instance is initialized.</summary>
    private bool initialized;
    private static char[] s_defaultInvalidChars = new char[1]
    {
      '^'
    };
    /// <summary>The invalid characters</summary>
    private char[] invalidCharacters = SymbolFactoryBase.DefaultInvalidChars;

    /// <summary>Symbol Factory services</summary>
    protected ISymbolFactoryServices Services => this.services;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.SymbolFactoryBase" /> class.
    /// </summary>
    /// <param name="nonCachedArrayElements">if set to <c>true</c> Array Elements will be memory optimized ind non-cached.</param>
    public SymbolFactoryBase(bool nonCachedArrayElements) => this.nonCachedArrayElements = nonCachedArrayElements;

    internal bool NonCachedArrayElements => this.nonCachedArrayElements;

    /// <summary>
    /// Gets a value indicating whether this instance is initialized.
    /// </summary>
    /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
    public bool IsInitialized => this.initialized;

    /// <summary>
    /// Initializes the the <see cref="T:TwinCAT.TypeSystem.ISymbolFactory" />.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <exception cref="T:System.ArgumentNullException">services</exception>
    public void Initialize(ISymbolFactoryServices services)
    {
      this.services = services != null ? services : throw new ArgumentNullException(nameof (services));
      this.initialized = true;
    }

    /// <summary>Gets the factory services.</summary>
    /// <value>The factory services.</value>
    public ISymbolFactoryServices? FactoryServices => this.services;

    public ISymbol CreateInstance(ISymbolInfo entry, ISymbol? parent)
    {
      ISymbol instance = entry != null ? this.OnCreateSymbol(entry, parent) : throw new ArgumentNullException(nameof (entry));
      ((IBindable) instance).Bind(this.FactoryServices.Binder);
      return instance;
    }

    public async Task<ResultValue<ISymbol>> CreateInstanceAsync(
      ISymbolInfo entry,
      ISymbol? parent,
      CancellationToken cancel)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      ResultValue<ISymbol> instanceAsync = await this.OnCreateSymbolAsync(entry, parent, cancel).ConfigureAwait(false);
      if (((ResultAds) instanceAsync).Succeeded)
        ((IBindable) instanceAsync.Value).Bind(this.FactoryServices.Binder);
      return instanceAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected bool TryResolveType(string typeName, [NotNullWhen(true)] out IDataType? type)
    {
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentOutOfRangeException(nameof (typeName));
      return ((IDataTypeResolver) this.services.Binder).TryResolveType(typeName, ref type);
    }

    /// <summary>Tries to resolve the specfied data type</summary>
    /// <param name="typeName">Name of the Type.</param>
    /// <param name="cancel">The cancel token.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">typeName</exception>
    /// <exception cref="T:System.ArgumentNullException">typeName</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected Task<ResultValue<IDataType>> ResolveTypeAsync(
      string typeName,
      CancellationToken cancel)
    {
      if (typeName == null)
        throw new ArgumentNullException(nameof (typeName));
      return ((IDataTypeResolver) this.services.Binder).ResolveTypeAsync(typeName, cancel);
    }

    protected ISymbol onCreateSymbol(ISymbolInfo entry, IDataType type, ISymbol? parent)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      return type.Category != 10 ? (type.Category != 5 ? (type.Category != 16 ? (type.Category != 4 ? (type.Category != 14 ? (type.Category != 15 ? (type.Category != 13 ? (type.Category != 2 ? this.OnCreatePrimitive(entry, type, parent) : (ISymbol) this.OnCreateAlias(entry, (IAliasType) type, parent)) : (ISymbol) this.OnCreatePointerInstance(entry, (IPointerType) type, parent)) : (ISymbol) this.OnCreateReferenceInstance(entry, (IReferenceType) type, parent)) : (ISymbol) this.OnCreateUnion(entry, (IUnionType) type, parent)) : (ISymbol) this.OnCreateArrayInstance(entry, (IArrayType) type, parent)) : (ISymbol) this.OnCreateInterface(entry, (IInterfaceType) type, parent)) : (ISymbol) this.OnCreateStruct(entry, (IStructType) type, parent)) : this.OnCreateString(entry, (IStringType) type, parent);
    }

    protected virtual async Task<ResultValue<ISymbol>> OnCreateSymbolAsync(
      ISymbolInfo entry,
      ISymbol? parent,
      CancellationToken cancel)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      ResultValue<IDataType> resultValue = await this.ResolveTypeAsync(entry.TypeName, cancel).ConfigureAwait(false);
      return new ResultValue<ISymbol>((AdsErrorCode) 0, !((ResultAds) resultValue).Succeeded ? this.OnCreatePrimitive(entry, (IDataType) null, parent) : this.onCreateSymbol(entry, resultValue.Value, parent));
    }

    protected virtual ISymbol OnCreateSymbol(ISymbolInfo entry, ISymbol? parent)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      IDataType type = (IDataType) null;
      return string.IsNullOrEmpty(entry.TypeName) ? this.OnCreateVirtualStruct(entry.Name, entry.Name, parent) : (!this.TryResolveType(entry.TypeName, out type) ? this.OnCreatePrimitive(entry, (IDataType) null, parent) : this.onCreateSymbol(entry, type, parent));
    }

    /// <summary>
    /// Creates all Element Instances of the specified array parent symbol.
    /// </summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <param name="arrayType">Type of the array.</param>
    /// <returns>SymbolCollection.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// parentInstance
    /// or
    /// arrayType
    /// </exception>
    public ISymbolCollection<ISymbol> CreateArrayElementInstances(
      ISymbol parentInstance,
      IArrayType arrayType)
    {
      if (parentInstance == null)
        throw new ArgumentNullException(nameof (parentInstance));
      return arrayType != null ? this.OnCreateArrayElementInstances(parentInstance, arrayType) : throw new ArgumentNullException(nameof (arrayType));
    }

    /// <summary>
    /// Creates all Element Instances of the specified array parent symbol.
    /// </summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <param name="arrayType">Type of the array.</param>
    /// <returns>SymbolCollection.</returns>
    protected virtual ISymbolCollection<ISymbol> OnCreateArrayElementInstances(
      ISymbol parentInstance,
      IArrayType arrayType)
    {
      ISymbolCollection<ISymbol> elementInstances;
      if (this.nonCachedArrayElements)
      {
        elementInstances = (ISymbolCollection<ISymbol>) new ArrayElementSymbolCollection(parentInstance, arrayType, (ISymbolFactory) this);
      }
      else
      {
        elementInstances = (ISymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 0);
        foreach (int[] indices in new ArrayIndexIterator(arrayType))
        {
          ISymbol arrayElement = this.CreateArrayElement(arrayType, indices, parentInstance);
          ((ICollection<ISymbol>) elementInstances).Add(arrayElement);
        }
      }
      return elementInstances;
    }

    protected abstract IArrayInstance OnCreateArrayInstance(
      ISymbolInfo entry,
      IArrayType type,
      ISymbol? parent);

    protected abstract IStructInstance OnCreateStruct(
      ISymbolInfo entry,
      IStructType structType,
      ISymbol? parent);

    protected abstract IInterfaceInstance OnCreateInterface(
      ISymbolInfo entry,
      IInterfaceType interfaceType,
      ISymbol? parent);

    protected abstract IUnionInstance OnCreateUnion(
      ISymbolInfo entry,
      IUnionType unionType,
      ISymbol? parent);

    protected abstract IAliasInstance OnCreateAlias(
      ISymbolInfo entry,
      IAliasType aliasType,
      ISymbol? parent);

    protected abstract ISymbol OnCreateString(
      ISymbolInfo entry,
      IStringType stringType,
      ISymbol? parent);

    protected abstract ISymbol OnCreatePrimitive(
      ISymbolInfo entry,
      IDataType? dataType,
      ISymbol? parent);

    protected abstract IReferenceInstance OnCreateReferenceInstance(
      ISymbolInfo entry,
      IReferenceType referenceType,
      ISymbol? parent);

    protected abstract IPointerInstance OnCreatePointerInstance(
      ISymbolInfo entry,
      IPointerType structType,
      ISymbol? parent);

    public ISymbol CreateArrayElement(IArrayType arrayType, int[] indices, ISymbol? parent)
    {
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (arrayType == null)
        throw new ArgumentNullException(nameof (arrayType));
      if (indices.Length == 0)
        throw new ArgumentOutOfRangeException(nameof (indices));
      return this.OnCreateArrayElement(arrayType, indices, parent);
    }

    /// <summary>Handler function creating a new Array Element Symbol.</summary>
    /// <param name="arrayType">Resolved array type.</param>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>ISymbol.</returns>
    protected abstract ISymbol OnCreateArrayElement(
      IArrayType arrayType,
      int[] currentIndex,
      ISymbol parent);

    /// <summary>
    /// Creates the Member Instances collection for the specified parent instance
    /// </summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <param name="structType">Type of the structure.</param>
    /// <returns>SymbolCollection.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// parentInstance
    /// or
    /// structType
    /// </exception>
    public ISymbolCollection<ISymbol> CreateFieldInstances(
      ISymbol parentInstance,
      IDataType structType)
    {
      if (parentInstance == null)
        throw new ArgumentNullException(nameof (parentInstance));
      return structType != null ? this.OnCreateFieldInstances(parentInstance, structType) : throw new ArgumentNullException(nameof (structType));
    }

    /// <summary>Handler function creating the member instances.</summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <param name="parentType">Type of parent (Struct or Union).</param>
    /// <returns>SymbolCollection.</returns>
    protected virtual ISymbolCollection<ISymbol> OnCreateFieldInstances(
      ISymbol parentInstance,
      IDataType parentType)
    {
      if (parentInstance == null)
        throw new ArgumentNullException(nameof (parentInstance));
      if (parentType == null)
        throw new ArgumentNullException(nameof (parentType));
      SymbolCollection fieldInstances = new SymbolCollection((InstanceCollectionMode) 0);
      IEnumerable<IField> ifields = (IEnumerable<IField>) null;
      if (parentType.Category == 5)
        ifields = (IEnumerable<IField>) ((IInterfaceType) parentType).Members;
      else if (parentType.Category == 16)
        ifields = (IEnumerable<IField>) ((IInterfaceType) parentType).Members;
      else if (parentType.Category == 14)
        ifields = (IEnumerable<IField>) ((IUnionType) parentType).Fields;
      if (ifields != null)
      {
        foreach (IField field in ifields)
        {
          ISymbol fieldInstance = this.CreateFieldInstance(field, parentInstance);
          if (fieldInstance != null)
            fieldInstances.Add(fieldInstance);
          else
            AdsModule.Trace.TraceWarning("Couldn't create field '{0}' for instance '{1}'!", new object[2]
            {
              (object) ((IInstance) field).InstanceName,
              (object) ((IInstance) parentInstance).InstancePath
            });
        }
      }
      return (ISymbolCollection<ISymbol>) fieldInstances;
    }

    /// <summary>Creates the Instance member.</summary>
    /// <param name="field">Field</param>
    /// <param name="parent">Parent Struct</param>
    /// <returns>Instance member</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// member
    /// or
    /// parent
    /// </exception>
    public ISymbol CreateFieldInstance(IField field, ISymbol parent)
    {
      if (field == null)
        throw new ArgumentNullException(nameof (field));
      return parent != null ? this.OnCreateFieldInstance(field, parent) : throw new ArgumentNullException(nameof (parent));
    }

    /// <summary>
    /// Handler function creating a new <see cref="T:TwinCAT.TypeSystem.IStructInstance" /> member
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>ISymbol.</returns>
    protected abstract ISymbol OnCreateFieldInstance(IField member, ISymbol parent);

    /// <summary>Creates the reference/pointer instance.</summary>
    /// <param name="type">Reference/Pointer type.</param>
    /// <param name="parent">Parent Instance of the reference</param>
    /// <returns>Reference/Pointer instance.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// type
    /// or
    /// parent
    /// </exception>
    public ISymbol? CreateReferenceInstance(IPointerType type, ISymbol parent)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      return parent != null ? this.OnCreateReference(type, parent) : throw new ArgumentNullException(nameof (parent));
    }

    /// <summary>Handler function creating a new Reference Instance.</summary>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>ISymbol.</returns>
    protected abstract ISymbol? OnCreateReference(IPointerType type, ISymbol parent);

    public ISymbol CreateVirtualStruct(
      string instanceName,
      string instancePath,
      ISymbol? parent)
    {
      if (string.IsNullOrEmpty(instanceName))
        throw new ArgumentOutOfRangeException(nameof (instanceName));
      if (string.IsNullOrEmpty(instancePath))
        throw new ArgumentOutOfRangeException(nameof (instancePath));
      return this.OnCreateVirtualStruct(instanceName, instancePath, parent);
    }

    protected abstract ISymbol OnCreateVirtualStruct(
      string instanceName,
      string instancePath,
      ISymbol? parent);

    /// <summary>Combines member parent path</summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent.</param>
    /// <returns>System.String.</returns>
    protected string CombinePath(IField member, ISymbol parent)
    {
      if (member == null)
        throw new ArgumentNullException(nameof (member));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      return string.Join(".", new string[2]
      {
        ((IInstance) parent).InstancePath,
        ((IInstance) member).InstanceName
      });
    }

    /// <summary>Default Invalid Characters</summary>
    public static char[] DefaultInvalidChars => SymbolFactoryBase.s_defaultInvalidChars;

    /// <summary>
    /// Gets the invalid characters that are not allowed to appear within the Instance Name
    /// </summary>
    /// <value>The forbidden characters.</value>
    public char[] InvalidCharacters => this.invalidCharacters;

    /// <summary>
    /// Gets a value indicating whether this instance has invalid characters.
    /// </summary>
    /// <value><c>true</c> if this instance has invalid characters; otherwise, <c>false</c>.</value>
    public bool HasInvalidCharacters => this.invalidCharacters.Length != 0;

    /// <summary>Sets the invalid characters.</summary>
    /// <param name="invalidChars">The invalid chars.</param>
    public void SetInvalidCharacters(char[] invalidChars) => this.invalidCharacters = invalidChars;
  }
}
