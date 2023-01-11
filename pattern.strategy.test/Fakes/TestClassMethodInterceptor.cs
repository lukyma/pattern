using pattern.strategy.test.Fakes.Interceptor;
using System.Threading.Tasks;

namespace pattern.strategy.test.Fakes
{
    public interface ITestClassMethodInterceptor
    {
        Task AsyncInterceptorVoid();
        Task<int> AsyncInterceptorResult();
        void SyncInterceptorVoid();
        void SyncInterceptorVoid(int teste);
        int SyncInterceptorResult();
        void SyncInterceptorVoidException();
        Task AsyncInterceptorVoidException();
        Task AsyncWithoutInterceptorVoidException();
    }
    public class TestClassMethodInterceptor : ITestClassMethodInterceptor
    {
        [TestInterceptor]
        public virtual async Task AsyncInterceptorVoid()
        {
            await Task.Delay(2000);
            await Task.CompletedTask;
        }

        [TestInterceptor]
        public virtual async Task<int> AsyncInterceptorResult()
        {
            return await Task.FromResult(1);
        }

        [TestInterceptor]
        [TestInterceptor2(Order = 1)]
        public void SyncInterceptorVoid()
        {
        }

        [TestInterceptor2(Order = 1)]
        public void SyncInterceptorVoid(int teste)
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

        public Task AsyncWithoutInterceptorVoidException()
        {
            throw new System.NotImplementedException();
        }
    }
}
