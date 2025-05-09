using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.ListenAnyIP(443, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            listenOptions.UseHttps(httpsOptions =>
                {
                    httpsOptions.UseLettuceEncrypt(kestrel.ApplicationServices);
                }
            );
        }
    );
});

builder.Services.AddLettuceEncrypt();

builder
    .Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder
            .Configuration
            .GetSection("ReverseProxy")
    );

var app = builder.Build();

app.UseRouting();
app.MapReverseProxy();

app.Run();
