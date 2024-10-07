namespace llilyshkall.GeneticAlgorithm;

public class Rnd
{
  public static Random random = new Random();
}

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x = 0, int y = 0)
    {
        X = x;
        Y = y;
    }

    public Point(Point other)
    {
        X = other.X;
        Y = other.Y;
    }
}

public class Sizes
{
    public int Length {get; set;}
    public int Width {get; set;}
    public Sizes(int l = 0, int w = 0) 
    {
      Length = l;
      Width = w;
    }
    public Sizes(Sizes other)
    {
      Length = other.Length;
      Width = other.Width;
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
    LeftBottom = new Point(0,0);
    Sizes = new Sizes(0,0);
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
    LeftBottom.X += Rnd.random.Next(2 * dx) - dx;
    if (LeftBottom.X < 0) LeftBottom.X = 0;
    LeftBottom.Y += Rnd.random.Next(2 * dy) - dy;
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
    LeftBottom.X = Rnd.random.Next(maxX);
    LeftBottom.Y = Rnd.random.Next(maxY);
  }
}

public class Chromosome
{
  public Rectangle[] Rectangles {get;set;}
  public int Length => Rectangles.Length;
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
        Rectangles[i].Rotate();
        if (!CheckNoOverlap(Length)) Rectangles[i].Rotate();
      }
      if (Random.Shared.NextDouble() < mutationRate)
      {
        Rectangles[i].RandomMove(maxLength, maxWidth);
        while(!CheckNoOverlap(Length)) Rectangles[i].RandomMove(maxLength, maxWidth);
      }
    }
  }
  public string ToString()
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
      if (r.Sizes.Width == 1 && r.Sizes.Length == 1)
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
  private const double MUTATION_RATE = 0.3;
  private const int POPULATION_SIZE = 50;
  public List<Chromosome> Populations{get; set;}
  public GeneticAlgorithm(Sizes[] sizes)
  {
    Populations = new List<Chromosome>(){ new Chromosome(sizes) };
    Populations[0].RandomGenerate();
    for (int i = 1; i < POPULATION_SIZE; i++)
      Populations.Add(new Chromosome(Populations[0]));
    for (int i = 0; i < POPULATION_SIZE; i++)
      Populations[i].RandomGenerate();
  }
  public void NewGeneration()
  {
    Selection();
    Mutation();
  }
  private void Selection()
  {
    int bestArea = BestArea();
    
    for (int i = 0; i < Populations.Count; i++)
    {
      double p = Rnd.random.NextDouble();
      double q = (double)bestArea / Populations[i].Area;
      if (p > q)
      {
        Populations.RemoveAt(i);
        i--;
      }
    }
  }
  private void Mutation()
  {
    int size = Populations.Count;
    int bestArea = BestArea();
    int argBestArea = 0;
    for (int i = 0; i < Populations.Count; i++)
    {
      if (bestArea == Populations[i].Area) argBestArea = i;
    }
    for (int i = size; i < POPULATION_SIZE; i++) {
      Chromosome p = new Chromosome(Populations[argBestArea]);
      p.Mutate(MUTATION_RATE);
      Populations.Add(p);
    }
  }
  public int BestArea()
  {
    int ret = Populations[0].Area;
    foreach (var p in Populations)
    {
      ret = Math.Min(p.Area, ret);
    }
    return ret;
  }
  public new string ToString()
  {
    int bestArea = BestArea();
    foreach (var p in Populations)
    {
      if (p.Area == bestArea)
      {
        return p.ToString();
      }
    }

    return "error";
  }
}
