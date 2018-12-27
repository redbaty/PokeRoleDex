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
using System.Text;

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
            var pkm = Database.GetCollection<Pokemon>("Pokemon");
            var moves = Database.GetCollection<Move>("Moves");
            var abs = Database.GetCollection<Ability>("Abilities");
            

        }


        public async Embed PokemonEntry(Pokemon pkm){
            var sb = new StringBuilder();
            var type = pkm.Types.Count() >1 ? pkm.Types[0]+"/"+pkm.Types[1] : pkm.Types[1].ToString();
            var embed = new EmbedBuilder()
                .WithTitle(type+"\n"+pkm.DexName+" #"+pkm.Number)
                .WithDescription("Avg. Height: "+pkm.Height+" Avg. Weight: "+pkm.Weight)
                .AddField(pkm.Title+"Pokemon",pkm.Description);
            embed.AddField("Atributes",
                "```css"+
                "Strength | "+new String('⚫',pkm.PhyAtributes[0].Current)+new String('⚪',pkm.PhyAtributes[0].Current-pkm.PhyAtributes[0].Current)+
                "Dexterity| "+new String('⚫',pkm.PhyAtributes[1].Current)+new String('⚪',pkm.PhyAtributes[1].Current-pkm.PhyAtributes[1].Current)+
                "Vitality | "+new String('⚫',pkm.PhyAtributes[2].Current)+new String('⚪',pkm.PhyAtributes[2].Current-pkm.PhyAtributes[2].Current)+
                "Special  | "+new String('⚫',pkm.PhyAtributes[3].Current)+new String('⚪',pkm.PhyAtributes[3].Current-pkm.PhyAtributes[3].Current)+
                "Insight  | "+new String('⚫',pkm.PhyAtributes[4].Current)+new String('⚪',pkm.PhyAtributes[4].Current-pkm.PhyAtributes[4].Current)+"```",true);
            embed.AddField("More Info","```css"+
                "Base HP: ["+pkm.BaseHP+"]"+
                "Disobedience: "+new String('⚫',pkm.Disobedience.Current)+new String('⚪',pkm.Disobedience.Max-pkm.Disobedience.Current));
        }
    }
    
}