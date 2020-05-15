using System;
using System.Collections.Generic;

namespace Fighting_2
{
    public class Fighter
    {
        public readonly string Name;
        public readonly string Description;
        protected int baseHealth;
        public List<Fighter> Side;
        public int BaseHealth { get { return baseHealth; } }
        protected int health;
        public int Health { get { return health; } }
        protected int baseMana;
        public int BaseMana { get { return baseMana; } }
        protected int mana;
        protected int manaRegeneration;
        public int Mana { get { return mana; } }
        public List<Ability> Abilities;
        public Dictionary<Effect.EffectType, Effect> Effects;
        public void Damage(int damage, Fighter sender = null)
        {
            foreach (var e in Effects)
                if (Effect.ThenAttackedEffects.Contains(e.Key))
                    e.Value.ActivateAttackEffect(sender, damage, this);
            var newHealth = health - damage;
            if (newHealth > BaseHealth)
                health = BaseHealth;
            else
            health = newHealth;
            HealthChanged?.Invoke(this);
        }
        public void ApplyEffect(Effect effect)
        {
            if (Effects.ContainsKey(effect.Name))
                Effects.Remove(effect.Name);
            Effects.Add(effect.Name, effect);
        }
        public void ActivateAbility(Fighter target, Ability ability)
        {
            if (!Abilities.Contains(ability))
                throw new ArgumentException("Wrong ability!");
            if (ability.ManaCost > mana)
                return;
            mana -= ability.ManaCost;
            if (ability.ManaCost != 0)
                ManaChanged?.Invoke(this);
            ability.ApplyAbility(this, target);
        }
        public void EndOfTheTurn()
        {
            var effectsToRemove = new List<Effect.EffectType>();
            foreach (var e in Effects)
            {
                if (Effect.EndOfTheTurnEffects.Contains(e.Key))
                    e.Value.ActivateEndOfTheTurnEffect(this);
                e.Value.Duration--;
                if (e.Value.Duration == 0)
                    effectsToRemove.Add(e.Key);
            }
            foreach (var e in effectsToRemove)
                Effects.Remove(e);
            mana += manaRegeneration;
            mana = Math.Min(baseMana, mana);
            ManaChanged?.Invoke(this);
        }
        public Fighter(string name, int baseHealth, int baseMana, int manaRegeneration, List<Ability> abilities,
                        string description = null)
        {
            Name = name;
            this.baseHealth = baseHealth;
            this.baseMana = baseMana;
            this.manaRegeneration = manaRegeneration;
            Abilities = abilities;
            health = baseHealth;
            mana = baseMana;
            Effects = new Dictionary<Effect.EffectType, Effect>();
            Description = description;
        }
        public override string ToString()
        {
            return Name;
        }
        public event Action<Fighter> HealthChanged;
        public event Action<Fighter> ManaChanged;
    }
}
