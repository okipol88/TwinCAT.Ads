// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.Member
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using System.Diagnostics;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Represents a member of an <see cref="T:TwinCAT.Ads.TypeSystem.StructType" />
  /// </summary>
  [DebuggerDisplay("Name = { instanceName }, Type = {typeName}, Size = {size}, Offset = {offset}, Category = {category}, Static = {staticAddress}")]
  public sealed class Member : 
    Field,
    IMember,
    IField,
    IAttributedInstance,
    IInstance,
    IBitSize,
    IAlignmentSet
  {
    /// <summary>
    /// The offset of the <see cref="T:TwinCAT.Ads.TypeSystem.Member" /> within the parent <see cref="T:TwinCAT.Ads.TypeSystem.StructType" /> in bits or bytes.
    /// </summary>
    /// <exclude />
    private int offset = -1;
    private AdsDataTypeFlags _memberFlags;
    private uint _typeHashValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Member" /> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="subEntry">The sub entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Member(DataType parent, AdsFieldEntry subEntry)
      : base(parent, subEntry)
    {
      this.offset = subEntry != null ? subEntry.Offset : throw new ArgumentNullException(nameof (subEntry));
      this._memberFlags = subEntry.Flags;
      this._typeHashValue = subEntry.TypeHashValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Member" /> class.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="type">The type.</param>
    public Member(string fieldName, DataType type)
      : base(fieldName, type)
    {
      this._memberFlags = (AdsDataTypeFlags) 0;
    }

    /// <summary>
    /// Gets the offset of the <see cref="T:TwinCAT.Ads.TypeSystem.Member" /> within the parent <see cref="T:TwinCAT.Ads.TypeSystem.StructType" /> in bits or bytes dependent on <see cref="P:TwinCAT.Ads.TypeSystem.Instance.IsBitType" />
    /// </summary>
    /// <value>The offset.</value>
    public int Offset => this.offset;

    /// <summary>Gets the bit offset.</summary>
    /// <value>The bit offset.</value>
    public int BitOffset => this.IsBitType ? this.offset : this.offset * 8;

    /// <summary>Gets the byte offset.</summary>
    /// <value>The byte offset.</value>
    public int ByteOffset => this.IsBitType ? this.offset / 8 : this.offset;

    /// <summary>Gets the Field / Member Flags</summary>
    /// <value>The member flags.</value>
    internal AdsDataTypeFlags MemberFlags => this._memberFlags;

    /// <summary>Gets the type hash value (Only for Properties ???)</summary>
    /// <value>The type hash value.</value>
    internal uint TypeHashValue => this._typeHashValue;

    /// <summary>Sets the offset.</summary>
    /// <param name="offset">The offset.</param>
    /// <exception cref="T:System.NotImplementedException"></exception>
    /// <exclude />
    public void SetOffset(int offset) => this.offset = offset;
  }
}
