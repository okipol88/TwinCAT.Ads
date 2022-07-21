// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.AnyTypeMarshaler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using TwinCAT.Ads;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>AnyTypeMarshaler</summary>
  /// <remarks>
  /// Because this Converter/Marshaller solely works with .NET Types, it is only able to marshal PrimitiveTypes and ARRAY OF PrimitiveTypes.
  /// A little additional metadata is the args parameter that is used for Arrays and Strings to define the length respectively the Array size.
  /// This marshaler is used for the cases, where the .NET DataType for marshalling is given (ANY_TYPE marshalling, ANY_TYPE Notifications).
  /// </remarks>
  /// <exclude />
  /// <seealso cref="T:TwinCAT.TypeSystem.AnyTypeMarshaler" />
  /// <seealso cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AnyTypeMarshaler : IAnyTypeMarshaler, IGenericTypeMarshaler, ITypeMarshaler
  {
    /// <summary>Marshals the specified any value.</summary>
    /// <param name="anyValue">The value.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="destination">The destination memory.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentException">String arrays without Length specification are not are not supported! - args
    /// or
    /// Value string is too large! - anyValue
    /// or
    /// Too many values in string array! - anyValue
    /// or
    /// String at Index '{0}' is too long! - anyValue</exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    public int Marshal(object anyValue, int[]? args, Encoding encoding, Span<byte> destination)
    {
      if (anyValue == null)
        throw new ArgumentNullException(nameof (anyValue));
      PrimitiveTypeMarshaler primitiveTypeMarshaler = new PrimitiveTypeMarshaler(encoding);
      int start = 0;
      anyValue.GetType();
      if (args == null && anyValue.GetType().IsArray)
      {
        anyValue.GetType().GetArrayRank();
        Array array = (Array) anyValue;
        if (array.GetType().GetElementType() == typeof (string))
          throw new ArgumentException("String arrays without Length specification are not supported!", nameof (args));
        int[] lowerBounds = (int[]) null;
        int[] lengths = (int[]) null;
        PrimitiveTypeMarshaler.GetArrayBounds(array, out lowerBounds, out lengths);
        args = lengths;
      }
      int num1 = this.MarshalSize(anyValue, args, encoding);
      if (num1 > destination.Length)
        ((AdsErrorCode) 1797).ThrowOnError();
      if (args == null)
      {
        if (primitiveTypeMarshaler.CanMarshal(anyValue))
          start = primitiveTypeMarshaler.Marshal(anyValue, encoding, destination);
      }
      else if (anyValue.GetType() == typeof (string))
      {
        int num2 = args[0];
        string str = (string) anyValue;
        if (num2 <= 0 || str.Length > args[0])
          throw new ArgumentException("Value string is too large!", nameof (anyValue));
        StringMarshaler stringMarshaler = StringMarshaler.Default;
        start += stringMarshaler.Marshal(str, num1, destination);
      }
      else if (anyValue.GetType() == typeof (string[]))
      {
        int num3 = args[0];
        int num4 = args[1];
        string[] strArray = (string[]) anyValue;
        if (strArray.Length > num4)
          throw new ArgumentException("Too many values in string array!", nameof (anyValue));
        int length = num3 + 1;
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (strArray[index].Length > num3)
            throw new ArgumentException("String at Index '{0}' is too long!", nameof (anyValue));
          primitiveTypeMarshaler.Marshal((object) strArray[index], encoding, destination.Slice(start, length));
          start += length;
        }
      }
      else
      {
        Array array = anyValue.GetType().IsArray ? (Array) anyValue : throw new NotSupportedException();
        Type elementType = array.GetType().GetElementType();
        int[] lowerBounds1 = (int[]) null;
        int[] lengths = (int[]) null;
        PrimitiveTypeMarshaler.GetArrayBounds(array, out lowerBounds1, out lengths);
        int num5 = ArrayIndexConverter.ArraySubElementCount(lengths);
        int length = this.MarshalSize((object) elementType, encoding);
        int num6 = length * num5;
        int[] lowerBounds2 = (int[]) null;
        int[] upperBounds = (int[]) null;
        ArrayIndexConverter.GetBounds(lengths, out lowerBounds2, out upperBounds);
        foreach (int[] numArray in new ArrayIndexIterator(lowerBounds1, upperBounds))
        {
          object anyValue1 = array.GetValue(numArray);
          start += this.Marshal(anyValue1, (int[]) null, encoding, destination.Slice(start, length));
        }
      }
      return start;
    }

    /// <summary>
    /// Unmarshals the specified type from memory to value object.
    /// </summary>
    /// <param name="anyType">Any type.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="source">The source memory.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentException">type
    /// or
    /// anyType</exception>
    public int Unmarshal(
      Type anyType,
      int[]? args,
      ReadOnlySpan<byte> source,
      Encoding encoding,
      out object value)
    {
      if (anyType == (Type) null)
        throw new ArgumentNullException(nameof (anyType));
      int num = this.MarshalSize(anyType, args, encoding);
      int start = 0;
      if (anyType != typeof (string) && num != source.Length)
        throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Marshalling size is '{0}' bytes, but read data is '{1}' bytes. Cannot marshal type '{2}'!", (object) num, (object) source.Length, (object) anyType));
      PrimitiveTypeMarshaler primitiveTypeMarshaler = new PrimitiveTypeMarshaler(encoding);
      if (source.Length <= 0)
        throw new ArgumentOutOfRangeException(nameof (source));
      if (anyType.IsArray && args != null)
      {
        Type type = anyType;
        anyType.GetElementType();
        if (type == typeof (string[]))
        {
          int strLen = args[0];
          int length1 = args[1];
          string[] strArray = new string[length1];
          int length2 = primitiveTypeMarshaler.MarshalSize(encoding, strLen);
          for (int index = 0; index < length1; ++index)
            start += primitiveTypeMarshaler.Unmarshal(source.Slice(start, length2), out strArray[index]);
          value = (object) strArray;
        }
        else
        {
          DimensionCollection dimensionCollection = new DimensionCollection(args);
          IList<IDimensionCollection> idimensionCollectionList = (IList<IDimensionCollection>) new List<IDimensionCollection>();
          ((ICollection<IDimensionCollection>) idimensionCollectionList).Add((IDimensionCollection) dimensionCollection);
          start = this.UnmarshalArray(new AnyTypeSpecifier(type, idimensionCollectionList), encoding, source, out value);
        }
      }
      else if (anyType == typeof (string))
      {
        if (args == null || args.Length != 1)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid additional arguments for type '{0}'!", (object) anyType), nameof (args));
        string str;
        start = primitiveTypeMarshaler.Unmarshal(source, args[0] + 1, encoding, out str);
        value = (object) str;
      }
      else
      {
        if (!primitiveTypeMarshaler.CanMarshal(anyType))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "AnyType '{0}' not supported!", (object) anyType), nameof (anyType));
        if (args == null)
        {
          start = primitiveTypeMarshaler.Unmarshal(anyType, source, encoding, out value);
        }
        else
        {
          List<IDimensionCollection> idimensionCollectionList = new List<IDimensionCollection>();
          DimensionCollection dimensionCollection = new DimensionCollection(args);
          idimensionCollectionList.Add((IDimensionCollection) dimensionCollection);
          AnyTypeSpecifier typeSpec = new AnyTypeSpecifier(anyType, (IList<IDimensionCollection>) idimensionCollectionList);
          start = primitiveTypeMarshaler.UnmarshalArray(typeSpec, encoding, source, out value);
        }
      }
      return start;
    }

    /// <summary>Gets the MarshalSize of the specified type.</summary>
    /// <param name="anyType">The 'Any' type to marshal</param>
    /// <param name="args">The 'Any' type parameters</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentException">type
    /// or
    /// type</exception>
    public int MarshalSize(Type anyType, int[]? args, Encoding encoding)
    {
      if (anyType == (Type) null)
        throw new ArgumentNullException(nameof (anyType));
      int num = 0;
      PrimitiveTypeMarshaler primitiveTypeMarshaler = new PrimitiveTypeMarshaler(encoding);
      if (args == null)
        num = primitiveTypeMarshaler.MarshalSize(anyType);
      else if (anyType == typeof (string))
      {
        if (args.Length == 1 && args[0] >= 0)
          num = args[0] + 1;
      }
      else if (anyType == typeof (string[]))
      {
        if (args.Length == 2)
          num = (args[0] + 1) * args[1];
      }
      else if (anyType.IsArray)
      {
        int elementCount = new DimensionCollection(args).ElementCount;
        num = this.MarshalSize((object) anyType.GetElementType(), encoding) * elementCount;
      }
      return num;
    }

    /// <summary>Gets the marshalling size of the value in bytes.f</summary>
    /// <param name="anyValue">Any value.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>
    /// <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public int MarshalSize(object anyValue, int[]? args, Encoding encoding)
    {
      if (anyValue == null)
        throw new ArgumentNullException(nameof (anyValue));
      int num1 = 0;
      PrimitiveTypeMarshaler primitiveTypeMarshaler = new PrimitiveTypeMarshaler(encoding);
      if (args == null)
        num1 = primitiveTypeMarshaler.MarshalSize(anyValue);
      else if (anyValue.GetType() == typeof (string))
      {
        StringMarshaler stringMarshaler = StringMarshaler.Default;
        int num2 = args[0];
        string str = (string) anyValue;
        if (num2 <= 0 || str.Length > args[0])
          throw new ArgumentException("Value string is too large!", nameof (anyValue));
        num1 = stringMarshaler.MarshalSize(str);
      }
      else if (anyValue.GetType() == typeof (string[]))
      {
        int num3 = args[0];
        int num4 = args[1];
        if (((string[]) anyValue).Length > num4)
          throw new ArgumentException("Too many values in string array!", nameof (anyValue));
        num1 = (num3 + 1) * num4;
      }
      else if (anyValue.GetType().IsArray)
      {
        int elementCount = new DimensionCollection(args).ElementCount;
        Array array = (Array) anyValue;
        Type elementType = array.GetType().GetElementType();
        int[] lowerBounds = (int[]) null;
        int[] lengths = (int[]) null;
        PrimitiveTypeMarshaler.GetArrayBounds(array, out lowerBounds, out lengths);
        int num5 = ArrayIndexConverter.ArraySubElementCount(lengths);
        if (elementCount != num5)
          throw new MarshalException();
        num1 = this.MarshalSize(elementType, (int[]) null, encoding) * num5;
      }
      return num1;
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified value type.
    /// </summary>
    /// <param name="valueType">Type of the value.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// <c>true</c> if this instance can marshal the specified value type; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(Type valueType, int[]? args)
    {
      if (valueType == (Type) null)
        throw new ArgumentNullException(nameof (valueType));
      bool flag = false;
      if (args == null)
        flag = PrimitiveTypeMarshaler.Default.CanMarshal(valueType);
      else if (valueType == typeof (string))
      {
        if (args.Length == 1 && args[0] >= 0)
          flag = true;
      }
      else if (valueType == typeof (string[]))
      {
        if (args.Length == 2)
        {
          int num1 = args[0];
          int num2 = args[1];
          flag = num1 >= 0 && num2 >= 0;
        }
      }
      else if (valueType.IsArray)
      {
        int elementCount = new DimensionCollection(args).ElementCount;
        flag = this.CanMarshal(valueType.GetElementType(), (int[]) null);
      }
      return flag;
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified any value.
    /// </summary>
    /// <param name="anyValue">Any value.</param>
    /// <param name="args">The arguments.</param>
    /// <returns><c>true</c> if this instance can marshal the specified any value; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(object anyValue, int[]? args)
    {
      if (anyValue == null)
        throw new ArgumentNullException(nameof (anyValue));
      bool flag = false;
      if (args == null)
        flag = PrimitiveTypeMarshaler.Default.CanMarshal(anyValue);
      else if (anyValue.GetType() == typeof (string))
      {
        if (args.Length == 1 && args[0] >= ((string) anyValue).Length)
          flag = true;
      }
      else if (anyValue.GetType() == typeof (string[]))
      {
        if (args.Length == 2)
        {
          string[] source = (string[]) anyValue;
          int characters = args[0];
          int num = args[1];
          flag = source.Length == num && ((IEnumerable<string>) source).All<string>((Func<string, bool>) (e => e.Length <= characters));
        }
      }
      else if (anyValue.GetType().IsArray)
      {
        int elementCount = new DimensionCollection(args).ElementCount;
        Array array = (Array) anyValue;
        if (this.CanMarshal(array.GetType().GetElementType(), (int[]) null))
        {
          int[] lowerBounds = (int[]) null;
          int[] lengths = (int[]) null;
          PrimitiveTypeMarshaler.GetArrayBounds(array, out lowerBounds, out lengths);
          int num = ArrayIndexConverter.ArraySubElementCount(lengths);
          flag = elementCount == num;
        }
      }
      return flag;
    }

    private int UnmarshalArray(
      AnyTypeSpecifier spec,
      Encoding encoding,
      ReadOnlySpan<byte> data,
      out object val)
    {
      if (spec == null)
        throw new ArgumentNullException(nameof (spec));
      if (spec.ElementType == null)
        throw new ArgumentOutOfRangeException(nameof (spec));
      PrimitiveTypeMarshaler primitiveTypeMarshaler = new PrimitiveTypeMarshaler(encoding);
      if (primitiveTypeMarshaler.CanMarshal(spec.ElementType.Type))
        return primitiveTypeMarshaler.UnmarshalArray(spec, encoding, data, out val);
      if (!spec.Type.IsArray)
        throw new MarshalException(spec);
      int start = 0;
      ArrayIndexIterator arrayIndexIterator = new ArrayIndexIterator(spec.DimLengths[0].LowerBounds, spec.DimLengths[0].UpperBounds, true);
      Array instance = Array.CreateInstance(spec.ElementType.Type, spec.DimLengths[0].GetDimensionLengths());
      foreach (int[] numArray in arrayIndexIterator)
      {
        object obj;
        start += this.Unmarshal(spec.ElementType.Type, (int[]) null, data.Slice(start, data.Length - start), encoding, out obj);
        instance.SetValue(obj, numArray);
      }
      val = (object) instance;
      return start;
    }

    /// <summary>
    /// Unmarshals the specified source span/memory and creates the value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The source span/memory.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value.</param>
    /// <returns>The number of unmarshalled bytes.</returns>
    public int Unmarshal<T>(ReadOnlySpan<byte> source, Encoding encoding, out T val)
    {
      object obj;
      int num = this.Unmarshal(typeof (T), (int[]) null, source, encoding, out obj);
      val = (T) obj;
      return num;
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if this instance can marshal the specified type; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(Type type) => this.CanMarshal(type, (int[]) null);

    /// <summary>
    /// Determines whether this instance can marshal the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if this instance can marshal the specified value; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(object value) => this.CanMarshal(value, (int[]) null);

    /// <summary>Marshals the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="data">The data.</param>
    /// <returns>System.Int32.</returns>
    public int Marshal(object value, Encoding encoding, Span<byte> data) => this.Marshal(value, (int[]) null, encoding, data);

    /// <summary>Gets the MarshalSize of the value.</summary>
    /// <param name="value">The value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>The Number of marshalling bytes.</returns>
    public int MarshalSize(object value, Encoding encoding) => this.MarshalSize(value, (int[]) null, encoding);

    /// <summary>Unmarshals the specified type.</summary>
    /// <param name="type">The type.</param>
    /// <param name="data">The data.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value.</param>
    /// <returns>The number of unmarshalled bytes.</returns>
    public int Unmarshal(Type type, ReadOnlySpan<byte> data, Encoding encoding, out object val) => this.Unmarshal(type, (int[]) null, data, encoding, out val);
  }
}
