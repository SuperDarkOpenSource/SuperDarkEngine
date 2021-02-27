using System;

namespace Backend.UI.Tools
{
    public enum DefaultFloatingPostion
    {
        Left,

        Right,
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultToolWindowAttribute : Attribute
    {
        public DefaultToolWindowAttribute()
        {
            _floatingPos = DefaultFloatingPostion.Right;
        }

        /// <summary>
        /// Note for ToolWindows of type Document this argument is ignored.
        /// </summary>
        public DefaultFloatingPostion DefaultFloatingPostion
        {
            get => _floatingPos;
            set => _floatingPos = value;
        }

        private DefaultFloatingPostion _floatingPos;
    }
}
