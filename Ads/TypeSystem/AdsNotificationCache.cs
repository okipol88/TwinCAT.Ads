// Decompiled with JetBrains decompiler
// Type: TwinCAT.Ads.TypeSystem.AdsNotificationCache
// Assembly: TwinCAT.Ads, Version=6.0.164.0, Culture=neutral, PublicKeyToken=180016cd49e5e8c3
// MVID: E66A887E-650C-48BD-ACFF-80F7B9224E2B
// Assembly location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.dll
// XML documentation location: C:\projects\trimet_l1\contrib\beckhoff.twincat.ads\6.0.164\lib\net6.0\TwinCAT.Ads.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using TwinCAT.TypeSystem;
using TwinCAT.ValueAccess;


#nullable enable
namespace TwinCAT.Ads.TypeSystem
{
  /// <summary>
  /// Class caches the currently registered Notification handlers.
  /// </summary>
  internal class AdsNotificationCache
  {
    /// <summary>Sync object</summary>
    private object _sync = new object();
    /// <summary>Notification Handle --&gt; Symbol</summary>
    private Dictionary<uint, AdsNotificationCache.NotificationSymbolInfo> _notificationHandleDict = new Dictionary<uint, AdsNotificationCache.NotificationSymbolInfo>();
    /// <summary>Symbol --&gt; Notification Handle</summary>
    private Dictionary<ISymbol, AdsNotificationCache.NotificationSymbolInfo> _notificationSymbolDict = new Dictionary<ISymbol, AdsNotificationCache.NotificationSymbolInfo>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsNotificationCache" /> class.
    /// </summary>
    internal AdsNotificationCache()
    {
    }

    /// <summary>
    /// Gets the largest symbol size in bytes that is inside this <see cref="T:TwinCAT.Ads.TypeSystem.AdsNotificationCache" />
    /// </summary>
    /// <returns>System.Int32.</returns>
    internal int GetLargestSymbolSize()
    {
      int largestSymbolSize = 0;
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (this._notificationSymbolDict.Count > 0)
          largestSymbolSize = ((IEnumerable<ISymbol>) this._notificationSymbolDict.Keys).Max<ISymbol>((Func<ISymbol, int>) (symbol => ((IBitSize) symbol).ByteSize));
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
      return largestSymbolSize;
    }

    /// <summary>Gets the type of the notification.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>SymbolNotificationType.</returns>
    internal SymbolNotificationTypes GetNotificationType(ISymbol symbol)
    {
      SymbolNotificationTypes notificationType = (SymbolNotificationTypes) 0;
      AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo = (AdsNotificationCache.NotificationSymbolInfo) null;
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (this._notificationSymbolDict.TryGetValue(symbol, out notificationSymbolInfo))
          notificationType = notificationSymbolInfo.NotificationType;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
      return notificationType;
    }

