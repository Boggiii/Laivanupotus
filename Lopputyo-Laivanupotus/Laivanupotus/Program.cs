using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Laivanupotus
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetBufferSize(200, 200);

            string menuDifficultyHeader = "Choose your difficulty:";
            List<string> Difficulties = new List<string>()
            {
                "Beginner",
                "Medium",
                "Expert"
            };

            string menuSingleOrMultiHeader = "How many players:";
            List<string> SingleOrMulti = new List<string>()
            {
                "1 Player",
                "2 Players"
            };

            string menuShipCount = "How many ships: ";
            List<string> ShipCountOptions = new List<string>()
            {
                "6",
                "12",
                "18"
            };

            int winMenuIndex = 0;
            string Winner = "";
            List<string> menuWinScreen = new List<string>()
            {
                "Show statistics",
                "Exit"
            };

            int Difficulty = 0; // Vaikeusaste
            int SPMP = 0; // Singleplayer[0] tai Multiplayer[1]
            int shipCount = 0; // Kuinka monta laivaa (1 tile)

            int[,] mainBoard; // Player 1 lauta
            int[,] altBoard; // CPU / Player 2 lauta

            Statistics();

            Difficulty = newMenu(Difficulties, menuDifficultyHeader);
            SPMP = newMenu(SingleOrMulti, menuSingleOrMultiHeader);
            shipCount = (newMenu(ShipCountOptions, menuShipCount) + 1) * 6;

            int RowsColumns = 6 + Difficulty * 2;

            mainBoard = newBoard(RowsColumns);
            altBoard = newBoard(RowsColumns);

            bool p1Win = false;
            shipLocation(mainBoard, RowsColumns, shipCount, "Player 1");
            if (SPMP == 1) // Kaksinpeli
            {
                bool p2Win = false;
                shipLocation(altBoard, RowsColumns, shipCount, "Player 2");
                while (!p1Win && !p2Win)
                {
                    altBoard = shootAtBoard(altBoard, RowsColumns, "Player 1");
                    drawBoardMP(altBoard, RowsColumns, "Player 1");
                    p1Win = WinCondition(altBoard, RowsColumns);
                    mainBoard = shootAtBoard(mainBoard, RowsColumns, "Player 2");
                    drawBoardMP(mainBoard, RowsColumns, "Player 2");
                    p2Win = WinCondition(mainBoard, RowsColumns);
                }
                if (p1Win)
                {
                    Winner = "Player 1";
                    addScore("p1Win.txt");
                }
                else if (p2Win)
                {
                    Winner = "Player 2";
                    addScore("p2Win.txt");
                }
            }
            else // Tietokonetta vastaan pelataan
            {
                bool cpuWin = false;
                CPUChooseShipLocation(altBoard, shipCount, RowsColumns);
                while (!p1Win && !cpuWin)
                {
                    altBoard = shootAtBoard(altBoard, RowsColumns, "Player 1");
                    drawBoardMP(altBoard, RowsColumns, "CPU");
                    p1Win = WinCondition(altBoard, RowsColumns);
                    mainBoard = CPUShoot(mainBoard, RowsColumns);
                    drawBoard(mainBoard, RowsColumns, "Player 1");
                    cpuWin = WinCondition(mainBoard, RowsColumns);
                }
                if (p1Win)
                {
                    Winner = "Player 1";
                    addScore("p1Win.txt");
                }
                else if (cpuWin)
                {
                    Winner = "CPU";
                    addScore("cpuWin.txt");
                }
            }

            string menuWinner = Winner + " won!";
            winMenuIndex = newMenu(menuWinScreen, menuWinner);

            switch(winMenuIndex)
            {
                case 0:
                    Statistics();
                    break;
                case 1:
                    Environment.Exit(0);
                    break;
            }
        }

        static int newMenu(List<string> MenuItems, string Header) // Menuja varten
        {
            int index = default(int);
            bool notChosen = true;

            Console.CursorVisible = false;

            while (notChosen)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Header);
                for (int i = 0; i < Header.Length; i++)
                {
                    Console.Write("-");
                }
                Console.WriteLine();

                for (int i = 0; i < MenuItems.Count; i++)
                {
                    if (i == index)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    }
                    Console.WriteLine("  > " + MenuItems[i]);
                }

                ConsoleKeyInfo KeyPress = Console.ReadKey();
                if (KeyPress.Key == ConsoleKey.DownArrow)
                {
                    if (index < MenuItems.Count - 1)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                else if (KeyPress.Key == ConsoleKey.UpArrow)
                {
                    if (index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = MenuItems.Count - 1;
                    }
                }
                else if (KeyPress.Key == ConsoleKey.Enter)
                {
                    notChosen = false;
                }

                Console.ResetColor();
                Console.Clear();
            }

            return index;
        }

        static int[,] newBoard(int RowsColumns)
        {
            int[,] arrayBoard = new int[RowsColumns, RowsColumns];
            return arrayBoard;
        }

        static void drawBoardMP(int[,] Board, int RowsColumns, string playerName)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Press ANY KEY To Continue!!! (" + playerName + ")\n");
            Console.ResetColor();
            for (int a = 0; a < RowsColumns; a++)
            {
                Console.ResetColor();
                Console.Write(Environment.NewLine + "   "); // Musta sivurivi
                for (int b = 0; b < RowsColumns; b++) // Taulukon piirtäminen:
                {
                    if (Board[b, a] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(" ~ ");
                        Console.ResetColor();
                    }
                    else if (Board[b, a] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(" O ");
                        Console.ResetColor();
                    }
                    else if (Board[b, a] == 2)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" X ");
                        Console.ResetColor();
                    }
                    else if (Board[b, a] == 3)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(" ~ ");
                        Console.ResetColor();
                    }
                    Console.Write("  ");
                }
                Console.WriteLine();
            }
            Console.ReadKey();
            Console.Clear();
        }

        static void drawBoard(int[,] Board, int RowsColumns, string playerName)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Press ANY KEY To Continue!!! (" + playerName + ")\n");
            Console.ResetColor();
            for (int a = 0; a < RowsColumns; a++)
            {
                Console.ResetColor();
                Console.Write(Environment.NewLine + "   "); // Musta sivurivi
                for (int b = 0; b < RowsColumns; b++) // Taulukon piirtäminen:
                {
                    if (Board[b, a] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(" ~ ");
                        Console.ResetColor();
                    }
                    else if (Board[b, a] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(" O ");
                        Console.ResetColor();
                    }
                    else if (Board[b, a] == 2)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" X ");
                        Console.ResetColor();
                    }
                    else if (Board[b, a] == 3)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(" ~ ");
                        Console.ResetColor();
                    }
                    Console.Write("  ");
                }
                Console.WriteLine();
            }
            Console.ReadKey();
            Console.Clear();
        }

        static int[,] shootAtBoard(int[,] Board, int RowsColumns, string playerName)
        {
            /* 0. Ei ammuttu, ei laivaa
             * 1. Ammuttu, ei laivaa
             * 2. Amuttu, laiva
             * 3. Ei ammuttu, laiva
             */
            int x = 0;
            int y = 0;
            bool viable = false;

            while (!viable)
            {
                viable = true;
                if (Board[x, y] == 1 || Board[x, y] == 2)
                {
                    x++;
                    if (x >= RowsColumns)
                    {
                        x = 0;
                        y++;
                        viable = false;
                    }
                    else if (Board[x, y] == 1 || Board[x, y] == 2)
                    {
                        viable = false;
                    }
                }
            }

            bool notChosen = true;

            while (notChosen)
            {
                Console.WriteLine("Shoot at enemy (" + playerName + ")\n");
                for (int a = 0; a < RowsColumns; a++)
                {
                    Console.ResetColor();
                    Console.Write(Environment.NewLine + "   "); // Musta sivurivi
                    for (int b = 0; b < RowsColumns; b++) // Taulukon piirtäminen:
                    {
                        if (b == x && a == y)
                        {

                            if (Board[b, a] == 0)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" ~ ");
                                Console.ResetColor();
                            }
                            else if (Board[b, a] == 1)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" O ");
                                Console.ResetColor();
                            }
                            else if (Board[b, a] == 2)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" X ");
                                Console.ResetColor();
                            }
                            else if (Board[b, a] == 3)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" ~ ");
                                Console.ResetColor();
                            }
                        }
                        else if (Board[b, a] == 0)
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.Write(" ~ ");
                            Console.ResetColor();
                        }
                        else if (Board[b, a] == 1)
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(" O ");
                            Console.ResetColor();
                        }
                        else if (Board[b, a] == 2)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" X ");
                            Console.ResetColor();
                        }
                        else if (Board[b, a] == 3)
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.Write(" ~ ");
                            Console.ResetColor();
                        }
                        Console.Write("  ");
                    }
                    Console.WriteLine();
                }

                ConsoleKeyInfo KeyPress = Console.ReadKey();
                if (KeyPress.Key == ConsoleKey.DownArrow)
                {
                    if (RowsColumns - 1 > y)
                    {
                        y++;
                    }
                }
                else if (KeyPress.Key == ConsoleKey.UpArrow)
                {
                    if (0 < y)
                    {
                        y--;
                    }
                }
                else if (KeyPress.Key == ConsoleKey.LeftArrow)
                {
                    if (0 < x)
                    {
                        x--;
                    }
                }
                else if (KeyPress.Key == ConsoleKey.RightArrow)
                {
                    if (RowsColumns - 1 > x)
                    {
                        x++;
                    }
                }
                else if (KeyPress.Key == ConsoleKey.Enter)
                {
                    notChosen = false;
                    viable = false;
                    if (Board[x, y] == 0)
                    {
                        Board[x, y] = 1;
                    }
                    else if (Board[x, y] == 3)
                    {
                        Board[x, y] = 2;
                    }
                    else
                    {
                        notChosen = true;
                    }
                }
                Console.Clear();
            }
            return Board;
        }

        static int[,] shipLocation(int[,] Board, int RowsColumns, int shipCount, string playerName)
        {
            /* 0. Ei ammuttu, ei laivaa
             * 1. Ammuttu, ei laivaa
             * 2. Amuttu, laiva
             * 3. Ei ammuttu, laiva
             */

            for (int i = 0; i < shipCount; i++)
            {
                int x = 0;
                int y = 0;
                bool notChosen = true;


                while (notChosen)
                {
                    Console.WriteLine("Choose ship locations (" + playerName + ")\n");
                    for (int a = 0; a < RowsColumns; a++)
                    {
                        Console.ResetColor();
                        Console.Write(Environment.NewLine + "   "); // Musta sivurivi
                        for (int b = 0; b < RowsColumns; b++) // Taulukon piirtäminen:
                        {
                            if (b == x && a == y)
                            {

                                if (Board[b, a] == 0)
                                {
                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(" ~ ");
                                    Console.ResetColor();
                                }
                                else if (Board[b, a] == 1)
                                {
                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(" O ");
                                    Console.ResetColor();
                                }
                                else if (Board[b, a] == 2)
                                {
                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(" X ");
                                    Console.ResetColor();
                                }
                                else if (Board[b, a] == 3)
                                {
                                    Console.BackgroundColor = ConsoleColor.DarkRed;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(" H ");
                                    Console.ResetColor();
                                }
                            }
                            else if (Board[b, a] == 0)
                            {
                                Console.BackgroundColor = ConsoleColor.Blue;
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                Console.Write(" ~ ");
                                Console.ResetColor();
                            }
                            else if (Board[b, a] == 1)
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(" O ");
                                Console.ResetColor();
                            }
                            else if (Board[b, a] == 2)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkGreen;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(" X ");
                                Console.ResetColor();
                            }
                            else if (Board[b, a] == 3)
                            {
                                Console.BackgroundColor = ConsoleColor.Gray;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(" H ");
                                Console.ResetColor();
                            }
                            Console.Write("  ");
                        }
                        Console.WriteLine();
                    }
                    ConsoleKeyInfo KeyPress = Console.ReadKey();
                    if (KeyPress.Key == ConsoleKey.DownArrow)
                    {
                        if (RowsColumns - 1 > y)
                        {
                            y++;
                        }
                    }
                    else if (KeyPress.Key == ConsoleKey.UpArrow)
                    {
                        if (0 < y)
                        {
                            y--;
                        }
                    }
                    else if (KeyPress.Key == ConsoleKey.LeftArrow)
                    {
                        if (0 < x)
                        {
                            x--;
                        }
                    }
                    else if (KeyPress.Key == ConsoleKey.RightArrow)
                    {
                        if (RowsColumns - 1 > x)
                        {
                            x++;
                        }
                    }
                    else if (KeyPress.Key == ConsoleKey.Enter)
                    {
                        notChosen = false;
                        if (Board[x, y] == 0)
                        {
                            Board[x, y] = 3;
                        }
                        else if (Board[x, y] == 3)
                        {
                            notChosen = true;
                        }
                    }
                    Console.Clear();
                }
            }
            return Board;
        }

        static int[,] CPUChooseShipLocation(int[,] Board, int shipCount, int RowsColumns)
        {
            Random rng = new Random();
            int[,] occShipSpots = new int[RowsColumns, RowsColumns];
            int x, y;

            bool notUsed = true;

            for (int i = 0; i < shipCount; i++)
            {
                notUsed = true;
                while (notUsed)
                {
                    x = rng.Next(0, RowsColumns);
                    y = rng.Next(0, RowsColumns);
                    if (occShipSpots[x, y] == 1)
                    {
                        notUsed = true;
                    }
                    else
                    {
                        notUsed = false;
                        occShipSpots[x, y] = 1;
                        Board[x, y] = 3;
                    }
                }
            }
            return Board;
        }

        static int[,] CPUShoot(int[,] Board, int RowsColumns)
        {
            Random rng = new Random();
            int x, y;

            bool notUsed = true;

            notUsed = true;
            while (notUsed)
            {
                x = rng.Next(0, RowsColumns);
                y = rng.Next(0, RowsColumns);

                if (Board[x, y] == 1 || Board[x, y] == 2)
                {
                    notUsed = true;
                }
                else
                {
                    notUsed = false;
                    if (Board[x, y] == 0)
                    {
                        Board[x, y] = 1;
                    }
                    else if (Board[x, y] == 3)
                    {
                        Board[x, y] = 2;
                    }
                }
            }
            return Board;
        }

        static bool WinCondition(int[,] Board, int RowsColumns)
        {
            bool Condition = true;
            for (int y = 0; y < RowsColumns; y++)
            {
                for (int x = 0; x < RowsColumns; x++)
                {
                    if (Board[x, y] == 3)
                    {
                        Condition = false;
                    }
                }
            }
            return Condition;
        }

        static void addScore(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "Laivanupotus");
            Directory.CreateDirectory(path);
            try
            {
                int wins = int.Parse(File.ReadAllText(path + "\\" + fileName));
                wins++;
                File.WriteAllText(path + "\\" + fileName, wins.ToString());
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine(ex);

                File.WriteAllText(path + "\\" + fileName, "0");
            }
        }

        static void Statistics()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "Laivanupotus");
            try
            {
                int p1Win = int.Parse(File.ReadAllText(path + "\\p1Win.txt"));
                int p2Win = int.Parse(File.ReadAllText(path + "\\p2Win.txt"));
                int cpuWin = int.Parse(File.ReadAllText(path + "\\cpuWin.txt"));

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Player 1 Wins: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(p1Win);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Player 2 Wins: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(p2Win);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("CPU Wins: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(cpuWin);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nPress <ENTER> To Continue");

                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception ex)
            {
                File.WriteAllText(path + "\\p1Win.txt", 0.ToString());
                File.WriteAllText(path + "\\p2Win.txt", 0.ToString());
                File.WriteAllText(path + "\\cpuWin.txt", 0.ToString());
            }
        }
    }
}
