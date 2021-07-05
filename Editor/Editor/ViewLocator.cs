// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Backend.UI.Tools;
using Dock.Model.Core;
using Editor.ViewModels;

namespace Editor
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            
            Type type = null;

            if(data is BaseToolWindow toolWindow)
            {
                ToolWindowAttribute toolWindowAttribute = 
                    toolWindow.GetType().GetCustomAttribute<ToolWindowAttribute>();

                type = toolWindowAttribute.ViewType;
            }
            else
            {
                // I take no responsibility for the code in this else statement. 
                // Avalonia's default project generated this.
                // ~Russell

                var name = data.GetType().FullName.Replace("ViewModel", "View");
                type = Type.GetType(name);
            }


            if (type != null)
            {
                return (Control)Activator.CreateInstance(type);
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + data.GetType().FullName };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase || data is IDockable;
        }
    }
}