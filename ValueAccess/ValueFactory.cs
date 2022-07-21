// Decompiled with JetBrains decompiler
// Type: TwinCAT.ValueAccess.ValueFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TwinCAT.PlcOpen;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.ValueAccess
{
  /// <summary>Value Factory</summary>
  /// <seealso cref="T:TwinCAT.ValueAccess.IAccessorValueFactory" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ValueFactory : IAccessorValueFactory2, IAccessorValueFactory
  {
    /// <summary>The Value Creation mode</summary>
    private ValueCreationModes mode = (ValueCreationModes) 1;
    /// <summary>The value converter / Marshaller</summary>
    private ISymbolMarshaler valueConverter;
    /// <summary>A Backlink to the Value Accessor.</summary>
    /// <remarks>This enables the DynamicValueFactory to Dereference References 'On the fly'</remarks>
    /// <exclude />
    private IAccessorRawValue? accessor;

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.ValueAccess.ValueCreationModes" />
    /// </summary>
    /// <value>The mode.</value>
    public ValueCreationModes Mode => this.mode;

    /// <summary>The value converter / Marshaller</summary>
    public ISymbolMarshaler ValueConverter => this.valueConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.ValueFactory" /> class.
    /// </summary>
    /// <param name="mode">The mode.</param>
    public ValueFactory(ValueCreationModes mode)
    {
      this.mode = mode;
      this.valueConverter = (ISymbolMarshaler) new InstanceValueMarshaler();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.ValueFactory" /> class.
    /// </summary>
    public ValueFactory()
    {
      this.mode = (ValueCreationModes) 1;
      this.valueConverter = (ISymbolMarshaler) new InstanceValueMarshaler();
    }

    /// <summary>Sets the value accessor</summary>
    /// <param name="accessor">The accessor.</param>
    /// <exclude />
    public void SetValueAccessor(IAccessorRawValue accessor) => this.accessor = accessor;

    /// <summary>Gets the value accessor.</summary>
    /// <value>The value accessor or NULL</value>
    /// <remarks>The Value accessor can be used for the possibility to Read Values on ValueAccess on the Fly.
    /// E.g. when dereferencing ReferenceTypes on property access.
    /// The 'on-the-fly' access is optional and doesn't have to be supported, but the DynamicValueFactory can use if available.</remarks>
    /// <exclude />
    public IAccessorRawValue? ValueAccessor => this.accessor;

    /// <summary>
    /// Creates a primitive value, independent of any settings.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="sourceData">The source data.</param>
    /// <param name="timeStamp">The time stamp.</param>
    /// <returns>A primitive value.</returns>
    public object CreatePrimitiveValue(
      ISymbol symbol,
      ReadOnlySpan<byte> sourceData,
      DateTimeOffset timeStamp)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      Encoding encoding = (Encoding) null;
      EncodingAttributeConverter.TryGetEncoding((IEnumerable<ITypeAttribute>) ((IAttributedInstance) symbol).Attributes, out encoding);
      object primitiveValue;
      this.valueConverter.Unmarshal((IAttributedInstance) symbol, sourceData, (Type) null, ref primitiveValue);
      return primitiveValue;
    }

    /// <summary>Creates the value.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="source">The source memory.</param>
    /// <param name="parent">The parent value.</param>
    /// <param name="timeStamp">The time stamp.</param>
    /// <returns>System.Object.</returns>
    public virtual object CreateValue(
      ISymbol symbol,
      ReadOnlySpan<byte> source,
      IValue parent,
      DateTimeOffset timeStamp)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      return this.CreateValue(symbol, source, parent.TimeStamp);
    }

    /// <summary>Creates the value.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="source">The source memory.</param>
    /// <param name="timeStamp">The time stamp.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">data</exception>
    public virtual object CreateValue(
      ISymbol symbol,
      ReadOnlySpan<byte> source,
      DateTimeOffset timeStamp)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      if (source.Length != symbol.GetValueMarshalSize())
        throw new ArgumentOutOfRangeException(nameof (source));
      object obj;
      if (this.valueConverter.CanMarshal((IAttributedInstance) symbol, (Type) null))
      {
        IDataType enumType = ((IInstance) symbol).DataType;
        if (((IInstance) symbol).DataType is IResolvableType dataType)
          enumType = dataType.ResolveType((DataTypeResolveStrategy) 1);
        if (symbol.Category == 3 && (this.mode & 2) > 0)
        {
          obj = (object) EnumValueFactory.Create((IEnumType) enumType, source);
        }
        else
        {
          this.valueConverter.Unmarshal((IAttributedInstance) symbol, source, (Type) null, ref obj);
          if ((this.mode & 8) == null && obj is IPlcOpenTimeBase)
            obj = ((IPlcOpenTimeBase) obj).UntypedValue;
        }
      }
      else
        obj = (object) source.ToArray();
      return obj;
    }
  }
}