    /// <summary>Updates the specified symbol notification.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="type">The type.</param>
    /// <param name="settings">The settings.</param>
    /// <exception cref="T:System.ArgumentException">Symbol is not registered for Notifications!</exception>
    internal void Update(
      ISymbol symbol,
      SymbolNotificationTypes type,
      NotificationSettings settings)
    {
      AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo1 = (AdsNotificationCache.NotificationSymbolInfo) null;
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (!this._notificationSymbolDict.TryGetValue(symbol, out notificationSymbolInfo1))
          throw new ArgumentException("Symbol is not registered for Notifications!");
        notificationSymbolInfo1.Settings = settings;
        AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo2 = notificationSymbolInfo1;
        notificationSymbolInfo2.NotificationType = notificationSymbolInfo2.NotificationType | type;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>Adds the specified symbol notification</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="handle">The handle.</param>
    /// <param name="notificationType">Type of the notification.</param>
    /// <param name="settings">The settings.</param>
    /// <exception cref="T:System.ArgumentException">Symbol already registered!</exception>
    internal void Add(
      ISymbol symbol,
      uint handle,
      SymbolNotificationTypes notificationType,
      NotificationSettings settings)
    {
      AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo1 = (AdsNotificationCache.NotificationSymbolInfo) null;
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (this._notificationHandleDict.TryGetValue(handle, out notificationSymbolInfo1))
          throw new ArgumentException("Symbol already registered!");
        AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo2 = new AdsNotificationCache.NotificationSymbolInfo(symbol, handle, notificationType, settings);
        this._notificationHandleDict.Add(handle, notificationSymbolInfo2);
        this._notificationSymbolDict.Add(symbol, notificationSymbolInfo2);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>Removes the specified symbol notification.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="notificationType">Type of the notification.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal bool Remove(ISymbol symbol, SymbolNotificationTypes notificationType)
    {
      bool flag = false;
      AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo1 = (AdsNotificationCache.NotificationSymbolInfo) null;
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (this._notificationSymbolDict.TryGetValue(symbol, out notificationSymbolInfo1))
        {
          SymbolNotificationTypes notificationTypes = (SymbolNotificationTypes) (3 & ~notificationType);
          AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo2 = notificationSymbolInfo1;
          notificationSymbolInfo2.NotificationType = notificationSymbolInfo2.NotificationType & notificationTypes;
          if (notificationSymbolInfo1.NotificationType == null)
          {
            this._notificationHandleDict.Remove(notificationSymbolInfo1.Handle);
            flag = this._notificationSymbolDict.Remove(symbol);
          }
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
      return flag;
    }

    /// <summary>
    /// Removes all Symbol notifications for the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal bool Remove(ISymbol symbol)
    {
      bool flag = false;
      AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo = (AdsNotificationCache.NotificationSymbolInfo) null;
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (this._notificationSymbolDict.TryGetValue(symbol, out notificationSymbolInfo))
        {
          this._notificationHandleDict.Remove(notificationSymbolInfo.Handle);
          flag = this._notificationSymbolDict.Remove(notificationSymbolInfo.Symbol);
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
      return flag;
    }

    /// <summary>Tries to get the notification handle.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="handle">The handle.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    internal bool TryGetNotificationHandle(ISymbol symbol, out uint handle)
    {
      bool notificationHandle = false;
      AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo = (AdsNotificationCache.NotificationSymbolInfo) null;
      handle = 0U;
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        if (this._notificationSymbolDict.TryGetValue(symbol, out notificationSymbolInfo))
        {
          handle = notificationSymbolInfo.Handle;
          notificationHandle = true;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
      return notificationHandle;
    }

    internal bool TryGetRegisteredNotificationSettings(
      ISymbol symbol,
      [NotNullWhen(true)] out NotificationSettings? settings)
    {
      settings = (NotificationSettings) null;
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        AdsNotificationCache.NotificationSymbolInfo notificationSymbolInfo = (AdsNotificationCache.NotificationSymbolInfo) null;
        if (this._notificationSymbolDict.TryGetValue(symbol, out notificationSymbolInfo))
        {
          settings = notificationSymbolInfo.Settings;
          return true;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
      return false;
    }

    /// <summary>
    /// Determines whether this <see cref="T:TwinCAT.Ads.TypeSystem.AdsNotificationCache" /> has a registered notification for the symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns><c>true</c> if [contains] [the specified symbol]; otherwise, <c>false</c>.</returns>
    internal bool Contains(ISymbol symbol)
    {
      object sync = this._sync;
      bool lockTaken = false;
      try
      {
        Monitor.Enter(sync, ref lockTaken);
        return this._notificationSymbolDict.ContainsKey(symbol);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(sync);
      }
    }

    /// <summary>Notification info object</summary>
    private class NotificationSymbolInfo
    {
      private ISymbol _symbol;
      private SymbolNotificationTypes _notificationType;
      private uint _handle;
      private NotificationSettings _settings;

      /// <summary>Symbol</summary>
      internal ISymbol Symbol => this._symbol;

      /// <summary>Notification type</summary>
      internal SymbolNotificationTypes NotificationType
      {
        get => this._notificationType;
        set => this._notificationType = value;
      }

      /// <summary>Notification handle</summary>
      internal uint Handle => this._handle;

      /// <summary>Notification settings</summary>
      internal NotificationSettings Settings
      {
        get => this._settings;
        set => this._settings = value;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="T:TwinCAT.Ads.TypeSystem.AdsNotificationCache.NotificationSymbolInfo" /> class.
      /// </summary>
      /// <param name="symbol">The symbol.</param>
      /// <param name="handle">The handle.</param>
      /// <param name="notificationType">Type of the notification.</param>
      /// <param name="settings">The settings.</param>
      internal NotificationSymbolInfo(
        ISymbol symbol,
        uint handle,
        SymbolNotificationTypes notificationType,
        NotificationSettings settings)
      {
        this._symbol = symbol;
        this._notificationType = notificationType;
        this._handle = handle;
        this._settings = settings;
      }
    }
  }
}
