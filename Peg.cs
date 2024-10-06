using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Battleship
{
    public enum PegType
    {
        Red = 1, 
        Grey = 2, 
        Transparent = 0
    }
    public class Peg : Button
    {
        public Vector Position;
        private PegType _type;

        public Peg(Vector position, PegType type)
        {
            this.Position = position;
            this._type = type;

            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;

            this.Opacity = 1;
            this.Margin = new Thickness(5, 5, 5, 5);

            //it's fine
            switch (_type)
            {
                case PegType.Red:
                    this.Background = Brushes.IndianRed;       
                    break;
                case PegType.Grey:
                    this.Background = Brushes.Gray;
                    break;
                case PegType.Transparent:
                    this.Background = Brushes.Transparent;
                    break;
            }

        }
    }
}
