// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DataTypeStringParser
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>DataType String Parser class.</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DataTypeStringParser
  {
    private const string arraySpec = "^(?:ARRAY\\b)\\s*\\[(\\s*(?<lb>-?\\d+)\\s*\\.\\.\\s*(?<ub>-?\\d+\\s*),)*\\s*(?<lb>-?\\d+)\\s*\\.\\.\\s*(?<ub>-?\\d+\\s*)\\]\\s*OF\\s*(?<elementType>.*)$";
    private const string arraySpec2 = "^(?:ARRAY\\b)\\s*\\[({0},)*{0}\\]\\s*OF\\s*(?<elementType>{2})$";
    private const string arrayDynSpec = "^(?:ARRAY\\b)\\s*\\[(?:\\s*(?<lb>[^.]+)\\s*\\.\\.\\s*(?<ub>[^.]+),)*\\s*(?<lb>[^.]+)\\s*\\.\\.\\s*(?<ub>[^.]+\\s*)\\]\\s*OF\\s*(?<elementType>.*)$";
    private const string stringSpec = "^((?:W)?STRING\\((?<length>\\d*)\\))|((?:W)?STRING\\[(?<length>\\d*)\\])$";
    private const string pointerSpec = "^POINTER\\sTO\\s(?<pointerType>.+)$";
    private const string referenceSpec = "^REFERENCE\\sTO\\s(?<referenceType>.+)$";
    private const string subrangeSpec = "^(?<baseType>\\w+)\\s*\\((?<lb>-?\\d*)\\.\\.(?<ub>-?\\d*)\\)$";
    private const string platformBoundSpec = "^(?:(?:XINT)|(?:UXINT)|(?:XWORD))$";
    private const string plcOpenSpec = "^(?:(?:TOD)|(?:DT)|(?:TIME)|(?:DATE)|(?:LTIME)|(?:TIME_OF_DAY)|(?:DATE_AND_TIME))$";
    private const string integralSpec = "^(?:(?:XINT)|(?:UXINT)|(?:XWORD)|(?:TOD)|(?:DT)|(?:TIME)|(?:DATE)|(?:LTIME)|(?:TIME_OF_DAY)|(?:DATE_AND_TIME))$";
    /// <summary>
    /// Regular expression, parsing a Standard Array name with specified Dimension boundaries.
    /// </summary>
    private static Regex arrayExpression = new Regex("^(?:ARRAY\\b)\\s*\\[(\\s*(?<lb>-?\\d+)\\s*\\.\\.\\s*(?<ub>-?\\d+\\s*),)*\\s*(?<lb>-?\\d+)\\s*\\.\\.\\s*(?<ub>-?\\d+\\s*)\\]\\s*OF\\s*(?<elementType>.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    /// <summary>
    /// Regular expression, ,parsing a Dynamic Array name, where the Dimension boundaries are return as unparsed string.
    /// </summary>
    private static Regex arrayDynExpression = new Regex("^(?:ARRAY\\b)\\s*\\[(?:\\s*(?<lb>[^.]+)\\s*\\.\\.\\s*(?<ub>[^.]+),)*\\s*(?<lb>[^.]+)\\s*\\.\\.\\s*(?<ub>[^.]+\\s*)\\]\\s*OF\\s*(?<elementType>.*)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex stringExpression = new Regex("^((?:W)?STRING\\((?<length>\\d*)\\))|((?:W)?STRING\\[(?<length>\\d*)\\])$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex pointerExpression = new Regex("^POINTER\\sTO\\s(?<pointerType>.+)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex referenceExpression = new Regex("^REFERENCE\\sTO\\s(?<referenceType>.+)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex subRangeExpression = new Regex("^(?<baseType>\\w+)\\s*\\((?<lb>-?\\d*)\\.\\.(?<ub>-?\\d*)\\)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex platformBoundExpression = new Regex("^(?:(?:XINT)|(?:UXINT)|(?:XWORD))$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex plcOpenExpression = new Regex("^(?:(?:TOD)|(?:DT)|(?:TIME)|(?:DATE)|(?:LTIME)|(?:TIME_OF_DAY)|(?:DATE_AND_TIME))$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex integralExpression = new Regex("^(?:(?:XINT)|(?:UXINT)|(?:XWORD)|(?:TOD)|(?:DT)|(?:TIME)|(?:DATE)|(?:LTIME)|(?:TIME_OF_DAY)|(?:DATE_AND_TIME))$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>Tries to parse the string.</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="length">The character count of the string (not including the /0)</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns><c>true</c> if successfull, <c>false</c> otherwise.</returns>
    internal static bool TryParseString(string typeName, out int length, [NotNullWhen(true)] out Encoding? encoding)
    {
      string input = typeName.Trim();
      Match match = DataTypeStringParser.stringExpression.Match(input);
      length = -1;
      if (match.Success)
      {
        Group group = match.Groups[nameof (length)];
        int count = group.Captures.Count;
        length = int.Parse(group.Captures[0].Value, (IFormatProvider) CultureInfo.InvariantCulture);
        int num = input[0] == 'w' ? 1 : (input[0] == 'W' ? 1 : 0);
        encoding = num == 0 ? StringMarshaler.DefaultEncoding : StringMarshaler.UnicodeEncoding;
        return true;
      }
      encoding = (Encoding) null;
      return false;
    }

    /// <summary>Determines whether the specified type name is string.</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns><c>true</c> if the specified type name is string; otherwise, <c>false</c>.</returns>
    internal static bool IsString(string typeName)
    {
      Encoding encoding = (Encoding) null;
      return DataTypeStringParser.TryParseString(typeName, out int _, out encoding);
    }

    internal static bool TryParseArray(
      string typeName,
      [NotNullWhen(true)] out DimensionCollection? dims,
      [NotNullWhen(true)] out string? baseType)
    {
      Match match = DataTypeStringParser.arrayExpression.Match(typeName);
      dims = (DimensionCollection) null;
      baseType = (string) null;
      if (!match.Success)
        return false;
      baseType = match.Groups["elementType"].Value;
      Group group1 = match.Groups["ub"];
      Group group2 = match.Groups["lb"];
      int count = group1.Captures.Count;
      dims = new DimensionCollection();
      for (int i = 0; i < count; ++i)
      {
        object capture = (object) group2.Captures[i];
        int num1 = int.Parse(group2.Captures[i].Value, (IFormatProvider) CultureInfo.InvariantCulture);
        int num2 = int.Parse(group1.Captures[i].Value, (IFormatProvider) CultureInfo.InvariantCulture) - num1 + 1;
        dims.Add((IDimension) new Dimension(num1, num2));
      }
      return true;
    }

    internal static bool TryParseDynamicArray(
      string typeName,
      [NotNullWhen(true)] out List<List<Dictionary<string, string>>>? list,
      [NotNullWhen(true)] out List<string>? baseTypes)
    {
      string baseType = typeName;
      bool dynamicArray = false;
      list = new List<List<Dictionary<string, string>>>();
      baseTypes = new List<string>();
      baseTypes.Add(typeName);
      List<Dictionary<string, string>> dims = (List<Dictionary<string, string>>) null;
      while (DataTypeStringParser.TryParseDynamicArray(baseType, out dims, out baseType))
      {
        dynamicArray = true;
        list.Add(dims);
        baseTypes.Add(baseType);
      }
      if (!dynamicArray)
      {
        list = (List<List<Dictionary<string, string>>>) null;
        baseTypes = (List<string>) null;
      }
      return dynamicArray;
    }

    /// <summary>Tries to parse a dynamic array string</summary>
    /// <param name="typeName">Name of the Dynamic array.</param>
    /// <param name="dims">List of Dimension specifications.</param>
    /// <param name="baseType">Base type string</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <remarks>The Problem is, that dynamic arrays contain expressions/Variables for Upper/Lower Bound and not the real Dimension values.
    /// Therefore, this function parses the dimensions only as Dictionary (LowerBound/UpperBound --&gt; Value), in a list for each dimension.
    /// Furthermore, the base Type string of the ElementType is extracted.
    /// </remarks>
    private static bool TryParseDynamicArray(
      string typeName,
      [NotNullWhen(true)] out List<Dictionary<string, string>>? dims,
      [NotNullWhen(true)] out string? baseType)
    {
      Match match = DataTypeStringParser.arrayDynExpression.Match(typeName);
      dims = (List<Dictionary<string, string>>) null;
      baseType = (string) null;
      if (!match.Success)
        return false;
      dims = new List<Dictionary<string, string>>();
      baseType = match.Groups["elementType"].Value;
      Group group1 = match.Groups["ub"];
      Group group2 = match.Groups["lb"];
      int count = group1.Captures.Count;
      for (int i = 0; i < count; ++i)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string str1 = group2.Captures[i].Value;
        string str2 = group1.Captures[i].Value;
        dictionary.Add("LowerBound", str1);
        dictionary.Add("UpperBound", str2);
        dims.Add(dictionary);
      }
      return true;
    }

    /// <summary>Determines whether the specified type name is array.</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns><c>true</c> if the specified type name is array; otherwise, <c>false</c>.</returns>
    internal static bool IsArray(string typeName)
    {
      DimensionCollection dims = (DimensionCollection) null;
      string baseType = (string) null;
      return DataTypeStringParser.TryParseArray(typeName, out dims, out baseType);
    }

    /// <summary>Tries to parse the pointer type</summary>
    /// <param name="typeName">Name of the Pointer type</param>
    /// <param name="referencedType">Type of the referenced type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">typteName</exception>
    internal static bool TryParsePointer(string typeName, [NotNullWhen(true)] out string? referencedType)
    {
      if (string.IsNullOrEmpty(typeName))
        throw new ArgumentOutOfRangeException(nameof (typeName));
      referencedType = (string) null;
      Match match = DataTypeStringParser.pointerExpression.Match(typeName);
      if (!match.Success)
        return false;
      Group group = match.Groups["pointerType"];
      referencedType = group.Captures[0].Value;
      return true;
    }

    /// <summary>
    /// Determines whether the specified type name is pointer.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns><c>true</c> if the specified type name is pointer; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">typeName</exception>
    internal static bool IsPointer(string typeName) => !string.IsNullOrEmpty(typeName) ? DataTypeStringParser.TryParsePointer(typeName, out string _) : throw new ArgumentOutOfRangeException(nameof (typeName));

    /// <summary>Tries to parse a reference type</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="referencedType">Type of the referenced.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal static bool TryParseReference(string typeName, [NotNullWhen(true)] out string? referencedType)
    {
      referencedType = (string) null;
      Match match = DataTypeStringParser.referenceExpression.Match(typeName);
      if (!match.Success)
        return false;
      Group group = match.Groups["referenceType"];
      referencedType = group.Captures[0].Value;
      return true;
    }

    /// <summary>
    /// Determines whether the specified type name is reference.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns><c>true</c> if the specified type name is reference; otherwise, <c>false</c>.</returns>
    internal static bool IsReference(string typeName) => DataTypeStringParser.TryParseReference(typeName, out string _);

    internal static bool TryParseSubRange(string typeName, [NotNullWhen(true)] out string? baseType)
    {
      Match match = DataTypeStringParser.subRangeExpression.Match(typeName);
      baseType = (string) null;
      if (!match.Success)
        return false;
      Group group = match.Groups[nameof (baseType)];
      baseType = group.Captures[0].Value;
      return true;
    }

    /// <summary>Tries to parse the string as SubRange type.</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <param name="baseType">Type of the base.</param>
    /// <param name="lowerBound">The lower bound.</param>
    /// <param name="upperBound">The upper bound.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal static bool TryParseSubRange<T>(
      string typeName,
      [NotNullWhen(true)] out string? baseType,
      out T lowerBound,
      out T upperBound)
      where T : struct, IConvertible
    {
      lowerBound = default (T);
      upperBound = default (T);
      string lowerBound1 = (string) null;
      string upperBound1 = (string) null;
      return DataTypeStringParser.TryParseSubRange(typeName, out baseType, out lowerBound1, out upperBound1) && DataTypeStringParser.TryParse<T>(lowerBound1, out lowerBound) & DataTypeStringParser.TryParse<T>(upperBound1, out upperBound);
    }

    internal static bool TryParseSubRange(
      string typeName,
      Type managedBaseType,
      [NotNullWhen(true)] out string? baseType,
      [NotNullWhen(true)] out object? lowerBound,
      [NotNullWhen(true)] out object? upperBound)
    {
      bool subRange = false;
      lowerBound = (object) null;
      upperBound = (object) null;
      string lowerBound1 = (string) null;
      string upperBound1 = (string) null;
      if (DataTypeStringParser.TryParseSubRange(typeName, out baseType, out lowerBound1, out upperBound1))
        subRange = DataTypeStringParser.TryParse(lowerBound1, managedBaseType, out lowerBound) & DataTypeStringParser.TryParse(upperBound1, managedBaseType, out upperBound);
      if (!subRange)
      {
        baseType = (string) null;
        lowerBound = (object) null;
        upperBound = (object) null;
      }
      return subRange;
    }

    internal static bool TryParseSubRange(
      string typeName,
      [NotNullWhen(true)] out string? baseType,
      [NotNullWhen(true)] out string? lowerBound,
      [NotNullWhen(true)] out string? upperBound)
    {
      Match match = DataTypeStringParser.subRangeExpression.Match(typeName);
      baseType = (string) null;
      lowerBound = (string) null;
      upperBound = (string) null;
      if (!match.Success)
        return false;
      Group group1 = match.Groups[nameof (baseType)];
      Group group2 = match.Groups["lb"];
      Group group3 = match.Groups["ub"];
      baseType = group1.Captures[0].Value;
      lowerBound = group2.Captures[0].Value;
      upperBound = group3.Captures[0].Value;
      return true;
    }

    internal static bool TryParse<T>(string str, out T value) where T : struct, IConvertible
    {
      bool flag = false;
      try
      {
        value = (T) Convert.ChangeType((object) str, typeof (T), (IFormatProvider) null);
        flag = true;
      }
      catch (OverflowException ex)
      {
        value = default (T);
      }
      catch (NotSupportedException ex)
      {
        value = default (T);
      }
      return flag;
    }

    internal static bool TryParse(string str, Type type, [NotNullWhen(true)] out object? value)
    {
      try
      {
        value = Convert.ChangeType((object) str, type, (IFormatProvider) null);
        return true;
      }
      catch (OverflowException ex)
      {
        value = (object) null;
      }
      catch (NotSupportedException ex)
      {
        value = (object) null;
      }
      catch (FormatException ex)
      {
        value = (object) null;
      }
      return false;
    }

    /// <summary>
    /// Determines whether the specified string is a subrange type.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns><c>true</c> if [is sub range] [the specified type name]; otherwise, <c>false</c>.</returns>
    internal static bool IsSubRange(string typeName) => DataTypeStringParser.TryParseSubRange(typeName, out string _);

    /// <summary>
    /// Determines whether the specified type name is a 'special type' (UXINT,XINT,XWORD).
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns><c>true</c> if [is special type name] [the specified type name]; otherwise, <c>false</c>.</returns>
    internal static bool IsPlatformBoundType(string typeName) => DataTypeStringParser.platformBoundExpression.Match(typeName).Success;

    /// <summary>
    /// Determines whether the specified Type name is an (complex) intrinsic type
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <remarks>
    /// (TOD)|(DT)|(TIME)|(DATE)|(LTIME)|(TIME_OF_DAY)|(DATE_AND_TIME)
    /// (XINT)|(UXINT)|(XWORD)
    /// </remarks>
    /// <returns><c>true</c> if [is intrinsic type] [the specified type name]; otherwise, <c>false</c>.</returns>
    internal static bool IsIntegralType(string typeName) => DataTypeStringParser.integralExpression.Match(typeName).Success;

    /// <summary>
    /// Determines whether the specified Type name is one of the PLC Open types
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <remarks>
    /// (TOD)|(DT)|(TIME)|(DATE)|(LTIME)|(TIME_OF_DAY)|(DATE_AND_TIME)
    /// </remarks>
    /// <returns><c>true</c> if [is intrinsic type] [the specified type name]; otherwise, <c>false</c>.</returns>
    internal static bool IsPlcOpenType(string typeName) => DataTypeStringParser.plcOpenExpression.Match(typeName).Success;
  }
}
