using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Workshop.Microservices.ServiceB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseKestrel()
                   .UseUrls("http://*:5002")
                   .UseStartup<Startup>();
    }
}