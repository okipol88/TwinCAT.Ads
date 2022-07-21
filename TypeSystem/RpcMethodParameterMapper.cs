// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.RpcMethodParameterMapper
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class RpcMethodParameterMapper.</summary>
  internal class RpcMethodParameterMapper
  {
    private IRpcMethod _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.RpcMethodParameterMapper" /> class.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <exception cref="T:System.ArgumentNullException">method</exception>
    internal RpcMethodParameterMapper(IRpcMethod method) => this._method = method != null ? method : throw new ArgumentNullException(nameof (method));

    /// <summary>Copies the out parameters.</summary>
    /// <param name="outParameters">The out parameters.</param>
    /// <param name="args">The arguments.</param>
    internal void CopyOutParameters(object[] outParameters, object[] args)
    {
      IRpcMethodParameterCollection parameters = this._method.Parameters;
      for (int methodParameterIndex = 0; methodParameterIndex < ((ICollection<IRpcMethodParameter>) parameters).Count; ++methodParameterIndex)
      {
        int inIndex = -1;
        int outIndex = -1;
        this.getIndexInArgs(methodParameterIndex, out inIndex, out outIndex);
        if (outIndex >= 0)
          args[methodParameterIndex] = outParameters[outIndex];
      }
    }

    /// <summary>Gets the mapped indexes for the RpcMethod parameter.</summary>
    /// <param name="methodParameterIndex">Index of the method parameter.</param>
    /// <param name="inIndex">The in index2.</param>
    /// <param name="outIndex">The out index2.</param>
    private void getIndexInArgs(int methodParameterIndex, out int inIndex, out int outIndex)
    {
      IRpcMethodParameterCollection parameters = this._method.Parameters;
      MethodParamFlags parameterFlags = ((IList<IRpcMethodParameter>) parameters)[methodParameterIndex].ParameterFlags;
      int num1 = -1;
      int num2 = -1;
      for (int index = 0; index < methodParameterIndex; ++index)
      {
        if ((((IList<IRpcMethodParameter>) parameters)[index].ParameterFlags & parameterFlags) > 0)
        {
          if ((((IList<IRpcMethodParameter>) parameters)[index].ParameterFlags & 1) > 0)
            ++num1;
          if ((((IList<IRpcMethodParameter>) parameters)[index].ParameterFlags & 2) > 0)
            ++num2;
        }
      }
      inIndex = (((IList<IRpcMethodParameter>) parameters)[methodParameterIndex].ParameterFlags & 1) <= 0 ? -1 : num1 + 1;
      if ((((IList<IRpcMethodParameter>) parameters)[methodParameterIndex].ParameterFlags & 2) > 0)
        outIndex = num2 + 1;
      else
        outIndex = -1;
    }

    /// <summary>
    /// Tries to map the arguments to the In/Out RpcParameters
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal bool TryMapParameters(
      object[] args,
      [NotNullWhen(true)] out object[]? inParameters,
      [NotNullWhen(true)] out object[]? outParameters)
    {
      int count1 = ((ICollection<IRpcMethodParameter>) this._method.InParameters).Count;
      int count2 = ((ICollection<IRpcMethodParameter>) this._method.OutParameters).Count;
      inParameters = (object[]) null;
      outParameters = (object[]) null;
      if (args.Length != ((ICollection<IRpcMethodParameter>) this._method.Parameters).Count)
        return false;
      inParameters = new object[count1];
      outParameters = new object[count2];
      IRpcMethodParameterCollection parameters = this._method.Parameters;
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < args.Length; ++index)
      {
        if ((((IList<IRpcMethodParameter>) parameters)[index].ParameterFlags & 1) > 0)
          inParameters[num1++] = args[index];
        if ((((IList<IRpcMethodParameter>) parameters)[index].ParameterFlags & 2) > 0)
          outParameters[num2++] = args[index];
      }
      return true;
    }
  }
}
