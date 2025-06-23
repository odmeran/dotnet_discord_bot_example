using Discord.Interactions;
using System;
using System.Threading.Tasks;

namespace MyDiscordBot.Modules;

public class MathModule : InteractionModuleBase <SocketInteractionContext> {
	[SlashCommand("isnumber", "Check if the input text is a number.")]
	public Task IsNumber(double number) =>
		RespondAsync($"The text {number} is a number!", ephemeral: true);

	[SlashCommand("multiply", "Get the product of two numbers.")]
	public async Task Multiply(int a, int b) {
		long product = Math.BigMul(a, b);
		await RespondAsync($"The product of `{a} * {b}` is `{product}`.",
												ephemeral: true);
	}

	[SlashCommand("sqroot", "Get the square root of a number.")]
	public async Task Sqroot(double number) {
		double root = Math.Sqrt(number);
		await RespondAsync($"The square root of `{number}` is `{root}`.",
												ephemeral: true);
	}
}

