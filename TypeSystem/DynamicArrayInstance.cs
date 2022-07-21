// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicArrayInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using TwinCAT.Ads.TypeSystem;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Dynamic Array Instance</summary>
  public class DynamicArrayInstance : 
    DynamicSymbol,
    IArrayInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IIndexedAccess
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicArrayInstance" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    internal DynamicArrayInstance(IArrayInstance symbol)
      : base((IValueSymbol) symbol)
    {
    }

    /// <summary>
    /// Gets the contained Array Elements as read only collection.
    /// </summary>
    /// <value>The elements.</value>
    public ISymbolCollection<ISymbol> Elements => ((IArrayInstance) this._InnerSymbol).Elements;

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.ISymbol" /> with the specified indices.
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <returns>ISymbol.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">indices</exception>
    public ISymbol this[int[] indices]
    {
      get
      {
        ISymbol symbol = (ISymbol) null;
        if (!this.TryGetElement(indices, out symbol))
          throw new ArgumentOutOfRangeException(nameof (indices));
        return symbol;
      }
    }

    public bool TryGetElement(int[] indices, [NotNullWhen(true)] out ISymbol? symbol)
    {
      IArrayType dataType = (IArrayType) this.DataType;
      if (dataType != null && ArrayType.AreIndicesValid(indices, dataType, false))
      {
        int elementPosition = ArrayType.GetElementPosition(indices, dataType);
        symbol = ((IList<ISymbol>) this.SubSymbols)[elementPosition];
        return true;
      }
      symbol = (ISymbol) null;
      return false;
    }

    public bool TryGetElement(IList<int[]> jaggedIndices, [NotNullWhen(true)] out ISymbol? symbol) => ((IIndexedAccess) this._InnerSymbol).TryGetElement(jaggedIndices, ref symbol);

    /// <summary>Gets the dimensions as read only collection.</summary>
    /// <value>The dimensions.</value>
    public IDimensionCollection Dimensions => ((IArrayInstance) this._InnerSymbol).Dimensions;

    /// <summary>Gets the type of the contained elements.</summary>
    /// <value>The type of the element.</value>
    public IDataType? ElementType => ((IArrayInstance) this._InnerSymbol).ElementType;

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
      IArrayType dataType = (IArrayType) this.DataType;
      return dataType != null ? DynamicArrayInstance.TryGetIndex((DynamicSymbol) this, dataType, binder, indexes, out result) : base.TryGetIndex(binder, indexes, out result);
    }

    /// <summary>
    /// Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for indexing operations.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="arrayType">Type of the array.</param>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class, <paramref name="indexes" />[0] is equal to 3.</param>
    /// <param name="result">The result of the index operation.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    internal static bool TryGetIndex(
      DynamicSymbol symbol,
      IArrayType arrayType,
      GetIndexBinder binder,
      object[] indexes,
      out object result)
    {
      int[] indices = new int[indexes.GetLength(0)];
      for (int index = 0; index < indexes.GetLength(0); ++index)
        indices[index] = (int) indexes[index];
      ArrayType.CheckIndices(indices, arrayType, false);
      result = (object) ((IList<ISymbol>) symbol.SubSymbols)[ArrayType.GetElementPosition(indices, arrayType)];
      return true;
    }

    /// <summary>
    /// Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations that access objects by a specified index.
    /// </summary>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="indexes[][]" /> is equal to 3.</param>
    /// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="value" /> is equal to 10.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.</returns>
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) => base.TrySetIndex(binder, indexes, value);
  }
}
