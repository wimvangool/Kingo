using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberNameTransformation : IMemberTransformation
    {
        private readonly Func<string, string> _nameSelector;

        internal MemberNameTransformation(Func<string, string> nameSelector)
        {
            _nameSelector = nameSelector;
        }

        public MemberByTransformation Execute(MemberByTransformation member, Type newMemberType)
        {
            return member.Transform(newMemberType, _nameSelector);
        }
    }
}
