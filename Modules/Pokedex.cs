using System;
using System.IO;
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
        public LiteDatabase Database {get;set;}
        public  IConfiguration Config {get;set;}

        [Command("Pokemon", RunMode = RunMode.Async)]
        public async Task FindPokemon([Remainder] string Name){
            //Get the pokemon collection
            var pkm = Database.GetCollection<Pokemon>("Pokemon");
            var moves = Database.GetCollection<Move>("Moves");
            var abs = Database.GetCollection<Ability>("Abilities");
            
            var results = pkm
                .IncludeAll()
                .Find(x => x.DexName.Contains(Name.ToLower()));
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
            int index = 0;
            int limit = results.Count()-1;
            var n = new Emoji("▶");
            var p = new Emoji("◀");
            var msg = await ReplyAsync("Searching the pokedex...");

            await msg.AddReactionAsync(p); //Add Previous button
            await msg.AddReactionAsync(n); //Add Next button

            Interactive.AddReactionCallback(msg,new InlineReactionCallback(Interactive,Context,new ReactionCallbackData(Context.User.Mention+", Here are the results of your search! This pokedex result will be interactable for 5 minutes.",PokemonEntry(results.ElementAt(index)),false,false,TimeSpan.FromMinutes(5))
            .WithCallback(p,(c,r) => GoBack(r,c,msg,index,limit)) 
            .WithCallback(n,(c,r) => Advance(r,c,msg,index,limit))
            ));
        }
        [Command("Move", RunMode = RunMode.Async)]
        public async Task FindMove([Remainder] string Name){
            var moves = Database.GetCollection<Move>("Moves");
            var results = moves.IncludeAll().Find(x => x.Name.Contains(Name.ToLower()));
            if (results.Count() == 0){
                await ReplyAsync(Context.User.Mention+", No moves was found with that name.");
                return;
            }

            if(results.Count() > 20){
                await ReplyAsync(Context.User.Mention+" Too many results! Narrow your search a bit.");
            }

            if(results.Count() == 1){
                await ReplyAsync(Context.User.Mention+", Move found!", false,MoveEmbed(results.First()));
                return;
            }
            int index = 0;
            int limit = results.Count()-1;
            var n = new Emoji("▶");
            var p = new Emoji("◀");
            var msg = await ReplyAsync("Searching the movedex...");
            await msg.AddReactionAsync(p); //Add Previous button
            await msg.AddReactionAsync(n); //Add Next button

            Interactive.AddReactionCallback(msg,new InlineReactionCallback(Interactive,Context,new ReactionCallbackData(Context.User.Mention+", Here are the results of your search! This pokedex result will be interactable for 5 minutes.",MoveEmbed(results.ElementAt(index)),false,false,TimeSpan.FromMinutes(5))
            .WithCallback(p,(c,r) => GoBack(r,c,msg,index,limit)) 
            .WithCallback(n,(c,r) => Advance(r,c,msg,index,limit))
            ));
        }
        [Command("Ability", RunMode = RunMode.Async)]
        public async Task Findab([Remainder] string Name){
            var abs = Database.GetCollection<Ability>("Abilities");
            var results = abs.IncludeAll().Find(x => x.name.Contains(Name.ToLower()));
            if (results.Count() == 0){
                await ReplyAsync(Context.User.Mention+", No abilities was found with that name.");
                return;
            }

            if(results.Count() > 20){
                await ReplyAsync(Context.User.Mention+" Too many results! Narrow your search a bit.");
            }

            if(results.Count() == 1){
                await ReplyAsync(Context.User.Mention+", Ability found!", false,AbilityEmbed(results.First()));
                return;
            }
            int index = 0;
            int limit = results.Count()-1;
            var n = new Emoji("▶");
            var p = new Emoji("◀");
            var msg = await ReplyAsync("Searching the Abilitydex...");
            await msg.AddReactionAsync(p); //Add Previous button
            await msg.AddReactionAsync(n); //Add Next button

            Interactive.AddReactionCallback(msg,new InlineReactionCallback(Interactive,Context,new ReactionCallbackData(Context.User.Mention+", Here are the results of your search! This pokedex result will be interactable for 5 minutes.",AbilityEmbed(results.ElementAt(index)),false,false,TimeSpan.FromMinutes(5))
            .WithCallback(p,(c,r) => GoBack(r,c,msg,index,limit)) 
            .WithCallback(n,(c,r) => Advance(r,c,msg,index,limit))
            ));
        }
        
        public Embed PokemonEntry(Pokemon pkm){
            var sb = new StringBuilder();
            var type = pkm.Types.Count() >1 ? pkm.Types[0]+"/"+pkm.Types[1] : pkm.Types[1].ToString();
            string abs = pkm.Abilities.Count() >1 ? pkm.Abilities[0].name+"\n"+pkm.Abilities[1].name : pkm.Abilities[0].name;
            var embed = new EmbedBuilder()
                .WithTitle(pkm.DexName+" #"+pkm.Number)
                .WithDescription(type+"\nAvg. Height: "+pkm.Height+" Avg. Weight: "+pkm.Weight)
                .AddField(pkm.Title+"Pokemon",pkm.Description)
                .WithColor(Dictionaries.Colors.GetValueOrDefault(pkm.Types[0]))
                .WithThumbnailUrl(Directory.GetCurrentDirectory()+@"/Data/"+pkm.Number+".png");
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
        public Embed MoveEmbed(Move move){
            var embed = new EmbedBuilder()
                .WithTitle(move.Name)
                .WithDescription(move.MoveType.ToString()+"\n"+move.Description)
                .AddField("Type", move.Type.ToString(),true)
                .AddField("Targets",move.Target.ToString(),true)
                .AddField("Power",move.Power,true)
                .AddField("Accuracy Pool",move.Accuracy, true)
                .AddField("Damage Pool",string.Join(" + ",move.DamagePool)+" + "+move.Power.ToString(),true)
                .AddField("Effects",move.Effects)
                .WithColor(Dictionaries.Colors.GetValueOrDefault(move.Type));
            return embed.Build();
        }
        public Embed AbilityEmbed(Ability Ability){
            var embed = new EmbedBuilder()
                .WithTitle(Ability.name)
                .WithDescription(Ability.description)
                .AddField("Effect", Ability.effect);
            return embed.Build();
        }
        public async Task Advance(SocketReaction reaction, SocketCommandContext context, IUserMessage msg,int index, int limit){
                if (index == limit){
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