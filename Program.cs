//Group 4
//
//﻿
using System.Security.Principal;
using System;
using System.Data;
using System.Numerics;

namespace Tic_Tac_Two_Final_Project
{
    public class HelloWorld
    {
        public static int NUM_ROWS = 5;
        public static int NUM_COLS = 5;
        static void Main(string[] args)
        {
            TT2();
        }
        public static void TT2()
        {
            Func<int[,], int[], int, int[]>[] strat = { HumanStrat, AIStrat };
            string[] stratNames = { "Human", "Strat1" };
            int p1 = 0, p2 = 1;
            Func<int[,], int[], int, int[]> p1Strat = strat[0];
            Func<int[,], int[], int, int[]> p2Strat = strat[1];
            int speed = 0; // 0=fast. 10=slow.
            string ui = "";
            int num = 0;
            bool gameOver = false;

            while (ui.ToUpper() != "QUIT" && gameOver == false)
            {
                Console.WriteLine("Type 'help' for options.");
                for (int i = 0; i < stratNames.Length; i++)
                {
                    Console.WriteLine(i + ". " + stratNames[i]);
                }
                Console.WriteLine("P1:\t" + p1 + " " + stratNames[p1]);
                Console.WriteLine("P2:\t" + p2 + " " + stratNames[p2]);
                Console.WriteLine("Play");
                Console.WriteLine("Quit");
                Console.Write("> ");
                ui = Console.ReadLine();
                if (ui.Length > 7 && ui.Substring(0, 6).ToUpper() == "SET P1")
                {
                    int.TryParse("0" + ui.Substring(7), out p1);
                    if (p1 < strat.Length && p1 >= 0)
                    {
                        p1Strat = strat[p1];
                    }
                    else
                    {
                        p1 = 0;
                        p1Strat = strat[0];
                    }
                }
                if (ui.Length > 7 && ui.Substring(0, 6).ToUpper() == "SET P2")
                {
                    int.TryParse("0" + ui.Substring(7), out p2);
                    if (p2 < strat.Length && p2 >= 0)
                        p2Strat = strat[p2];
                    else
                    {
                        p2 = 0;
                        p2Strat = strat[0];
                    }
                }
                if (ui.Length > 9 && ui.Substring(0, 9).ToUpper() == "SET SPEED")
                {
                    int.TryParse("0" + ui.Substring(9), out speed);
                    if (speed < 0)
                        speed = 0;
                }
                if (ui.ToUpper() == "HELP")
                    PrintHelp();
                if (ui.ToUpper() == "PLAY")
                    gameOver = Play(p1Strat, p2Strat, speed, NUM_ROWS, NUM_COLS);
                if (gameOver == true)
                {
                    Console.Clear();
                    Console.WriteLine("Thanks for playing!");
                    Console.ReadLine();
                    ui = "QUIT";
                }
            }
        }
        public static void PrintHelp()
        {
            Console.WriteLine("set p1 #");
            Console.WriteLine("set p2 #");
            Console.WriteLine("set speed #");
            Console.WriteLine("play");
            Console.WriteLine("play #");
        }

        public static bool Play(Func<int[,], int[], int, int[]> strat1, Func<int[,], int[], int, int[]> strat2, int speed, int rows, int cols)
        {
            Func<int[,], int[], int, int[]>[] strat = { strat1, strat2 };
            int[,] b = NewBoard(rows, cols);
            int[] gridCenter = { 2, 2 };
            int[,] tempBoard = null;
            int turn = 0; //X or O
            int row = 0, col = 0;
            int piece = 0;
            int[] Move = { 2, 2 };

            while (Winner(b, gridCenter) == 0)
            {
                if (turn % 2 == 0)
                    piece = -1; //X
                else
                    piece = 1; //O
                PrintBoard(b, gridCenter);
                tempBoard = Copy(b);
                Move = strat[turn % 2](tempBoard, gridCenter, turn);
                if (Move[1] == 9) //quit
                {
                    return true;
                }
                if (Move[0] == 99) //rules
                {
                    Console.Clear();
                    Console.Write("-The goal of the game is to get 3 in any direction within the grid (pieces outside the grid don't count when you count 3 in a row)\n-You may only place a piece inside the grid\n The grid can only be moved so that all 9 squares fit in the 5x5 box\n-You may move your own piece from outside or on the grid to a point ON THE GRID\n");
                }
                else if (Move.Length == 2) //placing piece
                {
                    row = Move[0];
                    col = Move[1];
                    b[row, col] = piece;
                    Console.Clear();
                }
                else if (Move.Length == 3) //Moving grid
                {
                    Console.Clear();
                    if (Move[2] == 1)
                        turn--;
                }
                else if (Move.Length == 4) //move piece onto the grid
                {
                    row = Move[0];
                    col = Move[1];
                    b[row, col] = piece;
                    b[Move[2], Move[3]] = 0;
                    Console.Clear();
                }
                turn++;
            }
            PrintBoard(b, gridCenter);
            if (Winner(b, gridCenter) == 2)
            {
                Console.WriteLine("Tie!");
                Console.ReadLine();
            }
            else if (Winner(b, gridCenter) == 1)
            {
                Console.WriteLine("O wins!");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("X wins!");
                Console.ReadLine();
            }

            return true;

        }
        public static int[] PlaceOnBoard(int[,] b, int player, int col, int row)
        {
            int[] loc = { -1, 0 };
            // Returns row and col it was placed. loc[0] = -1 if fail.
            if (b[row, col] == 0)
            {
                b[row, col] = player;
                loc[0] = row;
                loc[1] = col;
            }
            return loc;
        }

