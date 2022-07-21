// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.ListExtensions
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class ListExtensions.</summary>
  /// <exclude />
  public static class ListExtensions
  {
    /// <summary>Splits the List in chunks.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="theList">The list.</param>
    /// <param name="chunkSize">Size of the chunk.</param>
    /// <returns>List&lt;List&lt;T&gt;&gt;.</returns>
    public static List<List<T>> Chunk<T>(this List<T> theList, int chunkSize)
    {
      if (theList == null)
        throw new ArgumentNullException(nameof (theList));
      if (!theList.Any<T>())
        return new List<List<T>>();
      List<List<T>> objListList = new List<List<T>>();
      List<T> objList = new List<T>();
      objListList.Add(objList);
      int num = 0;
      foreach (T the in theList)
      {
        if (num >= chunkSize)
        {
          num = 0;
          objList = new List<T>();
          objListList.Add(objList);
        }
        ++num;
        objList.Add(the);
      }
      return objListList;
    }
  }
}
