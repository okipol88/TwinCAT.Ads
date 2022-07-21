// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DynamicPointerInstance
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Dynamic Pointer Instance</summary>
  public sealed class DynamicPointerInstance : 
    DynamicSymbol,
    IPointerInstance,
    ISymbol,
    IAttributedInstance,
    IInstance,
    IBitSize
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.TypeSystem.DynamicPointerInstance" /> class.
    /// </summary>
    /// <param name="pointerInstance">The pointer instance.</param>
    internal DynamicPointerInstance(IPointerInstance pointerInstance)
      : base((IValueSymbol) pointerInstance)
    {
    }

    /// <summary>Returns the enumeration of all dynamic member names.</summary>
    /// <returns>A sequence that contains dynamic member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames() => (IEnumerable<string>) new List<string>(base.GetDynamicMemberNames())
    {
      "__deref"
    };

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
    /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
    public override bool TryGetMember(GetMemberBinder binder, [NotNullWhen(true)] out object? result)
    {
      if (binder == null)
        throw new ArgumentNullException(nameof (binder));
      if (StringComparer.OrdinalIgnoreCase.Compare(binder.Name, "__deref") != 0)
        return base.TryGetMember(binder, out result);
      result = (object) this.Reference;
      return result != null;
    }

    /// <summary>
    /// Gets the resolved reference of Pointer / Reference (or NULL if PVOID)
    /// </summary>
    /// <value>The reference symbol or NULL if PVOID Pointer.</value>
    public ISymbol? Reference => ((ICollection<ISymbol>) this.SubSymbols).Count > 0 ? ((IList<ISymbol>) this.SubSymbols)[0] : (ISymbol) null;
  }
}
