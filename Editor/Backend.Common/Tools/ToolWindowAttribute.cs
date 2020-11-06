using Avalonia;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Common.Tools
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToolWindowAttribute : Attribute
    {

        public ToolWindowAttribute(string displayName, ToolWindowType type, Type viewType, bool shouldShareViewModelInstance = true)
        {
            DisplayName = displayName;

            ToolWindowType = type;

            ViewType = viewType;

            ShareViewModelInstance = shouldShareViewModelInstance;
        }

        public string DisplayName { get; private set; }

        public ToolWindowType ToolWindowType { get; private set; }

        public Type ViewType { get; private set; }

        public bool ShareViewModelInstance { get; private set; }

        public Size MinSize { get; set; } = new Size(-1, -1);
        
    }
}
