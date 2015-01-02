using System.ComponentModel.DataAnnotations;

namespace System.ComponentModel
{
    public sealed class CommandStub : RequestMessage<CommandStub>
    {
        public CommandStub() { }

        private CommandStub(CommandStub message, bool makeReadOnly) : base(makeReadOnly)
        {
            _value = message._value;
        }

        #region [====== Value ======]

        private string _value;

        [RequiredConstraint]
        public string Value
        {
            get { return _value; }
            set { SetValue(ref _value, value, () => Value); }
        }

        #endregion

        public override CommandStub Copy(bool makeReadOnly)
        {
            return new CommandStub(this, makeReadOnly);
        }
    }
}
