using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Kingo.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Framing;

namespace Kingo.Samples.Chess
{
    internal sealed class ServiceBus : IDisposable
    {
        private static readonly string _ConnectionString = ConfigurationManager.ConnectionStrings["RabbitMQ"].ConnectionString;
        private const string _EventsExchange = "Events";
        private const string _ChessInboxQueue = "Chess_Inbox";
        private const string _TypeInfoHeader = "type_info";

        private readonly ServiceHostManager _serviceHostManager;
        private readonly ConnectionFactory _connectionFactory;
        private readonly ServiceProcessor _processor;

        private RabbitMQ.Client.IConnection _connection;
        private IModel _receiverChannel;
        private bool _isDisposed;

        private ServiceBus()
        {
            _serviceHostManager = ServiceHostManager.CreateServiceHostManager();
            _connectionFactory = new ConnectionFactory
            { Uri = _ConnectionString };
            _processor = new ServiceProcessor();
        }        

        private void Start()
        {            
            _connection = _connectionFactory.CreateConnection();
            _connection.ConnectionBlocked += (s, e) => WriteError("Connection was blocked.");
            _connection.ConnectionShutdown += (s, e) => WriteError("Connection was closed.");

            _receiverChannel = StartListeningForEvents(_connection);
            _receiverChannel.ModelShutdown += (s, e) => WriteError("Channel was closed.");
            _serviceHostManager.Open();
        }

        private IModel StartListeningForEvents(RabbitMQ.Client.IConnection connection)
        {
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += HandleEvent;
            
            channel.ExchangeDeclare(_EventsExchange, ExchangeType.Fanout);
            channel.QueueDeclare(_ChessInboxQueue, false, true, true, null);
            channel.QueueBind(_ChessInboxQueue, _EventsExchange, string.Empty);
            channel.BasicConsume(_ChessInboxQueue, false, consumer);

            return channel;
        }        

        private void HandleEvent(object sender, BasicDeliverEventArgs e)
        {
            var typeToContractMap = TypeToContractMap.FullyQualifiedName;
            var typeInfo = Encoding.UTF8.GetString((byte[]) e.BasicProperties.Headers[_TypeInfoHeader]);
            var type = typeToContractMap.GetType(typeInfo);

            var body = Encoding.UTF8.GetString(e.Body);
            var @event = Serializer.Deserialize(body, type);            

            HandleEvent(@event);
        }

        private void HandleEvent(object @event)
        {
            lock (_processor)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                try
                {
                    Console.WriteLine("Received Event: {0}.", @event.GetType().Name);                    
                }
                finally
                {
                    Console.ResetColor();
                }
            }
            _processor.Handle(@event);
        }

        private void Stop()
        {            
            _serviceHostManager.Close();
            _receiverChannel.Close();
            _connection.Close();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            if (_connection != null)
            {
                _receiverChannel.Dispose();             
                _connection.Dispose();                
            }
            _serviceHostManager.Dispose();
            _isDisposed = true;
        }

        public void Publish(object @event)
        {
            if (_isDisposed)
            {
                throw NewInstanceDisposedException();
            }
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            if (_connection.IsOpen)
            {
                PublishEvent(@event);

                lock (_processor)
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    try
                    {                        
                        Console.WriteLine("Published message: {0}.", @event.GetType().Name);
                        return;
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                }
            }
            throw NewConnectionClosedException();
        }

        private void PublishEvent(object @event)
        {
            var typeToContractMap = TypeToContractMap.FullyQualifiedName;
            var typeInfo = typeToContractMap.GetContract(@event.GetType());
            var body = Encoding.UTF8.GetBytes(Serializer.Serialize(@event));            
            
            PublishEvent(body, typeInfo);
        }

        private void PublishEvent(byte[] body, string typeInfo)
        {
            var properties = new BasicProperties
            {
                Headers = new Dictionary<string, object>
                {
                    { _TypeInfoHeader, typeInfo }
                }
            };         
            using (var channel = _connection.CreateModel())
            {                
                channel.BasicPublish(_EventsExchange, string.Empty, properties, body);
            }
        }

        private static ServiceBus _Instance;

        public static ServiceBus Instance
        {
            get { return _Instance; }
        }                

        #region [====== Main ======]

        private static void Main(string[] args)
        {
            try
            {
                Run();
            }
            catch (BrokerUnreachableException)
            {
                WriteError("Could not connect to the RabbitMQ Server. Make sure you've installed RabbitMQ and the connection settings are correct.");
                WaitForExit();
            }  
            catch (Exception exception)
            {
                WriteError("Unexpected error occurred: " + exception.Message);
                WaitForExit();
            }
        }

        private static void Run()
        {
            using (_Instance = CreateNewInstance())
            {
                Console.WriteLine("Starting all services...");

                _Instance.Start();

                Console.WriteLine("All services have been started.");
                
                WaitForExit();

                _Instance.Stop();

                Console.WriteLine("All services have been shut down.");
            }
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            try
            {
                Console.WriteLine(message);                
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void WaitForExit()
        {
            Console.WriteLine("Press [enter] to exit...");
            Console.ReadLine();
        }

        #endregion

        private static ServiceBus CreateNewInstance()
        {
            return new ServiceBus();
        }       

        private static Exception NewInstanceDisposedException()
        {
            return new ObjectDisposedException(typeof(ServiceBus).Name);
        }

        private static Exception NewConnectionAlreadyOpenException()
        {
 	        throw new NotImplementedException();
        }

        private static Exception NewConnectionAlreadyClosedException()
        {
            throw new NotImplementedException();
        }

        private static Exception NewConnectionClosedException()
        {
            throw new NotImplementedException();
        }
    }
}
