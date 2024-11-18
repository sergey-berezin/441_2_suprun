namespace llilyshkall.GeneticAlgorithm;

public class Point(int x = 0, int y = 0)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public Point(Point other) : this(other.X, other.Y)
    {
    }
}

public class Sizes(int l = 0, int w = 0)
{
    public int Length {get; set;} = l;
    public int Width {get; set;} = w;

    public Sizes(Sizes other) : this(other.Length, other.Width)
    {
    }
}

public class Rectangle
{
  public Point LeftBottom {get; set;}
  public Sizes Sizes {get; set;}
  public Rectangle(int l, int w, int x = 0, int y = 0)
  {
    LeftBottom = new Point(x, y);
    Sizes = new Sizes(l, w);
  }
  public Rectangle()
  {
    LeftBottom = new Point();
    Sizes = new Sizes();
  }
  public Rectangle(Rectangle other)
  {
    LeftBottom = new Point(other.LeftBottom);
    Sizes = new Sizes(other.Sizes);
  }
  public int Area => Sizes.Length * Sizes.Width;
  public void SetSize(Sizes sizes)
  {
    Sizes.Length = sizes.Length;
    Sizes.Width = sizes.Width;
  }
  public void Rotate() {
    (Sizes.Length, Sizes.Width) = (Sizes.Width, Sizes.Length);
  }
  public void RandomMove(int dx, int dy)
  {
    Random random = new Random();
    LeftBottom.X += random.Next(2 * dx) - dx;
    if (LeftBottom.X < 0) LeftBottom.X = 0;
    LeftBottom.Y += random.Next(2 * dy) - dy;
    if (LeftBottom.Y < 0) LeftBottom.Y = 0;
  }
  public bool CheckNoOverlap(Rectangle other)
  {
    // Проверяем, не находится ли левая часть одного прямоугольника справа от другого
    if (LeftBottom.X >= other.LeftBottom.X + other.Sizes.Length || other.LeftBottom.X >= LeftBottom.X + Sizes.Length)
      return true;

    // Проверяем, не находится ли верхняя часть одного прямоугольника ниже другой
    if (LeftBottom.Y >= other.LeftBottom.Y + other.Sizes.Width || other.LeftBottom.Y >= LeftBottom.Y + Sizes.Width)
      return true;

    return false;
  }
  public void RandomPosition(int maxX, int maxY)
  {
    Random random = new Random();
    LeftBottom.X = random.Next(maxX);
    LeftBottom.Y = random.Next(maxY);
  }
}

