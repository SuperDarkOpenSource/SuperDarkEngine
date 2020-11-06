using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tool.Example.Document
{
    public class ExampleDocumentView : UserControl
    {
        public ExampleDocumentView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}