        public static int[,] Copy(int[,] b)
        {
            int[,] copy = new int[b.GetLength(0), b.GetLength(1)];
            for (int r = 0; r < b.GetLength(0); r++)
                for (int c = 0; c < b.GetLength(1); c++)
                    copy[r, c] = b[r, c];
            return copy;
        }
        public static int[,] NewBoard(int rows, int columns)
        {
            return new int[rows, columns];
        }
        public static int[] HumanStrat(int[,] b, int[] gridCenter, int turns)
        {
            //int ui = 0;
            int sumX = 0, sumO = 0;
            int[] move = { 0, 0 };
            int[] fail = { 0, 0, 0 };
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (b[i, j] == 1)
                        sumO++;
                    else if (b[i, j] == -1)
                        sumX++;
                }
            }
            if (turns % 2 == 0)
            {
                Console.WriteLine("Player X,");
                if (sumX == 4)
                {
                    Console.WriteLine("Pick an option:\n1 - Move the grid\n2 - Move a piece onto the grid\n3 - Rulebook\nQ - Quit");
                    string input = Console.ReadLine();
                    if (input.ToUpper() == "Q")
                    {
                        fail[1] = 9;
                        return fail;
                    }
                    while (input != "1" && input != "2" && input != "3" && input != "4")
                    {
                        Console.Write("Enter a valid option: ");
                        input = Console.ReadLine();
                    }
                    if (input.Equals("3"))
                    {
                        Console.Write("-The goal of the game is to get 3 in any direction within the grid(pieces outside the grid dont count when you count 3 in a row)\n-You may only place a piece inside the grid\n The grid can only be moved so that all 9 squares fit in the 5x5 box\n-You may move your own piece from outside or on the grid to a point ON THE GRID\n");
                        Console.WriteLine("Pick a new option");
                        input = Console.ReadLine();
                        while (input.ToLower() != "q" && input != "1" && input != "2")
                        {
                            Console.Write("Enter a valid option: ");
                            input = Console.ReadLine();
                        }
                    }
                    if (input.Equals("1"))
                    {
                        int[] tempGrid = SmallTracker(gridCenter);
                        if (tempGrid.Length == 3)
                            fail[2] = 1;
                    }
                    else if (input.Equals("2"))
                    {
                        move = Option3MovePiece(b, gridCenter, turns);
                        return move;
                    }
                    return fail; //option 2 and Q
                }
            }
            else
            {
                Console.WriteLine("Player O,");
                if (sumO == 4)
                {
                    Console.WriteLine("Pick an option:\n1 - Move the grid\n2 - Move a piece onto the grid\n3 - Rulebook\nQ - Quit");
                    string input = Console.ReadLine();
                    if (input.ToUpper() == "Q")
                    {
                        fail[1] = 9;
                        return fail;
                    }
                    while (input != "1" && input != "2" && input != "3" && input != "4")
                    {
                        Console.Write("Enter a valid option: ");
                        input = Console.ReadLine();
                    }
                    if (input.Equals("3"))
                    {
                        Console.Write("-The goal of the game is to get 3 in any direction within the grid(pieces outside the grid dont count when you count 3 in a row)\n-You may only place a piece inside the grid\n The grid can only be moved so that all 9 squares fit in the 5x5 box\n-You may move your own piece from outside or on the grid to a point ON THE GRID\n");
                        Console.WriteLine("Pick a new option");
                        input = Console.ReadLine();
                        while (input.ToLower() != "q" && input != "1" && input != "2")
                        {
                            Console.Write("Enter a valid option: ");
                            input = Console.ReadLine();
                        }
                    }
                    if (input.Equals("1"))
                    {
                        int[] tempGrid = SmallTracker(gridCenter);
                        if (tempGrid.Length == 3)
                            fail[2] = 1;
                    }
                    else if (input.Equals("2"))
                    {
                        move = Option3MovePiece(b, gridCenter, turns);
                        return move;
                    }
                    return fail; //option 2 and Q
                }
            }

