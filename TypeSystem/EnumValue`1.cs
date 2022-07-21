// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.EnumValue`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using TwinCAT.Ads.Internal;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Enum Value</summary>
  /// <typeparam name="T">Enum base type (byte,sbyte,short,ushort,int,uint,long or ulong)</typeparam>
  public class EnumValue<T> : IEnumValue where T : IConvertible
  {
    /// <summary>Name of the Enum Value (as string)</summary>
    private string _name = string.Empty;
    /// <summary>The Value of the Enum</summary>
    private T _value;
    private int _size;

    /// <summary>Gets the name of the Enum Value</summary>
    /// <value>The name.</value>
    public string Name => this._name;

    /// <summary>Gets the value.</summary>
    /// <value>The value.</value>
    public T Primitive => this._value;

    /// <summary>
    /// Converts to <see cref="T:TwinCAT.Ads.Internal.AdsEnumInfoEntry" />
    /// </summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <returns>AdsEnumInfoEntry.</returns>
    internal AdsEnumInfoEntry ToAdsEnumEntry(IStringMarshaler marshaler) => new AdsEnumInfoEntry(this._name, this.RawValue, marshaler);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.EnumValue`1" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exception cref="T:System.ArgumentException">Wrong Enum base type.</exception>
    internal EnumValue(AdsEnumInfoEntry entry)
    {
      this._name = entry.Name;
      this._size = entry.Value.Length;
      Type type = typeof (T);
      if (type == typeof (byte))
        this._value = (T) Convert.ChangeType((object) entry.Value[0], typeof (byte), (IFormatProvider) CultureInfo.InvariantCulture);
      else if (type == typeof (sbyte))
        this._value = (T) Convert.ChangeType((object) (sbyte) entry.Value[0], typeof (sbyte), (IFormatProvider) CultureInfo.InvariantCulture);
      else if (type == typeof (short))
        this._value = (T) Convert.ChangeType((object) BitConverter.ToInt16(entry.Value, 0), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      else if (type == typeof (ushort))
        this._value = (T) Convert.ChangeType((object) BitConverter.ToUInt16(entry.Value, 0), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      else if (type == typeof (int))
        this._value = (T) Convert.ChangeType((object) BitConverter.ToInt32(entry.Value, 0), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      else if (type == typeof (uint))
        this._value = (T) Convert.ChangeType((object) BitConverter.ToUInt32(entry.Value, 0), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      else if (type == typeof (long))
      {
        this._value = (T) Convert.ChangeType((object) BitConverter.ToInt64(entry.Value, 0), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      else
      {
        if (!(type == typeof (ulong)))
          throw new ArgumentException("Wrong Enum base type.");
        this._value = (T) Convert.ChangeType((object) BitConverter.ToUInt64(entry.Value, 0), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    internal EnumValue(IEnumType<T> enumType, T value)
    {
      this._size = ((IBitSize) enumType).Size;
      this._value = value;
      this._name = enumType.ToString(this._value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.EnumValue`1" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="valueSize">Value size in bytes.</param>
    public EnumValue(string name, T value, int valueSize)
    {
      this._size = valueSize;
      this._value = value;
      this._name = name;
    }

    /// <summary>
    /// Gets the Value of the enumeration value (value as object)
    /// </summary>
    /// <value>The object value.</value>
    object IEnumValue.Primitive => (object) this.Primitive;

    /// <summary>Gets the raw value of the enumeration (as byte array)</summary>
    /// <value>The raw value.</value>
    /// <exception cref="T:System.NotSupportedException">Base type of enum is not allowed!</exception>
    public byte[] RawValue
    {
      get
      {
        Type type = typeof (T);
        byte[] rawValue;
        if (type == typeof (byte))
          rawValue = new byte[1]
          {
            this.Primitive.ToByte((IFormatProvider) null)
          };
        else if (type == typeof (sbyte))
          rawValue = BitConverter.GetBytes((short) this.Primitive.ToSByte((IFormatProvider) null));
        else if (type == typeof (short))
          rawValue = BitConverter.GetBytes(this.Primitive.ToInt16((IFormatProvider) null));
        else if (type == typeof (ushort))
          rawValue = BitConverter.GetBytes(this.Primitive.ToUInt16((IFormatProvider) null));
        else if (type == typeof (int))
          rawValue = BitConverter.GetBytes(this.Primitive.ToInt32((IFormatProvider) null));
        else if (type == typeof (uint))
          rawValue = BitConverter.GetBytes(this.Primitive.ToUInt32((IFormatProvider) null));
        else if (type == typeof (long))
        {
          rawValue = BitConverter.GetBytes(this.Primitive.ToInt64((IFormatProvider) null));
        }
        else
        {
          if (!(type == typeof (ulong)))
            throw new NotSupportedException("Base type of enum is not allowed!");
          rawValue = BitConverter.GetBytes(this.Primitive.ToUInt64((IFormatProvider) null));
        }
        return rawValue;
      }
    }

    /// <summary>
    /// Gets the enumeration base type (sint,byte,short,ushort,int,uint,Int64,UInt64 supported)
    /// </summary>
    /// <value>The type of the base.</value>
    public Type ManagedBaseType => typeof (T);

    /// <summary>Gets the size of the Enum value (in bytes)</summary>
    /// <value>The size.</value>
    public int Size => this._size;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
    public override string? ToString() => this.Name;

    public static bool TryParse(IEnumType<T> type, string str, [NotNullWhen(true)] out EnumValue<T>? value)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      IEnumValue ienumValue;
      if (((IEnumValueCollection<IEnumValue, IConvertible>) type.EnumValues).TryParse(str, ref ienumValue))
      {
        value = (EnumValue<T>) ienumValue;
        return true;
      }
      value = (EnumValue<T>) null;
      return false;
    }

    public static EnumValue<T> Parse(IEnumType<T> type, string str)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (string.IsNullOrEmpty(str))
        throw new ArgumentOutOfRangeException(nameof (str));
      EnumValue<T> enumValue = (EnumValue<T>) null;
      if (!EnumValue<T>.TryParse(type, str, out enumValue))
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot parse '{0}' to EnumValue of type '{1}'!", (object) str, (object) ((IDataType) type).Name));
      return enumValue;
    }
  }
}
