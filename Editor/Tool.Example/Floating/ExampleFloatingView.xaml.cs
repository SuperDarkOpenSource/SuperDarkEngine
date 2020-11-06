using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tool.Example.Floating
{
    public class ExampleFloatingView : UserControl
    {
        public ExampleFloatingView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
