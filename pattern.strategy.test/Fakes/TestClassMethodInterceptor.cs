using pattern.strategy.test.Fakes.Interceptor;
using System.Threading.Tasks;

namespace pattern.strategy.test.Fakes
{
    public interface ITestClassMethodInterceptor
    {
        Task AsyncInterceptorVoid();
        Task<int> AsyncInterceptorResult();
        void SyncInterceptorVoid();
        int SyncInterceptorResult();
        void SyncInterceptorVoidException();
        Task AsyncInterceptorVoidException();
    }
    public class TestClassMethodInterceptor : ITestClassMethodInterceptor
    {
        [TestInterceptor]
        public virtual async Task AsyncInterceptorVoid()
        {
            await Task.CompletedTask;
        }

        [TestInterceptor]
        public virtual async Task<int> AsyncInterceptorResult()
        {
            return await Task.FromResult(1);
        }

        [TestInterceptor]
        public void SyncInterceptorVoid()
        {
        }

        [TestInterceptor]
        public int SyncInterceptorResult()
        {
            return 1;
        }

        public void SyncInterceptorVoidException()
        {
            throw new System.NotImplementedException();
        }

        public Task AsyncInterceptorVoidException()
        {
            throw new System.NotImplementedException();
        }
    }
}
