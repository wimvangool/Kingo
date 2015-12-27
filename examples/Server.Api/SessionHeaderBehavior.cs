using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Kingo.Samples.Chess
{
    public sealed class SessionHeaderBehavior : IEndpointBehavior, IClientMessageInspector, IDispatchMessageInspector
    {
        #region [====== ExtensionElement ======]

        public sealed class ExtensionElement : BehaviorExtensionElement
        {
            public override Type BehaviorType
            {
                get { return typeof(SessionHeaderBehavior); }
            }

            protected override object CreateBehavior()
            {
                return new SessionHeaderBehavior();
            }
        }

        #endregion

        #region [====== EndpointBehavior ======]

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        }

        public void Validate(ServiceEndpoint endpoint) { }

        #endregion        

        #region [====== ClientMessageInspector ======]

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var session = Session.Current;
            if (session != null)
            {
                request.Headers.Add(NewSessionHeader(session.PlayerName));
            }
            return null;
        }

        private static MessageHeader NewSessionHeader(string playerName)
        {
            return MessageHeader.CreateHeader(SessionHeader.Name, SessionHeader.Namespace, new SessionHeader(playerName));
        }

        public void AfterReceiveReply(ref Message reply, object correlationState) { }

        #endregion

        #region [====== DispatchMessageInspector ======]

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            Debug.Assert(Session.Current == null);

            SessionHeader header;

            if (TryGetSessionHeader(request.Headers, out header))
            {
                Session.CreateSession(header.PlayerName);
            }            
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState) { }

        private static bool TryGetSessionHeader(MessageHeaders headers, out SessionHeader header)
        {
            var headerIndex = headers.FindHeader(SessionHeader.Name, SessionHeader.Namespace);
            if (headerIndex >= 0)
            {
                header = headers.GetHeader<SessionHeader>(headerIndex);
                return true;
            }
            header = null;
            return false;
        }

        #endregion
    }
}
