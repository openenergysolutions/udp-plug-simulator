// SPDX-FileCopyrightText: 2022 Open Energy Solutions Inc
//
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace UdpPlugSimulator
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var fileOption = new Option<FileInfo>(
            aliases: new string[] { "--config", "-c" },
            description: "Path to the configuration file.  Default is /config/config.json");

            var rootCommand = new RootCommand("UDP Plug Simulator");
            rootCommand.AddOption(fileOption);

            rootCommand.SetHandler(async (file) =>
            {
                await Run(file!);
            },
                fileOption);

            return await rootCommand.InvokeAsync(args);
        }

        static async Task Run(FileInfo info)
        {
            string config;

            if (info == null)
            {
                config = "/config/config.json";

                if (!File.Exists(config))
                {
                    config = "./config/config.json";
                }
            }
            else
            {
                config = info.FullName;
            }

            if (File.Exists(config))
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile(config)
                    .AddEnvironmentVariables()
                    .Build();

                Settings settings = configuration.GetSection("Settings").Get<Settings>();

                using IHost host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                        services
                            .AddLogging(opt =>
                            {
                                opt.AddSimpleConsole(c =>
                                {
                                    c.TimestampFormat = "[HH:mm:ss] ";
                                });
                            })
                            .AddSingleton<ISettings>(settings)
                            .AddSingleton<IDeviceManager, DeviceManager>()
                            )
                    .Build();

                using IServiceScope serviceScope = host.Services.CreateScope();
                IServiceProvider provider = serviceScope.ServiceProvider;

                var manager = provider.GetRequiredService<IDeviceManager>();
                manager.Start();

                await host.RunAsync();

                manager.Stop();
            }
            else
            {
                Console.WriteLine($"Config file not found. [{config}]");
            }
        }
    }
}