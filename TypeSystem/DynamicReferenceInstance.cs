// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicReferenceInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Dynamic Reference Instance</summary>
  public class DynamicReferenceInstance : 
    DynamicSymbol,
    IReferenceInstanceAccess,
    IReferenceInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IIndexedAccess
  {
    /// <summary>The resolved alias type</summary>
    protected IDataType? resolvedReferenceType;
    /// <summary>Dictionary of normalized Instance Names</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    private WeakReference<IDictionary<string, ISymbol>>? _weakNormalizedNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicReferenceInstance" /> class.
    /// </summary>
    /// <param name="refInstance">The ref instance.</param>
    internal DynamicReferenceInstance(IReferenceInstance refInstance)
      : base((IValueSymbol) refInstance)
    {
      IReferenceType dataType = (IReferenceType) ((IInstance) refInstance).DataType;
      IResolvableType iresolvableType = (IResolvableType) dataType;
      if (iresolvableType != null)
        this.resolvedReferenceType = iresolvableType.ResolveType((DataTypeResolveStrategy) 1);
      else
        this.resolvedReferenceType = (IDataType) dataType;
    }

    /// <summary>Gets the Normalized names table.</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected IDictionary<string, ISymbol> GetNormalizedNames()
    {
      IDictionary<string, ISymbol> target = (IDictionary<string, ISymbol>) null;
      if (this._weakNormalizedNames == null || !this._weakNormalizedNames.TryGetTarget(out target))
      {
        target = this.OnCreateNormalizedNames();
        if (this._weakNormalizedNames == null)
          this._weakNormalizedNames = new WeakReference<IDictionary<string, ISymbol>>(target);
        else
          this._weakNormalizedNames.SetTarget(target);
      }
      return target;
    }

    /// <summary>Creates the normalized names table</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual IDictionary<string, ISymbol> OnCreateNormalizedNames() => (IDictionary<string, ISymbol>) new Dictionary<string, ISymbol>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets the Category of the Referenced Symbol.</summary>
    /// <value>The resolved category.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public DataTypeCategory ResolvedCategory => this.resolvedReferenceType != null ? this.resolvedReferenceType.Category : (DataTypeCategory) 0;

    /// <summary>
    /// Gets the resolved byte size of the <see cref="T:TwinCAT.TypeSystem.IReferenceInstance" />.
    /// </summary>
    /// <value>The resolved byte size.</value>
    public int ResolvedByteSize => this.resolvedReferenceType != null ? ((IBitSize) this.resolvedReferenceType).ByteSize : 0;

    /// <summary>
    /// Gets the resolved type of the <see cref="T:TwinCAT.TypeSystem.IReferenceInstance" />.
    /// </summary>
    /// <value>The resolved type.</value>
    public IDataType? ResolvedType => this.resolvedReferenceType;

    /// <summary>
    /// Gets the referenced type of the <see cref="T:TwinCAT.TypeSystem.IReferenceInstance" />
    /// </summary>
    /// <value>The referenced type</value>
    public IDataType? ReferencedType => ((IReferenceInstance) this._InnerSymbol).ReferencedType;

    /// <summary>
    /// Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for indexing operations.
    /// </summary>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class, <paramref name="indexes" />[0] is equal to 3.</param>
    /// <param name="result">The result of the index operation.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      if (indexes == null)
        throw new ArgumentNullException(nameof (indexes));
      return this.resolvedReferenceType != null && this.resolvedReferenceType.Category == 4 ? DynamicArrayInstance.TryGetIndex((DynamicSymbol) this, (IArrayType) this.resolvedReferenceType, binder, indexes, out result) : base.TryGetIndex(binder, indexes, out result);
    }

    /// <summary>
    /// Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations that access objects by a specified index.
    /// </summary>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="indexes[][]" /> is equal to 3.</param>
    /// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="value" /> is equal to 10.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.</returns>
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) => base.TrySetIndex(binder, indexes, value);

    bool IIndexedAccess.TryGetElement(int[] indices, [NotNullWhen(true)] out ISymbol? symbol) => ((IIndexedAccess) this._InnerSymbol).TryGetElement(indices, ref symbol);

    bool IIndexedAccess.TryGetElement(IList<int[]> jaggedIndices, [NotNullWhen(true)] out ISymbol? symbol) => ((IIndexedAccess) this._InnerSymbol).TryGetElement(jaggedIndices, ref symbol);
  }
}
