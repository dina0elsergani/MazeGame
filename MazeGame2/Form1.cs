using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MazeGame2
{
    public partial class Form1 : Form
    {
        private const int CellSize = 30;
        private const int MazeWidth = 20;
        private const int MazeHeight = 20;
        private const int PlayerSize = 20;

        private readonly List<Point> _path = new List<Point>();
        private readonly List<Point> _visited = new List<Point>();
        private readonly List<Point> _walls = new List<Point>();
        private Point _start;
        private readonly Point _end = new Point(MazeWidth - 2, MazeHeight - 2);
        private Point _player;

        public Form1()
        {
            this.KeyPreview = true;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GenerateMaze();
            boardPanel.Paint += BoardPanel_Paint;
        }

        private void BoardPanel_Paint(object sender, PaintEventArgs e)
        {
            foreach (var wall in _walls)
            {
                e.Graphics.FillRectangle(Brushes.Black, wall.X * CellSize, wall.Y * CellSize, CellSize, CellSize);
            }

            foreach (var cell in _visited)
            {
                e.Graphics.FillRectangle(Brushes.LightGray, cell.X * CellSize, cell.Y * CellSize, CellSize, CellSize);
            }

            e.Graphics.FillRectangle(Brushes.Green, _start.X * CellSize, _start.Y * CellSize, CellSize, CellSize);
            e.Graphics.FillRectangle(Brushes.Red, _end.X * CellSize, _end.Y * CellSize, CellSize, CellSize);

            e.Graphics.FillEllipse(Brushes.Blue, _player.X * CellSize + (CellSize - PlayerSize) / 2, _player.Y * CellSize + (CellSize - PlayerSize) / 2, PlayerSize, PlayerSize);

            for (int i = 0; i < _path.Count - 1; i++)
            {
                e.Graphics.DrawLine(Pens.Blue, _path[i].X * CellSize + CellSize / 2, _path[i].Y * CellSize + CellSize / 2, _path[i + 1].X * CellSize + CellSize / 2, _path[i + 1].Y * CellSize + CellSize / 2);
            }
        }

        private void GenerateMaze()
        {
            _walls.Clear();
            _visited.Clear();
            _path.Clear();
            _start = new Point(1, 1);

            for (int x = 0; x < MazeWidth; x++)
            {
                for (int y = 0; y < MazeHeight; y++)
                {
                    if (x == 0 || y == 0 || x == MazeWidth - 1 || y == MazeHeight - 1)
                    {
                        _walls.Add(new Point(x, y));
                    }
                    else if (x % 2 == 0 && y % 2 == 0)
                    {
                        _walls.Add(new Point(x, y));
                    }
                }
            }
            _walls.Remove(_end);

            _player = _start;
            _visited.Add(_start);

            DFS(_start);

            boardPanel.Invalidate();
        }

        private void DFS(Point current)
        {
            _visited.Add(current);

            List<Point> neighbors = GetValidNeighbors(current);

            foreach (Point neighbor in neighbors)
            {
                if (!_visited.Contains(neighbor))
                {
                    _path.Add(current);
                    _path.Add(neighbor);

                    if (neighbor == _end)
                    {
                        return;
                    }

                    DFS(neighbor);

                    if (_path.Last() == _end)
                    {
                        return;
                    }

                    _path.RemoveAt(_path.Count - 1);
                    _path.RemoveAt(_path.Count - 1);
                }
            }
        }

        private List<Point> GetValidNeighbors(Point current)
        {
            List<Point> neighbors = new List<Point>
        {
            new Point(current.X - 1, current.Y),
            new Point(current.X + 1, current.Y),
            new Point(current.X, current.Y - 1),
            new Point(current.X, current.Y + 1)
        };

            neighbors.RemoveAll(neighbor => !IsValid(neighbor));
            return neighbors;
        }

        private bool IsValid(Point point)
        {
            return point.X >= 0 && point.X < MazeWidth &&
                   point.Y >= 0 && point.Y < MazeHeight &&
                   !_walls.Contains(point);
        }
    }
}