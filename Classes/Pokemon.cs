using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace PokeRoleBot.Classes
{
    public class Pokemon : Base
    {
        [BsonId]
        public int Number {get;set;}
        public Type[] Types {get;set;} = new Type[2];
        public string DexName {get;set;}
        public string Title {get;set;}
        public string Description {get;set;}
        public string Height {get;set;}
        public string Weight {get;set;}
        public List<DexMoves> DexMoves {get;set;} = new List<DexMoves>(); //This is the list of moves they *can* learn, not the ones they have.
        public Score Disobedience {get;set;} = new Score();
        public Score Happyness {get;set;} = new Score(); //Only shown on team/captured pokemon
        public Score Loyalty {get;set;} = new Score(); //Only shown on team/captured pokemon
        [BsonRef("Abilities")]
        public Ability[] Abilities {get;set;} = new Ability[2];
    }

}