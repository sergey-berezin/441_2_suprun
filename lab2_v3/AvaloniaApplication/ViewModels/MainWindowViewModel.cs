using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using llilyshkall.GeneticAlgorithm;
using ReactiveUI;

namespace AvaloniaApplication.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public ObservableCollection<Sizes> Sizes { get; }
    public GeneticAlgorithm Ga;
    public MainWindowViewModel()
    {
        Sizes = new ObservableCollection<Sizes>
        {
            new Sizes(1, 1), 
            new Sizes(1, 1), 
            new Sizes(2, 3), 
            new Sizes(2, 1), 
            new Sizes(3, 3),
            new Sizes(3, 4)
        };
    }
    
    public void RemoveItem(Sizes item)
    {
        if (Sizes.Contains(item))
        {
            Sizes.Remove(item);
        }
    }

    public void NewPopulation()
    {
        Ga = new GeneticAlgorithm(Sizes.ToArray());
        Console.WriteLine(Ga.ToString());
    }

    public void NewGeneration()
    {
        Ga.NewGeneration();
        Console.WriteLine(Ga.ToString());
    }
}

