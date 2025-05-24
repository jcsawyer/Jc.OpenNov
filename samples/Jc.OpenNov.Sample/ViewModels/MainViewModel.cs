using CommunityToolkit.Mvvm.ComponentModel;

namespace Jc.OpenNov.Sample.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
