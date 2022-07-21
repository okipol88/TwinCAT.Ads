// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ArrayElementValueIterator
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Iterator for Array Element Values.</summary>
  /// <exclude />
  public class ArrayElementValueIterator : IEnumerable<object>, IEnumerable
  {
    /// <summary>The Array Value</summary>
    private IArrayValue _arrayValue;
    /// <summary>The Array Instance</summary>
    private IArrayInstance _array;
    /// <summary>The Array Type</summary>
    private IArrayType _type;
    /// <summary>Index Iterator</summary>
    private ArrayIndexIterator _indexIter;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ArrayElementValueIterator" /> class.
    /// </summary>
    /// <param name="arrayValue">The array value.</param>
    public ArrayElementValueIterator(IArrayValue arrayValue)
    {
      this._arrayValue = arrayValue != null ? arrayValue : throw new ArgumentNullException(nameof (arrayValue));
      this._array = (IArrayInstance) ((IValue) arrayValue).Symbol;
      this._type = ((IValue) arrayValue).DataType != null ? (IArrayType) ((IValue) arrayValue).DataType : throw new CannotResolveDataTypeException(((IInstance) ((IValue) arrayValue).Symbol).TypeName);
      this._indexIter = new ArrayIndexIterator(this._type);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<object> GetEnumerator()
    {
      foreach (int[] numArray in this._indexIter)
      {
        object obj = (object) null;
        if (this._arrayValue.TryGetIndexValue(numArray, ref obj))
          yield return obj;
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
