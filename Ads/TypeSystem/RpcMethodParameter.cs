// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.RpcMethodParameter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Diagnostics;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Class RpcMethodParameter.</summary>
  [DebuggerDisplay("Name = { _name}, TypeName = { _typeName }, Size = { _size }, Flags = { _flags }, LengthIdx = { _lengthParaIndex }")]
  public class RpcMethodParameter : IRpcMethodParameter
  {
    /// <summary>size of datatype ( in bytes )</summary>
    private int _size;
    /// <summary>size of biggest element for alignment</summary>
    private int _alignSize;
    /// <summary>adsDataType of symbol (if alias)</summary>
    private AdsDataTypeId _dataTypeId;
    /// <summary>Method Parameter Flags</summary>
    private MethodParamFlags _flags;
    /// <summary>Data Type Guid</summary>
    private Guid _typeGuid;
    /// <summary>The _length is para</summary>
    /// <remarks>This field references to the Parameter that defines the length for this
    /// generic one. Equally to the marshalling attributes of COM (sizeof, lenght)
    /// this enables to transport parameter of type (PVOID)
    /// </remarks>
    private int _lengthParaIndex;
    /// <summary>Name of datatype with terminating \0</summary>
    private string _name;
    /// <summary>type name of dataitem</summary>
    private string _typeName;
    /// <summary>Parameter Comment</summary>
    private string _comment;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.RpcMethodParameter" /> class.
    /// </summary>
    /// <param name="paraInfo">The para information.</param>
    internal RpcMethodParameter(AdsMethodParaInfo paraInfo)
    {
      this._name = paraInfo.name;
      this._size = (int) paraInfo.size;
      this._alignSize = (int) paraInfo.alignSize;
      this._dataTypeId = paraInfo.dataType;
      this._flags = paraInfo.flags;
      this._typeGuid = paraInfo.typeGuid;
      this._lengthParaIndex = (int) paraInfo.lengthIsPara;
      this._name = paraInfo.name;
      this._typeName = paraInfo.type;
      this._comment = paraInfo.comment;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.RpcMethodParameter" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="flags">The flags.</param>
    public RpcMethodParameter(string name, IDataType type, MethodParamFlags flags)
      : this(name, type, flags, 0, (string) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.RpcMethodParameter" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="lengthIsIndex">Index of the length is.</param>
    public RpcMethodParameter(
      string name,
      IDataType type,
      MethodParamFlags flags,
      int lengthIsIndex)
      : this(name, type, flags, lengthIsIndex, (string) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.RpcMethodParameter" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="flags">The flags.</param>
    /// <param name="lengthIsIndex">Index of the length is.</param>
    /// <param name="comment">The comment.</param>
    public RpcMethodParameter(
      string name,
      IDataType type,
      MethodParamFlags flags,
      int lengthIsIndex,
      string? comment)
    {
      this._name = name;
      this._size = ((IBitSize) type).ByteSize;
      this._alignSize = ((IBitSize) type).ByteSize;
      this._dataTypeId = ((DataType) type).DataTypeId;
      this._flags = flags;
      this._typeGuid = Guid.Empty;
      this._lengthParaIndex = lengthIsIndex;
      this._typeName = type.Name;
      this._comment = comment ?? string.Empty;
    }

    /// <summary>
    /// Gets the size of the <see cref="T:TwinCAT.Ads.TypeSystem.RpcMethodParameter" />
    /// </summary>
    /// <value>The size.</value>
    public int Size => this._size;

    /// <summary>Gets the size of biggest element for alignment</summary>
    /// <value>The size of the align.</value>
    public int AlignSize => this._alignSize;

    /// <summary>Gets the parameter flags.</summary>
    /// <value>The parameter flags.</value>
    public MethodParamFlags ParameterFlags => this._flags;

    /// <summary>
    /// Gets the Unique identifier of the parameters data type.
    /// </summary>
    /// <value>The type unique identifier.</value>
    public Guid TypeGuid => this._typeGuid;

    /// <summary>
    /// Gets the index of the LengthIs parameter (within the MethodParameter List)
    /// </summary>
    /// <value>The index of the length is parameter.</value>
    /// <remarks>This field references to the Parameter that defines the length for this
    /// generic one. Equally to the marshalling attributes of COM (sizeof, length)
    /// this enables to transport parameter of type (PVOID)</remarks>
    public int LengthIsParameterIndex => this._lengthParaIndex;

    /// <summary>
    /// Gets a value indicating whether this instance has a related LengthIs Parameter.
    /// </summary>
    /// <value><c>true</c> if this instance has a LengthIs parameter; otherwise, <c>false</c>.</value>
    public bool HasLengthIsParameter => this._lengthParaIndex != 0;

    /// <summary>Gets the Parameter Name</summary>
    /// <value>The name.</value>
    public string Name => this._name;

    /// <summary>Gets the Data type of the Parameter</summary>
    /// <value>The type.</value>
    public string TypeName => this._typeName;

    /// <summary>Gets the Parameter Comment.</summary>
    /// <value>The comment.</value>
    public string Comment => this._comment;

    /// <summary>
    /// Gets a value indicating whether this instance is input.
    /// </summary>
    /// <value><c>true</c> if this instance is input; otherwise, <c>false</c>.</value>
    public bool IsInput => ((Enum) (object) this._flags).HasFlag((Enum) (object) (MethodParamFlags) 1);

    /// <summary>
    /// Gets a value indicating whether this instance is output.
    /// </summary>
    /// <value><c>true</c> if this instance is output; otherwise, <c>false</c>.</value>
    public bool IsOutput => ((Enum) (object) this._flags).HasFlag((Enum) (object) (MethodParamFlags) 2);

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents this instance.</returns>
    public override string ToString() => this._name + " : " + this._typeName;
  }
}
