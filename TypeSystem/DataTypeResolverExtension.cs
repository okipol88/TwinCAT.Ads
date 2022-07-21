// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.DataTypeResolverExtension
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem
{
  internal static class DataTypeResolverExtension
  {
    public static IDataType Resolve(this IDataType dataType)
    {
      IDataType idataType = dataType;
      if (dataType is IResolvableType iresolvableType)
        idataType = iresolvableType.ResolveType((DataTypeResolveStrategy) 1);
      return idataType;
    }

    public static Type? GetManagedType(this IDataType dataType)
    {
      Type managed = (Type) null;
      dataType.TryGetManagedType(out managed);
      return managed;
    }

    public static bool TryGetManagedType(this IDataType dataType, [NotNullWhen(true)] out Type? managed)
    {
      if (dataType == null)
        throw new ArgumentNullException(nameof (dataType));
      dataType.Resolve();
      return PrimitiveTypeMarshaler.TryGetManagedType(dataType, out managed);
    }
  }
}
