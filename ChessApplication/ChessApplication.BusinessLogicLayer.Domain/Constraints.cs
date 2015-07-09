namespace SummerBreeze.ChessApplication
{
    internal static class Constraints
    {
        internal const int UsernameMinLength = 4;
        internal const int UsernameMaxLength = 12;
        internal const string UsernameRegex = @"^[A-Za-z_][A-Za-z_0-9]*$";

        internal const int PasswordMinLength = 6;
        internal const int PasswordMaxLength = 15;
        internal const string PasswordRegex = @"^\S*$";
    }
}
