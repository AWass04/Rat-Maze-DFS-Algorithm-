using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace MouseMaze
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Defining the maze
            char[,] Maze =
            {
                {'0','1','1','1','1','1','1','1','1','1','1','1'},
                {'1','0','1','1','0','1','1','0','0','1','0','1'},
                {'1','1','0','1','0','1','0','1','1','0','1','1'},
                {'1','1','1','0','1','0','1','1','1','1','0','1'},
                {'1','0','0','1','1','0','1','0','1','0','1','1'},
                {'1','1','1','1','1','0','1','1','0','1','0','1'},
                {'1','0','1','0','0','1','1','0','1','1','0','1'},
                {'1','1','0','1','1','1','1','0','1','0','1','1'},
                {'1','0','1','0','0','0','1','1','1','1','1','1'},
                {'1','0','1','1','1','1','0','1','1','0','1','1'},
                {'1','1','1','0','1','1','0','1','0','1','0','1'},
                {'1','0','0','1','0','0','1','0','1','1','1','E'},
                {'1','1','1','1','1','1','1','1','1','1','1','1'},
            };

            
            /*
            Second test maze 
            char[,] Maze =
            {
                {'0','1','1','1','1','1','1','1','1','1','1','1','1'},
                {'1','0','1','1','0','1','0','1','1','0','0','0','E'},
                {'1','1','0','1','0','1','0','1','0','1','1','1','1'},
                {'1','1','1','0','1','0','1','1','1','0','0','0','1'},
                {'1','0','1','0','1','0','1','1','0','1','1','1','1'},
                {'1','0','1','0','1','1','0','1','1','0','0','1','1'},
                {'1','0','1','1','0','1','1','1','1','1','1','0','1'},
                {'1','1','0','1','1','0','1','0','0','1','1','0','1'},
                {'1','1','1','0','1','0','1','1','1','0','0','1','1'},
                {'1','0','0','1','1','0','1','0','0','1','1','1','1'},
                {'1','1','1','0','1','0','1','1','1','0','0','1','1'},
                {'1','1','0','1','0','1','1','0','1','1','1','0','1'},
                {'1','0','1','1','0','1','0','1','1','0','0','1','1'},
                {'1','1','0','1','1','1','0','1','0','1','1','1','1'},
                {'1','0','1','0','0','1','1','0','1','1','0','1','1'},
                {'1','0','1','1','1','0','0','1','0','0','1','0','1'},
                {'1','1','1','1','1','1','1','1','1','1','1','1','1'}
            };
            
            */

            PrintMaze(Maze);

            bool endFound = false;

            Stack<point> Moves = new Stack<point>(); // Track of moves
            Stack<point> LastVisted = new Stack<point>(); // Track of moves before last

            var position = new point { X = 0, Y = 0, Count = 1 }; // Starting position

            do // Do until end is found or no other possible moves
            {

                if (Maze[position.X, position.Y] != 'E') position = FindNextMove(position, Maze, Moves, LastVisted);
                else
                {
                    endFound = true;

                    Maze[position.X, position.Y] = 'R';
                    PrintMaze(Maze);

                    // Restore the original position in the maze.
                    Maze[position.X, position.Y] = position.X == 0 && position.Y == 0 ? '1' : '0';
                }

            } while (Moves.Count != 0 && !endFound);

            if (endFound) 
            {
                Console.WriteLine("\nExit has been found");
                Console.WriteLine("\nPath to exit:");
                Console.WriteLine("-------------");

                Stack<point> RouteToExit = new Stack<point>(); 

                foreach (point p in Moves) RouteToExit.Push(p); // Get moves in correct order

                // Display route to exit
                foreach (point p in RouteToExit)
                {
                    Console.Write("(" + (p.Y + 1) + "," + (p.X +1) + ")");
                    Console.Write(" --> ");
                }

                Console.Write("End Found\n");
            }
            else Console.WriteLine("\nThere is no exit");
        }

        static void PrintMaze(char[,] maze)
        {
            //Thread.Sleep(1000); // Delay so user can view maze
            Console.Clear(); // Clear old maze

            Console.WriteLine("        Rat Maze        ");
            Console.WriteLine("------------------------\n");

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    // Change colour based upon whats being printed
                    if (maze[i, j] == 'R') Console.ForegroundColor = ConsoleColor.Cyan;
                    else if (maze[i, j] == '1') Console.ForegroundColor = ConsoleColor.Gray;
                    else if (maze[i, j] == '0') Console.ForegroundColor = ConsoleColor.DarkGreen;
                    else if (maze[i, j] == 'E') Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write(maze[i, j] + " ");

                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        public class point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Count { get; set; }

        }

        static point FindNextMove(point position, char[,] Maze, Stack<point> Moves, Stack<point> LastVisted)
        {
            bool pathFound = false;

            int x = position.X;
            int y = position.Y;
            int count = position.Count;

            var nextMove = new point { X = 1, Y = 1, Count = 1 };

            if (count != 1) count++;

            // Define the directions array.
            var directions = new (int dx, int dy)[]
            {
                (0, -1),   // North
                (1, 0),    // East
                (0, 1),    // South
                (-1, 0),   // West
                (-1, 1),   // South-West
                (-1, -1),  // North-West
                (1, 1),    // South-East
                (1, -1)    // North-East
            };

            // For each possible direction, check for moves
            for (int i = count; i <= 8; i++)  
            {
                
                // Get the corresponding co-oridents dependant on counter
                int dx = directions[count - 1].dx;
                int dy = directions[count - 1].dy;

                // Calculate the new position 
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newY >= 0) // if co-ordients are in the bounds of maze
                {
                    char symbol = Maze[newX, newY];

                    // If it is a path and isnt the position came from 
                    if ((symbol == '0' || symbol == 'E') && (Moves.Count == 0 || (newX != Moves.Peek().X || newY != Moves.Peek().Y)) && (LastVisted.Count == 0 || (newX != LastVisted.Peek().X || newY != LastVisted.Peek().Y)))
                    {
                        pathFound = true;

                        var currentPosition = new point { X = x, Y = y, Count = i }; // get current position
                        Moves.Push(currentPosition);
                         
                        nextMove = new point { X = newX, Y = newY, Count = 1 };
                        break;
                    }
                }

                count++;
               
            }

            // If no path go back to previous space
            if (!pathFound && Moves.Count > 0)
            {
                nextMove = Moves.Pop();
            }

            // Show rat's current location in the maze
            Maze[position.X, position.Y] = 'R';
            PrintMaze(Maze);

            Maze[position.X, position.Y] = position.X == 0 && position.Y == 0 ? '1' : '0';

            var pastMove = new point { X = x, Y = y, Count = 1 };
            LastVisted.Push(pastMove);

            return nextMove;
        }

    }
}
