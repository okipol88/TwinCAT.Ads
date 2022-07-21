// Decompiled with JetBrains decompiler
// Type: SymbolParser
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
using TwinCAT.Ads;
using TwinCAT.Ads.Internal;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
/// <summary>
/// Class parsing Symbols from String or from AdsStream (for internal use only)
/// </summary>
/// <exclude />
[EditorBrowsable(EditorBrowsableState.Never)]
public static class SymbolParser
{
  private const string arrIndex2 = "(?<jagged>(?:\\[\\s*(?<arrayIndices>(?<indices>-?\\d+)(?:,\\s*(?<indices>-?\\d+))*)\\]))+";
  private const string arrIndexOver = "(?:\\[T(?<oversamplingIndex>\\d+)\\])";
  private const string indexSpec = "^(?:(?<jagged>(?:\\[\\s*(?<arrayIndices>(?<indices>-?\\d+)(?:,\\s*(?<indices>-?\\d+))*)\\]))+|(?:\\[T(?<oversamplingIndex>\\d+)\\]))$";
  private const string arrayInstSpec = "^(?:(?<path>\\S*)\\.)?(?:(?<name>[^\\[\\]\\s]+)(?<indicesStr>(?<jagged>(?:\\[\\s*(?<arrayIndices>(?<indices>-?\\d+)(?:,\\s*(?<indices>-?\\d+))*)\\]))+|(?:\\[T(?<oversamplingIndex>\\d+)\\])))$";
  private static Regex arrayExpression = new Regex("^(?:(?<path>\\S*)\\.)?(?:(?<name>[^\\[\\]\\s]+)(?<indicesStr>(?<jagged>(?:\\[\\s*(?<arrayIndices>(?<indices>-?\\d+)(?:,\\s*(?<indices>-?\\d+))*)\\]))+|(?:\\[T(?<oversamplingIndex>\\d+)\\])))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
  private static Regex arrayIndexExpression = new Regex("^(?:(?<jagged>(?:\\[\\s*(?<arrayIndices>(?<indices>-?\\d+)(?:,\\s*(?<indices>-?\\d+))*)\\]))+|(?:\\[T(?<oversamplingIndex>\\d+)\\]))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

  internal static bool TryParseType(string typeName, IBinder resolver, [NotNullWhen(true)] out IDataType? type)
  {
    if (string.IsNullOrEmpty(typeName))
      throw new ArgumentOutOfRangeException(nameof (typeName));
    if (resolver == null)
      throw new ArgumentNullException(nameof (resolver));
    DataType dataType = (DataType) null;
    int length = -1;
    Encoding encoding = (Encoding) null;
    DimensionCollection dims = (DimensionCollection) null;
    string baseType = (string) null;
    string referencedType = (string) null;
    ISubRangeType subRange = (ISubRangeType) null;
    if (DataTypeStringParser.TryParseString(typeName, out length, out encoding))
      dataType = encoding != StringMarshaler.UnicodeEncoding ? (DataType) new StringType(length, encoding) : (DataType) new WStringType(length);
    else if (DataTypeStringParser.TryParseArray(typeName, out dims, out baseType))
    {
      IDataType elementType = (IDataType) null;
      if (((IDataTypeResolver) resolver).TryResolveType(baseType, ref elementType))
        dataType = (DataType) new ArrayType(typeName, elementType, dims, (AdsDataTypeFlags) 1);
    }
    else if (DataTypeStringParser.TryParsePointer(typeName, out referencedType))
      dataType = (DataType) new PointerType(typeName, referencedType, ((IDataTypeResolver) resolver).PlatformPointerSize);
    else if (DataTypeStringParser.TryParseReference(typeName, out referencedType))
      dataType = (DataType) new ReferenceType(typeName, referencedType, ((IDataTypeResolver) resolver).PlatformPointerSize);
    else if (SubRangeTypeFactory.TryCreate(typeName, (IDataTypeResolver) resolver, out subRange))
      dataType = (DataType) subRange;
    if (dataType != null)
    {
      resolver.RegisterType((IDataType) dataType);
      resolver.OnTypeGenerated((IDataType) dataType);
      type = (IDataType) dataType;
    }
    else
    {
      type = (IDataType) null;
      resolver.OnTypeResolveError(typeName);
      AdsModule.Trace.TraceWarning("Type '" + typeName + "' could not be resolved. Not in DataType tables. Ignoring Type!");
    }
    return type != null;
  }

  /// <summary>Tries to parse an Array Instance</summary>
  /// <param name="fullPath">The name with indices string.</param>
  /// <param name="fullPathWithoutIndexes">The instanceName.</param>
  /// <param name="instanceName">Name of the instance.</param>
  /// <param name="indicesStr">The indices string.</param>
  /// <param name="jaggedIndices">The indices.</param>
  /// <param name="type">The type.</param>
  /// <returns><c>true</c> if the string specifies an array instance, <c>false</c> otherwise.</returns>
  internal static bool TryParseArrayElement(
    string fullPath,
    [NotNullWhen(true)] out string? fullPathWithoutIndexes,
    [NotNullWhen(true)] out string? instanceName,
    [NotNullWhen(true)] out string? indicesStr,
    [NotNullWhen(true)] out IList<int[]>? jaggedIndices,
    [NotNullWhen(true)] out ArrayIndexType? type)
  {
    string input = fullPath.Trim();
    fullPathWithoutIndexes = string.Empty;
    instanceName = string.Empty;
    Match match = SymbolParser.arrayExpression.Match(input);
    if (match.Success)
    {
      Group group1 = match.Groups["path"];
      if (group1 != null && group1.Captures.Count > 0 && group1.Captures[0] != null)
        fullPathWithoutIndexes = group1.Captures[0]?.ToString() + ".";
      Group group2 = match.Groups["name"];
      instanceName = group2.Captures[0].Value;
      fullPathWithoutIndexes += instanceName;
      Group group3 = match.Groups[nameof (indicesStr)];
      indicesStr = group3.Captures[0].Value;
      Group group4 = match.Groups["indices"];
      Group group5 = match.Groups["arrayIndices"];
      Group group6 = match.Groups["oversamplingIndex"];
      if (group5.Captures.Count > 0)
      {
        jaggedIndices = SymbolParser.getIndicesFromGroup(group5);
        type = jaggedIndices.Count <= 1 ? new ArrayIndexType?(ArrayIndexType.Standard) : new ArrayIndexType?(ArrayIndexType.Jagged);
        return true;
      }
      if (group6.Captures.Count > 0)
      {
        jaggedIndices = SymbolParser.getIndicesFromGroup(group6);
        type = new ArrayIndexType?(ArrayIndexType.Oversample);
        return true;
      }
    }
    fullPathWithoutIndexes = (string) null;
    instanceName = (string) null;
    indicesStr = (string) null;
    jaggedIndices = (IList<int[]>) null;
    type = new ArrayIndexType?(ArrayIndexType.Standard);
    return false;
  }

  /// <summary>Tries to parse the indices from an indices string.</summary>
  /// <param name="indicesStr">The indices string.</param>
  /// <param name="jaggedIndices">The jagged indices.</param>
  /// <param name="type">The type.</param>
  /// <returns><c>true</c> if the string specifies an array instance, <c>false</c> otherwise.</returns>
  internal static bool TryParseIndices(
    string indicesStr,
    [NotNullWhen(true)] out IList<int[]>? jaggedIndices,
    [NotNullWhen(true)] out ArrayIndexType? type)
  {
    string input = indicesStr.Trim();
    Match match = SymbolParser.arrayIndexExpression.Match(input);
    if (match.Success)
    {
      Group group1 = match.Groups["arrayIndices"];
      Group group2 = match.Groups["oversamplingIndex"];
      if (group1.Captures.Count > 0)
      {
        jaggedIndices = SymbolParser.getIndicesFromGroup(group1);
        type = jaggedIndices.Count <= 1 ? new ArrayIndexType?(ArrayIndexType.Standard) : new ArrayIndexType?(ArrayIndexType.Jagged);
        return true;
      }
      if (group2.Captures.Count > 0)
      {
        jaggedIndices = SymbolParser.getIndicesFromGroup(group2);
        type = new ArrayIndexType?(ArrayIndexType.Oversample);
        return true;
      }
    }
    jaggedIndices = (IList<int[]>) null;
    type = new ArrayIndexType?(ArrayIndexType.Standard);
    return false;
  }

  private static IList<int[]> getIndicesFromGroup(Group group)
  {
    List<int[]> indicesFromGroup = new List<int[]>();
    for (int i = 0; i < group.Captures.Count; ++i)
    {
      string[] strArray = group.Captures[i].Value.Split(',');
      int[] numArray = new int[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        numArray[index] = int.Parse(strArray[index], (IFormatProvider) CultureInfo.InvariantCulture);
      indicesFromGroup.Add(numArray);
    }
    return (IList<int[]>) indicesFromGroup;
  }

  /// <summary>
  /// Tries to parse the parent path of this <see cref="T:TwinCAT.Ads.TypeSystem.Symbol" />
  /// </summary>
  /// <param name="symbol">The symbol.</param>
  /// <param name="parentPath">The parent path (out-parameter).</param>
  /// <param name="parentName">Name of the parent (out-parameter).</param>
  /// <returns>true if found, false if not contained.</returns>
  internal static bool TryParseParentPath(
    IInstance symbol,
    [NotNullWhen(true)] out string? parentPath,
    [NotNullWhen(true)] out string? parentName)
  {
    bool parentPath1 = false;
    string[] sourceArray = symbol.InstancePath.Split('.');
    int length = sourceArray.GetLength(0);
    if (length >= 2)
    {
      if (!string.IsNullOrEmpty(sourceArray[length - 2]))
      {
        parentName = sourceArray[length - 2];
        string[] strArray = new string[length - 1];
        Array.Copy((Array) sourceArray, (Array) strArray, length - 1);
        parentPath = string.Join<string>(".", (IEnumerable<string>) strArray);
        parentPath1 = true;
      }
      else
      {
        parentPath = (string) null;
        parentName = (string) null;
      }
    }
    else
    {
      parentPath = (string) null;
      parentName = (string) null;
    }
    return parentPath1;
  }
}
