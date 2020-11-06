using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Common.Tools
{
    public enum ToolWindowType
    {
        /// <summary>
        /// Floating Tool Window. Think of Visual studio's Solution Explorer 
        /// window. That kind of dockable anywhere tool.
        /// </summary>
        Floating,

        /// <summary>
        /// This ToolWindow must live in the body of a Window. Think of
        /// the text part of a Text Window, or Unity Engine's Camera Window.
        /// </summary>
        Document
    }
}
