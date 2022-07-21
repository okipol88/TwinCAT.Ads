// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.InterfaceType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Represents an interface type</summary>
  public class InterfaceType : DataType, IInterfaceType, IDataType, IBitSize, IRpcCallableType
  {
    /// <summary>The binder resolved interface types.</summary>
    private IInterfaceType?[]? _implements;
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
    /// <summary>RPC Method description</summary>
    private RpcMethodCollection _rpcMethods = RpcMethodCollection.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.InterfaceType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal InterfaceType(AdsDataTypeEntry entry)
      : base((DataTypeCategory) 16, entry)
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

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.InterfaceType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    internal InterfaceType(string name)
      : base(name, (AdsDataTypeId) 65, (DataTypeCategory) 16, -1, (Type) null)
    {
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
    /// Gets a value indicating whether this instance is derived.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is derived; otherwise, <c>false</c>.
    /// </value>
    public bool IsDerived => this._baseTypeId > 0;

    /// <summary>
    /// Gets a read only collection of the <see cref="T:TwinCAT.TypeSystem.IMember">Members</see> of the <see cref="T:TwinCAT.TypeSystem.IInterfaceType" />.
    /// </summary>
    /// <value>The members as read only collection.</value>
    /// <remarks>
    /// If the <see cref="T:TwinCAT.TypeSystem.IStructType" /> is derived, only the extended members are returned. To get
    /// all supported members down the inheritance chain, use the <see cref="P:TwinCAT.TypeSystem.IInterfaceType.AllMembers" /> property.
    /// </remarks>
    public IMemberCollection Members => (IMemberCollection) new ReadOnlyMemberCollection(this._members);

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
  }
}
