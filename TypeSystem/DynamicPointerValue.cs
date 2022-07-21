// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicPointerValue
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class DynamicPointerValue.</summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.DynamicValue" />
  public class DynamicPointerValue : DynamicValue
  {
    /// <summary>Pointer Deref indicator</summary>
    internal const string s_pointerDeref = "__deref";
    private object? pointerValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicReferenceValue" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="data">RawData</param>
    /// <param name="timeStamp">The time stamp (UserMode,</param>
    /// <param name="factory">The factory.</param>
    /// <exception cref="T:System.ArgumentNullException">factory
    /// or
    /// symbol</exception>
    /// <exception cref="T:System.ArgumentNullException">symbol</exception>
    internal DynamicPointerValue(
      ISymbol symbol,
      byte[] data,
      DateTimeOffset timeStamp,
      IAccessorValueFactory factory)
      : base(symbol, data, timeStamp, factory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicValue" /> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="data">RawData</param>
    /// <param name="parentValue">The parent value.</param>
    internal DynamicPointerValue(ISymbol symbol, byte[] data, DynamicValue parentValue)
      : base(symbol, data, parentValue)
    {
    }

    /// <summary>Returns the enumeration of all dynamic member names.</summary>
    /// <returns>A sequence that contains dynamic member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames()
    {
      List<string> stringList = new List<string>();
      return (IEnumerable<string>) new List<string>()
      {
        "__deref"
      };
    }

    /// <summary>Tries the get member value.</summary>
    /// <param name="name">The name.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:TwinCAT.TypeSystem.SymbolException"></exception>
    public override bool TryGetMemberValue(string name, [NotNullWhen(true)] out object? result)
    {
      result = (object) null;
      if (this.ResolvedType != null && this.ResolvedType.Category == 13 && name == "__deref")
      {
        IDynamicSymbol reference = (IDynamicSymbol) ((IPointerInstance) this.Symbol).Reference;
        result = this.ResolvePointerValue();
      }
      return result != null;
    }

    internal object? ResolvePointerValue()
    {
      if (this.pointerValue == null)
      {
        IAccessorDynamicValue dynamicValueAccessor = this.DynamicValueAccessor;
        if (dynamicValueAccessor != null)
        {
          ISymbol reference = ((IPointerInstance) this.Symbol).Reference;
          if (reference != null)
          {
            DateTimeOffset? nullable;
            this.pointerValue = ((IAccessorValue) dynamicValueAccessor).ReadValue(reference, ref nullable);
          }
        }
      }
      return this.pointerValue;
    }

    private IAccessorDynamicValue? DynamicValueAccessor => this.ValueFactory is IAccessorValueFactory2 valueFactory ? valueFactory.ValueAccessor as IAccessorDynamicValue : (IAccessorDynamicValue) null;
  }
}
