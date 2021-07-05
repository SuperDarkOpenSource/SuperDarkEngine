using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data;
using Backend.Common.MessagePropagator;
using Backend.UI.Tools;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;
using Editor.MenuBar.Reflection;
using Editor.ViewModels;
using ReactiveUI;

namespace Editor.Dock
{
    public class ToolDockFactory : Factory
    {
        public ToolDockFactory(IMessagePropagator messagePropagator, Hashtable dependenciesTable)
        {
            _messagePropagator = messagePropagator;
            _DI = dependenciesTable;

            RegisterToolWindows();
        }

        public override IRootDock CreateLayout()
        {

            var leftFloating = GetDefaultFloatingWindowsByPostion(DefaultFloatingPostion.Left);
            var rightFloating = GetDefaultFloatingWindowsByPostion(DefaultFloatingPostion.Right);

            var listDocuments = new List<IDockable>();
            foreach (var item in _defaultDocumentTools)
            {
                listDocuments.Add(ConstructToolWindow(item.Value));
            }

            var documentDock = new DocumentDock()
            {
                Id = "DocumentsPane",
                Title = "DocumentsPane",
                Proportion = double.NaN,
                ActiveDockable = (listDocuments.Count > 0) ? listDocuments[0] : null,
                VisibleDockables = listDocuments,
                IsCollapsable = false
            };
            
            var horizontalDockables = CreateList<IDockable>(
                documentDock
            );

            if (leftFloating.Count > 0)
            {
                horizontalDockables.Insert(0, new ToolDock()
                {
                    Id="LeftPaneTop",
                    Title = "LeftPaneTop",
                    Proportion = 1/4D,
                    ActiveDockable = leftFloating[0],
                    VisibleDockables = leftFloating
                });
                
                horizontalDockables.Insert(1, new SplitterDockable()
                {
                    Id = "LeftSplitter",
                    Title = "LeftSplitter"
                });
            }

            if (rightFloating.Count > 0)
            {
                horizontalDockables.Add(new SplitterDockable()
                {
                    Id = "RightSplitter",
                    Title = "RightSplitter"
                });
                
                horizontalDockables.Add(new ToolDock()
                {
                    Id = "RightPaneTop",
                    Title = "RightPaneTop",
                    Proportion = 1/4D,
                    ActiveDockable = rightFloating[0],
                    VisibleDockables = rightFloating
                });
            }

            IDockable horizontalDock = new ProportionalDock()
            {
                Id = "LeftPane",
                Title = "LeftPane",
                Proportion = double.NaN,

                Orientation = Orientation.Horizontal,
                ActiveDockable = null,
                VisibleDockables = horizontalDockables
            };
            
            var mainVisibleDockables = CreateList<IDockable>(horizontalDock);

            var mainLayout = new ProportionalDock
            {
                Id = "MainLayout",
                Title = "MainLayout",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                ActiveDockable = null,
                VisibleDockables = mainVisibleDockables
                
            };
            
            var rootViewModel = new RootViewModel()
            {
                Id = "Main",
                Title = "Main",
                ActiveDockable = mainLayout,
                VisibleDockables = CreateList<IDockable>(mainLayout)
            };

            var root = CreateRootDock();

            root.Id = "Root";
            root.Title = "Root";
            root.ActiveDockable = rootViewModel;
            root.DefaultDockable = rootViewModel;
            root.VisibleDockables = CreateList<IDockable>(rootViewModel);

            _rootDock = root;
            
            return root;
        }

        public override void InitLayout(IDockable layout)
        {
            this.ContextLocator = new Dictionary<string, Func<object>>
            {
                [nameof(IRootDock)] = () => _derp,
                [nameof(IProportionalDock)] = () => _derp,
                [nameof(IDocumentDock)] = () => _derp,
                [nameof(IToolDock)] = () => _derp,
                [nameof(ISplitterDockable)] = () => _derp,
                [nameof(IDockWindow)] = () => _derp,
                [nameof(IDocument)] = () => _derp,
                [nameof(ITool)] = () => _derp,
                ["LeftPane"] = () => _derp,
                ["LeftPaneTop"] = () => _derp,
                ["LeftPaneTopSplitter"] = () => _derp,
                ["LeftPaneBottom"] = () => _derp,
                ["RightPane"] = () => _derp,
                ["RightPaneTop"] = () => _derp,
                ["RightPaneTopSplitter"] = () => _derp,
                ["RightPaneBottom"] = () => _derp,
                ["DocumentsPane"] = () => _derp,
                ["MainLayout"] = () => _derp,
                ["LeftSplitter"] = () => _derp,
                ["RightSplitter"] = () => _derp,
                ["MainLayout"] = () => _derp,
                ["Main"] = () => _derp,
                ["Main"] = () => _derp,
                ["VerticalPane"] = () => _derp,
                ["BottomPane"] = () => _derp,
                ["BottomSplitter"] = () => _derp,
                ["TopPane"] = () => _derp,
                ["TopSplitter"] = () => _derp,
                ["WindowDocument"] = () => _derp,
            };

            foreach (var item in _floatingToolWindows)
            {
                ContextLocator.Add(item.Key, () => ConstructToolView(item.Value.ToolWindowAttribute.ViewType));
            }
            
            foreach (var item in _documentToolWindows)
            {
                ContextLocator.Add(item.Key, () => ConstructToolView(item.Value.ToolWindowAttribute.ViewType));
            }

            this.HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () =>
                {
                    var hostWindow = new HostWindow()
                    {
                        [!HostWindow.TitleProperty] = new Binding("ActiveDockable.Title")
                    };
                    return hostWindow;
                }
            };

