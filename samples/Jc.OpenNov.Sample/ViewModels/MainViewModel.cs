using System;
using System.Windows.Input;
using Jc.OpenNov.Avalonia;
using ReactiveUI;

namespace Jc.OpenNov.Sample.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ICommand StarNfcCommand { get; }
    public ICommand StopNfcCommand { get; }
    
    public MainViewModel()
    {
        StarNfcCommand = ReactiveCommand.Create(StartNfc);
        StopNfcCommand = ReactiveCommand.Create(StopNfc);
        
        Avalonia.OpenNov.Current.OnDataRead += OnDataRead;
        Avalonia.OpenNov.Current.OnTagDetected += OnTagDetected;
        Avalonia.OpenNov.Current.OnError += OnError;
    }
    
    ~MainViewModel()
    {
        Avalonia.OpenNov.Current.OnDataRead -= OnDataRead;
        Avalonia.OpenNov.Current.OnTagDetected -= OnTagDetected;
        Avalonia.OpenNov.Current.OnError -= OnError;
    }
    
    private void OnDataRead(object? sender, Data.PenResult e)
    {
        if (e is Data.PenResult.Success success)
        {
            Serial = success.Data.Serial;
        }
    }
    
    private void OnTagDetected(object? sender, ITag? e)
    {
        var bytes = e?.GetId();
        if (bytes is null)
        {
            TagId = "Tag not found";
            return;
        }
        
        TagId = Convert.ToHexString(bytes);
    }
    
    private void OnError(object? sender, Exception e)
    {
        // Handle error event
    }
    
    private void StartNfc()
    {
        Avalonia.OpenNov.Current.MonitorNfc(stopCondition: null);
    }
    
    private void StopNfc()
    {
        Avalonia.OpenNov.Current.StopNfc();
        TagId = string.Empty;
        Serial = string.Empty;
    }

    private string _tagId;
    public string TagId
    {
        get => _tagId;
        set => this.RaiseAndSetIfChanged(ref _tagId, value);
    }

    private string _serial;
    public string Serial
    {
        get => _serial;
        set => this.RaiseAndSetIfChanged(ref _serial, value);
    }
}
