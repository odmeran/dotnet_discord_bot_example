using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Discord.Interactions;
using MyDiscordBot.Services;

using IHost host = Host.CreateDefaultBuilder(args)
		.ConfigureAppConfiguration(config => {
				// Add the config file to IConfiguration variables.
				//config.AddYamlFile("_config.yml", false);
				config.AddJsonFile("appsettings.json");
		})
		.ConfigureServices(services => {
				// Add the discord client to services.
				// ! Register DiscordSocketClient first.
				services.AddSingleton<DiscordSocketClient>();

				// Add the interaction service to services.
				// ! Register InteractionService with the required
				//   dependencies.
				services.AddSingleton<InteractionService>(provider => {
					var client
							= provider.GetRequiredService<DiscordSocketClient>();
					return new InteractionService(client);
				});

				// ! Register hosted services.
				// Add the slash command handler.
				services.AddHostedService<InteractionHandlingService>();
				// Add the discord startup service.
				services.AddHostedService<DiscordBotStartupService>();
		})
		.Build();

await host.RunAsync();