            // No fucking clue what this does...
            // Dock.Avalonia has shit documentation.
            this.DockableLocator = new Dictionary<string, Func<IDockable>>
            {
            };
            
            base.InitLayout(layout);
        }

        public MenuItemViewModel GetMenuBarItems()
        {
            MenuItemViewModel floatingToolsMenuItem = new MenuItemViewModel()
            {
                Name = "Tool Windows",
                Command = null,
                CommandParameter = null,
                SortOrder = 0,
                Children = new List<MenuItemViewModel>()
            };

            int floatingOrder = 0;
            foreach (var item in _floatingToolWindows)
            {
                floatingToolsMenuItem.Children.Add(new MenuItemViewModel()
                {
                    Name = item.Key,
                    Command = ReactiveCommand.CreateFromTask<object>(InstantiateNewToolWindow),
                    CommandParameter = item.Value,
                    Children = null,
                    SortOrder = floatingOrder++
                });
            }

            MenuItemViewModel documentToolMenuItems = new MenuItemViewModel()
            {
                Name = "Document Windows",
                Command = null,
                CommandParameter = null,
                SortOrder = 1,
                Children = new List<MenuItemViewModel>()
            };

            int documentOrder = 0;
            foreach (var item in _documentToolWindows)
            {
                documentToolMenuItems.Children.Add(new MenuItemViewModel()
                {
                    Name = item.Key,
                    Command = ReactiveCommand.CreateFromTask<object>(InstantiateNewToolWindow),
                    CommandParameter = item.Value,
                    Children = null,
                    SortOrder = documentOrder++
                });
            }

            return new MenuItemViewModel()
            {
                Name = "View",
                Command = null,
                CommandParameter = null,
                Children = new List<MenuItemViewModel>(){ floatingToolsMenuItem, documentToolMenuItems },
                SortOrder = 5
            };
        }

        private Task InstantiateNewToolWindow(object parameter)
        {
            ToolWindowFactoryInfo factoryInfo = (ToolWindowFactoryInfo)parameter;

            BaseToolWindow toolWindow = ConstructToolWindow(factoryInfo);

            IDockable toolWindowDock = null;
            
            if (factoryInfo.ToolWindowAttribute.ToolWindowType == ToolWindowType.Document)
            {
                toolWindowDock = new DocumentDock()
                {
                    Proportion = Double.NaN,
                    ActiveDockable = toolWindow,
                    VisibleDockables = CreateList<IDockable>(toolWindow)
                };
            }
            else
            {
                toolWindowDock = new ToolDock()
                {
                    Proportion = Double.NaN,
                    ActiveDockable = toolWindow,
                    VisibleDockables = CreateList<IDockable>(toolWindow)
                };
            }

            toolWindowDock.Id = factoryInfo.ToolWindowAttribute.DisplayName;
            toolWindowDock.Title = factoryInfo.ToolWindowAttribute.DisplayName;
            
            var window = CreateWindowFrom(toolWindowDock);

            window.Owner = _rootDock;
            window.Factory = this;
            
            // REEEEEEEEEEE why is there no fucking documentation for this fucking library?
            AddWindow(_rootDock, window);
            
            // DO NOT set this boolean to true. Don't do it. Trust me.
            window.Present(false);
            
            return Task.CompletedTask;
        }

        private void RegisterToolWindows()
        {
            ToolWindowReflectionUtil.GetAllToolWindows(AppDomain.CurrentDomain)
                .ForEach(e => AddToolWindow(e));
        }

