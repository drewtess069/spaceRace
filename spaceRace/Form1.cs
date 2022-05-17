using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Media;

namespace spaceRace
{
    //Space Race Game -- Drew Tessmer, May 17
    //This is a game adaptation of the 1973 video game - Space Race

    public partial class Form1 : Form
    {
        //Set up player rectangles ot track throughout game
        Rectangle player1 = new Rectangle(240, 550, 20, 30);
        Rectangle player2 = new Rectangle(340, 550, 20, 30);

        //Set up rectangle that slowly gets smaller to show time left
        Rectangle timeLeft = new Rectangle(297, 0, 6, 600);

        //Sounds for asteroid hitting rocket and player scoring point
        SoundPlayer crashPlayer = new SoundPlayer(Properties.Resources.asteroidCollision);
        SoundPlayer pointPlayer = new SoundPlayer(Properties.Resources.ding);

        //Set up random generator for asteroid speeds and y values of asteroids
        Random randGen = new Random();

        //Set up global variables
        int p1Score = 0;
        int p2Score = 0;

        int playerSpeed = 8;

        int asteroidHeight = 15;
        int asteroidWidth = 25;

        int timerTick;
        int countdown = 3;

        string gameState = "waiting";

        bool upDown = false;
        bool downDown = false;
        bool wDown = false;
        bool sDown = false;

        //Set up asteroids
        List<Rectangle> leftSide = new List<Rectangle>();
        List<Rectangle> rightSide = new List<Rectangle>();

        //Set up lists to hold random speeds
        List<int> leftSpeed = new List<int>();
        List<int> rightSpeed = new List<int>();

        SolidBrush whiteBrush = new SolidBrush(Color.White);

        //Set up images for rocket and asteroids
        Image rocketImage = Properties.Resources.rocketSmall;
        Image asteroid = Properties.Resources.asteroid;

        public Form1()
        {
            InitializeComponent();
        }

