using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using LiteDB;
using PokeRoleBot.Classes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PokeRoleBot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private LiteDatabase _database;
        private IServiceProvider _provider;
        private InteractiveService interactive;
        private readonly IConfiguration _config;

        public CommandHandlingService(IConfiguration config, IServiceProvider provider, DiscordSocketClient discord, CommandService commands,LiteDatabase database)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _database = database;
            _config = config;

            _discord.MessageReceived += MessageReceived;
            _discord.Ready += OnReady;
        }

        private async Task OnReady() 
        {
            // await Task.Run(() => new movelist().ParseMoves(_database));
            // await Task.Run(() => new ablitylist().ParseAblities(_database));
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            // Add additional initialization code here...
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos) && !message.HasStringPrefix(_config["prefix"], ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            if (result.Error.HasValue && 
                result.Error.Value != CommandError.UnknownCommand)
                await context.Channel.SendMessageAsync(result.ToString());
        }
    }
}
