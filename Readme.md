[![Nuget](https://img.shields.io/nuget/dt/Protacon.NetCore.WebApi.TestUtil.svg)](https://www.nuget.org/packages/Protacon.NetCore.WebApi.TestUtil/)

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

## Example StartUp classes
### Standalone test startup
```cs
    public class TestStartup
    {
        public TestStartup(IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.Filters.Add(new ValidateModelAttribute()));

            services.AddNodeServices();

            services.AddTransient<IPdfConvert, PdfConvert>();
            services.AddTransient<IPdfStorage, GoogleCloudPdfStorage>();
            services.AddTransient<IPdfQueue, PdfQueue>();
            services.AddTransient<IErrorPages, ErrorPages>();

            services.AddDbContext<PdfDataContext>(opt => opt.UseInMemoryDatabase());

            services.AddSingleton<IPdfStorage, InMemoryPdfStorage>();

            services.AddTransient<IPdfMerger, PdfMerger>();

            services.AddHangfire(config => config.UseMemoryStorage());

            services.Configure<AppSettings>(a => a.BaseUrl = "http://localhost:5000");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseMiddleware<TestAuthenticationMiddlewareForApiKey>();
            app.UseHangfireServer();
            app.UseMvc();
        }
    }
```

### With real Startup
```cs
public class TestStartup
{
    private readonly Startup _original;

    public TestStartup(IHostingEnvironment env)
    {
        _original = new Startup(env);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        _original.ConfigureServices(services);

        services
            .RemoveService<IInvitationMailer>()
            .AddSingleton(Substitute.For<IInvitationMailer>())
            .RemoveService<ICurrentAuthenticatedUserFromExternal>()
            .AddSingleton(Substitute.For<ICurrentAuthenticatedUserFromExternal>())
            .RemoveService<ICurrentUserAccess>()
            .AddTransient(_ =>
            {
                var currentUserAccessMock = Substitute.For<ICurrentUserAccess>();
                currentUserAccessMock.CanManageGroup(Arg.Any<Guid>()).Returns(true);
                currentUserAccessMock.CanManageApp(Arg.Any<Guid>()).Returns(true);
                currentUserAccessMock.CanManageUser(Arg.Any<Guid>()).Returns(true);
                currentUserAccessMock.CanManageApplications().Returns(true);
                return currentUserAccessMock;
            });

        services
            .RemoveService<AuthorizationDataContext>()
            .RemoveService<DbContextOptions<AuthorizationDataContext>>()

        services.Configure<TokenSettings>(x =>
        {
            x.Secret = "IsolatedTestStartupIsolatedTestStartupIsolatedTestStartup";
            x.RefreshTokenSecret = "IsolatedTestStartupIsolatedTestStartupIsolatedTestStartup_Refresh";
            x.InitialTokenSecret = "IsolatedTestStartupIsolatedTestStartupIsolatedTestStartup_Initial";
            x.TokenExpiresSeconds = 120
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<TestAuthenticationMiddlewareForClientJwt>();
        app.UseMvc();
    }
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
