﻿// @출처: https://github.com/CodingOctocat/WpfObservableRangeCollection/blob/master/WpfObservableRangeCollection/WpfObservableRangeCollection.cs#L13C1-L128C2
using P4VHelper.Base.Collection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace P4VHelper
{

    /// <summary>
    /// WPF version of <see cref="ObservableRangeCollection{T}"/> with <see cref="CollectionView"/> support.
    /// </summary>
    /// <remarks>If the <see cref="NotSupportedException"/> still occurred, try using <see cref="BindingOperations.EnableCollectionSynchronization(IEnumerable, Object)"/>.</remarks>
    /// <typeparam name="T"></typeparam>
    public class WpfObservableRangeCollection<T> : ObservableRangeCollection<T>
    {
        private class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
        {
            private readonly WpfObservableRangeCollection<T> _collection;

            public DeferredEventsCollection(WpfObservableRangeCollection<T> collection)
            {
                Debug.Assert(collection is not null);
                Debug.Assert(collection._deferredEvents is null);

                _collection = collection;
                _collection._deferredEvents = this;
            }

            public void Dispose()
            {
                _collection._deferredEvents = null;

                var handlers = _collection
                  .GetHandlers()
                  .ToLookup(h => h.Target is CollectionView);

                foreach (var handler in handlers[false])
                {
                    foreach (var e in this)
                    {
                        handler(_collection, e);
                    }
                }

                foreach (var cv in handlers[true]
                  .Select(h => h.Target)
                  .Cast<CollectionView>()
                  .Distinct())
                {
                    cv.Refresh();
                }
            }
        }

        private DeferredEventsCollection? _deferredEvents;

        /// <inheritdoc/>
        public WpfObservableRangeCollection(bool allowDuplicates = true, EqualityComparer<T>? comparer = null)
            : base(allowDuplicates, comparer)
        { }

        /// <inheritdoc/>
        public WpfObservableRangeCollection(IEnumerable<T> collection, bool allowDuplicates = true, EqualityComparer<T>? comparer = null)
            : base(collection, allowDuplicates, comparer)
        { }

        /// <inheritdoc/>
        public WpfObservableRangeCollection(List<T> list, bool allowDuplicates = true, EqualityComparer<T>? comparer = null)
            : base(list, allowDuplicates, comparer)
        { }

        /// <inheritdoc/>
        protected override IDisposable DeferEvents()
        {
            return new DeferredEventsCollection(this);
        }

        /// <summary>
        /// Raise <see cref="ObservableCollection{T}.CollectionChanged"/> event to any listeners.
        /// Properties/methods modifying this <see cref="ObservableCollection{T}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <remarks>
        /// When overriding this method, either call its base implementation
        /// or call <see cref="ObservableCollection{T}.BlockReentrancy"/> to guard against reentrant collection changes.
        /// </remarks>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (typeof(ObservableRangeCollection<T>).GetField(nameof(_deferredEvents), BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this) is ICollection<NotifyCollectionChangedEventArgs> deferredEvents)
            {
                deferredEvents.Add(e);

                return;
            }

            foreach (var handler in GetHandlers())
            {
                if (WpfObservableRangeCollection<T>.IsRange(e) && handler.Target is CollectionView cv)
                {
                    cv.Refresh();
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        private static bool IsRange(NotifyCollectionChangedEventArgs e)
        {
            return e.NewItems?.Count > 1 || e.OldItems?.Count > 1;
        }

        private IEnumerable<NotifyCollectionChangedEventHandler> GetHandlers()
        {
            var info = typeof(ObservableCollection<T>).GetField(nameof(CollectionChanged), BindingFlags.Instance | BindingFlags.NonPublic);
            var @event = info?.GetValue(this) as MulticastDelegate;

            return @event?.GetInvocationList()
              .Cast<NotifyCollectionChangedEventHandler>()
              .Distinct()
              ?? Enumerable.Empty<NotifyCollectionChangedEventHandler>();
        }
    }
}
