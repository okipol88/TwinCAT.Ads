// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.PlatformBoundType
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.ComponentModel;
using TwinCAT.Ads.Internal;
using TwinCAT.TypeSystem;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>Platform bound types.</summary>
  /// <remarks>These are <see cref="T:TwinCAT.Ads.TypeSystem.PrimitiveType">Primitive types</see> like (UXINT, XINT, XWORD, PWORD) whose size is dependant of the target platform (4 or 8 bytes).
  /// </remarks>
  /// <exclude />
  public sealed class PlatformBoundType : PrimitiveType
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PrimitiveType" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="byteSize">Size of the byte.</param>
    /// <param name="dotnetType">Type of the dotnet.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    private PlatformBoundType(string name, int byteSize, Type dotnetType)
      : base(name, (AdsDataTypeId) 65, byteSize, (PrimitiveTypeFlags) 67, dotnetType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.PlatformBoundType" /> class.
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">entry</exception>
    internal PlatformBoundType(AdsDataTypeEntry entry)
      : base(entry, (PrimitiveTypeFlags) 67, entry.Size == 8 ? typeof (ulong) : typeof (uint))
    {
      if (entry.Size != 4 && entry.Size != 8)
        throw new ArgumentOutOfRangeException(nameof (entry));
    }

    /// <summary>
    /// Called when this <see cref="T:TwinCAT.Ads.TypeSystem.DataType" /> is bound via the type binder.
    /// </summary>
    /// <param name="binder">The binder.</param>
    /// <exclude />
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void OnBound(IBinder binder)
    {
      if (this.Size == 4 || this.Size == 8)
      {
        if (((IDataTypeResolver) binder).PlatformPointerSize > 0)
          return;
        ((Binder) binder).SetPlatformPointerSize(this.Size);
      }
      else
      {
        this.Size = ((IDataTypeResolver) binder).PlatformPointerSize;
        if (this.Size == 4)
          this.ManagedType = typeof (uint);
        if (this.Size != 8)
          return;
        this.ManagedType = typeof (ulong);
      }
    }
  }
}
