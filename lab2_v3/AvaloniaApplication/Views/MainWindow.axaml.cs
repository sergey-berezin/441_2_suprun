using System;
using System.Collections.Generic;
using Avalonia.Controls;
using System.Linq;
using Avalonia.Interactivity;
using Avalonia.Media;
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
        var viewModel = DataContext as MainWindowViewModel;
        viewModel?.NewGeneration();
        DrawRectangles_Click();
    }
    private void DrawRectangles_Click()
    {
        // Очищаем Canvas перед рисованием
        DrawingCanvas.Children.Clear();
        List<Chromosome> p = ((MainWindowViewModel)DataContext).Ga.Populations;
        int idx = 0;
        int bestArea = p[0].Area;
        for (int i = 1; i < p.Count; i++)
        {
            if (p[i].Area < bestArea)
            {
                idx = i;
                bestArea = p[i].Area;
            }
        }

        for (int i = 0; i < p[idx].Rectangles.Length; i++)
        {
            var rect = new Avalonia.Controls.Shapes.Rectangle
            {
                Width = p[idx].Rectangles[i].Sizes.Width*10,
                Height = p[idx].Rectangles[i].Sizes.Length*10,
                Fill = Brushes.Blue, // Цвет заливки
                Stroke = Brushes.Black, // Цвет рамки
                StrokeThickness = 2
            };

            // Устанавливаем позицию прямоугольника
            Canvas.SetLeft(rect, p[idx].Rectangles[i].LeftBottom.X*10);
            Canvas.SetTop(rect, p[idx].Rectangles[i].LeftBottom.Y*10);

            // Добавляем прямоугольник на Canvas
            DrawingCanvas.Children.Add(rect);
        }
    }
}