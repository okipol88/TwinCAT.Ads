// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.AnyArrayConverter
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem
{
  /// <summary>Class AnyArrayConverter.</summary>
  /// <exclude />
  internal static class AnyArrayConverter
  {
    /// <summary>
    /// Determines whether the specified array type is jagged.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>true</c> if the specified type is jagged; otherwise, <c>false</c>.</returns>
    public static bool IsJagged(IArrayType type)
    {
      bool flag = false;
      if (((IDataType) type).Category == 4)
      {
        IArrayType iarrayType = type;
        if (iarrayType.ElementType != null && iarrayType.ElementType.Category == 4)
          flag = true;
      }
      return flag;
    }

    /// <summary>
    /// Tries to get the (jagged) dimensions from the array type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="dimLengths">The dim lengths.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool TryGetJaggedDimensions(
      IArrayType type,
      [NotNullWhen(true)] out IList<IDimensionCollection>? dimLengths)
    {
      List<IDimensionCollection> idimensionCollectionList = new List<IDimensionCollection>();
      IArrayType iarrayType;
      for (IDataType idataType = (IDataType) type; idataType != null && idataType.Category == 4; idataType = iarrayType.ElementType)
      {
        iarrayType = (IArrayType) idataType;
        idimensionCollectionList.Add(iarrayType.Dimensions);
      }
      if (idimensionCollectionList.Count > 0)
      {
        dimLengths = (IList<IDimensionCollection>) idimensionCollectionList;
        return true;
      }
      dimLengths = (IList<IDimensionCollection>) null;
      return false;
    }
  }
}
