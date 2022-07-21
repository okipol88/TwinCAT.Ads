// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.RpcMarshaler
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using TwinCAT.Ads.TypeSystem;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class RpcMarshaller base implementation (abstract)</summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.IRpcMarshal" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class RpcMarshaler : IRpcMarshal
  {
    /// <summary>The resolver</summary>
    private IDataTypeResolver _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.RpcMarshaler" /> class.
    /// </summary>
    /// <param name="dataTypeResolver">The data type resolver.</param>
    public RpcMarshaler(IDataTypeResolver dataTypeResolver) => this._resolver = dataTypeResolver;

    private void validateLengthIsParameter(
      IRpcMethod method,
      IRpcMethodParameter paraWithLengthIs,
      object[] parameterValues)
    {
      int index = paraWithLengthIs.LengthIsParameterIndex - 1;
      if (index < 0 || index >= ((ICollection<IRpcMethodParameter>) method.Parameters).Count)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
        interpolatedStringHandler.AppendLiteral("Parameter '");
        interpolatedStringHandler.AppendFormatted(paraWithLengthIs.Name);
        interpolatedStringHandler.AppendLiteral("' of RPC Method '");
        interpolatedStringHandler.AppendFormatted(method.Name);
        interpolatedStringHandler.AppendLiteral("' LengthIs attribute is out of range!");
        throw new ArgumentOutOfRangeException(nameof (paraWithLengthIs), interpolatedStringHandler.ToStringAndClear());
      }
      if (((Enum) (object) ((IList<IRpcMethodParameter>) method.Parameters)[index].ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 2))
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(83, 2);
        interpolatedStringHandler.AppendLiteral("Parameter '");
        interpolatedStringHandler.AppendFormatted(paraWithLengthIs.Name);
        interpolatedStringHandler.AppendLiteral("' of RPC Method '");
        interpolatedStringHandler.AppendFormatted(method.Name);
        interpolatedStringHandler.AppendLiteral("' has dynamic size out-parameter what is not supported!");
        throw new ArgumentOutOfRangeException(nameof (method), interpolatedStringHandler.ToStringAndClear());
      }
      int num = (int) Convert.ChangeType(parameterValues[index], typeof (int));
    }

    /// <summary>
    /// Gets the size of the data of the in parameters in bytes
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="outSpec">The out spec.</param>
    /// <param name="parameterValues">The parameter values.</param>
    /// <param name="inSize">Size of the in.</param>
    /// <param name="outSize">Size of the out.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentNullException">method</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">Cannot resolve type '{typeName}'</exception>
    public virtual void GetParameterMarshallingSize(
      IRpcMethod method,
      AnyTypeSpecifier[]? outSpec,
      object[]? parameterValues,
      out int inSize,
      out int outSize)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      inSize = 0;
      outSize = 0;
      for (int index = 0; index < ((ICollection<IRpcMethodParameter>) method.Parameters).Count; ++index)
      {
        IRpcMethodParameter parameter = ((IList<IRpcMethodParameter>) method.Parameters)[index];
        IDataType idataType = (IDataType) null;
        string typeName = ((IList<IRpcMethodParameter>) method.Parameters)[index].TypeName;
        if (!this._resolver.TryResolveType(typeName, ref idataType))
          throw new DataTypeException("Cannot resolve type '" + typeName + "'", typeName);
        if (((Enum) (object) parameter.ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 1))
          inSize += this.OnGetParameterSize(index, method.Parameters, parameterValues);
        if (((Enum) (object) parameter.ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 2))
        {
          if (parameter.HasLengthIsParameter)
            this.validateLengthIsParameter(method, parameter, parameterValues);
          outSize += this.OnGetParameterSize(index, method.Parameters, parameterValues);
        }
      }
    }

    /// <summary>Gets the size of the input marshalling data in bytes.</summary>
    /// <param name="method">The method.</param>
    /// <param name="parameterValues">The parameter values.</param>
    /// <returns>System.Int32.</returns>
    public virtual int GetInMarshallingSize(IRpcMethod method, object[] parameterValues)
    {
      int inSize = 0;
      int outSize = 0;
      this.GetParameterMarshallingSize(method, (AnyTypeSpecifier[]) null, parameterValues, out inSize, out outSize);
      return inSize;
    }

    public virtual int GetOutMarshallingSize(
      IRpcMethod method,
      AnyTypeSpecifier[]? outSpec,
      AnyTypeSpecifier? retSpec,
      object[]? parameterValues)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      int inSize = 0;
      int outSize = 0;
      this.GetParameterMarshallingSize(method, outSpec, parameterValues, out inSize, out outSize);
      if (!string.IsNullOrEmpty(method.ReturnType))
        outSize += method.ReturnTypeSize;
      return outSize;
    }

    /// <summary>Gets the Marshalling Size of the DataType</summary>
    /// <param name="dataType">Type of the data.</param>
    /// <returns>System.Int32.</returns>
    protected virtual int OnGetMarshallingSize(string dataType)
    {
      IDataType idataType = (IDataType) null;
      if (!this._resolver.TryResolveType(dataType, ref idataType))
        throw new MarshalException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "DataType '{0}' cannot be resolved!", (object) dataType));
      return ((IBitSize) idataType).Size;
    }

    /// <summary>Get the parameter size of a single parameter</summary>
    /// <param name="parameterIndex">Index of the parameter over all parameters.</param>
    /// <param name="parameters">The parameters (in or out parameters)</param>
    /// <param name="parameterValues">The parameter values.</param>
    /// <returns>Parameter Byte size</returns>
    /// <exception cref="T:System.ArgumentNullException">parameters</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException">TcRpcLengthIs parameter mismatch. Cannot find [IN] parameter '{parameter.LengthIsParameterIndex}' for parameter '{parameter}'!</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException">Cannot marshal PCCH type. Value is not a string!</exception>
    /// <exception cref="T:TwinCAT.TypeSystem.MarshalException">Cannot marshal PCCH type without LengthIs attribute!</exception>
    /// <remarks>This default implementation solely returns the Byte size of the parameter</remarks>
    public int OnGetParameterSize(
      int parameterIndex,
      IRpcMethodParameterCollection parameters,
      object[]? parameterValues)
    {
      RpcMethodParameter rpcMethodParameter = parameters != null ? (RpcMethodParameter) ((IList<IRpcMethodParameter>) parameters)[parameterIndex] : throw new ArgumentNullException(nameof (parameters));
      int parameterSize;
      if (rpcMethodParameter.HasLengthIsParameter && parameterValues != null)
      {
        RpcMethodParameter lengthIsParameter = (RpcMethodParameter) parameters.GetLengthIsParameter((IRpcMethodParameter) rpcMethodParameter);
        if (parameterValues.Length < rpcMethodParameter.LengthIsParameterIndex)
        {
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(81, 2);
          interpolatedStringHandler.AppendLiteral("TcRpcLengthIs parameter mismatch. Cannot find [IN] parameter '");
          interpolatedStringHandler.AppendFormatted<int>(rpcMethodParameter.LengthIsParameterIndex);
          interpolatedStringHandler.AppendLiteral("' for parameter '");
          interpolatedStringHandler.AppendFormatted<RpcMethodParameter>(rpcMethodParameter);
          interpolatedStringHandler.AppendLiteral("'!");
          throw new MarshalException(interpolatedStringHandler.ToStringAndClear());
        }
        object parameterValue = parameterValues[rpcMethodParameter.LengthIsParameterIndex - 1];
        if (lengthIsParameter != null && parameterValue != null)
        {
          int num = PrimitiveTypeMarshaler.Convert<int>(parameterValue);
          parameterSize = rpcMethodParameter.Size * num;
        }
        else
          parameterSize = rpcMethodParameter.Size;
      }
      else if (rpcMethodParameter.TypeName == "PCCH" && parameterValues != null)
      {
        if (!((Enum) (object) rpcMethodParameter.ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 1))
          throw new MarshalException("Cannot marshal PCCH type without LengthIs attribute!");
        StringMarshaler stringMarshaler = new StringMarshaler(Encoding.UTF8, (StringConvertMode) 2);
        if (!(parameterValues[parameterIndex] is string parameterValue))
          throw new MarshalException("Cannot marshal PCCH type. Value is not a string!");
        parameterSize = stringMarshaler.MarshalSize(parameterValue);
      }
      else
        parameterSize = rpcMethodParameter.Size;
      return parameterSize;
    }

    /// <summary>Gets the in parameters.</summary>
    /// <param name="method">The method.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="types">The types.</param>
    /// <returns>Number of returned parameters.</returns>
    protected int GetInParameters(
      IRpcMethod method,
      out RpcMethodParameterCollection? parameters,
      out IList<IDataType>? types)
    {
      return this.GetParameters(method, (MethodParamFlags) 1, out parameters, out types);
    }

    /// <summary>Gets the out parameters.</summary>
    /// <param name="method">The method.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="types">The types.</param>
    /// <returns>Number of returned parameters.</returns>
    protected int GetOutParameters(
      IRpcMethod method,
      out RpcMethodParameterCollection? parameters,
      out IList<IDataType>? types)
    {
      return this.GetParameters(method, (MethodParamFlags) 2, out parameters, out types);
    }

    /// <summary>Gets the type of the parameter.</summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>IDataType.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">DataType '{parameter.TypeName}' could not be resolved!</exception>
    protected IDataType GetType(IRpcMethodParameter parameter) => this.GetType(parameter.TypeName);

    /// <summary>Gets the type of the parameter.</summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns>IDataType.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.DataTypeException">DataType '{typeName}' could not be resolved!</exception>
    protected IDataType GetType(string typeName)
    {
      IDataType idataType = (IDataType) null;
      if (this._resolver.TryResolveType(typeName, ref idataType) && idataType is IResolvableType iresolvableType)
        idataType = iresolvableType.ResolveType((DataTypeResolveStrategy) 1);
      return idataType != null ? idataType : throw new DataTypeException("DataType '" + typeName + "' could not be resolved!", typeName);
    }

    /// <summary>Gets the parameters.</summary>
    /// <param name="method">The method.</param>
    /// <param name="mask">The mask.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="types">The types.</param>
    /// <returns>Number of returned parameters.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">parameters</exception>
    protected int GetParameters(
      IRpcMethod method,
      MethodParamFlags mask,
      out RpcMethodParameterCollection? parameters,
      out IList<IDataType>? types)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      types = (IList<IDataType>) new List<IDataType>();
      parameters = new RpcMethodParameterCollection();
      for (int index = 0; index < ((ICollection<IRpcMethodParameter>) method.Parameters).Count; ++index)
      {
        if ((((IList<IRpcMethodParameter>) method.Parameters)[index].ParameterFlags & mask) > 0)
          this.GetType(((IList<IRpcMethodParameter>) method.Parameters)[index]);
      }
      return ((ICollection<IDataType>) types).Count;
    }

    /// <summary>
    /// Marshals the Method Identification plus all In parameters into the In- (Write-) buffer
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="values">The parameter values.</param>
    /// <param name="destination">The destination memory.</param>
    /// <returns>Number of marshalled bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">method
    /// or
    /// parameterValues
    /// or
    /// data</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">method
    /// or
    /// parameterValues
    /// or
    /// data</exception>
    public int MarshalInParameters(IRpcMethod method, object[] values, Span<byte> destination)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (destination == (Span<byte>) (byte[]) null)
        throw new ArgumentNullException(nameof (destination));
      if (((ICollection<IRpcMethodParameter>) method.InParameters).Count > 0)
      {
        if (values == null)
          throw new ArgumentNullException(nameof (values));
        if (values.Length != ((ICollection<IRpcMethodParameter>) method.Parameters).Count)
          throw new ArgumentOutOfRangeException(nameof (values));
      }
      return this.OnMarshalInParameters(method, values, destination);
    }

    /// <summary>Unmarshals the Out-Parameters from the Source data.</summary>
    /// <param name="method">The method.</param>
    /// <param name="outSpec">The Output-Parameter specifications (for ANY_TYPE marshalling), NULL if marshalled by symbolic information.</param>
    /// <param name="source">The unmarshalled source data of the Out-Parameters.</param>
    /// <param name="parameterValues">The parameter values (used for potential LengthIs parameters)</param>
    /// <returns>Count of unmarshalled bytes.</returns>
    /// <remarks>This is used to Unmarshal the Out-Parameters on the Client side of the Rpc call.</remarks>
    public int UnmarshalOutParameters(
      IRpcMethod method,
      AnyTypeSpecifier[]? outSpec,
      ReadOnlySpan<byte> source,
      object[] parameterValues)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (parameterValues == null)
        throw new ArgumentNullException(nameof (parameterValues));
      if (((ICollection<IRpcMethodParameter>) method.Parameters).Count != parameterValues.Length)
        throw new ArgumentOutOfRangeException(nameof (parameterValues));
      return this.OnUnmarshalOutParameters(method, outSpec, source, parameterValues);
    }

    /// <summary>Marshals the Out-Parameters into the target data.</summary>
    /// <param name="method">The RPC method</param>
    /// <param name="parameterValues">The In-Parameter values (used for potential LengthIs parameters)</param>
    /// <param name="outSpec">The Output-Parameter specifications (for ANY_TYPE marshalling), NULL if marshalled by symbolic information.</param>
    /// <param name="target">The Out-Data created from parameters.</param>
    /// <returns>System.Int32.</returns>
    public int MarshalOutParameters(
      IRpcMethod method,
      object[] parameterValues,
      AnyTypeSpecifier[]? outSpec,
      Span<byte> target)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (parameterValues == null)
        throw new ArgumentNullException(nameof (parameterValues));
      if (((ICollection<IRpcMethodParameter>) method.Parameters).Count != parameterValues.Length)
        throw new ArgumentOutOfRangeException(nameof (parameterValues));
      return this.OnMarshalOutParameters(method, parameterValues, outSpec, target);
    }

    /// <summary>
    /// Unmarshals the in parameters from buffer to object values
    /// </summary>
    /// <param name="method">The Rpc Method.</param>
    /// <param name="source">The incoming data</param>
    /// <param name="parameterValues">The parameter values.</param>
    /// <returns>Count of Unmarshaled bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">method</exception>
    /// <exception cref="T:System.ArgumentNullException">parameterValues</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">parameterValues</exception>
    /// <remarks>This is used on the Server side of the RpcInvoke call.</remarks>
    public int UnmarshalInParameters(
      IRpcMethod method,
      ReadOnlySpan<byte> source,
      object[] parameterValues)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (parameterValues == null)
        throw new ArgumentNullException(nameof (parameterValues));
      if (((ICollection<IRpcMethodParameter>) method.Parameters).Count != parameterValues.Length)
        throw new ArgumentOutOfRangeException(nameof (parameterValues));
      return this.OnUnmarshalInParameters(method, source, parameterValues);
    }

    /// <summary>Handler function for marshalling the In Parameters.</summary>
    /// <param name="method">The method.</param>
    /// <param name="parameterValues">The parameter values.</param>
    /// <param name="destination">The destination memory.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">parameterValues</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">parameterValues</exception>
    protected virtual int OnMarshalInParameters(
      IRpcMethod method,
      object[] parameterValues,
      Span<byte> destination)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (parameterValues == null)
        throw new ArgumentNullException(nameof (parameterValues));
      if (((ICollection<IRpcMethodParameter>) method.Parameters).Count != parameterValues.Length)
        throw new ArgumentOutOfRangeException(nameof (parameterValues));
      int start = 0;
      for (int index = 0; index < parameterValues.Length; ++index)
      {
        IRpcMethodParameter parameter = ((IList<IRpcMethodParameter>) method.Parameters)[index];
        if (((Enum) (object) parameter.ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 1))
        {
          object parameterValue = parameterValues[index];
          int parameterSize = this.OnGetParameterSize(parameter, (AnyTypeSpecifier) null, parameterValues);
          start += this.OnMarshalParameter(parameter, (AnyTypeSpecifier) null, parameterValue, parameterSize, destination.Slice(start, parameterSize));
        }
      }
      return start;
    }

    protected abstract int OnGetParameterSize(
      IRpcMethodParameter param,
      AnyTypeSpecifier? spec,
      object[] parameterValues);

    protected abstract int OnMarshalParameter(
      IRpcMethodParameter parameter,
      AnyTypeSpecifier? spec,
      object value,
      int parameterSize,
      Span<byte> destination);

    /// <summary>Handler function unmarshalling the out parameters.</summary>
    /// <param name="method">The method.</param>
    /// <param name="outSpec">The out spec.</param>
    /// <param name="source">The source memory..</param>
    /// <param name="parameterValues">The parameter values.</param>
    /// <returns>Number of marshalled bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">method</exception>
    /// <exception cref="T:System.ArgumentNullException">parameterValues</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">parameterValues</exception>
    protected virtual int OnUnmarshalOutParameters(
      IRpcMethod method,
      AnyTypeSpecifier[]? outSpec,
      ReadOnlySpan<byte> source,
      object[] parameterValues)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (parameterValues == null)
        throw new ArgumentNullException(nameof (parameterValues));
      if (((ICollection<IRpcMethodParameter>) method.Parameters).Count != parameterValues.Length)
        throw new ArgumentOutOfRangeException(nameof (parameterValues));
      int start = 0;
      int index1 = 0;
      for (int index2 = 0; index2 < ((ICollection<IRpcMethodParameter>) method.Parameters).Count; ++index2)
      {
        if (((Enum) (object) ((IList<IRpcMethodParameter>) method.Parameters)[index2].ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 2))
        {
          AnyTypeSpecifier outSpec1 = outSpec?[index1];
          ++index1;
          start += this.OnUnmarshalOutParameter(index2, method, outSpec1, source.Slice(start), parameterValues);
        }
      }
      return start;
    }

    private int OnMarshalOutParameters(
      IRpcMethod method,
      object[] values,
      AnyTypeSpecifier[]? outSpec,
      Span<byte> target)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      int start = 0;
      int index1 = 0;
      for (int index2 = 0; index2 < ((ICollection<IRpcMethodParameter>) method.Parameters).Count; ++index2)
      {
        IRpcMethodParameter parameter = ((IList<IRpcMethodParameter>) method.Parameters)[index2];
        if (((Enum) (object) parameter.ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 2))
        {
          object obj = values[index2];
          AnyTypeSpecifier spec = (AnyTypeSpecifier) null;
          if (outSpec != null)
            spec = outSpec[index1];
          int parameterSize = this.OnGetParameterSize(parameter, spec, values);
          start += this.OnMarshalParameter(parameter, spec, obj, parameterSize, target.Slice(start, parameterSize));
          ++index1;
        }
      }
      return start;
    }

    /// <summary>Handler function unmarshalling the out parameters.</summary>
    /// <param name="method">The method.</param>
    /// <param name="source">The source memory..</param>
    /// <param name="parameterValues">The values.</param>
    /// <returns>Number of marshalled bytes.</returns>
    /// <exception cref="T:System.ArgumentNullException">method</exception>
    /// <exception cref="T:System.ArgumentNullException">parameterValues</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">parameterValues</exception>
    protected virtual int OnUnmarshalInParameters(
      IRpcMethod method,
      ReadOnlySpan<byte> source,
      object[] parameterValues)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (parameterValues == null)
        throw new ArgumentNullException(nameof (parameterValues));
      if (((ICollection<IRpcMethodParameter>) method.Parameters).Count != parameterValues.Length)
        throw new ArgumentOutOfRangeException(nameof (parameterValues));
      int start = 0;
      for (int index = 0; index < ((ICollection<IRpcMethodParameter>) method.Parameters).Count; ++index)
      {
        IRpcMethodParameter parameter = ((IList<IRpcMethodParameter>) method.Parameters)[index];
        if (((Enum) (object) parameter.ParameterFlags).HasFlag((Enum) (object) (MethodParamFlags) 1))
        {
          this.GetType(parameter);
          start += this.OnUnmarshalInParameter(index, method, source.Slice(start), parameterValues);
        }
      }
      return start;
    }

    public abstract int UnmarshalReturnValue(
      IRpcMethod method,
      AnyTypeSpecifier? returnSpec,
      ReadOnlySpan<byte> buffer,
      out object returnValue);

    public abstract int MarshalReturnValue(
      IRpcMethod method,
      AnyTypeSpecifier? returnSpec,
      object returnValue,
      Span<byte> buffer);

    protected abstract int OnUnmarshalOutParameter(
      int parameterIndex,
      IRpcMethod method,
      AnyTypeSpecifier? outSpec,
      ReadOnlySpan<byte> buffer,
      object[] parameterValues);

    /// <summary>
    /// Unmarshals a single RPC In parameter from the Buffer (used on the Symbolic Server side).
    /// </summary>
    /// <param name="parameterIndex">Index of the parameter.</param>
    /// <param name="method">The method.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="parameterValues">The parameter values.</param>
    /// <returns>System.Int32.</returns>
    protected abstract int OnUnmarshalInParameter(
      int parameterIndex,
      IRpcMethod method,
      ReadOnlySpan<byte> buffer,
      object[] parameterValues);

    public int UnmarshalRpcMethod(
      IRpcMethod method,
      AnyTypeSpecifier[]? outSpec,
      AnyTypeSpecifier? retSpec,
      object[] parameterValues,
      ReadOnlySpan<byte> source,
      out object? returnValue)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      if (parameterValues == null)
        throw new ArgumentNullException(nameof (parameterValues));
      if (((ICollection<IRpcMethodParameter>) method.Parameters).Count != parameterValues.Length)
        throw new ArgumentOutOfRangeException(nameof (parameterValues));
      int start = 0;
      returnValue = (object) null;
      IDataType idataType = (IDataType) null;
      if (method.ReturnTypeSize > 0 && !string.IsNullOrEmpty(method.ReturnType))
      {
        if (!this._resolver.TryResolveType(method.ReturnType, ref idataType))
          throw new CannotResolveDataTypeException(method.ReturnType);
        if (idataType is IResolvableType iresolvableType)
        {
          idataType = iresolvableType.ResolveType((DataTypeResolveStrategy) 0);
          start += this.UnmarshalReturnValue(method, retSpec, source.Slice(start), out returnValue);
        }
      }
      return start + this.UnmarshalOutParameters(method, outSpec, source.Slice(start, source.Length - start), parameterValues);
    }
  }
}
