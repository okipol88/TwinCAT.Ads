// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.SymbolicAnyTypeMarshaler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class SymbolicAnyTypeMarshaler.</summary>
  /// <remarks>This is the most sophisticated marshaler. It is able to marshal when symbolic information is available and can use PInvoke and Reflection.</remarks>
  /// <exclude />
  /// <seealso cref="T:TwinCAT.TypeSystem.AnyTypeMarshaler" />
  /// <seealso cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" />
  public class SymbolicAnyTypeMarshaler : IDataTypeMarshaler, ISymbolMarshaler
  {
    private PrimitiveTypeMarshaler _marshaler = PrimitiveTypeMarshaler.Default;
    /// <summary>Cache of already checked 'Any' proxy types.</summary>
    private Dictionary<Type, List<IDataType>> _checkedTypesDict = new Dictionary<Type, List<IDataType>>();

    /// <summary>
    /// Gets the default value encoding like specified by the used ValueAccessor.
    /// </summary>
    /// <value>The default value encoding.</value>
    public Encoding DefaultValueEncoding => this._marshaler.DefaultValueEncoding;

    /// <summary>
    /// Determines whether this instance can marshal the specified data type.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <returns><c>true</c> if this instance can marshal the specified data type; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">dataType</exception>
    /// <exception cref="T:System.ArgumentException">Marshalling mismatch! - valueType</exception>
    public bool CanMarshal(IDataType dataType, Type? valueType)
    {
      if (dataType == null)
        throw new ArgumentNullException(nameof (dataType));
      bool flag = false;
      dataType = dataType.Resolve();
      Type managed = (Type) null;
      if (this.TryGetManagedType(dataType, out managed))
      {
        if (valueType != (Type) null && managed != valueType)
          throw new ArgumentException("Marshalling mismatch!", nameof (valueType));
        flag = true;
      }
      else if (valueType != (Type) null)
        flag = this._marshaler.CanMarshal(valueType);
      if (!flag && valueType != (Type) null && dataType.Category == 5)
      {
        Exception error = (Exception) null;
        flag = this.CanMarshalByReflection(dataType, valueType, out error);
      }
      return flag;
    }

    /// <summary>
    /// Tries to get the managed representation of the <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="managed">The managed.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetManagedType(IDataType type, [NotNullWhen(true)] out Type? managed) => type.TryGetManagedType(out managed);

    /// <summary>
    /// Get the marshalling size of the <see cref="T:TwinCAT.TypeSystem.IDataType" />/value in bytes.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <returns>System.Int32.</returns>
    public int MarshalSize(IDataType dataType) => dataType != null ? ((IBitSize) dataType).ByteSize : throw new ArgumentNullException(nameof (dataType));

    /// <summary>
    /// Unmarshals the data from memory/span to a managed value.
    /// </summary>
    /// <param name="dataType">The type information of the data.</param>
    /// <param name="encoding">The used string encoding.</param>
    /// <param name="valueType">The managed type to create the value from.</param>
    /// <param name="data">The source memory / span.</param>
    /// <param name="value">The value.</param>
    /// <returns>The number of unmarshaled bytes.</returns>
    /// <exception cref="T:System.NotSupportedException">Type of symbol not supported</exception>
    public int Unmarshal(
      IDataType dataType,
      Encoding encoding,
      ReadOnlySpan<byte> data,
      Type? valueType,
      out object value)
    {
      if (dataType == null)
        throw new ArgumentNullException(nameof (dataType));
      int start = 0;
      IDataType idataType = dataType.Resolve();
      if (valueType == (Type) null)
        valueType = idataType.GetManagedType();
      int num;
      if (idataType.Category == 4)
      {
        if (valueType == (Type) null)
          throw new MarshalException(idataType);
        num = this.UnmarshalArray((IArrayType) idataType, encoding, valueType, data, -1, out value);
      }
      else if (idataType.Category == 5)
        num = this.UnmarshalStruct((IStructType) idataType, encoding, valueType, data, out value);
      else if (idataType.Category == 10)
      {
        IStringType istringType = (IStringType) idataType;
        string str;
        num = (encoding == StringMarshaler.DefaultEncoding || encoding == StringMarshaler.UnicodeEncoding ? new StringMarshaler(istringType.Encoding, (StringConvertMode) 1) : new StringMarshaler(encoding, (StringConvertMode) 1)).Unmarshal(data.Slice(start, ((IBitSize) idataType).ByteSize), ref str);
        value = (object) str;
      }
      else
      {
        if (!idataType.IsPrimitive)
          throw new MarshalException("DataType '" + idataType.Name + "' is not supported. Cannot marshal value!");
        num = this._marshaler.Unmarshal(idataType, data, (Type) null, out value);
        if (valueType != (Type) null)
          value = PrimitiveTypeMarshaler.Convert(value, valueType);
      }
      return num;
    }

    /// <summary>
    /// Creates a managed array and initializes it with information from ADS Read
    /// </summary>
    /// <param name="arrType">The array type (symbolic information)</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="managedType">The managed array type.</param>
    /// <param name="data">The source data / span.</param>
    /// <param name="jagLevel">The jag level (only used for recursive jagArray calls)</param>
    /// <param name="value">The unmarshaled value.</param>
    /// <returns>Read bytes</returns>
    /// <exception cref="T:System.ArgumentNullException">arrType
    /// or
    /// managedType</exception>
    /// <exception cref="T:System.ArgumentException">type
    /// or
    /// type
    /// or
    /// type
    /// or
    /// Cannot convert data type of symbol to this type. - type</exception>
    /// <exception cref="T:System.ArgumentException">type
    /// or
    /// type</exception>
    internal int UnmarshalArray(
      IArrayType arrType,
      Encoding encoding,
      Type managedType,
      ReadOnlySpan<byte> data,
      int jagLevel,
      out object value)
    {
      if (arrType == null)
        throw new ArgumentNullException(nameof (arrType));
      if (managedType == (Type) null)
        throw new ArgumentNullException(nameof (managedType));
      int start = 0;
      Type type = managedType.IsArray ? managedType.GetElementType() : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert datatype of symbol to this type. Expected an array, actual type is {0}", (object) managedType.ToString()), nameof (managedType));
      AdsDataTypeArrayInfo[] dataTypeArrayInfo = DimensionConverter.ToAdsDataTypeArrayInfo((IList<IDimension>) arrType.Dimensions);
      Type baseElementType = (Type) null;
      int jagLevel1;
      bool flag = this._marshaler.TryJaggedArray(managedType, out jagLevel1, out baseElementType);
      int elementCount = arrType.Dimensions.ElementCount;
      IDataType elementType1 = arrType.ElementType;
      if (elementType1 == null)
        throw new CannotResolveDataTypeException(((IDataType) arrType).Name);
      if (flag)
      {
        if (((ICollection<IDimension>) arrType.Dimensions).Count != 1)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert datatype of symbol to this type. Expected an array rank of {0}, actual rank is {1}", (object) ((ICollection<IDimension>) arrType.Dimensions).Count, (object) jagLevel1), nameof (arrType));
      }
      else
      {
        int count = ((ICollection<IDimension>) arrType.Dimensions).Count;
        if (((ICollection<IDimension>) arrType.Dimensions).Count != managedType.GetArrayRank())
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert datatype of symbol to this type. Expected an array rank of {0}, actual rank is {1}", (object) ((ICollection<IDimension>) arrType.Dimensions).Count, (object) managedType.GetArrayRank()), nameof (arrType));
      }
      AdsArrayDimensionsInfo arrayDimensionsInfo = new AdsArrayDimensionsInfo(dataTypeArrayInfo);
      Array instance;
      if (flag)
      {
        ++jagLevel;
        instance = Array.CreateInstance(type, arrayDimensionsInfo.DimensionElements[jagLevel]);
        foreach (int[] numArray in new ArrayIndexIterator(new int[1]
        {
          arrayDimensionsInfo.LowerBounds[jagLevel]
        }, new int[1]
        {
          arrayDimensionsInfo.UpperBounds[jagLevel]
        }, true))
        {
          object obj;
          start += this.UnmarshalArray((IArrayType) elementType1, encoding, type, data.Slice(start), jagLevel, out obj);
          instance.SetValue(obj, numArray);
        }
      }
      else
      {
        DataTypeCategory category = elementType1.Category;
        instance = Array.CreateInstance(type, arrayDimensionsInfo.DimensionElements);
        object obj;
        foreach (int[] numArray in new ArrayIndexIterator(arrayDimensionsInfo.LowerBounds, arrayDimensionsInfo.UpperBounds, true))
        {
          if (elementType1.IsPrimitive)
          {
            IManagedMappableType elementType2 = arrType.ElementType as IManagedMappableType;
            if (type != elementType2?.ManagedType)
              throw new ArgumentException("Cannot convert data type of symbol to this type.", nameof (managedType));
            start += this.Unmarshal(elementType1, encoding, data.Slice(start, ((IBitSize) elementType1).ByteSize), type, out obj);
            instance.SetValue(obj, numArray);
          }
          else if (elementType1.Category == 5)
          {
            start += this.UnmarshalStruct((IStructType) elementType1, encoding, type, data.Slice(start), out obj);
            instance.SetValue(obj, numArray);
          }
          else if (elementType1.Category == 4)
          {
            start += this.UnmarshalArray((IArrayType) elementType1, encoding, type, data.Slice(start), jagLevel, out obj);
            instance.SetValue(obj, numArray);
          }
        }
      }
      value = (object) instance;
      return start;
    }

    /// <summary>Marshals an array value to memory / span.</summary>
    /// <param name="arrayType">The ADS type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">Managed Array value.</param>
    /// <param name="data">The data.</param>
    /// <returns>The number of marshaled bytes.</returns>
    /// <exception cref="T:System.ArgumentException">type</exception>
    /// <exception cref="T:System.ArgumentException">type
    /// or
    /// Cannot convert ads array type of symbol to this type.;type</exception>
    internal int MarshalArray(
      IArrayType arrayType,
      Encoding encoding,
      object value,
      Span<byte> data)
    {
      IDataType dataType = arrayType != null ? arrayType.ElementType : throw new ArgumentNullException(nameof (arrayType));
      if (dataType == null)
        throw new CannotResolveDataTypeException(((IDataType) arrayType).Name);
      int start = 0;
      if (!(value is Array array))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this value. Expected an array, actual type is {0}", (object) value.GetType()), nameof (value));
      AdsDataTypeArrayInfo[] dataTypeArrayInfo = DimensionConverter.ToAdsDataTypeArrayInfo((IList<IDimension>) arrayType.Dimensions);
      this.checkArrayDimensions(array, dataTypeArrayInfo, false);
      int elements = new AdsArrayDimensionsInfo(dataTypeArrayInfo).Elements;
      int byteSize = ((IBitSize) dataType).ByteSize;
      ArrayIndexIterator arrayIndexIterator = new ArrayIndexIterator(arrayType, true);
      int num = 0;
      foreach (int[] numArray in arrayIndexIterator)
      {
        if (array.Rank == 1)
        {
          if (num >= array.Length)
            break;
        }
        start += this.Marshal(dataType, encoding, array.GetValue(numArray), data.Slice(start));
        ++num;
      }
      return start;
    }

    /// <summary>Converts byte data to an bitset object (bit access)</summary>
    /// <param name="type">The datatype.</param>
    /// <param name="bitOffset">The bit offset.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="data">The data.</param>
    /// <param name="result">The result.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:System.ArgumentNullException">type
    /// or
    /// data</exception>
    /// <exception cref="T:System.ArgumentException">BitSize not supported! BitSize must be 1 or dividable by 8!</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">type</exception>
    internal int UnmarshalBits(
      IDataType type,
      int bitOffset,
      Encoding encoding,
      ReadOnlySpan<byte> data,
      out object result)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (data == (ReadOnlySpan<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (data));
      if (!((IBitSize) type).IsBitType)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is no bit type.", (object) type.Name), nameof (type));
      if (((IBitSize) type).BitSize < 1)
        throw new ArgumentOutOfRangeException(nameof (type));
      return BitTypeConverter.Unmarshal(((IBitSize) type).BitSize, data, bitOffset, encoding, out result);
    }

    /// <summary>Check Array Dimensions</summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayInfo">The array information.</param>
    /// <param name="exact">Checks bounds exactly.</param>
    private void checkArrayDimensions(Array array, AdsDataTypeArrayInfo[] arrayInfo, bool exact)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (arrayInfo == null)
        throw new ArgumentNullException(nameof (arrayInfo));
      int jagLevel = 0;
      Type baseElementType = (Type) null;
      int jaggedElementCount = 0;
      if (this._marshaler.TryJaggedArray(array, out jagLevel, out baseElementType, out jaggedElementCount))
      {
        if (DimensionConverter.ToDimensionCollection(arrayInfo).ElementCount != jaggedElementCount)
          throw new ArgumentException("Cannot convert dataType of symbol to this type. Jagged array mismatching!");
      }
      else
      {
        if (arrayInfo.Length != array.Rank)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this type. Expected an array rank of {0}, actual rank is {1}", (object) arrayInfo.Length, (object) array.Rank), nameof (arrayInfo));
        if (arrayInfo.Length == 1)
        {
          if (exact && arrayInfo[0].Elements != array.Length)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this type. Expected an array of length {0}, actual length is {1}", (object) arrayInfo[0].Elements, (object) array.Length), nameof (arrayInfo));
          if (!exact && arrayInfo[0].Elements < array.Length)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this type. Expected an array of length {0}, actual length is {1}", (object) arrayInfo[0].Elements, (object) array.Length), nameof (arrayInfo));
        }
        else if (arrayInfo.Length == 2)
        {
          if (arrayInfo[0].Elements != array.GetLength(0))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this type. Expected an array of length {0}, actual length is {1}", (object) arrayInfo[0].Elements, (object) array.GetLength(0)), nameof (arrayInfo));
          if (arrayInfo[1].Elements != array.GetLength(1))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this type. Expected an array of length {0}, actual length is {1}", (object) arrayInfo[1].Elements, (object) array.GetLength(1)), nameof (arrayInfo));
        }
        else
        {
          if (arrayInfo.Length != 3)
            throw new ArgumentException("Cannot convert ads array type of symbol to this type.", nameof (arrayInfo));
          if (arrayInfo[0].Elements != array.GetLength(0))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this type. Expected an array of length {0}, actual length is {1}", (object) arrayInfo[0].Elements, (object) array.GetLength(0)), nameof (arrayInfo));
          if (arrayInfo[1].Elements != array.GetLength(1))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this type. Expected an array of length {0}, actual length is {1}", (object) arrayInfo[1].Elements, (object) array.GetLength(1)), nameof (arrayInfo));
          if (arrayInfo[2].Elements != array.GetLength(2))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert data type of symbol to this type. Expected an array of length {0}, actual length is {1}", (object) arrayInfo[2].Elements, (object) array.GetLength(2)), nameof (arrayInfo));
        }
      }
    }

    internal int UnmarshalStruct(
      IStructType structType,
      Encoding encoding,
      Type? managedStructType,
      ReadOnlySpan<byte> data,
      out object value)
    {
      int length;
      if (managedStructType == (Type) null)
      {
        length = ((IBitSize) structType).ByteSize;
        value = (object) data.Slice(0, length).ToArray();
      }
      else if (PrimitiveTypeMarshaler.Default.CanMarshal(managedStructType))
      {
        length = PrimitiveTypeMarshaler.Default.Unmarshal(managedStructType, data, encoding, out value);
      }
      else
      {
        value = this.createValue((IDataType) structType, managedStructType);
        this.InitializeInstanceValue((IDataType) structType, encoding, ref value, data);
        length = ((IBitSize) structType).ByteSize;
      }
      return length;
    }

    /// <summary>Creates an instance o the specified target type.</summary>
    /// <param name="sourceType">Source Type.</param>
    /// <param name="targetType">Target Type.</param>
    /// <returns>System.Object.</returns>
    private object createValue(IDataType sourceType, Type targetType)
    {
      Type targetType1 = targetType;
      object obj1;
      if (sourceType.Category == 4)
      {
        IArrayType arrayType = (IArrayType) sourceType;
        obj1 = (object) Array.CreateInstance(targetType, arrayType.Dimensions.GetDimensionLengths());
        Array array = (Array) obj1;
        int[] numArray1 = new int[((ICollection<IDimension>) arrayType.Dimensions).Count];
        ArrayIndexIterator arrayIndexIterator = new ArrayIndexIterator(arrayType, true);
        IDataType elementType = arrayType.ElementType;
        if (elementType == null)
          throw new CannotResolveDataTypeException(((IDataType) arrayType).Name);
        foreach (int[] numArray2 in arrayIndexIterator)
        {
          object obj2 = this.createValue(elementType, targetType1);
          array.SetValue(obj2, numArray2);
        }
      }
      else
        obj1 = !targetType.IsArray ? Activator.CreateInstance(targetType) : (object) Array.CreateInstance(targetType.GetElementType(), ((IBitSize) sourceType).ByteSize);
      return obj1;
    }

    /// <summary>
    /// Initializes the specified targetInstance value with the raw byte data.
    /// </summary>
    /// <param name="type">The source symbol.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="targetInstance">The target instance.</param>
    /// <param name="source">The source data.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    internal void InitializeInstanceValue(
      IDataType type,
      Encoding encoding,
      ref object targetInstance,
      ReadOnlySpan<byte> source)
    {
      Exception error;
      if (!this.CanMarshalByReflection(type, targetInstance.GetType(), out error))
        throw error;
      this.initializeInstanceValue(type, encoding, targetInstance, targetInstance.GetType(), (object) null, source);
    }

    /// <summary>
    /// Initializes the specified target instance with the raw byte data.
    /// </summary>
    /// <param name="dataType">The type.</param>
    /// <param name="encoding">The forced encoding or NULL.</param>
    /// <param name="targetInstance">The target instance.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="targetMember">The target member.</param>
    /// <param name="source">The source data.</param>
    /// <exception cref="T:System.ArgumentException">Type is not an enum type!;type</exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    private void initializeInstanceValue(
      IDataType dataType,
      Encoding encoding,
      object targetInstance,
      Type targetType,
      object? targetMember,
      ReadOnlySpan<byte> source)
    {
      object obj1 = (object) null;
      switch ((int) dataType.Category)
      {
        case 1:
        case 10:
          this.Unmarshal(dataType, encoding, source, dataType.GetManagedType(), out obj1);
          this.initializeInstanceValue(targetInstance, targetMember, obj1);
          break;
        case 2:
          IAliasType ialiasType = (IAliasType) dataType;
          this.initializeInstanceValue(ialiasType.BaseType ?? throw new CannotResolveDataTypeException(ialiasType.BaseTypeName), encoding, targetInstance, targetType, targetMember, source);
          break;
        case 3:
          IEnumValue ienumValue = EnumValueFactory.Create((IEnumType) dataType, source);
          object obj2 = targetType.IsEnum ? Enum.Parse(targetType, ((object) ienumValue).ToString(), true) : throw new ArgumentException("Type is not an enum type or enum base type!", nameof (dataType));
          this.initializeInstanceValue(targetInstance, targetMember, obj2);
          break;
        case 4:
          IArrayType iarrayType = (IArrayType) dataType;
          IDataType elementType = iarrayType.ElementType;
          if (elementType == null)
            throw new CannotResolveDataTypeException(iarrayType.ElementTypeName);
          int arrayRank = targetType.GetArrayRank();
          Array array = (Array) targetInstance;
          int[] numArray1 = new int[arrayRank];
          int[] numArray2 = new int[arrayRank];
          int[] lowerBounds = iarrayType.Dimensions.LowerBounds;
          int[] upperBounds = iarrayType.Dimensions.UpperBounds;
          for (int dimension = 0; dimension < arrayRank; ++dimension)
          {
            numArray1[dimension] = array.GetLowerBound(dimension);
            numArray2[dimension] = array.GetUpperBound(dimension);
          }
          for (int position = 0; position < iarrayType.Dimensions.ElementCount; ++position)
          {
            int[] indicesOfPosition = ((ArrayType) iarrayType).GetIndicesOfPosition(position);
            int[] targetMember1 = new int[indicesOfPosition.Length];
            for (int index = 0; index < indicesOfPosition.Length; ++index)
            {
              int num = numArray1[index] - lowerBounds[index];
              targetMember1[index] = indicesOfPosition[index] + num;
            }
            object targetInstance1 = array.GetValue(targetMember1);
            int elementOffset = ((ArrayType) iarrayType).GetElementOffset(indicesOfPosition);
            if (targetInstance1 != null)
              this.initializeInstanceValue(elementType, encoding, targetInstance1, targetInstance1.GetType(), (object) targetMember1, source.Slice(elementOffset, ((IBitSize) elementType).ByteSize));
            else
              AdsModule.Trace.TraceError("Failed to fill array element!");
          }
          break;
        case 5:
          using (IEnumerator<IMember> enumerator = ((IEnumerable<IMember>) ((IInterfaceType) dataType).AllMembers).GetEnumerator())
          {
            while (((IEnumerator) enumerator).MoveNext())
            {
              IMember current = enumerator.Current;
              PropertyInfo property = targetType.GetProperty(((IInstance) current).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
              if (property == (PropertyInfo) null || property.GetGetMethod() == (MethodInfo) null || property.GetSetMethod() == (MethodInfo) null)
              {
                FieldInfo field = targetType.GetField(((IInstance) current).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (field != (FieldInfo) null)
                {
                  if (((IInstance) current).DataType != null)
                    this.initializeInstanceValue(((IInstance) current).DataType, encoding, targetInstance, targetType, (object) field, source.Slice(current.ByteOffset, ((IBitSize) current).ByteSize));
                  else
                    AdsModule.Trace.TraceWarning("Cannot resolve member '{0}' not found within {1}!", new object[2]
                    {
                      (object) ((IInstance) current).InstanceName,
                      (object) targetType.ToString()
                    });
                }
                else
                  AdsModule.Trace.TraceWarning("Struct member '{0}' not found within {1}!", new object[2]
                  {
                    (object) ((IInstance) current).InstanceName,
                    (object) targetType.ToString()
                  });
              }
              else if (((IInstance) current).DataType != null)
                this.initializeInstanceValue(((IInstance) current).DataType, encoding, targetInstance, targetType, (object) property, source.Slice(current.ByteOffset, ((IBitSize) current).ByteSize));
              else
                AdsModule.Trace.TraceWarning("Cannot resolve member '{0}' not found within {1}!", new object[2]
                {
                  (object) ((IInstance) current).InstanceName,
                  (object) targetType.ToString()
                });
            }
            break;
          }
        case 9:
          break;
        case 12:
          break;
        case 13:
        case 15:
          int byteSize = ((IBitSize) dataType).ByteSize;
          break;
        default:
          throw new NotSupportedException();
      }
    }

    /// <summary>Initializes the instance value.</summary>
    /// <param name="instance">The instance.</param>
    /// <param name="member">The member.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="T:System.NotSupportedException"></exception>
    private void initializeInstanceValue(object instance, object member, object value)
    {
      if ((object) (member as FieldInfo) != null)
      {
        FieldInfo fieldInfo = (FieldInfo) member;
        object obj = PrimitiveTypeMarshaler.Convert(value, fieldInfo.FieldType);
        fieldInfo.SetValue(instance, obj);
      }
      else if ((object) (member as PropertyInfo) != null)
      {
        PropertyInfo propertyInfo = (PropertyInfo) member;
        object obj = PrimitiveTypeMarshaler.Convert(value, propertyInfo.PropertyType);
        propertyInfo.SetValue(instance, obj, Array.Empty<object>());
      }
      else
      {
        if (!(member is int[]))
          throw new NotSupportedException();
        ((Array) instance).SetValue(value, (int[]) member);
      }
    }

    /// <summary>Write (Managed) Struct value to ADS.</summary>
    /// <param name="structType">Type of the structure.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentException">type
    /// or
    /// type</exception>
    internal int MarshalStruct(
      IStructType structType,
      Encoding encoding,
      object value,
      Span<byte> data)
    {
      int num = 0;
      if (this._marshaler.CanMarshal(value))
      {
        num = this._marshaler.Marshal(value, data);
      }
      else
      {
        IMemberCollection allMembers = ((IInterfaceType) structType).AllMembers;
        Type type = value.GetType();
        foreach (IMember instance in (IEnumerable<IMember>) allMembers)
        {
          PropertyInfo property = type.GetProperty(((IInstance) instance).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
          MemberInfo member = (MemberInfo) property;
          object obj = (object) null;
          bool flag = false;
          if (property == (PropertyInfo) null || property.GetGetMethod() == (MethodInfo) null)
          {
            FieldInfo field = type.GetField(((IInstance) instance).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            member = (MemberInfo) field;
            if (field != (FieldInfo) null)
            {
              obj = field.GetValue(value);
              flag = true;
            }
          }
          else
          {
            obj = property.GetValue(value);
            flag = true;
          }
          if (!flag)
            throw new MarshalException((IInstance) instance, type, member);
          if (((IInstance) instance).DataType != null)
          {
            this.Marshal(((IInstance) instance).DataType, encoding, obj, data.Slice(instance.Offset, ((IBitSize) instance).ByteSize));
            num = instance.Offset + ((IBitSize) ((IInstance) instance).DataType).ByteSize;
          }
        }
      }
      return num;
    }

    /// <summary>
    /// Checks whether the type can be marshalled by reflection.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="error">The error.</param>
    /// <returns><c>true</c> if this instance [can marshal by reflection] the specified type; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// type
    /// or
    /// targetType
    /// </exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    private bool CanMarshalByReflection(IDataType type, Type targetType, [NotNullWhen(false)] out Exception? error)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetType));
      error = (Exception) null;
      List<IDataType> idataTypeList = (List<IDataType>) null;
      if (this._checkedTypesDict.TryGetValue(targetType, out idataTypeList) && idataTypeList.Contains(type))
      {
        error = (Exception) null;
        return true;
      }
      bool flag1 = true;
      switch ((int) type.Category)
      {
        case 1:
        case 13:
        case 15:
          if (((IBitSize) type).ByteSize > this._marshaler.MarshalSize(targetType))
          {
            error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Source type '{0}' is larger than target type '{1}'!", (object) type.Name, (object) targetType.Name));
            flag1 = false;
            break;
          }
          break;
        case 2:
          IAliasType ialiasType = (IAliasType) type;
          IDataType baseType1 = ialiasType.BaseType;
          if (baseType1 != null)
          {
            flag1 = this.CanMarshalByReflection(baseType1, targetType, out error);
            break;
          }
          error = (Exception) new CannotResolveDataTypeException(ialiasType.BaseTypeName);
          flag1 = false;
          break;
        case 3:
          IEnumType ienumType = (IEnumType) type;
          if (!targetType.IsEnum)
          {
            if (((IAliasType) ienumType).BaseType is IManagedMappableType baseType2)
            {
              flag1 = baseType2.ManagedType == targetType;
              if (!flag1)
              {
                error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is not an enum type or enum base type!", (object) targetType.Name));
                break;
              }
              break;
            }
            flag1 = false;
            error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is not an enum type or enum base type!", (object) targetType.Name));
            break;
          }
          string[] names1 = ((IEnumValueCollection<IEnumValue, IConvertible>) ienumType.EnumValues).GetNames();
          string[] names2 = Enum.GetNames(targetType);
          if (names1.Length > names2.Length)
          {
            flag1 = false;
            error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Enum Types '{0}' and '{1}' are not compatible!", (object) type.Name, (object) targetType.Name));
            break;
          }
          StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
          foreach (string x in names1)
          {
            bool flag2 = false;
            foreach (string y in names2)
            {
              if (ordinalIgnoreCase.Compare(x, y) == 0)
              {
                flag2 = true;
                break;
              }
            }
            if (!flag2)
            {
              flag1 = false;
              error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Enum Types '{0}' and '{1}' are not compatible!", (object) type.Name, (object) targetType.Name));
            }
          }
          break;
        case 4:
          IArrayType iarrayType = (IArrayType) type;
          IDataType elementType1 = iarrayType.ElementType;
          if (!targetType.IsArray)
          {
            error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is not an array type!", (object) targetType.Name));
            break;
          }
          if (elementType1 == null)
          {
            error = (Exception) new CannotResolveDataTypeException(iarrayType.ElementTypeName);
            break;
          }
          int arrayRank = targetType.GetArrayRank();
          if (((ICollection<IDimension>) iarrayType.Dimensions).Count != arrayRank)
          {
            error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Array Types '{0}' and '{1}' are not compatible!", (object) type.Name, (object) targetType.Name));
            break;
          }
          Type elementType2 = targetType.GetElementType();
          flag1 = this.CanMarshalByReflection(elementType1, elementType2, out error);
          break;
        case 5:
          IMemberCollection allMembers = ((IInterfaceType) type).AllMembers;
          bool flag3 = false;
          foreach (IMember imember in (IEnumerable<IMember>) allMembers)
          {
            PropertyInfo property = targetType.GetProperty(((IInstance) imember).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (property == (PropertyInfo) null)
            {
              FieldInfo field = targetType.GetField(((IInstance) imember).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
              if (field == (FieldInfo) null)
              {
                error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Field '{0}' is missing in type '{1}'", (object) ((IInstance) imember).InstanceName, (object) targetType));
                AdsModule.Trace.TraceWarning(error);
              }
              else
              {
                Type fieldType = field.FieldType;
                if (((IInstance) imember).DataType != null)
                  flag3 |= this.CanMarshalByReflection(((IInstance) imember).DataType, fieldType, out error);
              }
            }
            else if (((IInstance) imember).DataType != null)
              flag3 |= this.CanMarshalByReflection(((IInstance) imember).DataType, property.PropertyType, out error);
          }
          flag1 = flag3;
          break;
        case 9:
          ISubRangeType isubRangeType = (ISubRangeType) type;
          IDataType baseType3 = isubRangeType.BaseType;
          if (baseType3 != null)
          {
            flag1 = this.CanMarshalByReflection(baseType3, targetType, out error);
            break;
          }
          error = (Exception) new CannotResolveDataTypeException(isubRangeType.BaseTypeName);
          flag1 = false;
          break;
        case 10:
          if (targetType != typeof (string))
          {
            flag1 = false;
            error = (Exception) new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type mismatch! Target Type '{0}' is not a string (Marshalling AdsType '{1}')!", (object) type.Name, (object) targetType.Name));
            break;
          }
          break;
        case 12:
        case 14:
          flag1 = false;
          break;
        default:
          throw new NotSupportedException();
      }
      if (idataTypeList == null)
      {
        idataTypeList = new List<IDataType>();
        if (!this._checkedTypesDict.ContainsKey(targetType))
          this._checkedTypesDict.Add(targetType, idataTypeList);
      }
      if (flag1)
        idataTypeList.Add(type);
      return flag1;
    }

    /// <summary>Sets the type of the primitive.</summary>
    /// <param name="dataType">The datatype.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The value.</param>
    /// <param name="data">The destination data.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentNullException">dataType</exception>
    /// <exception cref="T:System.ArgumentNullException">primitiveType</exception>
    /// <exception cref="T:System.ArgumentException">Cannot convert datatype of symbol to this type.;type
    /// or
    /// Unexpected datatype. Cannot convert datatype of symbol to this type.;type</exception>
    internal int MarshalPrimitive(
      IDataType dataType,
      Encoding encoding,
      object? value,
      Span<byte> data)
    {
      if (dataType == null)
        throw new ArgumentNullException(nameof (dataType));
      return this._marshaler.Marshal(dataType, encoding, value, data);
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified value to the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if this instance can marshal the specified type; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">type</exception>
    public bool CanMarshal(IDataType type, Encoding encoding, object? value)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (PrimitiveTypeMarshaler.CanMarshal(type.Resolve(), encoding, value))
        return true;
      if (value == null)
        return false;
      Exception error = (Exception) null;
      return this.CanMarshalByReflection(type, value.GetType(), out error);
    }

    /// <summary>Marshals the specified data type.</summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value2">The value.</param>
    /// <param name="data">The data.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.NotSupportedException">Type of symbol not supported</exception>
    public int Marshal(IDataType dataType, Encoding encoding, object? value2, Span<byte> data)
    {
      object targetValue = value2;
      IDataType idataType = dataType.Resolve();
      Type managedType = idataType.GetManagedType();
      if (managedType != (Type) null && value2 != null && managedType != value2.GetType() && !PrimitiveTypeMarshaler.TryConvert(value2, dataType, out targetValue))
        throw new ArgumentOutOfRangeException(nameof (value2));
      if (!this.CanMarshal(dataType, encoding, targetValue))
        throw new ArgumentOutOfRangeException(nameof (value2));
      if (idataType.IsPrimitive)
        return this.MarshalPrimitive(idataType, encoding, targetValue, data);
      if (targetValue != null && idataType.Category == 4)
        return this.MarshalArray((IArrayType) idataType, encoding, targetValue, data);
      if (targetValue != null && idataType.Category == 5)
        return this.MarshalStruct((IStructType) idataType, encoding, targetValue, data);
      if (targetValue != null && targetValue is string && idataType.Category == 10)
        return new StringMarshaler(encoding, (StringConvertMode) 1).Marshal((string) targetValue, ((IBitSize) idataType).ByteSize, data);
      throw new NotSupportedException("Type of symbol not supported");
    }

    /// <summary>
    /// Tries to marshal the specified value to the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    /// <param name="marshalledBytes">The marshalled bytes.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">type</exception>
    public bool TryMarshal(
      IDataType type,
      Encoding encoding,
      object value,
      Span<byte> data,
      out int marshalledBytes)
    {
      IDataType idataType = type != null ? type.Resolve() : throw new ArgumentNullException(nameof (type));
      object targetValue = value;
      marshalledBytes = 0;
      if (idataType.Category == 3 && targetValue is string)
        targetValue = (object) EnumValueFactory.Create((IEnumType) idataType, targetValue);
      Type managedType = idataType.GetManagedType();
      if (managedType != (Type) null && targetValue != null && managedType != targetValue.GetType() && !PrimitiveTypeMarshaler.TryConvert(targetValue, type, out targetValue) || !this.CanMarshal(type, encoding, targetValue))
        return false;
      int num = this._marshaler.MarshalSize(targetValue, encoding);
      if (num <= 0 || num > ((IBitSize) type).ByteSize)
        return false;
      bool flag;
      if (idataType.Category == 10)
      {
        StringMarshaler stringMarshaler = new StringMarshaler(encoding, (StringConvertMode) 1);
        string str = (string) targetValue;
        marshalledBytes = stringMarshaler.Marshal(str, ((IBitSize) idataType).ByteSize, data);
        flag = true;
      }
      else
      {
        marshalledBytes = this.Marshal(idataType, encoding, targetValue, data);
        flag = true;
      }
      return flag;
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <returns><c>true</c> if this instance can marshal the specified symbol; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    public bool CanMarshal(IAttributedInstance symbol, Type? valueType)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      return ((IInstance) symbol).DataType != null && this.CanMarshal(((IInstance) symbol).DataType, valueType);
    }

    private Encoding GetSymbolValueEncoding(IAttributedInstance symbol)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      return symbol.GetValueEncoding() ?? this._marshaler.DefaultValueEncoding;
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified value to the specified type.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if this instance can marshal the specified symbol; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    public bool CanMarshal(IAttributedInstance symbol, object value)
    {
      IDataType dataType = symbol != null ? ((IInstance) symbol).DataType : throw new ArgumentNullException(nameof (symbol));
      IDataType type = dataType != null ? dataType.Resolve() : (IDataType) null;
      Encoding symbolValueEncoding = this.GetSymbolValueEncoding(symbol);
      return type != null && this.CanMarshal(type, symbolValueEncoding, value);
    }

    /// <summary>
    /// Tries to marshal the specified value represented by the specified symbol to the destination buffer.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="value">The value.</param>
    /// <param name="destination">The destination.</param>
    /// <param name="marshalledBytes">The marshalled bytes.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    public bool TryMarshal(
      IAttributedInstance symbol,
      object value,
      Span<byte> destination,
      out int marshalledBytes)
    {
      IDataType dataType = symbol != null ? ((IInstance) symbol).DataType : throw new ArgumentNullException(nameof (symbol));
      IDataType type = dataType != null ? dataType.Resolve() : (IDataType) null;
      if (type != null)
      {
        Encoding symbolValueEncoding = this.GetSymbolValueEncoding(symbol);
        return this.TryMarshal(type, symbolValueEncoding, value, destination, out marshalledBytes);
      }
      marshalledBytes = 0;
      return false;
    }

    /// <summary>Marshals the specified symbol value.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="value">The value.</param>
    /// <param name="destination">The destination span/memory.</param>
    /// <returns>The number of marshalled bytes.</returns>
    public int Marshal(IAttributedInstance symbol, object? value, Span<byte> destination)
    {
      IDataType dataType1 = symbol != null ? ((IInstance) symbol).DataType : throw new ArgumentNullException(nameof (symbol));
      IDataType dataType2 = dataType1 != null ? dataType1.Resolve() : (IDataType) null;
      if (dataType2 == null)
        throw new CannotResolveDataTypeException(((IInstance) symbol).TypeName);
      Encoding symbolValueEncoding = this.GetSymbolValueEncoding(symbol);
      return this.Marshal(dataType2, symbolValueEncoding, value, destination);
    }

    /// <summary>Gets the marshalling size of the symbol/value</summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    public int MarshalSize(IInstance symbol) => symbol != null ? ((IBitSize) symbol).ByteSize : throw new ArgumentNullException(nameof (symbol));

    /// <summary>Unmarshals the specified symbol.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="source">The source.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <param name="value">The value.</param>
    /// <returns>The number of the unmarshalled bytes.</returns>
    public int Unmarshal(
      IAttributedInstance symbol,
      ReadOnlySpan<byte> source,
      Type? valueType,
      out object value)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (valueType == typeof (byte[]))
      {
        value = (object) source.Slice(0, ((IBitSize) symbol).ByteSize).ToArray();
        return ((IBitSize) symbol).ByteSize;
      }
      IDataType dataType1 = ((IInstance) symbol).DataType;
      IDataType dataType2 = dataType1 != null ? dataType1.Resolve() : (IDataType) null;
      Encoding symbolValueEncoding = this.GetSymbolValueEncoding(symbol);
      if (dataType2 != null)
        return this.Unmarshal(dataType2, symbolValueEncoding, source, valueType, out value);
      throw new CannotResolveDataTypeException((IInstance) symbol);
    }
  }
}
