// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.RpcMethod
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>RPC Method Description</summary>
  public class RpcMethod : IRpcMethod
  {
    /// <summary>The name of the method</summary>
    private string _name;
    /// <summary>Method parameters.</summary>
    private RpcMethodParameterCollection _parameters;
    private int _returnAlignSize;
    private string _returnType = string.Empty;
    private int _returnTypeSize;
    private int _vTableIndex = -1;
    private string _comment = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.IRpcMethod" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    internal RpcMethod(AdsMethodEntry entry)
    {
      this._name = entry != null ? entry.name : throw new ArgumentNullException(nameof (entry));
      List<IRpcMethodParameter> coll = new List<IRpcMethodParameter>();
      if (entry.parameters != null)
      {
        for (int index = 0; index < entry.parameters.Length; ++index)
        {
          RpcMethodParameter rpcMethodParameter = new RpcMethodParameter(entry.parameters[index]);
          coll.Add((IRpcMethodParameter) rpcMethodParameter);
        }
      }
      this._parameters = new RpcMethodParameterCollection((IEnumerable<IRpcMethodParameter>) coll);
      this._returnAlignSize = (int) entry.returnAlignSize;
      this._returnTypeSize = (int) entry.returnSize;
      this._returnType = entry.returnType;
      this._vTableIndex = (int) entry.vTableIndex;
      this._comment = entry.comment;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.RpcMethod" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public RpcMethod(string name)
      : this(name, (RpcMethodParameterCollection) null, (IDataType) null, (string) null)
    {
    }

    public RpcMethod(string name, RpcMethodParameterCollection? parameters, IDataType? returnType)
      : this(name, parameters, returnType, (string) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.RpcMethod" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="parameters">The parameters.</param>
    public RpcMethod(string name, RpcMethodParameterCollection? parameters)
      : this(name, parameters, (IDataType) null, (string) null)
    {
    }

    public RpcMethod(
      string name,
      RpcMethodParameterCollection? parameters,
      IDataType? returnType,
      string? comment)
    {
      this._name = name;
      this._parameters = parameters ?? RpcMethodParameterCollection.Empty;
      if (returnType != null)
      {
        this._returnType = returnType.Name;
        this._returnAlignSize = ((IBitSize) returnType).ByteSize;
        this._returnTypeSize = ((IBitSize) returnType).ByteSize;
      }
      this._vTableIndex = -1;
      this._comment = comment ?? string.Empty;
    }

    /// <summary>Gets the name of the method</summary>
    /// <value>The name.</value>
    public string Name => this._name;

    /// <summary>
    /// Gets all parameters (In, Out and ref parameters) of the <see cref="T:TwinCAT.TypeSystem.IRpcMethod">.</see>
    /// </summary>
    /// <value>The parameters.</value>
    public IRpcMethodParameterCollection Parameters => (IRpcMethodParameterCollection) this._parameters.AsReadOnly();

    /// <summary>
    /// Gets the In-Parameters of the <see cref="T:TwinCAT.TypeSystem.IRpcMethod" />
    /// </summary>
    /// <value>The In- and Ref-Parameters</value>
    public IRpcMethodParameterCollection InParameters => (IRpcMethodParameterCollection) new RpcMethodParameterCollection(((IEnumerable<IRpcMethodParameter>) this._parameters).Where<IRpcMethodParameter>((Func<IRpcMethodParameter, bool>) (p => p.IsInput))).AsReadOnly();

    /// <summary>
    /// Gets the Out-Parameters of the <see cref="T:TwinCAT.TypeSystem.IRpcMethod" />
    /// </summary>
    /// <value>The In- and Ref-Parameters</value>
    public IRpcMethodParameterCollection OutParameters => (IRpcMethodParameterCollection) new RpcMethodParameterCollection(((IEnumerable<IRpcMethodParameter>) this._parameters).Where<IRpcMethodParameter>((Func<IRpcMethodParameter, bool>) (p => p.IsOutput))).AsReadOnly();

    /// <summary>
    /// Gets the size of the biggest element in bytes for Alignment
    /// </summary>
    /// <value>The size of the return align.</value>
    public int ReturnAlignSize => this._returnAlignSize;

    /// <summary>Gets the return type.</summary>
    /// <value>Return type.</value>
    public string ReturnType => this._returnType;

    /// <summary>Gets the Byte size of the return type.</summary>
    /// <value>The size of the return type.</value>
    public int ReturnTypeSize => this._returnTypeSize;

    /// <summary>Gets the V-table index of the method.</summary>
    /// <value>The index of the v table.</value>
    public int VTableIndex => this._vTableIndex;

    /// <summary>Gets the Method comment.</summary>
    /// <value>The comment.</value>
    public string Comment => this._comment;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:TwinCAT.TypeSystem.IRpcMethod" /> has no return parameter
    /// </summary>
    /// <value><c>true</c> if this instance is void; otherwise, <c>false</c>.</value>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public bool IsVoid => this._returnTypeSize == 0;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
    public override string ToString()
    {
      List<string> values = new List<string>();
      DefaultInterpolatedStringHandler interpolatedStringHandler;
      foreach (RpcMethodParameter parameter in this._parameters)
      {
        string str1 = string.Empty;
        if (parameter.ParameterFlags == 1)
          str1 = "in";
        else if (parameter.ParameterFlags == 2)
          str1 = "out";
        else if (parameter.ParameterFlags == 4)
          str1 = "ref";
        string str2;
        if (str1 == null)
        {
          str2 = parameter.TypeName + " " + parameter.Name;
        }
        else
        {
          interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
          interpolatedStringHandler.AppendLiteral("[");
          interpolatedStringHandler.AppendFormatted(str1);
          interpolatedStringHandler.AppendLiteral("] ");
          interpolatedStringHandler.AppendFormatted(parameter.TypeName);
          interpolatedStringHandler.AppendLiteral(" ");
          interpolatedStringHandler.AppendFormatted(parameter.Name);
          str2 = interpolatedStringHandler.ToStringAndClear();
        }
        values.Add(str2);
      }
      string str = string.Join<string>(",", (IEnumerable<string>) values);
      interpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 3);
      interpolatedStringHandler.AppendFormatted(this._returnType);
      interpolatedStringHandler.AppendLiteral(" ");
      interpolatedStringHandler.AppendFormatted(this._name);
      interpolatedStringHandler.AppendLiteral("(");
      interpolatedStringHandler.AppendFormatted(str);
      interpolatedStringHandler.AppendLiteral(")");
      return interpolatedStringHandler.ToStringAndClear();
    }

    internal RpcMethod AddParameter(IRpcMethodParameter item)
    {
      this._parameters.Add(item);
      return this;
    }

    internal RpcMethod AddParameter(string name, IDataType type, MethodParamFlags flags)
    {
      this._parameters.Add((IRpcMethodParameter) new RpcMethodParameter(name, type, flags));
      return this;
    }

    internal RpcMethod AddParameter(
      string name,
      IDataType type,
      MethodParamFlags flags,
      int lengthIsIndex)
    {
      this._parameters.Add((IRpcMethodParameter) new RpcMethodParameter(name, type, flags, lengthIsIndex));
      return this;
    }

    internal RpcMethod SetReturnType(IDataType returnType)
    {
      this._returnType = returnType.Name;
      this._returnAlignSize = ((IBitSize) returnType).ByteSize;
      this._returnTypeSize = ((IBitSize) returnType).ByteSize;
      return this;
    }
  }
}
