using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using Battleship;

namespace Battleship
{
    public enum ShipType
    {
        Carrier = 5,
        Battleship = 4,
        Cruiser = 3,
        Submarine = 3,
        Destroyer = 2
    }
    public enum ShipOrientation
    {
        Up = -90,
        Donw = 90,
        Left = 180,
        Right = 0
    }
    public class Ship : Button, ICloneable
    {
        private int _hits;
        public int Hits { 
            set 
            {
                _hits = value;
                if (this.Hits - (int)_shipType >= 0) this.IsSunken = true;
            }
            get { return _hits; }
        }
        public bool IsSunken = false;

        private ShipType _shipType;
        private Vector _positon;
        private ShipOrientation _orientation;
        private Vector _shipVector;
        private Vector _boardDimensions;
        private Matrix _rotationMatrix;

        public ShipType ShipType { get { return _shipType; } }
        public Vector Position
        {
            set 
            {
                Vector temp = _positon;
                _positon = value;
                if (!IsPositionValid()) 
                    _positon = temp;
            }
            get { return this._positon; }
        }
        public ShipOrientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }
        public Vector ShipVector
        {
            get 
            {
                return Vector.Multiply(_shipVector, this._rotationMatrix); 
            }
        }
        public Vector ShipBoardDimensions
        {
            get { return _boardDimensions; }
        }

        public Vector ShipTailPostion
        {
            get { return Vector.Add(_positon, ShipVector); }
        }
        public Vector ShipNosePostion
        {
            get { return _positon; }
        }

        public Ship(ShipType shipType, Vector position, ShipOrientation orientation, Vector boardDimensions)
        {
            this._shipType = shipType;
            this._positon = position;
            this._orientation = orientation;
            this._boardDimensions = boardDimensions;
            this.Hits = 0; //currently the ship does not know where it was hit (it's fine unless it wants to move mid game somehow...)

            this._shipVector = new Vector((int)this._shipType - 1, 0);
            this._rotationMatrix = new Matrix();
            this._rotationMatrix.Rotate((double) (int)this._orientation);

            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;

            this.Background = Brushes.DarkGray;
            this.Content = ((int)_shipType).ToString();
        }

        //it's dirty but it works (hopefully)
        public bool IsPositionValid()
        {
            //sihp's nose
            if (this.ShipNosePostion.X < 1 || this.ShipNosePostion.X > this._boardDimensions.X) return false;
            if (this.ShipNosePostion.Y < 1 || this.ShipNosePostion.Y > this._boardDimensions.Y) return false;

            //ship's tale
            if (this.ShipTailPostion.X < 1 || this.ShipTailPostion.X > this._boardDimensions.X) return false;
            if (this.ShipTailPostion.Y < 1 || this.ShipTailPostion.Y > this._boardDimensions.Y) return false;

            return true;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        } //for the ICloneable interface
    }
}
