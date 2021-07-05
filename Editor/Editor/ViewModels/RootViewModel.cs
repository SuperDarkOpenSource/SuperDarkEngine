using Dock.Model;
using Dock.Model.Controls;

namespace Editor.ViewModels
{
    public class RootViewModel : RootDock
    {
        public override IDockable Clone()
        {
            var rootViewModel = new RootViewModel();
            
            CloneHelper.CloneDockProperties(this, rootViewModel);
            CloneHelper.CloneRootDockProperties(this, rootViewModel);

            return rootViewModel;
        }
    }
}