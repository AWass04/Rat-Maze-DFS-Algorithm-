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
            char[,] maze =
            {
                {'0','1','1','1','1','1','1','1','1','1','1','1'},
                {'1','0','1','1','0','1','1','0','0','1','0','1'},
                {'1','1','0','1','0','1','0','1','1','0','1','1'},
                {'1','1','1','0','1','0','1','1','1','1','0','1'},
                {'1','0','0','1','1','0','1','0','1','0','1','1'},
                {'1','1','1','1','1','0','1','1','0','1','0','1'},
                {'1','0','1','0','0','1','1','0','1','1','0','1'},
                {'1','1','0','1','1','1','1','0','1','0','1','1'},
                {'1','0','1','0','0','0','1','1','1','1','0','1'},
                {'1','0','1','1','1','1','0','1','1','1','0','1'},
                {'1','1','1','0','1','1','0','1','0','0','0','1'},
                {'1','0','0','1','0','0','1','0','1','1','1','E'},
                {'1','1','1','1','1','1','1','1','1','1','1','1'}
            };
            
            /*Second test maze 
            char[,] maze =
            {
                {'0','1','1','1','1','1','1','1','1','1','1','1','1'},
                {'1','0','1','1','0','1','1','0','0','0','0','0','E'},
                {'1','1','0','1','0','1','0','1','1','0','1','1','1'},
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
            }; */

            bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)]; // 2D array to track if a spot has been visted
            int timesExplored = 1; // Times rat has run around maze counter
            bool endFound;
            Stack<point> moves = new Stack<point>(); // Track of moves
            Stack<point> routeToExit = new Stack<point>(); 
            List<string> allRoutes = new List<string>(); // List of all paths to exit
            string routeTaken = null;
   
            var position = new point { X = 0, Y = 0, count = 1, positionChecking = timesExplored }; // Starting position

            do
            {
                // Reset
                maze[0, 0] = '0';
                PrintMaze(maze, timesExplored);
                moves.Clear();
                routeToExit.Clear();
                position = new point { X = 0, Y = 0, count = 1, positionChecking = timesExplored };
                endFound = false;

                do // Main exploration
                {
                    if (maze[position.X, position.Y] != 'E')
                    {
                        position = FindNextMove(position, maze, moves, timesExplored, visited);
                    }
                    else
                    {
                        endFound = true;

                        // Display rat
                        maze[position.X, position.Y] = 'R';
                        PrintMaze(maze, timesExplored);
                        maze[position.X, position.Y] = position.X == 0 && position.Y == 0 ? '1' : '0'; // Reset
                    }

                } while (moves.Count != 0 && !endFound); // Do while theres possible moves and exit hasnt been found

                // Reset visited array and increase times explored
                ResetVisitedArray(visited);
                timesExplored++;

                if (endFound)
                {
                    routeTaken = null;
                    maze[position.X, position.Y] = 'E'; 
                    Console.Write("End Found\n");

                    routeTaken = BuildRouteToExit(moves, routeToExit);
                    if (!allRoutes.Contains(routeTaken)) allRoutes.Add(routeTaken);
                }

            } while (endFound && timesExplored < 9);

            if (endFound)
            {
                Console.WriteLine("\nExit has been found");
                DisplayRoutes(allRoutes);
            }
            else Console.WriteLine("\nThere is no exit");
        }

        public class point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int count { get; set; }
            public int positionChecking { get; set; }
        }

        static point FindNextMove(point position, char[,] maze, Stack<point> moves, int timesExplored, bool[,] visitedPositions)
        {
            bool pathFound = false;

            // Get positions info
            int x = position.X;
            int y = position.Y;
            int count = position.count;
            int positionChecking = position.positionChecking;

            var nextMove = new point { X = 1, Y = 1, count = 1 };

            // Wrap around back to start
            if (positionChecking != 1) positionChecking++;
            else if (positionChecking == 9) positionChecking = 1;

            // Define the directions array.
            var directions = new (int dx, int dy)[]
            {
                (0, -1),   // North
                (1, -1),   // North-East
                (1, 0),    // East
                (1, 1),    // South-East
                (0, 1),    // South
                (-1, 1),   // South-West
                (-1, 0),   // West
                (-1, -1)   // North-West
            };

            // For each possible direction, check for moves
            for (int i = count; count <= 8; i++)
            {
                if (positionChecking == 9) positionChecking = 1;

                // Get the corresponding co-ordinates based on counter
                int dx = directions[positionChecking - 1].dx;
                int dy = directions[positionChecking - 1].dy;

                // Calculate the new position 
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newY >= 0 && newX < maze.GetLength(0) && newY < maze.GetLength(1)) // Ensure in bounds
                {
                    char symbol = maze[newX, newY];

                    // Check if it's a valid path and hasn't been visited before
                    if ((symbol == '0' || symbol == 'E') && !visitedPositions[newX, newY])
                    {
                        pathFound = true;

                        var currentPosition = new point { X = x, Y = y, count = count, positionChecking = positionChecking }; // Get current position
                        moves.Push(currentPosition);

                        // Mark this new position as visited
                        visitedPositions[newX, newY] = true;

                        nextMove = new point { X = newX, Y = newY, count = 1, positionChecking = position.positionChecking };
                        break;
                    }
                }

                count++;
                positionChecking++;
            }

            // If no path found, go back to the previous space
            if (!pathFound && moves.Count > 0) nextMove = moves.Pop();

            // Show rat's current location in the maze
            maze[position.X, position.Y] = 'R';
            PrintMaze(maze, timesExplored);

            // Restore the original position in the maze
            maze[position.X, position.Y] = position.X == 0 && position.Y == 0 ? '1' : '0';

            return nextMove;
        }

        static void PrintMaze(char[,] maze, int timesExplored)
        {
            Thread.Sleep(200); // Delay so user can view maze
            Console.Clear(); // Clear old maze

            Console.WriteLine("        Rat Maze        ");
            Console.WriteLine("------------------------\n");

            Console.WriteLine("Times searched maze: " + timesExplored + "\n");

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    switch(maze[i, j]) // Set colour of character
                    {
                        case 'R': Console.ForegroundColor = ConsoleColor.Cyan; break;
                        case '1': Console.ForegroundColor = ConsoleColor.Gray; break;
                        case '0': Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                        case 'E': Console.ForegroundColor = ConsoleColor.Red; break;
                    }

                    Console.Write(maze[i, j] + " ");

                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        static string BuildRouteToExit(Stack<point> moves, Stack<point> routeToExit)
        {
            string routeTaken = null;

            foreach (point p in moves) routeToExit.Push(p); // Get moves in correct order
            foreach (point p in routeToExit) routeTaken += "(" + (p.Y + 1) + "," + (p.X + 1) + ")" + " --> "; // Display route to exit

            routeTaken += "End";

            return routeTaken;
        }

        static void DisplayRoutes(List<string> allRoutes)
        {
            Console.WriteLine("\nPaths to exit:");
            Console.WriteLine("-------------");

            string shortestRoute = null;

            foreach (string r in allRoutes)
            {
                Console.WriteLine(r + "\n\n"); // Display path

                if (shortestRoute == null || r.Length < shortestRoute.Length) shortestRoute = r; // Find which is the chortest path based on length of string
            }

            Console.WriteLine("Shortest path to the exit:");
            Console.WriteLine("--------------------------\n\n");
            Console.WriteLine(shortestRoute);
        }
        static void ResetVisitedArray(bool[,] visited)
        {
            for (int i = 0; i < visited.GetLength(0); i++)
            {
                for (int j = 0; j < visited.GetLength(1); j++)
                {
                    visited[i, j] = false;
                }
            }
        }
    }
}
    
