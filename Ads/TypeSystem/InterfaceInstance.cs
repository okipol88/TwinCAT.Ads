// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.InterfaceInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class representing a Struct Instance</summary>
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class InterfaceInstance : 
    Symbol,
    IInterfaceInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IRpcCallableInstance,
    IRpcStructInstance,
    IStructInstance
  {
    internal InterfaceInstance(
      AdsSymbolEntry entry,
      IInterfaceType type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(entry, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 16;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="factoryServices">The factory services.</param>
    protected InterfaceInstance(
      string instanceName,
      string instancePath,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, factoryServices)
    {
      this.Category = (DataTypeCategory) 16;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructInstance" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="parent">The parent (<see cref="T:TwinCAT.TypeSystem.IStructInstance" /> or <see cref="T:TwinCAT.TypeSystem.IAliasInstance" />) of this member instance symbol.</param>
    internal InterfaceInstance(Member member, ISymbol parent)
      : base(member, parent)
    {
      this.Category = (DataTypeCategory) 16;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructInstance" /> class.
    /// </summary>
    /// <param name="currentIndex">Index of the current.</param>
    /// <param name="oversample">if set to <c>true</c> [oversample].</param>
    /// <param name="parent">The parent.</param>
    internal InterfaceInstance(int[] currentIndex, bool oversample, ISymbol parent)
      : base(currentIndex, oversample, parent)
    {
      this.Category = (DataTypeCategory) 16;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InterfaceInstance(
      string instanceName,
      IInterfaceType type,
      ISymbol parent,
      int fieldOffset)
      : base(instanceName, (IDataType) type, parent, fieldOffset, ((ISymbolFactoryServicesProvider) parent).FactoryServices)
    {
      this.Category = (DataTypeCategory) 16;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public InterfaceInstance(
      string instanceName,
      string? instancePath,
      uint indexGroup,
      uint indexOffset,
      IInterfaceType? type,
      ISymbol? parent,
      ISymbolFactoryServices factoryServices)
      : base(instanceName, instancePath, indexGroup, indexOffset, (IDataType) type, parent, factoryServices)
    {
      this.Category = (DataTypeCategory) 16;
    }

    internal override int OnGetSubSymbolCount(ISymbol parentSymbol)
    {
      IInterfaceType dataType = (IInterfaceType) this.DataType;
      return dataType != null ? ((ICollection<IMember>) dataType.AllMembers).Count : 0;
    }

    /// <summary>Called when [create sub symbols].</summary>
    /// <param name="parentInstance">The parent instance.</param>
    /// <returns>SymbolCollection.</returns>
    internal override ISymbolCollection<ISymbol> OnCreateSubSymbols(
      ISymbol parentInstance)
    {
      ISymbolCollection<ISymbol> subSymbols = (ISymbolCollection<ISymbol>) SymbolCollection.Empty;
      try
      {
        IInterfaceType dataType = (IInterfaceType) this.DataType;
        subSymbols = dataType == null ? (ISymbolCollection<ISymbol>) new SymbolCollection((InstanceCollectionMode) 0) : ((ISymbolFactoryServicesProvider) this).FactoryServices.SymbolFactory.CreateFieldInstances((ISymbol) this, (IDataType) dataType);
      }
      catch (Exception ex)
      {
        AdsModule.Trace.TraceError(ex);
      }
      return subSymbols;
    }

    /// <summary>
    /// Gets the member instances of the <see cref="T:TwinCAT.TypeSystem.IStructInstance">Struct Instance</see>.
    /// </summary>
    /// <value>The member instances.</value>
    public ISymbolCollection<ISymbol> MemberInstances => this.SubSymbols;

    /// <summary>
    /// Gets a value indicating whether this instance is primitive.
    /// </summary>
    /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
    public override bool IsPrimitiveType => false;

    /// <summary>
    /// Gets a value indicating whether this Symbol is a container/complex type.
    /// </summary>
    /// <value><c>true</c> if this instance is container type; otherwise, <c>false</c>.</value>
    public override bool IsContainerType => true;

    /// <summary>
    /// Gets a value indicating whether this instance has RPC methods
    /// </summary>
    /// <value><c>true</c> if this instance has RPC methods; otherwise, <c>false</c>.</value>
    /// <remarks>If the struct instance supports RPC Methods, then the instance class is also
    /// supporting <see cref="T:TwinCAT.TypeSystem.IRpcStructInstance" />.</remarks>
    /// <seealso cref="T:TwinCAT.TypeSystem.IRpcStructInstance" />
    /// <seealso cref="T:TwinCAT.TypeSystem.IRpcMethod" />
    /// <seealso cref="T:TwinCAT.TypeSystem.IRpcMethodParameter" />
    public bool HasRpcMethods
    {
      get
      {
        StructType dataType = (StructType) this.DataType;
        return dataType != null && dataType.HasRpcMethods;
      }
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
    public object InvokeRpcMethod(string methodName, object[]? inParameters)
    {
      object[] outParameters = (object[]) null;
      return this.InvokeRpcMethod(methodName, inParameters, out outParameters);
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The input parameters or NULL</param>
    /// <param name="outParameters">The output parameters.</param>
    /// <returns>The return value of the Method (as object).</returns>
    /// <remarks>
    /// Because this overload doesn't provide any <see cref="T:TwinCAT.TypeSystem.AnyTypeSpecifier" /> specifications, only primitive datatypes will be correctly marshalled
    /// by this method. Complex types will fall back to byte[] arrays.
    /// </remarks>
    /// <example>
    /// The following sample shows how to call (Remote Procedures / Methods) within the PLC.
    /// <code language="C#" title="Dynamic Tree Mode" source="..\..\Samples\Sample.Ads.AdsClientCore\SymbolBrowserV2VirtualTree.cs" region="CODE_SAMPLE_RPCCALL" />
    /// </example>
    public object InvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      out object[]? outParameters)
    {
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentOutOfRangeException(nameof (methodName));
      object retValue = (object) null;
      int num = this.TryInvokeRpcMethod(methodName, inParameters, out outParameters, out retValue);
      if (num != 0)
        throw new RpcInvokeException((IInterfaceInstance) this, methodName, num);
      return retValue;
    }

    public object InvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters)
    {
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentOutOfRangeException(nameof (methodName));
      object retValue = (object) null;
      int num = this.TryInvokeRpcMethod(methodName, inParameters, outSpecifiers, retSpecifier, out outParameters, out retValue);
      if (num != 0)
        throw new RpcInvokeException((IInterfaceInstance) this, methodName, num);
      return retValue;
    }

    public int TryInvokeRpcMethod(
      string methodName,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters,
      out object? retValue)
    {
      IRpcMethod method = (IRpcMethod) null;
      if (!RpcInvokeParser.TryGetRpcMethod((IRpcCallableInstance) this, methodName, ref method))
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
        interpolatedStringHandler.AppendLiteral("RpcMethod '");
        interpolatedStringHandler.AppendFormatted(methodName);
        interpolatedStringHandler.AppendLiteral("' not found on Instance '");
        interpolatedStringHandler.AppendFormatted(this.InstancePath);
        interpolatedStringHandler.AppendLiteral("'!");
        throw new ArgumentOutOfRangeException(nameof (methodName), interpolatedStringHandler.ToStringAndClear());
      }
      return this.TryInvokeRpcMethod(method, inParameters, outSpecifiers, retSpecifier, out outParameters, out retValue);
    }

    public int TryInvokeRpcMethod(
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      out object[]? outParameters,
      out object? retValue)
    {
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException();
      this.EnsureRights((SymbolAccessRights) 4);
      DateTimeOffset? nullable;
      return ((IAccessorRpc) this.ValueAccessor).TryInvokeRpcMethod((IInstance) this, method, inParameters, outSpecifiers, retSpecifier, ref outParameters, ref retValue, ref nullable);
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
        throw new ArgumentNullException(nameof (methodName));
      IRpcMethod method = (IRpcMethod) null;
      if (!RpcInvokeParser.TryGetRpcMethod((IRpcCallableInstance) this, methodName, ref method))
        throw new ArgumentOutOfRangeException(nameof (methodName), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Symbol '{0}' (DataType: '{1}' doesn't implement method '{2}'", (object) this.InstancePath, (object) this.DataType?.Name, (object) methodName));
      return this.InvokeRpcMethodAsync(method, inParameters, outSpecifiers, retSpecifier, cancel);
    }

    private async Task<ResultRpcMethodAccess> InvokeRpcMethodAsync(
      IRpcMethod method,
      object[]? inParameters,
      AnyTypeSpecifier[]? outSpecifiers,
      AnyTypeSpecifier? retSpecifier,
      CancellationToken cancel)
    {
      InterfaceInstance interfaceInstance = this;
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (interfaceInstance.ValueAccessor) == null)
        throw new CannotAccessValueException();
      interfaceInstance.EnsureRights((SymbolAccessRights) 4);
      // ISSUE: explicit non-virtual call
      return await ((IAccessorRpc) __nonvirtual (interfaceInstance.ValueAccessor)).InvokeRpcMethodAsync((IInstance) interfaceInstance, method, inParameters, outSpecifiers, retSpecifier, cancel).ConfigureAwait(false);
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="retValue">The return value of the RPC method as object.</param>
    /// <returns>The result value of the call (ErrorCode). 0 means Succeeded.</returns>
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
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentNullException(nameof (methodName));
      object[] outParameters = (object[]) null;
      return this.TryInvokeRpcMethod(methodName, inParameters, out outParameters, out retValue);
    }

    /// <summary>Invokes the specified RPC Method</summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="inParameters">The parameters.</param>
    /// <param name="retValue">The return value of the RPC method as object.</param>
    /// <param name="outParameters">The out parameters.</param>
    /// <returns>The result value of the call (ErrorCode). 0 means Succeeded.</returns>
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
      if (string.IsNullOrEmpty(methodName))
        throw new ArgumentNullException(nameof (methodName));
      if (this.ValueAccessor == null)
        throw new CannotAccessValueException();
      IRpcMethod irpcMethod = (IRpcMethod) null;
      if (!RpcInvokeParser.TryGetRpcMethod((IRpcCallableInstance) this, methodName, ref irpcMethod))
        throw new ArgumentOutOfRangeException(nameof (methodName), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Symbol '{0}' (DataType: '{1}' doesn't implement method '{2}'", (object) this.InstancePath, (object) this.DataType?.Name, (object) methodName));
      this.EnsureRights((SymbolAccessRights) 4);
      DateTimeOffset? nullable;
      return ((IAccessorRpc) this.ValueAccessor).TryInvokeRpcMethod((IInstance) this, irpcMethod, inParameters, (AnyTypeSpecifier[]) null, (AnyTypeSpecifier) null, ref outParameters, ref retValue, ref nullable);
    }

    /// <summary>
    /// Gets the Method descriptions for the <see cref="T:TwinCAT.TypeSystem.IRpcCallableType" />
    /// </summary>
    /// <value>The methods.</value>
    public IRpcMethodCollection RpcMethods
    {
      get
      {
        StructType dataType = (StructType) this.DataType;
        return dataType != null ? dataType.RpcMethods : (IRpcMethodCollection) RpcMethodCollection.Empty;
      }
    }
  }
}
