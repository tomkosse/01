using System.Diagnostics;

var lines = File.ReadAllLines(args[0]);
var sw = Stopwatch.StartNew();

var seabed = GetCellMatrix(lines.Select(lines => lines.ToCharArray().Select(c => c).ToArray()).ToArray());

int countMoved = 1;
int step = 0;
var allCells = seabed.SelectMany(c => c).ToArray();
while (countMoved > 0)
{
    countMoved = 0;
    var eastbound = allCells.Where(c => c.Eastbound && c.CanMove).ToList();
    countMoved += eastbound.Sum(c => c.Move());
    var southbound = allCells.Where(c => c.Southbound && c.CanMove).ToList();
    countMoved += southbound.Sum(c => c.Move());
    step++;
}
sw.Stop();
System.Console.WriteLine($"Done! {step} in {sw.ElapsedMilliseconds}ms");

static Cell[][] GetCellMatrix(char[][] numberMatrix)
{
    Cell[][] cells = new Cell[numberMatrix.Length][];

    for (int y = 0; y < numberMatrix.Length; y++)
    {
        var row = numberMatrix[y];
        cells[y] = new Cell[row.Length];
        for (int x = 0; x < row.Length; x++)
        {
            var value = numberMatrix[y][x];
            cells[y][x] = new Cell(value, x, y);
        }
    }

    foreach (var cell in cells.SelectMany(o => o))
    {
        var neighbours = GetAdjecentCells(cells, cell.Y, cell.X);
        cell.East = neighbours.east;
        cell.South = neighbours.south;
    }
    return cells;
}

static (Cell south, Cell east) GetAdjecentCells(Cell[][] cellMatrix, int y, int x)
{
    List<Cell> cells = new List<Cell>();
    var row = cellMatrix[y];
    Cell south;
    if (y < cellMatrix.Length - 1)
    {
        south = cellMatrix[y + 1][x];
    }
    else
    {
        south = cellMatrix[0][x];
    }
    Cell east;
    if (x < row.Length - 1)
    {
        east = cellMatrix[y][x + 1];
    }
    else
    {
        east = cellMatrix[y][0];
    }
    return (south, east);
}

public class Cell
{
    public char? Value { get; set; }

    public int X { get; }
    public int Y { get; }

    public bool Occupied { get { return Value.HasValue; } }

    public Cell South { get; set; }
    public Cell East { get; set; }

    public bool Eastbound
    {
        get
        {
            return Value == '>';
        }
    }
    public bool Southbound
    {
        get
        {
            return Value == 'v';
        }
    }

    public bool CanMove
    {
        get
        {
            return (Eastbound && !East.Occupied) || (Southbound && !South.Occupied);
        }
    }

    public Cell(char value, int x, int y)
    {
        if (value == '.')
        {
            Value = null;
        }
        else
        {
            Value = value;
        }
        X = x;
        Y = y;
    }
    
    public int Move()
    {
        if (CanMove)
        {
            if (Eastbound)
            {
                East.Value = this.Value;
            }
            else
            {
                South.Value = this.Value;
            }
            this.Value = null;
            return 1;
        }
        return 0;
    }
}