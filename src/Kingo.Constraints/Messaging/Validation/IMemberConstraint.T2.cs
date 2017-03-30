namespace Kingo.Messaging.Validation
{
    internal interface IMemberConstraint<TValueIn, TValueOut>
    {
        IMemberConstraint<TValueIn, TOther> And<TOther>(IMemberConstraint<TValueOut, TOther> constraint);

        bool IsNotSatisfiedBy(Member<TValueIn> member, IErrorMessageCollection reader, out Member<TValueOut> transformedMember);
    }
}
