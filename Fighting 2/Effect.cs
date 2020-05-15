using System;
using System.Collections.Generic;
using System.Media;
using Microsoft.VisualBasic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fighting_2
{
    public class Effect
    {
        public static HashSet<EffectType> EndOfTheTurnEffects = new HashSet<EffectType>();
        public static HashSet<EffectType> OnAttackEffects = new HashSet<EffectType>();
        public static HashSet<EffectType> ThenAttackedEffects = new HashSet<EffectType>();
        public static HashSet<EffectType> InstantEffects = new HashSet<EffectType>();

        public enum EffectType
        {
            Poison,
            Regeneration,
            Strength,
            Protection,
            ShockShield,
            Dispell,
            MultiplyByEnemyEffects,
            CopyEffects,
            IncreaseIfPoison,
            PlayMinecraftMusic,
        }
        public readonly EffectType Name;
        public int Duration;
        public readonly int Value;
        public Effect(string name, int value, int duration = 1)
        {
            Name = (EffectType)Enum.Parse(typeof(EffectType), name);
            Value = value;
            Duration = duration;
        }
        public Effect(EffectType type, int value, int duration = 1)
        {
            Name = type;
            Value = value;
            Duration = duration;
        }
        public Effect Copy()
        {
            return new Effect(Name, Value, Duration);
        }
        public override string ToString()
        {
            return Name.ToString();
        }
        public void ActivateEndOfTheTurnEffect(Fighter host)
        {
            switch (Name)
            {
                case EffectType.Poison:
                    host.Damage(Value);
                    break;
                case EffectType.Regeneration:
                    host.Damage(-Value);
                    break;
            }
        }
        public int ActivateAttackEffect(Fighter offender, int damage, Fighter defender)
        {
            switch (Name)
            {
                case EffectType.Strength:
                    return damage * (1 + Value / 100);
            }
            return damage;
        }
        public int ActivateInstantEffects(Fighter offender, int damage, Fighter defender)
        {
            switch (Name)
            {
                case EffectType.Dispell:
                    defender.Effects = new Dictionary<EffectType, Effect>();
                    break;
                case EffectType.CopyEffects:
                    foreach (var e in defender.Effects)
                        offender.ApplyEffect(e.Value.Copy());
                    break;
                case EffectType.MultiplyByEnemyEffects:
                    return (int)(damage * (1 + (double)defender.Effects.Count * Value / 100));
                case EffectType.IncreaseIfPoison:
                    if (defender.Effects.ContainsKey(EffectType.Poison))
                        return new Effect(EffectType.Strength, Value).ActivateAttackEffect(offender, damage, defender);
                    break;
                case EffectType.PlayMinecraftMusic:
                    var musicPlayer = new SoundPlayer("music/minecraft.wav");
                    System.Threading.Thread.Sleep(20);
                    musicPlayer.Play();
                    break;
            }
            return damage;
        }
        public int ActivateThenAttackedEffects(Fighter offender, int damage, Fighter defender)
        {
            switch (Name)
            {
                case EffectType.Protection:
                    return damage * (1 - Value / 100);
                case EffectType.ShockShield:
                    offender.Damage(Value);
                    break;
            }
            return damage;
        }
    }
}
