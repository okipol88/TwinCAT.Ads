// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.EncodingAttributeConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


#nullable enable
namespace TwinCAT.TypeSystem
{
  internal static class EncodingAttributeConverter
  {
    internal static bool TryGetEncoding(ITypeAttribute att, [NotNullWhen(true)] out Encoding? encoding)
    {
      encoding = (Encoding) null;
      return att.Name == "TcEncoding" && EncodingAttributeConverter.TryGetEncoding(att.Value, out encoding);
    }

    internal static Encoding GetEncoding(TypeAttribute att)
    {
      Encoding encoding = (Encoding) null;
      if (!EncodingAttributeConverter.TryGetEncoding((ITypeAttribute) att, out encoding))
        throw new ArgumentException("Attributte '{0}' is not the 'TcEncoding' attribute!", att.Name);
      return encoding;
    }

    internal static bool TryGetEncoding(string encodingName, [NotNullWhen(true)] out Encoding? encoding)
    {
      encoding = (Encoding) null;
      try
      {
        encoding = Encoding.GetEncoding(encodingName);
      }
      catch (ArgumentException ex)
      {
        int result = 0;
        if (int.TryParse(encodingName, out result))
          encoding = Encoding.GetEncoding(result);
      }
      return encoding != null;
    }

    internal static bool TryGetEncoding(
      IEnumerable<ITypeAttribute> attributes,
      [NotNullWhen(true)] out Encoding? encoding)
    {
      if (attributes == null)
        throw new ArgumentNullException(nameof (attributes));
      ITypeAttribute att1 = attributes.FirstOrDefault<ITypeAttribute>((Func<ITypeAttribute, bool>) (att => string.CompareOrdinal(att.Name, "TcEncoding") == 0));
      if (att1 != null)
        return EncodingAttributeConverter.TryGetEncoding(att1, out encoding);
      encoding = (Encoding) null;
      return false;
    }
  }
}
