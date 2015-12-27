using System;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Players
{
    [Serializable]    
    public sealed class Player : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;
        private readonly Identifier _name; 
        private int _version;                       

        private Player(Guid id, int version, Identifier name)
            : base(new PlayerRegisteredEvent(id, version, name))
        {
            _id = id;
            _name = name;
            _version = version;
        }

        public override Guid Id
        {
            get { return _id; }
        }    
    
        public Identifier Name
        {
            get { return _name; }
        }

        protected override int Version
        {
            get { return _version; }
            set { _version = value;}
        }   
     
        public static Player Register(Guid id, Identifier name)
        {
            return new Player(id, 1, name);
        }
    }
}