        private void AddToolWindow(Type toolType)
        {
            if (!CanInjectToolConstructor(toolType))
            {
                return;
            }

            ToolWindowAttribute toolWindowAttribute =
                toolType.GetCustomAttribute<ToolWindowAttribute>();

            DefaultToolWindowAttribute defaultToolWindowAttribute =
                toolType.GetCustomAttribute<DefaultToolWindowAttribute>();

            ToolWindowFactoryInfo info = new ToolWindowFactoryInfo();
            info.Type = toolType;
            info.ToolWindowAttribute = toolWindowAttribute;
            info.DefaultToolWindowAttribute = defaultToolWindowAttribute;
            info.Instances = new List<BaseToolWindow>();

            if(toolWindowAttribute.ToolWindowType == ToolWindowType.Floating)
            {
                _floatingToolWindows.Add(toolWindowAttribute.DisplayName, info);
            }
            else
            {
                _documentToolWindows.Add(toolWindowAttribute.DisplayName, info);
            }

            if(defaultToolWindowAttribute != null)
            {
                if(toolWindowAttribute.ToolWindowType == ToolWindowType.Floating)
                {
                    _defaultFloatingTools.Add(toolWindowAttribute.DisplayName, info);
                }
                else
                {
                    _defaultDocumentTools.Add(toolWindowAttribute.DisplayName, info);
                }
            }
        }

        private List<IDockable> GetDefaultFloatingWindowsByPostion(DefaultFloatingPostion postion)
        {
            List<IDockable> result = new List<IDockable>();

            foreach (var toolWindowFactoryInfo in _defaultFloatingTools)
            {
                if (toolWindowFactoryInfo.Value.DefaultToolWindowAttribute.DefaultFloatingPostion == postion)
                {
                    result.Add(ConstructToolWindow(toolWindowFactoryInfo.Value));
                }
            }

            return result;
        }

        private bool CanInjectToolConstructor(Type type)
        {
            var constructors = type.GetConstructors();

            if(constructors.Length == 0)
            {
                return false;
            }

            var constructor = constructors[0];

            foreach(ParameterInfo parameterInfo in constructor.GetParameters())
            {
                if(GetDependency(parameterInfo.ParameterType) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private object GetDependency(Type dependency)
        {
            if(!_DI.ContainsKey(dependency))
            {
                return null;
            }

            return _DI[dependency];
        }

        private BaseToolWindow ConstructToolWindow(ToolWindowFactoryInfo factoryInfo)
        {
            if(factoryInfo.ToolWindowAttribute.ShareViewModelInstance)
            {
                if(factoryInfo.Instances.Count == 0)
                {
                    factoryInfo.Instances.Add(Activator.CreateInstance(factoryInfo.Type, 
                        GetConstructorArguments(factoryInfo.Type)) as BaseToolWindow);
                }

                return factoryInfo.Instances[0];
            }
            else
            {
                BaseToolWindow toolWindow = Activator.CreateInstance(factoryInfo.Type,
                    GetConstructorArguments(factoryInfo.Type)) as BaseToolWindow;

                factoryInfo.Instances.Add(toolWindow);

                return toolWindow;
            }
        }

        private object ConstructToolView(Type view)
        {
            return Activator.CreateInstance(view);
        }

        private object[] GetConstructorArguments(Type type)
        {
            List<object> resolvedDependencies = new List<object>();
            var constructor = type.GetConstructors()[0];

            foreach(ParameterInfo parameter in constructor.GetParameters())
            {
                resolvedDependencies.Add(GetDependency(parameter.ParameterType));
            }

            return resolvedDependencies.ToArray();
        }

        private struct ToolWindowFactoryInfo
        {
            public Type Type;
            public ToolWindowAttribute ToolWindowAttribute;
            public DefaultToolWindowAttribute DefaultToolWindowAttribute;
            public List<BaseToolWindow> Instances;
        }

        private IMessagePropagator _messagePropagator;
        private Hashtable _DI;
        private object _derp = new object();

        private IRootDock _rootDock = null;

        private Dictionary<string, ToolWindowFactoryInfo> _floatingToolWindows = new Dictionary<string, ToolWindowFactoryInfo>();
        private Dictionary<string, ToolWindowFactoryInfo> _documentToolWindows = new Dictionary<string, ToolWindowFactoryInfo>();
        private Dictionary<string, ToolWindowFactoryInfo> _defaultFloatingTools = new Dictionary<string, ToolWindowFactoryInfo>();
        private Dictionary<string, ToolWindowFactoryInfo> _defaultDocumentTools = new Dictionary<string, ToolWindowFactoryInfo>();
    }
}
