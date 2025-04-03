using patterns.strategy;
using System;
using Xunit;

namespace pattern.strategy.test.Tests.Interceptor
{
    public class RethrowHelperTest
    {
        [Fact]
        public void Rethrow_ExceptionNull()
        {
            Assert.Throws<ArgumentNullException>(() => RethrowHelper.Rethrow(null));
        }

        [Fact]
        public void RethrowInnerIfAggregate_ExceptionNull()
        {
            Assert.Throws<ArgumentNullException>(() => RethrowHelper.RethrowInnerIfAggregate(null));
        }

        [Fact]
        public void RethrowIfFaulted_ExceptionNull()
        {
            Assert.Throws<ArgumentNullException>(() => RethrowHelper.RethrowInnerIfAggregate(null));
        }
    }
}
