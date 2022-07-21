// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.MemberCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Collection of <see cref="T:TwinCAT.TypeSystem.IMember" /> objects.
  /// </summary>
  public class MemberCollection : 
    InstanceCollection<IMember>,
    IMemberCollection,
    IInstanceCollection<IMember>,
    IList<IMember>,
    ICollection<IMember>,
    IEnumerable<IMember>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MemberCollection" /> class.
    /// </summary>
    public MemberCollection()
      : base((InstanceCollectionMode) 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.MemberCollection" /> class (copy constructor)
    /// </summary>
    /// <param name="coll">The coll.</param>
    public MemberCollection(IEnumerable<IMember> coll)
      : base(coll, (InstanceCollectionMode) 0)
    {
    }

    /// <summary>
    /// Returns a read only copy of this collection (shallow copy)
    /// </summary>
    /// <returns>The readonly copy.</returns>
    public ReadOnlyMemberCollection AsReadOnly() => new ReadOnlyMemberCollection(this);

    /// <summary>Returns an Empty Member Collection.</summary>
    /// <returns>MemberCollection.</returns>
    public static MemberCollection Empty => new MemberCollection();

    /// <summary>
    /// Clones this <see cref="T:TwinCAT.TypeSystem.MemberCollection" />.
    /// </summary>
    /// <returns>A cloned <see cref="T:TwinCAT.TypeSystem.MemberCollection" />.</returns>
    public MemberCollection Clone() => new MemberCollection((IEnumerable<IMember>) this);

    public bool TryGetMember(string fieldName, [NotNullWhen(true)] out IMember? symbol) => this.TryGetInstance(fieldName, out symbol);

    /// <summary>
    /// Calculates the Byte Size of the <see cref="T:TwinCAT.TypeSystem.IMemberCollection" />
    /// </summary>
    /// <returns>System.Int32.</returns>
    /// <remarks>This takes only the instance fields/members into account.</remarks>
    public int CalcSize()
    {
      int num = 0;
      if (((ICollection<IMember>) this.Instances).Count > 0)
        num = ((IEnumerable<IMember>) this.Instances).Sum<IMember>((Func<IMember, int>) (m => ((IBitSize) m).ByteSize));
      return num;
    }

    /// <summary>Gets the Instance members (non static)</summary>
    /// <value>The instances.</value>
    public IInstanceCollection<IMember> Instances => (IInstanceCollection<IMember>) new MemberCollection(((IEnumerable<IMember>) this.InnerList).Where<IMember>((Func<IMember, bool>) (m => !((IInstance) m).IsStatic))).AsReadOnly();

    /// <summary>Gets the Static Members</summary>
    /// <value>The statics.</value>
    public IInstanceCollection<IMember> Statics => (IInstanceCollection<IMember>) new MemberCollection(((IEnumerable<IMember>) this.InnerList).Where<IMember>((Func<IMember, bool>) (m => ((IInstance) m).IsStatic))).AsReadOnly();

    /// <summary>Adds the specified item.</summary>
    /// <param name="item">The item.</param>
    public override void Add(IMember item) => base.Add(item);
  }
}
