// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsDataTypeFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Text;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>
  /// Factory class, producing <see cref="T:TwinCAT.Ads.TypeSystem.DataType" />s from <see cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" /> objects.
  /// </summary>
  internal static class AdsDataTypeFactory
  {
    /// <summary>Creates the type.</summary>
    /// <param name="entry">The entry.</param>
    /// <param name="factoryServices">The factory services.</param>
    /// <returns>DataType.</returns>
    /// <exception cref="T:TwinCAT.AdsException">Enum base type mismatch!</exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    internal static DataType CreateType(
      AdsDataTypeEntry entry,
      ISymbolFactoryServices factoryServices)
    {
      switch ((int) entry.GetCategory(((IDataTypeResolver) factoryServices.Binder).PlatformPointerSize))
      {
        case 1:
          if (DataTypeStringParser.IsPlatformBoundType(entry.EntryName))
            return (DataType) new PlatformBoundType(entry);
          if (DataTypeStringParser.IsPlcOpenType(entry.EntryName))
            return (DataType) new PlcOpenType(entry);
          if (entry.EntryName == "BOOL")
            return (DataType) new PrimitiveType(entry, (PrimitiveTypeFlags) 4, typeof (bool));
          Type tp = (Type) null;
          PrimitiveTypeMarshaler.TryGetManagedType(entry.DataTypeId, out tp);
          return (DataType) new PrimitiveType(entry, PrimitiveTypeMarshaler.GetPrimitiveFlags(entry.DataTypeId), tp);
        case 2:
          return (DataType) new AliasType(entry);
        case 3:
          AdsDataTypeId baseTypeId = entry.BaseTypeId;
          if (baseTypeId == 2)
            return (DataType) new EnumType<short>(entry);
          if (baseTypeId == 3)
            return (DataType) new EnumType<int>(entry);
          switch (baseTypeId - 16)
          {
            case 0:
              return (DataType) new EnumType<sbyte>(entry);
            case 1:
              return (DataType) new EnumType<byte>(entry);
            case 2:
              return (DataType) new EnumType<ushort>(entry);
            case 3:
              return (DataType) new EnumType<uint>(entry);
            case 4:
              return (DataType) new EnumType<long>(entry);
            case 5:
              return (DataType) new EnumType<ulong>(entry);
            default:
              throw new AdsException("Enum base type mismatch!");
          }
        case 4:
          return (DataType) new ArrayType(entry);
        case 5:
          return (DataType) new StructType(entry);
        case 9:
          ISubRangeType subRange = (ISubRangeType) null;
          SubRangeTypeFactory.TryCreate(entry, (IDataTypeResolver) factoryServices.Binder, out subRange);
          return (DataType) subRange;
        case 10:
          int length = 0;
          Encoding encoding = (Encoding) null;
          DataTypeStringParser.TryParseString(entry.EntryName, out length, out encoding);
          return encoding == Encoding.Unicode ? (DataType) new WStringType(length) : (DataType) new StringType(length, encoding);
        case 13:
          if (entry.EntryName == "PVOID")
            return (DataType) new PVoidType(entry);
          if (entry.EntryName == "PCCH")
            return (DataType) new PCCHType(entry);
          string referencedType1 = (string) null;
          DataTypeStringParser.TryParsePointer(entry.EntryName, out referencedType1);
          return (DataType) new PointerType(entry, referencedType1);
        case 14:
          return (DataType) new UnionType(entry);
        case 15:
          string referencedType2 = (string) null;
          DataTypeStringParser.TryParseReference(entry.EntryName, out referencedType2);
          return (DataType) new ReferenceType(entry, referencedType2);
        case 16:
          return (DataType) new InterfaceType(entry);
        default:
          throw new NotSupportedException();
      }
    }
  }
}
