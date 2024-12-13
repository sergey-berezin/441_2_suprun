using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace AvaloniaApplication.ViewModels;

public class BackgroundProcess
{
    private CancellationTokenSource _cts;
    private System.Threading.Tasks.Task _task;
    public bool IsRunning => _task != null && !_cts.Token.IsCancellationRequested;
    private MainWindowViewModel _viewModel;
    private Action _drawRectanglesClick;
    public async System.Threading.Tasks.Task Start(MainWindowViewModel viewModel, Action drawRectanglesClick)
    {
        _cts = new CancellationTokenSource();
        _viewModel = viewModel;
        _drawRectanglesClick = drawRectanglesClick;
        _task = System.Threading.Tasks.Task.Factory.StartNew(async () =>
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await ProcessButtonClick(_cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Фоновый процесс был прерван");
            }
        }, TaskCreationOptions.LongRunning);

    }

    private async System.Threading.Tasks.Task ProcessButtonClick(CancellationToken token)
    {
        Console.WriteLine("Кнопка нажата в фоновом процессе");
        int count = 0;
        int area = 0;
        while (!token.IsCancellationRequested && count++ < 100)
        {
            _viewModel.NewGeneration();
            if (area != _viewModel.Area)
            {
                area = _viewModel.Area;
                count = 0;
            }
            Console.WriteLine($"{count}, {area}");
            try
            {
                Dispatcher.UIThread.Post(() =>
                {
                    _drawRectanglesClick();
                });
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
            }

            Console.WriteLine($"{count}, {area}");
            await System.Threading.Tasks.Task.Delay(100);
        }

        Stop();
    }

    public void Stop()
    {
        _cts?.Cancel();
    }
}
