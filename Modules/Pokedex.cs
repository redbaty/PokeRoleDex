using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using LiteDB;
using PokeRoleBot.Classes;

namespace PokeRoleBot.Modules
{
    public class Pokedex : InteractiveBase<SocketCommandContext>
    {
        public LiteDatabase Database;
        public readonly IConfiguration Config;

        [Command("Pokemon", RunMode = RunMode.Async)]
        [Summary("Finds and shows you the information of a Pokemon! Usage: .Pokemon <Name>")]
        public async Task FindPokemon([Remainder] string Name){
            //Get the pokemon collection
            var db = Database.GetCollection<Pokemon>("Pokemon");
        }
    }
    
}