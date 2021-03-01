using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tool.SceneViewer
{
    public class SceneViewer : UserControl
    {
        public SceneViewer()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}