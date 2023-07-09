using Billing.Services;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddGrpc();

        var app = builder.Build();

        app.MapGrpcService<BillingService>();
        app.MapGet("/", () => "Hello");

        app.Run();
    }
}