using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.IO;

namespace example
{
  public class Program
  {
    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
      .AddEnvironmentVariables()
      .Build();

    public static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        //.WriteTo.Console(new RenderedCompactJsonFormatter()) // RenderedCompactJsonFormatter 는 Seq logger 와 어울림
        .WriteTo.File(
          new RenderedCompactJsonFormatter(),
          "./logs/log.json",
          rollingInterval: RollingInterval.Day,
          rollOnFileSizeLimit: true, // maximum 1GB, could be null
          retainedFileCountLimit: 31) // number of files, could be null
          //.WriteTo.Seq("http://localhost:5341")
          //.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341", apiKey: "l5ElRnJOpatCcDUjnAoI")
        .WriteTo.Seq(Configuration.GetValue<string>("SEQ_URL"), apiKey: Configuration.GetValue<string>("SEQ_API_KEY"))
        .CreateLogger();

      try
      {
        Log.Information("Starting up...");
        CreateHostBuilder(args).Build().Run();
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Application start-up failed.");
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .UseSerilog()
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.UseStartup<Startup>();
          });
  }
}