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
        [TestInterceptor(TypeTest = "1")]
        public void Teste1()
        {
        }

        [TestInterceptor(TypeTest = "2")]
        public void Teste2()
        {
        }
    }
}
