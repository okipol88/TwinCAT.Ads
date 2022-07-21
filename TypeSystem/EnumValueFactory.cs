// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.EnumValueFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.Ads;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Factory class for Enum Values.</summary>
  internal static class EnumValueFactory
  {
    /// <summary>
    /// Creates the specified <see cref="T:TwinCAT.TypeSystem.IEnumValue" />
    /// </summary>
    /// <param name="baseTypeId">Id of the enum base type.</param>
    /// <param name="enumInfo">The entry.</param>
    /// <returns>IEnumGenericValue.</returns>
    /// <exception cref="T:System.ArgumentNullException">entry</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">baseTypeId</exception>
    internal static IEnumValue Create(AdsDataTypeId baseTypeId, AdsEnumInfoEntry enumInfo)
    {
      if (enumInfo == null)
        throw new ArgumentNullException(nameof (enumInfo));
      if (baseTypeId == 2)
        return (IEnumValue) new EnumValue<short>(enumInfo);
      if (baseTypeId == 3)
        return (IEnumValue) new EnumValue<int>(enumInfo);
      switch (baseTypeId - 16)
      {
        case 0:
          return (IEnumValue) new EnumValue<sbyte>(enumInfo);
        case 1:
          return (IEnumValue) new EnumValue<byte>(enumInfo);
        case 2:
          return (IEnumValue) new EnumValue<ushort>(enumInfo);
        case 3:
          return (IEnumValue) new EnumValue<uint>(enumInfo);
        case 4:
          return (IEnumValue) new EnumValue<long>(enumInfo);
        case 5:
          return (IEnumValue) new EnumValue<ulong>(enumInfo);
        default:
          throw new ArgumentOutOfRangeException(nameof (baseTypeId));
      }
    }

    private static IEnumValue Create<X>(IEnumType enumType, object value) where X : struct, IConvertible
    {
      X x = !(value is string) ? (X) Convert.ChangeType(value, typeof (X)) : EnumTypeConverter<X>.Parse(enumType, (string) value);
      return (IEnumValue) new EnumValue<X>((IEnumType<X>) enumType, x);
    }

    /// <summary>Creates the specified enum type.</summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="value">The value.</param>
    /// <returns>IEnumValue.</returns>
    /// <exception cref="T:System.NotSupportedException"></exception>
    internal static IEnumValue Create(IEnumType enumType, object value)
    {
      Type managedType = ((IManagedMappableType) enumType).ManagedType;
      if (managedType == typeof (byte))
        return EnumValueFactory.Create<byte>(enumType, value);
      if (managedType == typeof (sbyte))
        return EnumValueFactory.Create<sbyte>(enumType, value);
      if (managedType == typeof (short))
        return EnumValueFactory.Create<short>(enumType, value);
      if (managedType == typeof (ushort))
        return EnumValueFactory.Create<ushort>(enumType, value);
      if (managedType == typeof (int))
        return EnumValueFactory.Create<int>(enumType, value);
      if (managedType == typeof (uint))
        return EnumValueFactory.Create<uint>(enumType, value);
      if (managedType == typeof (long))
        return EnumValueFactory.Create<long>(enumType, value);
      if (managedType == typeof (ulong))
        return EnumValueFactory.Create<ulong>(enumType, value);
      throw new NotSupportedException();
    }

    /// <summary>Creates the specified enum type.</summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="source">The bytes.</param>
    /// <returns>IEnumValue.</returns>
    /// <exception cref="T:System.ArgumentException">Wrong Enum base type.</exception>
    internal static IEnumValue Create(IEnumType enumType, ReadOnlySpan<byte> source)
    {
      Type managedType = ((IManagedMappableType) enumType).ManagedType;
      int num = 0;
      byte[] array = source.ToArray();
      object obj;
      if (managedType == typeof (byte))
        obj = (object) source[num];
      else if (managedType == typeof (sbyte))
        obj = (object) (sbyte) source[num];
      else if (managedType == typeof (short))
        obj = (object) BitConverter.ToInt16(array, num);
      else if (managedType == typeof (ushort))
        obj = (object) BitConverter.ToUInt16(array, num);
      else if (managedType == typeof (int))
        obj = (object) BitConverter.ToInt32(array, num);
      else if (managedType == typeof (uint))
        obj = (object) BitConverter.ToUInt32(array, num);
      else if (managedType == typeof (long))
      {
        obj = (object) BitConverter.ToInt64(array, num);
      }
      else
      {
        if (!(managedType == typeof (ulong)))
          throw new ArgumentException("Wrong Enum base type.");
        obj = (object) BitConverter.ToUInt64(array, num);
      }
      return EnumValueFactory.Create(enumType, obj);
    }
  }
}
