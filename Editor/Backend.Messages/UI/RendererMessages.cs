using System;
using Backend.Common.MessagePropagator;

namespace Backend.Messages.UI
{
    public class RendererCreated : Message<Guid> {}
    
    public class RendererSwapImage : Message<Guid> {}
    
    public class RendererDestroyed : Message<Guid> {}
}