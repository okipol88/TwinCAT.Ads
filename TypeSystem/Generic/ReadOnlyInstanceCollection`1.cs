// Decompiled with JetBrains decompiler
// Type: TwinCAT.TypeSystem.Generic.ReadOnlyInstanceCollection`1
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace TwinCAT.TypeSystem.Generic
{
  /// <summary>ReadOnly Instance collection</summary>
  /// <typeparam name="T"></typeparam>
  public class ReadOnlyInstanceCollection<T> : 
    ReadOnlyCollection<T>,
    IInstanceCollection<T>,
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
    where T : class, IInstance
  {
    /// <summary>
    /// Mode of the <see cref="T:TwinCAT.TypeSystem.IInstanceCollection`1" />
    /// </summary>
    private InstanceCollectionMode mode;

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.InstanceCollectionMode" />.
    /// </summary>
    /// <value>The mode.</value>
    public InstanceCollectionMode Mode => this.mode;

    public ReadOnlyInstanceCollection(IInstanceCollection<T> coll)
      : base((IList<T>) coll)
    {
      this.mode = coll != null ? coll.Mode : throw new ArgumentNullException(nameof (coll));
    }

    /// <summary>
    /// Determines whether the <see cref="T:TwinCAT.TypeSystem.Generic.ReadOnlyInstanceCollection`1" /> contains an instance with the specified instance path.
    /// </summary>
    /// <param name="instancePath">The instance path.</param>
    /// <returns>
    ///   <c>true</c> if contains the specified instance path; otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(string instancePath) => ((IInstanceCollection<T>) this.Items).Contains(instancePath);

    /// <summary>Gets the element with the specified instance path.</summary>
    /// <param name="instancePath">The instance path.</param>
    /// <returns>The instance if contained.</returns>
    public T this[string instancePath] => ((IInstanceCollection<T>) this.Items)[instancePath];

    /// <summary>
    /// Tries to get the instance with the specified instance path.
    /// </summary>
    /// <param name="instancePath">The instance path.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>true, if found, false if not contained.</returns>
    public bool TryGetInstance(string instancePath, [NotNullWhen(true)] out T? instance) => ((IInstanceCollection<T>) this.Items).TryGetInstance(instancePath, ref instance);

    /// <summary>Tries to get the instance by name.</summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <param name="symbols">The found symbols (out-parameter)</param>
    /// <returns>true, if found; false if not contained.</returns>
    public bool TryGetInstanceByName(string instanceName, [NotNullWhen(true)] out IList<T>? symbols) => ((IInstanceCollection<T>) this.Items).TryGetInstanceByName(instanceName, ref symbols);

    /// <summary>
    /// Determines whether the specified instance is contained.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <returns>true, if instance name is found.</returns>
    public bool ContainsName(string instanceName) => ((IInstanceCollection<T>) this.Items).ContainsName(instanceName);

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IInstance" />by instance path.
    /// </summary>
    /// <param name="instancePath">The instance path.</param>
    /// <returns>T.</returns>
    public T GetInstance(string instancePath) => ((IInstanceCollection<T>) this.Items).GetInstance(instancePath);

    /// <summary>
    /// Gets the <see cref="T:TwinCAT.TypeSystem.IInstance" /> by instance name.
    /// </summary>
    /// <param name="instanceName">Name of the instance.</param>
    /// <returns>IList&lt;T&gt;.</returns>
    public IList<T> GetInstanceByName(string instanceName) => ((IInstanceCollection<T>) this.Items).GetInstanceByName(instanceName);
  }
}
