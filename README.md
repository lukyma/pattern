[![Build Status](https://lucasmachado.visualstudio.com/MyTests/_apis/build/status/pattern?branchName=main)](https://lucasmachado.visualstudio.com/MyTests/_build/latest?definitionId=26&branchName=main)
[![NuGet](https://img.shields.io/nuget/dt/Pattern.svg)](https://www.nuget.org/packages/Pattern) 
[![NuGet](https://img.shields.io/nuget/vpre/Pattern.svg)](https://www.nuget.org/packages/Pattern)

# Introduction 
This package aims to facilitate the inclusion of design patterns applied to the Asp.net Core, 
integrating concepts of AOP (DynamicProxy).

Implemented patterns:
Strategy

# pattern
Patterns aplicados ao asp.net core.

# AOP Pattern

### Instructions

1 - Create Custom intercept attribute inheriting from ```InterceptorAttribute```

```
    public class TestInterceptorAttribute : InterceptorAttribute
    {
        protected override async Task<TResult> HandleInterceptAsync<TResult>(IInvocation invocation, Func<Task<TResult>> result)
        {
            return await result();
        }
    }
```

2 - Include attribute annotation above the method that needs to be intercepted

```

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

        public void Teste2()
        {
        }
    }

```

3 - Include in ServiceCollection with DependencyIncection with true parameter

```
services.AddScoppedStrategy<ITestService, TestService>(true);
```
Or with a different life cycle, according to your need.

 
# Strategy Pattern

### Instructions

1 - Include the following code without Startup.cs, as it will not work in its entirety without it.
```
//Logic implemented according to your code
services.AddSingletonStrategy<IStrategy<Request, Response>, RequestStrategy>();
//OR
services.AddScoppedStrategy<IStrategy<Request, Response>, RequestStrategy>();
//OR
services.services.AddTransientStrategy<IStrategy<Request, Response>, RequestStrategy>();

```

2 - Implementing the class:

```
public class RequestStrategy : IStrategy<Request, Response>

public RequestStrategy(...)
{
...
}

public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
{
    return new Response();
}
```

If you want to implement using the annotation for interceptor, include the annotation of interceptor in the method according to the code below:

```
[Validator(typeof(TestStrategyRequestValidator))]
public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
{
    return new Response();
}
```