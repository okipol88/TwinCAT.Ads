// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.Field
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.ComponentModel;
using System.Text;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Represents a field of an Struct/Alias/Union</summary>
  public class Field : Instance, IField, IAttributedInstance, IInstance, IBitSize
  {
    /// <summary>
    /// The parent <see cref="T:TwinCAT.Ads.TypeSystem.StructType" /> of this Member
    /// </summary>
    /// <exclude />
    private DataType? parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Member" /> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="subEntry">The sub entry.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Field(DataType parent, AdsFieldEntry subEntry)
      : base(subEntry)
    {
      this.parent = parent;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.Field" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    protected internal Field(string name, DataType type)
      : base(name, type, (AdsSymbolFlags) 0, (TypeAttributeCollection) null, string.Empty)
    {
      this.parent = (DataType) null;
    }

    /// <summary>
    /// Gets the Parent of this <see cref="T:TwinCAT.TypeSystem.IField" />.
    /// </summary>
    /// <value>The type of the parent (Alias, Union, Struct)</value>
    public IDataType? ParentType => (IDataType) this.parent;

    /// <summary>
    /// Gets the value encoding of this <see cref="T:TwinCAT.Ads.TypeSystem.Field" />
    /// </summary>
    /// <value>The value encoding.</value>
    public virtual Encoding ValueEncoding
    {
      get
      {
        Encoding valueEncoding = ((IAttributedInstance) this).GetValueEncoding();
        if (valueEncoding != null)
          return valueEncoding;
        IBinder binder = this.Binder;
        return binder != null ? ((ISymbolServer) binder.Provider).DefaultValueEncoding : StringMarshaler.DefaultEncoding;
      }
    }
  }
}
