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
    /// Interaction logic for BombDroppedControl.xaml
    /// </summary>
    public partial class BombDroppedControl : UserControl
    {

        
        public BombDroppedControl()
        {
            InitializeComponent();
            Loaded += BombDroppedControl_Loaded;
           
        }

        private void BombDroppedControl_Loaded(object sender, RoutedEventArgs e)
        {
            Int32 tileSize = GameBoard.ReturnTileSize();

            SpritesheetImage bombTile = new SpritesheetImage()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/BombSheet_4step.png", UriKind.Absolute)),
                FrameMaxX = 4,
                FrameMaxY = 1,
                FrameRate = 10,
                Width = tileSize,
                Height = tileSize,
                PlaysRemaining = 10,
                LoopForever = true,

            };
            bombTile.AnimationComplete += (o, s) =>
            {
                myBombCanvas.Children.Remove(bombTile);
            };

            Point centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);

            Canvas.SetTop(bombTile, centerPoint.Y - (tileSize / 2));
            Canvas.SetLeft(bombTile, centerPoint.X - (tileSize / 2));
            myBombCanvas.Children.Add(bombTile);
        }
    }
}
