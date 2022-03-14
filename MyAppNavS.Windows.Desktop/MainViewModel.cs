using CommunityToolkit.Mvvm.ComponentModel;

namespace MyAppNavS
{
    public partial class MainViewModel:ObservableObject
    {
        [ObservableProperty]
        private string text;
    }
}