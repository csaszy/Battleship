using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Battleship;

namespace Battleship
{
    public abstract class Layer : Grid
    {
        public static Random RND = new Random();

        protected Vector _dimensions;
        protected Layer(Vector dimensions)
        {
            this._dimensions = dimensions;

            //creating rows and cols
            for (int i = 0; i < _dimensions.Y; i++)
            {
                this.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < _dimensions.X; i++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
    }
    public class BaseLayer : Layer
    {
        public BaseLayer(Vector dimensions) : base(dimensions)
        {
            //letters
            byte l = 0x41;
            for (int i = 0; i < _dimensions.Y - 1; i++)
            {
                TextBlock tb = new TextBlock()
                {
                    Text = Convert.ToChar(l++).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                };
                this.Children.Add(tb);
                Grid.SetColumn(tb, i + 1);
                Grid.SetRow(tb, 0);
            }
            for (int i = 1; i < _dimensions.X; i++)
            {
                TextBlock tb = new TextBlock()
                {
                    Text = i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                };
                this.Children.Add(tb);
                Grid.SetRow(tb, i);
                Grid.SetColumn(tb, 0);
            }

            //buttons to make nice cell borders
            for (int x = 1; x < _dimensions.X; x++)
            {
                for (int y = 1; y < _dimensions.Y; y++)
                {
                    Button btn = new Button()
                    {
                        Opacity = .8,
                        IsEnabled = false,
                    };

                    this.Children.Add(btn);
                    Grid.SetColumn(btn, x);
                    Grid.SetRow(btn, y);
                }
            }
        }
    }
    public class ShipLayer : Layer
    {
        public List<Ship> Ships;
        public List<Ship> SunkenShips;

        private int[,] _boardMatrix;
        public int[,] BoardMatrix
        {
            get { return _boardMatrix; }
        }

        public ShipLayer(Vector dimensions) : base(dimensions)
        {
            this.Ships = new List<Ship>();
            this.SunkenShips = new List<Ship>();

            this._boardMatrix = new int[(int)_dimensions.Y, (int)_dimensions.X];
        }
        public void PlaceShipsRandomly()
        {
            var orientationTypes = Enum.GetValues(typeof(ShipOrientation));

            foreach (ShipType shiptype in Enum.GetValues(typeof(ShipType))) 
            {
                Ship ship = new Ship(shiptype, new Vector(0,0), (ShipOrientation)orientationTypes.GetValue(RND.Next(orientationTypes.Length)), _dimensions); 
                while (!PlaceShip(ship)) 
                { 
                    ship.Position = new Vector(
                        RND.Next(1 + (int)Math.Abs(Math.Min(0, ship.ShipVector.X)), (int)(_dimensions.X - Math.Max(0, ship.ShipVector.X))),     //X
                        RND.Next(1 + (int)Math.Abs(Math.Min(0, ship.ShipVector.Y)), (int)(_dimensions.Y - Math.Max(0, ship.ShipVector.Y)))      //Y
                        ); 
                }
            }
        }

        private bool PlaceShip(Ship ship)
        {
            //updating boardMatrix
            foreach (Ship s in Ships)
            {
                Vector shipStart = s.ShipNosePostion;
                Vector shipEnd = s.ShipTailPostion;

                if (s.ShipVector.X < 0 || s.ShipVector.Y < 0)
                {
                    shipStart = s.ShipTailPostion;
                    shipEnd = s.ShipNosePostion;
                }
                for (int x = (int)shipStart.X; x <= (int)shipEnd.X; x++)
                {
                    for (int y = (int)shipStart.Y; y <= (int)shipEnd.Y; y++)
                    {
                        _boardMatrix[y, x] = 1;
                    }
                }
            };

            //checking if ship position is valid
            if (!ship.IsPositionValid()) return false;
            
            try
            {
                Vector shipStart = ship.ShipNosePostion;
                Vector shipEnd = ship.ShipTailPostion;

                if (ship.ShipVector.X < 0 || ship.ShipVector.Y < 0)
                {
                    shipStart = ship.ShipTailPostion;
                    shipEnd = ship.ShipNosePostion;
                }
                for (int x = (int)shipStart.X; x <= (int)shipEnd.X; x++)
                {
                    for (int y = (int)shipStart.Y; y <= (int)shipEnd.Y; y++)
                    {
                        if (_boardMatrix[y, x] == 1) return false; //if not -> return early
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            
            Ships.Add(ship);

            return true;
        }
        public void RenderShips()
        {
            this.Children.Clear();
            foreach (Ship ship in this.Ships)
            {
                try
                {
                    this.Children.Add(ship);

                    Vector startDrawingShipFrom = ship.ShipNosePostion;
                    Vector endDrawingShipAt = ship.ShipTailPostion;

                    if(ship.ShipVector.X < 0 || ship.ShipVector.Y < 0)
                    {
                        startDrawingShipFrom = ship.ShipTailPostion;
                        endDrawingShipAt = ship.ShipNosePostion;
                    }

                    Grid.SetRow(ship, (int)startDrawingShipFrom.Y);
                    Grid.SetColumn(ship, (int)startDrawingShipFrom.X);

                    int rowSpanAmount = (int)endDrawingShipAt.Y - (int)startDrawingShipFrom.Y + 1;
                    int colSpanAmount = (int)endDrawingShipAt.X - (int)startDrawingShipFrom.X + 1;
                    if(rowSpanAmount > 0) Grid.SetRowSpan(ship, rowSpanAmount);
                    if(colSpanAmount > 0) Grid.SetColumnSpan(ship, colSpanAmount);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ship trying to render out of bounds :/");
                }
            }
        }
        public Ship FindShipAt(Vector coords)
        {
            foreach (Ship ship in Ships)
            {
                //couldn't figure out a cool math way of doing it :(
                Vector shipStart = ship.ShipNosePostion;
                Vector shipEnd = ship.ShipTailPostion;

                if (ship.ShipVector.X < 0 || ship.ShipVector.Y < 0)
                {
                    shipStart = ship.ShipTailPostion;
                    shipEnd = ship.ShipNosePostion;
                }
                for (int x = (int)shipStart.X; x <= (int)shipEnd.X; x++)
                {
                    for (int y = (int)shipStart.Y; y <= (int)shipEnd.Y; y++)
                    {
                        if (Vector.Equals(coords, new Vector(x, y)))
                            return ship;
                    }
                }
            }
            return null;
        }
    }
    public class PegLayer : Layer
    {
        private List<Peg> _pegs;
        public PegLayer(Vector dimensions) : base(dimensions)
        {
            this._pegs = new List<Peg>();
        }

        public bool PlacePeg(Vector position, PegType pegType)
        {
            if (position == null) return false;

            //TODO! Return false if position isn't valid

            Peg peg = new Peg(position, pegType);
            _pegs.Add(peg);

            RenderPegs();
            
            return true;
        }

        public void RenderPegs()
        {
            this.Children.Clear();
            foreach (Peg peg in this._pegs)
            {
                try
                {
                    this.Children.Add(peg);
                    Grid.SetRow(peg, (int)peg.Position.Y);
                    Grid.SetColumn(peg, (int)peg.Position.X);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't render peg");
                }
            }
        }
    }
    public class InteractLayer : Layer
    {
        public List<InteractElement> InteractElements;

        public InteractLayer(Vector dimensions) : base(dimensions)
        {
            this.InteractElements = new List<InteractElement>();

            //placing InteractionElements
            for (int x = 1; x < _dimensions.X; x++)
            {
                for (int y = 1; y < _dimensions.Y; y++)
                {
                    InteractElement ite = new InteractElement(new Vector(x, y))
                    {
                        Opacity = 1,
                        Background = Brushes.Transparent,
                    };

                    InteractElements.Add(ite);

                    this.Children.Add(ite);
                    Grid.SetColumn(ite, x);
                    Grid.SetRow(ite, y);
                }
            }
        }

        public bool DisableInteractElement(Vector coords)
        {
            InteractElement selectedIte = InteractElements.FirstOrDefault(ite => Vector.Equals(ite.Position, coords));

            if (selectedIte == null) return false; //failed

            selectedIte.IsEnabled = false;
            selectedIte.Opacity = 0;
            return true; //success
        }
    }
    public class Board : Grid
    {
        protected Vector _dimensions;

        public BaseLayer BaseLayer;
        public ShipLayer ShipLayer;
        public PegLayer PegLayer;
        public InteractLayer InteractLayer;

        public bool HasShipLayer = true;
        public bool HasPegLayer = true;
        public bool HasInteractLayer = true;

        public Board() 
        {
            this.ShowGridLines = true;
            
            this._dimensions = new Vector(11, 11);

            this.MinHeight = 300; this.MinWidth = 300;
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;
            this.RowDefinitions.Add(new RowDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());

            //adding layers in order
            this.Children.Add(this.BaseLayer = new BaseLayer(_dimensions));
            if(HasShipLayer) 
                this.Children.Add(this.ShipLayer = new ShipLayer(_dimensions));
            if(HasPegLayer) 
                this.Children.Add(this.PegLayer = new PegLayer(_dimensions));
            if(HasInteractLayer) 
                this.Children.Add(this.InteractLayer = new InteractLayer(_dimensions));
        }

        public bool DisableLayer(Grid layer)
        {
            if(layer == null) return false;
            try
            {
                this.Children.Remove(layer);
            }
            catch (Exception ex) { return false; }
            return true;
        }

    }
}
