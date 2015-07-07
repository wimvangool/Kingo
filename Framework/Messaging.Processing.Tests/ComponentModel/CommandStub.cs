using Syztem.ComponentModel.DataAnnotations;

namespace Syztem.ComponentModel
{
    public sealed class CommandStub : RequestMessageViewModel<CommandStub>
    {
        public CommandStub() { }

        private CommandStub(CommandStub message, bool makeReadOnly) : base(makeReadOnly)
        {
            _value = message._value;
        }

        #region [====== Value ======]

        private string _value;

        [RequiredMember]
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
