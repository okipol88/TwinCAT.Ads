// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.RpcStructReferenceInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Class RpcStructReferenceInstance.
  /// Implements the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" />
  /// Implements the <see cref="T:TwinCAT.TypeSystem.IRpcStructInstance" />
  /// </summary>
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IRpcStructInstance" />
  internal class RpcStructReferenceInstance : 
    ReferenceInstance,
    IRpcStructInstance,
    IStructInstance,
    IInterfaceInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IRpcCallableInstance
  {
    /// <summary>
    /// Gets the Method descriptions for the <see cref="T:TwinCAT.TypeSystem.IRpcCallableType" />
    /// </summary>
    /// <value>The methods.</value>
    public IRpcMethodCollection RpcMethods => this.ReferencedType is IRpcCallableType referencedType ? referencedType.RpcMethods : (IRpcMethodCollection) RpcMethodCollection.Empty;

    /// <summary>
    /// Gets the member instances of the <see cref="T:TwinCAT.TypeSystem.IStructInstance">Struct Instance</see>.
    /// </summary>
    /// <value>The member instances.</value>
    public ISymbolCollection<ISymbol> MemberInstances => this.SubSymbols;

    /// <summary>
    /// Gets a value indicating whether this instance has RPC methods.
    /// </summary>
    /// <value><c>true</c> if this instance has RPC methods; otherwise, <c>false</c>.</value>
    public bool HasRpcMethods => ((IReferenceType) this.DataType)?.ReferencedType is IStructType referencedType && ((IInterfaceType) referencedType).HasRpcMethods;

    internal RpcStructReferenceInstance(
      AdsSymbolEntry entry,
      IReferenceType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, type, parent, factoryServices)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal RpcStructReferenceInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">Indicates, that the oversample Symbol is to be created.</param>
    /// <param name="parent">The parent.</param>
    internal RpcStructReferenceInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ReferenceInstance" /> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="type">The type.</param>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="fieldOffset">The field offset.</param>
    internal RpcStructReferenceInstance(
      ISymbol parent,
      IReferenceType type,
      string instanceName,
      int fieldOffset)
      : base(instanceName, type, parent, fieldOffset)
    {
    }

    /// <summary>Invokes the RPC method.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <returns>System.Object.</returns>
    public object? InvokeRpcMethod(string methodName, object[]? inParameters)
    {
      object[] outParameters = (object[]) null;
      object retValue;
      int num = this.TryInvokeRpcMethod(methodName, inParameters, (AnyTypeSpecifier[]) null, (AnyTypeSpecifier) null, out outParameters, out retValue);
      if (num != 0)
        throw new RpcInvokeException((IInterfaceInstance) this, methodName, num);
      return retValue;
    }

    /// <summary>Invokes the RPC method.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.RpcInvokeException"></exception>
    public object? InvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      out object[]? outParameters)
    {
      object retValue;
      int num = this.TryInvokeRpcMethod(methodName, inParameters, (AnyTypeSpecifier[]) null, (AnyTypeSpecifier) null, out outParameters, out retValue);
      if (num != 0)
        throw new RpcInvokeException((IInterfaceInstance) this, methodName, num);
      return retValue;
    }

    public object? InvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters)
    {
      object retValue;
      int num = this.TryInvokeRpcMethod(methodName, inParameters, outSpecifiers, retSpecifier, out outParameters, out retValue);
      if (num != 0)
        throw new RpcInvokeException((IInterfaceInstance) this, methodName, num);
      return retValue;
    }

    /// <summary>Tries the invoke RPC method.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="retValue">The ret value.</param>
    /// <returns>System.Int32.</returns>
    public int TryInvokeRpcMethod(string methodName, object[]? inParameters, out object? retValue)
    {
      object[] outParameters = (object[]) null;
      return this.TryInvokeRpcMethod(methodName, inParameters, (AnyTypeSpecifier[]) null, (AnyTypeSpecifier) null, out outParameters, out retValue);
    }

    /// <summary>Tries the invoke RPC method.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <param name="retValue">The ret value.</param>
    /// <returns>System.Int32.</returns>
    public int TryInvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      out object[]? outParameters,
      out object? retValue)
    {
      return this.TryInvokeRpcMethod(methodName, inParameters, (AnyTypeSpecifier[]) null, (AnyTypeSpecifier) null, out outParameters, out retValue);
    }

    public int TryInvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters,
      out object? retValue)
    {
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentException("Method name cannot be empty", nameof (methodName));
      IAccessorRawValue valueAccessor = this.ValueAccessor;
      if (valueAccessor == null)
        throw new CannotAccessValueException();
      this.EnsureRights((SymbolAccessRights) 4);
      IRpcCallableType referencedType = (IRpcCallableType) this.ReferencedType;
      IRpcMethod irpcMethod = (IRpcMethod) null;
      if (referencedType == null)
        throw new CannotResolveDataTypeException((IInstance) this);
      if (!referencedType.RpcMethods.TryGetMethod(methodName, ref irpcMethod))
        throw new ArgumentOutOfRangeException(nameof (methodName));
      DateTimeOffset? nullable;
      return ((IAccessorRpc) valueAccessor).TryInvokeRpcMethod((IInstance) this, irpcMethod, inParameters, outSpecifiers, retSpecifier, ref outParameters, ref retValue, ref nullable);
    }

    /// <summary>Invokes the RPC method asynchronous.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>System.Threading.Tasks.Task&lt;TwinCAT.ValueAccess.ResultRpcMethodAccess&gt;.</returns>
    public Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      string methodName,
      object[]? inParameters,
      CancellationToken cancel)
    {
      return this.InvokeRpcMethodAsync(methodName, inParameters, (AnyTypeSpecifier[]) null, (AnyTypeSpecifier) null, cancel);
    }

    public Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      CancellationToken cancel)
    {
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentException("Invalid method name!", nameof (methodName));
      StructType referencedType = (StructType) this.ReferencedType;
      if (referencedType == null)
        throw new CannotResolveDataTypeException((IInstance) this);
      IRpcMethod method = (IRpcMethod) null;
      if (!referencedType.RpcMethods.TryGetMethod(methodName, ref method))
        throw new ArgumentOutOfRangeException(nameof (methodName));
      return this.InvokeRpcMethodAsync(method, inParameters, outSpecifiers, retSpecifier, cancel);
    }

    private async Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      CancellationToken cancel)
    {
      RpcStructReferenceInstance referenceInstance = this;
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      // ISSUE: explicit non-virtual call
      if (!(__nonvirtual (referenceInstance.ValueAccessor) is IAccessorRpc valueAccessor))
        throw new CannotAccessValueException();
      referenceInstance.EnsureRights((SymbolAccessRights) 4);
      return await valueAccessor.InvokeRpcMethodAsync((IInstance) referenceInstance, method, inParameters, outSpecifiers, retSpecifier, cancel).ConfigureAwait(false);
    }
  }
}
