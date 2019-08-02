using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SlidingTilesPuzzelSimulation
{
    public class SlidingTile : Button
    {
        public SlidingTile()
        {
            _slidingTileNumber = -1;
            _appurtenenceRow = -1;
            _appurtenenceColumn = -1;
            _slidingTileRowAndColumn = new RowAndColumn(_appurtenenceRow, _appurtenenceColumn);
            _location = new Point();
            _dockedAtDockingSpace = null;
            _finalDockingSpaceNumber = -1;
        }

        private int _slidingTileNumber;
        public int SlidingTileNumber
        {
            get { return _slidingTileNumber; }
            set { _slidingTileNumber = value; }
        }

        private int _appurtenenceRow;
        public int AppurtenenceRow
        {
            get { return _appurtenenceRow; }
            set { _appurtenenceRow = value; }
        }

        private int _appurtenenceColumn;
        public int AppurtenenceColumn
        {
            get { return _appurtenenceColumn; }
            set { _appurtenenceColumn = value; }
        }

        private RowAndColumn _slidingTileRowAndColumn;
        public RowAndColumn SlidingTileRowAndColumn
        {
            get { return _slidingTileRowAndColumn; }
            set { _slidingTileRowAndColumn = value; }
        }

        private Point _location;
        public Point Location1
        {
            get { return _location; }
        }

        private DockingSpace _dockedAtDockingSpace;
        public DockingSpace DockedAtDockingSpace
        {
            get { return _dockedAtDockingSpace; }
            set { _dockedAtDockingSpace = value; }
        }

        private int _finalDockingSpaceNumber;
        public int FinalDockingSpaceNumber
        {
            get { return _finalDockingSpaceNumber; }
            set { _finalDockingSpaceNumber = value; }
        }
    }
}
