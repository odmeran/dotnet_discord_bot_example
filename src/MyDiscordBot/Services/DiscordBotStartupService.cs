using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Discord;
using Discord.WebSocket;

namespace MyDiscordBot.Services;

public class DiscordBotStartupService : IHostedService {
	private readonly DiscordSocketClient _client;
	private readonly IConfiguration _config;
	private readonly ILogger<DiscordSocketClient> _logger;

	public DiscordBotStartupService(DiscordSocketClient client,
																IConfiguration config,
																ILogger<DiscordSocketClient> logger) {
		this._client = client;
		this._config = config;
		this._logger = logger;

		this._client.Log += msg => LogHelper.OnLogAsync(this._logger, msg);
	}

	public async Task StartAsync(CancellationToken cancellationToken) {
		await this._client.LoginAsync(TokenType.Bot,
																	this._config["Discord:Token"]);
		await this._client.StartAsync();
	}

	public async Task StopAsync(CancellationToken cancellationToken) {
		await this._client.LogoutAsync();
		await this._client.StopAsync();
	}
}
