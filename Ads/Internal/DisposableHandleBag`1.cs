// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.Internal.DisposableHandleBag`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using TwinCAT.Ads.SumCommand;


#nullable enable
namespace TwinCAT.Ads.Internal
{
  /// <summary>Class DisposableHandleBag.</summary>
  /// <typeparam name="TSource">The type of the t source.</typeparam>
  public abstract class DisposableHandleBag<TSource> : 
    IDisposableHandleBag<TSource>,
    IDisposableHandleBag,
    IDisposable
    where TSource : class
  {
    /// <summary>The connection</summary>
    protected IAdsConnection connection;
    /// <summary>The handle dictionary</summary>
    protected ISumHandleCollection? sumCommandHandles;
    /// <summary>Dictionary of successfully acquired handles</summary>
    protected IDictionary<TSource, uint>? sourceHandlesDict;
    /// <summary>List of the used symbols.</summary>
    protected IList<TSource> sourceObjects;
    /// <summary>Indicates, that the handles are invalidated.</summary>
    protected bool handlesInvalidated;
    /// <summary>
    /// Indicates, that the <see cref="T:TwinCAT.Ads.Internal.DisposableHandleBag`1" /> is initialized.
    /// </summary>
    protected bool handlesCreated;
    /// <summary>
    /// Indicates that this <see cref="T:TwinCAT.Ads.Internal.DisposableHandleBag" /> is disposed.
    /// </summary>
    protected bool isDisposed;
    /// <summary>Synchronization object</summary>
    protected object sync = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.Internal.DisposableHandleBag" /> class.
    /// </summary>
    /// <param name="connection">The client.</param>
    /// <param name="sourceObjects">The source objects.</param>
    /// <exception cref="T:System.ArgumentNullException">client</exception>
    /// <exception cref="T:System.ArgumentNullException">sourceObjects</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">sourceObjects</exception>
    protected DisposableHandleBag(IAdsConnection connection, IList<TSource> sourceObjects)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (sourceObjects == null)
        throw new ArgumentNullException(nameof (sourceObjects));
      if (sourceObjects.Count == 0)
        throw new ArgumentOutOfRangeException(nameof (sourceObjects));
      this.connection = connection;
      this.sourceObjects = sourceObjects;
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (disposing && !this.handlesInvalidated)
        this.ReleaseHandles();
      this.isDisposed = true;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    /// <summary>Closes this instance.</summary>
    public void Close() => this.Dispose(true);

    /// <summary>
    /// Creates the internal handles of the <see cref="T:TwinCAT.Ads.Internal.DisposableHandleBag`1"></see>.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public virtual int CreateHandles()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (DisposableHandleBag<TSource>));
      object sync = this.sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (this.handlesCreated)
          this.ReleaseHandles();
        this.sourceHandlesDict = (IDictionary<TSource, uint>) new Dictionary<TSource, uint>();
        this.sumCommandHandles = (ISumHandleCollection) new SumHandleList();
        this.handlesInvalidated = false;
        this.handlesCreated = true;
        return 0;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>
    /// Releases the handles of the <see cref="T:TwinCAT.Ads.Internal.DisposableHandleBag`1"></see>
    /// </summary>
    public virtual void ReleaseHandles()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (DisposableHandleBag<TSource>));
      object sync = this.sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        this.sourceHandlesDict = (IDictionary<TSource, uint>) null;
        this.sumCommandHandles = (ISumHandleCollection) null;
        this.handlesInvalidated = false;
        this.handlesCreated = false;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>Tries to get the specified handle</summary>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="handle">The handle.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="T:System.ObjectDisposedException">DisposableHandleBag</exception>
    public bool TryGetHandle(TSource instancePath, out uint handle)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (DisposableHandleBag<TSource>));
      if (this.sourceHandlesDict == null)
        throw new HandleBagNotInitializedException();
      object sync = this.sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        return this.sourceHandlesDict.TryGetValue(instancePath, out handle);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>
    /// Determines whether the handle was acquired by this <see cref="T:TwinCAT.Ads.Internal.DisposableHandleBag" />
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <returns><c>true</c> if [contains] [the specified handle]; otherwise, <c>false</c>.</returns>
    public bool Contains(uint handle)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (DisposableHandleBag<TSource>));
      if (this.sumCommandHandles == null)
        throw new HandleBagNotInitializedException();
      object sync = this.sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        return ((IEnumerable<uint>) this.sumCommandHandles.ValidHandles).Contains<uint>(handle);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>Gets the handle.</summary>
    /// <param name="instancePath">The instance path.</param>
    /// <returns>System.UInt32 or 0</returns>
    public uint GetHandle(TSource instancePath)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (DisposableHandleBag<TSource>));
      uint handle = 0;
      this.TryGetHandle(instancePath, out handle);
      return handle;
    }

    /// <summary>Tries to get the corresponding Source object</summary>
    /// <param name="handle">The handle.</param>
    /// <param name="sourceObject">The source object.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetSourceObject(uint handle, [NotNullWhen(true)] out TSource? sourceObject)
    {
      if (handle == 0U)
        throw new ArgumentOutOfRangeException(nameof (handle));
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (DisposableHandleBag<TSource>));
      if (this.sourceHandlesDict == null)
        throw new HandleBagNotInitializedException();
      object sync = this.sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        foreach (KeyValuePair<TSource, uint> keyValuePair in (IEnumerable<KeyValuePair<TSource, uint>>) this.sourceHandlesDict)
        {
          if ((int) keyValuePair.Value == (int) handle)
          {
            sourceObject = keyValuePair.Key;
            return true;
          }
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
      sourceObject = default (TSource);
      return false;
    }

    /// <summary>Gets the source object.</summary>
    /// <param name="handle">The handle.</param>
    /// <returns>TSource.</returns>
    public TSource GetSourceObject(uint handle)
    {
      TSource sourceObject = default (TSource);
      if (!this.TryGetSourceObject(handle, out sourceObject))
        throw new ArgumentException("Handle not found!", nameof (handle));
      return sourceObject;
    }
  }
}