            if (turns > 3)
            {
                Console.WriteLine("Pick an option:\n1 - Place a piece on the grid\n2 - Move the grid\n3- Move a piece onto the grid\n4 - Rulebook\nQ - Quit");
                string input = Console.ReadLine();
                if (input.ToUpper() == "Q")
                {
                    fail[1] = 9;
                    return fail;
                }
                while (input != "1" && input != "2" && input != "3" && input != "4")
                {
                    Console.Write("Enter a valid option: ");
                    input = Console.ReadLine();
                }
                if (input.Equals("4"))
                {
                    Console.Write("-The goal of the game is to get 3 in any direction within the grid (pieces outside the grid dont count when you count 3 in a row)\n-You may only place a piece inside the grid\n The grid can only be moved so that all 9 squares fit in the 5x5 box\n-You may move your own piece from outside or on the grid to a point ON THE GRID\n");
                    Console.WriteLine("Pick a new option");
                    input = Console.ReadLine();
                    while (input.ToLower() != "q" && input != "1" && input != "2" && input != "3")
                    {
                        Console.Write("Enter a valid option: ");
                        input = Console.ReadLine();
                    }
                }
                if (input.Equals("1"))
                {
                    move = PlayerMove(b, gridCenter, turns);
                    return move;
                }
                else if (input.Equals("2"))
                {
                    int[] tempGrid = SmallTracker(gridCenter);
                    if (tempGrid.Length == 3)
                        fail[2] = 1;
                }
                else if (input.Equals("3"))
                {
                    move = Option3MovePiece(b, gridCenter, turns);
                    return move;
                }
                return fail; //option 2 and Q
            }
            else
            {
                Console.WriteLine("Pick an option:\n1 - Place a piece on the grid\n2 - Rulebook\nQ - Quit");
                string input = Console.ReadLine();
                if (input.ToUpper() == "Q")
                {
                    fail[1] = 9;
                    return fail;
                }
                while (input != "1" && input != "2")
                {
                    Console.Write("Enter a valid option: ");
                    input = Console.ReadLine();
                }
                if (input.Equals("2"))
                {
                    Console.Write("-The goal of the game is to get 3 in any direction within the grid(pieces outside the grid don't count when you count 3 in a row)\n-You may only place a piece inside the grid\n The grid can only be moved so that all 9 squares fit in the 5x5 box\n-You may move your own piece from outside or on the grid to a point ON THE GRID\n");
                    Console.WriteLine("Pick a new option");
                    input = Console.ReadLine();
                    while (input.ToLower() != "q" && input != "1")
                    {
                        Console.Write("Enter a valid option: ");
                        input = Console.ReadLine();
                    }
                }
                if (input.Equals("1"))
                {
                    move = PlayerMove(b, gridCenter, turns);
                    return move;
                }
                return fail;
            }
        }


        public static int[] AIStrat(int[,] b, int[] gridCenter, int turns)
        {
            int[] forGridShift = { 0, 0, 0 };
            int[] origCent = gridCenter;
            Random r = new Random();
            int aiChoiceR = r.Next(0, 2);

            if (turns % 2 == 0) //X
            {
                if (PiecesOnBoard(b, -1) < 3)
                {
                    if (WinMoveDefend(b, gridCenter, -1).Length != 1)
                        return WinMoveDefend(b, gridCenter, -1);
                    else
                        return initialMove(b, gridCenter);
                }
                if (PiecesOnBoard(b, -1) < 4)
                {
                    if (WinMoveDefend(b, gridCenter, -1).Length != 1)
                        return WinMoveDefend(b, gridCenter, -1);
                    if (aiChoiceR == 0)
                        return aiMoveRand(b, origCent);
                }
                if (WinMoveDefend(b, gridCenter, -1).Length != 1)
                    return WinMoveDefend(b, gridCenter, -1);
            }
            else //o
            {
                if (PiecesOnBoard(b, 1) < 3)
                {
                    if (WinMoveDefend(b, gridCenter, 1).Length != 1)
                        return WinMoveDefend(b, gridCenter, 1);
                    else
                        return initialMove(b, gridCenter);
                }
                if (PiecesOnBoard(b, 1) < 4)
                {
                    if (WinMoveDefend(b, gridCenter, 1).Length != 1)
                        return WinMoveDefend(b, gridCenter, 1);
                    if (aiChoiceR == 0)
                        return aiMoveRand(b, origCent);
                }
                if (WinMoveDefend(b, gridCenter, 1).Length != 1)
                    return WinMoveDefend(b, gridCenter, 1);
            }

            gridCenter = AISmallTracker(origCent, r.Next(0, 9));
            return forGridShift;
        }


        public static int[] aiMoveRand(int[,] b, int[] gridCenter)
        {
            Random rng = new Random();
            int row = gridCenter[0], col = gridCenter[1];
            while (b[row, col] != 0)
            {
                row = rng.Next(gridCenter[0] - 1, gridCenter[0] + 2);
                col = rng.Next(gridCenter[1] - 1, gridCenter[1] + 2);
            }
            return new int[] { row, col };
        }


        public static int[] initialMove(int[,] b, int[] gridCenter)
        {
            //First check center, then check corners; if all fails, put a random piece
            Random rng = new Random();
            int row = gridCenter[0], col = gridCenter[1];

            if (b[row, col] != 0) //center is taken
            {   //check corners
                if (b[gridCenter[0] - 1, gridCenter[1] - 1] == 0)
                { //upperleft
                    row = gridCenter[0] - 1;
                    col = gridCenter[1] - 1;
                }
                else if (b[gridCenter[0] + 1, gridCenter[1] - 1] == 0)
                { //lowerleft
                    row = gridCenter[0] + 1;
                    col = gridCenter[1] - 1;
                }
                else if (b[gridCenter[0] - 1, gridCenter[1] + 1] == 0)
                { //upper-right
                    row = gridCenter[0] - 1;
                    col = gridCenter[1] + 1;
                }
                else if (b[gridCenter[0] + 1, gridCenter[1] + 1] == 0)
                { //lower-right
                    row = gridCenter[0] + 1;
                    col = gridCenter[1] + 1;
                }
                else
                {  //random
                    while (b[row, col] != 0 || !GridCheck(gridCenter, row, col))
                    {
                        row = rng.Next(gridCenter[0] - 1, gridCenter[0] + 2);
                        col = rng.Next(gridCenter[1] - 1, gridCenter[1] + 2);
                    }
                }
            }

            int[] move = { row, col };
            return move;
        }

        public static int[] WinMoveDefend(int[,] b, int[] gridCenter, int player)
        {
            int opponent = 1;
            if (player == 1)
                opponent = -1;
            int[] fail = new int[1];
            int[] move = new int[2];
            int[] small = new int[3];
            int colcent = gridCenter[1], rowcent = gridCenter[0];
            if (PiecesOnGrid(b, gridCenter, player) > 1 && PiecesOnBoard(b, player) < 4)
            {//has at least 2 pieces on the grid and still has pieces left
                for (int i = rowcent - 1; i < rowcent + 2; i++)
                {
                    for (int j = colcent - 1; j < colcent + 2; j++)
                    {
                        if (b[i, j] == 0)
                        {//win
                            b[i, j] = player;
                            if (Winner(b, gridCenter) == player)
                            {
                                b[i, j] = 0;
                                move[0] = i;
                                move[1] = j;
                                return move;
                            }
                            b[i, j] = 0;
                        }
                    }
                }
            }
            for (int num = 1; num < 9; num++)
            {//win with grid shift
                gridCenter = AISmallTracker(gridCenter, num);
                for (int i = rowcent - 1; i < rowcent + 2; i++)
                {
                    for (int j = colcent - 1; j < colcent + 2; j++)
                    {
                        if (Winner(b, gridCenter) == player)
                        {
                            small[0] = gridCenter[0];
                            small[1] = gridCenter[1];
                            gridCenter[0] = rowcent;
                            gridCenter[1] = colcent;
                            return small;
                        }
                    }
                }
            }

            if (PiecesOnGrid(b, gridCenter, opponent) > 1 && PiecesOnBoard(b, player) < 4)
            {//oppo has at least 2 pieces on the grid and you still have pieces left
                for (int i = rowcent - 1; i < rowcent + 2; i++)
                {
                    for (int j = colcent - 1; j < colcent + 2; j++)
                    {
                        //block
                        if (b[i, j] == 0)
                        {
                            b[i, j] = opponent;
                            if (Winner(b, gridCenter) == opponent)
                            {
                                b[i, j] = 0;
                                move[0] = i;
                                move[1] = j;
                                return move;
                            }
                            b[i, j] = 0;
                        }
                    }
                }
            }

            for (int num = 1; num < 9; num++)
            {  //block, move grid in opposite direction
                gridCenter = AISmallTracker(gridCenter, num);
                for (int i = rowcent - 1; i < rowcent + 2; i++)
                {
                    for (int j = colcent - 1; j < colcent + 2; j++)
                    {
                        if (Winner(b, gridCenter) == opponent)
                        {
                            gridCenter = AISmallTracker(gridCenter, 9 - num);
                            small[0] = gridCenter[0];
                            small[1] = gridCenter[1];
                            gridCenter[0] = rowcent;
                            gridCenter[1] = colcent;
                            return small;
                        }
                        gridCenter[0] = rowcent;
                        gridCenter[1] = colcent;
                    }
                }
            }
            //shift grid to block oppo once pieces have been used
            if (PiecesOnGrid(b, gridCenter, opponent) > 1 && PiecesOnBoard(b, player) > 4)
            {//oppo has at least 2 pieces on the grid and you run out of pieces 
                for (int i = rowcent - 1; i < rowcent + 2; i++)
                {
                    for (int j = colcent - 1; j < colcent + 2; j++)
                    {
                        if (b[i, j] == 0)
                        {
                            b[i, j] = opponent;
                            if (Winner(b, gridCenter) == opponent)
                            {
                                b[i, j] = 0;
                                Random rng = new Random();
                                gridCenter = AISmallTracker(gridCenter, rng.Next(1, 9));
                            }
                        }
                    }
                }
            }
            return fail;
        }

        public static int PiecesOnGrid(int[,] b, int[] gridCenter, int player) //return number of X pieces on the grid
        {
            int sum = 0;
            for (int row = gridCenter[0] - 1; row < gridCenter[0] + 2; row++)
            {
                for (int col = gridCenter[1] - 1; col < gridCenter[1] + 2; col++)
                {
                    if (b[row, col] == player)
                        sum++;
                }
            }
            return sum;
        }
        public static int PiecesOnBoard(int[,] b, int player)
        {
            int sum = 0;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (b[i, j] == player)
                        sum++;
                }
            }
            return sum;
        }

        public static int[] AISmallTracker(int[] centerpast, int num)
        {
            int fail = 1;
            int num1 = 0;
            Random rng = new Random();
            while (fail == 1)
            {
                if (num == 1 && centerpast[1] - 1 > 0)
                { //left
                    centerpast[1] -= 1;
                    fail = 0;
                }
                else if (num == 8 && centerpast[1] + 1 < 4)
                { //right
                    centerpast[1] += 1;
                    fail = 0;
                }
                else if (num == 2 && centerpast[0] - 1 > 0)
                { //up
                    centerpast[0] -= 1;
                    fail = 0;
                }
                else if (num == 7 && centerpast[0] + 1 < 4)
                { //down
                    centerpast[0] += 1;
                    fail = 0;
                }
                else if (num == 3 && centerpast[0] + 1 < 4 && centerpast[1] + 1 < 4)
                { //downdiagonalright
                    centerpast[0] += 1;
                    centerpast[1] += 1;
                    fail = 0;
                }
                else if (num == 4 && centerpast[0] + 1 < 4 && centerpast[1] - 1 > 0)
                { //downdiagonalleft
                    centerpast[0] += 1;
                    centerpast[1] -= 1;
                    fail = 0;
                }
                else if (num == 6 && centerpast[0] - 1 > 0 && centerpast[1] - 1 > 0)
                { //updiagonalleft
                    centerpast[0] -= 1;
                    centerpast[1] -= 1;
                    fail = 0;
                }
                else if (num == 5 && centerpast[0] - 1 > 0 && centerpast[1] + 1 < 4)
                { //updiagonalright
                    centerpast[0] -= 1;
                    centerpast[1] += 1;
                    fail = 0;
                }
                else
                {
                    num1 = rng.Next(1, 9);
                    while (num1 == 9 - num) //num1 is not > 8
                        num1 = rng.Next(1, 9);
                    num = num1;
                }
            }
            return centerpast;
        }

        /* public static int[] randomShift(int[] gridCenter)
         {
             int[] move = { 0, 0, 0 };
             int fail = 1;
             while (fail == 1)
             {
                 Random gpt = new Random();
                 int aiRandom = gpt.Next(0, 9);

                 if (aiRandom == 1 && gridCenter[1] - 1 > 0)
                 { //left
                     gridCenter[1] -= 1;
                     fail = 0;
                 }
                 else if (aiRandom == 2 && gridCenter[1] + 1 < 4)
                 { //right
                     gridCenter[1] += 1;
                     fail = 0;
                 }
                 else if (aiRandom == 3 && gridCenter[0] - 1 > 0)
                 { //up
                     gridCenter[0] -= 1;
                     fail = 0;
                 }
                 else if (aiRandom == 4 && gridCenter[0] + 1 < 4)
                 { //down
                     gridCenter[0] += 1;
                     fail = 0;
                 }
                 else if (aiRandom == 5 && gridCenter[0] + 1 < 4 && gridCenter[1] + 1 < 4)
                 { //downdiagonalright
                     gridCenter[0] += 1;
                     gridCenter[1] += 1;
                     fail = 0;
                 }
                 else if (aiRandom == 6 && gridCenter[0] + 1 < 4 && gridCenter[1] - 1 > 0)
                 { //downdiagonalleft
                     gridCenter[0] += 1;
                     gridCenter[1] -= 1;
                     fail = 0;
                 }
                 else if (aiRandom == 7 && gridCenter[0] - 1 > 0 && gridCenter[1] - 1 > 0)
                 { //updiagonalleft
                     gridCenter[0] -= 1;
                     gridCenter[1] -= 1;
                     fail = 0;
                 }
                 else if (aiRandom == 8 && gridCenter[0] - 1 > 0 && gridCenter[1] + 1 < 4)
                 { //updiagonalright
                     gridCenter[0] -= 1;
                     gridCenter[1] += 1;
                     fail = 0;
                 }
             }
             return move;
         }
        */



        public static int AddTurns(int prevTurn)
        {
            int turns = prevTurn;
            turns++;
            return turns;
        }
        public static int[] SmallTracker(int[] centerpast)
        {
            int[] exit = { 0, 0, 0 };
            Console.WriteLine("Choose a direction:\n1. Left\n2. Right\n3. Up\n4. Down\n5. Downdiagonalright\n6. Downdiagonalleft\n7. Updiagonalleft\n8. Updiagonalright\n9. Exit");
            int direction;
            string ui = Console.ReadLine();
            while (!int.TryParse(ui, out direction))
            {
                Console.WriteLine("Enter a number.");
                ui = Console.ReadLine();
            }
            while (direction != 9)
            {
                if (direction == 1 && centerpast[1] - 1 > 0)
                { //left
                    centerpast[1] -= 1;
                    return centerpast;
                }
                else if (direction == 2 && centerpast[1] + 1 < 4)
                { //right
                    centerpast[1] += 1;
                    return centerpast;
                }
                else if (direction == 3 && centerpast[0] - 1 > 0)
                { //up
                    centerpast[0] -= 1;
                    return centerpast;
                }
                else if (direction == 4 && centerpast[0] + 1 < 4)
                { //down
                    centerpast[0] += 1;
                    return centerpast;
                }
                else if (direction == 5 && centerpast[0] + 1 < 4 && centerpast[1] + 1 < 4)
                { //downdiagonalright
                    centerpast[0] += 1;
                    centerpast[1] += 1;
                    return centerpast;

                }
                else if (direction == 6 && centerpast[0] + 1 < 4 && centerpast[1] - 1 > 0)
                { //downdiagonalleft
                    centerpast[0] += 1;
                    centerpast[1] -= 1;
                    return centerpast;

                }
                else if (direction == 7 && centerpast[0] - 1 > 0 && centerpast[1] - 1 > 0)
                { //updiagonalleft
                    centerpast[0] -= 1;
                    centerpast[1] -= 1;
                    return centerpast;

                }
                else if (direction == 8 && centerpast[0] - 1 > 0 && centerpast[1] + 1 < 4)
                { //updiagonalright
                    centerpast[0] -= 1;
                    centerpast[1] += 1;
                    return centerpast;
                }
                else
                {
                    Console.Write("Enter a valid direction: ");
                    ui = Console.ReadLine();
                    while (!int.TryParse(ui, out direction))
                    {
                        Console.WriteLine("Enter a number.");
                        ui = Console.ReadLine();
                    }
                    //direction = int.Parse(Console.ReadLine());
                }
            }
            return exit;
        }




        public static int[] PlayerMove(int[,] b, int[] centerpast, int turns)
        {
            int[] move = { 0, 0 };

            bool check = false;
            while (!check)
            {
                Console.WriteLine();
                Console.WriteLine("Row?");
                string ui = Console.ReadLine();
                int r = 0, c = 0;
                while (!int.TryParse(ui, out r))
                {
                    Console.WriteLine("Enter a number.");
                    ui = Console.ReadLine();
                }
                Console.WriteLine("Column?");
                ui = Console.ReadLine();
                while (!int.TryParse(ui, out c))
                {
                    Console.WriteLine("Enter a number.");
                    ui = Console.ReadLine();
                }
                Console.WriteLine();
                if (!(c >= 0 && c <= 4) || !(r >= 0 && r <= 4)) //if out of bounds
                    Console.WriteLine("Enter a valid space.");
                else if ((c >= 0 && c <= 4) && (r >= 0 && r <= 4))// if in bounds
                {
                    if ((centerpast[0] == r || centerpast[0] == r - 1 || centerpast[0] == r + 1) && (centerpast[1] == c || centerpast[1] == c - 1 || centerpast[1] == c + 1))
                    {
                        if (b[r, c] == 0) // if in bounds + space is empty.
                        {
                            move[0] = r;
                            move[1] = c;
                            check = true;
                        }
                        else
                            Console.WriteLine("That space is already taken.");
                    }
                    else
                        Console.WriteLine("That space is not in the grid.");
                }
            }
            return move;
        }

        public static int[] Option3MovePiece(int[,] b, int[] gridCenter, int turns)
        {
            bool viable = false, status = false; //status is for whether or not the move has been executed
            int[] newLoc = { 0, 0, 0, 0 };
            //int row = 0, column = 0, row2 = 0, column2 = 0;
            int piece = 0; // Value for X or O
            if (turns % 2 == 0) // Even or Odd -> X or O
                piece = -1; //x
            else
                piece = 1; //o
            while (!viable && !status) // Loops until a viable move is chosen
            {
                Console.WriteLine("Row of the piece you’re moving: "); // Taking move location
                string ui = Console.ReadLine();
                while (!int.TryParse(ui, out newLoc[2]))
                {
                    Console.WriteLine("Enter a number.");
                    ui = Console.ReadLine();
                }
                Console.WriteLine("Column of the piece you’re moving: "); // Taking move location
                ui = Console.ReadLine();
                while (!int.TryParse(ui, out newLoc[3]))
                {
                    Console.WriteLine("Enter a number.");
                    ui = Console.ReadLine();
                }
                if (b[newLoc[2], newLoc[3]] == piece && newLoc[2] <= 4 && newLoc[2] >= 0 && newLoc[3] <= 4 && newLoc[3] >= 0) // Checks if piece exists and is within bounds
                {
                    viable = true;
                }
                else
                    Console.WriteLine("Please choose a viable location with your piece on it and make sure it’s a coordinate on the board");
            }

            while (viable && !status)
            {
                Console.WriteLine("Row of the new location: "); // Taking move location
                string ui = Console.ReadLine();
                while (!int.TryParse(ui, out newLoc[0]))
                {
                    Console.WriteLine("Enter a number.");
                    ui = Console.ReadLine();
                }
                Console.WriteLine("Column of the new location: "); // Taking move location
                ui = Console.ReadLine();
                while (!int.TryParse(ui, out newLoc[1]))
                {
                    Console.WriteLine("Enter a number.");
                    ui = Console.ReadLine();
                }
                if (b[newLoc[0], newLoc[1]] == 0 && GridCheck(gridCenter, newLoc[0], newLoc[1]))
                {
                    status = true;
                }
                else
                {
                    Console.WriteLine("Please choose a viable location."); // Loops until they find a viable location
                }
            }
            //b[row, column] = 0;
            Console.Clear();
            return newLoc;
        }

        public static int Winner(int[,] b, int[] gridCenter)
        {
            int sum = 0;
            int[] playerWin = { 0, 0 };
            // Check horizontal
            for (int r = gridCenter[0] - 1; r <= gridCenter[0] + 1; r++)
            {
                int c = gridCenter[1] - 1;
                if (b[r, c] != 0)
                {
                    sum = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        sum += b[r, c + i];
                    }
                    if (sum == 3 * b[r, c])
                    {
                        if (b[r, c] == 1) //o
                            playerWin[0]++;
                        else //x
                            playerWin[1]++;
                    }

                }
            }
            // Check vertical
            for (int c = gridCenter[1] - 1; c <= gridCenter[1] + 1; c++)
            {
                int r = gridCenter[0] - 1;
                if (b[r, c] != 0)
                {
                    sum = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        sum += b[r + i, c];
                    }
                    if (sum == 3 * b[r, c])
                    {
                        if (b[r, c] == -1) //x
                            playerWin[1]++;
                        else //o
                            playerWin[0]++;
                    }
                }
            }

            // Check \ and /
            int diagRow = gridCenter[0] - 1;
            int diagCol = gridCenter[1] - 1;
            int diagRow2 = gridCenter[0] + 1;

            if (b[diagRow, diagCol] != 0)
            {
                sum = 0;
                for (int i = 0; i < 3; i++)
                {
                    sum += b[diagRow + i, diagCol + i];
                }
                if (sum == 3 * b[diagRow, diagCol])
                {
                    if (b[diagRow, diagCol] == 1) //o
                        playerWin[0]++;
                    else
                        playerWin[1]++; //x
                }
            }
            if (b[diagRow2, diagCol] != 0)
            {
                sum = 0;
                for (int i = 0; i < 3; i++)
                {
                    sum += b[diagRow2 - i, diagCol + i];
                }
                if (sum == 3 * b[diagRow2, diagCol])
                {
                    if (b[diagRow2, diagCol] == 1) //o
                        playerWin[0]++;
                    else
                        playerWin[1]++; //x
                }
            }
            if (playerWin[0] == 0 && playerWin[1] == 0)
                return 0; // no winner
            else if (playerWin[0] != 0 && playerWin[1] != 0)
                return 2; //tie
            else if (playerWin[0] != 0 && playerWin[1] == 0)
                return 1; // 1st player, 1, wins OR 2nd player, -1, wins --> use turns counter to identify player
            else
                return -1;
        }



        public static string PlayerLetter(int p) //for printing board
        {
            if (p == 1)
                return "O";
            if (p == -1)
                return "X";
            return "-";
        }


        public static void PrintBoard(int[,] b, int[] gridCenter) //takes care of grid color
        {

            for (int r = 0; r < b.GetLength(0); r++)
            {
                for (int c = 0; c < b.GetLength(1); c++)
                {

                    if (r <= gridCenter[0] + 1 && r >= gridCenter[0] - 1 && c <= gridCenter[1] + 1 && c >= gridCenter[1] - 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(PlayerLetter(b[r, c]) + " ");
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(PlayerLetter(b[r, c]) + " ");
                    }
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(r + "\n");
            }

            for (int c = 0; c < b.GetLength(1); c++)
            {
                Console.Write(c + " ");
            }
            Console.Write("\n");
        }


        public static bool GridCheck(int[] gridCenter, int row, int column) //check if position is on grid
        {
            if ((row == gridCenter[0] - 1 || row == gridCenter[0] || row == gridCenter[0] + 1) && (column == gridCenter[1] - 1 || column == gridCenter[1] || column == gridCenter[1] + 1))
                return true;
            return false;
        }
        public static bool CenterCheck(int[] gridCenter) // check if position can be a gridCenter 
        {
            if (gridCenter[0] - 1 > 0 && gridCenter[0] + 1 < NUM_ROWS - 1 && gridCenter[1] - 1 > 0 && gridCenter[1] + 1 < NUM_COLS - 1)
                return true;
            return false;
        }


    }
}




