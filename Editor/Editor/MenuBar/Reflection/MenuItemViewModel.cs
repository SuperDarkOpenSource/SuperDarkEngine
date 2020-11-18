using System.Collections.Generic;
using System.Windows.Input;

namespace Editor.MenuBar.Reflection
{
    public class MenuItemViewModel
    {
        public string Name { get; set; }
        
        public ICommand Command { get; set; }

        public object CommandParameter { get; set; } = null;

        public int SortOrder { get; set; }

        public List<MenuItemViewModel> Children { get; set; } = null;
    }
}