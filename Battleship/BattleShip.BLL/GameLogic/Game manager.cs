using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Ships;
using BattleShip.BLL.Responses;

namespace BattleShip.BLL.GameLogic
{
    public class Game_manager
    {

        string player1name;
        string player2name;
        Board p1board = new Board();
        Board p2board = new Board();
        int currentPlayer = 1;
        bool endGame = false;
        bool nextShip = false;
        bool nextTurn = false;
        bool validShip = false;
        bool validGuess = false;


        public void Playgame()
        {
            StartScreen();

            Console.WriteLine("Hello player 1, enter your name!");
            player1name = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Hello player 2, enter your name!");
            player2name = Console.ReadLine();
            Console.Clear();

            SetUp();
            do
            {
                ProcessTurns();
            } while (!endGame);
        }

        private void SetUp()
        {

            for (var i = 0; i < 5; i++)
            {
                do
                {
                    Coordinate coord = AskCoordinate();
                    ShipDirection direction = AskDirection();
                    ShipType type = AskShipType();
                    PlaceShipRequest req = new PlaceShipRequest();
                    req.Coordinate = coord;
                    req.Direction = direction;
                    req.ShipType = type;
                    ShipPlacement sp = p1board.PlaceShip(req);
                    if (sp == ShipPlacement.NotEnoughSpace)
                    {
                        Console.WriteLine("Not enough space to put a ship there! Try again!\n");
                        nextShip = false;
                    }
                    else if (sp == ShipPlacement.Overlap)
                    {
                        Console.WriteLine("That ship would overlap one you already placed! Try again!\n");
                        nextShip = false;
                    }
                    else // shipplacement.ok
                    {
                        nextShip = true;
                    }
                } while (!nextShip);

            }

            Console.Clear();
            Console.WriteLine("{0}'s turn!!", player2name);

            for (int i = 0; i < 5; i++)
            {
                do
                {
                    Coordinate coord = AskCoordinate();
                    ShipDirection direction = AskDirection();
                    ShipType type = AskShipType();
                    PlaceShipRequest req = new PlaceShipRequest();
                    req.Coordinate = coord;
                    req.Direction = direction;
                    req.ShipType = type;
                    ShipPlacement sp = p2board.PlaceShip(req);
                    if (sp == ShipPlacement.NotEnoughSpace)
                    {
                        Console.WriteLine("Not enough space to put a ship there! Try again!");
                        nextShip = false;
                    }
                    if (sp == ShipPlacement.Overlap)
                    {
                        Console.WriteLine("That ship would overlap one you already placed! Try again!");
                        nextShip = false;
                    }
                    else // shipplacement.ok
                    {
                        nextShip = true;
                    }
                } while (!nextShip);
            }

            Console.Clear();

        }

        private void ProcessTurns()
        {
            do
            {
                Coordinate guess = GetGuess();
                FireShotResponse response = TakeShot(guess);
                ReportStatus(response);
                if (response.ShotStatus == ShotStatus.Hit || response.ShotStatus == ShotStatus.HitAndSunk || response.ShotStatus == ShotStatus.Miss || response.ShotStatus == ShotStatus.Victory)
                {

                    if (currentPlayer == 1)
                    {
                        p2board.AddMark(guess, response);
                        p2board.Display();
                    }

                    else // current player2
                    {
                        p1board.AddMark(guess, response);
                        p1board.Display();
                    }
                }

                if (response.ShotStatus != ShotStatus.Duplicate && response.ShotStatus != ShotStatus.Invalid)
                    nextTurn = true;
                else
                    nextTurn = false;

                if (response.ShotStatus == ShotStatus.Victory)
                    endGame = true;

            } while (!nextTurn);

            currentPlayer = NextPlayer();
        }

        private bool CheckValid(string x)
        {
            string x_string = x.Substring(0, 1);
            string y_string = x.Substring(1);
            if (x_string != "A" && x_string != "B" && x_string != "C" && x_string != "D" && x_string != "E" && x_string != "F" && x_string != "G" && x_string != "H" && x_string != "I" && x_string != "J")
            {
                Console.WriteLine("Invalid x coordinate, please try again\n");
                return false;
            }
            else if (y_string != "1" && y_string != "2" && y_string != "3" && y_string != "4" && y_string != "5" && y_string != "6" && y_string != "7" && y_string != "8" && y_string != "9" && y_string != "10")
            {
                Console.WriteLine("Invalid y coordinate, please try again\n");
                return false;
            }
            else
                return true;
        }

