using System;

namespace Editor.MenuBar.Reflection
{
    public class MenuBarItemAttribute : Attribute
    {
        public MenuBarItemAttribute(string displayName, string categoryName, int order)
        {
            DisplayName = displayName;

            CategoryName = categoryName;

            Order = order;
        }
        
        public string DisplayName { get; private set; }
        
        public string CategoryName { get; private set; }
        
        public int Order { get; private set; }
    }
}