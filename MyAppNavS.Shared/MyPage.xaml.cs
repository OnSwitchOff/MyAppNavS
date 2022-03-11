
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using Microsoft.UI.Xaml.Controls;

namespace MyAppNavS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyPage : Page
    {
        public MyPage()
        {
            this.InitializeComponent();
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
