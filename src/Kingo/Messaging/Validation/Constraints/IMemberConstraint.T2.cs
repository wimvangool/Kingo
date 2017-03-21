namespace Kingo.Messaging.Validation.Constraints
{
    internal interface IMemberConstraint<TValueIn, TValueOut>
    {
        IMemberConstraint<TValueIn, TOther> And<TOther>(IMemberConstraint<TValueOut, TOther> constraint);

        bool IsNotSatisfiedBy(Member<TValueIn> member, IErrorMessageReader reader, out Member<TValueOut> transformedMember);
    }
}
