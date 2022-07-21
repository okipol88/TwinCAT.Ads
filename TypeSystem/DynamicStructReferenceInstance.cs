// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicStructReferenceInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Class DynamicStructReferenceInstance.
  /// Implements the <see cref="T:TwinCAT.TypeSystem.DynamicReferenceInstance" />
  /// Implements the <see cref="T:TwinCAT.TypeSystem.IRpcStructInstance" />
  /// </summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.DynamicReferenceInstance" />
  /// <seealso cref="T:TwinCAT.TypeSystem.IRpcStructInstance" />
  internal class DynamicStructReferenceInstance : 
    DynamicReferenceInstance,
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
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicReferenceInstance" /> class.
    /// </summary>
    /// <param name="refInstance">The ref instance.</param>
    internal DynamicStructReferenceInstance(IReferenceInstance refInstance)
      : base(refInstance)
    {
    }

    protected override IDictionary<string, ISymbol> OnCreateNormalizedNames() => DynamicInterfaceInstance.createMemberDictionary(false, ((ISymbol) this._InnerSymbol).SubSymbols);

    /// <summary>Returns the enumeration of all dynamic member names.</summary>
    /// <returns>A sequence that contains dynamic member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames()
    {
      List<string> dynamicMemberNames = new List<string>((IEnumerable<string>) this.GetNormalizedNames().Keys);
      IRpcCallableType resolvedReferenceType = (IRpcCallableType) this.resolvedReferenceType;
      if (resolvedReferenceType != null)
      {
        foreach (IRpcMethod rpcMethod in (IEnumerable<IRpcMethod>) resolvedReferenceType.RpcMethods)
          dynamicMemberNames.Add(rpcMethod.Name);
      }
      return (IEnumerable<string>) dynamicMemberNames;
    }

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    public override bool TryGetMember(GetMemberBinder binder, [NotNullWhen(true)] out object? result)
    {
      if (this.resolvedReferenceType is IRpcCallableType resolvedReferenceType)
      {
        IRpcMethod irpcMethod = (IRpcMethod) null;
        if (resolvedReferenceType.RpcMethods.TryGetMethod(binder.Name, ref irpcMethod))
        {
          result = (object) irpcMethod;
          return true;
        }
      }
      ISymbol isymbol = (ISymbol) null;
      if (!this.GetNormalizedNames().TryGetValue(binder.Name, out isymbol))
        return base.TryGetMember(binder, out result);
      result = (object) isymbol;
      return true;
    }

    /// <summary>
    /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
    public override bool TrySetMember(SetMemberBinder binder, object? value) => base.TrySetMember(binder, value);

    /// <summary>
    /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as calling a method.
    /// </summary>
    /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args[][]" /> is equal to 100.</param>
    /// <param name="result">The result of the member invocation.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
    public override bool TryInvokeMember(
      InvokeMemberBinder binder,
      object?[]? args,
      out object? result)
    {
      if (this._InnerSymbol is IRpcCallableInstance innerSymbol)
      {
        IRpcMethod method = (IRpcMethod) null;
        if (innerSymbol.RpcMethods.TryGetMethod(binder.Name, ref method))
        {
          object[] inParameters = (object[]) null;
          object[] outParameters = (object[]) null;
          RpcMethodParameterMapper methodParameterMapper = new RpcMethodParameterMapper(method);
          if (args != null)
          {
            if (methodParameterMapper.TryMapParameters(args, out inParameters, out outParameters))
            {
              result = innerSymbol.InvokeRpcMethod(binder.Name, inParameters, ref outParameters);
              methodParameterMapper.CopyOutParameters(outParameters, args);
              return true;
            }
          }
          else
          {
            result = innerSymbol.InvokeRpcMethod(binder.Name, (object[]) null);
            return true;
          }
        }
      }
      return base.TryInvokeMember(binder, args, out result);
    }

    /// <summary>Gets the member instances.</summary>
    /// <value>The member instances.</value>
    public ISymbolCollection<ISymbol> MemberInstances => ((IInterfaceInstance) this._InnerSymbol).MemberInstances;

    /// <summary>
    /// Gets a value indicating whether this instance has RPC methods.
    /// </summary>
    /// <value><c>true</c> if this instance has RPC methods; otherwise, <c>false</c>.</value>
    public bool HasRpcMethods => ((IInterfaceInstance) this._InnerSymbol).HasRpcMethods;

    /// <summary>Gets the RPC methods.</summary>
    /// <value>The RPC methods.</value>
    public IRpcMethodCollection RpcMethods => ((IRpcCallableInstance) this._InnerSymbol).RpcMethods;

    /// <summary>Invokes the RPC method.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <returns>System.Object.</returns>
    public object? InvokeRpcMethod(string methodName, object[]? inParameters)
    {
      if (this._InnerSymbol is IRpcCallableInstance innerSymbol)
        return innerSymbol.InvokeRpcMethod(methodName, inParameters);
      throw new RpcInvokeException((IInterfaceInstance) this, methodName, -1);
    }

    /// <summary>Invokes the RPC method.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <returns>System.Object.</returns>
    public object? InvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      out object[]? outParameters)
    {
      if (this._InnerSymbol is IRpcCallableInstance innerSymbol)
        return innerSymbol.InvokeRpcMethod(methodName, inParameters, ref outParameters);
      throw new RpcInvokeException((IInterfaceInstance) this, methodName, -1);
    }

    /// <summary>Tries the invoke RPC method.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="retValue">The ret value.</param>
    /// <returns>System.Int32.</returns>
    public int TryInvokeRpcMethod(string methodName, object[]? inParameters, out object? retValue)
    {
      if (this._InnerSymbol is IRpcCallableInstance innerSymbol)
        return innerSymbol.TryInvokeRpcMethod(methodName, inParameters, ref retValue);
      throw new RpcInvokeException((IInterfaceInstance) this, methodName, -1);
    }

    /// <summary>Tries the invoke RPC method.</summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="inParameters">The in parameters.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <param name="retValue">The ret value.</param>
    /// <returns>System.Int32.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.RpcInvokeException">-1</exception>
    public int TryInvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      out object[]? outParameters,
      out object? retValue)
    {
      if (this._InnerSymbol is IRpcCallableInstance innerSymbol)
        return innerSymbol.TryInvokeRpcMethod(methodName, inParameters, ref outParameters, ref retValue);
      throw new RpcInvokeException((IInterfaceInstance) this, methodName, -1);
    }

    public Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      string methodName,
      object[]? inParameters,
      CancellationToken cancel)
    {
      if (this._InnerSymbol is IRpcCallableInstance innerSymbol)
        return innerSymbol.InvokeRpcMethodAsync(methodName, inParameters, cancel);
      throw new RpcInvokeException((IInterfaceInstance) this, methodName, -1);
    }
  }
}
