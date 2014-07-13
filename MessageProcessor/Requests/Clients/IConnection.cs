using System;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    public interface IConnection : IDisposable
    {
        void Open();

        void Close();
    }
}
