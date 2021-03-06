using System;
using System.Threading.Tasks;
using Backend.Common.MessagePropagator;
using Backend.Messages.UI;
using Backend.UI.Tools;

namespace Backend.UI.Renderer
{
    public abstract class RendererBaseToolWindow : BaseToolWindow
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

            _messagePropagator.GetMessage<RendererCreated>().Publish(_renderWindowGuid);
        }

        private void RegisterMessages()
        {
            HandleSubscriptionOnDispose(_messagePropagator.GetMessage<RendererSwapImage>()
                .Subscribe(OnRenderSwapImageMessage, ThreadHandler.UIThread));
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

        protected override void OnDisposal()
        {
            Task.Run(
                async () => await _messagePropagator.GetMessage<RendererDestroyed>().Publish(_renderWindowGuid));
        }

        private Guid _renderWindowGuid;
        private RenderControl _renderControl = null;
    }
}