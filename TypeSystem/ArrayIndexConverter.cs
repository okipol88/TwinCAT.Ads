// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ArrayIndexConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Converts ArrayIndices to string and position/subindex and vice versa (for internal use only)
  /// </summary>
  /// <exclude />
  public static class ArrayIndexConverter
  {
    /// <summary>Convert indices to string.</summary>
    /// <param name="indices">The indices.</param>
    /// <returns>The string representation (including brackets)</returns>
    public static string IndicesToString(int[] indices)
    {
      string empty = string.Empty;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) string.Join<int>(",", (IEnumerable<int>) indices));
    }

    /// <summary>Parses the Indices string to int array (as indices).</summary>
    /// <param name="indices">The indices.</param>
    /// <returns>System.Int32[].</returns>
    public static IList<int[]> StringToIndices(string indices)
    {
      if (string.IsNullOrEmpty(indices))
        throw new ArgumentOutOfRangeException(nameof (indices));
      IList<int[]> jaggedIndices = (IList<int[]>) null;
      ArrayIndexType? type = new ArrayIndexType?(ArrayIndexType.Standard);
      if (!SymbolParser.TryParseIndices(indices, out jaggedIndices, out type))
        throw new ArgumentOutOfRangeException(nameof (indices));
      return jaggedIndices;
    }

    /// <summary>
    /// Creates the Index String part for the Oversampling SubElement
    /// </summary>
    /// <param name="elementCount">Should be the element Count of the Dimension (one after the highest index).</param>
    /// <returns>The string representation.</returns>
    public static string OversamplingSubElementToString(int elementCount)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
      interpolatedStringHandler.AppendLiteral("[T");
      interpolatedStringHandler.AppendFormatted<int>(elementCount, "d");
      interpolatedStringHandler.AppendLiteral("]");
      return interpolatedStringHandler.ToStringAndClear();
    }

    /// <summary>Converts the SubIndex / Position to String.</summary>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="subIndex">Index of the sub.</param>
    /// <returns>System.String.</returns>
    public static string SubIndexToIndexString(int[] lowerBounds, int[] upperBounds, int subIndex)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      return lowerBounds.Length == 1 & ArrayIndexConverter.TryIsOversamplingElement(subIndex, lowerBounds, upperBounds) ? ArrayIndexConverter.OversamplingSubElementToString(subIndex) : ArrayIndexConverter.IndicesToString(ArrayIndexConverter.SubIndexToIndices(subIndex, lowerBounds, upperBounds, false));
    }

    /// <summary>
    /// Calculates an internal array that contains the factors/multiplicators of the different dimensions for position/subIndex calculations.
    /// </summary>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <returns>System.Int32[].</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// lowerBounds
    /// or
    /// upperBounds
    /// </exception>
    /// <exception cref="T:System.ArgumentException">Dimensions mismatch!</exception>
    public static int[] CalcSubIndexArray(int[] lowerBounds, int[] upperBounds)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      if (lowerBounds.Length != upperBounds.Length)
        throw new ArgumentException("Dimensions mismatch!");
      int length = lowerBounds.Length;
      int[] numArray1 = new int[length];
      for (int index = 0; index < length; ++index)
        numArray1[index] = Math.Max(upperBounds[index], lowerBounds[index]) - Math.Min(upperBounds[index], lowerBounds[index]) + 1;
      int[] numArray2 = new int[length];
      int index1 = lowerBounds.Length - 1;
      numArray2[index1] = 1;
      for (int index2 = index1 - 1; index2 >= 0; --index2)
        numArray2[index2] = numArray2[index2 + 1] * numArray1[index2 + 1];
      return numArray2;
    }

    /// <summary>
    /// Calculates the number of SubElements within the Array (including Oversampling Element)
    /// </summary>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <returns>System.Int32.</returns>
    public static int ArraySubElementCount(int[] lowerBounds, int[] upperBounds, bool oversampled)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      int num1 = 1;
      for (int index = 0; index < lowerBounds.Length; ++index)
      {
        int num2 = Math.Abs(upperBounds[index] - lowerBounds[index]) + 1;
        num1 *= num2;
      }
      if (oversampled)
        ++num1;
      return num1;
    }

    /// <summary>Gets the number of Single Elements within the Array.</summary>
    /// <param name="elements">The elements.</param>
    /// <returns>System.Int32.</returns>
    public static int ArraySubElementCount(int[] elements)
    {
      if (elements == null)
        throw new ArgumentNullException(nameof (elements));
      int num = 1;
      for (int index = 0; index < elements.Length; ++index)
        num *= elements[index];
      return num;
    }

    /// <summary>Converts subIndex / position to indices.</summary>
    /// <param name="subIndex">Subindex / Position.</param>
    /// <param name="type">The array type.</param>
    /// <returns>Indices.</returns>
    public static int[] SubIndexToIndices(int subIndex, IArrayType type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      return type is IOversamplingSupport ioversamplingSupport ? ArrayIndexConverter.SubIndexToIndices(subIndex, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, ioversamplingSupport.IsOversampled) : ArrayIndexConverter.SubIndexToIndices(subIndex, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, false);
    }

    /// <summary>Converts the subindex / position to the indices.</summary>
    /// <param name="subIndex">Subindex / position.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <returns>Indices.</returns>
    public static int[] SubIndexToIndices(
      int subIndex,
      int[] lowerBounds,
      int[] upperBounds,
      bool oversampled)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      if (lowerBounds.Length != upperBounds.Length)
        throw new ArgumentException("Dimensions mismatch!");
      if (subIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (subIndex));
      if (oversampled && lowerBounds.Length != 1)
        throw new ArgumentException("Oversampling arrays only support one dimension!");
      int length = lowerBounds.Length;
      int num = subIndex;
      int[] normalizedIndices = new int[length];
      int[] numArray = ArrayIndexConverter.CalcSubIndexArray(lowerBounds, upperBounds);
      for (int index = 0; index < length; ++index)
      {
        normalizedIndices[index] = num / numArray[index];
        num %= numArray[index];
      }
      return ArrayIndexConverter.DenormalizeIndices(normalizedIndices, lowerBounds, upperBounds, oversampled);
    }

    /// <summary>
    /// Converts the indices specifier to the subindex / position.
    /// </summary>
    /// <param name="indices">The indices (not normalized)</param>
    /// <param name="type">The array type.</param>
    /// <returns>The subindex / position.</returns>
    public static int IndicesToSubIndex(int[] indices, IArrayType type) => ArrayIndexConverter.IndicesToSubIndex(indices, type, false);

    /// <summary>
    /// Converts the indices specifier to the subindex / position.
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="type">The type.</param>
    /// <param name="normalizedIndices">if set to <c>true</c> [normalized indices].</param>
    /// <returns>System.Int32.</returns>
    public static int IndicesToSubIndex(int[] indices, IArrayType type, bool normalizedIndices)
    {
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      int[] indices1 = indices;
      bool oversampled = ArrayIndexConverter.IsOversampled(type);
      if (normalizedIndices)
        indices1 = ArrayIndexConverter.DenormalizeIndices(indices, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, oversampled);
      int subIndex = -1;
      if (!ArrayIndexConverter.TryGetSubIndex(indices1, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, oversampled, out subIndex))
        throw new ArgumentOutOfRangeException(nameof (indices));
      return subIndex;
    }

    /// <summary>
    /// Converts the indices specifier to the subindex / position.
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <returns>The subindex / position.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">indices</exception>
    public static int IndicesToSubIndex(
      int[] indices,
      int[] lowerBounds,
      int[] upperBounds,
      bool oversampled)
    {
      int subIndex = -1;
      if (!ArrayIndexConverter.TryGetSubIndex(indices, lowerBounds, upperBounds, oversampled, out subIndex))
        throw new ArgumentOutOfRangeException(nameof (indices));
      return subIndex;
    }

    /// <summary>
    /// Converts the indices specifier to the subindex / position.
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <param name="subIndex">Index of the sub.</param>
    /// <returns>The subindex / position.</returns>
    public static bool TryGetSubIndex(
      int[] indices,
      int[] lowerBounds,
      int[] upperBounds,
      bool oversampled,
      out int subIndex)
    {
      subIndex = -1;
      bool subIndex1 = ArrayIndexConverter.TryCheckIndices(indices, lowerBounds, upperBounds, false, oversampled);
      if (subIndex1)
      {
        int num = 0;
        int length = lowerBounds.Length;
        int[] numArray1 = ArrayIndexConverter.NormalizeIndices(indices, lowerBounds, upperBounds, oversampled);
        int[] numArray2 = ArrayIndexConverter.CalcSubIndexArray(lowerBounds, upperBounds);
        for (int index = 0; index < length; ++index)
          num += numArray1[index] * numArray2[index];
        subIndex = num;
      }
      return subIndex1;
    }

    /// <summary>Checks the indices against lower/upper bounds.</summary>
    /// <param name="indices">The indices.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="normalized">if set to <c>true</c> the indices are normalized.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">indices</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Indices are out of range.</exception>
    public static void CheckIndices(
      int[] indices,
      int[] lowerBounds,
      int[] upperBounds,
      bool normalized,
      bool oversampled)
    {
      if (!ArrayIndexConverter.TryCheckIndices(indices, lowerBounds, upperBounds, normalized, oversampled))
        throw new ArgumentOutOfRangeException(nameof (indices));
    }

    /// <summary>Checks the indices whether they are inside bounds.</summary>
    /// <param name="indices">The indices.</param>
    /// <param name="type">The Array type.</param>
    /// <returns><c>true</c> if the indices are insinde bounds, <c>false</c> otherwise.</returns>
    public static bool TryCheckIndices(IList<int[]> indices, IArrayType type)
    {
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      bool flag = true;
      IOversamplingSupport ioversamplingSupport = type as IOversamplingSupport;
      bool oversampled = false;
      if (ioversamplingSupport != null)
        oversampled = ioversamplingSupport.IsOversampled;
      IDataType idataType = (IDataType) type;
      foreach (int[] index in (IEnumerable<int[]>) indices)
      {
        IArrayType iarrayType = (IArrayType) idataType;
        if (iarrayType == null)
          throw new CannotResolveDataTypeException(type.ElementTypeName);
        flag |= ArrayIndexConverter.TryCheckIndices(index, iarrayType.Dimensions.LowerBounds, iarrayType.Dimensions.UpperBounds, oversampled);
        if (flag)
          idataType = type.ElementType;
        else
          break;
      }
      return flag;
    }

    /// <summary>
    /// Checks the indices whether they are inside bounds (only usable for first level of jagged arrays !!!)
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="type">The Array type.</param>
    /// <returns><c>true</c> if the indices are insinde bounds, <c>false</c> otherwise.</returns>
    public static bool TryCheckIndices(int[] indices, IArrayType type)
    {
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      return type is IOversamplingSupport ioversamplingSupport ? ArrayIndexConverter.TryCheckIndices(indices, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, ioversamplingSupport.IsOversampled) : ArrayIndexConverter.TryCheckIndices(indices, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, false);
    }

    /// <summary>Checks the indices whether they are inside bounds.</summary>
    /// <param name="indices">The indices.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="normalized">if set to <c>true</c> [normalized].</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// lowerBounds
    /// or
    /// upperBounds
    /// or
    /// indices
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// Dimensions mismatch!
    /// or
    /// Dimensions mismatch!
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">lowerBounds
    /// or
    /// upperBounds
    /// or
    /// indices</exception>
    /// <exception cref="T:System.ArgumentException">Dimensions mismatch!
    /// or
    /// Dimensions mismatch!</exception>
    public static bool TryCheckIndices(
      int[] indices,
      int[] lowerBounds,
      int[] upperBounds,
      bool normalized,
      bool oversampled)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (lowerBounds.Length != upperBounds.Length)
        throw new ArgumentException("Dimensions mismatch!");
      if (lowerBounds.Length != indices.Length)
        throw new ArgumentException("Dimensions mismatch!");
      if (oversampled)
      {
        if (indices.Length != 1)
          throw new ArgumentException("Only one dimension for oversampling arrays allowed!", nameof (indices));
        if (!normalized)
        {
          if (lowerBounds[0] <= upperBounds[0])
          {
            if (indices[0] == upperBounds[0] + 1)
              return true;
          }
          else if (indices[0] == upperBounds[0] - 1)
            return true;
        }
        else if (indices[0] == Math.Abs(upperBounds[0] - lowerBounds[0]) + 1)
          return true;
      }
      for (int index = 0; index < lowerBounds.Length; ++index)
      {
        if (!normalized)
        {
          if (indices[index] < Math.Min(lowerBounds[index], upperBounds[index]) || indices[index] > Math.Max(upperBounds[index], lowerBounds[index]))
            return false;
        }
        else if (indices[index] < 0 || indices[index] > Math.Abs(upperBounds[index] - lowerBounds[index]))
          return false;
      }
      return true;
    }

    /// <summary>Validates the indices.</summary>
    /// <param name="indices">The indices.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryCheckIndices(
      int[] indices,
      int[] lowerBounds,
      int[] upperBounds,
      bool oversampled)
    {
      return ArrayIndexConverter.TryCheckIndices(indices, lowerBounds, upperBounds, false, oversampled);
    }

    /// <summary>Validates the specified subElement index.</summary>
    /// <param name="subElement">The subElement index.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryCheckElement(
      int subElement,
      int[] lowerBounds,
      int[] upperBounds,
      bool oversampled)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      int num = ArrayIndexConverter.ArraySubElementCount(lowerBounds, upperBounds, oversampled);
      return subElement >= 0 && subElement < num;
    }

    /// <summary>Normalizes the indices.</summary>
    /// <param name="indices">The indices (non normalized within bounds).</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <returns>System.Int32[].</returns>
    public static int[] NormalizeIndices(
      int[] indices,
      int[] lowerBounds,
      int[] upperBounds,
      bool oversampled)
    {
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      ArrayIndexConverter.CheckIndices(indices, lowerBounds, upperBounds, false, oversampled);
      int[] numArray = new int[lowerBounds.Length];
      for (int index = 0; index < lowerBounds.Length; ++index)
        numArray[index] = Math.Abs(indices[index] - lowerBounds[index]);
      return numArray;
    }

    /// <summary>Normalizes the indices.</summary>
    /// <param name="indices">The indices.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Int32[].</returns>
    public static int[] NormalizeIndices(int[] indices, IArrayType type)
    {
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      return ArrayIndexConverter.NormalizeIndices(indices, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, ArrayIndexConverter.IsOversampled(type));
    }

    /// <summary>Determines whether the specified type is oversampled.</summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if the specified type is oversampled; otherwise, <c>false</c>.</returns>
    internal static bool IsOversampled(IArrayType type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      return type is IOversamplingSupport ioversamplingSupport && ioversamplingSupport.IsOversampled;
    }

    /// <summary>Denormalizes the indices.</summary>
    /// <param name="normalizedIndices">The normalized indices</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <param name="oversampled">if set to <c>true</c> [oversampled].</param>
    /// <returns>System.Int32[].</returns>
    public static int[] DenormalizeIndices(
      int[] normalizedIndices,
      int[] lowerBounds,
      int[] upperBounds,
      bool oversampled)
    {
      if (normalizedIndices == null)
        throw new ArgumentNullException(nameof (normalizedIndices));
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      ArrayIndexConverter.CheckIndices(normalizedIndices, lowerBounds, upperBounds, true, oversampled);
      int[] numArray = new int[lowerBounds.Length];
      for (int index = 0; index < lowerBounds.Length; ++index)
        numArray[index] = lowerBounds[index] > upperBounds[index] ? lowerBounds[index] - normalizedIndices[index] : lowerBounds[index] + normalizedIndices[index];
      return numArray;
    }

    /// <summary>Denormalizes the indices.</summary>
    /// <param name="normalizedIndices">The normalized indices.</param>
    /// <param name="type">The type.</param>
    /// <returns>System.Int32[].</returns>
    public static int[] DenormalizeIndices(int[] normalizedIndices, IArrayType type)
    {
      if (normalizedIndices == null)
        throw new ArgumentNullException(nameof (normalizedIndices));
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      return ArrayIndexConverter.DenormalizeIndices(normalizedIndices, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds, ArrayIndexConverter.IsOversampled(type));
    }

    /// <summary>
    /// Determines whether the subIndex specifies an oversampling element.
    /// </summary>
    /// <param name="subIndex">SubIndex / Position..</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <returns><c>true</c> if [is oversampling element] [the specified lower bounds]; otherwise, <c>false</c>.</returns>
    internal static bool IsOversamplingElement(int subIndex, int[] lowerBounds, int[] upperBounds)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      if (lowerBounds.Length != upperBounds.Length)
        throw new ArgumentException("Dimensions mismatch!");
      if (lowerBounds.Length != 1)
        throw new ArgumentOutOfRangeException(nameof (lowerBounds), "Oversampling arrays only support one Dimension!");
      return ArrayIndexConverter.IsOversamplingIndex(ArrayIndexConverter.SubIndexToIndices(subIndex, lowerBounds, upperBounds, true), lowerBounds, upperBounds);
    }

    /// <summary>
    /// Determines whether the subIndex specifies an oversampling element.
    /// </summary>
    /// <param name="subIndex">SubIndex / Position..</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <returns><c>true</c> if [is oversampling element] [the specified lower bounds]; otherwise, <c>false</c>.</returns>
    internal static bool TryIsOversamplingElement(
      int subIndex,
      int[] lowerBounds,
      int[] upperBounds)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      if (lowerBounds.Length != upperBounds.Length)
        throw new ArgumentException("Dimensions mismatch!");
      return lowerBounds.Length == 1 && ArrayIndexConverter.IsOversamplingIndex(ArrayIndexConverter.SubIndexToIndices(subIndex, lowerBounds, upperBounds, true), lowerBounds, upperBounds);
    }

    /// <summary>
    /// Determines whether the indices specify the oversampling Element
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if [is oversampling index] [the specified indices]; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// type
    /// or
    /// indices
    /// </exception>
    /// <exception cref="T:System.ArgumentException">Specified type is not an Oversampling type;type</exception>
    internal static bool IsOversamplingIndex(int[] indices, IArrayType type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (indices == null)
        throw new ArgumentNullException(nameof (indices));
      if (!(type is IOversamplingSupport ioversamplingSupport) || !ioversamplingSupport.IsOversampled)
        throw new ArgumentException("Specified type is not an Oversampling type", nameof (type));
      return ArrayIndexConverter.IsOversamplingIndex(indices, type.Dimensions.LowerBounds, type.Dimensions.UpperBounds);
    }

    /// <summary>
    /// Determines whether the indices specifies the oversampling element.
    /// </summary>
    /// <param name="indices">The indices.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <returns><c>true</c> if [is oversampling index] [the specified indices]; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// lowerBounds
    /// or
    /// upperBounds
    /// </exception>
    /// <exception cref="T:System.ArgumentException">Dimensions mismatch!</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Oversampling arrays only support one Dimension!</exception>
    internal static bool IsOversamplingIndex(int[] indices, int[] lowerBounds, int[] upperBounds)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      if (lowerBounds.Length != upperBounds.Length)
        throw new ArgumentException("Dimensions mismatch!");
      if (lowerBounds.Length != 1)
        throw new ArgumentOutOfRangeException(nameof (lowerBounds), "Oversampling arrays only support one Dimension!");
      ArrayIndexConverter.CheckIndices(indices, lowerBounds, upperBounds, false, true);
      return lowerBounds[0] <= upperBounds[0] ? indices[0] == upperBounds[0] + 1 : indices[0] == upperBounds[0] - 1;
    }

    /// <summary>Gets the dimension lenghts.</summary>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="upperBounds">The upper bounds.</param>
    /// <returns>System.Int32[].</returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    internal static int[] GetDimensionLenghts(int[] lowerBounds, int[] upperBounds)
    {
      if (lowerBounds == null)
        throw new ArgumentNullException(nameof (lowerBounds));
      if (upperBounds == null)
        throw new ArgumentNullException(nameof (upperBounds));
      if (lowerBounds.Length != upperBounds.Length)
        throw new ArgumentOutOfRangeException(nameof (upperBounds));
      int[] dimensionLenghts = new int[lowerBounds.Length];
      for (int index = 0; index < lowerBounds.Length; ++index)
        dimensionLenghts[index] = lowerBounds[index] > upperBounds[index] ? upperBounds[index] - lowerBounds[index] - 1 : upperBounds[index] - lowerBounds[index] + 1;
      return dimensionLenghts;
    }

    internal static void GetBounds(
      int[] normalizedDims,
      out int[] lowerBounds,
      out int[] upperBounds)
    {
      lowerBounds = new int[normalizedDims.Length];
      upperBounds = new int[normalizedDims.Length];
      for (int index = 0; index < normalizedDims.Length; ++index)
      {
        lowerBounds[index] = 0;
        upperBounds[index] = normalizedDims[index] - 1;
      }
    }
  }
}
