namespace Kingo.Messaging
{
    internal sealed class RunCommand
    {
        private readonly string _name;

        public RunCommand(string name)
        {
            _name = name;
        }

        public override string ToString() =>
            ToString(GetType().FriendlyName(), _name);
        
        private static string ToString(string type, string name) =>
            string.IsNullOrEmpty(name) ? type : $"{type} [Name = {name}]";
    }
}
