// Decompiled with JetBrains decompiler
// Type: SymbolRpcMarshaler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TwinCAT.TypeSystem;


#nullable enable
/// <summary>Marshalling class for uploaded Symbols</summary>
/// <exclude />
public class SymbolRpcMarshaler : RpcMarshaler
{
  private PrimitiveTypeMarshaler _marshaler = PrimitiveTypeMarshaler.DefaultFixedLengthString;
  private SymbolicAnyTypeMarshaler _symbolicMarshaler = new SymbolicAnyTypeMarshaler();

  /// <summary>
  /// Initializes a new instance of the <see cref="T:SymbolRpcMarshaler" /> class.
  /// </summary>
  /// <param name="resolver">The resolver.</param>
  public SymbolRpcMarshaler(IDataTypeResolver resolver)
    : base(resolver)
  {
  }

  private int marshalParameter(
    IRpcMethodParameter parameter,
    AnyTypeSpecifier? spec,
    object value,
    int valueSize,
    Span<byte> buffer)
  {
    if (parameter == null)
      throw new ArgumentNullException(nameof (parameter));
    if (buffer == (Span<byte>) (byte[]) null)
      throw new ArgumentNullException(nameof (buffer));
    Encoding encoding = StringMarshaler.DefaultEncoding;
    int num;
    if (spec == null)
    {
      IDataType type = this.GetType(parameter);
      if (type.Category == 10)
        encoding = ((IStringType) type).Encoding;
      if (type.Category == 13)
      {
        IPointerType pointerType = (IPointerType) type;
        if (((IPointerType) type).ReferencedType == null)
          throw new CannotResolveDataTypeException(pointerType.ReferenceTypeName);
        num = this._marshaler.Marshal(pointerType, encoding, value, valueSize, buffer);
      }
      else
        num = this._marshaler.Marshal(type, encoding, value, buffer);
    }
    else
      num = this._marshaler.Marshal(value, encoding, buffer);
    return num;
  }

  protected override int OnMarshalParameter(
    IRpcMethodParameter parameter,
    AnyTypeSpecifier? spec,
    object value,
    int parameterSize,
    Span<byte> destination)
  {
    return this.marshalParameter(parameter, spec, value, parameterSize, destination);
  }

  public override int UnmarshalReturnValue(
    IRpcMethod method,
    AnyTypeSpecifier? returnSpec,
    ReadOnlySpan<byte> source,
    out object returnValue)
  {
    return returnSpec == null ? this._symbolicMarshaler.Unmarshal(this.GetType(method.ReturnType), this._marshaler.DefaultValueEncoding, source.Slice(0, method.ReturnTypeSize), (Type) null, out returnValue) : this._marshaler.Unmarshal(returnSpec, source, this._marshaler.DefaultValueEncoding, out returnValue);
  }

  /// <summary>
  /// Unmarshals a single RPC In parameter from the Buffer (used on the Symbolic Server side).
  /// </summary>
  /// <param name="parameterIndex">Index of the parameter.</param>
  /// <param name="method">The method.</param>
  /// <param name="buffer">The buffer.</param>
  /// <param name="values">The values.</param>
  /// <returns>System.Int32.</returns>
  /// <exception cref="T:System.ArgumentException">Cannot determine LengthIs parameter value! - values</exception>
  /// <exception cref="T:TwinCAT.TypeSystem.MarshalException">Cannot marshal {dataType.Name} as in parameter. Cannot map to managed type!</exception>
  /// <exception cref="T:TwinCAT.TypeSystem.MarshalException">Cannot marshal {dataType.Name} as in parameter. Referenced type not found!</exception>
  protected override int OnUnmarshalInParameter(
    int parameterIndex,
    IRpcMethod method,
    ReadOnlySpan<byte> buffer,
    object[] values)
  {
    int num = 0;
    IRpcMethodParameter parameter = ((IList<IRpcMethodParameter>) method.Parameters)[parameterIndex];
    IDataType type1 = this.GetType(parameter);
    if (parameter.HasLengthIsParameter)
    {
      if (parameter.LengthIsParameterIndex - 1 >= parameterIndex)
        throw new ArgumentException("Cannot determine LengthIs parameter value!", nameof (values));
      int length1 = (int) Convert.ChangeType(values[parameter.LengthIsParameterIndex - 1], typeof (int));
      if (type1.Name == "PCCH")
      {
        string str;
        num = new StringMarshaler(Encoding.UTF8, (StringConvertMode) 2).Unmarshal(buffer.Slice(0, length1), ref str);
        values[parameterIndex] = (object) str;
      }
      else if (type1.Category == 13)
      {
        IDataType referencedType = ((IPointerType) type1).ReferencedType;
        Type type2 = referencedType != null ? referencedType.GetManagedType() : throw new MarshalException("Cannot marshal " + type1.Name + " as in parameter. Referenced type not found!");
        int length2 = length1 * ((IBitSize) referencedType).ByteSize;
        if (!(type2 != (Type) null))
          throw new MarshalException("Cannot marshal " + type1.Name + " as in parameter. Cannot map to managed type!");
        num = this._marshaler.Unmarshal(new AnyTypeSpecifier(type2.MakeArrayType(), new int[1]
        {
          length1
        }), false, buffer.Slice(0, length2), this._marshaler.DefaultValueEncoding, out values[parameterIndex]);
      }
    }
    if (num == 0)
      num = this._symbolicMarshaler.Unmarshal(type1, StringMarshaler.DefaultEncoding, buffer.Slice(0, ((IBitSize) type1).ByteSize), (Type) null, out values[parameterIndex]);
    return num;
  }

