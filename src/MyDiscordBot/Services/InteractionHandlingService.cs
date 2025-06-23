using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MyDiscordBot.Services;

public class InteractionHandlingService : IHostedService {
	private readonly DiscordSocketClient _client;
	private readonly InteractionService _iservice;
	private readonly IServiceProvider _services;
	private readonly IConfiguration _config;
	private readonly ILogger<InteractionService> _logger;

	public InteractionHandlingService(
			DiscordSocketClient client,
			InteractionService iservice,
			IServiceProvider services,
			IConfiguration config,
			ILogger<InteractionService> logger
			) {
		this._client = client;
		this._iservice = iservice;
		this._services = services;
		this._config = config;
		this._logger = logger;

		this._iservice.Log +=
				msg => LogHelper.OnLogAsync(this._logger, msg);
	}


	public async Task StartAsync(CancellationToken cancellationToken) {
		this._client.Ready +=
				() => this._iservice.RegisterCommandsGloballyAsync(true);
		this._client.InteractionCreated += this.OnInteractionAsync;

		await this._iservice.AddModulesAsync(Assembly.GetEntryAssembly(),
																				this._services);
	}

	public /*async*/ Task StopAsync(CancellationToken cancellationToken) {
		this._iservice.Dispose();
		return Task.CompletedTask;
	}

	private async Task OnInteractionAsync(SocketInteraction interaction) {
		try {
			var context = new SocketInteractionContext(this._client,
																								interaction);
			var result = await this._iservice.ExecuteCommandAsync(context,
																										this._services);

			if (!result.IsSuccess) {
				await context.Channel.SendMessageAsync(result.ToString());
			}
		} catch {
			if (interaction.Type == InteractionType.ApplicationCommand) {
				await interaction.GetOriginalResponseAsync()
						.ContinueWith(msg => msg.Result.DeleteAsync());
			}
		}
	}
}
