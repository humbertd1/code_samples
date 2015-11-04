using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShip.BLL.GameLogic;

namespace BattleShip.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            bool keepPlaying = false;
            Program p1 = new Program();

            do
            {
                Console.Clear(); // clears old stuff if they choose to play again
                Game_manager mgr = new Game_manager();
                mgr.Playgame();
                keepPlaying = p1.KeepPlaying();
            } while (keepPlaying);
        }

        private bool KeepPlaying()
        {
            Console.WriteLine("Would you like to play again? (Y/N)");
            string input = Console.ReadLine();
            if (input == "Y")
                return true;
            else
                return false;
        }

    }

}
    

