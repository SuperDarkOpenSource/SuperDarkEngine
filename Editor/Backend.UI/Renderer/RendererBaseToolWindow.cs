using System;
using System.Threading.Tasks;
using Backend.Common.MessagePropagator;
using Backend.Messages.UI;
using Backend.UI.Tools;

namespace Backend.UI.Renderer
{
    public abstract class RendererBaseToolWindow : BaseToolWindow, IDisposable
    {
        protected RendererBaseToolWindow(IMessagePropagator messagePropagator) : 
            base(messagePropagator)
        {
            CreateAndRegisterNewRenderer();
            
            RegisterMessages();
        }

        protected RendererBaseToolWindow(BaseToolWindow copy) : 
            base(copy)
        {
            CreateAndRegisterNewRenderer();
            
            RegisterMessages();
        }

        private void CreateAndRegisterNewRenderer()
        {
            _renderWindowGuid = Guid.NewGuid();

            _MessagePropagator.GetMessage<RendererCreated>().Publish(_renderWindowGuid);
        }

        private void RegisterMessages()
        {
            _rendererUpdateToken =
                _MessagePropagator.GetMessage<RendererSwapImage>().Subscribe(OnRenderSwapImageMessage);
        }

        /**
         * Do not call this. This method is called from the RenderControl to register itself with the VM
         */
        public void RegisterRenderControlInstance(RenderControl renderControl)
        {
            _renderControl = renderControl;
        }

        private Task OnRenderSwapImageMessage(Guid guid)
        {
            // We only care about our Renderer
            if (guid != _renderWindowGuid)
            {
                return Task.CompletedTask;
            }
            
            _renderControl.RendererUpdateControl();
            
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _MessagePropagator.GetMessage<RendererDestroyed>().Publish(_renderWindowGuid);
                _MessagePropagator.GetMessage<RendererSwapImage>().Unsubscribe(_rendererUpdateToken);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private Guid _renderWindowGuid;
        private ISubscriptionToken _rendererUpdateToken;
        private RenderControl _renderControl = null;
    }
}