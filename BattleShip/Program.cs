using System.Globalization;
using System.Runtime.InteropServices;

namespace BattleShip
{
	internal class Program
	{
		static void Main(string[] args)
		{
			BasePlayer player = new BasePlayer();
			BasePlayer ai = new BasePlayer();
			Grid aiGrid = ai.GetGrid();
			Grid playerGrid = player.GetGrid();
			int shots = 0;

            string titleScreen =
            (
                "  ____        _   _   _           _     _       \n" +
                " | __ )  __ _| |_| |_| | ___  ___| |__ (_)_ __  \n"+
                " |  _ \\ / _` | __| __| |/ _ \\/ __| '_ \\| | '_ \\\n " +
                "| |_) | (_| | |_| |_| |  __/\\__ \\ | | | | |_) |\n" +
                " |____/ \\__,_|\\__|\\__|_|\\___||___/_| |_|_| .__/ \n"+
                "                                         |_|    \r\n"
            );


            Console.WriteLine(titleScreen);
            Console.WriteLine("Welcome to Battleship! Press Enter to start.");
			Console.ReadLine();

			while (true)
			{
				Console.Clear();
				Console.WriteLine("Ai's Board (Shots Fired):");
				aiGrid.DisplayBoard(false);

				Console.WriteLine("\nYour Board:");
				playerGrid.DisplayBoard(false);
				Console.WriteLine("Enter to fire random shots");
				player.Attack(aiGrid);

				shots++;
				if (aiGrid.CheckWin())
				{
					Console.WriteLine("You win in " + shots + " shots!");
				}
				ai.Attack(playerGrid);
				if (playerGrid.CheckWin())
				{
					Console.WriteLine("Ai Wins");
					break;
				}
			}
		}
	}
	class BasePlayer
	{
		public static string[] shipNames = { "Battleship", "Cruiser", "Destroyer", "Submarine", "Carrier" };
		public static int[] shipLengths = { 4, 3, 3, 2, 5 };
		private Grid grid;
		private Random rand;

		public BasePlayer()
		{
			grid = new Grid();
			rand = new Random();
			PlaceShips();
		}
		public void PlaceShips()
		{
			Random rand = new Random();

			for (int i = 0; i < shipNames.Length; i++)
			{
				string direction = "H";
				if (rand.Next(0, 2) == 0)
				{
					direction = "V";
				}
				if (grid.PlaceShip(new Ship("battleship", 4), rand.Next(grid.BoardLength()), rand.Next(grid.BoardHeight()), direction))
				{
					i--;
				}
			}
		}

		public bool Attack(Grid enemyGrid)
		{
			int x, y;
			do
			{
				x = rand.Next(10);
				y = rand.Next(10);
			}
			while (enemyGrid.MakeGuess(x, y));
			return true;
		}
		public Grid GetGrid()
		{
			return grid;
		}
	}
	class Ship
	{
		public string Name;
		public int Length;
		public List<(int, int)> Coordinates;
		public int Hits;

		public Ship(string name, int length)
		{
			Name = name;
			Length = length;
			Coordinates = new List<(int, int)>();
			Hits = 0;
		}

		public bool IsSunk()
		{
			return Hits >= Length;
		}
	}
	class Grid
	{
		private char[,] Board;
		public List<Ship> Ships;
		public (int, int) BoardDimensions = (10, 10);
		public Grid()
		{
			Board = new char[BoardDimensions.Item1, BoardDimensions.Item2];
			Ships = new List<Ship>();
			for (int i = 0; i < BoardDimensions.Item1; i++)
			{
				for (int j = 0; j < BoardDimensions.Item2; j++)
				{
					Board[i, j] = '~';
				}
			}
		}
		public void DisplayBoard(bool hideShips)
		{
			string startLetter = "ABCDEFGHIJ";
			for (int i = 0; i < BoardDimensions.Item1; i++)
			{
				if (i == 0)
				{
					Console.WriteLine("  0 1 2 3 4 5 6 7 8 9");
				}
				for (int j = 0; j < BoardDimensions.Item2; j++)
				{
					if (j == 0)
					{
						Console.Write(startLetter[i] + " ");
					}
					char displayChar = Board[i, j];
					if (hideShips && displayChar == 'S')
					{
						displayChar = '~';
					}
					Console.WriteLine(displayChar + " ");
				}
				Console.ResetColor();
				Console.WriteLine();
			}
		}

		public int BoardLength()
		{
			return BoardDimensions.Item1;
		}
		public int BoardHeight()
		{
			return BoardDimensions.Item2;
		}
		public bool PlaceShip(Ship ship, int startX, int startY, string direction)
		{
			List<(int, int)> tempCoordinates = new List<(int, int)>();
			for (int i = 0; i < ship.Length; i++)
			{
				int x = startX;
				int y = startY;

				if (direction == "V")
				{
					x += i;
				}
				if (direction == "H")
				{
					y += i;
				}

				if (x >= 10 || y >= 10 || Board[x, y] != '~')
				{
					return false;
				}
				tempCoordinates.Add((x, y));
			}
			foreach (var coord in tempCoordinates)
			{
				Board[coord.Item1, coord.Item2] = 'S';
			}
			ship.Coordinates.AddRange(tempCoordinates);
			Ships.Add(ship);
			return true;
		}
		public bool MakeGuess(int x, int y)
		{
			if (Board[x, y] == 'S')
			{
				Board[x, y] = 'X';
				foreach (var ship in Ships)
				{
					if (ship.Coordinates.Contains((x, y)))
					{
						ship.Hits++;
						return true;
					}
				}
			}
			Board[x, y] = 'O';
			return false;
		}
		public bool CheckWin()
		{
			foreach (var ship in Ships)
			{
				if (!ship.IsSunk()) return false;
			}
			return true;
		}
	}
}