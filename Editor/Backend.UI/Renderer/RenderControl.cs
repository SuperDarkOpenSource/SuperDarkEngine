using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;

namespace Backend.UI.Renderer
{
    public class RenderControl : OpenGlControlBase
    {
        public RenderControl()
        {
            if (DataContext is RendererBaseToolWindow toolWindow)
            {
                toolWindow.RegisterRenderControlInstance(this);
            }
        }
        
        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            gl.ClearColor(0, 1, 0, 1);
            gl.Clear(GlConsts.GL_DEPTH_BUFFER_BIT | GlConsts.GL_COLOR_BUFFER_BIT);
        }

        public void RendererUpdateControl()
        {
            // Say that we need to repaint the OpenGLControl
            InvalidateVisual();
        }
    }
}