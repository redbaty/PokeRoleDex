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
        public string DexName {get;set;}
        public string Title {get;set;}
        public string Description {get;set;}
        public List<Move> DexMoves {get;set;} = new List<Move>(); //This is the list of moves they *can* learn, not the ones they have.
        public Score Disobedience {get;set;} = new Score();
        public Score Happyness {get;set;} = new Score(); //Only shown on team/captured pokemon
        public Score Loyalty {get;set;} = new Score(); //ONly shown on team/captured pokemon
    }

}