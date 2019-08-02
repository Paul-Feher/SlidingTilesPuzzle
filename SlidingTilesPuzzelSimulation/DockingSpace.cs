using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SlidingTilesPuzzelSimulation
{
    public class DockingSpace : FlatButton
    {
        public DockingSpace()
        {
            _dockingSpaceNumber = -1;
            _appurtenenceRow = -1;
            _appurtenenceColumn = -1;
            _dockingSpaceRowAndColumn = new RowAndColumn(_appurtenenceRow, _appurtenenceColumn);
            _location = new Point();
            _dockedSlidingTile = null;
            _leftAdjacentDockingSpace = null;
            _rightAdjacentDockingSpace = null;
            _upAdjacentDockingSpace = null;
            _downAdjacentDockingSpace = null;
        }

        private int _dockingSpaceNumber;
        public int DockingSpaceNumber
        {
            get { return _dockingSpaceNumber; }
            set { _dockingSpaceNumber = value; }
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

        private RowAndColumn _dockingSpaceRowAndColumn;
        public RowAndColumn DockingSpaceRowAndColumn
        {
            get { return _dockingSpaceRowAndColumn; }
            set { _dockingSpaceRowAndColumn = value; }
        }

        private Point _location;
        public Point Location1
        {
            get { return _location; }
        }

        private SlidingTile _dockedSlidingTile;
        public SlidingTile DockedSlidingTile
        {
            get { return _dockedSlidingTile; }
            set { _dockedSlidingTile = value; }
        }

        private DockingSpace _leftAdjacentDockingSpace;
        public DockingSpace LeftAdjacentDockingSpace
        {
            get { return _leftAdjacentDockingSpace; }
            set { _leftAdjacentDockingSpace = value; }
        }

        private DockingSpace _rightAdjacentDockingSpace;
        public DockingSpace RightAdjacentDockingSpace
        {
            get { return _rightAdjacentDockingSpace; }
            set { _rightAdjacentDockingSpace = value; }
        }

        private DockingSpace _upAdjacentDockingSpace;
        public DockingSpace UpAdjacentDockingSpace
        {
            get { return _upAdjacentDockingSpace; }
            set { _upAdjacentDockingSpace = value; }
        }

        private DockingSpace _downAdjacentDockingSpace;
        public DockingSpace DownAdjacentDockingSpace
        {
            get { return _downAdjacentDockingSpace; }
            set { _downAdjacentDockingSpace = value; }
        }
    }
}
