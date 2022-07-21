// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicReferenceValue
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class DynamicReferenceValue.</summary>
  /// <seealso cref="T:TwinCAT.TypeSystem.DynamicValue" />
  public class DynamicReferenceValue : DynamicValue
  {
    private object? referenceValue;

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
    internal DynamicReferenceValue(
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
    internal DynamicReferenceValue(ISymbol symbol, byte[] data, DynamicValue parentValue)
      : base(symbol, data, parentValue)
    {
    }

    private object? ResolveReferenceValue()
    {
      if (this.referenceValue == null)
      {
        IAccessorDynamicValue dynamicValueAccessor = this.DynamicValueAccessor;
        if (dynamicValueAccessor != null)
        {
          DateTimeOffset? nullable;
          this.referenceValue = ((IAccessorValue) dynamicValueAccessor).ReadValue(this.Symbol, ref nullable);
        }
      }
      return this.referenceValue;
    }

    private IAccessorDynamicValue? DynamicValueAccessor => this.ValueFactory is IAccessorValueFactory2 valueFactory ? valueFactory.ValueAccessor as IAccessorDynamicValue : (IAccessorDynamicValue) null;

    /// <summary>Reads the specified member element.</summary>
    /// <param name="memberInstance">The member instance.</param>
    /// <returns></returns>
    protected internal override object ReadMember(ISymbol memberInstance) => base.ReadMember(memberInstance);
  }
}
