using pattern.sample.api.StrategyHandler.Validator;
using patterns.strategy;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.sample.api.StrategyHandler
{
    public class TestStrategy : IStrategy<TestStrategyRequest, TestStrategyResponse>
    {
        [Validator(typeof(TestStrategyRequestValidator))]
        public async Task<TestStrategyResponse> HandleAsync(TestStrategyRequest request, CancellationToken cancellationToken)
        {
            return new TestStrategyResponse();
        }
    }
    public class TestStrategyRequest
    {
        public string Name { get; set; }
    }
    public class TestStrategyResponse
    {
    }
}
