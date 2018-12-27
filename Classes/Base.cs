using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace PokeRoleBot.Classes
{

    public class Base
    {
        public string Name {get;set;} = ""; //Counts as Nickname for Pokemon
        [BsonRef("Moves")]
        public List<Move> Moves {get;set;} = new List<Move>();
        public Score[] PhyAtributes {get;set;} = new Score[5]; 
        //In order: Strength, Dexterity, Vitality, Special and Insight
        //Note: Trainers do not use Special
        public Score[] MentalAtributes {get;set;} = new Score[5]; 
        //In order: Tough, Cool, Beauty, Cute, Smart/Intellgence
        //Note: For trainers, the 5th value is displayed as Intelligence
        //Note: For Pokemon, the 5th value is displayed as Smart
        public Score Fight {get;set;} = new Score();
        public Score Survival {get;set;} = new Score();
        public Score Contest {get;set;} = new Score();
        public Score[] FSpecialties {get;set;} = new Score[4];
        //In order: Brawl, Throw/Channel, Evasion, Clash/Weapons
        public Score[] SSpecialties {get;set;} = new Score[4];
        //In order: Alert, Athletic, Nature, Stealth
        public Score[] CSpecialties {get;set;} = new Score[4];
        //In order: Empathy, Etiquete, Intimidate, Perform
        public int BaseHP {get;set;} = 3;
        public int HP {get;set;}
        public int MaxWill{get;set;} = 3;
        public int Will {get;set;}
    }
    public class Move
    {
        [BsonId]
        public int Id {get;set;} //Moves will be stored in a seprate collection
        public string Name {get;set;}
        public Type Type {get;set;}
        public MoveCategory MoveType {get;set;}
        public Target Target {get;set;}
        public int Power {get;set;}
        public string DamagePool {get;set;}
        public string Effects {get;set;}
        public string Description {get;set;}
        public Dictionary<string,string> Tags {get;set;} = new Dictionary<string, string>();
    }
    public class DexMoves
    {
        [BsonRef("Moves")]
        public Move Move {get;set;}
        public int Cost{get;set;} //Exp cost per move
    }
    public class Ability 
    {
        [BsonId]
        public int Id {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
    }
    public class Score  
    //Blanket class for all Physical, mental, social scores as well as all skills and specialties. 
    //This is to allow the Pokemon and Trainer class to have arrays for said values.
    {
        public int Current {get;set;} = 0;
        public int Max {get;set;} = 5;
    }
    //The following enums are in order to make user input of any field in particular easier
    //Such as allowing the user to input the word "strength" and the service will recieve this as an enum.
    //So we can take this value and reference, for example, PhyAtribute[PhysicalAtributes.Value]
    //Rather than having to parse said string in the the code
    public enum PhysicalAtributes {Strength = 0, Dexterity = 1, Vitality = 2, Special = 3, Insight = 4} 
    public enum SocialAtributes {Tough = 0, Cool = 1, Beauty = 2, Cute = 3, Smart = 4, Intellgence = 4} //Smart/Intelligence returns the same int for obvious reasons
    public enum Type {Bug, Dark, Dragon, Electric, Fairy, Fighting, Fire, Flying, Ghost, Grass, Ground, Ice, Normal, Poison, Psychic, Rock, Steel, Water, None}
    public enum Target {User, Ally, Allies, Foe, Random, Foes, Area, Battlefield}
    public enum MoveCategory {Physical, Special, Support}
    static public class Dictionaries{
        static public Dictionary<string,string> MoveTags = new Dictionary<string, string>
        {
            { "Block", "The target can't escape and can't be switched back."},
            { "Charge", "The user most spend one action charging the move. The move is used with another action on its next turn."},
            { "Fist Based","The move requries hands or fists"},
            { "Basic Heal", "Roll dice equals to half the Pokemon's HP, heal the successes."},
            { "High Heal", "Roll dice equals to the Pokemon's Full HP, heal the successes."},
            { "Fixed Heal", "Heals a fixed amount of damage."},
            { "Fixed Lethal Heal", "Heals a fixed amount of lethal damage."},
            { "High Critical", "The move can score a critical hit with just 4 successes rather than 5."},
            { "Must Recharge", "After hitting with this move, the user most rest with its first action on next the round."},
            { "Never Fail", "If the user scores or is reduced to 0 successes on its accuracy roll, this move still hits with 1 success."},
            { "Priority", "This move ignores intiative order and acts right away."},
            { "Low Priority", "This move takes effect at the end of the round, even if its used at the beginning."},
            { "Rampage","The user may use this move up to 3 times without interruption, even during the same round. It cannot evade or perform another move. After the rampage is over, the user will be confused."},
            { "Recoil","The user will be hurt my its own move. Roll damage normally against your fue, then roll again each success you scored as damage to the user ignoring its defenses."},
            { "Shield", "If the Pokemon performs another shield move during the same round, that move's accuracy roll is lowered by 2."},
            { "Sound Based","These moves bypass substitute, Light Screen, Reflect and Cover."},
            { "Status Chance","This move has a chance to apply a status."},
            { "Double Actions", "You can use this move twice in a single round through multiple actions."},
            { "Successive Actions", "You can use this move up to 5 times in a single round through multiple actions."},
            { "Switcher Move","Only one switcher move may be used per round. Switch a pokemon in the battlefield for one of its allies still on their pokeball."},
            { "Weather", "The move changes the local weather."}
        };
        static public Dictionary<Type,Type[]> Strengths = new Dictionary<Type, Type[]> 
        {
            { Type.Bug, new Type[]{Type.Dark,Type.Grass, Type.Psychic}},
            { Type.Dark, new Type[] {Type.Ghost, Type.Psychic}},
            { Type.Dragon, new Type[] {Type.Dragon}},
            { Type.Electric, new Type[] {Type.Flying, Type.Water}},
            { Type.Fairy, new Type[]{Type.Dark, Type.Dragon, Type.Fighting}},
            { Type.Fighting, new Type[]{Type.Ice, Type.Normal, Type.Rock, Type.Steel}},
            { Type.Fire, new Type[]{Type.Bug, Type.Grass, Type.Ice, Type.Steel}},
            { Type.Flying, new Type[]{Type.Bug, Type.Fighting, Type.Grass}},
            { Type.Ghost, new Type[]{Type.Ghost, Type.Psychic}},
            { Type.Grass, new Type[]{Type.Ground, Type.Rock, Type.Water}},
            { Type.Ground, new Type[]{Type.Electric, Type.Fire, Type.Poison, Type.Rock, Type.Steel}},
            { Type.Ice, new Type[]{Type.Dragon, Type.Flying, Type.Grass, Type.Ground}},
            { Type.Normal, new Type[]{}},
            { Type.Poison, new Type[]{Type.Fairy, Type.Grass}},
            { Type.Psychic, new Type[]{Type.Fighting,Type.Poison}},
            { Type.Rock, new Type[]{Type.Bug, Type.Fire, Type.Flying, Type.Ice}},
            { Type.Steel, new Type[]{Type.Fairy, Type.Ice, Type.Rock}},
            { Type.Water, new Type[]{Type.Fire, Type.Ground, Type.Rock}}
        };
        static public Dictionary<Type,Type[]> Weakness = new Dictionary<Type, Type[]> 
        {
            { Type.Bug, new Type[]{Type.Fairy,Type.Fighting, Type.Fire, Type.Flying, Type.Ghost, Type.Steel}},
            { Type.Dark, new Type[] {Type.Dark, Type.Fairy, Type.Fighting}},
            { Type.Dragon, new Type[] {Type.Steel}},
            { Type.Electric, new Type[] {Type.Dragon, Type.Electric, Type.Grass}},
            { Type.Fairy, new Type[]{Type.Fire, Type.Poison, Type.Steel}},
            { Type.Fighting, new Type[]{Type.Bug, Type.Fairy, Type.Flying, Type.Poison, Type.Psychic}},
            { Type.Fire, new Type[]{Type.Dragon, Type.Fire, Type.Rock, Type.Water}},
            { Type.Flying, new Type[]{Type.Electric, Type.Rock, Type.Steel}},
            { Type.Ghost, new Type[]{Type.Dark}},
            { Type.Grass, new Type[]{Type.Bug, Type.Dragon, Type.Fire, Type.Flying, Type.Grass, Type.Poison, Type.Steel}},
            { Type.Ground, new Type[]{Type.Bug, Type.Grass}},
            { Type.Ice, new Type[]{Type.Fire, Type.Ice, Type.Steel, Type.Water}},
            { Type.Normal, new Type[]{Type.Ghost}},
            { Type.Poison, new Type[]{Type.Ghost, Type.Ground, Type.Poison, Type.Rock}},
            { Type.Psychic, new Type[]{Type.Psychic,Type.Steel}},
            { Type.Rock, new Type[]{Type.Fighting, Type.Ground, Type.Steel}},
            { Type.Steel, new Type[]{Type.Electric, Type.Fire, Type.Steel, Type.Water}},
            { Type.Water, new Type[]{Type.Dragon, Type.Grass, Type.Water}}
        };
        static public Dictionary<Type,Type[]> Immunities = new Dictionary<Type, Type[]> 
        {
            { Type.Bug, new Type[]{}},
            { Type.Dark, new Type[]{}},
            { Type.Dragon, new Type[] {Type.Fairy}},
            { Type.Electric, new Type[] {Type.Ground}},
            { Type.Fairy, new Type[]{}},
            { Type.Fighting, new Type[]{Type.Ghost}},
            { Type.Fire, new Type[]{}},
            { Type.Flying, new Type[]{}},
            { Type.Ghost, new Type[]{Type.Normal}},
            { Type.Grass, new Type[]{}},
            { Type.Ground, new Type[]{Type.Flying}},
            { Type.Ice, new Type[]{}},
            { Type.Normal, new Type[]{Type.Ghost}},
            { Type.Poison, new Type[]{Type.Steel}},
            { Type.Psychic, new Type[]{Type.Dark}},
            { Type.Rock, new Type[]{}},
            { Type.Steel, new Type[]{}},
            { Type.Water, new Type[]{}}
        };
    }
}