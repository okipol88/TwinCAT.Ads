// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicInterfaceInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Dynamic struct instance</summary>
  public class DynamicInterfaceInstance : 
    DynamicSymbol,
    IInterfaceInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IRpcCallableInstance
  {
    /// <summary>Dictionary of normalized Instance Names</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected WeakReference<IDictionary<string, ISymbol>>? _weakNormalizedNames;
    private const string ASYNC_EXT = "Async";

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicStructInstance" /> class.
    /// </summary>
    /// <param name="structInstance">The struct instance.</param>
    internal DynamicInterfaceInstance(IInterfaceInstance structInstance)
      : base((IValueSymbol) structInstance)
    {
    }

    /// <summary>Gets the Normalized names table.</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    private IDictionary<string, ISymbol> GetNormalizedNames()
    {
      IDictionary<string, ISymbol> target = (IDictionary<string, ISymbol>) null;
      if (this._weakNormalizedNames == null || !this._weakNormalizedNames.TryGetTarget(out target))
      {
        target = this.OnCreateNormalizedNames();
        if (this._weakNormalizedNames == null)
          this._weakNormalizedNames = new WeakReference<IDictionary<string, ISymbol>>(target);
        else
          this._weakNormalizedNames.SetTarget(target);
      }
      return target;
    }

    /// <summary>Creates the normalized names table</summary>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual IDictionary<string, ISymbol> OnCreateNormalizedNames()
    {
      IDictionary<string, ISymbol> memberDictionary = DynamicInterfaceInstance.createMemberDictionary(this.AllowIGIOAccess, ((IInterfaceInstance) this._InnerSymbol).MemberInstances);
      IRpcCallableType dataType = (IRpcCallableType) this.DataType;
      if (dataType != null)
      {
        foreach (IRpcMethod rpcMethod in (IEnumerable<IRpcMethod>) dataType.RpcMethods)
        {
          memberDictionary.Add(rpcMethod.Name ?? "", (ISymbol) this);
          memberDictionary.Add(rpcMethod.Name + "Async", (ISymbol) this);
        }
      }
      return memberDictionary;
    }

    internal static IDictionary<string, ISymbol> createMemberDictionary(
      bool allowIGIOAccess,
      ISymbolCollection<ISymbol> memberInstances)
    {
      Dictionary<string, ISymbol> memberDictionary = new Dictionary<string, ISymbol>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<string> source = new List<string>();
      if (allowIGIOAccess)
      {
        source.Add("IndexGroup");
        source.Add("IndexOffset");
        source.Add("ContextMask");
        source.Add("ImageBaseAddress");
        source.Add("DataTypeId");
      }
      source.Add("__deref");
      foreach (DynamicSymbol memberInstance in (IEnumerable<ISymbol>) memberInstances)
      {
        string key = memberInstance.NormalizedName;
        if (source.Contains<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          key = key.Insert(0, "_");
        memberDictionary.Add(key, (ISymbol) memberInstance);
      }
      return (IDictionary<string, ISymbol>) memberDictionary;
    }

    /// <summary>
    /// Gets the member instances of the <see cref="T:TwinCAT.TypeSystem.IStructInstance">Struct Instance</see>.
    /// </summary>
    /// <value>The member instances.</value>
    public ISymbolCollection<ISymbol> MemberInstances => this.SubSymbols;

    /// <summary>
    /// Gets a value indicating whether this instance has RPC methods
    /// </summary>
    /// <value><c>true</c> if this instance has RPC methods; otherwise, <c>false</c>.</value>
    /// <remarks>If the struct instance supports RPC Methods, then the instance class is also
    /// supporting <see cref="T:TwinCAT.TypeSystem.IRpcStructInstance" /></remarks>
    /// <seealso cref="T:TwinCAT.TypeSystem.IRpcStructInstance" />
    /// <seealso cref="T:TwinCAT.TypeSystem.IRpcMethod" />
    /// <seealso cref="T:TwinCAT.TypeSystem.IRpcMethodParameter" />
    public bool HasRpcMethods => ((IInterfaceInstance) this._InnerSymbol).HasRpcMethods;

    /// <summary>Returns the enumeration of all dynamic member names.</summary>
    /// <returns>A sequence that contains dynamic member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames() => (IEnumerable<string>) this.GetNormalizedNames().Keys;

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    public override bool TryGetMember(GetMemberBinder binder, [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      IRpcCallableInstance innerSymbol = (IRpcCallableInstance) this._InnerSymbol;
      string str = binder.Name;
      if (binder.Name.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
        str = binder.Name.Substring(0, binder.Name.Length - "Async".Length);
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
    /// <param name="returnValue">The result of the member invocation.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
    public override bool TryInvokeMember(
      InvokeMemberBinder binder,
      object?[]? args,
      out object? returnValue)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      if (args == null)
        throw new ArgumentNullException(nameof (args));
      IStructInstance innerSymbol = (IStructInstance) this._InnerSymbol;
      if (binder.Name.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
      {
        CancellationToken? nullable = new CancellationToken?();
        if (args != null && args.Length >= 1)
          nullable = args[args.Length - 1] as CancellationToken?;
        object[] objArray = (object[]) null;
        CancellationToken none;
        if (nullable.HasValue)
        {
          none = nullable.Value;
          if (args != null && args.Length > 1)
          {
            object[] destinationArray = (object[]) new AddingNewEventArgs[args.Length - 1];
            Array.Copy((Array) args, 0, (Array) destinationArray, 0, args.Length - 1);
          }
        }
        else
        {
          none = CancellationToken.None;
          objArray = args;
        }
        string str = binder.Name.Substring(0, binder.Name.Length - "Async".Length);
        IRpcMethod method = (IRpcMethod) null;
        if (((IRpcCallableInstance) innerSymbol).RpcMethods.TryGetMethod(str, ref method))
        {
          object[] inParameters = (object[]) null;
          object[] outParameters = (object[]) null;
          RpcMethodParameterMapper methodParameterMapper = new RpcMethodParameterMapper(method);
          if (args == null)
            return ((IRpcCallableInstance) innerSymbol).TryInvokeRpcMethod(binder.Name, inParameters, ref returnValue) == 0;
          if (methodParameterMapper.TryMapParameters(args, out inParameters, out outParameters))
          {
            int num = ((IRpcCallableInstance) innerSymbol).TryInvokeRpcMethod(binder.Name, inParameters, ref outParameters, ref returnValue);
            if (num == 0)
              methodParameterMapper.CopyOutParameters(outParameters, args);
            return num == 0;
          }
        }
      }
      else
      {
        IRpcMethod method = (IRpcMethod) null;
        if (((IRpcCallableInstance) innerSymbol).RpcMethods.TryGetMethod(binder.Name, ref method))
        {
          object[] inParameters = (object[]) null;
          object[] outParameters = (object[]) null;
          RpcMethodParameterMapper methodParameterMapper = new RpcMethodParameterMapper(method);
          if (args == null)
            return ((IRpcCallableInstance) innerSymbol).TryInvokeRpcMethod(binder.Name, (object[]) null, ref returnValue) == 0;
          if (methodParameterMapper.TryMapParameters(args, out inParameters, out outParameters))
          {
            bool flag = ((IRpcCallableInstance) innerSymbol).TryInvokeRpcMethod(binder.Name, inParameters, ref outParameters, ref returnValue) == 0;
            if (!flag)
              methodParameterMapper.CopyOutParameters(outParameters, args);
            return flag;
          }
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
          interpolatedStringHandler.AppendLiteral("Cannot map parameters to RpcMethod '");
          interpolatedStringHandler.AppendFormatted(((IInstance) innerSymbol).InstancePath);
          interpolatedStringHandler.AppendLiteral("'.'");
          interpolatedStringHandler.AppendFormatted(binder.Name);
          interpolatedStringHandler.AppendLiteral("'!");
          throw new ArgumentException(interpolatedStringHandler.ToStringAndClear());
        }
      }
      return base.TryInvokeMember(binder, args, out returnValue);
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The input parameters or NULL</param>
    /// <returns>The return value of the Method (as object).</returns>
    /// <remarks>This method only supports primitive data types as <paramref name="inParameters" />. Any available outparameters will be ignored.
    /// Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public object? InvokeRpcMethod(string methodName, object[]? inParameters)
    {
      object[] outParameters = (object[]) null;
      return this.InvokeRpcMethod(methodName, inParameters, out outParameters);
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <returns>The return value of the Method (as object).</returns>
    /// <remarks>The RpcMethod optionally support In-Parameters, Out-Parameters and Return values. Therefore the parameters <paramref name="inParameters" />, <paramref name="outParameters" />
    /// are allowed to be empty or NULL.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object? InvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      out object[]? outParameters)
    {
      object retValue = (object) null;
      outParameters = (object[]) null;
      int num = this.TryInvokeRpcMethod(methodName, inParameters, out outParameters, out retValue);
      if (num != 0)
        throw new RpcInvokeException((IInterfaceInstance) this, methodName, num);
      return retValue;
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="retValue">The return value of the RPC method as object.</param>
    /// <returns>The return value of the Method (as object).</returns>
    /// <remarks>
    /// Because this overload doesn't provide any <see cref="T:TwinCAT.TypeSystem.AnyTypeSpecifier" /> specifications, only primitive datatypes will be correctly marshalled
    /// by this method. Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public int TryInvokeRpcMethod(string methodName, object[]? inParameters, out object? retValue)
    {
      object[] outParameters = (object[]) null;
      return this.TryInvokeRpcMethod(methodName, inParameters, out outParameters, out retValue);
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="retValue">The return value of the RPC method as object.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <returns>The ADS Error Code.</returns>
    /// <remarks>
    /// Because this overload doesn't provide any <see cref="T:TwinCAT.TypeSystem.AnyTypeSpecifier" /> specifications, only primitive datatypes will be correctly marshalled
    /// by this method. Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public int TryInvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      out object[]? outParameters,
      out object? retValue)
    {
      return ((IRpcCallableInstance) this._InnerSymbol).TryInvokeRpcMethod(methodName, inParameters, ref outParameters, ref retValue);
    }

    /// <summary>Invokes the specified RPC Method asynchronously</summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="cancel">The cancellation token</param>
    /// <returns>
    /// A task that represents the asynchronous 'InvokeRpcMethod' operation. The <see cref="T:TwinCAT.ValueAccess.ResultRpcMethodAccess" /> results contains the return value (<see cref="P:TwinCAT.ValueAccess.ResultRpcMethodAccess.ReturnValue" />)
    /// together with the output parameters. The succeeded communication is indicated by the ErrorCode property (<see cref="P:TwinCAT.ValueAccess.ResultAccess.ErrorCode" />) after the communication.
    /// </returns>
    /// <remarks>
    /// Because this overload doesn't provide any <see cref="T:TwinCAT.TypeSystem.AnyTypeSpecifier" /> specifications, only primitive datatypes will be correctly marshalled
    /// by this method. Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      string methodName,
      object[]? inParameters,
      CancellationToken cancel)
    {
      return ((IRpcCallableInstance) this._InnerSymbol).InvokeRpcMethodAsync(methodName, inParameters, cancel);
    }

    /// <summary>
    /// Gets the Method descriptions for the <see cref="T:TwinCAT.TypeSystem.IRpcCallableType" />
    /// </summary>
    /// <value>The methods.</value>
    public IRpcMethodCollection RpcMethods => ((IRpcCallableInstance) this._InnerSymbol).RpcMethods;
  }
}
