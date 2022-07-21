// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.AdsFieldEntry
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class AdsFieldEntry (for internal use only)</summary>
  /// <seealso cref="T:TwinCAT.Ads.Internal.AdsDataTypeEntry" />
  /// <seealso cref="T:TwinCAT.Ads.Internal.IAdsCustomMarshal`1" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class AdsFieldEntry : AdsDataTypeEntry, IAdsCustomMarshal<AdsFieldEntry>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsFieldEntry" /> class.
    /// </summary>
    public AdsFieldEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.AdsFieldEntry" /> class.
    /// </summary>
    /// <param name="member">The member.</param>
    internal AdsFieldEntry(IMember member)
      : base(member)
    {
    }

    /// <summary>Gets the name of the sub item.</summary>
    /// <value>The name of the sub item.</value>
    public string SubItemName => this.EntryName;

    /// <summary>Marshals the specified marshaler.</summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <param name="buffer">The buffer.</param>
    /// <returns>System.Int32.</returns>
    public override int Marshal(IStringMarshaler marshaler, Span<byte> buffer) => base.Marshal(marshaler, buffer);

    /// <summary>Marshals the size.</summary>
    /// <param name="marshaler">The marshaler.</param>
    /// <returns>System.Int32.</returns>
    public override int MarshalSize(IStringMarshaler marshaler) => base.MarshalSize(marshaler);

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:TwinCAT.Ads.Internal.AdsFieldEntry" /> is static.
    /// </summary>
    /// <value><c>true</c> if this instance is static; otherwise, <c>false</c>.</value>
    public bool IsStatic => (this.Flags & 131072) > 0;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:TwinCAT.Ads.Internal.AdsFieldEntry" /> is static.
    /// </summary>
    /// <value><c>true</c> if this instance is static; otherwise, <c>false</c>.</value>
    public bool IsProperty => (this.Flags & 64) > 0;

    /// <summary>
    /// Determines whether this object is an SubItem (DataType Member, true) or a native DataType (false)
    /// </summary>
    /// <value><c>true</c> if this instance is sub item; otherwise, <c>false</c>.</value>
    public override bool IsSubItem => true;
  }
}
