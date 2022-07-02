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
    }
    public class TestClassMethodInterceptor : ITestClassMethodInterceptor
    {
        [TestInterceptor]
        public virtual async Task AsyncInterceptorVoid()
        {

        }

        [TestInterceptor]
        public virtual async Task<int> AsyncInterceptorResult()
        {
            return 1;
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
    }
}
