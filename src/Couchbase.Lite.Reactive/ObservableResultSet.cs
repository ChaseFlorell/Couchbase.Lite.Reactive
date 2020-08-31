using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Couchbase.Lite.Query;

namespace Couchbase.Lite.Reactive
{
    public static class ObservableResultSet
    {
        private static readonly ConcurrentDictionary<Guid, ObservableTracker> _observers = new ConcurrentDictionary<Guid, ObservableTracker>();
        private static readonly BehaviorSubject<int> _connectionCount = new BehaviorSubject<int>(0);

        public static IObservable<int> ConnectionCount => _connectionCount.AsObservable();

        public static void CloseActiveObservables()
        {
            Parallel.ForEach(_observers.Values, x =>
            {
                x.Observer.OnCompleted();
                x.Dispose();
            });

            _connectionCount.OnCompleted();
        }

        public static IObservable<IResultSet> Create(IQuery query) =>
            Observable.Create<IResultSet>(observer =>
            {
                var token = query.AddChangeListener((s, e) => OnLiveQueryChanged(e, observer));

                query.Execute();

                var key = Guid.NewGuid();
                var tracker = new ObservableTracker(key, observer);

                var disposable = Disposable.Create(query, q =>
                {
                    q.RemoveChangeListener(token);
                    q.Dispose();
                    _observers.TryRemove(key, out _);
                    _connectionCount.OnNext(_observers.Count);
                });

                tracker.AddDisposable(disposable);
                _observers.TryAdd(key, tracker);
                _connectionCount.OnNext(_observers.Count);

                return disposable;
            });

        private static void OnLiveQueryChanged(QueryChangedEventArgs queryChangedEventArgs, IObserver<IResultSet> observer)
        {
            if (queryChangedEventArgs.Error != null)
            {
                observer.OnError(queryChangedEventArgs.Error);

                return;
            }

            observer.OnNext(queryChangedEventArgs.Results);
        }
    }
}