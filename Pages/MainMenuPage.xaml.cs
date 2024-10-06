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

namespace Battleship.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Player p1 = new Player(tbPlayer1.Text, 0);
            Player p2 = new Player(tbPlayer2.Text, 1);

            p1.OceanBoard.ShipLayer.PlaceShipsRandomly();
            p2.OceanBoard.ShipLayer.PlaceShipsRandomly();

            p1.OceanBoard.ShipLayer.RenderShips(); 
            p2.OceanBoard.ShipLayer.RenderShips();

            FleetSetupPage fleetSetupPage = new FleetSetupPage(p1, p2);

            this.NavigationService.Navigate(fleetSetupPage);
        }

        
    }
}
