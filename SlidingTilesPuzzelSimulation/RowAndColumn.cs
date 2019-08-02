using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlidingTilesPuzzelSimulation
{
    public struct RowAndColumn
    {
        public RowAndColumn(int row, int column)
        {
            _row = row;
            _column = column;
        }

        private int _row;
        public int Row
        {
            get { return _row; }
        }

        private int _column;
        public int Column
        {
            get { return _column; }
        }
    }
}

