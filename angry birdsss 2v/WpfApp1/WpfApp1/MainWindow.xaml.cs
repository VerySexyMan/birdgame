using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using Application = System.Windows.Application;

namespace WpfApp1
{
    using System;

    class AngryBirds
    {
        public double g = 9.8;

        public double BirdX { get; set; }
        public double BirdY { get; set; }
        private int _startSpeed = 0;
        private double _time = 0;
        private double _startX;
        private double _startY;
        private double _startAngle;
        private bool _launched = false;


        public void Update()
        {
            if (!_launched)
                return;
            _time += 0.05 * 20; // 20 ms
            var startSpeedX = _startSpeed * Math.Cos(_startAngle * Math.PI / 180); // rad to degrees
            var startSpeedY = _startSpeed * Math.Sin(_startAngle * Math.PI / 180);

            BirdX = _startX + startSpeedX * _time;
            BirdY = _startY - (startSpeedY * _time - g * _time * _time / 2);
        }

        public void LaunchBird(int startSpeed, double angle, double startX, double startY)
        {
            _startSpeed = startSpeed;
            _startX = startX;
            _startY = startY;
            _startAngle = angle;
            _launched = true;
        }

        public void StopLaunch()
        {
            _launched = false;
            _time = 0;
        }
    }

    public partial class MainWindow : Window
    {
        Timer gameTimer = new Timer();
        bool moveLeft, moveRight;
        List<Rectangle> itemstoremove = new List<Rectangle>();
        Random rand = new Random();

        int enemySpriteCounter; // int to help change enemy images
        int enemyCounter = 300; // enemy spawn time
        int limit = 50; // limit of enemy spawns
        int score = 0; // default score
        int missed = 0; // default missed

        private string _basePath = System.IO.Directory.GetCurrentDirectory();
        private AngryBirds _physics = new AngryBirds();
        private const int delay = 20;

        private int _currentSpeed = 80;
        private int _currentAngle = 45;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = delay;
            gameTimer.Elapsed += GameLoop;
            gameTimer.Start();

            MyCanvas.Focus();

            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri($@"{_basePath}\images\background.png"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(1, 1, 1, 1);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            MyCanvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri($@"{_basePath}\images\bird.png"));
            player.Fill = playerImage;

            Angle.Value = _currentAngle;
            Speed.Value = _currentSpeed;

            AngleLabel.Content = $"Угол: {Angle.Value}";
            SpeedLabel.Content = $"Скорость: {Speed.Value}";
        }

        private void GameLoop(object sender, EventArgs e)
        {
            enemyCounter -= 1;

            _physics.Update();

            if (Application.Current == null)
                return;
            Application.Current.Dispatcher.Invoke(() =>
            {
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

                var x = MyCanvas.Children.OfType<Rectangle>().FirstOrDefault(el => (string)el.Tag == "bullet");

                if (x == null)
                    return;

                // move the bullet rectangle towards top of the screen
                Canvas.SetLeft(x, _physics.BirdX);
                Canvas.SetTop(x, _physics.BirdY);
                // check if bullet has reached top part of the screen


                if (Canvas.GetTop(x) < 10)
                {
                    // if it has then add it to the item to remove list
                    itemstoremove.Add(x);
                    missed++;
                    _physics.StopLaunch();
                }
                else if (Canvas.GetLeft(x) > 990)
                {
                    // if it has then add it to the item to remove list
                    itemstoremove.Add(x);
                    missed++;
                    _physics.StopLaunch();
                }
                else if (Canvas.GetTop(x) > 490)
                {
                    // if it has then add it to the item to remove list
                    itemstoremove.Add(x);
                    missed++;
                    _physics.StopLaunch();
                }
                else
                {
                    // run another for each loop inside of the main loop this one has a local variable called y
                    var bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    foreach (var y in MyCanvas.Children.OfType<Rectangle>()
                                 .Where(el => (string)el.Tag == "enemy"))
                    {
                        // make a local rect called enemy and put the enemies properties into it
                        var enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                        // now check if bullet and enemy is colliding or not
                        // if the bullet is colliding with the enemy rectangle
                        // make a rect class with the bullet rectangles properties
                        if (bullet.IntersectsWith(enemyHit))
                        {
                            itemstoremove.Add(x); // remove bullet
                            itemstoremove.Add(y); // remove enemy
                            score++; // add one to the score
                            _physics.StopLaunch();
                        }
                    }
                }
            });
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
                    Fill = Brushes.White
                };

                ImageBrush bulletSprite = new ImageBrush();
                bulletSprite.ImageSource = new BitmapImage(new Uri($@"{_basePath}\images\bird.png"));
                bullet.Fill = bulletSprite;
                // place the bullet on top of the player location
                Canvas.SetTop(bullet, Canvas.GetTop(player));
                // place the bullet middle of the player image
                Canvas.SetLeft(bullet, Canvas.GetLeft(player));
                // add the bullet to the screen
                MyCanvas.Children.Add(bullet);

                _physics.LaunchBird(_currentSpeed, _currentAngle, Canvas.GetLeft(player), Canvas.GetTop(player));
            }
        }

        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();

            enemySpriteCounter = rand.Next(1, 3);

            switch (enemySpriteCounter)
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri($@"{_basePath}\images\enemy1.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri($@"{_basePath}\images\enemy2.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri($@"{_basePath}\images\enemy3.png"));
                    break;
                default:
                    enemySprite.ImageSource = new BitmapImage(new Uri($@"{_basePath}\images\enemy1.png"));
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

            //GC.Collect();
        }

        private void Speed_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SpeedLabel != null)
                SpeedLabel.Content = $"Скорость: {(int)Speed.Value}";
            _currentSpeed = (int)Speed.Value;
        }

        private void Angle_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AngleLabel != null)
                AngleLabel.Content = $"Угол: {(int)Angle.Value}";
            _currentAngle = (int)Angle.Value;
        }
    }
}