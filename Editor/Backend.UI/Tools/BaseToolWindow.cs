using System;
using System.Collections.Generic;
using Backend.Common.MessagePropagator;
using System.Reflection;
using ReactiveUI;

namespace Backend.UI.Tools
{
    public class BaseToolWindow : ReactiveObject, IDisposable
    {
        protected BaseToolWindow(IMessagePropagator messagePropagator)
        {
            _messagePropagator = messagePropagator;

            ToolWindowAttribute toolWindowAttribute =
                this.GetType().GetCustomAttribute<ToolWindowAttribute>();
            
            ToolName = toolWindowAttribute?.DisplayName;

            //Id = toolWindowAttribute.DisplayName;
            //Title = toolWindowAttribute.DisplayName;
        }
        
        public string ToolName { get; }

        protected BaseToolWindow(BaseToolWindow copy)
        {
            _messagePropagator = copy._messagePropagator;
        }
        
        /// <summary>
        /// This handles the Unregistering of Message Subscribers when this VM is destroyed.
        /// </summary>
        /// <param name="subscriptionToken">The ISubscriptionToken to unregister</param>
        protected void HandleSubscriptionOnDispose(ISubscriptionToken subscriptionToken)
        {
            _subscriptionTokens.Add(subscriptionToken);
        }

        protected virtual void OnDisposal() { }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                OnDisposal();
                
                foreach (ISubscriptionToken subscriptionToken in _subscriptionTokens)
                {
                    subscriptionToken.Unsubscribe();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected readonly IMessagePropagator _messagePropagator;
        private readonly List<ISubscriptionToken> _subscriptionTokens = new();
    }
}
