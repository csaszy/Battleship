using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Player
    {
        private string _name;
        private int _id;

        public Board TargetBoard;
        public Board OceanBoard;

        public string Name { get { return _name; } }
        public int Id { get { return _id; } }

        public Player(string name, int id)
        {
            this._name = name;
            this._id = id;

            TargetBoard = new Board();
            OceanBoard = new Board();

            OceanBoard.DisableLayer(OceanBoard.InteractLayer);
        }

        public bool Attack(Vector coords, Player playerWhoWasHit)
        {
            Ship shipThatWasHit = playerWhoWasHit.OceanBoard.ShipLayer.FindShipAt(coords);

            if (shipThatWasHit == null) 
            {
                //player has missed
                this.TargetBoard.PegLayer.PlacePeg(coords, PegType.Grey);
                this.TargetBoard.InteractLayer.DisableInteractElement(coords);
                playerWhoWasHit.OceanBoard.PegLayer.PlacePeg(coords, PegType.Grey);

                return true;
            }

            //player has a hit
            shipThatWasHit.Hits++;

            this.TargetBoard.PegLayer.PlacePeg(coords, PegType.Red);
            this.TargetBoard.InteractLayer.DisableInteractElement(coords);
            playerWhoWasHit.OceanBoard.PegLayer.PlacePeg(coords, PegType.Red);

            return false;
        }
    }
}
