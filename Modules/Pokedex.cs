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
        public int index {get;set;} = 0;
        public IEnumerable<Pokemon> results {get;set;}

        [Command("Pokemon", RunMode = RunMode.Async)]
        [Summary("Finds and shows you the information of a Pokemon! Usage: .Pokemon <Name>")]
        public async Task FindPokemon([Remainder] string Name){
            //Get the pokemon collection
            var pkm = Database.GetCollection<Pokemon>("Pokemon");
            var moves = Database.GetCollection<Move>("Moves");
            var abs = Database.GetCollection<Ability>("Abilities");
            
            results = pkm
                .IncludeAll()
                .Find(x => x.DexName.ToLower().Contains(Name.ToLower()));
            if (results.Count() == 0){
                await ReplyAsync(Context.User.Mention+", No pokemon was found with that name.");
                return;
            }

            if(results.Count() > 20){
                await ReplyAsync(Context.User.Mention+" Too many results! Narrow your search a bit.");
            }

            if(results.Count() == 1){
                await ReplyAsync(Context.User.Mention+", Pokemon found!", false,PokemonEntry(results.First()));
                return;
            }
            var n = new Emoji("▶");
            var p = new Emoji("◀");
            var msg = await ReplyAsync("Searching the pokedex...");

            await msg.AddReactionAsync(p); //Add Previous button
            await msg.AddReactionAsync(n); //Add Next button

            Interactive.AddReactionCallback(msg,new InlineReactionCallback(Interactive,Context,new ReactionCallbackData(Context.User.Mention+", Here are the results of your search! This pokedex result will be interactable for 5 minutes.",PokemonEntry(results.ElementAt(index)),false,false,TimeSpan.FromMinutes(5))
            .WithCallback(p,(c,r) => GoBack(r,c,msg)) 
            .WithCallback(n,(c,r) => Advance(r,c,msg))
            ));
        }


        public Embed PokemonEntry(Pokemon pkm){
            var sb = new StringBuilder();
            var type = pkm.Types.Count() >1 ? pkm.Types[0]+"/"+pkm.Types[1] : pkm.Types[1].ToString();
            string abs = pkm.Abilities.Count() >1 ? pkm.Abilities[0].Name+"\n"+pkm.Abilities[1].Name : pkm.Abilities[0].Name;
            var embed = new EmbedBuilder()
                .WithTitle(type+"\n"+pkm.DexName+" #"+pkm.Number)
                .WithDescription("Avg. Height: "+pkm.Height+" Avg. Weight: "+pkm.Weight)
                .AddField(pkm.Title+"Pokemon",pkm.Description)
                .WithColor(Dictionaries.Colors.GetValueOrDefault(pkm.Types[0]))
                .WithThumbnailUrl(@"/Data/"+pkm.Number+".png");
            embed.AddField("Atributes",
                "```css"+
                "Strength | "+new String('⚫',pkm.PhyAtributes[0].Current)+new String('⚪',pkm.PhyAtributes[0].Current-pkm.PhyAtributes[0].Current)+
                "Dexterity| "+new String('⚫',pkm.PhyAtributes[1].Current)+new String('⚪',pkm.PhyAtributes[1].Current-pkm.PhyAtributes[1].Current)+
                "Vitality | "+new String('⚫',pkm.PhyAtributes[2].Current)+new String('⚪',pkm.PhyAtributes[2].Current-pkm.PhyAtributes[2].Current)+
                "Special  | "+new String('⚫',pkm.PhyAtributes[3].Current)+new String('⚪',pkm.PhyAtributes[3].Current-pkm.PhyAtributes[3].Current)+
                "Insight  | "+new String('⚫',pkm.PhyAtributes[4].Current)+new String('⚪',pkm.PhyAtributes[4].Current-pkm.PhyAtributes[4].Current)+"```",true);
            embed.AddField("More Info","```css"+
                "Base HP: ["+pkm.BaseHP+"]"+
                "Disobedience\n"+new String('⚫',pkm.Disobedience.Current)+new String('⚪',pkm.Disobedience.Max-pkm.Disobedience.Current)+
                "---------\nAbilities\n"+abs);
            foreach(var x in pkm.DexMoves)
            {
                sb.AppendLine("["+x.Cost.ToString("D2")+"] "+x.Move.Name);
            }
            embed.AddField("Moves",sb.ToString());

            return embed.Build();
        }
        public async Task Advance(SocketReaction reaction, SocketCommandContext context, IUserMessage msg){
                if (index == results.Count()-1){
                    await msg.RemoveReactionAsync(reaction.Emote,reaction.User.Value);
                    return;
                }
                else {
                    index++;
                    await msg.ModifyAsync(x => x.Embed = PokemonEntry(results.ElementAt(index)));
                    await msg.RemoveReactionAsync(reaction.Emote,reaction.User.Value);
                    return;
                }
            }
            public async Task GoBack(SocketReaction reaction, SocketCommandContext context, IUserMessage msg){
                if (index == 0){
                    await msg.RemoveReactionAsync(reaction.Emote,reaction.User.Value);
                    return;
                }
                else {
                    index--;
                    await msg.ModifyAsync(x => x.Embed = PokemonEntry(results.ElementAt(index)));
                    await msg.RemoveReactionAsync(reaction.Emote,reaction.User.Value);
                    return;
                }
            }
    
    }
    
}