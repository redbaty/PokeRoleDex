using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;

namespace PokeRoleBot.Classes
{
    public class movelist {
        public JsonMove[] moves {get;set;}
        public void ParseMoves(LiteDatabase database){
        var movejson = JsonConvert.DeserializeObject<movelist>(File.ReadAllText(Directory.GetCurrentDirectory()+@"/Data/Import/moves.json"));
            var Movedb = database.GetCollection<Move>("Moves");
            int counter = 0;
            foreach (var x in movejson.moves){
                var m = new Move(){
                    Name = x.name,
                    Description = x.description,
                    Accuracy = x.accuracy,
                    Effects = x.effect,
                    Power = Char.IsNumber(x.power,0) ? int.Parse(x.power) : 0,
                    Type = Enum.Parse<PokeRoleBot.Classes.Type>(x.type),
                    Target = Dictionaries.ParseTarget.GetValueOrDefault(x.targets),
                    MoveType = Enum.Parse<MoveCategory>(x.category),
                    DamagePool = Regex.Matches(x.damage_pool,@"\b[A-Za-z]+").Cast<Match>().Select(match => match.Value).ToArray()
                };
                Movedb.Insert(m);
                Console.WriteLine(m.Name);
                counter++;
            }
            Movedb.EnsureIndex(x=> x.Name,"LOWER($.Name)");
            Console.WriteLine("Successcully Parsed "+counter+" Moves.");
        }
    }
    public class JsonMove{
        public string name {get;set;}
        public string type {get;set;}
        public string targets {get;set;}
        public string power {get;set;}
        public string category {get;set;}
        public string accuracy {get;set;}
        public string damage_pool {get;set;}
        public string effect {get;set;}
        public string description {get;set;}
    }
    public class ablitylist{
        public Ability[] abilities {get;set;}
        public void ParseAblities(LiteDatabase database){
            var absjsons = JsonConvert.DeserializeObject<ablitylist>(File.ReadAllText(Directory.GetCurrentDirectory()+@"/Data/Import/abilities.json"));
            var abs = database.GetCollection<Ability>("Abilities");
            int counter = 0;
            foreach (var x in absjsons.abilities)
            {
                abs.Insert(new Ability(){
                    name = x.name,
                    description = x.description,
                    effect = x.effect
                });
                Console.WriteLine(x.name);
                counter++;
            }
            abs.EnsureIndex(x=> x.name,"LOWER($.name)");
            Console.WriteLine("Successcully Parsed "+counter+" Abilities.");
        }
    }
}