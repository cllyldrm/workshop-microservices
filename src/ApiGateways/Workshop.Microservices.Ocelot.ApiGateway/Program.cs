using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Workshop.Microservices.Ocelot.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration(ic => ic.AddJsonFile("ocelot.json"))
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseKestrel()
                   .UseUrls("http://*:5000")
                   .UseStartup<Startup>();
    }
}