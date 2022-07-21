// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.AlignedMemberCollection
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.Ads.TypeSystem;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>
  /// Class AlignedMemberCollection.
  /// Implements the <see cref="T:TwinCAT.TypeSystem.MemberCollection" />
  /// </summary>
  /// <remarks>The <see cref="T:TwinCAT.TypeSystem.AlignedMemberCollection" /> calculates its member offsets by itself. Dependant on the pack mode</remarks>
  /// <seealso cref="T:TwinCAT.TypeSystem.MemberCollection" />
  public class AlignedMemberCollection : MemberCollection
  {
    private AlignmentCalculator _alignment = AlignmentCalculator.Pack1;

    /// <summary>Returns an Empty Member Collection.</summary>
    /// <returns>MemberCollection.</returns>
    public static AlignedMemberCollection Empty => new AlignedMemberCollection();

    /// <summary>Adds the specified item.</summary>
    /// <param name="item">The item.</param>
    /// <exception cref="T:System.ArgumentNullException">item</exception>
    /// <exception cref="T:System.NotSupportedException">Static members are not supported yet!</exception>
    public override void Add(IMember item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      if (((IInstance) item).IsStatic)
        throw new NotSupportedException("Static members are not supported yet!");
      ((IAlignmentSet) item).SetOffset(this._alignment.GetNextOffset(this.Instances));
      base.Add(item);
    }

    /// <summary>
    /// Inserts the specified <see cref="T:TwinCAT.TypeSystem.IInstance" /> at the specified index.
    /// </summary>
    /// <param name="index">The instance.</param>
    /// <param name="instance">The item.</param>
    /// <exception cref="T:System.NotSupportedException"></exception>
    public override void Insert(int index, IMember instance) => throw new NotSupportedException();

    /// <summary>
    /// Removes the <see cref="T:TwinCAT.TypeSystem.IInstance" /> at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <exception cref="T:System.NotSupportedException"></exception>
    public override void RemoveAt(int index) => throw new NotSupportedException();
  }
}
