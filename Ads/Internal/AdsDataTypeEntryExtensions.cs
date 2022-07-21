// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsDataTypeEntryExtensions
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Extension class for <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" />
  /// </summary>
  /// <remarks>Specifies some supporting methods.</remarks>
  internal static class AdsDataTypeEntryExtensions
  {
    internal static DataTypeCategory GetCategory(
      this AdsDataTypeEntry entry,
      int platformPointerSize)
    {
      string referencedType = (string) null;
      int length = 0;
      Encoding encoding = (Encoding) null;
      string baseType = (string) null;
      if (entry.EntryName == "PVOID")
        return (DataTypeCategory) 13;
      if (entry.EntryName == "PCCH")
        return (DataTypeCategory) 13;
      if (entry.EntryName == "BOOL")
        return (DataTypeCategory) 1;
      if (DataTypeStringParser.IsPlatformBoundType(entry.EntryName))
        return (DataTypeCategory) 1;
      if (DataTypeStringParser.IsPlcOpenType(entry.EntryName))
        return (DataTypeCategory) 1;
      if (DataTypeStringParser.TryParseString(entry.EntryName, out length, out encoding))
        return (DataTypeCategory) 10;
      if (DataTypeStringParser.TryParseReference(entry.EntryName, out referencedType))
        return (DataTypeCategory) 15;
      if (DataTypeStringParser.TryParsePointer(entry.EntryName, out referencedType))
        return (DataTypeCategory) 13;
      if (DataTypeStringParser.TryParseSubRange(entry.EntryName, out baseType))
        return (DataTypeCategory) 9;
      if (entry.IsArray(platformPointerSize))
        return (DataTypeCategory) 4;
      if (entry.IsEnum(platformPointerSize))
        return (DataTypeCategory) 3;
      if (entry.IsInterface(platformPointerSize))
        return (DataTypeCategory) 16;
      if (entry.IsUnion(platformPointerSize))
        return (DataTypeCategory) 14;
      if (entry.IsAlias(platformPointerSize))
        return (DataTypeCategory) 2;
      return entry.IsStruct(platformPointerSize) ? (DataTypeCategory) 5 : CategoryConverter.FromId(entry.DataTypeId);
    }

    internal static bool IsArray(this AdsDataTypeEntry entry, int platformPointerSize) => entry.ArrayDimCount > 0;

    internal static bool IsEnum(this AdsDataTypeEntry entry, int platformPointerSize) => entry.EnumInfoCount > 0;

    internal static bool IsAlias(this AdsDataTypeEntry entry, int platformPointerSize) => !string.IsNullOrEmpty(entry.BaseTypeName) && entry.SubItemCount <= 0 && entry.MethodCount <= 0 && entry.EnumInfoCount <= 0 && (entry.ArrayInfos == null || entry.ArrayInfos.Length == 0) && !entry.IsImplementation();

    internal static bool IsInterface(this AdsDataTypeEntry entry, int platformPointerSize) => entry.ArrayDimCount == 0 && entry.EnumInfoCount == 0 && entry.ByteSize == platformPointerSize && entry.SubEntries != null && entry.SubEntries.Length != 0 && !entry.HasFields() && entry.HasProperties();

    internal static bool IsUnion(this AdsDataTypeEntry entry, int platformPointerSize) => entry.SubItemCount != 0 && entry.ArrayDimCount <= 0 && entry.EnumInfoCount <= 0 && entry.MethodCount <= 0 && string.IsNullOrEmpty(entry.BaseTypeName) && entry.HasFieldOffsetOverlap();

    internal static bool HasFieldOffsetOverlap(this AdsDataTypeEntry entry)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (entry.SubItemCount > 1)
      {
        int num1 = 0;
        foreach (AdsFieldEntry subEntry in entry.SubEntries)
        {
          if (!subEntry.IsStatic && !subEntry.IsProperty && subEntry.Offset >= 0)
          {
            int num2 = !subEntry.IsBitType ? subEntry.Offset * 8 : subEntry.Offset;
            flag1 |= num2 < num1;
            num1 += subEntry.BitSize;
          }
        }
        flag2 = entry.BitSize < num1;
      }
      return flag1;
    }

    internal static bool IsStruct(this AdsDataTypeEntry entry, int platformPointerSize)
    {
      if (entry.BaseTypeId != 65 || entry.ArrayDimCount > 0 || entry.EnumInfoCount > 0)
        return false;
      if (entry.IsImplementation())
        return true;
      return !entry.IsUnion(platformPointerSize) && !entry.IsInterface(platformPointerSize);
    }

    internal static bool HasProperties(this AdsDataTypeEntry entry)
    {
      AdsFieldEntry[] subEntries = entry.SubEntries;
      return (subEntries != null ? ((IEnumerable<AdsFieldEntry>) subEntries).FirstOrDefault<AdsFieldEntry>((Func<AdsFieldEntry, bool>) (e => ((Enum) (object) e.Flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 64))) : (AdsFieldEntry) null) != null;
    }

    internal static bool HasFields(this AdsDataTypeEntry entry)
    {
      AdsFieldEntry[] subEntries = entry.SubEntries;
      return (subEntries != null ? ((IEnumerable<AdsFieldEntry>) subEntries).FirstOrDefault<AdsFieldEntry>((Func<AdsFieldEntry, bool>) (e => !((Enum) (object) e.Flags).HasFlag((Enum) (object) (AdsDataTypeFlags) 64))) : (AdsFieldEntry) null) != null;
    }

    internal static bool IsImplementation(this AdsDataTypeEntry entry, [NotNullWhen(true)] out IList<string>? interfaces)
    {
      List<string> list = ((IEnumerable<ITypeAttribute>) entry.Attributes).Where<ITypeAttribute>((Func<ITypeAttribute, bool>) (a => a.Name.Equals("TcImplements", StringComparison.OrdinalIgnoreCase))).Select<ITypeAttribute, string>((Func<ITypeAttribute, string>) (a => a.Value)).ToList<string>();
      interfaces = list.Count <= 0 ? (IList<string>) null : (IList<string>) list;
      return list.Count > 0;
    }

    internal static bool IsImplementation(this AdsDataTypeEntry entry) => ((IEnumerable<ITypeAttribute>) entry.Attributes).FirstOrDefault<ITypeAttribute>((Func<ITypeAttribute, bool>) (a => a.Name.Equals("TcImplements", StringComparison.OrdinalIgnoreCase))) != null;
  }
}
