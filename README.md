# Introduction 
This package aims to facilitate the inclusion of design patterns applied to the Asp.net Core, 
integrating concepts of AOP (DynamicProxy) and Validation using FluentValidation.

Implemented patterns:
Strategy

# pattern
Patterns aplicados ao asp.net core.
 
1 - Strategy Pattern

# Intructions

1 - Include the following code without Startup.cs, as it will not work in its entirety without it.
```

services.AddSingleton<IProxyGenerator, ProxyGenerator>();
services.AddScoped<IAsyncValidatorInterceptor, ValidatorInterceptor>();

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

If you want to implement using the annotation for validation (FluentValidation), include the annotation in the `` `HandleAsync``` method according to the code below:

```
[Validator(typeof(TestStrategyRequestValidator))]
public async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
{
    return new Response();
}
```