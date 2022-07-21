// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicValueMarshaler
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
using System.Runtime.CompilerServices;
using System.Text;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Converter class that convert 'Any' objects.</summary>
  /// <remarks>The <see cref="T:TwinCAT.TypeSystem.DynamicValueMarshaler" /> adds Struct/Enum/Array creation possibilities to the <see cref="T:TwinCAT.TypeSystem.InstanceValueMarshaler" />.</remarks>
  /// <exclude />
  public class DynamicValueMarshaler : ISymbolMarshaler
  {
    private InstanceValueMarshaler _internalConverter = new InstanceValueMarshaler();
    /// <summary>Cache of already checked 'Any' proxy types.</summary>
    private Dictionary<Type, List<IDataType>> _checkedTypesDict = new Dictionary<Type, List<IDataType>>();

    /// <summary>
    /// Gets the default value encoding like specified by the used ValueAccessor.
    /// </summary>
    /// <value>The default value encoding.</value>
    public Encoding DefaultValueEncoding => this._internalConverter.DefaultValueEncoding;

    /// <summary>Unmarshals the specified memory to the symbol value.</summary>
    /// <param name="symbol">The symbol to unmarshal.</param>
    /// <param name="valueType">Type of the target.</param>
    /// <param name="source">The source memory.</param>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:System.ArgumentNullException">type
    /// or
    /// targetType
    /// or
    /// data</exception>
    /// <exception cref="T:System.ArgumentNullException">type
    /// or
    /// targetType
    /// or
    /// data</exception>
    public int Unmarshal(
      IAttributedInstance symbol,
      ReadOnlySpan<byte> source,
      Type? valueType,
      out object value)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (source == (ReadOnlySpan<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (source));
      return this._internalConverter.Unmarshal(symbol, source, valueType, out value);
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <returns><c>true</c> if this instance can marshal the specified symbol; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(IAttributedInstance symbol, Type? valueType)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      return this._internalConverter.CanMarshal(symbol, valueType);
    }

    internal void InitializeInstanceValue(
      ISymbol symbol,
      ref object targetInstance,
      ReadOnlySpan<byte> data)
    {
      Encoding valueEncoding = ((IAttributedInstance) symbol).ValueEncoding;
      this.InitializeInstanceValue(((IInstance) symbol).DataType ?? throw new CannotResolveDataTypeException((IInstance) symbol), valueEncoding, ref targetInstance, data);
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
      try
      {
        this.CheckType(type, targetInstance.GetType());
      }
      catch (MarshalException ex)
      {
        throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot Type '{0}' to type '{1}'!", (object) type.Name, (object) targetInstance.GetType()), (Exception) ex);
      }
      this.initializeInstanceValue(type, encoding, targetInstance, targetInstance.GetType(), (object) null, source);
    }

    /// <summary>
    /// Initializes the specified target instance with the raw byte data.
    /// </summary>
    /// <param name="dataType">The type.</param>
    /// <param name="encoding">The forced encoding or NULL.</param>
    /// <param name="targetInstance">The target instance.</param>
    /// <param name="valueType">Type of the target.</param>
    /// <param name="targetMember">The target member.</param>
    /// <param name="source">The source data.</param>
    /// <exception cref="T:System.ArgumentException">Type is not an enum type!;type</exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    private void initializeInstanceValue(
      IDataType dataType,
      Encoding encoding,
      object targetInstance,
      Type valueType,
      object? targetMember,
      ReadOnlySpan<byte> source)
    {
      switch ((int) dataType.Category)
      {
        case 1:
        case 10:
          if (targetMember == null)
            throw new ArgumentException("Need to specify target member", nameof (targetMember));
          object obj1;
          this._internalConverter.TypeMarshaller.Unmarshal(dataType, encoding, source, valueType, ref obj1);
          this.initializeInstanceValue(targetInstance, targetMember, obj1);
          break;
        case 2:
          IAliasType ialiasType = (IAliasType) dataType;
          this.initializeInstanceValue(ialiasType.BaseType ?? throw new CannotResolveDataTypeException(ialiasType.BaseTypeName), encoding, targetInstance, valueType, targetMember, source);
          break;
        case 3:
          if (targetMember == null)
            throw new ArgumentException("Need to specify target member", nameof (targetMember));
          IEnumValue ienumValue = EnumValueFactory.Create((IEnumType) dataType, source);
          if (!valueType.IsEnum)
            throw new ArgumentException("Type is not an enum type or enum base type!", nameof (valueType));
          object obj2 = Enum.Parse(valueType, ienumValue.Name, true);
          this.initializeInstanceValue(targetInstance, targetMember, obj2);
          break;
        case 4:
          IArrayType iarrayType = (IArrayType) dataType;
          IDataType elementType = iarrayType.ElementType;
          if (elementType == null)
            throw new CannotResolveDataTypeException(iarrayType.ElementTypeName);
          int arrayRank = valueType.GetArrayRank();
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
              PropertyInfo property = valueType.GetProperty(((IInstance) current).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
              IDataType dataType1 = ((IInstance) current).DataType;
              if (dataType1 == null)
                throw new CannotResolveDataTypeException((IInstance) current);
              if (property == (PropertyInfo) null || property.GetGetMethod() == (MethodInfo) null || property.GetSetMethod() == (MethodInfo) null)
              {
                FieldInfo field = valueType.GetField(((IInstance) current).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (field != (FieldInfo) null)
                  this.initializeInstanceValue(dataType1, encoding, targetInstance, valueType, (object) field, source.Slice(current.ByteOffset, ((IBitSize) current).ByteSize));
                else
                  AdsModule.Trace.TraceWarning("Struct member '{0}' not found within {1}!", new object[2]
                  {
                    (object) ((IInstance) current).InstanceName,
                    (object) valueType.ToString()
                  });
              }
              else
                this.initializeInstanceValue(dataType1, encoding, targetInstance, valueType, (object) property, source.Slice(current.ByteOffset, ((IBitSize) current).ByteSize));
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
      if (instance == null)
        throw new ArgumentNullException(nameof (instance));
      if (member == null)
        throw new ArgumentNullException(nameof (member));
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

    /// <summary>Checks the 'Any'/Proxy type.</summary>
    /// <param name="type">The type.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException">
    /// </exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    private void CheckType(IDataType type, Type targetType)
    {
      List<IDataType> idataTypeList = (List<IDataType>) null;
      if (this._checkedTypesDict.TryGetValue(targetType, out idataTypeList) && idataTypeList.Contains(type))
        return;
      switch ((int) type.Category)
      {
        case 1:
        case 13:
        case 15:
          if (((IBitSize) type).ByteSize > PrimitiveTypeMarshaler.Default.MarshalSize(targetType))
            throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Source type '{0}' is larger than target type '{1}'!", (object) type.Name, (object) targetType.Name));
          goto case 12;
        case 2:
          IAliasType ialiasType = (IAliasType) type;
          IDataType baseType1 = ialiasType.BaseType;
          if (baseType1 == null)
            throw new CannotResolveDataTypeException(ialiasType.BaseTypeName);
          try
          {
            this.CheckType(baseType1, targetType);
            goto case 12;
          }
          catch (MarshalException ex)
          {
            throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot Marshal Alias '{0}' !", (object) ((IDataType) ialiasType).Name), (Exception) ex);
          }
        case 3:
          IEnumType ienumType = (IEnumType) type;
          if (!targetType.IsEnum)
          {
            IManagedMappableType baseType2 = ((IAliasType) ienumType).BaseType as IManagedMappableType;
            bool flag = false;
            if (baseType2 == null)
              throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is not an enum type or enum base type!", (object) targetType.Name));
            flag = baseType2.ManagedType == targetType;
            goto case 12;
          }
          else
          {
            string[] names1 = ((IEnumValueCollection<IEnumValue, IConvertible>) ienumType.EnumValues).GetNames();
            string[] names2 = Enum.GetNames(targetType);
            if (names1.Length > names2.Length)
              throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Enum Types '{0}' and '{1}' are not compatible!", (object) type.Name, (object) targetType.Name));
            StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
            foreach (string x in names1)
            {
              bool flag = false;
              foreach (string y in names2)
              {
                if (ordinalIgnoreCase.Compare(x, y) == 0)
                {
                  flag = true;
                  break;
                }
              }
              if (!flag)
                throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Enum Types '{0}' and '{1}' are not compatible!", (object) type.Name, (object) targetType.Name));
            }
            goto case 12;
          }
        case 4:
          IArrayType iarrayType = (IArrayType) type;
          IDataType elementType1 = iarrayType.ElementType;
          if (elementType1 == null)
            throw new CannotResolveDataTypeException(iarrayType.ElementTypeName);
          int num = targetType.IsArray ? targetType.GetArrayRank() : throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is not an array type!", (object) targetType.Name));
          if (((ICollection<IDimension>) iarrayType.Dimensions).Count != num)
            throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Array Types '{0}' and '{1}' are not compatible!", (object) type.Name, (object) targetType.Name));
          Type elementType2 = targetType.GetElementType();
          try
          {
            this.CheckType(elementType1, elementType2);
            goto case 12;
          }
          catch (MarshalException ex)
          {
            throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot Marshal Elements of Array '{0}'!", (object) ((IDataType) iarrayType).Name), (Exception) ex);
          }
        case 5:
          IStructType istructType = (IStructType) type;
          using (IEnumerator<IMember> enumerator = ((IEnumerable<IMember>) ((IInterfaceType) istructType).AllMembers).GetEnumerator())
          {
            while (((IEnumerator) enumerator).MoveNext())
            {
              IMember current = enumerator.Current;
              PropertyInfo property = targetType.GetProperty(((IInstance) current).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
              IDataType dataType = ((IInstance) current).DataType;
              if (dataType == null)
                throw new CannotResolveDataTypeException((IInstance) current);
              if (property == (PropertyInfo) null)
              {
                FieldInfo field = targetType.GetField(((IInstance) current).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (!(field == (FieldInfo) null))
                {
                  Type fieldType = field.FieldType;
                  try
                  {
                    this.CheckType(dataType, fieldType);
                  }
                  catch (MarshalException ex)
                  {
                    throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot Marshal Member '{0}' of Source Struct '{1}' to field '{2}' of target struct '{3}'!", (object) ((IInstance) current).InstanceName, (object) ((IDataType) istructType).Name, (object) field.Name, (object) targetType.Name), (Exception) ex);
                  }
                }
              }
              else
                this.CheckType(dataType, property.PropertyType);
            }
            goto case 12;
          }
        case 9:
          ISubRangeType isubRangeType = (ISubRangeType) type;
          IDataType baseType3 = isubRangeType.BaseType;
          if (baseType3 == null)
            throw new CannotResolveDataTypeException(isubRangeType.BaseTypeName);
          try
          {
            this.CheckType(baseType3, targetType);
            goto case 12;
          }
          catch (MarshalException ex)
          {
            throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot Marshal Subrange '{0}'!", (object) ((IDataType) isubRangeType).Name), (Exception) ex);
          }
        case 10:
          if (targetType != typeof (string))
            throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type mismatch! Target Type '{0}' is not a string (Marshalling AdsType '{1}')!", (object) type.Name, (object) targetType.Name));
          goto case 12;
        case 12:
        case 14:
          if (idataTypeList == null)
          {
            idataTypeList = new List<IDataType>();
            if (!this._checkedTypesDict.ContainsKey(targetType))
              this._checkedTypesDict.Add(targetType, idataTypeList);
          }
          idataTypeList.Add(type);
          break;
        default:
          throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Converts the specified Value of the the Value Instance object to raw bytes.
    /// </summary>
    /// <param name="symbol">Type of the data.</param>
    /// <param name="value">The object.</param>
    /// <returns>System.Byte[].</returns>
    public byte[] Marshal(IAttributedInstance symbol, object value)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      IDataType dataType = ((IInstance) symbol).DataType;
      if (dataType == null)
        throw new CannotResolveDataTypeException((IInstance) symbol);
      byte[] destination = new byte[((IBitSize) symbol).ByteSize];
      this.CheckType(dataType, value.GetType());
      this.Marshal(symbol, value, (Span<byte>) destination);
      return destination;
    }

    /// <summary>Calculates the MarshalSize of the value.</summary>
    /// <param name="symbol">The type.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    public int MarshalSize(IInstance symbol) => symbol != null ? this._internalConverter.MarshalSize(symbol) : throw new ArgumentNullException(nameof (symbol));

    /// <summary>
    /// Marshals the specified Symbol value to a memory location.
    /// </summary>
    /// <param name="symbol">The Symbol.</param>
    /// <param name="val">The value.</param>
    /// <param name="destination">The b value.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentException">Type is not an enum type!;type
    /// or
    /// Struct member not found!;type</exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    public int Marshal(IAttributedInstance symbol, object val, Span<byte> destination) => this.Marshal((symbol != null ? ((IInstance) symbol).DataType : throw new ArgumentNullException(nameof (symbol))) ?? throw new CannotResolveDataTypeException((IInstance) symbol), symbol.ValueEncoding, val, destination);

    /// <summary>Marshals the specified value to a memory location.</summary>
    /// <param name="type">The type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value or NULL (only if NULL-Pointer value)</param>
    /// <param name="destination">The b value.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentException">Type is not an enum type!;type
    /// or
    /// Struct member not found!;type</exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    internal int Marshal(IDataType type, Encoding encoding, object val, Span<byte> destination)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (type.Category != 13 && val == null)
        throw new ArgumentNullException(nameof (val), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Value <NULL> cannot be set on non-pointer type '{0}'!", (object) type.Name));
      int num1 = 0;
      Type type1 = typeof (object);
      if (val != null)
        type1 = val.GetType();
      int start = 0;
      int num2;
      switch ((int) type.Category)
      {
        case 1:
        case 9:
        case 10:
          num1 = this._internalConverter.TypeMarshaller.Marshal(type, encoding, val, destination.Slice(start, destination.Length - start));
          num2 = start + num1;
          break;
        case 2:
          IAliasType ialiasType = (IAliasType) type;
          num1 = this.Marshal(ialiasType.BaseType ?? throw new CannotResolveDataTypeException(ialiasType.BaseTypeName), encoding, val, destination);
          break;
        case 3:
          IEnumType enumType = (IEnumType) type;
          if (!type1.IsEnum)
            throw new ArgumentException("The Type '" + type1.Name + "' is not an enum type!", nameof (type));
          IEnumValue ienumValue = EnumValueFactory.Create(enumType, val);
          ienumValue.RawValue.CopyTo<byte>(destination);
          num2 = start + ienumValue.RawValue.Length;
          num1 += ienumValue.RawValue.Length;
          break;
        case 4:
          IArrayType iarrayType = (IArrayType) type;
          IDataType elementType = iarrayType.ElementType;
          int arrayRank = type1.GetArrayRank();
          Array array = (Array) val;
          int[] numArray1 = new int[arrayRank];
          int[] numArray2 = new int[arrayRank];
          int[] numArray3 = new int[arrayRank];
          int[] numArray4 = new int[arrayRank];
          for (int index = 0; index < arrayRank; ++index)
          {
            numArray1[index] = array.GetLowerBound(index);
            numArray2[index] = array.GetUpperBound(index);
            numArray3[index] = ((Dimension) ((IList<IDimension>) iarrayType.Dimensions)[index]).LowerBound;
            numArray4[index] = ((Dimension) ((IList<IDimension>) iarrayType.Dimensions)[index]).UpperBound;
          }
          for (int position = 0; position < iarrayType.Dimensions.ElementCount; ++position)
          {
            int[] indicesOfPosition = ((ArrayType) iarrayType).GetIndicesOfPosition(position);
            int[] indices = new int[indicesOfPosition.Length];
            for (int index = 0; index < indicesOfPosition.Length; ++index)
            {
              int num3 = numArray1[index] - numArray3[index];
              indices[index] = indicesOfPosition[index] + num3;
            }
            object val1 = array.GetValue(indices);
            int elementOffset = ((ArrayType) iarrayType).GetElementOffset(indices);
            if (val1 == null)
              throw new MarshalException(type, val1);
            num1 += this.Marshal(type, encoding, val1, destination.Slice(start + elementOffset, ((IBitSize) type).ByteSize));
          }
          break;
        case 5:
          using (IEnumerator<IMember> enumerator = ((IEnumerable<IMember>) ((IInterfaceType) type).AllMembers).GetEnumerator())
          {
            while (((IEnumerator) enumerator).MoveNext())
            {
              IMember current = enumerator.Current;
              PropertyInfo property = type1.GetProperty(((IInstance) current).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
              if (property == (PropertyInfo) null || property.GetGetMethod() == (MethodInfo) null || property.GetSetMethod() == (MethodInfo) null)
              {
                FieldInfo field = type1.GetField(((IInstance) current).InstanceName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (field == (FieldInfo) null)
                {
                  DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
                  interpolatedStringHandler.AppendLiteral("Struct member '");
                  interpolatedStringHandler.AppendFormatted(((IInstance) current).InstanceName);
                  interpolatedStringHandler.AppendLiteral("' (Type: ");
                  interpolatedStringHandler.AppendFormatted(type1.Name);
                  interpolatedStringHandler.AppendLiteral(") not found!");
                  throw new ArgumentException(interpolatedStringHandler.ToStringAndClear(), nameof (type));
                }
                object val2 = field.GetValue(val);
                if (val2 == null)
                  throw new MarshalException((IInstance) current, ((IInstance) current).TypeName, val2);
                num1 += this.Marshal((IAttributedInstance) current, val2, destination.Slice(start, ((IBitSize) current).ByteSize));
              }
              else
              {
                object val3 = property.GetValue((object) type1, Array.Empty<object>());
                if (val3 == null)
                  throw new MarshalException((IInstance) current, ((IInstance) current).TypeName, val3);
                num1 += this.Marshal((IAttributedInstance) current, val3, destination.Slice(start, ((IBitSize) current).ByteSize));
              }
            }
            break;
          }
        default:
          throw new NotSupportedException();
      }
      return num1;
    }

    /// <summary>Tries to get the corresponding managed type.</summary>
    /// <param name="type">The type.</param>
    /// <param name="managed">The managed.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetManagedType(IDataType type, [NotNullWhen(true)] out Type? managed) => this._internalConverter.TryGetManagedType(type, out managed);

    /// <summary>Tries to get the corresponding managed type.</summary>
    /// <param name="symbol">The type.</param>
    /// <param name="managed">The managed.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetManagedType(IAttributedInstance symbol, [NotNullWhen(true)] out Type? managed) => this._internalConverter.TryGetManagedType(symbol, out managed);
  }
}
