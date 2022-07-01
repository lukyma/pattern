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
        [TestInterceptor(Order = 1, TypeTest = "teste1")]
        public void Teste1()
        {
        }

        [TestInterceptor(Order = 1, TypeTest = "teste2")]
        public void Teste2()
        {
        }
    }
}
