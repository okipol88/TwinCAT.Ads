// Decompiled with JetBrains decompiler
// Type: TwinCAT.ValueAccess.DynamicValueFactory
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.ComponentModel;
using TwinCAT.PlcOpen;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.ValueAccess
{
  /// <summary>Class DynamicValueFactory.</summary>
  /// <seealso cref="T:TwinCAT.ValueAccess.IAccessorValueFactory" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DynamicValueFactory : ValueFactory
  {
    /// <summary>The value update mode</summary>
    private ValueUpdateMode _updateMode = (ValueUpdateMode) 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.DynamicValueFactory" /> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public DynamicValueFactory(SymbolLoaderSettings settings)
      : base(settings.ValueCreation)
    {
      this._updateMode = settings.ValueUpdateMode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.ValueAccess.DynamicValueFactory" /> class.
    /// </summary>
    /// <param name="valueCreationMode">The value creation mode.</param>
    public DynamicValueFactory(ValueCreationModes valueCreationMode)
      : base(valueCreationMode)
    {
      this._updateMode = (ValueUpdateMode) 1;
    }

    /// <summary>Gets the Value update mode.</summary>
    /// <value>The update mode.</value>
    public ValueUpdateMode UpdateMode => this._updateMode;

    private bool canResolveToPrimitive(IDataType dataType)
    {
      if (dataType.IsPrimitive)
        return true;
      if (dataType.Category != 4)
        return false;
      IDataType elementType = ((IArrayType) dataType).ElementType;
      return elementType != null && this.canResolveToPrimitive(elementType);
    }

    /// <summary>Creates the symbols value from raw memory data</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="sourceData">Memory data.</param>
    /// <param name="timeStamp">The timestamp of the memory data.</param>
    /// <returns>The unmarshalled value object.</returns>
    public override object CreateValue(
      ISymbol symbol,
      ReadOnlySpan<byte> sourceData,
      DateTimeOffset timeStamp)
    {
      IDataType idataType1 = symbol != null ? ((IInstance) symbol).DataType : throw new ArgumentNullException(nameof (symbol));
      if (((IInstance) symbol).DataType is IResolvableType dataType)
        idataType1 = dataType.ResolveType((DataTypeResolveStrategy) 1);
      if (idataType1 == null)
        throw new CannotResolveDataTypeException(((IInstance) symbol).TypeName);
      object obj;
      if (!((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 4))
      {
        if (this.canResolveToPrimitive(idataType1))
        {
          if (idataType1.Category == 3 && ((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 2))
            obj = (object) EnumValueFactory.Create((IEnumType) idataType1, sourceData);
          else if (((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 1))
          {
            this.ValueConverter.Unmarshal((IAttributedInstance) symbol, sourceData, (Type) null, ref obj);
            if ((this.Mode & 8) == null && obj is IPlcOpenTimeBase)
              obj = ((IPlcOpenTimeBase) obj).UntypedValue;
          }
          else
            obj = (object) new DynamicValue(symbol, sourceData.ToArray(), timeStamp, (IAccessorValueFactory) this);
        }
        else if (idataType1.Category == 4 && ((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 1) && !((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 2))
        {
          IArrayType iarrayType = (IArrayType) idataType1;
          IDataType idataType2 = iarrayType.ElementType;
          if (idataType2 is IResolvableType iresolvableType)
            idataType2 = iresolvableType.ResolveType((DataTypeResolveStrategy) 1);
          if (idataType2 == null)
            throw new CannotResolveDataTypeException(iarrayType.ElementTypeName);
          if (((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 1) && idataType2.IsPrimitive)
          {
            if (((IInstance) symbol).IsReference)
              ;
            obj = (object) this.createPrimitiveValueArray((IArrayInstance) symbol, sourceData, timeStamp);
          }
          else
            obj = (object) new DynamicValue(symbol, sourceData.ToArray(), timeStamp, (IAccessorValueFactory) this);
        }
        else
          obj = !idataType1.IsReference ? (!((IInstance) symbol).IsPointer ? (object) new DynamicValue(symbol, sourceData.ToArray(), timeStamp, (IAccessorValueFactory) this) : (object) new DynamicPointerValue(symbol, sourceData.ToArray(), timeStamp, (IAccessorValueFactory) this)) : (object) new DynamicReferenceValue(symbol, sourceData.ToArray(), timeStamp, (IAccessorValueFactory) this);
      }
      else if (((IInstance) symbol).IsPointer)
        this.ValueConverter.Unmarshal((IAttributedInstance) symbol, sourceData, (Type) null, ref obj);
      else
        obj = !((IInstance) symbol).IsReference ? (!((IInstance) symbol).IsPointer ? (object) new DynamicValue(symbol, sourceData.ToArray(), timeStamp, (IAccessorValueFactory) this) : (object) new DynamicPointerValue(symbol, sourceData.ToArray(), timeStamp, (IAccessorValueFactory) this)) : (object) new DynamicReferenceValue(symbol, sourceData.ToArray(), timeStamp, (IAccessorValueFactory) this);
      return obj;
    }

    private Array createPrimitiveValueArray(
      IArrayInstance arraySymbol,
      ReadOnlySpan<byte> data,
      DateTimeOffset utcReadTime)
    {
      IArrayType dataType1 = (IArrayType) ((IInstance) arraySymbol).DataType;
      IDataType dataType2 = dataType1 != null ? dataType1.ElementType : throw new CannotResolveDataTypeException(((IInstance) arraySymbol).TypeName);
      if (dataType2 is IResolvableType iresolvableType)
        dataType2 = iresolvableType.ResolveType((DataTypeResolveStrategy) 1);
      if (dataType2 == null)
        throw new CannotResolveDataTypeException(dataType1.ElementTypeName);
      int elementCount = dataType1.Dimensions.ElementCount;
      Type managed = (Type) null;
      int[] dimensionLengths = dataType1.Dimensions.GetDimensionLengths();
      Array primitiveValueArray = !dataType2.TryGetManagedType(out managed) ? Array.CreateInstance(typeof (object), dimensionLengths) : Array.CreateInstance(managed, dimensionLengths);
      foreach (int[] indices in new ArrayIndexIterator(dataType1, true))
      {
        int subIndex = ArrayIndexConverter.IndicesToSubIndex(indices, dataType1, true);
        object obj = this.CreateValue(((IList<ISymbol>) ((ISymbol) arraySymbol).SubSymbols)[subIndex], data.Slice(subIndex * ((IBitSize) dataType2).ByteSize, ((IBitSize) dataType2).ByteSize), utcReadTime);
        primitiveValueArray.SetValue(obj, indices);
      }
      return primitiveValueArray;
    }

    /// <summary>Creates the  symbols value from raw memory data.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="sourceData">The data.</param>
    /// <param name="parent">The parent value</param>
    /// <param name="timeStamp">The time stamp.</param>
    /// <returns>System.Object.</returns>
    /// <remarks>The <paramref name="parent" /> argument is used to organize values in hierarchies, equally to the corresponding symbol/instance trees.</remarks>
    public override object CreateValue(
      ISymbol symbol,
      ReadOnlySpan<byte> sourceData,
      IValue parent,
      DateTimeOffset timeStamp)
    {
      IDataType enumType = symbol != null ? ((IInstance) symbol).DataType : throw new ArgumentNullException(nameof (symbol));
      if (((IInstance) symbol).DataType is IResolvableType dataType)
        enumType = dataType.ResolveType((DataTypeResolveStrategy) 1);
      if (enumType == null)
        throw new CannotResolveDataTypeException(((IInstance) symbol).TypeName);
      object obj;
      if (!((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 4) && enumType.IsPrimitive)
      {
        if (enumType.Category == 3 && ((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 2))
          obj = (object) EnumValueFactory.Create((IEnumType) enumType, sourceData);
        else if (((Enum) (object) this.Mode).HasFlag((Enum) (object) (ValueCreationModes) 1))
          this.ValueConverter.Unmarshal((IAttributedInstance) symbol, sourceData, (Type) null, ref obj);
        else
          obj = (object) new DynamicValue(symbol, sourceData.ToArray(), (DynamicValue) parent);
      }
      else if (((IInstance) symbol).IsReference)
      {
        if (this.ValueAccessor is IAccessorDynamicValue valueAccessor)
        {
          byte[] array = sourceData.ToArray();
          DateTimeOffset? nullable;
          obj = ((IAccessorRawValue) valueAccessor).TryReadRaw(symbol, (Memory<byte>) array, ref nullable) != 0 ? (object) new DynamicValue(symbol, sourceData.ToArray(), (DynamicValue) parent) : (object) new DynamicReferenceValue(symbol, array, (DynamicValue) parent);
        }
        else
          obj = (object) new DynamicValue(symbol, sourceData.ToArray(), (DynamicValue) parent);
      }
      else
        obj = !((IInstance) symbol).IsPointer ? (object) new DynamicValue(symbol, sourceData.ToArray(), (DynamicValue) parent) : (object) new DynamicPointerValue(symbol, sourceData.ToArray(), (DynamicValue) parent);
      return obj;
    }
  }
}
