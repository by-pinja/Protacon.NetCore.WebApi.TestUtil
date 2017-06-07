# Test utilities for Net Core web api

This is lightweight wrapper and collection of useful tools to work with .Net Core isolated test host.

# Examples
## Example GET
```cs
    [Fact]
    public void WhenGetIsCalled_ThenAssertingItWorks()
    {
        TestHost.Run<TestStartup>().Get("/returnthree/")
            .ExpectStatusCode(HttpStatusCode.OK)
            .WithContentOf<int>()
            .Passing(
                x => x.Should().Be(3));
    }
```

## Example POST
```cs
    [Fact]
    public void WhenPostIsCalled_ThenAssertingItWorks()
    {
        TestHost.Run<TestStartup>().Post("/returnsame/", new DummyRequest { Value = "3" })
            .ExpectStatusCode(HttpStatusCode.OK)
            .WithContentOf<DummyRequest>()
            .Passing(x => x.Value.Should().Be("3"));
    }
```

## Further
See complete list of examples from test project.

## Mocking depencies from DI
```cs
host.MockSetup<IExternalDepency>(x => x.SomeCall(Arg.Is("abc")).Returns("3"));
```

## Replacing depencies in test host
```cs
    public void ConfigureServices(IServiceCollection services)
    {
        _original.ConfigureServices(services);

        services.RemoveService<IExternalDepency>()
            .AddSingleton(Substitute.For<IExternalDepency>());
    }
```

## Asserting and mocking
These utilities are framework independant. Use your favorite assertion and mocking libraries.
