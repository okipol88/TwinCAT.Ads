// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicSymbolFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Instance factory for dynamic symbols (for internal use only)
  /// </summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DynamicSymbolFactory : ISymbolFactory, ISymbolFactoryOversampled
  {
    private bool nonCachedArrayElements;
    /// <summary>Default Invalid Characters for dynamic symbols</summary>
    /// <remarks>By default, the following characters are Invalid and are replaced by '_':
    /// '^', ' ' (Space), '(', ')','-','.'
    /// </remarks>
    /// <seealso cref="P:TwinCAT.TypeSystem.ISymbolFactory.InvalidCharacters" />
    public static readonly char[] DefaultInvalidChars = new char[5]
    {
      '^',
      ' ',
      '(',
      ')',
      '-'
    };
    /// <summary>Inner static/aggregated Factory</summary>
    private ISymbolFactory inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicSymbolFactory" /> class.
    /// </summary>
    /// <param name="inner">The inner (static) instance factory.</param>
    /// <param name="nonCachedArrayElements">if set to <c>true</c> Array elements will not be cached.</param>
    /// <exception cref="T:System.ArgumentNullException">inner</exception>
    public DynamicSymbolFactory(ISymbolFactory inner, bool nonCachedArrayElements)
    {
      this.inner = inner != null ? inner : throw new ArgumentNullException(nameof (inner));
      this.nonCachedArrayElements = nonCachedArrayElements;
      this.inner.SetInvalidCharacters(DynamicSymbolFactory.DefaultInvalidChars);
    }

    /// <summary>
    /// Initializes the the <see cref="T:TwinCAT.TypeSystem.ISymbolFactory" />.
    /// </summary>
    /// <param name="services">The services.</param>
    public void Initialize(ISymbolFactoryServices services) => this.inner.Initialize(services);

    public ISymbol CreateInstance(ISymbolInfo entry, ISymbol? parent) => (ISymbol) this.WrapSymbol((IValueSymbol) this.inner.CreateInstance(entry, parent));

    public async Task<ResultValue<ISymbol>> CreateInstanceAsync(
      ISymbolInfo entry,
      ISymbol? parent,
      CancellationToken cancel)
    {
      IValueSymbol wrapped = (IValueSymbol) null;
      ResultValue<ISymbol> resultValue = await this.inner.CreateInstanceAsync(entry, parent, cancel).ConfigureAwait(false);
      if (((ResultAds) resultValue).Succeeded)
        wrapped = this.WrapSymbol((IValueSymbol) resultValue.Value);
      ResultValue<ISymbol> instanceAsync = new ResultValue<ISymbol>(((ResultAds) resultValue).ErrorCode, (ISymbol) wrapped);
      wrapped = (IValueSymbol) null;
      return instanceAsync;
    }

    /// <summary>Wraps the specified Symbol into a dynamic wrapper</summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>IValueSymbol.</returns>
    public IValueSymbol WrapSymbol(IValueSymbol symbol) => symbol != null ? this.create(symbol) : throw new ArgumentNullException(nameof (symbol));

    /// <summary>
    /// Creates all Element Instances of the specified array parent symbol.
    /// </summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <param name="arrayType">Type of the array.</param>
    /// <returns>SymbolCollection.</returns>
    public ISymbolCollection<ISymbol> CreateArrayElementInstances(
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
        foreach (int[] currentIndex in new ArrayIndexIterator(arrayType))
        {
          ISymbol arrayElement = this.CreateArrayElement(arrayType, currentIndex, parentInstance);
          ((ICollection<ISymbol>) elementInstances).Add(arrayElement);
        }
        if (parentInstance is IOversamplingArrayInstance)
        {
          ISymbol oversamplingElement = this.CreateOversamplingElement(parentInstance);
          ((ICollection<ISymbol>) elementInstances).Add(oversamplingElement);
        }
      }
      return elementInstances;
    }

    /// <summary>Creates the array element</summary>
    /// <param name="arrayType">Resolved Array type.</param>
    /// <param name="currentIndex">Array Index of the Element</param>
    /// <param name="parent">Array Instance</param>
    /// <returns>Resolved Array Type</returns>
    /// <exception cref="T:System.ArgumentNullException">parent</exception>
    public ISymbol CreateArrayElement(
      IArrayType arrayType,
      int[] currentIndex,
      ISymbol parent)
    {
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      return (ISymbol) this.create((IValueSymbol) this.inner.CreateArrayElement(arrayType, currentIndex, parent));
    }

    /// <summary>Creates the oversampling array Element.</summary>
    /// <param name="parent">Array Instance.</param>
    /// <returns>ISymbol.</returns>
    public ISymbol CreateOversamplingElement(ISymbol parent) => parent != null ? (ISymbol) this.create((IValueSymbol) ((ISymbolFactoryOversampled) this.inner).CreateOversamplingElement(parent)) : throw new ArgumentNullException(nameof (parent));

    /// <summary>Creates the member instances.</summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <param name="parentType">Type of the structure.</param>
    /// <returns>SymbolCollection.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// parentInstance
    /// or
    /// structType
    /// </exception>
    public ISymbolCollection<ISymbol> CreateFieldInstances(
      ISymbol parentInstance,
      IDataType parentType)
    {
      if (parentInstance == null)
        throw new ArgumentNullException(nameof (parentInstance));
      return parentType != null ? this.OnCreateFieldInstances(parentInstance, parentType) : throw new ArgumentNullException(nameof (parentType));
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

    /// <summary>Creates a single Instance member on a struct parent</summary>
    /// <param name="field">Field</param>
    /// <param name="parent">Parent Struct/Alias/Union</param>
    /// <returns>Instance member</returns>
    /// <remarks>Because the Alias type can act like a struct, the parent can be an IAliasInstance also.</remarks>
    public ISymbol CreateFieldInstance(IField field, ISymbol parent) => (ISymbol) this.create((IValueSymbol) this.inner.CreateFieldInstance(field, parent));

    /// <summary>Creates the reference/pointer instance.</summary>
    /// <param name="type">Reference/Pointer type.</param>
    /// <param name="parent">Parent Instance of the reference</param>
    /// <returns>Reference/Pointer instance.</returns>
    public ISymbol? CreateReferenceInstance(IPointerType type, ISymbol parent)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      IValueSymbol referenceInstance1 = (IValueSymbol) this.inner.CreateReferenceInstance(type, (ISymbol) ((DynamicSymbol) parent)._InnerSymbol);
      ISymbol referenceInstance2 = (ISymbol) null;
      if (referenceInstance1 != null)
        referenceInstance2 = (ISymbol) this.create(referenceInstance1);
      return referenceInstance2;
    }

    /// <summary>
    /// Creates a dynamic wrapper for the specified (static) symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>ISymbol.</returns>
    private IValueSymbol create(IValueSymbol symbol)
    {
      DataTypeCategory dataTypeCategory = symbol != null ? ((ISymbol) symbol).Category : throw new ArgumentNullException(nameof (symbol));
      switch (dataTypeCategory - 2)
      {
        case 0:
          if (symbol is IAliasInstance aliasInstance)
            return (IValueSymbol) new DynamicAliasInstance(aliasInstance);
          AdsModule.Trace.TraceWarning(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' cannot be resolved to alias", (object) ((IInstance) symbol).InstancePath));
          return (IValueSymbol) new DynamicSymbol(symbol);
        case 1:
label_29:
          return (IValueSymbol) new DynamicSymbol(symbol);
        case 2:
          if (symbol is IArrayInstance)
            return symbol is IOversamplingArrayInstance ? (IValueSymbol) new DynamicOversamplingArrayInstance((IOversamplingArrayInstance) symbol) : (IValueSymbol) new DynamicArrayInstance((IArrayInstance) symbol);
          AdsModule.Trace.TraceWarning(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' cannot be resolved to array", (object) ((IInstance) symbol).InstancePath));
          return (IValueSymbol) new DynamicSymbol(symbol);
        case 3:
          if (symbol is IStructInstance)
            return (IValueSymbol) new DynamicStructInstance((IStructInstance) symbol);
          AdsModule.Trace.TraceWarning(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' cannot be resolved to struct", (object) ((IInstance) symbol).InstancePath));
          return (IValueSymbol) new DynamicSymbol(symbol);
        default:
          switch (dataTypeCategory - 13)
          {
            case 0:
              if (symbol is IPointerInstance pointerInstance)
                return (IValueSymbol) new DynamicPointerInstance(pointerInstance);
              AdsModule.Trace.TraceWarning(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' cannot be resolved to pointer", (object) ((IInstance) symbol).InstancePath));
              return (IValueSymbol) new DynamicSymbol(symbol);
            case 1:
              if (symbol is IUnionInstance unionInstance)
                return (IValueSymbol) new DynamicUnionInstance(unionInstance);
              AdsModule.Trace.TraceWarning(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' cannot be resolved to union", (object) ((IInstance) symbol).InstancePath));
              return (IValueSymbol) new DynamicSymbol(symbol);
            case 2:
              if (symbol is IReferenceInstance refInstance)
                return refInstance.ReferencedType != null && refInstance.ReferencedType.Category == 5 ? (IValueSymbol) new DynamicStructReferenceInstance(refInstance) : (IValueSymbol) new DynamicReferenceInstance(refInstance);
              AdsModule.Trace.TraceWarning(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' cannot be resolved to reference", (object) ((IInstance) symbol).InstancePath));
              return (IValueSymbol) new DynamicSymbol(symbol);
            case 3:
              if (symbol is IInterfaceInstance structInstance)
                return (IValueSymbol) new DynamicInterfaceInstance(structInstance);
              AdsModule.Trace.TraceWarning(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' cannot be resolved to struct", (object) ((IInstance) symbol).InstancePath));
              return (IValueSymbol) new DynamicSymbol(symbol);
            default:
              goto label_29;
          }
      }
    }

    public ISymbol CreateVirtualStruct(
      string instanceName,
      string instancePath,
      ISymbol? parent)
    {
      return (ISymbol) new DynamicVirtualStructInstance((IVirtualStructInstance) this.inner.CreateVirtualStruct(instanceName, instancePath, parent));
    }

    /// <summary>
    /// Gets the invalid characters that are not allowed to appear within the Instance Name
    /// </summary>
    /// <value>The forbidden characters.</value>
    /// <seealso cref="M:TwinCAT.TypeSystem.DynamicSymbolFactory.SetInvalidCharacters(System.Char[])" />
    /// <seealso cref="P:TwinCAT.TypeSystem.DynamicSymbolFactory.HasInvalidCharacters" />
    public char[] InvalidCharacters => this.inner.InvalidCharacters;

    /// <summary>Sets the invalid characters.</summary>
    /// <param name="invalidChars">The invalid chars.</param>
    /// <seealso cref="P:TwinCAT.TypeSystem.DynamicSymbolFactory.InvalidCharacters" />
    public void SetInvalidCharacters(char[] invalidChars) => this.inner.SetInvalidCharacters(invalidChars);

    /// <summary>
    /// Gets a value indicating whether this instance has invalid characters.
    /// </summary>
    /// <value><c>true</c> if this instance has invalid characters; otherwise, <c>false</c>.</value>
    /// <seealso cref="P:TwinCAT.TypeSystem.DynamicSymbolFactory.InvalidCharacters" />
    public bool HasInvalidCharacters => this.inner.HasInvalidCharacters;

    /// <summary>Gets the factory services.</summary>
    /// <value>The factory services.</value>
    public ISymbolFactoryServices? FactoryServices => this.inner.FactoryServices;
  }
}
