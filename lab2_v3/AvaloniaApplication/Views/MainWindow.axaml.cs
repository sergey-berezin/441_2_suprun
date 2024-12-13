using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using System.Linq;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaApplication.ViewModels;
using DynamicData;
using llilyshkall.GeneticAlgorithm;
using Grid = AvaloniaApplication.ViewModels.Grid;
using Rectangle = llilyshkall.GeneticAlgorithm.Rectangle;

namespace AvaloniaApplication.Views;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
    private BackgroundProcess _backgroundProcess;
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: Sizes item })
        {
            var viewModel = DataContext as MainWindowViewModel;
            viewModel?.RemoveItem(item);
        }
    }
    private void Button_Throw(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Button_NewPopulation(object? sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        viewModel?.NewPopulation();
        DrawRectangles_Click();
    }

    private void Button_NewGeneration(object? sender, RoutedEventArgs e)
    {
        if (_backgroundProcess == null || !_backgroundProcess.IsRunning)
        {
            _backgroundProcess = new BackgroundProcess();
            var viewModel = DataContext as MainWindowViewModel;
            _backgroundProcess.Start(viewModel, DrawRectangles_Click);
        }
        
    }
    private void DrawRectangles_Click()
    {
        // Очищаем Canvas перед рисованием
        DrawingCanvas.Children.Clear();
        var geneticAlgorithm = ((MainWindowViewModel)DataContext!).Ga;
        if (geneticAlgorithm == null) return;
    
        Chromosome p = geneticAlgorithm.BestChromosome();
        Point max = new Point(0, 0);
        foreach (Rectangle rectangle in p.Rectangles)
        {
            max.X = Math.Max(max.X, rectangle.LeftBottom.X + rectangle.Sizes.Length);
            max.Y = Math.Max(max.Y, rectangle.LeftBottom.Y + rectangle.Sizes.Width);
        }
    
        foreach (var r in p.Rectangles)
        {
            var rect = new Avalonia.Controls.Shapes.Rectangle
            {
                Width = r.Sizes.Length * 10,
                Height = r.Sizes.Width * 10,
                Fill = Brushes.Blue, // Цвет заливки
                Stroke = Brushes.Black, // Цвет рамки
                StrokeThickness = 2
            };

            // Устанавливаем позицию прямоугольника
            Canvas.SetLeft(rect, r.LeftBottom.X * 10);
            Canvas.SetTop(rect, r.LeftBottom.Y * 10);

            // Добавляем прямоугольник на Canvas
            DrawingCanvas.Children.Add(rect);
        }
        
    }


    private void Clear_Click(object? sender, RoutedEventArgs e)
    {
        if (_backgroundProcess != null && _backgroundProcess.IsRunning) _backgroundProcess.Stop();
        DrawingCanvas.Children.Clear();
        var viewModel = DataContext as MainWindowViewModel;
        viewModel?.Reset();
    }

    private void AddRectangle_Click(object? sender, RoutedEventArgs e)
    {
        decimal? l = Length.Value, w = Width.Value;
        if (l % 1 != 0 || l <= 0 || l >= 11 ||
            w % 1 != 0 || w <= 0 || w >= 11) return;
        var viewModel = DataContext as MainWindowViewModel;
        viewModel?.AddRectangle((int)Length.Value, (int)Width.Value);
        
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TextBoxName.Text)) return;
    
        var ga = ((MainWindowViewModel)DataContext!).Ga;
        if (ga == null) return;

        using (ViewModelBase db = new ViewModelBase())
        {
            var existingTask = db.Tasks.FirstOrDefault(t => t.Name == TextBoxName.Text);

            if (existingTask != null)
            {
                Console.WriteLine("Задача с таким именем уже существует");
                return;
            }

            Task t = new Task() { Name = TextBoxName.Text, Generation = ga.GenerationNumber};
            db.Tasks.Add(t);
            db.SaveChanges();
            foreach (var p in ga.Populations)
            {
                Grid grid = new Grid() { TaskId = t.Id };
                db.Grids.Add(grid);
                db.SaveChanges();
                foreach (var r in p.Rectangles)
                {
                    RectangleDb rect = new RectangleDb()
                    {
                        X = r.LeftBottom.X,
                        Y = r.LeftBottom.Y,
                        Width = r.Sizes.Width,
                        Length = r.Sizes.Length,
                        GridId = grid.Id
                    };
                    db.Rectangles.Add(rect);
                }
                db.SaveChanges();
            }
        }
        ((MainWindowViewModel)DataContext!).UpdateTask();
    }

    private void Button_Load(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: Task item })
        {
            var viewModel = DataContext as MainWindowViewModel;

            using (ViewModelBase db = new ViewModelBase())
            {
                var existingTask = db.Tasks.FirstOrDefault(t => t.Name == item.Name);
                if (existingTask == null)
                {
                    Console.WriteLine("такой задачи нет");
                    return;
                }

                var q = new GeneticAlgorithm();
                q.GenerationNumber = existingTask.Generation;
                var grids = db.Grids.Where(g => g.TaskId ==  existingTask.Id).ToList();
                foreach (var g in grids)
                {
                    var rectangles = db.Rectangles.Where(r => r.GridId == g.Id).ToList();
                    q.Populations.Add(new Chromosome(rectangles.Count));
                    for (int i = 0; i < rectangles.Count; i++)
                    {
                        q.Populations.Last().Rectangles[i] = new Rectangle(rectangles[i].Length, rectangles[i].Width, rectangles[i].X, rectangles[i].Y);;
                    }
                }

                viewModel.Ga = q;
            }
            DrawRectangles_Click();
        }
    }

    private void Button_Del(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: Task item })
        {
            using (ViewModelBase db = new ViewModelBase())
            {
                var existingTask = db.Tasks.FirstOrDefault(t => t.Name == item.Name);
                if (existingTask == null)
                {
                    Console.WriteLine("такой задачи нет");
                    return;
                }
                
                var grids = db.Grids.Where(g => g.TaskId ==  existingTask.Id).ToList();
                foreach (var g in grids)
                {
                    var rectangles = db.Rectangles.Where(r => r.GridId == g.Id).ToList();
                    for (int i = 0; i < rectangles.Count; i++)
                    {
                        db.Rectangles.Remove(rectangles[i]);
                    }
                    db.Grids.Remove(g);
                }
                db.Tasks.Remove(existingTask);
                db.SaveChanges();
            }
            
            ((MainWindowViewModel)DataContext!).UpdateTask();
        }
    }
}