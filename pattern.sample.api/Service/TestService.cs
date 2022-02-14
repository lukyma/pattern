using pattern.sample.api.Interceptor;

namespace pattern.sample.api.Service
{
    public interface ITestService
    {
        void Teste1();
        void Teste2();
    }

    public class TestService : ITestService
    {
        [TestInterceptor]
        public void Teste1()
        {
        }

        [TestInterceptor2]
        public void Teste2()
        {
        }
    }
}