        private Coordinate AskCoordinate()
        {
            string answer;
            bool valid = false;

            do
            {
                Console.WriteLine("Enter the desired starting coordinate of your ship (A through J, 1 to 10)");
                answer = Console.ReadLine();
                valid = CheckValid(answer);
            } while (!valid);

            string x_string = answer.Substring(0, 1);
            string x_stringnum = Convert(x_string);
            string y_string = answer.Substring(1);
            int x_int = int.Parse(x_stringnum);
            int y_int = int.Parse(y_string);
            Coordinate coordinate = new Coordinate(x_int, y_int);
            Console.WriteLine();
            return coordinate;
        }

        private ShipType AskShipType()
        {
            string type;
            do
            {
                Console.WriteLine("Enter the type of ship you're currently placing! \n(Cruiser, Battleship, Carrier, Submarine, or Destroyer)");
                type = Console.ReadLine();
                if (type == "Destroyer" || type == "Carrier" || type == "Cruiser" || type == "Battleship" || type == "Submarine")
                    validShip = true;
                else
                {
                    Console.WriteLine("That is not a valid ship type, try again.\n");
                    validShip = false;
                }
            } while (!validShip);

            Console.WriteLine();

            if (type == "Destroyer")
                return ShipType.Destroyer;
            else if (type == "Battleship")
                return ShipType.Battleship;
            else if (type == "Submarine")
                return ShipType.Submarine;
            else if (type == "Carrier")
                return ShipType.Carrier;
            else // (type == "Cruiser")
                return ShipType.Cruiser;
        }

        private Coordinate GetGuess()
        {
            string playerGuess;

            do
            {
                Console.WriteLine("Player {0}\nEnter the board position where you'd like to make your guess", currentPlayer);
                playerGuess = Console.ReadLine();
                validGuess = CheckValid(playerGuess);
            } while (!validGuess);

            string x_string = playerGuess.Substring(0, 1);
            string y_string = playerGuess.Substring(1);
            string intGuess = Convert(x_string);
            int x_int = int.Parse(intGuess);
            int y_int = int.Parse(y_string);
            Coordinate coordinate = new Coordinate(x_int, y_int);
            return coordinate;
        }

        private ShipDirection AskDirection()
        {
            bool validDirection = false;
            ShipDirection dir = new ShipDirection();
            do
            {
                Console.WriteLine("Which duration should the ship go when placed?\n(Up, Down, Left, or Right)");
                string direction = Console.ReadLine();
                if (direction == "Up")
                {
                    validDirection = true;
                    dir = ShipDirection.Up;
                }
                else if (direction == "Down")
                {
                    validDirection = true;
                    dir = ShipDirection.Down;
                }
                else if (direction == "Right")
                {
                    validDirection = true;
                    dir = ShipDirection.Right;
                }
                else if (direction == "Left")
                {
                    validDirection = true;
                    dir = ShipDirection.Left;
                }
                else
                {
                    Console.WriteLine("Not a valid direction, try again.\n");
                }
            } while (!validDirection);
            Console.WriteLine();
            return dir;
        }

        private string Convert(string str)
        {
            switch (str)
            {
                case "A":
                    return "1";
                case "B":
                    return "2";
                case "C":
                    return "3";
                case "D":
                    return "4";
                case "E":
                    return "5";
                case "F":
                    return "6";
                case "G":
                    return "7";
                case "H":
                    return "8";
                case "I":
                    return "9";
                case "J":
                    return "10";
                default:
                    return "99";
            }


        }

        private FireShotResponse TakeShot(Coordinate x)
        {
            if (currentPlayer == 1)
            {
                var fsr = p2board.FireShot(x);
                return fsr;
            }
            else
            {
                var fsr = p1board.FireShot(x);
                return fsr;
            }
        }

        private int NextPlayer()
        {
            if (currentPlayer == 1)
                return 2;
            else return 1;
        }

        private void ReportStatus(FireShotResponse response)
        {
            if (response.ShotStatus == ShotStatus.Duplicate)
                Console.WriteLine("\nYou already tried that position, try again!");
            if (response.ShotStatus == ShotStatus.Hit)
                Console.WriteLine("It's a hit!");
            if (response.ShotStatus == ShotStatus.HitAndSunk)
                Console.WriteLine("Hit and sunk!!");
            if (response.ShotStatus == ShotStatus.Invalid)
                Console.WriteLine("\nNot a valid coordinate, try again!");
            if (response.ShotStatus == ShotStatus.Miss)
                Console.WriteLine("You missed.");
            if (response.ShotStatus == ShotStatus.Victory)
            {
                Console.WriteLine("You win!!!");
                endGame = true;
            }
        }

        private void StartScreen()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("\n- - - - - - - - - - - - - - - - - - -");
            Console.ResetColor();

            Console.WriteLine("  !!  WELCOME TO BATTLESHIP  !!");
            Console.WriteLine("\t\t  created by: Danny H");


            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("- - - - - - - - - - - - - - - - - - -");
            Console.ResetColor();

            Console.WriteLine("\nPress enter to begin...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
