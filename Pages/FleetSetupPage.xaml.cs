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
    /// Interaction logic for FleetSetupPage.xaml
    /// </summary>
    public partial class FleetSetupPage : Page
    {
        Player Player1, Player2;
        int _visitCount = 0; //hate naming shit

        public FleetSetupPage(Player p1, Player p2)
        {
            InitializeComponent();
            Player1 = p1;
            Player2 = p2;
        }

        public void Setup(Player player)
        {
            tbPlayerName.Text = player.Name;

            gBody.Children.Add(player.OceanBoard);
            Grid.SetColumn(player.OceanBoard, 1);
            Grid.SetRow(player.OceanBoard, 0);
            Grid.SetRowSpan(player.OceanBoard, 1);

        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            Setup(this.Player2);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainGamePage mainGamePage = new MainGamePage(this.Player1, this.Player2);
            this.NavigationService.Navigate(mainGamePage);
            return;
            Setup(this.Player1);
        }
    }
}
