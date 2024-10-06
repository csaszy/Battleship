using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Battleship.Pages
{
    /// <summary>
    /// Interaction logic for MainGamePage.xaml
    /// </summary>
    public partial class MainGamePage : Page
    {
        private Player _currentPlayer;
        private Player _upcommingPlayer;
        private Player _winnerPlayer;

        private bool _gameOver = false;

        private int RoundCounter;

        public MainGamePage(Player p1, Player p2)
        {
            //good for now
            _currentPlayer = p1;
            _upcommingPlayer = p2;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //sets the event handlers for the buttons (kudos to arongeo)
            foreach (InteractElement ite in _currentPlayer.TargetBoard.InteractLayer.InteractElements)
            {
                ite.Click += new RoutedEventHandler(InteractLayer_Button_OnClick);
            }
            foreach (InteractElement ite in _upcommingPlayer.TargetBoard.InteractLayer.InteractElements)
            {
                ite.Click += new RoutedEventHandler(InteractLayer_Button_OnClick);
            }

            //disable the continue button
            btnContinue.IsEnabled = false;
            btnContinue.Opacity = 0;

            SetupNextRound();
        }

        private void SetupNextRound()
        {

            RoundCounter++;

            if (RoundCounter > 1) //because this funciton loads the first round too
            {
                MessageBox.Show("Nem talált :(");

                gBody.Children.Clear();

                MessageBox.Show(
                $"[{_currentPlayer.Name}] nézz félre! [{_upcommingPlayer.Name}] következik! \n " +
                $"Az OK gombra kattintva folytatódik a játék");

                Player temp = _currentPlayer;
                _currentPlayer = _upcommingPlayer;
                _upcommingPlayer = temp;
            }

            //setting up textboxes
            tbPlayerName.Text = _currentPlayer.Name;
            tbRoundCounter.Text = RoundCounter.ToString();

            LoadPlayerBoards(_currentPlayer);
        }

        private void LoadPlayerBoards(Player player)
        {
            gBody.Children.Clear();
            
            gBody.Children.Add(player.OceanBoard);
            Grid.SetColumn(player.OceanBoard, 1);
            Grid.SetRow(player.OceanBoard, 1);

            gBody.Children.Add(player.TargetBoard);
            Grid.SetColumn(player.TargetBoard, 0);
            Grid.SetRow(player.TargetBoard, 1);
        }

        private bool IsGameOver()
        {
            if(_upcommingPlayer.OceanBoard.ShipLayer.SunkenShips.Count() >= _upcommingPlayer.OceanBoard.ShipLayer.Ships.Count()) //all ships sunk
                return true; 
            return false;
        }
        private bool IsThereNewSunkenShip()
        {
            foreach (Ship ship in this._upcommingPlayer.OceanBoard.ShipLayer.Ships)
            {
                if (ship.IsSunken && !this._upcommingPlayer.OceanBoard.ShipLayer.SunkenShips.Contains(ship))
                {
                    this._upcommingPlayer.OceanBoard.ShipLayer.SunkenShips.Add(ship);

                    MessageBox.Show($"Sikeresen elsüllyesztetted [{_upcommingPlayer.Name}] {ship.ShipType} típusú hajóját.", "Hajó süllyed!");

                    return true;
                }
            }

            return false;
        }
        
        private void InteractLayer_Button_OnClick(object sender, RoutedEventArgs e)
        {
            if(_gameOver) return;

            bool currentPlayerHasMissed = this._currentPlayer.Attack((sender as InteractElement).Position, _upcommingPlayer);
            
            if (currentPlayerHasMissed)
                SetupNextRound();

            IsThereNewSunkenShip(); //probably not the cleanest solution but it works

            //we have to check for game over after the attack 
            if (IsGameOver())
            {
                _gameOver = true;
                MessageBox.Show("Game over!");

                _winnerPlayer = _currentPlayer;

                btnContinue.Opacity = 1; btnContinue.IsEnabled = true;

                return;
            }

        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            GameOverPage gop = new GameOverPage(_winnerPlayer);
            this.NavigationService.Navigate(gop);
        }
    }
}
