using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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
    public enum GameStates
    {
        Title,
        MainMenu,
        PlayerSelect,
        Game,
        EndScreen
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameStates curGameState = GameStates.Title;
        

        public MainWindow()
        {
            InitializeComponent();
        }
              

        public static void ChangeGameState(GameStates curGameState)
        {
            switch (curGameState)
            {
                case (GameStates.EndScreen):
                    curGameState = GameStates.EndScreen;
                    break;
            }
        }
    }
}
