using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Backend.UI.Tools
{
    public class ToolWindowReflectionUtil
    {
        public static List<Type> GetAllToolWindows(AppDomain domain)
        {
            List<Type> list = new List<Type>();
            
            foreach (Assembly assembly in domain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    ToolWindowAttribute toolWindowAttribute = 
                        type.GetCustomAttribute<ToolWindowAttribute>();

                    if(toolWindowAttribute != null && 
                       type.IsSubclassOf(typeof(BaseToolWindow)))
                    {
                        list.Add(type);
                    }
                }
            }
            

            return list;
        }
    }
}