  protected override int OnUnmarshalOutParameter(
    int parameterIndex,
    IRpcMethod method,
    AnyTypeSpecifier? outSpec,
    ReadOnlySpan<byte> source,
    object[] values)
  {
    int num = 0;
    IRpcMethodParameter parameter = ((IList<IRpcMethodParameter>) method.Parameters)[parameterIndex];
    IDataType type1 = this.GetType(parameter);
    if (parameter.HasLengthIsParameter)
    {
      int length1 = (int) Convert.ChangeType(values[parameter.LengthIsParameterIndex - 1], typeof (int));
      this.OnGetParameterSize(parameterIndex, method.Parameters, values);
      if (type1.Name == "PCCH")
      {
        string str;
        num = new StringMarshaler(Encoding.UTF8, (StringConvertMode) 2).Unmarshal(source.Slice(0, length1), ref str);
        values[parameterIndex] = (object) str;
      }
      else if (type1.Category == 13)
      {
        IDataType referencedType = ((IPointerType) type1).ReferencedType;
        Type type2 = referencedType != null ? referencedType.GetManagedType() : throw new MarshalException("Cannot marshal " + type1.Name + " as out parameter. Referenced type not found!");
        int length2 = length1 * ((IBitSize) referencedType).ByteSize;
        if (!(type2 != (Type) null))
          throw new MarshalException("Cannot marshal " + type1.Name + " as out parameter. Cannot map to managed type!");
        num = this._marshaler.Unmarshal(new AnyTypeSpecifier(type2.MakeArrayType(), new int[1]
        {
          length1
        }), false, source.Slice(0, length2), this._marshaler.DefaultValueEncoding, out values[parameterIndex]);
      }
    }
    if (num == 0)
      num = outSpec == null ? this._symbolicMarshaler.Unmarshal(type1, this._marshaler.DefaultValueEncoding, source.Slice(0, ((IBitSize) type1).ByteSize), (Type) null, out values[parameterIndex]) : this._marshaler.Unmarshal(outSpec, source, this._marshaler.DefaultValueEncoding, out values[parameterIndex]);
    return num;
  }

  public override int MarshalReturnValue(
    IRpcMethod method,
    AnyTypeSpecifier? retSpec,
    object? returnValue,
    Span<byte> buffer)
  {
    if (method == null)
      throw new ArgumentNullException(nameof (method));
    return retSpec == null ? this._symbolicMarshaler.Marshal(this.GetType(method.ReturnType), this._symbolicMarshaler.DefaultValueEncoding, returnValue, buffer) : this._marshaler.Marshal(returnValue, buffer);
  }

  protected override int OnGetParameterSize(
    IRpcMethodParameter parameter,
    AnyTypeSpecifier? spec,
    object[] parameterValues)
  {
    int parameterSize;
    if (spec != null)
    {
      parameterSize = this._marshaler.MarshalSize(spec, this._marshaler.DefaultValueEncoding);
    }
    else
    {
      IDataType type = this.GetType(parameter);
      parameterSize = ((IBitSize) type).ByteSize;
      if (type.IsReference)
        parameterSize = ((IReferenceType) type).ResolvedByteSize;
      if (parameter.LengthIsParameterIndex > 0)
      {
        int num = PrimitiveTypeMarshaler.Convert<int>(parameterValues[parameter.LengthIsParameterIndex - 1]);
        if (type.IsReference)
          parameterSize = ((IReferenceType) type).ResolvedByteSize * num;
        else if (type.IsPointer)
        {
          IPointerType ipointerType = (IPointerType) type;
          IDataType referencedType = ipointerType.ReferencedType;
          if (referencedType == null)
          {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 2);
            interpolatedStringHandler.AppendLiteral("Cannot resolve dataType '");
            interpolatedStringHandler.AppendFormatted(((IDataType) ipointerType).Name);
            interpolatedStringHandler.AppendLiteral("'. Base type '");
            interpolatedStringHandler.AppendFormatted(ipointerType.ReferenceTypeName);
            interpolatedStringHandler.AppendLiteral("' is not known!");
            throw new DataTypeException(interpolatedStringHandler.ToStringAndClear(), (IDataType) ipointerType);
          }
          parameterSize = ((IBitSize) referencedType).ByteSize * num;
        }
        else
          parameterSize = ((IBitSize) type).ByteSize * num;
      }
    }
    return parameterSize;
  }
}
