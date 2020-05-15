using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fighting_2
{
    public static class AI
    {
        // best AI in the world IMHO
        public static void MakeDesicion(GameModel game)
        {
            var fighter = game.GetActiveFighter;
            var random = new Random();
            game.ChoosedTarget(fighter, 
                fighter.Abilities[random.Next(0, fighter.Abilities.Count)], 
                game.Fighters[game.GetOppositeSide][random.Next(0, game.Fighters[game.GetOppositeSide].Count)]);
        }
    }
}
