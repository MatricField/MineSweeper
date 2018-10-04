using MineSweep.Model;
using MineSweeper.ViewModel;
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

namespace MineSweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand RightClickCommand = new RoutedCommand();

        private Game Game => (Game)DataContext;

        public StopwatchViewModel StopwatchViewModel { get; }

        public MainWindow()
        {
            StopwatchViewModel = new StopwatchViewModel();
            InitializeComponent();
            Game.GameWon += GameEndedHandler;
            Game.Exploded += GameEndedHandler;
        }

        private void GameEndedHandler(object sender, EventArgs e)
        {
            StopwatchViewModel.Stop();
        }

        private void GameReset_Click(object sender, RoutedEventArgs e)
        {
            Game.Reset();
            StopwatchViewModel.Reset();
        }

        private void PreStart_click(object sender, RoutedEventArgs e)
        {
            var x = Grid.GetRow((UIElement)sender);
            var y = Grid.GetColumn((UIElement)sender);
            Game.Initialize(x, y);
            StopwatchViewModel.Start();
        }

        private void Unexplored_Click(object sender, RoutedEventArgs e)
        {
            var x = Grid.GetRow((UIElement)sender);
            var y = Grid.GetColumn((UIElement)sender);
            Game.Explore(x, y);
        }

        private void RightClick_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var x = Grid.GetRow((UIElement)sender);
            var y = Grid.GetColumn((UIElement)sender);
            Game.Mark(x, y);
        }

        private void AlwaysCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void SetDifficulty_Clicked(object sender, RoutedEventArgs e)
        {
            Game.Reset();
            SetDifficulty(MapMenuNameToDifficulty((MenuItem)sender));
        }

        private DifficultyEnum MapMenuNameToDifficulty(MenuItem menuitem)
        {
            switch (menuitem.Name)
            {
                case "Menu_Beginner":
                    return DifficultyEnum.Beginner;
                case "Menu_Intermediate":
                    return DifficultyEnum.Intermediate;
                case "Menu_Expert":
                    return DifficultyEnum.Expert;
                default:
                    throw new InvalidOperationException();
            };
        }

        private void SetDifficulty(DifficultyEnum difficulty)
        {
            switch(difficulty)
            {
                case DifficultyEnum.Beginner:
                    Game.ChangeDifficulty(in Difficulty.Beginner);
                    break;
                case DifficultyEnum.Intermediate:
                    Game.ChangeDifficulty(in Difficulty.Intermediate);
                    break;
                case DifficultyEnum.Expert:
                    Game.ChangeDifficulty(in Difficulty.Expert);
                    break;
                case DifficultyEnum.Custom:
                    throw new NotImplementedException();
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
