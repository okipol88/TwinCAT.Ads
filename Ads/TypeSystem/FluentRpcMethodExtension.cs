// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.FluentRpcMethodExtension
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class FluentRpcMethodExtension.</summary>
  /// <remarks>
  /// This method extends the <see cref="T:TwinCAT.Ads.TypeSystem.RpcMethod" /> with extension methods to construct methods fluently.
  /// </remarks>
  public static class FluentRpcMethodExtension
  {
    /// <summary>Adds a parameter to the RpcMethod</summary>
    /// <param name="method">The method.</param>
    /// <param name="parameter">The Parameter.</param>
    /// <returns>RpcMethod.</returns>
    public static RpcMethod AddParameter(
      this RpcMethod method,
      IRpcMethodParameter parameter)
    {
      method.AddParameter(parameter);
      return method;
    }

    /// <summary>Adds a parameter to the RpcMethod</summary>
    /// <param name="method">The method.</param>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="flags">The flags.</param>
    /// <returns>RpcMethod.</returns>
    public static RpcMethod AddParameter(
      this RpcMethod method,
      string name,
      IDataType type,
      MethodParamFlags flags)
    {
      method.AddParameter(name, type, flags);
      return method;
    }

    /// <summary>Adds a parameter to the RpcMethod</summary>
    /// <param name="method">The method.</param>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The type.</param>
    /// <param name="flags">Flags of the method parameter.</param>
    /// <param name="lengthIsIndex">LengthIs attribute.</param>
    /// <returns>RpcMethod.</returns>
    public static RpcMethod AddParameter(
      this RpcMethod method,
      string name,
      IDataType type,
      MethodParamFlags flags,
      int lengthIsIndex)
    {
      method.AddParameter(name, type, flags, lengthIsIndex);
      return method;
    }

    /// <summary>Sets the ReturnType of the RpcMethod.</summary>
    /// <param name="method">The method.</param>
    /// <param name="returnType">The datatype of the methods return value.</param>
    /// <returns>RpcMethod.</returns>
    public static RpcMethod SetReturnType(this RpcMethod method, IDataType returnType)
    {
      method.SetReturnType(returnType);
      return method;
    }
  }
}
