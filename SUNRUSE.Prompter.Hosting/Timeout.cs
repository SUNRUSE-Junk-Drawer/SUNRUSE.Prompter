﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SUNRUSE.Prompter.Hosting
{
    /// <summary><see cref="IDisposable.Dispose"/>s of an <see cref="IDisposable"/> after a timeout, with the option to <see cref="Reset"/> that timeout or <see cref="IDisposable.Dispose"/> immediately.</summary>
    public sealed class Timeout : IDisposable
    {
        /// <summary>The <see cref="TimeSpan"/> after which to <see cref="IDisposable.Dispose"/> of <see cref="ToDispose"/> if not <see cref="Reset"/>.</summary>
        public readonly TimeSpan Duration;

        /// <summary>The <see cref="IDisposable"/> to <see cref="IDisposable.Dispose"/> of after the set <see cref="Duration"/> if not <see cref="Reset"/>.</summary>
        public readonly IDisposable ToDispose;

        private bool Disposed;
        private Task CurrentTask;
        private CancellationTokenSource CurrentCancellationTokenSource;
        private object Synchronization = new object();

        /// <inheritdoc />
        public Timeout(TimeSpan duration, IDisposable toDispose)
        {
            Duration = duration;
            ToDispose = toDispose;
            Reset();
        }

        /// <summary>Delays the <see cref="IDisposable.Dispose"/> of <see cref="ToDispose"/> by the set <see cref="Duration"/>, as though reset.</summary>
        public void Reset()
        {
            if (Disposed) return;
            lock (Synchronization)
            {
                if (Disposed) return;

                if (CurrentTask != null)
                {
                    CurrentCancellationTokenSource.Cancel();
                    CurrentCancellationTokenSource.Dispose();
                    CurrentCancellationTokenSource = null;
                }

                CurrentCancellationTokenSource = new CancellationTokenSource();
                CurrentTask = Task.Delay(Duration, CurrentCancellationTokenSource.Token);
                CurrentTask.ContinueWith(Continuation, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }

        private void Continuation(Task forTask)
        {
            if (Disposed) return;
            lock (Synchronization)
            {
                if (CurrentTask != forTask) return;
                PerformDispose();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Disposed) return;
            lock (Synchronization) PerformDispose();
        }

        private void PerformDispose()
        {
            if (Disposed) return;
            Disposed = true;
            if (CurrentCancellationTokenSource != null)
            {
                CurrentCancellationTokenSource.Cancel();
                CurrentCancellationTokenSource.Dispose();
                CurrentCancellationTokenSource = null;
            }
            CurrentTask = null;
            ToDispose.Dispose();
        }
    }
}
