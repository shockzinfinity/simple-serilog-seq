using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System;

namespace example
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        //.WriteTo.Console(new RenderedCompactJsonFormatter()) // RenderedCompactJsonFormatter 는 Seq logger 와 어울림
        //.WriteTo.File(new RenderedCompactJsonFormatter(), "/logs/log.ndjson")
        //.WriteTo.Seq("http://localhost:5341")
        .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://loclhost:5341")
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