using System;
using Couchbase.Lite.Query;

namespace Couchbase.Lite.Reactive
{
    internal sealed class ObservableTracker
    {
        public ObservableTracker(Guid key, IObserver<IResultSet> observer)
        {
            Observer = observer;
            Key = key;
        }

        public void AddDisposable(IDisposable disposable)
        {
            Disposable = disposable;
        }

        public IObserver<IResultSet> Observer { get; }
        public Guid Key { get; }
        public IDisposable Disposable { get; private set; }
    }
}