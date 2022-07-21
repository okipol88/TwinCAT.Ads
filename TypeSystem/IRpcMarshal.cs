// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.IRpcMarshal
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Interface IRpcMarshaller</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IRpcMarshal
  {
    /// <summary>
    /// Marshals the Method (in) Parameter values to the memory destination.
    /// </summary>
    /// <remarks>This is used on the Client side of the RpcInvoke call.</remarks>
    /// <param name="method">The method.</param>
    /// <param name="values">The parameter values.</param>
    /// <param name="destination">The memory destination..</param>
    /// <returns>Number of marshalled bytes.</returns>
    int MarshalInParameters(IRpcMethod method, object[] values, Span<byte> destination);

    /// <summary>
    /// Unmarshals the in parameters from buffer to object values
    /// </summary>
    /// <remarks>This is used on the Server side of the RpcInvoke call.</remarks>
    /// <param name="method">The Rpc Method.</param>
    /// <param name="source">The incoming data</param>
    /// <param name="values">The marshalled InParameter values.</param>
    /// <returns>Count of Unmarshaled bytes.</returns>
    int UnmarshalInParameters(IRpcMethod method, ReadOnlySpan<byte> source, object[] values);

    /// <summary>Unmarshals the Out-Parameters from the Source data.</summary>
    /// <param name="method">The method.</param>
    /// <param name="outSpec">The Output-Parameter specifications (for ANY_TYPE marshalling), NULL if marshalled by symbolic information.</param>
    /// <param name="source">The unmarshalled source data of the Out-Parameters.</param>
    /// <param name="values">The parameter values (used for potential LengthIs parameters)</param>
    /// <returns>Count of unmarshalled bytes.</returns>
    /// <remarks>This is used to Unmarshal the Out-Parameters on the Client side of the Rpc call.</remarks>
    int UnmarshalOutParameters(
      IRpcMethod method,
      AnyTypeSpecifier[]? outSpec,
      ReadOnlySpan<byte> source,
      object[] values);

    /// <summary>Marshals the Out-Parameters into the target data.</summary>
    /// <param name="method">The RPC method</param>
    /// <param name="values">The In-Parameter values (used for potential LengthIs parameters)</param>
    /// <param name="outSpec">The Output-Parameter specifications (for ANY_TYPE marshalling), NULL if marshalled by symbolic information.</param>
    /// <param name="target">The Out-Data created from parameters.</param>
    /// <returns>System.Int32.</returns>
    int MarshalOutParameters(
      IRpcMethod method,
      object[] values,
      AnyTypeSpecifier[]? outSpec,
      Span<byte> target);

    int UnmarshalReturnValue(
      IRpcMethod method,
      AnyTypeSpecifier? retSpec,
      ReadOnlySpan<byte> source,
      out object returnValue);

    int MarshalReturnValue(
      IRpcMethod method,
      AnyTypeSpecifier? retSpec,
      object returnValue,
      Span<byte> buffer);

    int UnmarshalRpcMethod(
      IRpcMethod method,
      AnyTypeSpecifier[]? outSpec,
      AnyTypeSpecifier? retSpec,
      object[] values,
      ReadOnlySpan<byte> source,
      out object? returnValue);

    /// <summary>Gets the size of the input marshalling data in bytes.</summary>
    /// <param name="method">The method.</param>
    /// <param name="values">The parameter values.</param>
    /// <returns>System.Int32.</returns>
    int GetInMarshallingSize(IRpcMethod method, object[] values);

    int GetOutMarshallingSize(
      IRpcMethod method,
      AnyTypeSpecifier[]? outSpec,
      AnyTypeSpecifier? retSpec,
      object[] values);
  }
}
