// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ArrayIndexIterator
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
  /// <summary>Iterator for iterating Indices of an Array Type.</summary>
  /// <exclude />
  public class ArrayIndexIterator : IEnumerable<int[]>, IEnumerable
  {
    /// <summary>Lower bounds</summary>
    private int[] _lowerBounds;
    /// <summary>Upper bounds</summary>
    private int[] _upperBounds;
    /// <summary>Aligns the returned indices to 0-basing arrays</summary>
    private bool _zeroShift;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ArrayIndexIterator" /> class.
    /// </summary>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    internal ArrayIndexIterator(int[] lowerBounds, int[] upperBounds)
      : this(lowerBounds, upperBounds, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ArrayIndexIterator" /> class.
    /// </summary>
    /// <param name="array">The array.</param>
    internal ArrayIndexIterator(Array array)
    {
      int rank = array.Rank;
      this._lowerBounds = new int[rank];
      this._upperBounds = new int[rank];
      for (int dimension = 0; dimension < rank; ++dimension)
      {
        this._lowerBounds[dimension] = array.GetLowerBound(dimension);
        this._upperBounds[dimension] = array.GetUpperBound(dimension);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ArrayIndexIterator" /> class.
    /// </summary>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="zeroShift">if set to <c>true</c> [zero shift].</param>
    internal ArrayIndexIterator(int[] lowerBounds, int[] upperBounds, bool zeroShift)
    {
      this._lowerBounds = lowerBounds;
      this._upperBounds = upperBounds;
      this._zeroShift = zeroShift;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ArrayIndexIterator" /> class.
    /// </summary>
    /// <param name="arrayType">Type of the array.</param>
    /// <param name="zeroShift">if set to <c>true</c> [zero shift].</param>
    internal ArrayIndexIterator(IArrayType arrayType, bool zeroShift)
    {
      if (arrayType == null)
        throw new ArgumentNullException(nameof (arrayType));
      this._lowerBounds = arrayType.Dimensions.LowerBounds;
      this._upperBounds = arrayType.Dimensions.UpperBounds;
      this._zeroShift = zeroShift;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ArrayIndexIterator" /> class.
    /// </summary>
    /// <param name="arrayType">Type of the array.</param>
    public ArrayIndexIterator(IArrayType arrayType)
      : this(arrayType, false)
    {
    }

    /// <summary>Gets the index factors.</summary>
    /// <returns>System.Int32[].</returns>
    internal int[] getIndexFactors()
    {
      int length = this._lowerBounds.Length;
      int[] indexFactors = new int[length];
      for (int index = length - 1; index >= 0; --index)
      {
        if (index == length - 1)
        {
          indexFactors[index] = 1;
        }
        else
        {
          int num = this._upperBounds[index + 1] - this._lowerBounds[index + 1] + 1;
          indexFactors[index] = indexFactors[index + 1] * num;
        }
      }
      return indexFactors;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
    public IEnumerator<int[]> GetEnumerator()
    {
      if (this._lowerBounds.Length != 0)
      {
        int highestRank = this._lowerBounds.Length - 1;
        int[] actual = new int[this._lowerBounds.Length];
        Array.Copy((Array) this._lowerBounds, (Array) actual, this._lowerBounds.Length);
        int currentRank = highestRank;
        while (this._upperBounds[currentRank] - this._lowerBounds[currentRank] + 1 > 0)
        {
          int[] numArray = new int[actual.Length];
          for (int index = 0; index < actual.Length; ++index)
            numArray[index] = !this._zeroShift ? actual[index] : actual[index] - this._lowerBounds[index];
          yield return numArray;
          bool flag1 = actual[currentRank] == this._upperBounds[currentRank];
          bool flag2 = false;
          while (flag1 && currentRank >= 0)
          {
            --currentRank;
            if (currentRank >= 0)
            {
              flag1 = actual[currentRank] == this._upperBounds[currentRank];
              flag2 = true;
            }
          }
          if (currentRank < 0)
            break;
          ++actual[currentRank];
          if (flag2)
          {
            for (int index = currentRank + 1; index <= highestRank; ++index)
              actual[index] = this._lowerBounds[index];
            currentRank = highestRank;
          }
        }
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
