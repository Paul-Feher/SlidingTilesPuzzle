using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlidingTilesPuzzelSimulation
{
    /// <summary>
    /// 15Puzzle
    /// The Game of Fifteen is a puzzle played on a square, two-dimensional board with numbered tiles that slide. 
    /// The goal of this puzzle is to arrange the board’s tiles starting from a given arbitrary starting arrangement and to arrange them from smallest to largest, left to right, top to bottom, 
    /// with an empty space in the board’s bottom-right corner.
    /// Sliding any tile that borders the board’s empty space into that space constitutes a "move". Tiles may not be moved diagonally, or forcibly lifted or removed from the board.
    ///  
    /// The 15 puzzle consists of 15 squares numbered from 1 to 15 that are placed in a 4×4 box leaving one position out of the 16 empty. The goal is to reposition the squares from a given 
    /// arbitrary starting arrangement by sliding them one at a time into the configuration in which all the tiles are arranged in consecutive order and the empty position in the 16th place. 
    /// The basic form is of a 4x4 grid, usualy made with sliding tiles in a tray. There are 15 tiles numbered 1 through 15 and the 16th place is empty. 
    /// Before play begins, the tiles are randomly scrambled among the 16 positions, which is the starting position. The object of the game is to unscramble the tiles (by sliding, not by 
    /// lifting them) to get them into consecutive order, with the empty space in the bottom right position (this will be reffered to as the home position).
    /// For some initial arrangements, this rearrangement is possible, but for others, it is not.
    /// 
    /// To address the solvability of a given initial arrangement, proceed as follows: 
    /// If the square containing the number i appears "before" (reading the squares in the box from left to right and top to bottom) n numbers that are less than i, then call it an inversion 
    /// of order n, and denote it n_i. Then define:
    /// 
    ///             N=sum_(i=1)^(15)n_i=sum_(i=2)^(15)n_i, 
    ///             
    /// where the sum need run only from 2 to 15 rather than 1 to 15 since there are no numbers less than 1 (so n_1 must equal 0). Stated more simply,  N=i(p) is the number of permutation 
    /// inversions in the list of numbers. 
    /// Also define e to be the row number of the empty square.
    /// 
    /// Then if N+e is even, the position is possible, otherwise it is not. In other words, if the permutation symbol (-1)^(i(p)) of the list is +1, the position is possible, whereas if the 
    /// signature is -1, it is not. This can be formally proved using alternating groups.
    /// </summary>
    public partial class Form_Main : Form
    {
        private Random rnd = new Random();
        private static Timer timer1 = new Timer();

        private const int dockingSpaceSideSize = 21;
        private const int spacing = 3;
        private const int dockingSpaceHeight = 75;
        private const int dockingSpaceWidth = 75;
        private const int slidingTileHeight = 75;
        private const int slidingTileWidth = 75;
        private const int firstButtonHorizontalCoordinate = 6;
        private const int firstButtonVerticalCoordinate = 6;
        private Color dockingSpaceBackColor = Color.Bisque;
        private Color dockingSpaceForeColor = Color.Red;
            private Color slidingTileBackColor = Color.LightGray;
        private Color slidingTileForeColor = Color.Blue;
        private Color slidingTileFlatAppearanceBorderColor = Color.Brown;

        public Form_Main()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            //this.ClientSize = new Size(350, 350);
            this.Size = new System.Drawing.Size(800, 800);
            int formWidth = this.Size.Width;
            int formHeight = this.Size.Height;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;

            _numberOfSlidingTiles = 15;
            _numberOfDockingSpaces = _numberOfSlidingTiles + 1;

            _numberOfColumns = (int)Math.Sqrt(_numberOfSlidingTiles + 1);
            _numberOfRows = (int)Math.Sqrt(_numberOfSlidingTiles + 1);
            _grid = new int[_numberOfDockingSpaces + 1, _numberOfDockingSpaces + 1];
            _flattenedGrid = new int[_numberOfDockingSpaces + 1];
            int _numberToCheckForSquarability = _numberOfDockingSpaces;
            double result = Math.Sqrt(_numberToCheckForSquarability);
            bool isSquare = result % 1 == 0;

            //CustomizedPanel panel = new CustomizedPanel();

            panel1.ClientSize = new Size(_numberOfDockingSpaces * dockingSpaceSideSize - 4 * spacing, _numberOfDockingSpaces * dockingSpaceSideSize - 4 * spacing);
            panel1.Location = new Point((this.ClientSize.Width - panel1.ClientSize.Width) / 2, (this.ClientSize.Height - panel1.ClientSize.Height) / 2);
            panel1.BackColor = Color.FromArgb(127, 126, 214, 224);
            panel1.Select();
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Paint += new PaintEventHandler(Panel_Paint);
            panel1.Resize += new EventHandler(Panel_Resize);

            _emptyDockingSpace = null;

            _allDockingSpaces = new List<DockingSpace>();
            _allSlidingTiles = new List<SlidingTile>();
            _allSlidingTilesAdjacentToTheEmptyDockingSpace = null;
            CreateAllDockingSpaces(); // _allDockingSpaces initialization and _dockingSpaceByDockingSpaceNumber initialization.
            CompleteDockingSpacesNeighbours();
            _availableDockingSpaces = new List<DockingSpace>(_allDockingSpaces);
            _startingSlidingTileNumber = 1;
            CreateAllSlidingTilesLocationless(_startingSlidingTileNumber); // _allSlidingTiles initialization and _slidingTileBySlidingTileNumber initialization.
            
            textBox1.TextAlign = HorizontalAlignment.Center;
            textBox2.TextAlign = HorizontalAlignment.Center;
            textBox3.TextAlign = HorizontalAlignment.Center;
            textBox4.TextAlign = HorizontalAlignment.Center;
            _numberOfClicks = 0;
            _seconds = 0;
            
            button_StartNewGame.Click += new EventHandler(button_StartNewGame_Click);
            button_Exit.Click += new EventHandler(button_Exit_Click);

            FindSolvableSlidingTilesStartingArrangement();
        }

        private int _numberOfSlidingTiles;
        private int _numberOfDockingSpaces;

        private int _numberOfColumns;
        private int _numberOfRows;

        private int[,] _grid;
        private int[] _flattenedGrid; // Contains all the items in the grid in consecutive order, from to to bottom and from left to right, but in a single dimensional array (a vector).

        private int _tabIndex;

        private List<DockingSpace> _allDockingSpaces;
        private List<DockingSpace> _availableDockingSpaces;
        private Dictionary<int, DockingSpace> _dockingSpaceByDockingSpaceNumber;
        private Dictionary<Point, DockingSpace> _dockingSpaceByLocation;
        private static Dictionary<RowAndColumn, DockingSpace> _dockingSpaceByItsRowAndColumn;
        private DockingSpace _emptyDockingSpace;

        private int _startingSlidingTileNumber;
        private List<SlidingTile> _allSlidingTiles;
        private Dictionary<int, SlidingTile> _slidingTileBySlidingTileNumber;
        private List<SlidingTile> _allSlidingTilesAdjacentToTheEmptyDockingSpace;
        private int _seconds;
        private int _numberOfClicks;
        private TimeUnits _resultingTime;
        private bool _isSolvable;

        private void CreateAllDockingSpaces()
        {
            _dockingSpaceByDockingSpaceNumber = new Dictionary<int, DockingSpace>();
            _dockingSpaceByLocation = new Dictionary<Point, DockingSpace>();
            _dockingSpaceByItsRowAndColumn = new Dictionary<RowAndColumn, DockingSpace>();

            string fontFamilyName = "Times New Roman";
            float fontSize = 12F;
            FontStyle fontStyle = FontStyle.Bold;
            GraphicsUnit graphicsUnit = GraphicsUnit.Point;
            byte gdiCharSet = ((byte)(0));

            _tabIndex = 0;
            int dockingSpaceNumber = 1;
            for (int row = 1; row <= _numberOfRows; row++)
            {
                for (int column = 1; column <= _numberOfColumns; column++)
                {
                    DockingSpace currentDockingSpace = new DockingSpace();
                    currentDockingSpace.AllowDrop = true;
                    currentDockingSpace.AppurtenenceColumn = column;
                    currentDockingSpace.AppurtenenceRow = row;
                    currentDockingSpace.BackColor = dockingSpaceBackColor;
                    currentDockingSpace.ForeColor = dockingSpaceForeColor;
                    currentDockingSpace.Enabled = true;
                    currentDockingSpace.Font = new System.Drawing.Font(fontFamilyName, fontSize, fontStyle, graphicsUnit, gdiCharSet);
                    currentDockingSpace.Height = dockingSpaceHeight;
                    currentDockingSpace.IsAccessible = true;
                    int currentHorizontalCoordinate = firstButtonHorizontalCoordinate + (column - 1) * (dockingSpaceWidth + spacing);
                    int currentVerticalCoordinate = firstButtonVerticalCoordinate + (row - 1) * (dockingSpaceHeight + spacing);
                    currentDockingSpace.Location = new Point(panel1.Location.X + currentHorizontalCoordinate + spacing, panel1.Location.Y + currentVerticalCoordinate + spacing);
                    currentDockingSpace.Name = dockingSpaceNumber.ToString();
                    currentDockingSpace.DockingSpaceNumber = dockingSpaceNumber;
                    currentDockingSpace.Parent = this;
                    this.Controls.Add(currentDockingSpace);
                    if (panel1.Contains(currentDockingSpace))
                    {
                        currentDockingSpace.SendToBack();
                    }
                    currentDockingSpace.DockingSpaceRowAndColumn = new RowAndColumn(row, column);
                    currentDockingSpace.Size = new Size(dockingSpaceWidth, dockingSpaceHeight);
                    currentDockingSpace.TabIndex = _tabIndex;
                    currentDockingSpace.TabStop = true;
                    currentDockingSpace.Tag = null;

                    StringBuilder buildText = new StringBuilder();
                    buildText.Append("Docking" + Environment.NewLine + "  Space " + Environment.NewLine);
                    if (dockingSpaceNumber.ToString().Length == 1)
                    {
                        buildText.Append("  ");
                    }
                    else
                    {
                        buildText.Append(" ");
                    }
                    buildText.Append(dockingSpaceNumber.ToString());

                    currentDockingSpace.Text = buildText.ToString();
                    currentDockingSpace.Visible = true;
                    currentDockingSpace.Width = dockingSpaceWidth;
                    _allDockingSpaces.Add(currentDockingSpace);
                    _dockingSpaceByItsRowAndColumn.Add(currentDockingSpace.DockingSpaceRowAndColumn, currentDockingSpace);
                    _dockingSpaceByDockingSpaceNumber.Add(dockingSpaceNumber, currentDockingSpace);
                    _dockingSpaceByLocation.Add(currentDockingSpace.Location, currentDockingSpace);

                    _tabIndex++;
                    dockingSpaceNumber++;
                }
            }
        }

        private void CompleteDockingSpacesNeighbours()
        {
            for (int i = 0; i < _allDockingSpaces.Count; i++)
            {
                DockingSpace currentDockingSpace = _allDockingSpaces[i];
                int currentDockingSpaceNumber = currentDockingSpace.DockingSpaceNumber;
                currentDockingSpace = _dockingSpaceByDockingSpaceNumber[currentDockingSpaceNumber];
                int row = currentDockingSpace.AppurtenenceRow;
                int column = currentDockingSpace.AppurtenenceColumn;
                if (column == 1)
                {
                    currentDockingSpace.LeftAdjacentDockingSpace = null;
                }
                else
                {
                    RowAndColumn leftAdjacentDockingSpaceRowAndColumn = new RowAndColumn(currentDockingSpace.AppurtenenceRow, currentDockingSpace.AppurtenenceColumn - 1);
                    DockingSpace leftAdjacentDockingSpace = _dockingSpaceByItsRowAndColumn[leftAdjacentDockingSpaceRowAndColumn];
                    int leftAdjacentDockingSpaceNumber = leftAdjacentDockingSpace.DockingSpaceNumber;
                    leftAdjacentDockingSpace = _dockingSpaceByDockingSpaceNumber[leftAdjacentDockingSpaceNumber];
                    currentDockingSpace.LeftAdjacentDockingSpace = leftAdjacentDockingSpace;
                }
                if (column == _numberOfColumns)
                {
                    currentDockingSpace.RightAdjacentDockingSpace = null;
                }
                else
                {
                    RowAndColumn rightAdjacentDockingSpaceRowAndColumn = new RowAndColumn(currentDockingSpace.AppurtenenceRow, currentDockingSpace.AppurtenenceColumn + 1);
                    DockingSpace rightAdjacentDockingSpace = _dockingSpaceByItsRowAndColumn[rightAdjacentDockingSpaceRowAndColumn];
                    int rightAdjacentDockingSpaceNumber = rightAdjacentDockingSpace.DockingSpaceNumber;
                    rightAdjacentDockingSpace = _dockingSpaceByDockingSpaceNumber[rightAdjacentDockingSpaceNumber];
                    currentDockingSpace.RightAdjacentDockingSpace = rightAdjacentDockingSpace;
                }
                if (row == 1)
                {
                    currentDockingSpace.UpAdjacentDockingSpace = null;
                }
                else
                {
                    RowAndColumn upAdjacentDockingSpaceRowAndColumn = new RowAndColumn(currentDockingSpace.AppurtenenceRow - 1, currentDockingSpace.AppurtenenceColumn);
                    DockingSpace upAdjacentDockingSpace = _dockingSpaceByItsRowAndColumn[upAdjacentDockingSpaceRowAndColumn];
                    int upAdjacentDockingSpaceNumber = upAdjacentDockingSpace.DockingSpaceNumber;
                    upAdjacentDockingSpace = _dockingSpaceByDockingSpaceNumber[upAdjacentDockingSpaceNumber];
                    currentDockingSpace.UpAdjacentDockingSpace = upAdjacentDockingSpace;
                }
                if (row == _numberOfRows)
                {
                    currentDockingSpace.DownAdjacentDockingSpace = null;
                }
                else
                {
                    RowAndColumn downAdjacentDockingSpaceRowAndColumn = new RowAndColumn(currentDockingSpace.AppurtenenceRow + 1, currentDockingSpace.AppurtenenceColumn);
                    DockingSpace downAdjacentDockingSpace = _dockingSpaceByItsRowAndColumn[downAdjacentDockingSpaceRowAndColumn];
                    int downAdjacentDockingSpaceNumber = downAdjacentDockingSpace.DockingSpaceNumber;
                    downAdjacentDockingSpace = _dockingSpaceByDockingSpaceNumber[downAdjacentDockingSpaceNumber];
                    currentDockingSpace.DownAdjacentDockingSpace = downAdjacentDockingSpace;
                }
            }
        }

        /// <summary>
        /// Z-order is the visual layering of controls on a form along the form's z-axis (depth).
        /// The window at the top of the z-order overlaps all other windows. All other windows overlap the window at the bottom of the z-order.
        /// Use the BringToFront and SendToBack methods to manipulate the z-order of the controls.
        /// </summary>
        private void CreateAllSlidingTilesLocationless(int startingSlidingTileNumber)
        {
            string fontFamilyName = "Times New Roman";
            float fontSize = 20.2F;
            FontStyle fontStyle = FontStyle.Bold;
            GraphicsUnit graphicsUnit = GraphicsUnit.Point;
            byte gdiCharSet = ((byte)(0));
            _slidingTileBySlidingTileNumber = new Dictionary<int, SlidingTile>();
            int slidingTileNumber = startingSlidingTileNumber;
            for (int row = 1; row <= _numberOfRows; row++)
            {
                for (int column = 1; column <= _numberOfColumns; column++)
                {
                    if (slidingTileNumber <= _numberOfSlidingTiles + _startingSlidingTileNumber - 1)
                    {
                        SlidingTile currentSlidingTile = new SlidingTile();
                        currentSlidingTile.AutoSize = false;
                        currentSlidingTile.BackColor = slidingTileBackColor;
                        currentSlidingTile.DockedAtDockingSpace = null;
                        currentSlidingTile.SlidingTileNumber = slidingTileNumber;
                        currentSlidingTile.Enabled = true;
                        currentSlidingTile.FlatAppearance.BorderColor = slidingTileFlatAppearanceBorderColor;
                        currentSlidingTile.FlatAppearance.BorderSize = 0;
                        currentSlidingTile.FlatStyle = FlatStyle.Flat;
                        currentSlidingTile.Font = new System.Drawing.Font(fontFamilyName, fontSize, fontStyle, graphicsUnit, gdiCharSet);
                        currentSlidingTile.ForeColor = slidingTileForeColor;
                        currentSlidingTile.Height = slidingTileHeight;
                        currentSlidingTile.IsAccessible = true;
                        currentSlidingTile.Name = slidingTileNumber.ToString();
                        currentSlidingTile.Parent = this;
                        this.Controls.Add(currentSlidingTile);
                        if (this.Contains(currentSlidingTile))
                        {
                            currentSlidingTile.BringToFront();
                        }
                        _tabIndex++;
                        currentSlidingTile.TabIndex = _tabIndex;
                        currentSlidingTile.TabStop = true;
                        currentSlidingTile.Tag = null;
                        currentSlidingTile.Text = slidingTileNumber.ToString();
                        currentSlidingTile.Visible = true;
                        currentSlidingTile.Width = slidingTileWidth;

                        currentSlidingTile.Paint += new PaintEventHandler(SlidingTile_Paint);

                        _allSlidingTiles.Add(currentSlidingTile);
                        _slidingTileBySlidingTileNumber.Add(slidingTileNumber, currentSlidingTile);
                        slidingTileNumber++;
                    }
                }
            }
        }

        private void InitializeSlidingTilesArrangement()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            _seconds = 0;
            _numberOfClicks = 0;
            panel1.Invalidate();
            _availableDockingSpaces = new List<DockingSpace>(_allDockingSpaces);
            //SetConsecutiveSlidingTilesArrangement();
            //SetConsecutiveInReverseOrderSlidingTilesArrangement();
            //SetConsecutiveInReverseOrderButSolvableSlidingTilesArrangement();
            SetRandomSlidingTilesArrangement();
            PopulateAllSlidingTilesAdjacentToTheEmptyDockingSpace(_emptyDockingSpace); // _allSlidingTilesAdjacentToTheEmptyDockingSpace initialization.
            //for (int row = 0; row <= _numberOfRows; row++)
            //{
            //    for (int column = 0; column <= _numberOfColumns; column++)
            //    {
            //        textBox5.AppendText(_grid[row, column].ToString() + "   ");
            //    }
            //    textBox5.AppendText(Environment.NewLine);
            //}
        }

        private void SetConsecutiveSlidingTilesArrangement()
        {
            for (int i = 0; i < _allSlidingTiles.Count; i++)
            {
                SlidingTile currentSlidingTile = _allSlidingTiles[i];
                DockingSpace dockingSpaceToDockAt = _allDockingSpaces[i];
                currentSlidingTile.Location = dockingSpaceToDockAt.Location;
                currentSlidingTile.DockedAtDockingSpace = dockingSpaceToDockAt;
                dockingSpaceToDockAt.DockedSlidingTile = currentSlidingTile;
                _grid[dockingSpaceToDockAt.AppurtenenceRow, dockingSpaceToDockAt.AppurtenenceColumn] = currentSlidingTile.SlidingTileNumber;
                _flattenedGrid[dockingSpaceToDockAt.DockingSpaceNumber] = currentSlidingTile.SlidingTileNumber;
            }
            _emptyDockingSpace = _availableDockingSpaces[15];
            _flattenedGrid[_emptyDockingSpace.DockingSpaceNumber] = 0;
            _emptyDockingSpace.DockedSlidingTile = null;
            if (CountInversions() == 0 && (_emptyDockingSpace.DockingSpaceNumber == _numberOfDockingSpaces))
            {
                GameOver();
            }
        }

        private void SetConsecutiveInReverseOrderSlidingTilesArrangement()
        {
            int currentSlidingTileNumber = _allSlidingTiles.Count - 1;
            for (int i = 0; i < _allSlidingTiles.Count; i++)
            {
                SlidingTile currentSlidingTile = _allSlidingTiles[currentSlidingTileNumber];
                DockingSpace dockingSpaceToDockAt = _allDockingSpaces[i];
                currentSlidingTile.Location = dockingSpaceToDockAt.Location;
                currentSlidingTile.DockedAtDockingSpace = dockingSpaceToDockAt;
                dockingSpaceToDockAt.DockedSlidingTile = currentSlidingTile;
                _grid[dockingSpaceToDockAt.AppurtenenceRow, dockingSpaceToDockAt.AppurtenenceColumn] = currentSlidingTile.SlidingTileNumber;
                _flattenedGrid[dockingSpaceToDockAt.DockingSpaceNumber] = currentSlidingTile.SlidingTileNumber;
                currentSlidingTileNumber--;
            }
            _emptyDockingSpace = _availableDockingSpaces[15];
            _emptyDockingSpace.DockedSlidingTile = null;
            _grid[_emptyDockingSpace.AppurtenenceRow, _emptyDockingSpace.AppurtenenceColumn] = 0;
            _flattenedGrid[_emptyDockingSpace.DockingSpaceNumber] = 0;
            if (CountInversions() == 0 && (_emptyDockingSpace.DockingSpaceNumber == _numberOfDockingSpaces))
            {
                GameOver();
            }
        }

        private void SetConsecutiveInReverseOrderButSolvableSlidingTilesArrangement()
        {
            SlidingTile currentSlidingTile;
            int slidingTileNumber = _allSlidingTiles.Count - 1;
            for (int i = 0; i < _allSlidingTiles.Count; i++)
            {
                if (slidingTileNumber == 1)
                {
                    currentSlidingTile = _allSlidingTiles[0];
                }
                else
                {
                    if (slidingTileNumber == 0)
                    {
                        currentSlidingTile = _allSlidingTiles[1];
                    }
                    else
                    {
                        currentSlidingTile = _allSlidingTiles[slidingTileNumber];
                    }
                }
                DockingSpace dockingSpaceToDockAt = _allDockingSpaces[i];
                currentSlidingTile.Location = dockingSpaceToDockAt.Location;
                currentSlidingTile.DockedAtDockingSpace = dockingSpaceToDockAt;
                dockingSpaceToDockAt.DockedSlidingTile = currentSlidingTile;
                _grid[dockingSpaceToDockAt.AppurtenenceRow, dockingSpaceToDockAt.AppurtenenceColumn] = slidingTileNumber;
                _flattenedGrid[dockingSpaceToDockAt.DockingSpaceNumber] = slidingTileNumber;
                slidingTileNumber--;
            }
            _emptyDockingSpace = _availableDockingSpaces[15];
            _emptyDockingSpace.DockedSlidingTile = null;
            _grid[_emptyDockingSpace.AppurtenenceRow, _emptyDockingSpace.AppurtenenceColumn] = 0;
            _flattenedGrid[_emptyDockingSpace.DockingSpaceNumber] = 0;
            if (CountInversions() == 0 && (_emptyDockingSpace.DockingSpaceNumber == _numberOfDockingSpaces))
            {
                GameOver();
            }
        }

        private void SetRandomSlidingTilesArrangement()
        {
            foreach (SlidingTile currentSlidingTile in _allSlidingTiles) // Take all sliding tiles in consecutive order.
            {
                DockingSpace dockingSpaceToDockAt = _availableDockingSpaces[rnd.Next(0, _availableDockingSpaces.Count)]; // Randomly select a docking space to dock the current sliding tile to.
                currentSlidingTile.Location = dockingSpaceToDockAt.Location;
                currentSlidingTile.DockedAtDockingSpace = dockingSpaceToDockAt;
                dockingSpaceToDockAt.DockedSlidingTile = currentSlidingTile;
                _grid[dockingSpaceToDockAt.AppurtenenceRow, dockingSpaceToDockAt.AppurtenenceColumn] = currentSlidingTile.SlidingTileNumber;
                _flattenedGrid[dockingSpaceToDockAt.DockingSpaceNumber] = currentSlidingTile.SlidingTileNumber;
                _availableDockingSpaces.Remove(dockingSpaceToDockAt);
            }
            _emptyDockingSpace = _availableDockingSpaces[0];
            _emptyDockingSpace.DockedSlidingTile = null;
            _grid[_emptyDockingSpace.AppurtenenceRow, _emptyDockingSpace.AppurtenenceColumn] = 0;
            _flattenedGrid[_emptyDockingSpace.DockingSpaceNumber] = 0;
            if (CountInversions() == 0 && (_emptyDockingSpace.DockingSpaceNumber == _numberOfDockingSpaces))
            {
                GameOver();
            }
        }

        private void ManuallySetCustomSlidingTilesState()
        {
            foreach (SlidingTile currentSlidingTile in _allSlidingTiles)
            {

            }
        }

        private int CountInversions()
        {
            int inversionsCount = 0;
            List<int> allSlidingTilesNumbers = new List<int>();
            int[] flattenedGrid = new int[_numberOfSlidingTiles + _startingSlidingTileNumber + 1];
            for (int i = 0; i < _allDockingSpaces.Count; i++)
            {
                DockingSpace currentDockingSpace = _allDockingSpaces[i];
                if (currentDockingSpace.DockedSlidingTile != null)
                {
                    SlidingTile dockedSlidingTile = currentDockingSpace.DockedSlidingTile;
                    int slidingTileNumber = dockedSlidingTile.SlidingTileNumber;
                    allSlidingTilesNumbers.Add(slidingTileNumber);
                }
            }
            for (int i = 0; i < allSlidingTilesNumbers.Count - 1; i++)
            {
                for (int k = i + 1; k < allSlidingTilesNumbers.Count; k++)
                {
                    if (allSlidingTilesNumbers[i] > allSlidingTilesNumbers[k])
                    {
                        flattenedGrid[allSlidingTilesNumbers[i]]++;
                    }
                }
            }
            for (int i = 0; i < flattenedGrid.Length; i++)
            {
                inversionsCount += flattenedGrid[i];
            }

            return inversionsCount;
        }

        /// <summary>
        /// In general, for a given grid of width N, we can find out if a N*N – 1 puzzle is solvable or not by following the below simple rules:
        /// 
        /// 1. If N is odd, then the puzzle instance is solvable if the number of inversions is even in the input state.
        /// 2. If N is even, the puzzle instance is solvable if:
        ///     2.1. if the blank is on an even row counting from the bottom (second-last, fourth-last, etc.) then the number of inversions must be odd.
        ///     2.2. if the blank is on an odd row counting from the bottom (last, third-last, fifth-last, etc.) then the number of inversions must be even.
        /// For all other cases, the puzzle instance is not solvable.
        /// 
        /// What is an inversion here?
        /// If we assume the tiles written out in a single row (1D Array) instead of being spread in N-rows (2D Array), a pair of tiles (a, b) form an inversion if a appears before b and > b.
        /// 
        /// The formula for determining solvability for a 4x4 grid: ((grid width odd) && (#inversions even)) || ((grid width even) && ((blank on odd row from bottom) == (#inversions even)))
        /// 
        /// A parity argument is used to show that half of the starting positions for the n-puzzle are impossible to resolve, no matter how many moves are made. 
        /// This is done by considering a function of the tile configuration that is invariant under any valid move, and then using this to partition the space of all possible labeled states into 
        /// two equivalence classes of reachable and unreachable states.
        /// The invariant is the parity of the permutation of all 16 squares plus the parity of the taxicab distance (number of rows plus number of columns) of the empty square from the lower right 
        /// corner. 
        /// This is an invariant because each move changes both the parity of the permutation and the parity of the taxicab distance. 
        /// In particular, if the empty square is in the lower right corner then the puzzle is solvable if and only if the permutation of the remaining pieces is even.
        /// In an alternate view of the problem, we can consider the invariant to be the sum of the parity of the number of inversions in the current order of the 15 numbered pieces and the parity 
        /// of the difference in the row number of the empty square from the row number of the last row. (Let's call it row distance from the last row.) This is an invariant because each column move, 
        /// when we move a piece within the same column, changes both the parity of the number of inversions (by changing the number of inversions by ±1, ±3) and the parity of the row distance from 
        /// the last row (changing row distance by ±1); and each row move, when we move a piece within the same row, does not change any of the two parities. 
        /// Now if we look at the solved state of the puzzle, this sum is even. Hence it is easy to prove by induction that any state of the puzzle for which the above sum is odd cannot be solvable. 
        /// In particular, if the empty square is in the lower right corner (even anywhere in the last row) then the puzzle is solvable if and only if the number of inversions of the numbered pieces 
        /// is even.
        /// </summary>
        /// <param name="inversionsCount"></param>
        /// <returns></returns>
        private bool IsPuzzleSolvable()
        {
            bool puzzleIsSolvable = false;
            int rowContainingTheEmptyDockingSpaceCountingFromTheTop = _emptyDockingSpace.AppurtenenceRow;
            int rowContainingTheEmptyDockingSpaceCountingFromTheBottom = _numberOfRows - rowContainingTheEmptyDockingSpaceCountingFromTheTop + 1; /* In a grid of any size, the row number of any row counted 
                                                                                                                                                   * from the bottom equals the difference of the total number 
                                                                                                                                                   * of rows in the grid and the row number of the same row 
                                                                                                                                                   * counted from the top plus 1.
                                                                                                                                                   */
            int inversionsCount = CountInversions();

            if (_numberOfDockingSpaces % 2 == 1) // N (the grid width) is odd: the number of inversions must be even (inversionsCount % 2 == 0).
            {
                puzzleIsSolvable = inversionsCount % 2 == 0;
            }
            else /* N (the grid width) is even: if the blank is on an even row counting from the bottom (rowCountingFromTheBottom % 2 == 0) then the number of inversions must be odd (inversionsCount % 2 == 1).
                  *                             if the blank is on an odd row counting from the bottom (rowCountingFromTheBottom % 2 == 1) then the number of inversions must be even (inversionsCount % 2 == 0).
                  */
            {
                if (rowContainingTheEmptyDockingSpaceCountingFromTheBottom % 2 == 0) // the blank is on an even row counting from the bottom.
                {
                    puzzleIsSolvable = inversionsCount % 2 == 1; // The number of inversions must be odd.
                }
                else // The blank is on an odd row counting from the bottom.
                {
                    puzzleIsSolvable = inversionsCount % 2 == 0; // The number of inversions must be even.
                }
            }

            return puzzleIsSolvable;
        }

        private void PopulateAllSlidingTilesAdjacentToTheEmptyDockingSpace(DockingSpace emptyDockingSpace)
        {
            _allSlidingTilesAdjacentToTheEmptyDockingSpace = new List<SlidingTile>();
            if (_emptyDockingSpace.LeftAdjacentDockingSpace != null)
            {
                DockingSpace leftAdjacentDockingSpace = _emptyDockingSpace.LeftAdjacentDockingSpace;
                if (leftAdjacentDockingSpace != null)
                {
                    SlidingTile slidingTilesAdjacentToTheEmptyDockingSpaceToTheLeft = leftAdjacentDockingSpace.DockedSlidingTile;
                    int slidingTilesAdjacentToTheEmptyDockingSpaceNumber = slidingTilesAdjacentToTheEmptyDockingSpaceToTheLeft.SlidingTileNumber;
                    slidingTilesAdjacentToTheEmptyDockingSpaceToTheLeft = _slidingTileBySlidingTileNumber[slidingTilesAdjacentToTheEmptyDockingSpaceNumber];
                    _allSlidingTilesAdjacentToTheEmptyDockingSpace.Add(slidingTilesAdjacentToTheEmptyDockingSpaceToTheLeft);

                }
            }
            if (_emptyDockingSpace.RightAdjacentDockingSpace != null)
            {
                DockingSpace rightAdjacentDockingSpace = _emptyDockingSpace.RightAdjacentDockingSpace;
                if (rightAdjacentDockingSpace != null)
                {
                    SlidingTile slidingTilesAdjacentToTheEmptyDockingSpaceToTheRight = rightAdjacentDockingSpace.DockedSlidingTile;
                    int slidingTilesAdjacentToTheEmptyDockingSpaceNumber = slidingTilesAdjacentToTheEmptyDockingSpaceToTheRight.SlidingTileNumber;
                    slidingTilesAdjacentToTheEmptyDockingSpaceToTheRight = _slidingTileBySlidingTileNumber[slidingTilesAdjacentToTheEmptyDockingSpaceNumber];
                    _allSlidingTilesAdjacentToTheEmptyDockingSpace.Add(slidingTilesAdjacentToTheEmptyDockingSpaceToTheRight);
                }
            }
            if (_emptyDockingSpace.UpAdjacentDockingSpace != null)
            {
                DockingSpace upAdjacentDockingSpace = _emptyDockingSpace.UpAdjacentDockingSpace;
                if (upAdjacentDockingSpace != null)
                {
                    SlidingTile slidingTilesAdjacentToTheEmptyDockingSpaceUpwards = upAdjacentDockingSpace.DockedSlidingTile;
                    int slidingTilesAdjacentToTheEmptyDockingSpaceNumber = slidingTilesAdjacentToTheEmptyDockingSpaceUpwards.SlidingTileNumber;
                    slidingTilesAdjacentToTheEmptyDockingSpaceUpwards = _slidingTileBySlidingTileNumber[slidingTilesAdjacentToTheEmptyDockingSpaceNumber];
                    _allSlidingTilesAdjacentToTheEmptyDockingSpace.Add(slidingTilesAdjacentToTheEmptyDockingSpaceUpwards);
                }

            }
            if (_emptyDockingSpace.DownAdjacentDockingSpace != null)
            {
                DockingSpace downAdjacentDockingSpace = _emptyDockingSpace.DownAdjacentDockingSpace;
                if (downAdjacentDockingSpace != null)
                {
                    SlidingTile slidingTilesAdjacentToTheEmptyDockingSpaceDownwards = downAdjacentDockingSpace.DockedSlidingTile;
                    int slidingTilesAdjacentToTheEmptyDockingSpaceNumber = slidingTilesAdjacentToTheEmptyDockingSpaceDownwards.SlidingTileNumber;
                    slidingTilesAdjacentToTheEmptyDockingSpaceDownwards = _slidingTileBySlidingTileNumber[slidingTilesAdjacentToTheEmptyDockingSpaceNumber];
                    _allSlidingTilesAdjacentToTheEmptyDockingSpace.Add(slidingTilesAdjacentToTheEmptyDockingSpaceDownwards);
                }

            }
        }

        private void GameOver()
        {
            UnsubscribeAllSlidingTilesFromClickEvent();
            timer1.Enabled = false;
            timer1.Stop();
            textBox3.Text = "Game Over." + Environment.NewLine +
                            "Elapsed time: " + _resultingTime.ResultingMinutes.ToString() + " minutes, " + _resultingTime.ResultingSeconds.ToString() + " seconds." + Environment.NewLine +
                            "Number of moves: " + _numberOfClicks.ToString() + ".";
        }

        private void UnsubscribeAllSlidingTilesFromClickEvent()
        {
            for (int i = 0; i < _allSlidingTiles.Count; i++)
            {
                SlidingTile currentSlidingTile = _allSlidingTiles[i];
                currentSlidingTile.Click -= new EventHandler(SlidingTile_Click);
            }
        }

        private void SlidingTile_Click(object sender, EventArgs e)
        {
            if (sender is SlidingTile)
            {
                if (_numberOfClicks == 0)
                {
                    InitializeTimer();
                }
                SlidingTile senderSlidingTile = sender as SlidingTile;
                int senderSlidingTileNumber = senderSlidingTile.SlidingTileNumber;
                SlidingTile clickedSlidingTile = _slidingTileBySlidingTileNumber[senderSlidingTileNumber];
                if (_allSlidingTilesAdjacentToTheEmptyDockingSpace.Contains(clickedSlidingTile))
                {
                    DockingSpace dockedAtDockingSpace = (DockingSpace)senderSlidingTile.DockedAtDockingSpace;
                    int previouslyDockedAtDockingSpaceNumber = dockedAtDockingSpace.DockingSpaceNumber;
                    int previouslyEmptyDockingSpaceNumber = _emptyDockingSpace.DockingSpaceNumber;
                    if (_allSlidingTilesAdjacentToTheEmptyDockingSpace.Contains(clickedSlidingTile))
                    {
                        ConvertEmptyDockingSpaceToDockedAtDockingSpace(previouslyEmptyDockingSpaceNumber, clickedSlidingTile);
                        ConvertDockedAtDockingSpaceToEmptyDockingSpace(previouslyDockedAtDockingSpaceNumber, clickedSlidingTile);
                        _numberOfClicks++;
                    }
                }
                textBox2.Text = "Number of moves: " + _numberOfClicks.ToString() + ".";
                if (CountInversions() == 0 && (_emptyDockingSpace.DockingSpaceNumber == _numberOfDockingSpaces))
                {
                    GameOver();
                }
            }
        }

        private void ConvertEmptyDockingSpaceToDockedAtDockingSpace(int previouslyEmptyDockingSpaceNumber, SlidingTile clickedSlidingTile)
        {
            DockingSpace currentlyDockedAtDockingSpace = _dockingSpaceByDockingSpaceNumber[previouslyEmptyDockingSpaceNumber];
            //clickedSlidingTile.Location = currentlyDockedAtDockingSpace.Location;
            clickedSlidingTile.DockedAtDockingSpace = currentlyDockedAtDockingSpace;
            currentlyDockedAtDockingSpace.DockedSlidingTile = clickedSlidingTile;
            clickedSlidingTile.Location = currentlyDockedAtDockingSpace.Location;
            if (CountInversions() == 0 && (_emptyDockingSpace.DockingSpaceNumber == _numberOfDockingSpaces))
            {
                GameOver();
            }
        }

        private void ConvertDockedAtDockingSpaceToEmptyDockingSpace(int previouslyDockedAtDockingSpaceNumber, SlidingTile clickedSlidingTile)
        {
            DockingSpace previouslyDockedAtDockingSpace = _dockingSpaceByDockingSpaceNumber[previouslyDockedAtDockingSpaceNumber];
            _emptyDockingSpace = previouslyDockedAtDockingSpace;
            _emptyDockingSpace.DockedSlidingTile = null;
            _allSlidingTilesAdjacentToTheEmptyDockingSpace.Clear();
            PopulateAllSlidingTilesAdjacentToTheEmptyDockingSpace(_emptyDockingSpace); // _allSlidingTilesAdjacentToTheEmptyDockingSpace initialization.
            if (CountInversions() == 0 && (_emptyDockingSpace.DockingSpaceNumber == _numberOfDockingSpaces))
            {
                GameOver();
            }
        }

        public void InitializeTimer()
        {
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000; // In miliseconds.
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _seconds++;
            _resultingTime = TimeSpanUtil.ConvertSecondsIntoDaysHoursMinutesAndSeconds(_seconds);
            textBox1.Text = "Elapsed time: " + _resultingTime.ResultingDays.ToString() + " days, " + _resultingTime.ResultingHours.ToString() + " hours, " + _resultingTime.ResultingMinutes.ToString() + " minutes, " +
                                               _resultingTime.ResultingSeconds.ToString() + " seconds, " + _resultingTime.ResultingMilliseconds.ToString() + " milliseconds.";
        }

        private int CalculateDockingSpaceNumberByRowAndColumn(int row, int column)
        {
            return (row - 1) * _numberOfColumns + column;
        }

        private int CalculateDockingSpaceNumberByRowAndColumn(RowAndColumn dockingSpaceRowAndColumn)
        {
            return (dockingSpaceRowAndColumn.Row - 1) * _numberOfColumns + dockingSpaceRowAndColumn.Column;
        }

        private RowAndColumn CalculateRowAndColumnByDockingSpaceNumber(int dockingSpaceNumber)
        {
            if (dockingSpaceNumber > _numberOfRows * _numberOfColumns)
            {
                throw new Exception("The value: " + dockingSpaceNumber.ToString() + " is a wrong value for the dockingSpaceNumber!" + Environment.NewLine +
                                    "The dockingSpaceNumber must be a number between 1 and " + (_numberOfRows * _numberOfColumns).ToString() + ".");
            }
            int row = 0;
            double quotient = (double)dockingSpaceNumber / (double)_numberOfColumns;
            row = (int)Math.Ceiling(quotient);
            int column = dockingSpaceNumber % _numberOfColumns;
            if (column == 0)
            {
                column = _numberOfColumns;
            }

            return new RowAndColumn(row, column);
        }

        private void SlidingTile_Paint(object sender, PaintEventArgs e)
        {
            if (sender is SlidingTile)
            {
                SlidingTile currentButton = sender as SlidingTile;
                ControlPaint.DrawBorder(e.Graphics, currentButton.ClientRectangle,
                                        SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset,
                                        SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset,
                                        SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset,
                                        SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset);

            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            ControlPaint.DrawBorder(e.Graphics, panel.DisplayRectangle, Color.Brown, ButtonBorderStyle.Inset);
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            Invalidate();
            Application.DoEvents();
        }

        private void button_StartNewGame_Click(object sender, EventArgs e)
        {
            UnsubscribeAllSlidingTilesFromClickEvent();
            FindSolvableSlidingTilesStartingArrangement();
        }

        private void FindSolvableSlidingTilesStartingArrangement()
        {
            do
            {
                InitializeSlidingTilesArrangement();
                _isSolvable = IsPuzzleSolvable();
            }
            while (!_isSolvable);
            if (_isSolvable)
            {
                SubscribeAllSlidingTilesToClickEvent();
                textBox4.Text = "The puzzle is solvable.";
            }
            else
            {
                textBox4.Text = "The puzzle NOT solvable.";
            }
        }

        private void SubscribeAllSlidingTilesToClickEvent()
        {
            for (int i = 0; i < _allSlidingTiles.Count; i++)
            {
                SlidingTile currentSlidingTile = _allSlidingTiles[i];
                currentSlidingTile.Click += new EventHandler(SlidingTile_Click);
            }
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
