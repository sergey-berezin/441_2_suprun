using System;
using System.Collections.Generic;
using Avalonia.Controls;
using System.Linq;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaApplication.ViewModels;
using llilyshkall.GeneticAlgorithm;

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
}