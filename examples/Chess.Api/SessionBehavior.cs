using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Kingo.Samples.Chess
{
    public sealed class SessionBehavior : IEndpointBehavior, IClientMessageInspector, IDispatchMessageInspector
    {
        #region [====== ExtensionElement ======]

        public sealed class ExtensionElement : BehaviorExtensionElement
        {
            public override Type BehaviorType
            {
                get { return typeof(SessionBehavior); }
            }

            protected override object CreateBehavior()
            {
                return new SessionBehavior();
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
                request.Headers.Add(NewSessionHeader(session));
            }
            return null;
        }

        private static MessageHeader NewSessionHeader(Session session)
        {
            return MessageHeader.CreateHeader(Session.HeaderName, Session.HeaderNamespace, session);
        }

        public void AfterReceiveReply(ref Message reply, object correlationState) { }

        #endregion

        #region [====== DispatchMessageInspector ======]

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {            
            Session session;

            if (TryGetSessionHeader(request.Headers, out session))
            {
                Session.CreateSessionScope(session.PlayerId, session.PlayerName);
            }            
            return null;
        }        

        public void BeforeSendReply(ref Message reply, object correlationState) { }

        private static bool TryGetSessionHeader(MessageHeaders headers, out Session session)
        {
            var headerIndex = headers.FindHeader(Session.HeaderName, Session.HeaderNamespace);
            if (headerIndex >= 0)
            {
                session = headers.GetHeader<Session>(headerIndex);
                return true;
            }
            session = null;
            return false;
        }

        #endregion
    }
}
