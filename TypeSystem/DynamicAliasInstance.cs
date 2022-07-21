// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicAliasInstance
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
  /// <summary>
  /// Class DynamicAliasInstance. This class cannot be inherited.
  /// </summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.DynamicSymbol" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IAliasInstance" />
  public sealed class DynamicAliasInstance : 
    DynamicSymbol,
    IAliasInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IIndexedAccess
  {
    /// <summary>The resolved alias type</summary>
    private IDataType? resolvedAlias;
    /// <summary>Dictionary of normalized Instance Names</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    private WeakReference<Dictionary<string, ISymbol>>? _weakNormalizedNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicAliasInstance" /> class.
    /// </summary>
    /// <param name="aliasInstance">The alias instance.</param>
    internal DynamicAliasInstance(IAliasInstance aliasInstance)
      : base((IValueSymbol) aliasInstance)
    {
      IAliasType dataType = (IAliasType) ((IInstance) aliasInstance).DataType;
      if (dataType is IResolvableType iresolvableType)
        this.resolvedAlias = iresolvableType.ResolveType((DataTypeResolveStrategy) 1);
      else
        this.resolvedAlias = (IDataType) dataType;
    }

    /// <summary>Gets the Normalized names table.</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    private Dictionary<string, ISymbol> GetNormalizedNames()
    {
      Dictionary<string, ISymbol> target = (Dictionary<string, ISymbol>) null;
      if (this._weakNormalizedNames == null || !this._weakNormalizedNames.TryGetTarget(out target))
      {
        target = this.OnCreateNormalizedNames();
        if (this._weakNormalizedNames == null)
          this._weakNormalizedNames = new WeakReference<Dictionary<string, ISymbol>>(target);
        else
          this._weakNormalizedNames.SetTarget(target);
      }
      return target;
    }

    /// <summary>Creates the normalized names table</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    private Dictionary<string, ISymbol> OnCreateNormalizedNames()
    {
      Dictionary<string, ISymbol> normalizedNames = new Dictionary<string, ISymbol>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.resolvedAlias != null && this.resolvedAlias.Category == 5)
      {
        foreach (DynamicSymbol subSymbol in (IEnumerable<ISymbol>) this.SubSymbols)
          normalizedNames.Add(subSymbol.NormalizedName, (ISymbol) subSymbol);
      }
      return normalizedNames;
    }

    /// <summary>Returns the enumeration of all dynamic member names.</summary>
    /// <returns>A sequence that contains dynamic member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames() => this.resolvedAlias != null && this.resolvedAlias.Category == 5 ? (IEnumerable<string>) this.GetNormalizedNames().Keys : base.GetDynamicMemberNames();

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    public override bool TryGetMember(GetMemberBinder binder, [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      if (this.resolvedAlias != null && this.resolvedAlias.Category == 5)
      {
        ISymbol isymbol = (ISymbol) null;
        if (this.GetNormalizedNames().TryGetValue(binder.Name, out isymbol))
        {
          result = (object) isymbol;
          return true;
        }
      }
      return base.TryGetMember(binder, out result);
    }

    /// <summary>
    /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
    public override bool TrySetMember(SetMemberBinder binder, object? value) => base.TrySetMember(binder, value);

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
      return this.resolvedAlias != null && this.resolvedAlias.Category == 4 ? DynamicArrayInstance.TryGetIndex((DynamicSymbol) this, (IArrayType) this.resolvedAlias, binder, indexes, out result) : base.TryGetIndex(binder, indexes, out result);
    }

    /// <summary>
    /// Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations that access objects by a specified index.
    /// </summary>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="indexes[][]" /> is equal to 3.</param>
    /// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="value" /> is equal to 10.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.</returns>
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) => base.TrySetIndex(binder, indexes, value);

    bool IIndexedAccess.TryGetElement(int[] indices, out ISymbol? symbol) => ((IIndexedAccess) this._InnerSymbol).TryGetElement(indices, ref symbol);

    bool IIndexedAccess.TryGetElement(IList<int[]> jaggedIndices, out ISymbol? symbol) => ((IIndexedAccess) this._InnerSymbol).TryGetElement(jaggedIndices, ref symbol);
  }
}
