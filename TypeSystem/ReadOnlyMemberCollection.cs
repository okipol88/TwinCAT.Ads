// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.ReadOnlyMemberCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.TypeSystem.Generic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Read only collection of <see cref="T:TwinCAT.TypeSystem.IMember" /> objects
  /// </summary>
  public class ReadOnlyMemberCollection : 
    ReadOnlyInstanceCollection<IMember>,
    IMemberCollection,
    IInstanceCollection<IMember>,
    IList<IMember>,
    ICollection<IMember>,
    IEnumerable<IMember>,
    IEnumerable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.ReadOnlyMemberCollection" /> class.
    /// </summary>
    /// <param name="members">The members.</param>
    public ReadOnlyMemberCollection(MemberCollection members)
      : base((IInstanceCollection<IMember>) members)
    {
    }

    public bool TryGetMember(string memberName, [NotNullWhen(true)] out IMember? symbol) => ((InstanceCollection<IMember>) this.Items).TryGetInstance(memberName, out symbol);

    /// <summary>
    /// Returns an Empty <see cref="T:TwinCAT.TypeSystem.ReadOnlyMemberCollection" />
    /// </summary>
    /// <returns>ReadOnlyMemberCollection.</returns>
    public static ReadOnlyMemberCollection Empty => MemberCollection.Empty.AsReadOnly();

    /// <summary>Gets the Static Members</summary>
    /// <value>The statics.</value>
    public IInstanceCollection<IMember> Statics => ((MemberCollection) this.Items).Statics;

    /// <summary>Gets the Instance members (non static)</summary>
    /// <value>The instances.</value>
    public IInstanceCollection<IMember> Instances => ((MemberCollection) this.Items).Instances;

    /// <summary>
    /// Calculates the Byte Size of the <see cref="T:TwinCAT.TypeSystem.IMemberCollection" />
    /// </summary>
    /// <returns>System.Int32.</returns>
    /// <remarks>This takes only the instance fields/members into account.</remarks>
    public int CalcSize() => ((MemberCollection) this.Items).CalcSize();
  }
}
