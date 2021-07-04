using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Backend.Common.Reflection
{
    public static class ReflectionUtil
    {
        public static Dictionary<Type, AttributeType> 
            GetAllTypesWithAttribute<AttributeType, BaseType>(AppDomain domain) 
            where AttributeType : Attribute
        {
            Dictionary<Type, AttributeType> list = new Dictionary<Type, AttributeType>();

            foreach (Assembly assembly in domain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    AttributeType attribute = type.GetCustomAttribute<AttributeType>();

                    if (attribute != null && type.IsSubclassOf(typeof(BaseType)))
                    {
                        list.Add(type, attribute);
                    }
                }
            }

            return list;
        }
    }
}