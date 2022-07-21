// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.EnumTypeConverter`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Generic type converter class for Enum values</summary>
  /// <typeparam name="T"></typeparam>
  internal class EnumTypeConverter<T> where T : struct, IConvertible
  {
    /// <summary>
    /// Converts the primitive value into the appropriate <see cref="T:TwinCAT.TypeSystem.EnumValue`1" />
    /// </summary>
    /// <param name="enumType">Enum base data type.</param>
    /// <param name="value">The value as primitive.</param>
    /// <returns>EnumValue&lt;T&gt;.</returns>
    private static EnumValue<T>? GetEntry(IEnumType enumType, T value)
    {
      EnumValue<T> entry = (EnumValue<T>) null;
      foreach (EnumValue<T> enumValue in (IEnumerable<IEnumValue>) enumType.EnumValues)
      {
        if (enumValue.Primitive.Equals((object) value))
        {
          entry = enumValue;
          break;
        }
      }
      return entry;
    }

    /// <summary>
    /// Gets the enum value object corresponding to the specified string value.
    /// </summary>
    /// <param name="enumType">Enum base type.</param>
    /// <param name="value">The value (in string representation).</param>
    /// <returns>EnumValue&lt;T&gt;.</returns>
    private static EnumValue<T>? GetEntry(IEnumType enumType, string value)
    {
      StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
      foreach (EnumValue<T> enumValue in (IEnumerable<IEnumValue>) enumType.EnumValues)
      {
        if (ordinalIgnoreCase.Compare(enumValue.Name, value) == 0)
          return enumValue;
      }
      return (EnumValue<T>) null;
    }

    /// <summary>
    /// Tries to get the string representation of the enum value.
    /// </summary>
    /// <param name="enumType">Base type of the enum.</param>
    /// <param name="value">The primitive value.</param>
    /// <param name="nameValue">String representation of the value.</param>
    /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// enumType
    /// or
    /// value
    /// </exception>
    /// <exception cref="T:System.ArgumentException">Specified type is not an enum type!;enumType</exception>
    internal static bool TryGetName(IEnumType enumType, T value, [NotNullWhen(true)] out string? nameValue)
    {
      if (enumType == null)
        throw new ArgumentNullException(nameof (enumType));
      EnumValue<T> enumValue = ((IDataType) enumType).Category == 3 ? EnumTypeConverter<T>.GetEntry(enumType, value) : throw new ArgumentException("Specified type is not an enum type!", nameof (enumType));
      if (enumValue != null)
      {
        nameValue = enumValue.Name;
        return true;
      }
      nameValue = (string) null;
      return false;
    }

    /// <summary>
    /// Tries to get the primitive value of the string represented value
    /// </summary>
    /// <param name="enumType">Base type of the enum.</param>
    /// <param name="stringValue">The string value.</param>
    /// <param name="value">The value as primitive.</param>
    /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">enumType</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">stringValue</exception>
    /// <exception cref="T:System.ArgumentException">Specified type is not an enum type!;enumType</exception>
    /// <remarks>The string value here is compared to the list of valid Enum Fields. No parsing is done to the base type.</remarks>
    internal static bool TryGetValue(IEnumType enumType, string stringValue, [NotNullWhen(true)] out T value)
    {
      if (enumType == null)
        throw new ArgumentNullException(nameof (enumType));
      if (string.IsNullOrEmpty(stringValue))
        throw new ArgumentOutOfRangeException(nameof (stringValue));
      EnumValue<T> enumValue = ((IDataType) enumType).Category == 3 ? EnumTypeConverter<T>.GetEntry(enumType, stringValue) : throw new ArgumentException("Specified type is not an enum type!", nameof (enumType));
      if (enumValue != null)
      {
        value = enumValue.Primitive;
        return true;
      }
      value = default (T);
      return false;
    }

    /// <summary>Tries to parse the EnumValue.</summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="stringValue">The string value.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <remarks>The string value is compared against the EnumFields and furthermore a BaseType parsing is done in addition.</remarks>
    /// <exception cref="T:System.ArgumentNullException">enumType</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">stringValue</exception>
    /// <exception cref="T:System.ArgumentException">Specified type is not an enum type! - enumType</exception>
    internal static bool TryParse(IEnumType enumType, string stringValue, out T value)
    {
      if (enumType == null)
        throw new ArgumentNullException(nameof (enumType));
      if (string.IsNullOrEmpty(stringValue))
        throw new ArgumentOutOfRangeException(nameof (stringValue));
      if (((IDataType) enumType).Category != 3)
        throw new ArgumentException("Specified type is not an enum type!", nameof (enumType));
      return EnumTypeConverter<T>.TryGetValue(enumType, stringValue, out value) || EnumTypeConverter<T>.TryChangeType(stringValue, out value);
    }

    /// <summary>Parses the string value</summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="stringValue">The string value.</param>
    /// <returns>T.</returns>
    /// <exception cref="T:System.FormatException">Value '{stringValue}' is not valid for type '{enumType.Name}'!</exception>
    internal static T Parse(IEnumType enumType, string stringValue)
    {
      T obj;
      if (!EnumTypeConverter<T>.TryParse(enumType, stringValue, out obj))
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
        interpolatedStringHandler.AppendLiteral("Value '");
        interpolatedStringHandler.AppendFormatted(stringValue);
        interpolatedStringHandler.AppendLiteral("' is not valid for type '");
        interpolatedStringHandler.AppendFormatted(((IDataType) enumType).Name);
        interpolatedStringHandler.AppendLiteral("'!");
        throw new FormatException(interpolatedStringHandler.ToStringAndClear());
      }
      return obj;
    }

    /// <summary>
    /// Tries to change the string value to the specified type.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="val">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private static bool TryChangeType(string value, out T val)
    {
      try
      {
        val = (T) Convert.ChangeType((object) value, typeof (T));
        return true;
      }
      catch (Exception ex)
      {
        val = default (T);
        return false;
      }
    }

    /// <summary>
    /// Converts the string represented value to its primitive value.
    /// </summary>
    /// <param name="enumType">Base type of the enum.</param>
    /// <param name="stringValue">The string value.</param>
    /// <returns>The value as primitive.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// enumType
    /// or
    /// value
    /// </exception>
    /// <exception cref="T:System.ArgumentException">Specified type is not an enum type!;enumType</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">stringValue</exception>
    internal static T ToValue(IEnumType enumType, string stringValue)
    {
      if (enumType == null)
        throw new ArgumentNullException(nameof (enumType));
      if (stringValue == null)
        throw new ArgumentNullException(nameof (stringValue));
      if (((IDataType) enumType).Category == 3)
        throw new ArgumentException("Specified type is not an enum type!", nameof (enumType));
      T obj = default (T);
      if (!EnumTypeConverter<T>.TryGetValue(enumType, stringValue, out obj))
        throw new ArgumentOutOfRangeException(nameof (stringValue));
      return obj;
    }

    /// <summary>
    /// Converts the primitive value of the specified enum type into its string representation.
    /// </summary>
    /// <param name="enumType">Base type of the enum.</param>
    /// <param name="val">The value as primitive.</param>
    /// <returns>A <see cref="T:System.String" /> that represents the value.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">val</exception>
    internal static string ToString(IEnumType enumType, T val)
    {
      string nameValue = (string) null;
      if (!EnumTypeConverter<T>.TryGetName(enumType, val, out nameValue))
        throw new ArgumentOutOfRangeException(nameof (val));
      return nameValue;
    }
  }
}
