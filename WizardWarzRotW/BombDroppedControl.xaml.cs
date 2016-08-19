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
            SpritesheetImage bombTile = new SpritesheetImage()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/BombSheet_4step.png", UriKind.Absolute)),
                FrameMaxX = 4,
                FrameMaxY = 1,
                FrameRate = 10,
                Width = 64,
                Height = 64,
                PlaysRemaining = 10,
                LoopForever = true,

            };
            bombTile.AnimationComplete += (o, s) =>
            {
                myBombCanvas.Children.Remove(bombTile);
            };

            myBombCanvas.Children.Add(bombTile);
        }
    }
}
