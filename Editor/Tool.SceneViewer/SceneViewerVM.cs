using System;
using Backend.Common.MessagePropagator;
using Backend.UI.Renderer;
using Backend.UI.Tools;

namespace Tool.SceneViewer
{
    [ToolWindow("Scene", ToolWindowType.Document, typeof(SceneViewer))]
    [DefaultToolWindow()]
    public class SceneViewerVM : RendererBaseToolWindow
    {
        public SceneViewerVM(IMessagePropagator messagePropagator) :
            base(messagePropagator)
        {
        }
    }
}