// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.SymbolValueEncodingExtension
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;


#nullable enable
namespace TwinCAT.TypeSystem
{
  internal static class SymbolValueEncodingExtension
  {
    public static bool HasEncoding(this IAttributedInstance instance) => ((IInstance) instance).DataType != null && ((IInstance) instance).DataType.Category == 10;

    /// <summary>Tries to get the Encoding of the symbol instance.</summary>
    /// <param name="instance">The instance.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns><c>true</c> if encoding found on the symbol instance, <c>false</c> otherwise.</returns>
    public static bool TryGetValueEncoding(this IAttributedInstance instance, [NotNullWhen(true)] out Encoding? encoding)
    {
      encoding = (Encoding) null;
      if (!EncodingAttributeConverter.TryGetEncoding((IEnumerable<ITypeAttribute>) instance.Attributes, out encoding) && ((IInstance) instance).DataType != null && ((IInstance) instance).DataType.Resolve() is IStringType istringType)
        encoding = istringType.Encoding;
      return encoding != null;
    }

    /// <summary>Gets the encoding.</summary>
    /// <param name="instance">The instance.</param>
    /// <returns>System.Text.Encoding or NULL if nothing specified.</returns>
    public static Encoding? GetValueEncoding(this IAttributedInstance instance)
    {
      Encoding encoding = (Encoding) null;
      instance.TryGetValueEncoding(out encoding);
      return encoding;
    }
  }
}
