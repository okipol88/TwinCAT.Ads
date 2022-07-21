// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.PrimitiveTypeMarshaler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.PlcOpen;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Converter class for all forms of Primitive Types</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PrimitiveTypeMarshaler : IGenericTypeMarshaler, ITypeMarshaler, IStringMarshaler
  {
    /// <summary>
    /// Default Encoding (<see cref="P:TwinCAT.TypeSystem.StringMarshaler.DefaultEncoding" />).
    /// </summary>
    private static readonly Encoding s_DefaultEncoding = StringMarshaler.DefaultEncoding;
    /// <summary>
    /// The Encoding used by this <see cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" />.
    /// </summary>
    private readonly Encoding _encoding;
    /// <summary>
    /// The internal <see cref="T:TwinCAT.TypeSystem.StringMarshaler" />
    /// </summary>
    private readonly StringMarshaler _stringMarshaler;

    /// <summary>
    /// Gets the (default) string Encoding of this <see cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" />
    /// </summary>
    /// <value>The encoding.</value>
    public Encoding DefaultValueEncoding => this._encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" /> class.
    /// </summary>
    /// <param name="encoding">The encoding.</param>
    /// <exclude />
    public PrimitiveTypeMarshaler(Encoding encoding)
    {
      this._encoding = encoding;
      if (this._encoding == null)
        this._encoding = StringMarshaler.DefaultEncoding;
      this._stringMarshaler = new StringMarshaler(this._encoding, (StringConvertMode) 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" /> class.
    /// </summary>
    /// <param name="encoding">The default encoding to be used.</param>
    /// <param name="stringConvertMode">The string convert mode.</param>
    /// <exception cref="T:System.ArgumentNullException">encoding</exception>
    public PrimitiveTypeMarshaler(Encoding encoding, StringConvertMode stringConvertMode)
    {
      this._encoding = encoding != null ? encoding : throw new ArgumentNullException(nameof (encoding));
      this._stringMarshaler = new StringMarshaler(this._encoding, stringConvertMode);
    }

    /// <summary>
    /// Gets the default Converter (with Encoding.Default encoding)
    /// </summary>
    /// <value>The default marshaler.</value>
    public static PrimitiveTypeMarshaler Default => new PrimitiveTypeMarshaler(PrimitiveTypeMarshaler.s_DefaultEncoding);

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" /> initialized with default string encoding and <see cref="F:TwinCAT.TypeSystem.StringConvertMode.FixedLengthZeroTerminated" />
    /// </summary>
    /// <value>The default fixed length string.</value>
    internal static PrimitiveTypeMarshaler DefaultFixedLengthString => new PrimitiveTypeMarshaler(PrimitiveTypeMarshaler.s_DefaultEncoding, (StringConvertMode) 1);

    /// <summary>Gets the unicode converter.</summary>
    /// <value>The unicode.</value>
    public static PrimitiveTypeMarshaler Unicode => new PrimitiveTypeMarshaler(Encoding.Unicode);

    /// <summary>Gets the size of the string terminator '\0'</summary>
    /// <value>The size of the string terminator.</value>
    public int StringTerminatorSize => this._stringMarshaler.StringTerminatorSize;

    /// <summary>The encoding used by this marshaler.</summary>
    /// <value>The encoding.</value>
    public Encoding Encoding => this._encoding;

    /// <summary>
    /// Determines whether the specified category is a primitive type
    /// </summary>
    /// <param name="cat">The category.</param>
    /// <returns><c>true</c> if [is primitive type] [the specified category]; otherwise, <c>false</c>.</returns>
    /// <remarks>Primitive types are types that are indicated with Primitive, String, SubRange, Enum, Pointer</remarks>
    public static bool IsPrimitiveType(DataTypeCategory cat)
    {
      if (cat <= 3)
      {
        if (cat != 1 && cat != 3)
          goto label_4;
      }
      else if (cat - 9 > 1 && cat != 13 && cat != 16)
        goto label_4;
      return true;
label_4:
      return false;
    }

    /// <summary>
    /// Determines whether the specified type identifier is primitive.
    /// </summary>
    /// <param name="typeId">The type identifier.</param>
    /// <returns><c>true</c> if the specified type identifier is primitive; otherwise, <c>false</c>.</returns>
    internal static bool IsPrimitive(AdsDataTypeId typeId)
    {
      if (typeId <= 31)
      {
        if (typeId != null && typeId - 30 > 1)
          goto label_4;
      }
      else if (typeId != 34 && typeId != 65)
        goto label_4;
      return false;
label_4:
      return true;
    }

    /// <summary>
    /// Creates <see cref="T:TwinCAT.TypeSystem.PrimitiveTypeMarshaler" /> from <see cref="T:TwinCAT.TypeSystem.IDataType" />
    /// </summary>
    /// <remarks>PLC code contains STRING (Default Encoding) and WSTRING (Unicode Encoding) types, which are converted
    /// differently. This factory method creates the appropriate converter. </remarks>
    /// <param name="type">The type.</param>
    /// <returns>PrimitiveTypeConverter.</returns>
    /// <exception cref="T:System.ArgumentNullException">type</exception>
    /// <seealso cref="M:TwinCAT.TypeSystem.PrimitiveTypeMarshaler.TryGetStringEncoding(TwinCAT.TypeSystem.IDataType,System.Text.Encoding@)" />
    public static PrimitiveTypeMarshaler CreateFrom(IDataType type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      Encoding encoding;
      return !PrimitiveTypeMarshaler.TryGetStringEncoding(type, out encoding) ? PrimitiveTypeMarshaler.Default : new PrimitiveTypeMarshaler(encoding);
    }

    /// <summary>
    /// Gets the <see cref="P:TwinCAT.TypeSystem.PrimitiveTypeMarshaler.DefaultValueEncoding" /> from the <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns><c>true</c> if encoding found, <c>false</c> otherwise.</returns>
    /// <seealso cref="M:TwinCAT.TypeSystem.PrimitiveTypeMarshaler.CreateFrom(TwinCAT.TypeSystem.IDataType)" />
    public static bool TryGetStringEncoding(IDataType type, out Encoding? encoding)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      bool stringEncoding = false;
      encoding = (Encoding) null;
      if (type.Category == 10)
      {
        stringEncoding = StringMarshaler.TryGetStringEncoding(type, ref encoding);
        if (!stringEncoding)
          stringEncoding = DataTypeStringParser.TryParseString(type.Name, out int _, out encoding);
      }
      return stringEncoding;
    }

    /// <summary>
    /// Determines whether the specified category is a container type.
    /// </summary>
    /// <param name="cat">The data type category.</param>
    /// <returns><c>true</c> if [is container type] [the specified category]; otherwise, <c>false</c>.</returns>
    /// <remarks>Container Types are Array, Structs and its derivates (Function,FunctionBlock and Program)</remarks>
    public static bool IsContainerType(DataTypeCategory cat) => cat - 4 <= 4 || cat - 13 <= 1;

    /// <summary>Determines the size of the specified data type.</summary>
    /// <param name="typeId">The TypeId.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">tp</exception>
    internal int MarshalSize(AdsDataTypeId typeId)
    {
      switch ((int) typeId)
      {
        case 0:
        case 1:
label_7:
          throw new ArgumentOutOfRangeException(nameof (typeId));
        case 2:
label_4:
          return 2;
        case 3:
        case 4:
label_5:
          return 4;
        case 5:
label_6:
          return 8;
        default:
          switch (typeId - 16)
          {
            case 0:
            case 1:
            case 17:
              return 1;
            case 2:
              goto label_4;
            case 3:
              goto label_5;
            case 4:
            case 5:
              goto label_6;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 18:
              goto label_7;
            default:
              if (typeId == 65)
                goto label_7;
              else
                goto label_7;
          }
      }
    }

    /// <summary>Determines whether the managed type is a jagged array</summary>
    /// <param name="jaggedArray">The array to test for (jagged)</param>
    /// <param name="jagLevel">The jag level.</param>
    /// <param name="baseElementType">Type of the (jagged) base element.</param>
    /// <returns><c>true</c> if [is jagged array] [the specified managed type]; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">managedType</exception>
    public bool TryJaggedArray(Type jaggedArray, out int jagLevel, out Type? baseElementType)
    {
      if (jaggedArray == (Type) null)
        throw new ArgumentNullException(nameof (jaggedArray));
      bool flag = false;
      jagLevel = 0;
      baseElementType = (Type) null;
      if (jaggedArray.IsArray)
      {
        Type elementType = jaggedArray.GetElementType();
        if (elementType != (Type) null && elementType.IsArray)
        {
          int num = 0;
          Type type;
          for (type = jaggedArray; type != (Type) null && type.IsArray; type = type.GetElementType())
            ++num;
          jagLevel = num;
          baseElementType = type;
          flag = true;
        }
      }
      return flag;
    }

    /// <summary>Determines whether the managed type is a jagged array</summary>
    /// <param name="array">The array.</param>
    /// <param name="jagLevel">The jag level.</param>
    /// <param name="baseElementType">Type of the (jagged) base element.</param>
    /// <param name="jaggedElementCount">The jagged element count. This is the amount of compatible base elements in a multi-dimensional array</param>
    /// <returns><c>true</c> if [is jagged array] [the specified managed type]; otherwise, <c>false</c>.</returns>
    /// <exception cref="T:System.ArgumentNullException">managedType</exception>
    public bool TryJaggedArray(
      Array array,
      out int jagLevel,
      out Type? baseElementType,
      out int jaggedElementCount)
    {
      Type jaggedArray = array != null ? array.GetType() : throw new ArgumentNullException(nameof (array));
      bool flag = this.TryJaggedArray(jaggedArray, out jagLevel, out baseElementType);
      jaggedElementCount = 0;
      if (flag)
      {
        int rank = array.Rank;
        ArrayIndexIterator arrayIndexIterator = new ArrayIndexIterator(array);
        int val2 = 1;
        int length = array.Length;
        if (jaggedArray.GetElementType().IsArray)
        {
          foreach (int[] numArray in arrayIndexIterator)
          {
            int jaggedElementCount1;
            if (this.TryJaggedArray((Array) array.GetValue(numArray), out int _, out Type _, out jaggedElementCount1))
              val2 = Math.Max(jaggedElementCount1, val2);
          }
        }
        jaggedElementCount = val2 * array.Length;
      }
      return flag;
    }

    /// <summary>Unmarshals the specified byte data to a string value.</summary>
    /// <param name="source">The memory data to unmarshal.</param>
    /// <param name="strLen">Length of the string on fixed length strings, -1 otherwise.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The value.</param>
    /// <returns>The number of consumed bytes from the data array (System.Int32).</returns>
    /// <exception cref="T:System.ArgumentNullException">data</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">offset
    /// or
    /// encoding</exception>
    public int Unmarshal(
      ReadOnlySpan<byte> source,
      int strLen,
      Encoding encoding,
      out string value)
    {
      if (encoding != Encoding.Unicode && encoding != Encoding.ASCII && encoding != StringMarshaler.DefaultEncoding)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 1);
        interpolatedStringHandler.AppendLiteral("Encoding '");
        interpolatedStringHandler.AppendFormatted<Encoding>(encoding);
        interpolatedStringHandler.AppendLiteral("' is not supported by this method!");
        throw new NotSupportedException(interpolatedStringHandler.ToStringAndClear());
      }
      int byteCount = this._encoding.GetByteCount("a");
      int num;
      if (strLen >= 0)
      {
        int length = strLen * byteCount;
        num = length > source.Length ? this._stringMarshaler.Unmarshal(source, encoding, ref value) : this._stringMarshaler.Unmarshal(source.Slice(0, length), encoding, ref value);
      }
      else
        num = this._stringMarshaler.Unmarshal(source, encoding, ref value);
      return num;
    }

    /// <summary>
    /// Unmarshals the specified source data to a managed value.
    /// </summary>
    /// <param name="typeId">The type ID.</param>
    /// <param name="isBitType">The type is handled as bit type..</param>
    /// <param name="source">The memory data to unmarshal.</param>
    /// <param name="val">The result value.</param>
    /// <returns>Number of consumed bytes (System.Int32).</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">tp</exception>
    /// <exception cref="T:System.ArgumentNullException">data
    /// or
    /// data</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">offset
    /// or
    /// tp</exception>
    internal int Unmarshal(
      AdsDataTypeId typeId,
      bool isBitType,
      ReadOnlySpan<byte> source,
      out object val)
    {
      int num = 0;
      byte[] array = source.ToArray();
      switch (typeId - 2)
      {
        case 0:
          val = (object) BitConverter.ToInt16(array, num);
          break;
        case 1:
          val = (object) BitConverter.ToInt32(array, num);
          break;
        case 2:
          val = (object) BitConverter.ToSingle(array, num);
          break;
        case 3:
          val = (object) BitConverter.ToDouble(array, num);
          break;
        default:
          switch (typeId - 16)
          {
            case 0:
              val = (object) (sbyte) source[num];
              break;
            case 1:
              val = (object) source[num];
              break;
            case 2:
              val = (object) BitConverter.ToUInt16(array, num);
              break;
            case 3:
              val = (object) BitConverter.ToUInt32(array, num);
              break;
            case 4:
              val = (object) BitConverter.ToInt64(array, num);
              break;
            case 5:
              val = (object) BitConverter.ToUInt64(array, num);
              break;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
              throw new ArgumentOutOfRangeException(nameof (typeId));
            case 17:
              val = (object) BitConverter.ToBoolean(array, num);
              break;
            default:
              if (typeId == 65)
                goto case 6;
              else
                goto case 6;
          }
          break;
      }
      return this.MarshalSize(typeId);
    }

    /// <summary>
    /// Tries to get the managed type of the <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="managed">The managed.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">Cannot unmarshal type '{0}'!</exception>
    internal static bool TryGetManagedType(IDataType type, [NotNullWhen(true)] out Type? managed)
    {
      managed = (Type) null;
      if (type is IResolvableType iresolvableType)
        type = iresolvableType.ResolveType((DataTypeResolveStrategy) 1);
      IManagedMappableType imanagedMappableType = type as IManagedMappableType;
      managed = (Type) null;
      if (imanagedMappableType != null && imanagedMappableType.ManagedType != (Type) null)
      {
        managed = imanagedMappableType.ManagedType;
      }
      else
      {
        PrimitiveTypeFlags primitiveTypeFlags = (PrimitiveTypeFlags) 0;
        DataTypeCategory category = type.Category;
        switch (category - 1)
        {
          case 0:
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            if (type is IPrimitiveType)
            {
              primitiveTypeFlags = ((IPrimitiveType) type).PrimitiveFlags;
              flag3 = (primitiveTypeFlags & 4) == 4;
            }
            if (primitiveTypeFlags != null)
            {
              flag1 = (primitiveTypeFlags & 8) == 8;
              flag2 = (primitiveTypeFlags & 2) == 2;
            }
            if (flag1)
            {
              if (((IBitSize) type).ByteSize == 2)
              {
                managed = typeof (float);
                break;
              }
              if (((IBitSize) type).ByteSize == 4)
              {
                managed = typeof (double);
                break;
              }
              break;
            }
            if (((IBitSize) type).ByteSize == 1)
            {
              managed = !flag3 ? (!flag2 ? typeof (sbyte) : typeof (byte)) : typeof (bool);
              break;
            }
            if (((IBitSize) type).ByteSize == 2)
            {
              managed = !flag2 ? typeof (short) : typeof (ushort);
              break;
            }
            if (((IBitSize) type).ByteSize == 4)
            {
              managed = !flag2 ? typeof (int) : typeof (uint);
              break;
            }
            if (((IBitSize) type).ByteSize != 8)
              throw new DataTypeException("Cannot unmarshal type '{0}'!", type);
            managed = !flag2 ? typeof (long) : typeof (ulong);
            break;
          case 1:
            break;
          case 2:
            IDataType baseType = ((IAliasType) type).BaseType;
            if (baseType != null)
              return PrimitiveTypeMarshaler.TryGetManagedType(baseType, out managed);
            break;
          case 3:
            IArrayType iarrayType = (IArrayType) type;
            IDataType elementType = iarrayType.ElementType;
            if (elementType != null)
            {
              Type managed1 = (Type) null;
              if (PrimitiveTypeMarshaler.TryGetManagedType(elementType, out managed1))
              {
                int count = ((ICollection<IDimension>) iarrayType.Dimensions).Count;
                managed = count <= 1 ? managed1.MakeArrayType() : managed1.MakeArrayType(count);
                break;
              }
              break;
            }
            break;
          default:
            switch (category - 10)
            {
              case 0:
                managed = typeof (string);
                break;
              case 3:
              case 5:
              case 6:
                if (((IBitSize) type).ByteSize == 4)
                {
                  managed = typeof (uint);
                  break;
                }
                if (((IBitSize) type).ByteSize == 8)
                {
                  managed = typeof (ulong);
                  break;
                }
                break;
            }
            break;
        }
      }
      return managed != (Type) null;
    }

    internal static bool TryGetManagedType(AdsDataTypeId typeId, [NotNullWhen(true)] out Type? tp)
    {
      tp = (Type) null;
      if (typeId <= 21)
      {
        switch (typeId - 2)
        {
          case 0:
            tp = typeof (short);
            break;
          case 1:
            tp = typeof (int);
            break;
          case 2:
            tp = typeof (float);
            break;
          case 3:
            tp = typeof (double);
            break;
          default:
            switch (typeId - 16)
            {
              case 0:
                tp = typeof (sbyte);
                break;
              case 1:
                tp = typeof (byte);
                break;
              case 2:
                tp = typeof (ushort);
                break;
              case 3:
                tp = typeof (uint);
                break;
              case 4:
                tp = typeof (long);
                break;
              case 5:
                tp = typeof (ulong);
                break;
            }
            break;
        }
      }
      else if (typeId - 30 > 1)
      {
        if (typeId == 33)
          tp = typeof (bool);
      }
      else
        tp = typeof (string);
      return tp != (Type) null;
    }

    /// <summary>Tries to get the managed type.</summary>
    /// <param name="typeId">The type identifier.</param>
    /// <param name="tp">The tp.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal static bool TryGetDataTypeId(Type tp, out AdsDataTypeId typeId)
    {
      if (tp == typeof (sbyte))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 16;
        return true;
      }
      if (tp == typeof (byte))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 17;
        return true;
      }
      if (tp == typeof (short))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 2;
        return true;
      }
      if (tp == typeof (ushort))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 18;
        return true;
      }
      if (tp == typeof (int))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 3;
        return true;
      }
      if (tp == typeof (uint))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 19;
        return true;
      }
      if (tp == typeof (long))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 20;
        return true;
      }
      if (tp == typeof (ulong))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 21;
        return true;
      }
      if (tp == typeof (float))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 4;
        return true;
      }
      if (tp == typeof (double))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 5;
        return true;
      }
      if (tp == typeof (bool))
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref typeId = 33;
        return true;
      }
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref typeId = 65;
      return false;
    }

    /// <summary>Gets the primitive flags.</summary>
    /// <param name="type">The type.</param>
    /// <returns>PrimitiveTypeFlags.</returns>
    internal static PrimitiveTypeFlags GetPrimitiveFlags(IDataType type)
    {
      PrimitiveTypeFlags primitiveFlags = (PrimitiveTypeFlags) 0;
      IPrimitiveType iprimitiveType = (IPrimitiveType) type;
      if (iprimitiveType != null)
        primitiveFlags = iprimitiveType.PrimitiveFlags;
      return primitiveFlags;
    }

    /// <summary>Gets the primitive flags.</summary>
    /// <param name="typeId">The type identifier.</param>
    /// <returns>PrimitiveTypeFlags.</returns>
    internal static PrimitiveTypeFlags GetPrimitiveFlags(AdsDataTypeId typeId)
    {
      PrimitiveTypeFlags primitiveFlags = (PrimitiveTypeFlags) 0;
      if (typeId - 2 > 1)
      {
        if (typeId - 4 > 1)
        {
          switch (typeId - 16)
          {
            case 0:
            case 4:
              goto label_4;
            case 1:
            case 2:
            case 3:
            case 5:
              primitiveFlags = (PrimitiveTypeFlags) 66;
              goto label_7;
            case 16:
              break;
            case 17:
              primitiveFlags = (PrimitiveTypeFlags) 4;
              goto label_7;
            default:
              goto label_7;
          }
        }
        primitiveFlags = (PrimitiveTypeFlags) 72;
        goto label_7;
      }
label_4:
      primitiveFlags = (PrimitiveTypeFlags) 64;
label_7:
      return primitiveFlags;
    }

    /// <summary>Gets the primitive flags.</summary>
    /// <param name="tp">The type identifier.</param>
    /// <returns>PrimitiveTypeFlags.</returns>
    internal static PrimitiveTypeFlags GetPrimitiveFlags(Type tp)
    {
      PrimitiveTypeFlags primitiveFlags = (PrimitiveTypeFlags) 0;
      if (tp == typeof (bool))
        primitiveFlags = (PrimitiveTypeFlags) 4;
      else if (tp == typeof (int))
        primitiveFlags = (PrimitiveTypeFlags) 64;
      else if (tp == typeof (short))
        primitiveFlags = (PrimitiveTypeFlags) 64;
      else if (tp == typeof (byte))
        primitiveFlags = (PrimitiveTypeFlags) 1;
      else if (tp == typeof (float))
        primitiveFlags = (PrimitiveTypeFlags) 8;
      else if (tp == typeof (double))
        primitiveFlags = (PrimitiveTypeFlags) 8;
      else if (tp == typeof (long))
        primitiveFlags = (PrimitiveTypeFlags) 64;
      else if (tp == typeof (ulong))
        primitiveFlags = (PrimitiveTypeFlags) 66;
      else if (tp == typeof (uint))
        primitiveFlags = (PrimitiveTypeFlags) 66;
      else if (tp == typeof (ushort))
        primitiveFlags = (PrimitiveTypeFlags) 66;
      else if (tp == typeof (sbyte))
        primitiveFlags = (PrimitiveTypeFlags) 64;
      else if (tp == typeof (TimeSpan))
        primitiveFlags = (PrimitiveTypeFlags) 32;
      else if (tp == typeof (DateTime))
        primitiveFlags = (PrimitiveTypeFlags) 48;
      else if (tp == typeof (DateTimeOffset))
        primitiveFlags = (PrimitiveTypeFlags) 48;
      else if (tp == typeof (DT))
        primitiveFlags = (PrimitiveTypeFlags) 48;
      else if (tp == typeof (DATE))
        primitiveFlags = (PrimitiveTypeFlags) 16;
      else if (tp == typeof (TIME))
        primitiveFlags = (PrimitiveTypeFlags) 32;
      else if (tp == typeof (LTIME))
        primitiveFlags = (PrimitiveTypeFlags) 32;
      else if (tp == typeof (TOD))
        primitiveFlags = (PrimitiveTypeFlags) 32;
      else if (tp == typeof (BitArray))
        primitiveFlags = (PrimitiveTypeFlags) 128;
      return primitiveFlags;
    }

    /// <summary>
    /// Unmarshals a primitive value from source buffer and creates a managed value.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="source">The data.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <param name="value">The primitive value.</param>
    /// <returns>The number of read bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">type</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">source
    /// or
    /// ValueType parameter mismatches dataTypes managed type! - valueType</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">Cannot map to .NET Value!</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">type</exception>
    /// <exception cref="T:System.ArgumentNullException">Type is not primitive!
    /// or
    /// Cannot map to .NET Value!
    /// or
    /// Cannot map to .NET Value!</exception>
    public int Unmarshal(
      IDataType dataType,
      ReadOnlySpan<byte> source,
      Type? valueType,
      out object value)
    {
      return this.Unmarshal(dataType, source, valueType, this._encoding, out value);
    }

    /// <summary>
    /// Unmarshals a primitive value from source buffer and creates a managed value.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="source">The data.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The primitive value.</param>
    /// <returns>The number of read bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">type</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">source
    /// or
    /// ValueType parameter mismatches dataTypes managed type! - valueType</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">Cannot map to .NET Value!</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">type</exception>
    /// <exception cref="T:System.ArgumentNullException">Type is not primitive!
    /// or
    /// Cannot map to .NET Value!
    /// or
    /// Cannot map to .NET Value!</exception>
    public int Unmarshal(
      IDataType dataType,
      ReadOnlySpan<byte> source,
      Type? valueType,
      Encoding encoding,
      out object value)
    {
      if (dataType == null)
        throw new ArgumentNullException(nameof (dataType));
      if (source.Length != ((IBitSize) dataType).ByteSize)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 3);
        interpolatedStringHandler.AppendLiteral("Byte size of type '");
        interpolatedStringHandler.AppendFormatted(dataType.Name);
        interpolatedStringHandler.AppendLiteral("' (");
        interpolatedStringHandler.AppendFormatted<int>(((IBitSize) dataType).ByteSize);
        interpolatedStringHandler.AppendLiteral(") doesn`t match buffer size of '");
        interpolatedStringHandler.AppendFormatted<int>(source.Length);
        interpolatedStringHandler.AppendLiteral("'");
        throw new ArgumentOutOfRangeException(nameof (source), interpolatedStringHandler.ToStringAndClear());
      }
      PrimitiveTypeMarshaler primitiveTypeMarshaler = PrimitiveTypeMarshaler.Default;
      Type type = dataType.GetManagedType();
      if (valueType != (Type) null && type != (Type) null && valueType != type)
        throw new ArgumentOutOfRangeException(nameof (valueType), "ValueType parameter mismatches dataTypes managed type!");
      if (type == (Type) null)
        type = valueType;
      if (!this.CanMarshal(dataType))
        throw new MarshalException(dataType, type);
      if (dataType.Category == 10)
      {
        IStringType istringType = (IStringType) dataType;
        string str;
        this._stringMarshaler.Unmarshal(istringType, source.Slice(0, ((IBitSize) istringType).ByteSize), ref str);
        value = (object) str;
      }
      else
        primitiveTypeMarshaler.Unmarshal(type, source, encoding, out value);
      return ((IBitSize) dataType).ByteSize;
    }

    /// <summary>
    /// Unmarshals the specified memory data to a primitive value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The data.</param>
    /// <param name="val">The value.</param>
    /// <returns>The number of unmarshalled bytes.</returns>
    /// <remarks>
    /// </remarks>
    public int Unmarshal<T>(ReadOnlySpan<byte> source, out T val) => this.Unmarshal<T>(source, this._encoding, out val);

    /// <summary>
    /// Unmarshals the specified memory data to a primitive value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The data.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value.</param>
    /// <returns>The number of unmarshalled bytes.</returns>
    public int Unmarshal<T>(ReadOnlySpan<byte> source, Encoding encoding, out T val)
    {
      bool flag = PrimitiveTypeMarshaler.IsMarshalledAsBitType(typeof (T));
      val = default (T);
      int num;
      object val1;
      if (flag)
      {
        bool[] val2;
        num = this.UnmarshalBits(typeof (T), source, out val2);
        val1 = (object) val2;
      }
      else
        num = this.Unmarshal(typeof (T), source, encoding, out val1);
      val = (T) val1;
      return num;
    }

    /// <summary>
    /// Unmarshals the specified data and creates a managed value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="typeSpecifier">Any type information.</param>
    /// <param name="data">The data.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value.</param>
    /// <returns>System.Int32.</returns>
    public int Unmarshal<T>(
      AnyTypeSpecifier typeSpecifier,
      ReadOnlySpan<byte> data,
      Encoding encoding,
      out T val)
    {
      int num;
      object val1;
      if (PrimitiveTypeMarshaler.IsMarshalledAsBitType(typeof (T)))
      {
        bool[] val2;
        num = this.UnmarshalBits(typeof (T), data, out val2);
        val1 = (object) val2;
      }
      else
        num = this.Unmarshal(typeSpecifier, data, encoding, out val1);
      val = (T) val1;
      return num;
    }

    /// <summary>
    /// Unmarshals the specified source data and returns a managed value.
    /// </summary>
    /// <param name="type">DataType.</param>
    /// <param name="source">The raw data..</param>
    /// <param name="val">The created value.</param>
    /// <returns>The Consumed/Unmarshaled bytes (System.Int32).</returns>
    /// <remarks>The <see cref="P:TwinCAT.TypeSystem.PrimitiveTypeMarshaler.DefaultValueEncoding" /> will be used for strings.
    /// </remarks>
    public int Unmarshal(Type type, ReadOnlySpan<byte> source, out object val) => this.Unmarshal(type, source, this._encoding, out val);

    /// <summary>
    /// Unmarshals the specified source data and returns a managed value.
    /// </summary>
    /// <param name="type">DataType.</param>
    /// <param name="source">The raw data..</param>
    /// <param name="encoding">The encoding to use for strings.</param>
    /// <param name="val">The created value.</param>
    /// <returns>The Consumed/Unmarshaled bytes (System.Int32).</returns>
    public int Unmarshal(Type type, ReadOnlySpan<byte> source, Encoding encoding, out object val)
    {
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      if (source.Length == 0)
        throw new ArgumentOutOfRangeException(nameof (source));
      AnyTypeSpecifier typeSpecifier;
      if (type.IsArray)
      {
        Type elementType = type.GetElementType();
        int num = this.MarshalSize((object) elementType, encoding);
        int arrayRank = type.GetArrayRank();
        int length = source.Length / num;
        if (arrayRank > 1)
          throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot marshal type '{0}'!", (object) type.Name));
        if (elementType == typeof (byte))
        {
          byte[] destination = new byte[length];
          source.CopyTo((Span<byte>) destination);
          val = (object) destination;
          return length * num;
        }
        List<IDimensionCollection> idimensionCollectionList = new List<IDimensionCollection>();
        DimensionCollection dimensionCollection = new DimensionCollection();
        dimensionCollection.Add((IDimension) new Dimension(0, length));
        idimensionCollectionList.Add((IDimensionCollection) dimensionCollection);
        typeSpecifier = new AnyTypeSpecifier(type, (IList<IDimensionCollection>) idimensionCollectionList);
      }
      else
        typeSpecifier = new AnyTypeSpecifier(type);
      return this.Unmarshal(typeSpecifier, source, encoding, out val);
    }

    /// <summary>
    /// Unmarshals the specified type initialized by the source data.
    /// </summary>
    /// <param name="typeSpecifier">The type specifier.</param>
    /// <param name="data">The raw data..</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The created value.</param>
    /// <returns>Consumed bytes (System.Int32).</returns>
    /// <exception cref="T:System.ArgumentNullException">tp
    /// or
    /// data</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">offset
    /// or
    /// tp</exception>
    public int Unmarshal(
      AnyTypeSpecifier typeSpecifier,
      ReadOnlySpan<byte> data,
      Encoding encoding,
      out object val)
    {
      if (typeSpecifier == null)
        throw new ArgumentNullException(nameof (typeSpecifier));
      if (data.Length == 0)
        throw new ArgumentOutOfRangeException(nameof (data));
      bool bitSize = PrimitiveTypeMarshaler.IsMarshalledAsBitType(typeSpecifier.Type);
      return this.Unmarshal(typeSpecifier, bitSize, data, encoding, out val);
    }

    /// <summary>Get the Marshal size of an array.</summary>
    /// <param name="type">The type.</param>
    /// <param name="dimElements">The dim elements.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>The marshal size of the array in bytes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">type</exception>
    public int MarshalSize(Type type, int[] dimElements, Encoding encoding)
    {
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      Type type1 = type.IsArray ? type.GetElementType() : throw new ArgumentOutOfRangeException(nameof (type), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is no array!", (object) type.Name));
      int num = ArrayIndexConverter.ArraySubElementCount(dimElements);
      int size = 0;
      this.TryGetMarshalSize((object) type1, encoding, out size);
      return size * num;
    }

    /// <summary>Gets the marshal size of the specified type in bytes.</summary>
    /// <param name="spec">The type specifier</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>Marshal size in bytes</returns>
    public int MarshalSize(AnyTypeSpecifier spec, Encoding encoding)
    {
      int size = 0;
      this.TryGetMarshalSize(spec, encoding, out size);
      return size;
    }

    /// <summary>Marshals the Array to the Span/Memory destination.</summary>
    /// <param name="value">The value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="destination">The data.</param>
    /// <param name="marshalled">The marshalled.</param>
    /// <returns>
    /// <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">value</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">value - No ArrayType
    /// or
    /// value
    /// or
    /// value</exception>
    internal bool TryMarshalArray(
      object value,
      Encoding encoding,
      Span<byte> destination,
      out int marshalled)
    {
      Type type = value != null ? value.GetType() : throw new ArgumentNullException(nameof (value));
      if (!type.IsArray)
        throw new ArgumentOutOfRangeException(nameof (value), "No ArrayType");
      marshalled = 0;
      if (type == typeof (byte[]))
      {
        byte[] source = (byte[]) value;
        if (source.Length != destination.Length)
          throw new ArgumentOutOfRangeException(nameof (value));
        source.CopyTo<byte>(destination);
        marshalled += source.Length;
      }
      else if (type == typeof (char[]))
      {
        byte[] bytes = this._encoding.GetBytes((char[]) value);
        if (bytes.Length != destination.Length)
          throw new ArgumentOutOfRangeException(nameof (value));
        bytes.CopyTo<byte>(destination);
        marshalled += bytes.Length;
      }
      else
      {
        if (!type.IsArray)
          return false;
        Array array = (Array) value;
        int length = array.Length;
        Type elementType = value.GetType().GetElementType();
        if (PrimitiveTypeMarshaler.IsMarshalledAsBitType(value.GetType()))
        {
          bool[] values = (bool[]) value;
          byte[] source = new byte[destination.Length];
          new BitArray(values).CopyTo((Array) source, 0);
          source.CopyTo<byte>(destination);
          marshalled += source.Length;
        }
        else
        {
          int size1 = 0;
          if (!this.TryGetMarshalSize((object) elementType, encoding, out size1))
            size1 = destination.Length / length;
          foreach (int[] numArray in new ArrayIndexIterator(array))
          {
            int size2 = 0;
            this.TryMarshal(array.GetValue(numArray), encoding, destination.Slice(marshalled, size1), out size2);
            marshalled += size2;
          }
        }
      }
      return true;
    }

    /// <summary>Unmarshals the (primitive) array.</summary>
    /// <param name="typeSpec">The type spec.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="data">The data.</param>
    /// <param name="val">The value.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentNullException">typeSpec</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">typeSpec</exception>
    internal int UnmarshalArray(
      AnyTypeSpecifier typeSpec,
      Encoding encoding,
      ReadOnlySpan<byte> data,
      out object val)
    {
      if (typeSpec == null)
        throw new ArgumentNullException(nameof (typeSpec));
      int num1 = 0;
      bool flag = false;
      AnyTypeSpecifier elementType = typeSpec.ElementType;
      if (elementType == null)
        throw new ArgumentOutOfRangeException(nameof (typeSpec));
      int size = 0;
      if (!this.TryGetMarshalSize(elementType, encoding, out size))
        size = data.Length / typeSpec.DimLengths[0].ElementCount;
      Array array;
      if (flag)
      {
        bool[] val1;
        num1 = this.UnmarshalBits(typeSpec.Type, data, out val1);
        array = (Array) val1;
      }
      else
      {
        array = Array.CreateInstance(elementType.Type, typeSpec.DimLengths[0].GetDimensionLengths());
        ArrayIndexIterator arrayIndexIterator = new ArrayIndexIterator(typeSpec.DimLengths[0].LowerBounds, typeSpec.DimLengths[0].UpperBounds, true);
        int num2 = 0;
        int start = 0;
        PrimitiveTypeMarshaler fixedLengthString = PrimitiveTypeMarshaler.DefaultFixedLengthString;
        foreach (int[] numArray in arrayIndexIterator)
        {
          object val2;
          int num3 = fixedLengthString.Unmarshal(typeSpec.ElementType, data.Slice(start, size), encoding, out val2);
          start += num3;
          num1 += num3;
          array.SetValue(val2, numArray);
          ++num2;
        }
      }
      val = (object) array;
      return num1;
    }

    /// <summary>Gets the length of the array.</summary>
    /// <param name="arrayType">Type of the array.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="marshalSize">Size of the marshal.</param>
    /// <returns>System.Int32.</returns>
    private int GetArrayLength(Type arrayType, Encoding encoding, int marshalSize)
    {
      Type managedType = arrayType.IsArray ? arrayType.GetElementType() : throw new ArgumentOutOfRangeException(nameof (arrayType));
      while (managedType != (Type) null && managedType.IsArray)
        managedType = managedType.GetElementType();
      int size = 0;
      if (!this.TryGetMarshalSize((object) managedType, encoding, out size))
        return 0;
      return !PrimitiveTypeMarshaler.IsMarshalledAsBitType(managedType) ? marshalSize / size : marshalSize / 1 * 8;
    }

    /// <summary>
    /// Unmarshals the specified bit-type initialized by source data.
    /// </summary>
    /// <param name="arrayType">Type of the array.</param>
    /// <param name="data">The raw data..</param>
    /// <param name="val">The created value.</param>
    /// <returns>Consumed bytes (System.Int32).</returns>
    /// <exception cref="T:System.ArgumentNullException">tp
    /// or
    /// data</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">tp
    /// or
    /// data</exception>
    public int UnmarshalBits(Type arrayType, ReadOnlySpan<byte> data, out bool[] val)
    {
      if (arrayType == (Type) null)
        throw new ArgumentNullException(nameof (arrayType));
      if (!arrayType.IsArray)
        throw new ArgumentOutOfRangeException(nameof (arrayType));
      int num = 0;
      arrayType.GetElementType();
      int length = (data.Length - num) / 1 * 8;
      val = new bool[length];
      byte[] numArray = new byte[data.Length];
      data.CopyTo((Span<byte>) numArray);
      new BitArray(numArray).CopyTo((Array) val, 0);
      return data.Length;
    }

    /// <summary>
    /// Unmarshals the specified type initialized by the source data.
    /// </summary>
    /// <param name="typeSpec">The type spec.</param>
    /// <param name="bitSize">if set to <c>true</c> [bit size].</param>
    /// <param name="data">The raw data..</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The created value.</param>
    /// <returns>Consumed bytes (System.Int32).</returns>
    /// <exception cref="T:System.ArgumentNullException">typeSpec</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">data</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    internal unsafe int Unmarshal(
      AnyTypeSpecifier typeSpec,
      bool bitSize,
      ReadOnlySpan<byte> data,
      Encoding encoding,
      out object val)
    {
      if (typeSpec == null)
        throw new ArgumentNullException(nameof (typeSpec));
      if (data.Length == 0)
        throw new ArgumentOutOfRangeException(nameof (data));
      Type type = typeSpec.Type;
      int num1;
      if (type == typeof (bool))
      {
        val = (object) BitConverter.ToBoolean(data);
        num1 = 1;
      }
      else if (type == typeof (int))
      {
        val = (object) BitConverter.ToInt32(data);
        num1 = 4;
      }
      else if (type == typeof (short))
      {
        val = (object) BitConverter.ToInt16(data);
        num1 = 2;
      }
      else if (type == typeof (byte))
      {
        val = (object) data[0];
        num1 = 1;
      }
      else if (type == typeof (float))
      {
        val = (object) BitConverter.ToSingle(data);
        num1 = 4;
      }
      else if (type == typeof (double))
      {
        val = (object) BitConverter.ToDouble(data);
        num1 = 8;
      }
      else if (type == typeof (long))
      {
        val = (object) BitConverter.ToInt64(data);
        num1 = 8;
      }
      else if (type == typeof (uint))
      {
        val = (object) BitConverter.ToUInt32(data);
        num1 = 4;
      }
      else if (type == typeof (ulong))
      {
        val = (object) BitConverter.ToUInt64(data);
        num1 = 8;
      }
      else if (type == typeof (ushort))
      {
        val = (object) BitConverter.ToUInt16(data);
        num1 = 2;
      }
      else if (type == typeof (sbyte))
      {
        val = (object) (sbyte) data[0];
        num1 = 1;
      }
      else if (type == typeof (TimeSpan))
      {
        uint uint32 = BitConverter.ToUInt32(data);
        val = (object) PlcOpenTimeConverter.MillisecondsToTimeSpan(uint32);
        num1 = 4;
      }
      else if (type == typeof (DateTimeOffset))
      {
        uint uint32 = BitConverter.ToUInt32(data);
        val = (object) PlcOpenDateConverterBase.ToDateTime(uint32);
        num1 = 4;
      }
      else if (type == typeof (DateTime))
      {
        uint uint32 = BitConverter.ToUInt32(data);
        val = (object) PlcOpenDateConverterBase.ToDateTime(uint32);
        num1 = 4;
      }
      else if (type == typeof (DT))
      {
        uint uint32 = BitConverter.ToUInt32(data);
        val = (object) new DT(uint32);
        num1 = 4;
      }
      else if (type == typeof (DATE))
      {
        uint uint32 = BitConverter.ToUInt32(data);
        val = (object) new DATE(uint32);
        num1 = 4;
      }
      else if (type == typeof (TIME))
      {
        uint uint32 = BitConverter.ToUInt32(data);
        val = (object) new TIME(uint32);
        num1 = 4;
      }
      else if (type == typeof (LTIME))
      {
        ulong uint64 = BitConverter.ToUInt64(data);
        val = (object) new LTIME(uint64);
        num1 = 8;
      }
      else if (type == typeof (TOD))
      {
        uint uint32 = BitConverter.ToUInt32(data);
        val = (object) new TOD(uint32);
        num1 = 4;
      }
      else if (type.IsArray)
        num1 = this.UnmarshalArray(typeSpec, encoding, data, out val);
      else if (type == typeof (string))
      {
        string str;
        num1 = this._stringMarshaler.Unmarshal(data, encoding, ref str);
        val = (object) str;
      }
      else
      {
        int num2 = type.Attributes.HasFlag((Enum) TypeAttributes.ExplicitLayout) || type.Attributes.HasFlag((Enum) TypeAttributes.SequentialLayout) ? System.Runtime.InteropServices.Marshal.SizeOf(type) : throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot marshal managed type '{0}'", (object) type));
        if (num2 > data.Length)
          throw new MarshalException();
        fixed (byte* numPtr = &MemoryMarshal.GetReference<byte>(data))
        {
          val = System.Runtime.InteropServices.Marshal.PtrToStructure(new IntPtr((void*) numPtr), type);
          num1 = num2;
        }
      }
      return num1;
    }

    /// <summary>
    /// Marshals the specified value to Span/Memory destination.
    /// </summary>
    /// <param name="typeId">The typeId.</param>
    /// <param name="value">The value.</param>
    /// <param name="destination">The memory destination.</param>
    /// <returns>System.Byte[].</returns>
    /// <exception cref="T:System.ArgumentNullException">data</exception>
    /// <exception cref="T:System.NotSupportedException"></exception>
    public int Marshal(AdsDataTypeId typeId, object value, Span<byte> destination)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      byte[] numArray;
      switch ((int) typeId)
      {
        case 0:
        case 1:
label_18:
          throw new NotSupportedException();
        case 2:
          numArray = BitConverter.GetBytes(((IConvertible) value).ToInt16((IFormatProvider) null));
          break;
        case 3:
          numArray = BitConverter.GetBytes(((IConvertible) value).ToInt32((IFormatProvider) null));
          break;
        case 4:
          numArray = BitConverter.GetBytes(((IConvertible) value).ToSingle((IFormatProvider) null));
          break;
        case 5:
          numArray = BitConverter.GetBytes(((IConvertible) value).ToDouble((IFormatProvider) null));
          break;
        default:
          switch (typeId - 16)
          {
            case 0:
              numArray = new byte[1]
              {
                (byte) ((IConvertible) value).ToSByte((IFormatProvider) null)
              };
              break;
            case 1:
              numArray = new byte[1]
              {
                ((IConvertible) value).ToByte((IFormatProvider) null)
              };
              break;
            case 2:
              numArray = BitConverter.GetBytes(((IConvertible) value).ToUInt16((IFormatProvider) null));
              break;
            case 3:
              numArray = BitConverter.GetBytes(((IConvertible) value).ToUInt32((IFormatProvider) null));
              break;
            case 4:
              numArray = BitConverter.GetBytes(((IConvertible) value).ToInt64((IFormatProvider) null));
              break;
            case 5:
              numArray = BitConverter.GetBytes(((IConvertible) value).ToUInt64((IFormatProvider) null));
              break;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 16:
            case 18:
              goto label_18;
            case 14:
              numArray = new byte[StringMarshaler.Default.MarshalSize((string) value)];
              StringMarshaler.Default.Marshal((string) value, numArray.AsSpan<byte>());
              break;
            case 15:
              numArray = new byte[StringMarshaler.Unicode.MarshalSize((string) value)];
              StringMarshaler.Unicode.Marshal((string) value, numArray.AsSpan<byte>());
              break;
            case 17:
              numArray = BitConverter.GetBytes(((IConvertible) value).ToBoolean((IFormatProvider) null));
              break;
            default:
              if (typeId == 65)
                goto label_18;
              else
                goto label_18;
          }
          break;
      }
      numArray.CopyTo<byte>(destination);
      return numArray.Length;
    }

    /// <summary>
    /// Gets the (ADS) representation byte size of the value object.
    /// </summary>
    /// <param name="val">The object value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>Marshalled size of the object when transferred.</returns>
    /// <exception cref="T:System.ArgumentNullException">val</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">val</exception>
    public int MarshalSize(object? val, Encoding encoding)
    {
      int size = 0;
      this.TryGetMarshalSize(val, encoding, this._stringMarshaler.StringMode, out size);
      return size;
    }

    /// <summary>
    /// Gets the (ADS) representation byte size of the value object.
    /// </summary>
    /// <param name="val">The object value.</param>
    /// <returns>Marshalled size of the object when transferred.</returns>
    /// <exception cref="T:System.ArgumentNullException">val</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">val</exception>
    public int MarshalSize(object? val) => (object) (val as Type) != null ? this.MarshalSize((Type) val) : this.MarshalSize(val, this._encoding);

    /// <summary>
    /// Determines whether the managed type will be Marshalled as bit type
    /// </summary>
    /// <param name="managedType">Type of the managed.</param>
    /// <returns><c>true</c> if [is marshalled as bit type] [the specified managed type]; otherwise, <c>false</c>.</returns>
    public static bool IsMarshalledAsBitType(Type managedType) => false;

    /// <summary>
    /// Gets the marshal size of the string given by its length.
    /// </summary>
    /// <param name="encoding">The encoding.</param>
    /// <param name="strLen">Length of the string.</param>
    /// <returns>Marshalling size of the string.</returns>
    public int MarshalSize(Encoding encoding, int strLen) => this._stringMarshaler.MarshalSize(encoding, strLen);

    /// <summary>
    /// Gets the marshal size of the string given by its length.
    /// </summary>
    /// <param name="strLen">Length of the string.</param>
    /// <returns>Marshalling size of the string.</returns>
    public int MarshalSize(int strLen) => this._stringMarshaler.MarshalSize(this._encoding, strLen);

    /// <summary>Gets the marshal size of the string.</summary>
    /// <param name="value">The string value.</param>
    /// <returns>Marshalling size of the string.</returns>
    public int MarshalSize(string value) => this._stringMarshaler.MarshalSize(value);

    /// <summary>
    /// Determines whether this instance can marshal the <see cref="T:TwinCAT.TypeSystem.DataTypeCategory" />
    /// </summary>
    /// <param name="category">The data type category</param>
    /// <returns><c>true</c> if this instance can marshal the specified category; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(DataTypeCategory category)
    {
      if (category != 1 && category != 3)
      {
        switch (category - 9)
        {
          case 0:
          case 1:
          case 4:
          case 6:
          case 7:
            break;
          default:
            return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Determines the specified value can be marshalled to the specified datatype
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// <c>true</c> if this instance can marshal the specified type; otherwise, <c>false</c>.</returns>
    public static bool CanMarshal(IDataType type, Encoding encoding, object? value)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      bool flag = false;
      Type managedType = type.Resolve().GetManagedType();
      if (managedType != (Type) null)
      {
        if (type.Category == 10)
          flag = StringMarshaler.CanMarshal(((IBitSize) type).ByteSize, encoding, (StringConvertMode) 2, (string) value);
        else if (type.Category == 4)
        {
          IDataType elementType = ((IArrayType) type).ElementType;
          if (value.GetType().IsArray && elementType != null)
          {
            Array array = (Array) value;
            Type type1 = value.GetType();
            type1.GetElementType();
            type1.GetArrayRank();
            int[] lowerBounds;
            PrimitiveTypeMarshaler.GetArrayBounds(array, out lowerBounds, out int[] _);
            object obj = ((Array) value).GetValue(lowerBounds);
            flag = PrimitiveTypeMarshaler.CanMarshal(elementType, encoding, obj);
          }
          else
            flag = false;
        }
        else
          flag = managedType == value.GetType() || PrimitiveTypeMarshaler.CanConvert(value, managedType);
      }
      return flag;
    }

    /// <summary>Determines the specified type can be marshalled.</summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if this instance can marshal the specified type; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(IDataType type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (this.CanMarshal(type.Category))
        return true;
      if (type.Category == 4)
      {
        IDataType elementType = ((IArrayType) type).ElementType;
        return elementType != null && this.CanMarshal(elementType);
      }
      return type.GetManagedType() != (Type) null;
    }

    /// <summary>
    /// Determines whether ADS (AnyType Marshalling) can marshal the specified data type.
    /// </summary>
    /// <param name="type">The Managed data type.</param>
    /// <returns><c>true</c> if this instance can marshal the specified data type; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(Type type)
    {
      int size = 0;
      return PrimitiveTypeMarshaler.TryGetMarshalSize(type, out size);
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if this instance can marshal the specified value; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(object value)
    {
      int size = 0;
      return this.TryGetMarshalSize(value, StringMarshaler.DefaultEncoding, out size);
    }

    /// <summary>
    /// Determines whether this instance can marshal the specified type identifier.
    /// </summary>
    /// <param name="typeId">The type identifier.</param>
    /// <returns><c>true</c> if this instance can marshal the specified type identifier; otherwise, <c>false</c>.</returns>
    public bool CanMarshal(AdsDataTypeId typeId)
    {
      if (typeId <= 21)
      {
        if (typeId - 2 > 3 && typeId - 16 > 5)
          goto label_4;
      }
      else if (typeId - 30 > 1 && typeId != 33)
        goto label_4;
      return true;
label_4:
      return false;
    }

    /// <summary>
    /// Gets the MarshalSize of a string of the specified length.
    /// </summary>
    /// <param name="strLen">Length of the string.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="size">The size.</param>
    /// <returns>
    /// <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetStringMarshalSize(int strLen, Encoding encoding, out int size)
    {
      size = 0;
      size = this._stringMarshaler.MarshalSize(encoding, strLen);
      return size != 0;
    }

    /// <summary>
    /// Gets the (AdsMarshalling) Size of the specified string
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="size">The size.</param>
    /// <returns>
    /// <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetStringMarshalSize(string value, Encoding encoding, out int size)
    {
      size = this._stringMarshaler.MarshalSize((object) value, encoding);
      return size >= 0;
    }

    /// <summary>Gets the (AdsMarshalling) Size of the specified type.</summary>
    /// <param name="anyType">The managed Data type to be marshalled via ADS.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="size">The size.</param>
    /// <returns>
    /// <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetArrayMarshalSize(AnyTypeSpecifier anyType, Encoding encoding, out int size)
    {
      if (anyType == null)
        throw new ArgumentNullException(nameof (anyType));
      if (!anyType.Type.IsArray)
        throw new ArgumentOutOfRangeException(nameof (anyType));
      size = 0;
      AnyTypeSpecifier elementType = anyType.ElementType;
      int size1 = 0;
      int num = 1;
      if (!this.TryGetMarshalSize(elementType, encoding, out size1))
        return false;
      for (int index = 0; index < ((ICollection<IDimensionCollection>) anyType.DimLengths).Count; ++index)
      {
        IDimensionCollection dimLength = anyType.DimLengths[index];
        num *= dimLength.ElementCount;
      }
      size = num * size1;
      return true;
    }

    /// <summary>
    /// Tries to get the marshal size of the specified data type.
    /// </summary>
    /// <param name="anyType">Any type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="size">The marshal size of the type.</param>
    /// <returns>
    /// <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetMarshalSize(AnyTypeSpecifier anyType, Encoding encoding, out int size)
    {
      if (anyType == null)
        throw new ArgumentNullException(nameof (anyType));
      return anyType.Category != 4 ? (anyType.Category != 10 ? PrimitiveTypeMarshaler.TryGetMarshalSize(anyType.Type, out size) : this.TryGetStringMarshalSize(anyType.StrLen, encoding, out size)) : this.TryGetArrayMarshalSize(anyType, encoding, out size);
    }

    /// <summary>
    /// Tries to get the marshal size of the specified data type.
    /// </summary>
    /// <param name="anyType">Any type.</param>
    /// <param name="size">The marshal size of the type.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetMarshalSize(AnyTypeSpecifier anyType, out int size) => this.TryGetMarshalSize(anyType, this._encoding, out size);

    /// <summary>Gets the MarshalSize of the specified managed type.</summary>
    /// <param name="dataType">The managed Data type to be marshalled via ADS.</param>
    /// <param name="size">The size.</param>
    /// <returns><c>true</c> if marshaling is possible, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">dataType</exception>
    public static bool TryGetMarshalSize(Type dataType, out int size)
    {
      if (dataType == (Type) null)
        throw new ArgumentNullException(nameof (dataType));
      size = 0;
      if (dataType == typeof (bool))
        size = 1;
      else if (dataType == typeof (int))
        size = 4;
      else if (dataType == typeof (short))
        size = 2;
      else if (dataType == typeof (byte))
        size = 1;
      else if (dataType == typeof (float))
        size = 4;
      else if (dataType == typeof (double))
        size = 8;
      else if (dataType == typeof (long))
        size = 8;
      else if (dataType == typeof (ulong))
        size = 8;
      else if (dataType == typeof (uint))
        size = 4;
      else if (dataType == typeof (ushort))
        size = 2;
      else if (dataType == typeof (sbyte))
        size = 1;
      else if (!(dataType == typeof (char)))
      {
        if (dataType == typeof (TimeSpan))
          size = 4;
        else if (dataType == typeof (DateTime))
          size = 4;
        else if (dataType == typeof (DateTimeOffset))
          size = 4;
        else if (dataType == typeof (DT))
          size = 4;
        else if (dataType == typeof (DATE))
          size = 4;
        else if (dataType == typeof (TIME))
          size = 4;
        else if (dataType == typeof (LTIME))
          size = 8;
        else if (dataType == typeof (TOD))
          size = 4;
        else if (dataType.Attributes.HasFlag((Enum) TypeAttributes.ExplicitLayout) || dataType.Attributes.HasFlag((Enum) TypeAttributes.SequentialLayout))
          size = System.Runtime.InteropServices.Marshal.SizeOf(dataType);
        else if (!(dataType == typeof (string)))
        {
          int num = dataType.IsArray ? 1 : 0;
        }
      }
      return size > 0 || dataType == typeof (void);
    }

    /// <summary>Gets the MarshalSize of the specified value.</summary>
    /// <param name="value">The managed Data type to be marshalled via ADS.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="size">The size.</param>
    /// <returns>
    /// <c>true</c> if marshalling size can be determined, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">val</exception>
    public bool TryGetMarshalSize(object value, Encoding encoding, out int size)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      value.GetType();
      bool marshalSize;
      if (value is string)
        marshalSize = this.TryGetStringMarshalSize((string) value, encoding, out size);
      else if (value is Array)
        marshalSize = this.TryGetArrayMarshalSize(new AnyTypeSpecifier(value), encoding, out size);
      else if (value is ICustomMarshaler)
      {
        ICustomMarshaler customMarshaler = (ICustomMarshaler) value;
        size = customMarshaler.GetNativeDataSize();
        marshalSize = true;
      }
      else
        marshalSize = this.TryGetMarshalSize(value, encoding, this._stringMarshaler.StringMode, out size);
      return marshalSize;
    }

    /// <summary>Gets the MarshalSize of the specified value</summary>
    /// <param name="value">The managed Data type to be marshalled via ADS.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="convertMode">The string convert mode.</param>
    /// <param name="size">The marshalling size in bytes.</param>
    /// <returns><c>true</c> if marshalling size can be determined, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">val</exception>
    /// <exception cref="T:System.ArgumentNullException">val</exception>
    private bool TryGetMarshalSize(
      object? value,
      Encoding encoding,
      StringConvertMode convertMode,
      out int size)
    {
      if (value == null)
      {
        size = 0;
        return true;
      }
      Type type = (object) (value as Type) == null ? value.GetType() : (Type) value;
      size = 0;
      if (type == typeof (BitArray))
      {
        BitArray bitArray = (BitArray) value;
        size = bitArray.Length / 8;
        if (bitArray.Length % 8 > 0)
          ++size;
      }
      else
      {
        if (type.IsArray && value is Array)
        {
          Array array1 = (Array) value;
          Type elementType = type.GetElementType();
          int length = array1.Length;
          Array array2 = array1;
          while (elementType.IsArray)
          {
            int num = 0;
            for (int index = 0; index < array2.Length; ++index)
            {
              Array array3 = (Array) array2.GetValue(index);
              num = num < array3.Length ? array3.Length : num;
            }
            elementType = elementType.GetElementType();
            length *= num;
          }
          int num1 = this.MarshalSize((object) elementType, encoding);
          size = !PrimitiveTypeMarshaler.IsMarshalledAsBitType(type) ? length * num1 : length / 8 + (length % 8 > 0 ? 1 : 0);
          return true;
        }
        switch (value)
        {
          case string _:
            StringMarshaler stringMarshaler = new StringMarshaler(encoding, convertMode);
            size = stringMarshaler.MarshalSize((string) value);
            break;
          case IEnumValue _:
            size = ((IEnumValue) value).Size;
            return true;
          default:
            return PrimitiveTypeMarshaler.TryGetMarshalSize(type, out size);
        }
      }
      return size > 0;
    }

    /// <summary>Gets the MarshalSize of the specified managed type.</summary>
    /// <param name="dataType">Type of the data.</param>
    /// <returns>int.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException">$"Cannot marshal dotnet type '{dataType.Name}'!</exception>
    public int MarshalSize(Type dataType)
    {
      int size;
      if (!PrimitiveTypeMarshaler.TryGetMarshalSize(dataType, out size))
        throw new MarshalException("Cannot marshal dotnet type '" + dataType.Name + "'!");
      return size;
    }

    /// <summary>Marshals the value to the destination span/memory</summary>
    /// <param name="value">The value.</param>
    /// <param name="destination">The marshalling destination.</param>
    /// <param name="size">Number of marshalled bytes.</param>
    /// <returns>
    /// <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">val</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">val</exception>
    public bool TryMarshal(object value, Span<byte> destination, out int size) => this.TryMarshal(value, this._encoding, destination, out size);

    /// <summary>Marshals the value to the destination span/memory</summary>
    /// <param name="value">The value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="destination">The marshalling destination.</param>
    /// <param name="size">Number of marshalled bytes.</param>
    /// <returns>
    /// <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">val</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">val</exception>
    public unsafe bool TryMarshal(
      object? value,
      Encoding encoding,
      Span<byte> destination,
      out int size)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      bool flag = false;
      Type type = value.GetType();
      size = this.MarshalSize(value, encoding);
      if (size > destination.Length)
        ((AdsErrorCode) 1797).ThrowOnError();
      if (type == typeof (bool))
      {
        BitConverter.GetBytes(((IConvertible) value).ToBoolean((IFormatProvider) null)).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (int))
      {
        int int32 = ((IConvertible) value).ToInt32((IFormatProvider) null);
        BinaryPrimitives.WriteInt32LittleEndian(destination, int32);
        flag = true;
      }
      else if (type == typeof (short))
      {
        short int16 = ((IConvertible) value).ToInt16((IFormatProvider) null);
        BinaryPrimitives.WriteInt16LittleEndian(destination, int16);
        flag = true;
      }
      else if (type == typeof (byte))
      {
        byte num = ((IConvertible) value).ToByte((IFormatProvider) null);
        destination[0] = num;
        flag = true;
      }
      else if (type == typeof (float))
      {
        BitConverter.GetBytes(((IConvertible) value).ToSingle((IFormatProvider) null)).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (double))
      {
        BitConverter.GetBytes(((IConvertible) value).ToDouble((IFormatProvider) null)).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (long))
      {
        long int64 = ((IConvertible) value).ToInt64((IFormatProvider) null);
        BinaryPrimitives.WriteInt64LittleEndian(destination, int64);
        flag = true;
      }
      else if (type == typeof (ulong))
      {
        ulong uint64 = ((IConvertible) value).ToUInt64((IFormatProvider) null);
        BinaryPrimitives.WriteUInt64LittleEndian(destination, uint64);
        flag = true;
      }
      else if (type == typeof (uint))
      {
        uint uint32 = ((IConvertible) value).ToUInt32((IFormatProvider) null);
        BinaryPrimitives.WriteUInt32LittleEndian(destination, uint32);
        flag = true;
      }
      else if (type == typeof (ushort))
      {
        ushort uint16 = ((IConvertible) value).ToUInt16((IFormatProvider) null);
        BinaryPrimitives.WriteUInt16LittleEndian(destination, uint16);
        flag = true;
      }
      else if (type == typeof (sbyte))
      {
        sbyte num = ((IConvertible) value).ToSByte((IFormatProvider) null);
        destination[0] = (byte) num;
        flag = true;
      }
      else if (type == typeof (TimeSpan))
      {
        PlcOpenTimeConverter.GetBytes(new TIME((TimeSpan) value)).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (DateTime))
      {
        PlcOpenDTConverter.GetBytes((DateTime) value).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (DateTimeOffset))
      {
        PlcOpenDTConverter.GetBytes(((DateTimeOffset) value).LocalDateTime).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (DT))
      {
        PlcOpenDTConverter.GetBytes((DT) value).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (DATE))
      {
        PlcOpenDateConverter.GetBytes((DATE) value).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (TIME))
      {
        PlcOpenTimeConverter.GetBytes((TIME) value).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (LTIME))
      {
        PlcOpenTimeConverter.GetBytes((LTIME) value).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (TOD))
      {
        PlcOpenTODConverter.GetBytes((TOD) value).CopyTo<byte>(destination);
        flag = true;
      }
      else if (type == typeof (BitArray))
      {
        BitArray bitArray = (BitArray) value;
        int length = bitArray.Count / 8;
        if (bitArray.Count % 8 > 0)
          ++length;
        byte[] source = new byte[length];
        bitArray.CopyTo((Array) source, 0);
        source.CopyTo<byte>(destination);
        flag = true;
      }
      else if (type.IsArray)
      {
        Array array = (Array) value;
        int marshalled = 0;
        flag = this.TryMarshalArray((object) array, encoding, destination, out marshalled);
      }
      else if (type == typeof (string))
      {
        size = this._stringMarshaler.Marshal((string) value, encoding, destination);
        flag = true;
      }
      else if (value is ICustomMarshaler)
      {
        ICustomMarshaler customMarshaler = (ICustomMarshaler) value;
        int nativeDataSize = customMarshaler.GetNativeDataSize();
        IntPtr native = customMarshaler.MarshalManagedToNative(value);
        new Span<byte>(native.ToPointer(), nativeDataSize).CopyTo(destination);
        customMarshaler.CleanUpNativeData(native);
        flag = true;
      }
      else if (type.Attributes.HasFlag((Enum) TypeAttributes.ExplicitLayout) || type.Attributes.HasFlag((Enum) TypeAttributes.SequentialLayout))
      {
        System.Runtime.InteropServices.Marshal.SizeOf(value);
        fixed (byte* numPtr = &MemoryMarshal.GetReference<byte>(destination))
          System.Runtime.InteropServices.Marshal.StructureToPtr(value, new IntPtr((void*) numPtr), false);
        flag = true;
      }
      return flag;
    }

    /// <summary>Marshals the value/type to span/memory.</summary>
    /// <param name="pointerType">The type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The value.</param>
    /// <param name="lengthIs">Lenght of the value in bytes.</param>
    /// <param name="destination">The destination memory.</param>
    /// <param name="marshaledBytes">The number of marshaled bytes.</param>
    /// <returns>The number of marshalled bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">pointerType</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.CannotResolveDataTypeException"></exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">type</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">destination</exception>
    public bool TryMarshal(
      IPointerType pointerType,
      Encoding encoding,
      object? value,
      int lengthIs,
      Span<byte> destination,
      out int marshaledBytes)
    {
      if (pointerType == null)
        throw new ArgumentNullException(nameof (pointerType));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      bool flag = false;
      marshaledBytes = 0;
      IDataType referencedType = pointerType.ReferencedType;
      if (referencedType == null)
        throw new CannotResolveDataTypeException(pointerType.ReferenceTypeName);
      if (value.GetType() == typeof (byte[]))
      {
        byte[] array = (byte[]) value;
        int length1 = array.Length;
        int length2 = Math.Min(lengthIs, length1);
        array.AsSpan<byte>(0, length2).CopyTo(destination.Slice(0, length2));
        marshaledBytes = length2;
        flag = true;
      }
      else if (value.GetType() == typeof (string))
      {
        if (pointerType is PCCHType)
        {
          StringMarshaler stringMarshaler = new StringMarshaler(Encoding.UTF8, (StringConvertMode) 0);
          int val2 = stringMarshaler.MarshalSize((string) value);
          int num = Math.Min(lengthIs, val2);
          marshaledBytes = stringMarshaler.Marshal((string) value, num, destination);
          flag = true;
        }
        else if (((IBitSize) referencedType).ByteSize == 1)
        {
          StringMarshaler stringMarshaler = new StringMarshaler(encoding, (StringConvertMode) 0);
          int val2 = stringMarshaler.MarshalSize((string) value);
          int num = Math.Min(lengthIs, val2);
          marshaledBytes = stringMarshaler.Marshal((string) value, num, destination);
          flag = true;
        }
      }
      return flag;
    }

    /// <summary>Marshals the value/type to span/memory.</summary>
    /// <param name="type">The type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value.</param>
    /// <param name="destination">The destination memory.</param>
    /// <param name="marshaledBytes">The number of marshaled bytes.</param>
    /// <returns>The number of marshalled bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">type</exception>
    /// <exception cref="T:System.ArgumentNullException">destination</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">val</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">type</exception>
    public unsafe bool TryMarshal(
      IDataType type,
      Encoding encoding,
      object? val,
      Span<byte> destination,
      out int marshaledBytes)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (destination == (Span<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (destination));
      if (val == null)
        throw new ArgumentNullException(nameof (val));
      marshaledBytes = 0;
      bool flag1 = false;
      if (type is IManagedMappableType imanagedMappableType && imanagedMappableType.ManagedType != (Type) null)
      {
        object targetValue = val;
        Type managedType = imanagedMappableType.ManagedType;
        if (val != null && managedType != val.GetType())
        {
          if (type is IEnumType)
          {
            IEnumType ienumType = (IEnumType) type;
            if (val is string)
              val = (object) ienumType.Parse((string) val);
          }
          if (!PrimitiveTypeMarshaler.TryConvert(val, managedType, out targetValue))
          {
            string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert Value '{0} ({1})' to type '{2}'", val, (object) val.GetType(), (object) managedType);
            throw new ArgumentOutOfRangeException(nameof (val), val, message);
          }
        }
        this.Marshal(targetValue, encoding, destination.Slice(0, ((IBitSize) type).ByteSize));
        marshaledBytes = ((IBitSize) type).ByteSize;
        flag1 = true;
      }
      else
      {
        DataTypeCategory category = type.Category;
        switch (category - 1)
        {
          case 0:
            PrimitiveTypeFlags primitiveTypeFlags = (PrimitiveTypeFlags) 0;
            bool flag2 = false;
            bool flag3 = false;
            byte[] source1 = Array.Empty<byte>();
            if (type is IPrimitiveType)
              primitiveTypeFlags = ((IPrimitiveType) type).PrimitiveFlags;
            if (primitiveTypeFlags != null)
            {
              flag2 = (primitiveTypeFlags & 8) == 8;
              flag3 = (primitiveTypeFlags & 2) == 2;
            }
            if (flag2)
            {
              if (((IBitSize) type).ByteSize == 2)
                source1 = BitConverter.GetBytes(((IConvertible) val).ToSingle((IFormatProvider) null));
              else if (((IBitSize) type).ByteSize == 4)
                source1 = BitConverter.GetBytes(((IConvertible) val).ToDouble((IFormatProvider) null));
            }
            else if (((IBitSize) type).ByteSize == 1)
            {
              if ((primitiveTypeFlags & 4) == 4)
                source1 = BitConverter.GetBytes(((IConvertible) val).ToBoolean((IFormatProvider) null));
              else
                source1 = new byte[1]{ (byte) val };
            }
            else if (((IBitSize) type).ByteSize == 2)
              source1 = !flag3 ? BitConverter.GetBytes(((IConvertible) val).ToInt16((IFormatProvider) null)) : BitConverter.GetBytes(((IConvertible) val).ToUInt16((IFormatProvider) null));
            else if (((IBitSize) type).ByteSize == 4)
              source1 = !flag3 ? BitConverter.GetBytes(((IConvertible) val).ToInt32((IFormatProvider) null)) : BitConverter.GetBytes(((IConvertible) val).ToUInt32((IFormatProvider) null));
            else if (((IBitSize) type).ByteSize == 8)
              source1 = !flag3 ? BitConverter.GetBytes(((IConvertible) val).ToInt64((IFormatProvider) null)) : BitConverter.GetBytes(((IConvertible) val).ToUInt64((IFormatProvider) null));
            source1.CopyTo<byte>(destination);
            marshaledBytes = ((IBitSize) type).ByteSize;
            flag1 = true;
            break;
          case 1:
            break;
          case 2:
            IDataType baseType = ((IAliasType) type).BaseType;
            if (baseType != null)
            {
              object val1 = !(val is IEnumValue) ? val : ((IEnumValue) val).Primitive;
              marshaledBytes = this.Marshal(baseType, encoding, val1, destination);
              flag1 = true;
              break;
            }
            break;
          case 3:
            IArrayType arrayType = (IArrayType) type;
            IDataType elementType = arrayType.ElementType;
            if (val.GetType().IsArray && elementType != null)
            {
              Array array = (Array) val;
              Type type1 = val.GetType();
              type1.GetElementType();
              type1.GetArrayRank();
              PrimitiveTypeMarshaler.GetArrayBounds(array, out int[] _, out int[] _);
              ArrayIndexIterator arrayIndexIterator = new ArrayIndexIterator(arrayType, true);
              bool flag4 = true;
              foreach (int[] numArray in arrayIndexIterator)
              {
                object val2 = ((Array) val).GetValue(numArray);
                int marshaledBytes1 = 0;
                flag4 &= this.TryMarshal(elementType, encoding, val2, destination.Slice(marshaledBytes, ((IBitSize) elementType).ByteSize), out marshaledBytes1);
                if (flag4)
                  marshaledBytes += marshaledBytes1;
                else
                  break;
              }
              flag1 = flag4;
              break;
            }
            break;
          case 4:
            Type type2 = val.GetType();
            if (val is byte[])
            {
              byte[] source2 = (byte[]) val;
              if (source2.Length == ((IBitSize) type).ByteSize)
              {
                source2.CopyTo<byte>(destination);
                marshaledBytes = ((IBitSize) type).ByteSize;
                flag1 = true;
                break;
              }
              break;
            }
            if (val is ICustomMarshaler)
            {
              ICustomMarshaler customMarshaler = (ICustomMarshaler) val;
              int nativeDataSize = customMarshaler.GetNativeDataSize();
              if (nativeDataSize == ((IBitSize) type).ByteSize)
              {
                IntPtr native = customMarshaler.MarshalManagedToNative(val);
                new Span<byte>(native.ToPointer(), nativeDataSize).CopyTo(destination);
                customMarshaler.CleanUpNativeData(native);
                marshaledBytes = nativeDataSize;
                flag1 = true;
                break;
              }
              break;
            }
            if (type2.Attributes.HasFlag((Enum) TypeAttributes.ExplicitLayout) || type2.Attributes.HasFlag((Enum) TypeAttributes.SequentialLayout))
            {
              int num = System.Runtime.InteropServices.Marshal.SizeOf(val);
              if (num == ((IBitSize) type).ByteSize)
              {
                fixed (byte* numPtr = &MemoryMarshal.GetReference<byte>(destination))
                  System.Runtime.InteropServices.Marshal.StructureToPtr(val, new IntPtr((void*) numPtr), false);
                marshaledBytes = num;
                flag1 = true;
                break;
              }
              break;
            }
            break;
          default:
            if (category == 13 || category - 15 <= 1)
            {
              byte[] source3 = new byte[((IBitSize) type).ByteSize];
              if (((IBitSize) type).ByteSize == 4)
                source3 = BitConverter.GetBytes(((IConvertible) val).ToUInt32((IFormatProvider) null));
              else if (((IBitSize) type).ByteSize == 8)
                source3 = BitConverter.GetBytes(((IConvertible) val).ToUInt64((IFormatProvider) null));
              source3.CopyTo<byte>(destination);
              marshaledBytes = ((IBitSize) type).ByteSize;
              flag1 = true;
              break;
            }
            break;
        }
      }
      return flag1;
    }

    /// <summary>Marshals the value/type to span/memory.</summary>
    /// <param name="type">The type.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value.</param>
    /// <param name="destination">The destination memory.</param>
    /// <returns>The number of marshalled bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">type</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">value</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">type</exception>
    public int Marshal(IDataType type, Encoding encoding, object? val, Span<byte> destination)
    {
      int marshaledBytes = 0;
      if (!this.TryMarshal(type, encoding, val, destination, out marshaledBytes))
        throw new MarshalException(type, val);
      return marshaledBytes;
    }

    /// <summary>Marshals the specified pointer type.</summary>
    /// <param name="pointerType">Type of the pointer.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="val">The value.</param>
    /// <param name="lengthIs">The length is.</param>
    /// <param name="destination">The destination.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    public int Marshal(
      IPointerType pointerType,
      Encoding encoding,
      object? val,
      int lengthIs,
      Span<byte> destination)
    {
      int marshaledBytes = 0;
      if (!this.TryMarshal(pointerType, encoding, val, lengthIs, destination, out marshaledBytes))
        throw new MarshalException((IDataType) pointerType, val);
      return marshaledBytes;
    }

    /// <summary>Marshals the value to memory/span.</summary>
    /// <param name="val">The value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="destination">The destination memory/span.</param>
    /// <returns>The number of marshalled bytes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">tp</exception>
    public int Marshal(object val, Encoding encoding, Span<byte> destination)
    {
      int size = 0;
      if (!this.TryMarshal(val, encoding, destination, out size))
        throw new ArgumentOutOfRangeException(nameof (val), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Managed datatype '{0}' not supported!", (object) val.GetType()));
      return size;
    }

    /// <summary>Marshals the value to memory/span.</summary>
    /// <param name="val">The value.</param>
    /// <param name="destination">The destination memory/span.</param>
    /// <returns>The number of marshalled bytes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">tp</exception>
    public int Marshal(object val, Span<byte> destination) => this.Marshal(val, this._encoding, destination);

    /// <summary>
    /// Converts the specified source value to the specified target type.
    /// </summary>
    /// <param name="sourceValue">The source value.</param>
    /// <returns>Value as targetType (System.Object).</returns>
    /// <exception cref="T:System.ArgumentNullException">tp</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    public static T Convert<T>(object sourceValue) => (T) PrimitiveTypeMarshaler.Convert(sourceValue, typeof (T));

    /// <summary>
    /// Converts the specified source value to the specified target type.
    /// </summary>
    /// <param name="sourceValue">The source value.</param>
    /// <param name="targetType">Target type.</param>
    /// <returns>Value as targetType (System.Object).</returns>
    /// <exception cref="T:System.ArgumentNullException">tp</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException"></exception>
    public static object Convert(object sourceValue, Type targetType)
    {
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetType));
      Type type = sourceValue != null ? sourceValue.GetType() : throw new ArgumentNullException(nameof (sourceValue));
      if (type == targetType)
        return sourceValue;
      object targetValue;
      if (!PrimitiveTypeMarshaler.TryConvert(sourceValue, targetType, out targetValue))
        throw new AdsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert value '{0}' to type '{1}'!", (object) type.Name, (object) targetType.Name));
      return targetValue;
    }

    /// <summary>
    /// Determines whether this instance can convert the specified source value.
    /// </summary>
    /// <param name="sourceValue">The source value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <returns><c>true</c> if this instance can convert the specified source value; otherwise, <c>false</c>.</returns>
    public static bool CanConvert(object sourceValue, Type targetType)
    {
      if (sourceValue == null)
        throw new ArgumentNullException(nameof (sourceValue));
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetType));
      object targetValue = (object) null;
      return PrimitiveTypeMarshaler.TryConvert(sourceValue, targetType, out targetValue);
    }

    /// <summary>
    /// Determines whether this instance can convert the specified source value.
    /// </summary>
    /// <param name="sourceValue">The source value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <returns><c>true</c> if this instance can convert the specified source value; otherwise, <c>false</c>.</returns>
    public static bool CanConvert(object sourceValue, IDataType targetType)
    {
      if (sourceValue == null)
        throw new ArgumentNullException(nameof (sourceValue));
      if (targetType == null)
        throw new ArgumentNullException(nameof (targetType));
      object targetValue = (object) null;
      return PrimitiveTypeMarshaler.TryConvert(sourceValue, targetType, out targetValue);
    }

    /// <summary>
    /// Try to convert the specified source value to the specified target type.
    /// </summary>
    /// <param name="sourceValue">The source value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="targetValue">The target value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryConvert(object sourceValue, IDataType targetType, [NotNullWhen(true)] out object? targetValue)
    {
      if (sourceValue == null)
        throw new ArgumentNullException(nameof (sourceValue));
      if (targetType == null)
        throw new ArgumentNullException(nameof (targetType));
      if (!targetType.IsPrimitive)
        throw new ArgumentOutOfRangeException(nameof (targetType));
      targetValue = (object) null;
      IDataType idataType = targetType.Resolve();
      if (sourceValue is string && idataType.Category == 3)
      {
        IConvertible sourceValue1 = (IConvertible) null;
        if (((IEnumType) idataType).TryParse((string) sourceValue, ref sourceValue1))
          return PrimitiveTypeMarshaler.TryConvert((object) sourceValue1, targetType, out targetValue);
      }
      else
      {
        Type managedType = targetType.GetManagedType();
        if (managedType != (Type) null)
          return PrimitiveTypeMarshaler.TryConvert(sourceValue, managedType, out targetValue);
      }
      return false;
    }

    /// <summary>
    /// Try to convert the specified source value to the specified target type.
    /// </summary>
    /// <param name="sourceValue">The source value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="targetValue">The target value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static unsafe bool TryConvert(
      object sourceValue,
      Type targetType,
      [NotNullWhen(true)] out object? targetValue)
    {
      if (sourceValue == null)
        throw new ArgumentNullException(nameof (sourceValue));
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetType));
      targetValue = (object) null;
      bool flag1 = true;
      if (sourceValue.GetType() == targetType)
      {
        bool flag2 = true;
        targetValue = sourceValue;
        return flag2;
      }
      if (sourceValue is IConvertible)
      {
        try
        {
          if (targetType == typeof (bool))
            targetValue = (object) ((IConvertible) sourceValue).ToBoolean((IFormatProvider) null);
          else if (targetType == typeof (int))
            targetValue = (object) ((IConvertible) sourceValue).ToInt32((IFormatProvider) null);
          else if (targetType == typeof (short))
            targetValue = (object) ((IConvertible) sourceValue).ToInt16((IFormatProvider) null);
          else if (targetType == typeof (byte))
            targetValue = (object) ((IConvertible) sourceValue).ToByte((IFormatProvider) null);
          else if (targetType == typeof (float))
            targetValue = (object) ((IConvertible) sourceValue).ToSingle((IFormatProvider) null);
          else if (targetType == typeof (double))
            targetValue = (object) ((IConvertible) sourceValue).ToDouble((IFormatProvider) null);
          else if (targetType == typeof (long))
            targetValue = (object) ((IConvertible) sourceValue).ToInt64((IFormatProvider) null);
          else if (targetType == typeof (ulong))
            targetValue = (object) ((IConvertible) sourceValue).ToUInt64((IFormatProvider) null);
          else if (targetType == typeof (uint))
            targetValue = (object) ((IConvertible) sourceValue).ToUInt32((IFormatProvider) null);
          else if (targetType == typeof (ushort))
            targetValue = (object) ((IConvertible) sourceValue).ToUInt16((IFormatProvider) null);
          else if (targetType == typeof (sbyte))
            targetValue = (object) ((IConvertible) sourceValue).ToSByte((IFormatProvider) null);
          else if (targetType == typeof (string))
            targetValue = (object) sourceValue.ToString();
          else if (targetType.IsEnum)
            targetValue = Enum.ToObject(targetType, sourceValue);
          else if (targetType == typeof (DATE))
          {
            DATE date = (DATE) null;
            if (PlcOpenDateConverter.TryConvert(sourceValue, ref date))
            {
              targetValue = (object) date;
            }
            else
            {
              targetValue = (object) null;
              flag1 = false;
            }
          }
          else if (targetType == typeof (TIME))
          {
            TIME time = (TIME) null;
            if (PlcOpenTimeConverter.TryConvert(sourceValue, ref time))
            {
              targetValue = (object) time;
            }
            else
            {
              targetValue = (object) null;
              flag1 = false;
            }
          }
          else if (targetType == typeof (LTIME))
          {
            LTIME ltime = (LTIME) null;
            if (PlcOpenTimeConverter.TryConvert(sourceValue, ref ltime))
            {
              targetValue = (object) ltime;
            }
            else
            {
              targetValue = (object) null;
              flag1 = false;
            }
          }
          else if (targetType == typeof (DT))
          {
            DT dt = (DT) null;
            if (PlcOpenDTConverter.TryConvert(sourceValue, ref dt))
            {
              targetValue = (object) dt;
            }
            else
            {
              targetValue = (object) null;
              flag1 = false;
            }
          }
          else if (targetType == typeof (TOD))
          {
            TOD tod = (TOD) null;
            if (PlcOpenTODConverter.TryConvert(sourceValue, ref tod))
            {
              targetValue = (object) tod;
            }
            else
            {
              targetValue = (object) null;
              flag1 = false;
            }
          }
          else
          {
            targetValue = (object) null;
            flag1 = false;
          }
        }
        catch (FormatException ex)
        {
          targetValue = (object) null;
          flag1 = false;
        }
      }
      else if (sourceValue.GetType() == typeof (BitArray))
        targetValue = BitTypeConverter.ToNumeric(targetType, (BitArray) sourceValue);
      else if (sourceValue.GetType().IsArray && targetType.IsArray)
      {
        flag1 = PrimitiveTypeMarshaler.TryCastArray((Array) sourceValue, targetType, out targetValue);
      }
      else
      {
        switch (sourceValue)
        {
          case IEnumValue _:
            flag1 = PrimitiveTypeMarshaler.TryConvert(((IEnumValue) sourceValue).Primitive, targetType, out targetValue);
            break;
          case TimeBase _:
            flag1 = PlcOpenTimeConverter.TryConvert((TimeBase) sourceValue, targetType, ref targetValue);
            break;
          case DateBase _:
            flag1 = PlcOpenDateConverter.TryConvert((DateBase) sourceValue, targetType, ref targetValue);
            break;
          case LTimeBase _:
            flag1 = PlcOpenTimeConverter.TryConvert((LTimeBase) sourceValue, targetType, ref targetValue);
            break;
          case DateTimeOffset _ when targetType == typeof (DateTime):
            targetValue = (object) ((DateTimeOffset) sourceValue).DateTime;
            flag1 = true;
            break;
          case DateTime _ when targetType == typeof (DateTimeOffset):
            targetValue = (object) new DateTimeOffset((DateTime) sourceValue);
            flag1 = true;
            break;
          case byte[] _ when targetType.Attributes.HasFlag((Enum) TypeAttributes.ExplicitLayout) || targetType.Attributes.HasFlag((Enum) TypeAttributes.SequentialLayout):
            int num = System.Runtime.InteropServices.Marshal.SizeOf(targetType);
            byte[] array = (byte[]) sourceValue;
            if (num == array.Length)
            {
              fixed (byte* numPtr = &MemoryMarshal.GetReference<byte>(array.AsSpan<byte>()))
              {
                targetValue = System.Runtime.InteropServices.Marshal.PtrToStructure(new IntPtr((void*) numPtr), targetType);
                flag1 = true;
              }
              break;
            }
            break;
          default:
            if (targetType == typeof (TIME))
            {
              TIME time;
              flag1 = PlcOpenTimeConverter.TryConvert(sourceValue, ref time);
              targetValue = (object) time;
              break;
            }
            if (targetType == typeof (LTIME))
            {
              LTIME ltime;
              flag1 = PlcOpenTimeConverter.TryConvert(sourceValue, ref ltime);
              targetValue = (object) ltime;
              break;
            }
            if (targetType == typeof (TOD))
            {
              TOD tod;
              flag1 = PlcOpenTODConverter.TryConvert(sourceValue, ref tod);
              targetValue = (object) tod;
              break;
            }
            if (targetType == typeof (DATE))
            {
              DATE date;
              flag1 = PlcOpenDateConverter.TryConvert(sourceValue, ref date);
              targetValue = (object) date;
              break;
            }
            if (targetType == typeof (DT))
            {
              DT dt;
              flag1 = PlcOpenDTConverter.TryConvert(sourceValue, ref dt);
              targetValue = (object) dt;
              break;
            }
            targetValue = (object) null;
            flag1 = false;
            break;
        }
      }
      return targetValue != null;
    }

    /// <summary>Gets the array bounds.</summary>
    /// <param name="array">The array.</param>
    /// <param name="lowerBounds">The lower bounds.</param>
    /// <param name="lengths">The lengths.</param>
    internal static void GetArrayBounds(Array array, out int[] lowerBounds, out int[] lengths)
    {
      int arrayRank = array.GetType().GetArrayRank();
      lengths = new int[arrayRank];
      lowerBounds = new int[arrayRank];
      for (int dimension = 0; dimension < arrayRank; ++dimension)
      {
        lengths[dimension] = array.GetLength(dimension);
        lowerBounds[dimension] = array.GetLowerBound(dimension);
      }
    }

    /// <summary>Tries to cast the array.</summary>
    /// <param name="sourceValue">The source value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="targetValue">The target value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// sourceValue
    /// or
    /// targetValue
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">targetType</exception>
    private static unsafe bool TryCastArray(
      Array sourceValue,
      Type targetType,
      [NotNullWhen(true)] out object? targetValue)
    {
      if (sourceValue == null)
        throw new ArgumentNullException(nameof (sourceValue));
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetValue));
      if (!targetType.IsArray)
        throw new ArgumentOutOfRangeException(nameof (targetType));
      Type type = sourceValue.GetType();
      Array array1 = sourceValue;
      Type elementType1 = type.GetElementType();
      Type elementType2 = targetType.GetElementType();
      targetValue = (object) null;
      if (elementType1 == elementType2)
      {
        targetValue = (object) sourceValue;
        return true;
      }
      if (elementType1 == typeof (byte) && (elementType2.Attributes.HasFlag((Enum) TypeAttributes.ExplicitLayout) || elementType2.Attributes.HasFlag((Enum) TypeAttributes.SequentialLayout)))
      {
        byte[] array2 = (byte[]) sourceValue;
        int length1 = sourceValue.Length;
        int length2 = System.Runtime.InteropServices.Marshal.SizeOf(elementType2);
        int length3 = length1 / length2;
        if (length1 % length2 != 0)
          return false;
        Array instance = Array.CreateInstance(elementType2, length3);
        for (int index = 0; index < length3; ++index)
        {
          fixed (byte* numPtr = &MemoryMarshal.GetReference<byte>(array2.AsSpan<byte>(index * length2, length2)))
          {
            object structure = System.Runtime.InteropServices.Marshal.PtrToStructure(new IntPtr((void*) numPtr), elementType2);
            instance.SetValue(structure, index);
          }
        }
        targetValue = (object) instance;
        return true;
      }
      int[] lengths;
      PrimitiveTypeMarshaler.GetArrayBounds(array1, out int[] _, out lengths);
      targetValue = (object) Array.CreateInstance(elementType2, lengths);
      for (int index = 0; index < array1.Length; ++index)
      {
        object targetValue1;
        if (PrimitiveTypeMarshaler.TryConvert(array1.GetValue(index), elementType2, out targetValue1))
        {
          ((Array) targetValue).SetValue(targetValue1, index);
        }
        else
        {
          targetValue = (object) null;
          return false;
        }
      }
      return true;
    }

    /// <summary>Marshals the specified string value.</summary>
    /// <param name="value">The value.</param>
    /// <param name="destination">The destination span/memory.</param>
    /// <returns>Number of marshalled bytes.</returns>
    public int Marshal(string value, Span<byte> destination) => this._stringMarshaler.Marshal(value, destination);

    /// <summary>Unmarshals a string from memory/span.</summary>
    /// <param name="source">The source memory/span.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="value">The unmarshaled value.</param>
    /// <returns>The number of unmarshaled bytes.</returns>
    public int Unmarshal(ReadOnlySpan<byte> source, Encoding encoding, out string value) => this._stringMarshaler.Unmarshal(source, encoding, ref value);

    /// <summary>Unmarshals a string from memory/span.</summary>
    /// <param name="source">The source memory/span.</param>
    /// <param name="value">The unmarshaled value.</param>
    /// <returns>The number of unmarshaled bytes.</returns>
    public int Unmarshal(ReadOnlySpan<byte> source, out string value) => this.Unmarshal(source, this._encoding, out value);

    /// <summary>Gets the marshal size of the specified string type.</summary>
    /// <param name="stringType">Type of the string.</param>
    /// <returns>Marshalling size of the string</returns>
    public int MarshalSize(IStringType stringType) => this._stringMarshaler.MarshalSize(stringType);

    /// <summary>Marshals the specified string type.</summary>
    /// <param name="stringType">Type of the string.</param>
    /// <param name="value">The value.</param>
    /// <param name="destination">The destination.</param>
    /// <returns>System.Int32.</returns>
    public int Marshal(IStringType stringType, string value, Span<byte> destination) => this._stringMarshaler.Marshal(stringType, value, destination);

    /// <summary>Unmarshals the specified string type.</summary>
    /// <param name="stringType">Type of the string.</param>
    /// <param name="source">The source memory/span.</param>
    /// <param name="value">The unmarshaled string value.</param>
    /// <returns>The number of unmarshalled bytes.</returns>
    public int Unmarshal(IStringType stringType, ReadOnlySpan<byte> source, out string value) => this._stringMarshaler.Unmarshal(stringType, source, ref value);
  }
}
