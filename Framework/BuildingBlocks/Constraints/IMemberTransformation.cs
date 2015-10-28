using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal interface IMemberTransformation
    {
        MemberByTransformation Execute(MemberByTransformation member, Type newMemberType);
    }
}
