using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Fighting_2
{
    public class GameModel
    {
        public enum Side
        {
            Player,
            AI
        }
        public Side Turn;
        public Dictionary<Side, List<Fighter>> Fighters;
        public int indexOfActiveFighter;
        public Fighter GetActiveFighter { get { if (Fighters[side].Count == 0)
                                                    GameOver(GetOppositeSide);
                                                return Fighters[side][indexOfActiveFighter]; } }
        public event Action<Fighter> FighterChanged;
        public event Action<Ability, List<Fighter>> PlayerChooseTarget;
        public event Action Death;
        public event Action<Side> GameOver;
        public Side side;
        
        public void EndOfTheTurn()
        {
            var currentLength = Fighters[side].Count;
            for (var i = 0; i < Fighters[side].Count; i++)
            { 
                Fighters[side][i].EndOfTheTurn();
                if (currentLength > Fighters[side].Count)
                {
                    currentLength = Fighters[side].Count;
                    i--;
                }
            }
            side = GetOppositeSide;
            indexOfActiveFighter = 0;
            LoopRun();
        }
        public void Run()
        {
            while (true)
                LoopRun();
        }
        public void LoopRun()
        {
            if (side == Side.AI)
            {
                AI.MakeDesicion(this);
                if (indexOfActiveFighter > Fighters[side].Count - 1)
                    EndOfTheTurn();
            }
            else
                FighterChanged?.Invoke(GetActiveFighter);
        }
        public void ChoosedTarget(Fighter sender, Ability ability, Fighter target)
        {
            if (sender.Mana >= ability.ManaCost)
            {
                sender.ActivateAbility(target, ability);
            }
            if (target.Health <= 0)
            {
                target.Side.Remove(target);
                Death();
                if (target.Side.Count == 0)
                    GameOver(GetOppositeSide);
            }
            if (indexOfActiveFighter == Fighters[side].Count - 1)
                EndOfTheTurn();
            else
            {
                indexOfActiveFighter++;
                LoopRun();
            }
        }
        public void ChooseTargetForAbility(Ability ability)
        {
            if (side == Side.Player)
            {
                var targetsList = ability.IsAttackingAbility ? Fighters[GetOppositeSide] : Fighters[side];
                PlayerChooseTarget(ability, targetsList);
            }
            else
                EndOfTheTurn();
        }
        public void Init()
        {
            var paths = Directory.GetFiles("data");
            foreach (var filePath in paths)
            {
                var reader = new StreamReader(filePath);
                var fighter = new Fighter(reader.ReadLine(),
                                        int.Parse(reader.ReadLine()), 
                                        int.Parse(reader.ReadLine()), 
                                        int.Parse(reader.ReadLine()), 
                                        new List<Ability>(),
                                        reader.ReadLine());
                if (reader.ReadLine() == "AI")
                {
                    fighter.Side = Fighters[Side.AI];
                    Fighters[Side.AI].Add(fighter);
                }
                else
                {
                    fighter.Side = Fighters[Side.Player];
                    Fighters[Side.Player].Add(fighter);
                }
                while (!reader.EndOfStream)
                    InitAbilities(reader, fighter);
                InitEffects();
                fighter.HealthChanged += (attackedFighter) =>
                {
                    if (attackedFighter.Health <= 0)
                    {
                        Fighters[side].Remove(attackedFighter);
                        Death();
                    }
                };
            }
        }
        private void InitAbilities(StreamReader reader, Fighter fighter)
        {
            var ability = new Ability(int.Parse(reader.ReadLine()),
                int.Parse(reader.ReadLine()),
                ParseTypeOfAbility(reader.ReadLine()),
                new List<Effect>(),
                reader.ReadLine(),
                reader.ReadLine());
            while (InitEffects(reader, ability));
            fighter.Abilities.Add(ability);                
        }
        private bool ParseTypeOfAbility(string type)
        {
            return type == "attack";
        }
        private bool InitEffects(StreamReader reader, Ability ability)
        {
            var name = reader.ReadLine();
            if (name == "!")
                return false;
            ability.appliedEffects.Add(new Effect(name, int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine())));
            return true;
        }
        private void InitEffects()
        {
            // Это звездец в пушистых тапочках
            Effect.EndOfTheTurnEffects.Add(Effect.EffectType.Poison);
            Effect.EndOfTheTurnEffects.Add(Effect.EffectType.Regeneration);
            Effect.ThenAttackedEffects.Add(Effect.EffectType.Strength);
            Effect.OnAttackEffects.Add(Effect.EffectType.Protection);
            Effect.OnAttackEffects.Add(Effect.EffectType.ShockShield);
            Effect.InstantEffects.Add(Effect.EffectType.Dispell);
            Effect.InstantEffects.Add(Effect.EffectType.MultiplyByEnemyEffects);
            Effect.InstantEffects.Add(Effect.EffectType.IncreaseIfPoison);
            Effect.InstantEffects.Add(Effect.EffectType.CopyEffects);
            Effect.InstantEffects.Add(Effect.EffectType.PlayMinecraftMusic);
        }
        public GameModel()
        {
            Fighters = new Dictionary<Side, List<Fighter>>();
            Fighters.Add(Side.AI, new List<Fighter>());
            Fighters.Add(Side.Player, new List<Fighter>());
            Init();
            Turn = Side.Player;
            indexOfActiveFighter = 0;
            side = Side.Player;
        }
        public Side GetOppositeSide
        {
            get
            {
                if (side == Side.AI)
                    return Side.Player;
                return Side.AI;
            }
        }
    }
}