        //Run following code on game start, sets up variables and clears lists
        public void GameInitialize()
        {
            titleLabel.Text = "";
            subTitleLabel.Text = "";

            gameTimer.Enabled = true;
            gameState = "running";
            leftSide.Clear();
            rightSide.Clear();
            leftSpeed.Clear();
            rightSpeed.Clear();

            p1Score = 0;
            p2Score = 0;
            timerTick = 0;
            countdown = 3;

            timeLeft.Location = new Point(timeLeft.X, 0);

            player1.X = 240;
            player1.Y = 550;

            player2.X = 340;
            player2.Y = 550;

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Set up start and end screens
            if (gameState == "waiting")
            {
                p1ScoreLabel.Text = "";
                p2ScoreLabel.Text = "";

                titleLabel.Text = "Space Race";
                subTitleLabel.Text = "Press space to begin or press escape to exit";
            }
            else if (gameState == "running")
            {
                //Track players and asteroids
                //Display players scores which increase every time they reach the top of the screen

                p1ScoreLabel.Text = $"{p1Score}";
                p2ScoreLabel.Text = $"{p2Score}";

                e.Graphics.DrawImage(rocketImage, player1);
                e.Graphics.DrawImage(rocketImage, player2);
                if (countdown == -1)
                {
                    e.Graphics.FillRectangle(whiteBrush, timeLeft);
                }
                for (int i = 0; i < leftSide.Count(); i++)
                {
                    e.Graphics.DrawImage(asteroid, leftSide[i]);
                }

                for (int i = 0; i < rightSide.Count(); i++)
                {
                    e.Graphics.DrawImage(asteroid, rightSide[i]);
                }
            }
            else if (gameState == "over")
            {
                p1ScoreLabel.Text = "";
                p2ScoreLabel.Text = "";

                if (p1Score > p2Score)
                {
                    titleLabel.Text = $"Player 1 wins!\n{p1Score} - {p2Score}";

                }
                else if (p2Score > p1Score)
                {
                    titleLabel.Text = $"Player 2 wins!\n{p2Score} - {p1Score}";
                }
                else
                {
                    titleLabel.Text = $"Tie Game!\n{p1Score} - {p2Score}";
                }
                subTitleLabel.Text = "Press space to play again or press escape to exit";
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Set up keys to track player inputs

            switch (e.KeyCode)
            {
                case Keys.Up:
                    upDown = true;
                    break;
                case Keys.Down:
                    downDown = true;
                    break;
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.Space:
                    if (gameState == "waiting" || gameState == "over" || gameState == "win")
                    {
                        GameInitialize();
                    }
                    break;
                case Keys.Escape:
                    if (gameState == "waiting" || gameState == "over" || gameState == "win")
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //Set up keys to track player inputs

            switch (e.KeyCode)
            {
                case Keys.Up:
                    upDown = false;
                    break;
                case Keys.Down:
                    downDown = false;
                    break;
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            timerTick++;

            //Clear label at end of countdown
            if (timerTick % 50 == 0 && countdown == -1)
            {
                titleLabel.Text = "";
            }

            //Display countdown
            if (timerTick % 50 == 0)
            {
                if (countdown == 3)
                {
                    countdown--;
                    titleLabel.Text = "3";
                }
                else if (countdown == 2)
                {
                    countdown--;
                    titleLabel.Text = "2";
                }
                else if (countdown == 1)
                {
                    countdown--;
                    titleLabel.Text = "1";
                }
                else if (countdown == 0)
                {
                    countdown--;
                    titleLabel.Text = "START";
                }
            }

            //Set up rectangle that decreases as game is played, displays amount of time left
            if (countdown == -1)
            {
                if (timerTick % 5 == 0)
                {
                    timeLeft.Location = new Point(timeLeft.X, timeLeft.Y + 3);

                    if (timeLeft.Y >= 600)
                    {
                        gameTimer.Enabled = false;
                        gameState = "over";
                    }
                }
                //Set up player movements, only allow them to move when countdown is done
                if (wDown == true && player1.Y >= 0)
                {
                    player1.Y -= playerSpeed;
                }
                else if (sDown == true && player1.Y < this.Height - player1.Height)
                {
                    player1.Y += playerSpeed;
                }

                if (upDown == true && player2.Y >= 0)
                {
                    player2.Y -= playerSpeed;
                }
                else if (downDown == true && player2.Y < this.Height - player1.Height)
                {
                    player2.Y += playerSpeed;
                }
            }

            //Check if a player scores, add a point to score label if they do
            if (player1.Y <= 0)
            {
                p1Score++;
                player1.Y = 550;

                //play point sound
                pointPlayer.Play();
            }
            if (player2.Y <= 0)
            {
                p2Score++;
                player2.Y = 550;

                //play point sound
                pointPlayer.Play();
            }

            //Move asteroids on left side based on random speed
            for (int i = 0; i < leftSide.Count(); i++)
            {
                leftSpeed.Add(randGen.Next(3, 10));

                //find the new postion of x based on speed 
                int x = leftSide[i].X + leftSpeed[i];
                int y = randGen.Next(0, this.Height - asteroidHeight - 10);

                //replace the rectangle in the list with updated one using new y 
                leftSide[i] = new Rectangle(x, leftSide[i].Y, asteroidWidth, asteroidHeight);
            }

            //Move asteroids on right side based on random speed
            for (int i = 0; i < rightSide.Count(); i++)
            {
                rightSpeed.Add(randGen.Next(3, 10));

                //find the new postion of x based on speed 

                int x = rightSide[i].X - rightSpeed[i];
                int y = randGen.Next(0, 520);

                //replace the rectangle in the list with updated one using new y 
                rightSide[i] = new Rectangle(x, rightSide[i].Y, asteroidWidth, asteroidHeight);
            }

            int spawnTimer = randGen.Next(8, 15);

            //Spawn new asteroids to left side at random timing intervals
            if (timerTick % spawnTimer == 0)
            {
                int y = randGen.Next(0, 520);

                leftSide.Add(new Rectangle(0 - asteroidWidth, y, asteroidWidth, asteroidHeight));
            }

            //Spawn new asteroids to right side at random timing intervals
            if (timerTick % spawnTimer == 0)
            {
                int y = randGen.Next(0, 520);

                rightSide.Add(new Rectangle(900, y, asteroidWidth, asteroidHeight));
            }

            //Remove left asteroids from lists if they go off the screen 
            for (int i = 0; i < leftSide.Count(); i++)
            {
                if (leftSide[i].X >= this.Width)
                {
                    leftSide.RemoveAt(i);
                    leftSpeed.RemoveAt(i);
                }
            }

            //Remove right asteroids from lists if they go off the screen 
            for (int i = 0; i < rightSide.Count(); i++)
            {
                if (rightSide[i].Y <= 0 - asteroidWidth)
                {
                    rightSide.RemoveAt(i);
                    rightSpeed.RemoveAt(i);
                }
            }

            //Set up player collisions with asteroids and play sound
            for (int i = 0; i < leftSide.Count(); i++)
            {
                if (player1.IntersectsWith(leftSide[i]))
                {
                    player1.Y = 550;

                    crashPlayer.Play();
                }
            }
            for (int i = 0; i < rightSide.Count(); i++)
            {
                if (player1.IntersectsWith(rightSide[i]))
                {
                    player1.Y = 550;

                    crashPlayer.Play();
                }
            }
            for (int i = 0; i < leftSide.Count(); i++)
            {
                if (player2.IntersectsWith(leftSide[i]))
                {
                    player2.Y = 550;

                    crashPlayer.Play();
                }
            }
            for (int i = 0; i < rightSide.Count(); i++)
            {
                if (player2.IntersectsWith(rightSide[i]))
                {
                    player2.Y = 550;

                    crashPlayer.Play();
                }
            }
            Refresh();
        }
    }
}
