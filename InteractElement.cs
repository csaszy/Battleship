using Battleship.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Battleship
{
    public class InteractElement : Button
    {
        private Vector _position;
        public Vector Position { get { return _position; } }

        public InteractElement(Vector position)
        {
            this._position = position;
        }
    }
}
