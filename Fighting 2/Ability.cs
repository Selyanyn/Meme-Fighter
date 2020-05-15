using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fighting_2
{
    public class Ability
    {
        public string Name;
        public bool IsAttackingAbility;
        public int Damage;
        public List<Effect> appliedEffects;
        public string Description;
        public int ManaCost;
        public Ability(int damage, int manaCost, bool isAttackingAbility = true, List<Effect> effects = null, string name = "", string description = "")
        {
            Damage = damage;
            ManaCost = manaCost;
            IsAttackingAbility = isAttackingAbility;
            appliedEffects = effects;
            Name = name;
            Description = description;
        }
        public void ApplyAbility(Fighter sender, Fighter target)
        {
            var resultDamage = Damage;
            foreach (var e in appliedEffects)
            {
                if (!Effect.InstantEffects.Contains(e.Name))
                    target.ApplyEffect(e.Copy());
                else
                    resultDamage = e.ActivateInstantEffects(sender, resultDamage, target);
                    
            }
            target.Damage(resultDamage, sender);
        }
        public override string ToString()
        {
            return Description.Substring(0, Math.Min(Description.Length, 8));
        }
    }
}
