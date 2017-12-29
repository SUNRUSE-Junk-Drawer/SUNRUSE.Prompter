// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using Xunit;

namespace SUNRUSE.Prompter.Hosting.Tests
{
    public sealed class TimeoutFactoryTests
    {
        public sealed class DisposableMock : IDisposable
        {
            public int DisposedTimes = 0;

            public void Dispose()
            {
                DisposedTimes++;
            }
        }

        [Fact, Trait("Type", "Unit")]
        public void CreatesATimeout()
        {
            var disposable = new DisposableMock();
            var timeoutFactory = new TimeoutFactory();

            using (var timeout = timeoutFactory.Create(TimeSpan.FromMilliseconds(1500), disposable))
            {

                Assert.IsType<Timeout>(timeout);
            }
        }

        [Fact, Trait("Type", "Unit")]
        public void UsesTheGivenDuration()
        {
            var disposable = new DisposableMock();
            var timeoutFactory = new TimeoutFactory();

            using (var timeout = timeoutFactory.Create(TimeSpan.FromMilliseconds(1500), disposable))
            {

                Assert.Equal(TimeSpan.FromMilliseconds(1500), Assert.IsType<Timeout>(timeout).Duration);
            }
        }

        [Fact, Trait("Type", "Unit")]
        public void UsesTheGivenDisposable()
        {
            var disposable = new DisposableMock();
            var timeoutFactory = new TimeoutFactory();

            using (var timeout = timeoutFactory.Create(TimeSpan.FromMilliseconds(1500), disposable))
            {

                Assert.Same(disposable, Assert.IsType<Timeout>(timeout).ToDispose);
            }
        }

        [Fact, Trait("Type", "Unit")]
        public void DoesNotDisposeOfTheGivenInstance()
        {
            var disposable = new DisposableMock();
            var timeoutFactory = new TimeoutFactory();

            using (var timeout = timeoutFactory.Create(TimeSpan.FromMilliseconds(1500), disposable))
            {

                Assert.Equal(0, disposable.DisposedTimes);
            }
        }
    }
}
