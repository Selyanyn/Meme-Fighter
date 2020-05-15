using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Fighting_2
{
    [TestFixture]
    public class GameModelShould
    {
        [Test]
        public void TestOnAddingFighter()
        {
            Directory.SetCurrentDirectory(@"C:\Users\lenovo\source\repos\Fighting 2\Fighting 2\bin\Debug");
            var game = new GameModel();
            var result = game.Fighters[game.side][0];
            Assert.AreEqual(100, result.BaseHealth);
            Assert.AreEqual(10, result.BaseMana);
            Assert.AreEqual(14, result.Abilities[0].Damage);
        }
        [Test]
        public void TestOfDamagingFighter()
        {
            Directory.SetCurrentDirectory(@"C:\Users\lenovo\source\repos\Fighting 2\Fighting 2\bin\Debug");
            var game = new GameModel();
            var result = game.Fighters[game.side][0];
            var random = new Random();
            var damage = random.Next(0, 40);
            result.Damage(damage);
            Assert.AreEqual(100 - damage, result.Health);
            Assert.AreEqual(10, result.BaseMana);
            Assert.AreEqual(14, result.Abilities[0].Damage);
        }
        [Test]
        public void TestPoisonEffect()
        {
            Directory.SetCurrentDirectory(@"C:\Users\lenovo\source\repos\Fighting 2\Fighting 2\bin\Debug");
            var game = new GameModel();
            var testFighter = game.Fighters[game.side][0];
            testFighter.Effects.Add(Effect.EffectType.Poison, new Effect(Effect.EffectType.Poison, 10, 1));
            testFighter.EndOfTheTurn();
            Assert.AreEqual(90, testFighter.Health);
            Assert.AreEqual(0, testFighter.Effects.Count);
        }
        [Test]
        public void TestRegenerationEffect()
        {
            Directory.SetCurrentDirectory(@"C:\Users\lenovo\source\repos\Fighting 2\Fighting 2\bin\Debug");
            var game = new GameModel();
            var testFighter = game.Fighters[game.side][0];
            testFighter.Damage(20);
            var regenEffect = new Effect(Effect.EffectType.Regeneration, 12, 2);
            testFighter.Effects.Add(Effect.EffectType.Regeneration, regenEffect);
            testFighter.EndOfTheTurn();
            Assert.AreEqual(92, testFighter.Health);
            Assert.AreEqual(1, testFighter.Effects.Count);
            Assert.IsTrue(testFighter.Effects.ContainsKey(Effect.EffectType.Regeneration));
            regenEffect.Duration--;
            Assert.AreEqual(testFighter.Effects[Effect.EffectType.Regeneration], regenEffect);
        }
        [Test]
        public void TestAI()
        {
            Directory.SetCurrentDirectory(@"C:\Users\lenovo\source\repos\Fighting 2\Fighting 2\bin\Debug");
            var game = new GameModel();
            var AIAbility = new List<Ability> { new Ability(10, 0, true, new List<Effect>()), new Ability(20, 0, true, new List<Effect>()) };
            game.Fighters[GameModel.Side.AI] = new List<Fighter> { new Fighter("AIFighter", 20, 0, 0, AIAbility) };
            game.Fighters[GameModel.Side.Player] = new List<Fighter> { new Fighter("Target", 100, 0, 0, new List<Ability>()) };
            var target = game.Fighters[GameModel.Side.Player][0];
            game.side = GameModel.Side.AI;
            game.indexOfActiveFighter = 0;
            AI.MakeDesicion(game); // AI applies to an active player - fighter right now
            Assert.IsTrue(target.Health == 80 || target.Health == 90);
        }
    }
}
