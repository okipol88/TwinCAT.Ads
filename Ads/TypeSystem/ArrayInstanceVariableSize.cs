// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.ArrayInstanceVariableSize
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Class AnySizeArrayInstance.
  /// Implements the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" />
  /// </summary>
  /// <remarks>This ArrayInstance type is used for so called 'AnySize' arrays. That means
  /// array instances that referenced by pointers that are marked with the 'TcArraySize' attribute.
  /// The field reference in the attribute is used to specify the size of the referenced Array dynamically
  /// during runtime.
  /// </remarks>
  /// <seealso cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" />
  /// <exclude />
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class ArrayInstanceVariableSize : ArrayInstance, IArrayInstanceVariableSize
  {
    private ArrayType _dynamicBaseType;
    private ISymbol? _lengthIsSymbol;
    /// <summary>The cached dim lengths</summary>
    private int[]? _cachedDimLengths;
    /// <summary>
    /// Indicates, that the dynamic type was read at least one time.
    /// </summary>
    private bool _dynamicTypeRead;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstance" /> class.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="type">The type.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="fieldOffset">The field offset.</param>
    internal ArrayInstanceVariableSize(
      string instanceName,
      IArrayType type,
      ISymbol parent,
      int fieldOffset)
      : base(instanceName, type, parent, fieldOffset)
    {
      this.Category = (DataTypeCategory) 4;
      this._dynamicBaseType = (ArrayType) type;
    }

    /// <summary>Gets the parent pointer Symbol</summary>
    /// <value>The parent pointer.</value>
    private PointerInstance? ParentPointer => (PointerInstance) this.Parent;

    /// <summary>Gets the name of the 'TcArrayLengthIs' field</summary>
    /// <value>The name of the tc array size field.</value>
    private string TcArraySizeFieldName
    {
      get
      {
        PointerInstance parentPointer = this.ParentPointer;
        string str = (string) null;
        if ((Symbol) parentPointer != (Symbol) null)
          parentPointer.Attributes.TryGetValue("TcArrayLengthIs", ref str);
        return str ?? string.Empty;
      }
    }

    /// <summary>Gets the parent structure.</summary>
    /// <value>The parent structure.</value>
    private IStructInstance? ParentStruct
    {
      get
      {
        IPointerInstance parentPointer = (IPointerInstance) this.ParentPointer;
        return parentPointer != null ? (IStructInstance) ((ISymbol) parentPointer).Parent : (IStructInstance) null;
      }
    }

    /// <summary>Gets the 'LengthIs' symbol</summary>
    /// <value>The array size symbol.</value>
    /// <remarks>This can be symbol of an Primitive value (e.g INT, for one index) or
    /// an Array of primtive values (e.g. ARRAY OF INT) </remarks>
    public ISymbol? LengthIsSymbol
    {
      get
      {
        if (this._lengthIsSymbol == null)
        {
          IStructInstance parentStruct = this.ParentStruct;
          string arraySizeFieldName = this.TcArraySizeFieldName;
          if (parentStruct != null && !string.IsNullOrEmpty(arraySizeFieldName))
          {
            IList<ISymbol> isymbolList = (IList<ISymbol>) null;
            if (((IInstanceCollection<ISymbol>) ((IInterfaceInstance) parentStruct).MemberInstances).TryGetInstanceByName(arraySizeFieldName, ref isymbolList) && ((ICollection<ISymbol>) isymbolList).Count == 1)
              this._lengthIsSymbol = isymbolList[0];
          }
        }
        return this._lengthIsSymbol;
      }
    }

    /// <summary>Handler function getting the SubSymbols</summary>
    /// <returns>ISymbolCollection.</returns>
    /// <remarks>
    /// The SubSymbols are determined instead of the WeakReference Cache of the base implementation.
    /// </remarks>
    /// <exclude />
    protected override ISymbolCollection<ISymbol> OnGetSubSymbols()
    {
      ISymbolCollection<ISymbol> subSymbols = this.OnCreateSubSymbols((ISymbol) this);
      this.subSymbols = new WeakReference<ISymbolCollection<ISymbol>>(subSymbols);
      return subSymbols;
    }

    /// <summary>
    /// Handler function determining the SubSymbolCode (optimized)
    /// </summary>
    /// <returns>System.Int32.</returns>
    /// <exclude />
    /// <remarks>This overload enforces that, the SubSymbolCount will be recreated on access, and not be cached as coded
    /// in the base implementation.</remarks>
    protected override int OnGetSubSymbolCount() => this.OnGetSubSymbolCount((ISymbol) this);

    /// <summary>Gets the Array of Dynamic Dim Lengths (cached).</summary>
    /// <value>The dynamic dim lengths.</value>
    public int[] DynamicDimLengths => this._cachedDimLengths != null ? this._cachedDimLengths : Array.Empty<int>();

    /// <summary>
    /// Tries to read TcLengthIs symbol, that is referenced by this <see cref="T:TwinCAT.Ads.TypeSystem.ArrayInstanceVariableSize" /> array.
    /// </summary>
    /// <param name="lengthIsValue">The length is value.</param>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryReadLengthIs(out int[]? lengthIsValue)
    {
      AdsErrorCode adsErrorCode = (AdsErrorCode) 0;
      object obj = (object) null;
      lengthIsValue = (int[]) null;
      IValueSymbol lengthIsSymbol = (IValueSymbol) this.LengthIsSymbol;
      if (lengthIsSymbol != null)
      {
        try
        {
          adsErrorCode = (AdsErrorCode) lengthIsSymbol.TryReadValue(200, ref obj);
          if (adsErrorCode == null)
          {
            object sourceValue = obj;
            bool flag1 = PrimitiveTypeMarshaler.CanConvert(sourceValue, typeof (int));
            bool flag2 = PrimitiveTypeMarshaler.CanConvert(sourceValue, typeof (int[]));
            if (flag1)
            {
              int num = PrimitiveTypeMarshaler.Convert<int>(sourceValue);
              lengthIsValue = new int[1]{ num };
              this._cachedDimLengths = lengthIsValue;
            }
            else if (flag2)
            {
              lengthIsValue = PrimitiveTypeMarshaler.Convert<int[]>(sourceValue);
              this._cachedDimLengths = lengthIsValue;
            }
            else
              adsErrorCode = (AdsErrorCode) 1;
          }
        }
        catch (Exception ex)
        {
          adsErrorCode = (AdsErrorCode) 1;
        }
      }
      return adsErrorCode;
    }

    private async Task<ResultAnyValue> ReadArraySizeAsync(CancellationToken cancel)
    {
      IValueSymbol lengthIsSymbol = (IValueSymbol) this.LengthIsSymbol;
      if (lengthIsSymbol == null)
        return ResultAnyValue.Empty;
      ResultReadValueAccess resultReadValueAccess = await lengthIsSymbol.ReadValueAsync(cancel).ConfigureAwait(false);
      return new ResultAnyValue((AdsErrorCode) ((ResultAccess) resultReadValueAccess).ErrorCode, ((ResultReadValueAccess<object>) resultReadValueAccess).Value, ((ResultAccess) resultReadValueAccess).InvokeId);
    }

    /// <summary>
    /// Handler function getting the size of the <see cref="T:TwinCAT.Ads.TypeSystem.Instance" />
    /// </summary>
    /// <returns>System.Int32.</returns>
    protected override int OnGetSize()
    {
      if (this.InternalSize == 0)
        this.TryUpdateDimensions();
      return this.InternalSize;
    }

    /// <summary>
    /// Handler function is called, when SubSymbols (the elements) are called.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exclude />
    protected override bool OnAccessSubSymbols() => this.TryUpdateDimensions() == 0;

    /// <summary>Updates the dimensions of this VariableSize Array</summary>
    /// <returns>AdsErrorCode.</returns>
    public AdsErrorCode TryUpdateDimensions()
    {
      int[] cachedDimLengths = this._cachedDimLengths;
      int[] lengthIsValue = (int[]) null;
      AdsErrorCode adsErrorCode = this.TryReadLengthIs(out lengthIsValue);
      if (adsErrorCode == null)
      {
        this._dynamicTypeRead = true;
        if (!((IStructuralEquatable) lengthIsValue).Equals((object) cachedDimLengths, StructuralComparisons.StructuralEqualityComparer))
        {
          this.DataType = (IDataType) new ArrayTypeVariableSize(this._dynamicBaseType, 0, (IArrayInstanceVariableSize) this);
          this.Size = ((IBitSize) this.DataType).Size;
        }
      }
      return adsErrorCode;
    }

    /// <summary>Handler function for reading the raw value</summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>System.Byte[].</returns>
    protected override byte[] OnReadRawValue(int timeout)
    {
      this.TryUpdateDimensions();
      return base.OnReadRawValue(timeout);
    }

    protected override Task<ResultReadRawAccess> OnReadRawValueAsync(
      CancellationToken cancel)
    {
      this.TryUpdateDimensions();
      return base.OnReadRawValueAsync(cancel);
    }

    /// <summary>Handler function for reading the dynamic value.</summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The Value</returns>
    protected override object OnReadValue(int timeout)
    {
      this.TryUpdateDimensions();
      return base.OnReadValue(timeout);
    }

    /// <summary>Handler function for writing the RawValue</summary>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>The Value</returns>
    protected override Task<ResultReadValueAccess> OnReadValueAsync(
      CancellationToken cancel)
    {
      this.TryUpdateDimensions();
      return base.OnReadValueAsync(cancel);
    }

    /// <summary>Handler function for writing the RawValue</summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout.</param>
    protected override void OnWriteRawValue(ReadOnlyMemory<byte> value, int timeout)
    {
      this.TryUpdateDimensions();
      base.OnWriteRawValue(value, timeout);
    }

    protected override Task<ResultWriteAccess> OnWriteRawValueAsync(
      ReadOnlyMemory<byte> value,
      CancellationToken cancel)
    {
      this.TryUpdateDimensions();
      return base.OnWriteRawValueAsync(value, cancel);
    }

    /// <summary>Handler function for writing the dynamic value</summary>
    /// <param name="value">The value.</param>
    /// <param name="timeout">The timeout.</param>
    protected override void OnWriteValue(object value, int timeout)
    {
      this.TryUpdateDimensions();
      base.OnWriteValue(value, timeout);
    }

    protected override Task<ResultWriteAccess> OnWriteValueAsync(
      object value,
      CancellationToken cancel)
    {
      this.TryUpdateDimensions();
      return base.OnWriteValueAsync(value, cancel);
    }

    /// <summary>
    /// Tries to resolve the <see cref="T:TwinCAT.TypeSystem.IDataType" />.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected override bool TryResolveType()
    {
      if (!this._dynamicTypeRead)
        this.TryUpdateDimensions();
      return base.TryResolveType();
    }

    public override bool TryGetElement(int[] indices, [NotNullWhen(true)] out ISymbol? symbol)
    {
      if (!this._dynamicTypeRead)
        this.TryUpdateDimensions();
      return base.TryGetElement(indices, out symbol);
    }
  }
}
