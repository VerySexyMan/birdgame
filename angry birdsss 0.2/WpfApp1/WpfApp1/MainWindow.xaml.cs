using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    using System;

    class angry_birds
    {
        public double V_0;   // начальная скорость
        public double angle; // угол броска
        public double time; // интервал времени
        public double wall_distance; // нахождение стены от точки старта полета птички
        public double wall_height; // высота стены
        public double weight; // масса птицы
        public double coef; // коэффициент сопротивления воздуха
        public double g = 9.8;

        public void count()
        {
            double X = 0;
            double T = 0;
            double V_x = V_0 * Math.Cos(angle);
            double V_y = V_0 * Math.Sin(angle);
            double time_wall = wall_distance / (V_0 * Math.Cos(angle));
            double Y = 0;
            double max_Y = V_0 * Math.Sin(angle) * time_wall - g * Math.Pow(time_wall, 2) / 2;
            if (max_Y < wall_height)
            {
                //Console.Write("Птичка врезалась в стенку!");
            }
            else
            {
                while (Y >= 0)
                {
                    T = T + time;
                    V_x = V_x - time * (coef * V_x / weight);
                    V_y = V_y - time * (g + (coef * V_x / weight));
                    X = X + time * V_x;
                    Y = Y + time * V_y;
                    if (Y < 0)
                        break;
                    //Console.Write($"Длина: {X} м  Высота: {Y} м  Период: {T} с \n");
                }
            }
        }
    }

   /* class Program
    {
        public static void Main(string[] args)
        {
            var bird = new angry_birds();
            bird.V_0 = 20;
            bird.weight = 2;
            bird.angle = 45;
            bird.time = 0.5;
            bird.wall_distance = 10;
            bird.wall_height = 10;
            bird.coef = 0.3;
            bird.count();
        }
    }
    */
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool moveLeft, moveRight;
        List<Rectangle> itemstoremove = new List<Rectangle>();
        Random rand = new Random();

        int enemySpriteCounter; // int to help change enemy images
        int enemyCounter = 100; // enemy spawn time
        int limit = 50; // limit of enemy spawns
        int score = 0; // default score
        int missed = 0; // default missed
        Rect playerHitBox;


        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            MyCanvas.Focus();

            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri(@"C:\Users\Arkady.Aminov\Desktop\angry birdsss 0.1\WpfApp1\WpfApp1\images\background.png"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(1, 1, 1, 1);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            MyCanvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri(@"C:\Users\Arkady.Aminov\Desktop\angry birdsss 0.1\WpfApp1\WpfApp1\images\bird.png"));
            player.Fill = playerImage;
            

        }
        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            enemyCounter -= 1;


            scoreText.Content = "Score: " + score;
            missedText.Content = "Missed: " + missed;

            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            foreach (Rectangle y in itemstoremove)
            {
                // remove them permanently from the canvas
                MyCanvas.Children.Remove(y);
            }

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                // if any rectangle has the tag bullet in it
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    // move the bullet rectangle towards top of the screen
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + 5);
                    // make a rect class with the bullet rectangles properties
                    Rect bullet = new Rect(Canvas.GetRight(x), Canvas.GetTop(x), x.Width, x.Height);
                    // check if bullet has reached top part of the screen


                    if (Canvas.GetTop(x) < 10)
                    {
                        // if it has then add it to the item to remove list
                        itemstoremove.Add(x);
                        missed++;
                    }

                    if (Canvas.GetLeft(x) > 990)
                    {
                        // if it has then add it to the item to remove list
                        itemstoremove.Add(x);
                        missed++;
                    }


                    if (Canvas.GetTop(x) > 490)
                    {
                        // if it has then add it to the item to remove list
                        itemstoremove.Add(x);
                        missed++;
                    }

                    // run another for each loop inside of the main loop this one has a local variable called y
                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        // if y is a rectangle and it has a tag called enemy
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            // make a local rect called enemy and put the enemies properties into it
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                            // now check if bullet and enemy is colliding or not
                            // if the bullet is colliding with the enemy rectangle
                            if (bullet.IntersectsWith(enemyHit))
                            {
                                itemstoremove.Add(x); // remove bullet
                                itemstoremove.Add(y); // remove enemy
                                score++; // add one to the score
                            }
                        }
                    }
                }

            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Rectangle bullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 35,
                    Width = 35,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                ImageBrush bulletSprite = new ImageBrush();
                bulletSprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\Arkady.Aminov\Desktop\angry birdsss 0.1\WpfApp1\WpfApp1\images\bird.png"));
                bullet.Fill = bulletSprite;
                // place the bullet on top of the player location
                Canvas.SetTop(bullet, Canvas.GetTop(player));
                // place the bullet middle of the player image
                Canvas.SetLeft(bullet, Canvas.GetLeft(player));
                // add the bullet to the screen
                MyCanvas.Children.Add(bullet);
            }
        }
        
        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();

            enemySpriteCounter = rand.Next(1, 3);

            switch (enemySpriteCounter)
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\Arkady.Aminov\Desktop\angry birdsss 0.1\WpfApp1\WpfApp1\images\enemy1.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\Arkady.Aminov\Desktop\angry birdsss 0.1\WpfApp1\WpfApp1\images\enemy2.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\Arkady.Aminov\Desktop\angry birdsss 0.1\WpfApp1\WpfApp1\images\enemy3.png"));
                    break;
                default:
                    enemySprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\Arkady.Aminov\Desktop\angry birdsss 0.1\WpfApp1\WpfApp1\images\enemy1.png"));
                    break;
            }

            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 50,
                Width = 50,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, rand.Next(100, 400));
            Canvas.SetLeft(newEnemy, rand.Next(300, 900));
            MyCanvas.Children.Add(newEnemy);

            GC.Collect();
        }
    }
}
