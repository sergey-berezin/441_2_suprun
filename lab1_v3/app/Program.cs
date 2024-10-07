using llilyshkall.GeneticAlgorithm;

Sizes[] sizes = 
{
  new Sizes(1, 1), 
  new Sizes(1, 1), 
  new Sizes(2, 2), 
  new Sizes(2, 2), 
  new Sizes(3, 3)
};

// Sizes[] sizes = 
// {
//   new Sizes(1, 1), 
//   new Sizes(1, 1), 
//   new Sizes(2, 3), 
//   new Sizes(2, 1), 
//   new Sizes(3, 3),
//   new Sizes(3, 4)
// };

GeneticAlgorithm ga = new GeneticAlgorithm(sizes);

int generationsCount = 1;

while (true)
{
  Console.WriteLine($"Generation: {generationsCount++}");
  Console.WriteLine(ga.ToString());
  ga.NewGeneration();
  var cki = Console.ReadKey(true);
  if (cki.Key == ConsoleKey.Spacebar) break;
}

Console.WriteLine($"Best: {ga.BestArea()}\n");