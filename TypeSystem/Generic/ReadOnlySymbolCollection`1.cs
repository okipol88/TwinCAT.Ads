// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.ReadOnlySymbolCollection`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml


#nullable enable
namespace TwinCAT.TypeSystem.Generic
{
  /// <summary>Read only symbol collection.</summary>
  /// <typeparam name="T"></typeparam>
  public class ReadOnlySymbolCollection<T> : ReadOnlyInstanceCollection<T> where T : class, ISymbol
  {
    public ReadOnlySymbolCollection(IInstanceCollection<T> coll)
      : base(coll)
    {
    }

    /// <summary>Returns an empty collection.</summary>
    /// <returns>ReadOnlySymbolCollection&lt;T&gt;.</returns>
    public static ReadOnlySymbolCollection<T> Empty => SymbolCollection<T>.Empty.AsReadOnly();
  }
}
