using System;
using System.Collections.Generic;
using System.Reflection;
using Backend.Common.MessagePropagator;
using Backend.Common.Reflection;
using Backend.UI.Tools;

namespace Editor.Factory
{
    public class ToolWindowFactory
    {
        public ToolWindowFactory(IMessagePropagator messagePropagator)
        {
            _messagePropagator = messagePropagator;
            
            CreateDI();
            
            GetToolWindows();
        }

        public IEnumerable<BaseToolWindow> GetDefaultFloatingTools()
        {
            var result = new List<BaseToolWindow>();

            foreach (var tool in _floatingTools)
            {
                result.Add(InstantiateTool(tool.Key, tool.Value));
            }
            
            return result;
        }

        public IEnumerable<BaseToolWindow> GetDefaultDocumentTools()
        {
            var result = new List<BaseToolWindow>();

            foreach (var tool in _documentTools)
            {
                result.Add(InstantiateTool(tool.Key, tool.Value));
            }

            return result;
        }

        private void GetToolWindows()
        {
            var toolWindows = ReflectionUtil
                .GetAllTypesWithAttribute<ToolWindowAttribute, BaseToolWindow>(AppDomain.CurrentDomain);

            foreach (var tool in toolWindows)
            {
                switch (tool.Value.ToolWindowType)
                {
                    case ToolWindowType.Floating:
                        _floatingTools.Add(tool.Key, tool.Value);
                        break;
                    case ToolWindowType.Document:
                        _documentTools.Add(tool.Key, tool.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private BaseToolWindow InstantiateTool(Type type, ToolWindowAttribute toolWindowAttribute)
        {
            BaseToolWindow toolWindow;
            
            if (!_instances.ContainsKey(type))
            {
                toolWindow = (BaseToolWindow) Activator.CreateInstance(type, GetDIArgsForObject(type))!;
                
                _instances.Add(type, new List<BaseToolWindow>() { toolWindow });
                return toolWindow;
            }

            if (toolWindowAttribute.ShareViewModelInstance)
            {
                return _instances[type][0];
            }

            toolWindow = (BaseToolWindow) Activator.CreateInstance(type, GetDIArgsForObject(type))!;
            _instances[type].Add(toolWindow);

            return toolWindow;
        }

        private object[] GetDIArgsForObject(Type type)
        {
            // Get the first constructor
            ConstructorInfo info = type.GetConstructors()[0];

            List<object> args = new();

            foreach (ParameterInfo param in info.GetParameters())
            {
                Type arg = param.ParameterType;
                
                if (_di.ContainsKey(arg))
                {
                    args.Add(_di[arg]);
                }
                else
                {
                    args.Add(null);
                }
            }

            return args.ToArray();
        }

        private void CreateDI()
        {
            _di.Add(typeof(IMessagePropagator), _messagePropagator);
        }

        private IMessagePropagator _messagePropagator;
        private Dictionary<Type, object> _di = new();
        
        private Dictionary<Type, ToolWindowAttribute> _documentTools = new();
        private Dictionary<Type, ToolWindowAttribute> _floatingTools = new();

        private Dictionary<Type, List<BaseToolWindow>> _instances = new();
    }
}