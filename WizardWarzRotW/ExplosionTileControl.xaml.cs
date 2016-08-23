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
    /// Interaction logic for ExplosionTileControl.xaml
    /// </summary>
    public partial class ExplosionTileControl : UserControl
    {
        SpritesheetImage explosionTile;

        public ExplosionTileControl()
        {
            InitializeComponent();
            Loaded += ExplosionTileControl_Loaded;
        }

        private void ExplosionTileControl_Loaded(object sender, RoutedEventArgs e)
        {
            explosionTile = new SpritesheetImage()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/CentreExplosion.png", UriKind.Absolute)),
                FrameMaxX = 3,
                FrameMaxY = 1,
                FrameRate = 30,
                Width = 64,
                Height = 64,
                PlaysRemaining = 1,
                LoopForever = false,

            };
            explosionTile.AnimationComplete += (o, s) =>
            {
                explosionTile.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/CentreExplosionEnd.png", UriKind.Absolute));
            };

            myExplosionCanvas.Children.Add(explosionTile);
        }
    }
}
