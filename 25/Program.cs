using System.Diagnostics;

var lines = File.ReadAllLines(args[0]);
var sw = Stopwatch.StartNew();

var allCells = GetCellMatrix(lines.Select(lines => lines.ToCharArray()).ToArray());

int countMoved = 1;
int step = 0;

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

static Cell[] GetCellMatrix(char[][] numberMatrix)
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
    var arr = cells.SelectMany(o => o).ToArray();
    foreach (var cell in arr)
    {
        var neighbours = GetAdjecentCells(cells, cell.Y, cell.X);
        cell.East = neighbours.east;
        cell.South = neighbours.south;
    }
    return arr;
}

static (Cell south, Cell east) GetAdjecentCells(Cell[][] cellMatrix, int y, int x)
{
    Cell south = y < cellMatrix.Length - 1 ? cellMatrix[y + 1][x] : cellMatrix[0][x];
    Cell east = x < cellMatrix[y].Length - 1 ? cellMatrix[y][x + 1] : cellMatrix[y][0];
    return (south, east);
}

public class Cell
{
    public char? Value { get; set; }

    public int X { get; }
    public int Y { get; }

    public bool Occupied => Value.HasValue;

    public Cell South { get; set; }
    public Cell East { get; set; }

    public bool Eastbound => Value == '>';

    public bool Southbound => Value == 'v';

    public bool CanMove => (Eastbound && !East.Occupied) || (Southbound && !South.Occupied);

    public Cell(char value, int x, int y)
    {
        Value = value == '.' ? null : value;

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