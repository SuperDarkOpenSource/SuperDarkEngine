using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Backend.Common.MessagePropagator;
using Backend.Common.Reflection;

namespace Editor.MenuBar.Reflection
{
    public class MenuBarFactory
    {
        
        public static List<MenuItemViewModel> GetAllMenuBarItems(IMessagePropagator messagePropagator)
        {
            var args = new object[] { messagePropagator };
            
            var types = ReflectionUtil
                .GetAllTypesWithAttribute<MenuBarItemAttribute, 
                    BaseMenuBarItem>(AppDomain.CurrentDomain);

            var menus = new Dictionary<string, MenuItemViewModel>();

            foreach (var item in types)
            {
                try
                {
                    MenuItemViewModel menuItemViewModel = new MenuItemViewModel
                    {
                        Name = item.Value.DisplayName,
                        SortOrder = item.Value.Order,
                        Command = (Activator.CreateInstance(item.Key, args) as BaseMenuBarItem)?.Command
                    };

                    Debug.Assert(menuItemViewModel.Command != null);

                    if (!menus.ContainsKey(item.Value.CategoryName))
                    {
                        MenuItemViewModel root = new MenuItemViewModel
                        {
                            Name = "_" + item.Value.CategoryName,
                            Command = null,
                            SortOrder = 0,
                            Children = new List<MenuItemViewModel>()
                        };

                        menus.Add(item.Value.CategoryName, root);
                    }

                    menus[item.Value.CategoryName].Children.Add(menuItemViewModel);
                }
                // Ignore MenuBarItems that fail to build (unusual constructor)
                catch (Exception) { }
            }

            var result = menus.Select(e => e.Value)
                .OrderBy(e=> e.SortOrder).ToList();

            // Sort the children elements before returning
            result.ForEach(e => e.Children?.Sort(
                (a, b) => (a.SortOrder > b.SortOrder) ? 1 : -1));
            
            return result;
        }
    }
}