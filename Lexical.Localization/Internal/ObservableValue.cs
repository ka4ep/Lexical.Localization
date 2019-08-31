// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           30.8.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Observable value
    /// </summary>
    public class ObservableValue<T> : IObservable<T>
    {
        /// <summary>
        /// Is value in readonly state.
        /// </summary>
        protected bool isReadonly;

        /// <summary>
        /// Value.
        /// </summary>
        protected bool value;

        /// <summary>
        /// Subscriptions to modifications to the value.
        /// </summary>
        protected Subscription[] subscriptions;

        /// <summary>
        /// Value.
        /// </summary>
        public bool Value
        {
            get => value;
            set
            {
                Subscription[] _subscriptions;
                lock (this)
                {
                    if (value == this.value) return;
                    if (isReadonly) throw new InvalidOperationException("Cannot change in read-only state");
                    this.value = value;
                    _subscriptions = subscriptions;
                }

                // Notify subcribees                
                if (_subscriptions != null)
                {
                    StructList2<Exception> errors = new StructList2<Exception>();
                    foreach (IObserver<bool> o in _subscriptions)
                    {
                        try
                        {
                            o.OnNext(value);
                        }
                        catch (Exception e)
                        {
                            errors.Add(e);
                        }
                    }
                    if (errors.Count > 0) throw new AggregateException(errors);
                }
            }
        }

        /// <summary>
        /// Is the observable value in readonly state.
        /// </summary>
        public bool Readonly
        {
            get => isReadonly;
            set
            {
                if (value == isReadonly) return;
                if (isReadonly) throw new InvalidOperationException("Cannot change read-only state");
                isReadonly = value;
            }
        }

        /// <summary>
        /// Set into read-only state.
        /// </summary>
        /// <returns></returns>
        public ObservableValue<T> SetReadonly()
        {
            isReadonly = true;
            return this;
        }

        /// <summary>
        /// Subscribe for changes.
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            // Return dummy disposable
            if (isReadonly) return DummyDisposable.Instance;

            // Create Suscription handle
            Subscription s = new Subscription(this, observer);

            // Add handle to array
            lock (this)
            {
                if (subscriptions == null)
                {
                    subscriptions = new Subscription[] { s };
                }
                else
                {
                    int l = subscriptions.Length;
                    Subscription[] newArray = new Subscription[l + 1];
                    Array.Copy(subscriptions, 0, newArray, 0, l);
                    newArray[l] = s;
                    subscriptions = newArray;
                }
            }

            // Return handle
            return s;
        }

        /// <summary>
        /// Create observe policy with false default value.
        /// </summary>
        public ObservableValue()
        {
        }

        /// <summary>
        /// Create observe policy with <paramref name="initialValue"/> initial value.
        /// </summary>
        /// <param name="initialValue"></param>
        /// <param name="isReadonly"></param>
        public ObservableValue(bool initialValue, bool isReadonly)
        {
            this.Value = initialValue;
            this.isReadonly = isReadonly;
        }

        /// <summary>
        /// Dummy disposable that does nothing.
        /// </summary>
        protected class DummyDisposable : IDisposable
        {
            static IDisposable instance = new DummyDisposable();

            /// <summary>
            /// Dummy disposable singleton.
            /// </summary>
            public static IDisposable Instance => instance;

            /// <summary>
            /// Dispose (does nothing)
            /// </summary>
            public void Dispose()
            {
            }
        }

        /// <summary>
        /// Subscription handle
        /// </summary>
        protected class Subscription : IDisposable
        {
            ObservableValue<T> parent;
            IObserver<T> observer;

            /// <summary>
            /// Create subscription handle
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="observer"></param>
            public Subscription(ObservableValue<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
            }

            /// <summary>
            /// Dispose subscription handle.
            /// </summary>
            public void Dispose()
            {
                // Get parent reference
                var _parent = parent;
                var _observer = observer;

                // No parent reference, has already been disposed
                if (_parent == null) return;

                // Remove from parent's subscriptions array.
                lock (_parent)
                {
                    if (parent.subscriptions != null)
                    {
                        int ix = Array.IndexOf(parent.subscriptions, this);
                        if (ix >= 0)
                        {
                            int newLen = parent.subscriptions.Length - 1;
                            if (newLen == 0) parent.subscriptions = null;
                            {
                                Subscription[] newArray = new Subscription[newLen];
                                Array.Copy(parent.subscriptions, 0, newArray, 0, ix);
                                Array.Copy(parent.subscriptions, ix + 1, newArray, ix, newLen - ix);
                                parent.subscriptions = newArray;
                            }
                        }
                    }
                }

                // Remove references
                this.parent = null;
                this.observer = null;

                // Notify completed
                if (_observer != null) _observer.OnCompleted();
            }
        }
    }
}