public class Chromosome
{
  public Rectangle[] Rectangles {get;set;}
  private int Length => Rectangles.Length;
  public Chromosome(Sizes[] sizes)
  {
    Rectangles = new Rectangle[sizes.Length];
    for (int i = 0; i < Rectangles.Length; i++)
    {
      Rectangles[i] = new Rectangle(sizes[i].Length, sizes[i].Width);
    }
  }
  public Chromosome(Chromosome other)
  {
    Rectangles = new Rectangle[other.Rectangles.Length];
    for (int i = 0; i < other.Rectangles.Length; i++)
    {
      Rectangles[i] = new Rectangle(other.Rectangles[i]);
    }
  }
  public void RandomGenerate()
  {
    int length = 0, width = 0;
    foreach (var r in Rectangles)
    {
      length += r.Sizes.Length;
      width += r.Sizes.Width;
    }
    length *= 2;
    width *= 2;
    for (int i = 0; i < Length; i++) 
    {
      Rectangles[i].RandomPosition(length*2, width*2);
      while (!CheckNoOverlap(i + 1))
      {
        Rectangles[i].RandomPosition(length*2, width*2);
      }
    }
  }
  private bool CheckNoOverlap(int n)
  {
    for (int i = 0; i < n - 1; i++) 
    {
      for (int j = i + 1; j < n; j++)
      {
        if (!Rectangles[i].CheckNoOverlap(Rectangles[j]))
        {
          return false;
        }
      }
    }
    return true;
  }
  public int Area
  {
    get {
      if (Length == 0) return 0;
      Point leftBottom = new Point(Rectangles[0].LeftBottom);
      Point rightTop = new Point(Rectangles[0].LeftBottom.X + Rectangles[0].Sizes.Length, Rectangles[0].LeftBottom.Y +Rectangles[0].Sizes.Width);
      foreach (var r in Rectangles)
      {
        leftBottom.X = Math.Min(leftBottom.X, r.LeftBottom.X);
        leftBottom.Y = Math.Min(leftBottom.Y, r.LeftBottom.Y);
        rightTop.X = Math.Max(rightTop.X, r.LeftBottom.X + r.Sizes.Length);
        rightTop.Y = Math.Max(rightTop.Y, r.LeftBottom.Y + r.Sizes.Width);
      }
      return (rightTop.X - leftBottom.X) * (rightTop.Y - leftBottom.Y);
    }
  }
  public void Mutate(double mutationRate)
  {
    int maxLength = 0, maxWidth = 0;
    foreach (var r in Rectangles)
    {
      maxLength = Math.Max(maxLength, r.Sizes.Length);
      maxWidth = Math.Max(maxWidth, r.Sizes.Width);
    }

    for (int i = 0;i < Length;i++)
    {
      if (Random.Shared.NextDouble() < mutationRate)
      {
        Rectangles[i].RandomMove(maxLength, maxWidth);
        while(!CheckNoOverlap(Length)) Rectangles[i].RandomMove(maxLength, maxWidth);
      }
    }
  }
  public override string ToString()
  {
    Point min = new Point(Rectangles[0].LeftBottom), max = new Point(Rectangles[0].LeftBottom.X +Rectangles[0].Sizes.Length, Rectangles[0].LeftBottom.Y +Rectangles[0].Sizes.Width);
    foreach (var r in Rectangles)
    {
      min.X = Math.Min(r.LeftBottom.X, min.X);
      min.Y = Math.Min(r.LeftBottom.Y, min.Y);
      max.X = Math.Max(r.LeftBottom.X + r.Sizes.Length, max.X);
      max.Y = Math.Max(r.LeftBottom.Y + r.Sizes.Width, max.Y);
    }
    char[,] grid = new char[max.Y - min.Y + 1, max.X - min.X + 1];
    for (int i = 0; i < max.Y - min.Y + 1; i++)
    {
      for (int j = 0; j < max.X - min.X + 1; j++)
      {
        grid[i, j] = ' ';
      }
    }

    foreach (var r in Rectangles)
    {
      if (r.Sizes is { Width: 1, Length: 1 })
      {
        grid[r.LeftBottom.Y - min.Y, r.LeftBottom.X - min.X] = '#';
      }

      if (r.Sizes.Length != 1)
      {
        for (int i = 0; i < r.Sizes.Length; i++)
        {
          grid[r.LeftBottom.Y - min.Y, r.LeftBottom.X + i - min.X] = '━';
          grid[r.LeftBottom.Y + r.Sizes.Width - min.Y - 1, r.LeftBottom.X + i - min.X] = '━';
        }
      }

      if (r.Sizes.Width != 1)
      {
        for (int i = 0; i < r.Sizes.Width; i++)
        {
          grid[r.LeftBottom.Y + i - min.Y, r.LeftBottom.X - min.X] = '┃';
          grid[r.LeftBottom.Y + i - min.Y, r.LeftBottom.X + r.Sizes.Length - min.X - 1] = '┃';
        }
      }
      
      if(r.Sizes.Width != 1 && r.Sizes.Length != 1)
      {
        grid[r.LeftBottom.Y- min.Y, r.LeftBottom.X- min.X] = '┗';
        grid[r.LeftBottom.Y + r.Sizes.Width - min.Y - 1, r.LeftBottom.X + r.Sizes.Length - min.X - 1] = '┓';
        grid[r.LeftBottom.Y + r.Sizes.Width - min.Y - 1, r.LeftBottom.X- min.X] = '┏';
        grid[r.LeftBottom.Y - min.Y, r.LeftBottom.X + r.Sizes.Length - min.X - 1] = '┛';
      }
    }
    string ret = "";
    ret += "┌";
    for (int j = 0; j < max.X - min.X + 1; j++ )
    {
      ret += "─";
    }
    ret += "┐\n";
    for (int i = max.Y - min.Y - 1; i >= 0; i-- )
    {
      ret += "│";
      for (int j = 0; j < max.X - min.X + 1; j++ )
      {
        ret += grid[i,j];
      }
      ret += "│";
      ret += "\n";
    }
    ret += "└";
    for (int j = 0; j < max.X - min.X + 1; j++ )
    {
      ret += "─";
    }
    ret += "┘";
    ret += $"\nArea: {Area}\nPoints:\n";
    foreach (var r in Rectangles)
    {
      ret += $"({r.LeftBottom.X - min.X}, {r.LeftBottom.Y - min.Y}), ({r.LeftBottom.X + r.Sizes.Width - min.X}, {r.LeftBottom.Y + r.Sizes.Length - min.Y})\n";
    }
    return ret;
  }
}

