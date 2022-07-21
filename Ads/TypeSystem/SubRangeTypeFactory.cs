// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.SubRangeTypeFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Type Factory for <see cref="T:TwinCAT.TypeSystem.ISubRangeType" /> implementing types.
  /// </summary>
  internal static class SubRangeTypeFactory
  {
    internal static bool TryCreate(
      string name,
      IDataTypeResolver resolver,
      [NotNullWhen(true)] out ISubRangeType? subRange)
    {
      subRange = (ISubRangeType) null;
      string baseType1 = (string) null;
      if (DataTypeStringParser.TryParseSubRange(name, out baseType1))
      {
        IDataType baseType2 = (IDataType) null;
        if (resolver.TryResolveType(baseType1, ref baseType2))
          SubRangeTypeFactory.TryCreate(name, baseType2, out subRange);
      }
      return subRange != null;
    }

    private static bool TryCreate(string name, IDataType baseType, [NotNullWhen(true)] out ISubRangeType? subRange)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (baseType == null)
        throw new ArgumentNullException(nameof (baseType));
      subRange = (ISubRangeType) null;
      Type managedType = ((IManagedMappableType) baseType).ManagedType;
      string baseType1;
      if (managedType == typeof (sbyte))
      {
        sbyte lowerBound;
        sbyte upperBound;
        if (DataTypeStringParser.TryParseSubRange<sbyte>(name, out baseType1, out lowerBound, out upperBound))
          subRange = (ISubRangeType) new SubRangeType<sbyte>(name, baseType.Name, 1, lowerBound, upperBound);
      }
      else if (managedType == typeof (byte))
      {
        byte lowerBound;
        byte upperBound;
        if (DataTypeStringParser.TryParseSubRange<byte>(name, out baseType1, out lowerBound, out upperBound))
          subRange = (ISubRangeType) new SubRangeType<byte>(name, baseType.Name, 1, lowerBound, upperBound);
      }
      else if (managedType == typeof (short))
      {
        short lowerBound;
        short upperBound;
        if (DataTypeStringParser.TryParseSubRange<short>(name, out baseType1, out lowerBound, out upperBound))
          subRange = (ISubRangeType) new SubRangeType<short>(name, baseType.Name, 2, lowerBound, upperBound);
      }
      else if (managedType == typeof (ushort))
      {
        ushort lowerBound;
        ushort upperBound;
        if (DataTypeStringParser.TryParseSubRange<ushort>(name, out baseType1, out lowerBound, out upperBound))
          subRange = (ISubRangeType) new SubRangeType<ushort>(name, baseType.Name, 2, lowerBound, upperBound);
      }
      else if (managedType == typeof (int))
      {
        int lowerBound;
        int upperBound;
        if (DataTypeStringParser.TryParseSubRange<int>(name, out baseType1, out lowerBound, out upperBound))
          subRange = (ISubRangeType) new SubRangeType<int>(name, baseType.Name, 4, lowerBound, upperBound);
      }
      else if (managedType == typeof (uint))
      {
        uint lowerBound;
        uint upperBound;
        if (DataTypeStringParser.TryParseSubRange<uint>(name, out baseType1, out lowerBound, out upperBound))
          subRange = (ISubRangeType) new SubRangeType<uint>(name, baseType.Name, 4, lowerBound, upperBound);
      }
      else if (managedType == typeof (long))
      {
        long lowerBound;
        long upperBound;
        if (DataTypeStringParser.TryParseSubRange<long>(name, out baseType1, out lowerBound, out upperBound))
          subRange = (ISubRangeType) new SubRangeType<long>(name, baseType.Name, 8, lowerBound, upperBound);
      }
      else
      {
        ulong lowerBound;
        ulong upperBound;
        if (managedType == typeof (ulong) && DataTypeStringParser.TryParseSubRange<ulong>(name, out baseType1, out lowerBound, out upperBound))
          subRange = (ISubRangeType) new SubRangeType<ulong>(name, baseType.Name, 8, lowerBound, upperBound);
      }
      return subRange != null;
    }

    internal static bool TryCreate(
      AdsDataTypeEntry entry,
      IDataTypeResolver resolver,
      [NotNullWhen(true)] out ISubRangeType? subRange)
    {
      subRange = (ISubRangeType) null;
      string baseType1 = (string) null;
      if (DataTypeStringParser.TryParseSubRange(entry.EntryName, out baseType1))
      {
        IDataType baseType2 = (IDataType) null;
        if (resolver.TryResolveType(baseType1, ref baseType2))
          SubRangeTypeFactory.TryCreate(entry.EntryName, baseType2, out subRange);
      }
      return subRange != null;
    }
  }
}
