// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.StructType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Represents a struct type</summary>
  public class StructType : 
    DataType,
    IStructType,
    IInterfaceType,
    IDataType,
    IBitSize,
    IRpcCallableType,
    IRpcStructType
  {
    /// <summary>The binder resolved interface implementation types</summary>
    private IInterfaceType?[]? _implements;
    /// <summary>RPC Method description</summary>
    private RpcMethodCollection _rpcMethods = RpcMethodCollection.Empty;
    private MemberCollection _members = new MemberCollection();
    /// <summary>ID of the base type of Derived.</summary>
    private AdsDataTypeId _baseTypeId;
    /// <summary>
    /// Base Type Name of the <see cref="T:TwinCAT.Ads.TypeSystem.StructType" /> if derived
    /// </summary>
    private string _baseTypeName = string.Empty;
    /// <summary>Base Type of the Struct if derived.</summary>
    private IDataType? _baseType;
    /// <summary>
    /// All members of this <see cref="T:TwinCAT.Ads.TypeSystem.StructType" />
    /// </summary>
    private MemberCollection? _allMembers;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal StructType(AdsDataTypeEntry entry)
      : base((DataTypeCategory) 5, entry)
    {
      this._baseTypeId = entry.BaseTypeId;
      this._baseTypeName = entry.TypeName;
      for (int index = 0; index < entry.SubItemCount; ++index)
      {
        AdsFieldEntry subEntry = entry.SubEntries[index];
        if (subEntry != null)
        {
          Member instance = new Member((DataType) this, subEntry);
          if (!this._members.isUnique((IInstance) instance))
            ((IInstanceInternal) instance).SetInstanceName(this._members.createUniquepathName((IInstance) instance));
          this._members.Add((IMember) instance);
        }
      }
      this._rpcMethods = new RpcMethodCollection(entry.Methods);
    }

    public StructType(string name, IDataType? baseType)
      : this(name, baseType, (IMemberCollection) null)
    {
    }

    public StructType(
      string name,
      IDataType? baseType,
      IMemberCollection? fields,
      IRpcMethodCollection? methods)
      : this(name, baseType, fields)
    {
      this._rpcMethods = methods != null ? new RpcMethodCollection((IEnumerable<IRpcMethod>) methods) : RpcMethodCollection.Empty;
      if (this._rpcMethods.Count <= 0)
        return;
      this.flags = (AdsDataTypeFlags) (this.flags | 2048);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.StructType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public StructType(string name)
      : this(name, (IDataType) null, (IMemberCollection) null)
    {
    }

    public StructType(string name, IDataType? baseType, IMemberCollection? fields)
      : base(name, (AdsDataTypeId) 65, (DataTypeCategory) 5, -1, (Type) null)
    {
      int size = baseType != null ? ((IBitSize) baseType).Size : 0;
      if (fields != null)
      {
        this.SetSize(size + fields.CalcSize(), (Type) null);
        this._members = (MemberCollection) new AlignedMemberCollection();
      }
      else
      {
        this.SetSize(size, (Type) null);
        this._members = (MemberCollection) AlignedMemberCollection.Empty;
      }
    }

    internal StructType AddAligned(IMember member)
    {
      int num = this.BaseType != null ? ((IBitSize) this.BaseType).Size : 0;
      this._members.Add(member);
      this.SetSize(num + this._members.CalcSize(), (Type) null);
      return this;
    }

    /// <summary>
    /// Gets the names of the interfaces, this <see cref="T:TwinCAT.TypeSystem.IDataType" /> implements.
    /// </summary>
    /// <value>The interface implementations.</value>
    public string[] InterfaceImplementationNames
    {
      get
      {
        ITypeAttribute itypeAttribute = ((IEnumerable<ITypeAttribute>) this.Attributes).SingleOrDefault<ITypeAttribute>((Func<ITypeAttribute, bool>) (a => a.Name.Equals("TcImplements", StringComparison.OrdinalIgnoreCase)));
        return itypeAttribute != null ? itypeAttribute.Value.Split(new char[59]) : Array.Empty<string>();
      }
    }

    /// <summary>
    /// Gets the resolved interface types, this <see cref="T:TwinCAT.TypeSystem.IDataType" /> implments.
    /// </summary>
    /// <value>The implements.</value>
    public IInterfaceType?[]? InterfaceImplementations
    {
      get
      {
        if (this._implements == null)
        {
          string[] implementationNames = this.InterfaceImplementationNames;
          if (implementationNames.Length != 0)
          {
            foreach (string str in implementationNames)
              ;
          }
        }
        return this._implements;
      }
    }

    /// <summary>
    /// Gets the Method descriptions for the <see cref="T:TwinCAT.TypeSystem.IRpcCallableType" />
    /// </summary>
    /// <value>The methods.</value>
    /// <remarks>The DataType (Structure) must be marked with the PlcAttribute 'TcRpcEnable' to enable RpcMethods, otherwise
    /// RpcMethods are not passed through to the ADS symbolic information.</remarks>
    public IRpcMethodCollection RpcMethods => (IRpcMethodCollection) this._rpcMethods.AsReadOnly();

    /// <summary>
    /// Gets a value indicating whether this instance has RPC Methods.
    /// </summary>
    /// <value><c>true</c> if this instance has methods; otherwise, <c>false</c>.</value>
    /// <remarks>The DataType (Structure) must be marked with the PlcAttribute 'TcRpcEnable' to enable RpcMethods, otherwise
    /// RpcMethods are not passed through to the ADS symbolic information.</remarks>
    public bool HasRpcMethods => this._rpcMethods.Count > 0;

    internal StructType AddMethod(IRpcMethod method)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      this._rpcMethods.Add(method);
      this.flags = (AdsDataTypeFlags) (this.flags | 2048);
      return this;
    }

    /// <summary>
    /// Called when this <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> is bound via the type binder.
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void OnBound(IBinder binder)
    {
      foreach (Instance member in (InstanceCollection<IMember>) this._members)
        member.Bind(binder);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is derived.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is derived; otherwise, <c>false</c>.
    /// </value>
    public bool IsDerived => this._baseTypeId > 0;

    /// <summary>
    /// Gets a read only collection of the <see cref="T:TwinCAT.TypeSystem.IMember">Members</see> of the <see cref="T:TwinCAT.TypeSystem.IStructType" />.
    /// </summary>
    /// <value>The members as read only collection.</value>
    /// <remarks>
    /// If the <see cref="T:TwinCAT.TypeSystem.IStructType" /> is derived, only the extended members are returned. To get
    /// all supported members down the inheritance chain, use the <see cref="P:TwinCAT.TypeSystem.IInterfaceType.AllMembers" /> property.
    /// </remarks>
    public IMemberCollection Members => (IMemberCollection) new ReadOnlyMemberCollection(this._members);

    /// <summary>Gets the the Name of the Base class (if derived)</summary>
    /// <value>Empty if not derived.</value>
    public string BaseTypeName => this._baseTypeName;

    /// <summary>Gets the structs Base Type (Null if not derived).</summary>
    public IDataType? BaseType
    {
      get
      {
        if (this.IsDerived && this._baseType == null && !string.IsNullOrEmpty(this._baseTypeName) && this.resolver != null)
          ((IBindable2) this).ResolveWithBinder(false, this.resolver);
        return this._baseType;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is resolved.
    /// </summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <value><c>true</c> if this instance is resolved; otherwise, <c>false</c>.</value>
    /// <exclude />
    public override bool IsBindingResolved(bool recurse)
    {
      bool flag = true;
      if (!string.IsNullOrEmpty(this._baseTypeName))
        flag = this._baseType != null;
      if (flag & recurse)
        flag = ((IEnumerable) this.AllMembers).Cast<Member>().All<Member>((Func<Member, bool>) (m => m.IsBindingResolved(recurse)));
      return flag;
    }

    /// <summary>Handler function resolving the DataType binding</summary>
    /// <param name="recurse">if set to <c>true</c> [recurse].</param>
    /// <param name="binder">The binder.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exclude />
    protected override bool OnResolveWithBinder(bool recurse, IBinder binder)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool flag = true;
      if (!this.IsBindingResolved(recurse))
      {
        if (!string.IsNullOrEmpty(this._baseTypeName))
        {
          if (this._baseType == null)
            flag = ((IDataTypeResolver) binder).TryResolveType(this._baseTypeName, ref this._baseType);
          if (this._baseType != null & recurse)
            flag = ((IBindable2) this._baseType).ResolveWithBinder(recurse, binder);
        }
        if (((!flag ? 0 : (this._allMembers != null ? 1 : 0)) & (recurse ? 1 : 0)) != 0)
          flag = this._allMembers.Cast<IBindable2>().Select<IBindable2, bool>((Func<IBindable2, bool>) (b => b.ResolveWithBinder(recurse, binder))).All<bool>((Func<bool, bool>) (b => b));
      }
      return flag;
    }

    /// <summary>
    /// Handler function resolving the DataType binding asynchronously.
    /// </summary>
    /// <param name="recurse">if set to this method resolves all subtypes recursivly.</param>
    /// <param name="binder">The binder.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    /// <exclude />
    protected override async Task<bool> OnResolveWithBinderAsync(
      bool recurse,
      IBinder binder,
      CancellationToken cancel)
    {
      StructType structType = this;
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      bool ret = true;
      if (!structType.IsBindingResolved(recurse))
      {
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable;
        if (!string.IsNullOrEmpty(structType._baseTypeName))
        {
          if (structType._baseType == null)
          {
            ResultValue<IDataType> resultValue = await ((IDataTypeResolver) binder).ResolveTypeAsync(structType._baseTypeName, cancel).ConfigureAwait(false);
            if (((ResultAds) resultValue).Succeeded)
              structType._baseType = resultValue.Value;
          }
          if (structType._baseType != null & recurse)
          {
            configuredTaskAwaitable = ((IBindable2) structType._baseType).ResolveWithBinderAsync(recurse, binder, cancel).ConfigureAwait(false);
            ret = await configuredTaskAwaitable;
          }
        }
        if (ret & recurse && structType._allMembers != null)
        {
          foreach (Member allMember in (InstanceCollection<IMember>) structType._allMembers)
          {
            bool flag = ret;
            configuredTaskAwaitable = ((IBindable2) allMember).ResolveWithBinderAsync(true, binder, cancel).ConfigureAwait(false);
            ret = flag | await configuredTaskAwaitable;
          }
        }
      }
      return ret;
    }

    /// <summary>Gets all members (down the derivation hierarchy)</summary>
    /// <value>All members.</value>
    public IMemberCollection AllMembers
    {
      get
      {
        if (!this.IsDerived)
          return (IMemberCollection) new ReadOnlyMemberCollection(this._members);
        if (this._allMembers == null)
        {
          this._allMembers = new MemberCollection((IEnumerable<IMember>) this._members);
          IDataType baseType = this.BaseType;
          while (baseType != null)
            this._allMembers.AddRange((IEnumerable<IMember>) ((IInterfaceType) baseType).Members);
        }
        return (IMemberCollection) new ReadOnlyMemberCollection(this._allMembers);
      }
    }
  }
}
