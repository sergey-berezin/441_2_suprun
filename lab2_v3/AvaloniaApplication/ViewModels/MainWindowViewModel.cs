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

// Decompiled with JetBrains decompiler
// Type: llilyshkall.GeneticAlgorithm.GeneticAlgorithm
// Assembly: lib, Version=2.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: C0A9BB4B-5C71-4320-B082-DC319EB0C716
// Assembly location: /Users/isuprun/.nuget/packages/llilyshkall.geneticalgorithm/2.0.1/lib/net8.0/lib.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class MainWindowViewModel : ReactiveObject
{
    public ObservableCollection<Sizes> Sizes { get; }
    public GeneticAlgorithm? Ga = null;
    private int _generation;
    public int Generation
    {
        get => _generation;
        set => this.RaiseAndSetIfChanged(ref _generation, value); 
    }

    private int _area;
    public int Area
    {
        get => _area;
        set => this.RaiseAndSetIfChanged(ref _area, value); 
    }
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
        Area = Ga.BestChromosome().Area;
        Generation = 1;
    }

    public void NewGeneration()
    {
        if (Ga == null) return;
        Ga.NewGeneration();
        Area = Ga.BestChromosome().Area;
        Generation++;
    }

    public void Reset()
    {
        Ga = null;
        Area = 0;
        Generation = 0;
    }

    public void AddRectangle(int l, int w)
    {
        Sizes.Add(new Sizes(w, l));
    }
}



