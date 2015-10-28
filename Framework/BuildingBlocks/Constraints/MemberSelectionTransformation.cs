using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberSelectionTransformation : IMemberTransformation
    {
        private readonly Identifier _fieldOrProperty;

        internal MemberSelectionTransformation(Identifier fieldOrProperty)
        {
            _fieldOrProperty = fieldOrProperty;
        }

        public MemberByTransformation Execute(MemberByTransformation member, Type newMemberType)
        {
            return member.Transform(newMemberType, _fieldOrProperty);
        }
    }
}
