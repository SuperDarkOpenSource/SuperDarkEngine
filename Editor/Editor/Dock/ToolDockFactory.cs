using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Avalonia.Data;
using Backend.Common.MessagePropagator;
using Backend.Common.Tools;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using Editor.ViewModels;

namespace Editor.Dock
{
    class ToolDockFactory : Factory
    {
        public ToolDockFactory(IMessagePropagator messagePropagator, Hashtable dependenciesTable)
        {
            _messagePropagator = messagePropagator;
            _DI = dependenciesTable;

            RegisterToolWindows();
        }

        public override IDock CreateLayout()
        {

            var leftFloating = GetDefaultFloatingWindowsByPostion(DefaultFloatingPostion.Left);
            var rightFloating = GetDefaultFloatingWindowsByPostion(DefaultFloatingPostion.Right);

            var listDocuments = new List<IDockable>();
            foreach (var item in _defaultDocumentTools)
            {
                listDocuments.Add(ConstructToolWindow(item.Value));
            }
            
            var horizontalDockables = CreateList<IDockable>(
                new DocumentDock()
                {
                    Id = "DocumentsPane",
                    Title = "DocumentsPane",
                    Proportion = double.NaN,
                    ActiveDockable = (listDocuments.Count > 0) ? listDocuments[0] : null,
                    VisibleDockables = listDocuments
                }
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
                
                horizontalDockables.Insert(1, new SplitterDock()
                {
                    Id = "LeftSplitter",
                    Title = "LeftSplitter"
                });
            }

            if (rightFloating.Count > 0)
            {
                horizontalDockables.Add(new SplitterDock()
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
                [nameof(ISplitterDock)] = () => _derp,
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

        private Dictionary<string, ToolWindowFactoryInfo> _floatingToolWindows = new Dictionary<string, ToolWindowFactoryInfo>();
        private Dictionary<string, ToolWindowFactoryInfo> _documentToolWindows = new Dictionary<string, ToolWindowFactoryInfo>();
        private Dictionary<string, ToolWindowFactoryInfo> _defaultFloatingTools = new Dictionary<string, ToolWindowFactoryInfo>();
        private Dictionary<string, ToolWindowFactoryInfo> _defaultDocumentTools = new Dictionary<string, ToolWindowFactoryInfo>();
    }
}
