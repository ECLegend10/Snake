﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Windows.Media;
using System.Media;
using System.IO;

namespace Snake
{
    struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int lastFoodTime = 0;
            int foodDissapearTime = 8000;
            int negativePoints = 0;

            Position[] directions = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
            };
            double sleepTime = 100;
            int direction = right;
            Random randomNumbersGenerator = new Random();
            Console.BufferHeight = Console.WindowHeight;
            lastFoodTime = Environment.TickCount;

            // Initialise obstacles
            List<Position> obstacles = new List<Position>()
            {
                new Position(12, 12),
                new Position(14, 20),
                new Position(7, 7),
                new Position(19, 19),
                new Position(6, 9),
            };
            foreach (Position obstacle in obstacles)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(obstacle.col, obstacle.row);
                Console.Write("=");
            }

            Queue<Position> snakeElements = new Queue<Position>();
            for (int i = 0; i <= 3; i++) //change the initial length of snake from 5 to 3
            {
                snakeElements.Enqueue(new Position(0, i));
            }

            // Initialise food
            Position food;
            do
            {
                food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                    randomNumbersGenerator.Next(0, Console.WindowWidth));
            }
            while (snakeElements.Contains(food) || obstacles.Contains(food));
            Console.SetCursorPosition(food.col, food.row);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("@");

            foreach (Position position in snakeElements)
            {
                Console.SetCursorPosition(position.col, position.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");
            }


            //Below are the music packs
            MediaPlayer backgroundMusic = new MediaPlayer();//Continous background music
            backgroundMusic.Open(new System.Uri(Path.Combine(System.IO.Directory.GetCurrentDirectory(),@"..\..\sounds\\backgroundMusic.wav")));
            

            SoundPlayer changeEffect = new SoundPlayer(@"..\..\sounds\changePosition.wav");//sound effect when changing directions
            SoundPlayer eatEffect = new SoundPlayer(@"..\..\sounds\munchApple.wav");//sound effect when eating an apple
            SoundPlayer ObstacleEffect = new SoundPlayer(@"..\..\sounds\obstacleHit.wav");//sound effect when an obstacle is hit

            while (true)
            {
                backgroundMusic.Play();
                negativePoints++;
                // Control direction of snake
                if (Console.KeyAvailable)
                {
                    changeEffect.Play();
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if (direction != right) direction = left;
                    }
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if (direction != left) direction = right;
                    }
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (direction != down) direction = up;
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (direction != up) direction = down;
                    }
                }

                // Reassign snake's head after crossing border
                Position snakeHead = snakeElements.Last();  // Head at end of queue
                Position nextDirection = directions[direction];

                // If crossed border, move to other end of the terminal
                Position snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                    snakeHead.col + nextDirection.col);

                if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 1;
                if (snakeNewHead.row < 0) snakeNewHead.row = Console.WindowHeight - 1;
                if (snakeNewHead.row >= Console.WindowHeight) snakeNewHead.row = 0;
                if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;

                // If the snake hits itself or hits the obstacles
                if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                {
                    ObstacleEffect.Play();
                    Thread.Sleep(500);
                    int userPoints = (snakeElements.Count - 6) * 100- negativePoints;
                    //if (userPoints < 0) userPoints = 0;
                    userPoints = Math.Max(userPoints, 0);
                    string gameovertext = "Game over!";
                    string yourpointsare = "Your points are: {0}";
                    string resultmessage;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition((Console.WindowWidth - gameovertext.Length)/2, Console.WindowHeight/4);
                    Console.WriteLine(gameovertext);
                    Console.SetCursorPosition((Console.WindowWidth - yourpointsare.Length) / 2, (Console.WindowHeight / 4)+1);
                    Console.WriteLine(yourpointsare, userPoints);
                    
                    if (userPoints >=30)//checks if the player meets winning requirement
                    {
                        resultmessage = "Congratulation! You've won the game :D";
                        Console.SetCursorPosition((Console.WindowWidth - resultmessage.Length) / 2, (Console.WindowHeight / 4) + 2);
                        Console.WriteLine(resultmessage);
                    }
                    else 
                    {
                        resultmessage = "Sorry, you've lost :(";
                        Console.SetCursorPosition((Console.WindowWidth - resultmessage.Length) / 2, (Console.WindowHeight / 4) + 2);
                        Console.WriteLine(resultmessage);
                        Console.SetCursorPosition((Console.WindowWidth - 33) / 2, (Console.WindowHeight / 4) + 3);
                        Console.WriteLine("Reach 100 Points next time to win");
                    }

                    return;
                }

                Console.SetCursorPosition(snakeHead.col, snakeHead.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");

                snakeElements.Enqueue(snakeNewHead);
                Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (direction == right) Console.Write(">");
                if (direction == left) Console.Write("<");
                if (direction == up) Console.Write("^");
                if (direction == down) Console.Write("v");

                // If snake consumes the food:
                if (snakeNewHead.col == food.col && snakeNewHead.row == food.row)
                {
                    // feeding the snake
                    do
                    {
                        food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));
                        eatEffect.Play();
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;  // Reset last food Time
                    Console.SetCursorPosition(food.col, food.row);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("@");
                    sleepTime--;

                    // Generate new obstacle
                    Position obstacle = new Position();
                    do
                    {
                        obstacle = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(obstacle) ||
                        obstacles.Contains(obstacle) ||
                        (food.row != obstacle.row && food.col != obstacle.row));
                    obstacles.Add(obstacle);
                    Console.SetCursorPosition(obstacle.col, obstacle.row);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("=");
                }
                else
                {
                    // moving...
                    Position last = snakeElements.Dequeue();  // Remove the last bit of snake.
                    Console.SetCursorPosition(last.col, last.row);
                    Console.Write(" ");
                    
                }

                // If food not consumed before time limit, generate new food.
                if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                {
                    negativePoints = negativePoints + 50;  // Additional negative points
                    Console.SetCursorPosition(food.col, food.row);
                    Console.Write(" ");
                    do  // Generate new food position
                    {
                        food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;
                }

                Console.SetCursorPosition(food.col, food.row);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("@");

                sleepTime -= 0.01;

                Thread.Sleep((int)sleepTime);
            }
            
        }
    }
}