public class GeneticAlgorithm
{
  private const double MutationRate = 0.3;
  private const int PopulationSize = 50;
  private int _bestArea;
  private int _argBestArea;
  private int[] _areas;
  private readonly List<Chromosome> _populations;
  public GeneticAlgorithm(Sizes[] sizes)
  {
    _populations = [new Chromosome(sizes)];
    // создаем планировки
    for (int i = 0; i < PopulationSize - 1; i++)
      _populations.Add(new Chromosome(sizes));
      
    // начальная генерация (рандомизация) планировок, параллельно
    var tasks = new Task[PopulationSize];
    for (int i = 0; i < PopulationSize; i++)
    {
      tasks[i] = Task.Factory.StartNew(pi =>
      {
        int idx = (int)pi;
        _populations[idx].RandomGenerate();
      }, i);
    }
    
    Task.WaitAll(tasks);
  }
  public void NewGeneration()
  {
    Mutation();
    Selection();
  }
  public new string ToString()
  {
    CalculateAreas();
    return _populations[_argBestArea].ToString();
  }
  // вычисляем все площади и самую маленькую площадь параллельно
  private void CalculateAreas()
  {
    _bestArea = -1;
    _argBestArea = -1;
    _areas = new int[_populations.Count];
    var tasks = new Task[_populations.Count];
    for (int i = 0; i < _populations.Count; i++)
    {
      tasks[i] = Task.Factory.StartNew(pi =>
      {
        int idx = (int)pi;
        _areas[idx] = _populations[idx].Area;
      }, i);
    }
    Task.WaitAll(tasks);
    _bestArea = _areas[0];
    _argBestArea = 0;
    for (int i = 0; i < _populations.Count; i++)
    {
      if (_bestArea > _areas[i])
      {
        _bestArea = _areas[i];
        _argBestArea = i;
      }
    }
  }
  private void Selection()
  {
    CalculateAreas();
    for (int i = _populations.Count - 1; i >= 0; i--)
    {
      Random random = new Random();
      double p = random.NextDouble();
      double q = (double)_bestArea / _areas[i];
      if (p > q)
      {
        _populations.RemoveAt(i);
        i--;
      }
    }
  }
  private void Mutation()
  {
    CalculateAreas();
    int size = _populations.Count;
    
    for (int i = size; i < PopulationSize; i++) {
      Chromosome p = new Chromosome(_populations[_argBestArea]);
      _populations.Add(p);
    }

    int countTasks = PopulationSize - size;
    var tasks = new Task[countTasks];
    for (int i = 0; i < countTasks; i++)
    {
      tasks[i] = Task.Factory.StartNew(pi =>
      {
        int idx = (int)pi;
        _populations[idx].Mutate(MutationRate);
      }, i + size);
    }
    
    Task.WaitAll(tasks);
  }
  public Chromosome BestChromosome()
  {
    CalculateAreas();
    Chromosome ret = new Chromosome(_populations[_argBestArea]);
    Point min = new Point(ret.Rectangles[0].LeftBottom);
    foreach (var r in ret.Rectangles)
    {
      min.X = Math.Min(r.LeftBottom.X, min.X);
      min.Y = Math.Min(r.LeftBottom.Y, min.Y);
    }
    foreach (var r in ret.Rectangles)
    {
      r.LeftBottom.X -= min.X;
      r.LeftBottom.Y -= min.Y;
    }
    return ret;
  }
}
