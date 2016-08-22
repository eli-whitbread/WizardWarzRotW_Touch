using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WizardWarzRotW
{
    /// <summary>
    /// Interaction logic for ExplosionRadiusControl.xaml
    /// </summary>
    public partial class ExplosionRadiusControl : UserControl
    {
        bool isHorizontal;
        SpritesheetImage explosionRadiusImg;
        BitmapImage horizRightImage, horizLeftImage, vertUpImage, vertDownImage, horizEndImage, vertEndImage, mySource;
        int myFrameX, myFrameY;

        public ExplosionRadiusControl(string dir)
        {
            InitializeComponent();
            horizRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/HorizontalExplosion_Right.png", UriKind.Absolute));
            horizLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/HorizontalExplosion_Left.png", UriKind.Absolute));
            vertUpImage = new BitmapImage(new Uri("pack://application:,,,/Resources/VerticalExplosion_Up.png", UriKind.Absolute));
            vertDownImage = new BitmapImage(new Uri("pack://application:,,,/Resources/VerticalExplosion_Down.png", UriKind.Absolute));
            horizEndImage = new BitmapImage(new Uri("pack://application:,,,/Resources/HorizontalExplosion_End.png", UriKind.Absolute));
            vertEndImage = new BitmapImage(new Uri("pack://application:,,,/Resources/VerticalExplosion_End.png", UriKind.Absolute));
            SetExplosionTileDirection(dir);
            Loaded += ExplosionRadiusControl_Loaded;
        }

        private void ExplosionRadiusControl_Loaded(object sender, RoutedEventArgs e)
        {
            Int32 tileSize = GameBoard.ReturnTileSize();

            explosionRadiusImg = new SpritesheetImage()
            {
                Source = mySource,
                FrameMaxX = myFrameX,
                FrameMaxY = myFrameY,
                FrameRate = 30,
                Width = tileSize,
                Height = tileSize,
                PlaysRemaining = 1,
                LoopForever = false,

            };
            explosionRadiusImg.AnimationComplete += (o, s) =>
            {
                if (isHorizontal == true)
                {
                    explosionRadiusImg.Source = horizEndImage;
                }
                else
                {
                    explosionRadiusImg.Source = vertEndImage;
                }
            };

            Point centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);

            Canvas.SetTop(explosionRadiusImg, centerPoint.Y - (tileSize / 2));
            Canvas.SetLeft(explosionRadiusImg, centerPoint.X - (tileSize / 2));

            myERCanvas.Children.Add(explosionRadiusImg);
        }

        public void SetExplosionTileDirection(string dir)
        {
            switch (dir)
            {
                case "Left":
                    mySource = horizLeftImage;
                    myFrameX = 3;
                    myFrameY = 1;
                    isHorizontal = true;
                    break;
                case "Right":
                    mySource = horizRightImage;
                    myFrameX = 3;
                    myFrameY = 1;
                    isHorizontal = true;
                    break;
                case "Up":
                    mySource = vertUpImage;
                    myFrameX = 1;
                    myFrameY = 3;
                    isHorizontal = false;
                    break;
                case "Down":
                    mySource = vertDownImage;
                    myFrameX = 1;
                    myFrameY = 3;
                    isHorizontal = false;
                    break;
                default:
                    break;
            }
        }
    }
}
