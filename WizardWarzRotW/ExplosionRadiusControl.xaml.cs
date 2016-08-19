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
        BitmapImage horizRightImage, horizLeftImage, vertUpImage, vertDownImage, horizEndImage, vertEndImage;

        public ExplosionRadiusControl()
        {
            InitializeComponent();
            horizRightImage = new BitmapImage(new Uri("pack://application:,,,/Resources/HorizontalExplosion_Right.png", UriKind.Absolute));
            horizLeftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/HorizontalExplosion_Left.png", UriKind.Absolute));
            vertUpImage = new BitmapImage(new Uri("pack://application:,,,/Resources/VerticalExplosion_Up.png", UriKind.Absolute));
            vertDownImage = new BitmapImage(new Uri("pack://application:,,,/Resources/VerticalExplosion_Down.png", UriKind.Absolute));
            horizEndImage = new BitmapImage(new Uri("pack://application:,,,/Resources/HorizontalExplosion_End.png", UriKind.Absolute));
            vertEndImage = new BitmapImage(new Uri("pack://application:,,,/Resources/VerticalExplosion_End.png", UriKind.Absolute));
            Loaded += ExplosionRadiusControl_Loaded;
        }

        private void ExplosionRadiusControl_Loaded(object sender, RoutedEventArgs e)
        {
            explosionRadiusImg = new SpritesheetImage()
            {
                Source = horizRightImage,
                FrameMaxX = 3,
                FrameMaxY = 1,
                FrameRate = 30,
                Width = 64,
                Height = 64,
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

            SetExplosionTileDirection("Right");

            myERCanvas.Children.Add(explosionRadiusImg);
        }

        public void SetExplosionTileDirection(string dir)
        {
            switch (dir)
            {
                case "Left":
                    explosionRadiusImg.Source = horizLeftImage;
                    isHorizontal = true;
                    break;
                case "Right":
                    explosionRadiusImg.Source = horizRightImage;
                    isHorizontal = true;
                    break;
                case "Up":
                    explosionRadiusImg.Source = vertUpImage;
                    explosionRadiusImg.FrameMaxX = 1;
                    explosionRadiusImg.FrameMaxY = 3;
                    isHorizontal = false;
                    break;
                case "Down":
                    explosionRadiusImg.Source = vertDownImage;
                    explosionRadiusImg.FrameMaxX = 1;
                    explosionRadiusImg.FrameMaxY = 3;
                    isHorizontal = false;
                    break;
                default:
                    break;
            }
        }
    }
}
