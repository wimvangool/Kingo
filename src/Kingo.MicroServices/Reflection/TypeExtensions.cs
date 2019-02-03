using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kingo.Reflection
{
    /// <summary>
    /// Contains several extensions method for the <see cref="Type" /> class.
    /// </summary>
    public static class TypeExtensions
    {
        #region [====== Equality / Inequality ======]

        /// <summary>
        /// Attempts to obtain the equality operator (==) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="equalityOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the equality operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetEqualityOperator<TIn1, TIn2>(this Type type, out Func<TIn1, TIn2, bool> equalityOperator) =>
             TryGetBinaryOperator(type, "op_Equality", out equalityOperator);

        /// <summary>
        /// Attempts to obtain the equality operator (!=) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="inequalityOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the inequality operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetInequalityOperator<TIn1, TIn2>(this Type type, out Func<TIn1, TIn2, bool> inequalityOperator) =>
             TryGetBinaryOperator(type, "op_Inequality", out inequalityOperator);

        #endregion

        #region [====== Comparison Operators ======]

        /// <summary>
        /// Attempts to obtain the less than operator (&lt;) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="lessThanOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the less than operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetLessThanOperator<TIn1, TIn2>(this Type type, out Func<TIn1, TIn2, bool> lessThanOperator) =>
             TryGetBinaryOperator(type, "op_LessThan", out lessThanOperator);

        /// <summary>
        /// Attempts to obtain the less than or equal to operator (&lt;=) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="lessThanOrEqualOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the less than or equal to operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetLessThanOrEqualOperator<TIn1, TIn2>(this Type type, out Func<TIn1, TIn2, bool> lessThanOrEqualOperator) =>
             TryGetBinaryOperator(type, "op_LessThanOrEqual", out lessThanOrEqualOperator);

        /// <summary>
        /// Attempts to obtain the greater than operator (&gt;) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="greaterThanOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the greater than operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetGreaterThanOperator<TIn1, TIn2>(this Type type, out Func<TIn1, TIn2, bool> greaterThanOperator) =>
             TryGetBinaryOperator(type, "op_GreaterThan", out greaterThanOperator);

        /// <summary>
        /// Attempts to obtain the greater than or equal to operator (&gt;=) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="greaterThanOrEqualOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the greater than or equal to operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetGreaterThanOrEqualOperator<TIn1, TIn2>(this Type type, out Func<TIn1, TIn2, bool> greaterThanOrEqualOperator) =>
             TryGetBinaryOperator(type, "op_GreaterThanOrEqual", out greaterThanOrEqualOperator);

        #endregion

        #region [====== Addition / Subtraction ======]

        /// <summary>
        /// Attempts to obtain the addition operator (+) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="additionOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the addition operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetAdditionOperator<TIn1, TIn2, TOut>(this Type type, out Func<TIn1, TIn2, TOut> additionOperator) =>
             TryGetBinaryOperator(type, "op_Addition", out additionOperator);

        /// <summary>
        /// Attempts to obtain the subtraction operator (-) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="subtractiontOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the subtraction operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetSubtractionOperator<TIn1, TIn2, TOut>(this Type type, out Func<TIn1, TIn2, TOut> subtractiontOperator) =>
             TryGetBinaryOperator(type, "op_Subtraction", out subtractiontOperator);

        #endregion

        #region [====== Multiply / Division ======]

        /// <summary>
        /// Attempts to obtain the multiply operator (*) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="multiplyOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the multiply operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetMultiplyOperator<TIn1, TIn2, TOut>(this Type type, out Func<TIn1, TIn2, TOut> multiplyOperator) =>
             TryGetBinaryOperator(type, "op_Multiply", out multiplyOperator);

        /// <summary>
        /// Attempts to obtain the division operator (/) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="divisionOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the division operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetDivisionOperator<TIn1, TIn2, TOut>(this Type type, out Func<TIn1, TIn2, TOut> divisionOperator) =>
             TryGetBinaryOperator(type, "op_Division", out divisionOperator);

        #endregion

        #region [====== Modulus / ExclusiveOr ======]

        /// <summary>
        /// Attempts to obtain the modulus operator (%) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="modulusOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the modulus operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetModulusOperator<TIn1, TIn2, TOut>(this Type type, out Func<TIn1, TIn2, TOut> modulusOperator) =>
             TryGetBinaryOperator(type, "op_Modulus", out modulusOperator);

        /// <summary>
        /// Attempts to obtain the exclusiveOr operator (^) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="exclusiveOrOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the exclusiveOr operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetExclusiveOrOperator<TIn1, TIn2, TOut>(this Type type, out Func<TIn1, TIn2, TOut> exclusiveOrOperator) =>
             TryGetBinaryOperator(type, "op_ExclusiveOr", out exclusiveOrOperator);

        #endregion

        #region [====== BitwiseAnd / BitwiseOr ======]

        /// <summary>
        /// Attempts to obtain the bitwiseAnd operator (&amp;) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="bitwiseAndOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the bitwiseAnd operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetBitwiseAndOperator<TIn1, TIn2, TOut>(this Type type, out Func<TIn1, TIn2, TOut> bitwiseAndOperator) =>
             TryGetBinaryOperator(type, "op_BitwiseAnd", out bitwiseAndOperator);

        /// <summary>
        /// Attempts to obtain the bitwiseOr operator (|) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn1">Type of the first argument.</typeparam>
        /// <typeparam name="TIn2">Type of the second argument.</typeparam>
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="bitwiseOrOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the bitwiseOr operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetBitwiseOrOperator<TIn1, TIn2, TOut>(this Type type, out Func<TIn1, TIn2, TOut> bitwiseOrOperator) =>
             TryGetBinaryOperator(type, "op_BitwiseOr", out bitwiseOrOperator);

        #endregion

        #region [====== LeftShift / RightShift ======]

        /// <summary>
        /// Attempts to obtain the leftShift operator (&lt;&lt;) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>        
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="leftShiftOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the leftShift operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetLeftShiftOperator<TIn, TOut>(this Type type, out Func<TIn, int, TOut> leftShiftOperator) =>
             TryGetBinaryOperator(type, "op_LeftShift", out leftShiftOperator);

        /// <summary>
        /// Attempts to obtain the rightShift operator (&gt;&gt;) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>        
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="rightShiftOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the rightShift operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetRightShiftOperator<TIn, TOut>(this Type type, out Func<TIn, int, TOut> rightShiftOperator) =>
             TryGetBinaryOperator(type, "op_RightShift", out rightShiftOperator);

        #endregion

        #region [====== UnaryPlus / UnaryNegation ======]

        /// <summary>
        /// Attempts to obtain the unaryPlus operator (+) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>        
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="unaryPlusOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the unaryPlus operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetUnaryPlusOperator<TIn, TOut>(this Type type, out Func<TIn, TOut> unaryPlusOperator) =>
             TryGetUnaryOperator(type, "op_UnaryPlus", out unaryPlusOperator);

        /// <summary>
        /// Attempts to obtain the unaryNegation operator (-) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>        
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="unaryNegationtOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the unaryNegation operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetUnaryNegationOperator<TIn, TOut>(this Type type, out Func<TIn, TOut> unaryNegationtOperator) =>
             TryGetUnaryOperator(type, "op_UnaryNegation", out unaryNegationtOperator);

        #endregion

        #region [====== LogicalNot / OnesComplement ======]

        /// <summary>
        /// Attempts to obtain the logicalNot operator (!) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>        
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="logicalNotOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the logicalNot operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetLogicalNotOperator<TIn, TOut>(this Type type, out Func<TIn, TOut> logicalNotOperator) =>
             TryGetUnaryOperator(type, "op_LogicalNot", out logicalNotOperator);

        /// <summary>
        /// Attempts to obtain the onesComplement operator (~) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>        
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="onesComplementOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the onesComplement operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetOnesComplementOperator<TIn, TOut>(this Type type, out Func<TIn, TOut> onesComplementOperator) =>
             TryGetUnaryOperator(type, "op_OnesComplement", out onesComplementOperator);

        #endregion

        #region [====== True / False ======]

        /// <summary>
        /// Attempts to obtain the true operator (true) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>                
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="trueOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the true operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetTrueOperator<TIn>(this Type type, out Func<TIn, bool> trueOperator) =>
             TryGetUnaryOperator(type, "op_True", out trueOperator);

        /// <summary>
        /// Attempts to obtain the false operator (false) defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>                
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="falseOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the false operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetFalseOperator<TIn>(this Type type, out Func<TIn, bool> falseOperator) =>
             TryGetUnaryOperator(type, "op_False", out falseOperator);

        #endregion

        #region [====== Implicit / Explicit ======]

        /// <summary>
        /// Attempts to obtain the implicit operator defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the argument.</typeparam>        
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="implicitOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the implicit operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetImplicitOperator<TIn, TOut>(this Type type, out Func<TIn, TOut> implicitOperator) =>
             TryGetUnaryOperator(type, "op_Implicit", out implicitOperator);

        /// <summary>
        /// Attempts to obtain the explicit operator defined within the specified <paramref name="type"/>
        /// that has the specified argument-types.
        /// </summary>
        /// <typeparam name="TIn">Type of the first argument.</typeparam>        
        /// <typeparam name="TOut">Return-type of the operator.</typeparam>
        /// <param name="type">Type in which the operator is defined.</param>
        /// <param name="explicitOperator">
        /// If this method returns <c>true</c>, refers to the operator; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is a closed type and the explicit operator with the exact
        /// argument types as specified has been defined within the specified <paramref name="type"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryGetExplicitOperator<TIn, TOut>(this Type type, out Func<TIn, TOut> explicitOperator) =>
             TryGetUnaryOperator(type, "op_Explicit", out explicitOperator);

        #endregion

        #region [====== Operator Search ======]

        private static bool TryGetUnaryOperator<TIn, TOut>(Type type, string name, out Func<TIn, TOut> unaryOperator)
        {
            var methods =
                from method in GetOperatorsOfType(type, name, typeof(TOut))
                where HasParameterOfType(method, typeof(TIn))
                select method;

            var unaryOperatorMethod = methods.SingleOrDefault();
            if (unaryOperatorMethod != null)
            {
                unaryOperator = value => (TOut) unaryOperatorMethod.Invoke(null, new object[] { value });
                return true;
            }
            unaryOperator = null;
            return false;
        }

        private static bool TryGetBinaryOperator<TIn1, TIn2, TOut>(Type type, string name, out Func<TIn1, TIn2, TOut> binaryOperator)
        {
            var methods =
                from method in GetOperatorsOfType(type, name, typeof(TOut))
                where HasParametersOfType(method, typeof(TIn1), typeof(TIn2))
                select method;

            var binaryOperatorMethod = methods.SingleOrDefault();
            if (binaryOperatorMethod != null)
            {
                binaryOperator = (left, right) => (TOut) binaryOperatorMethod.Invoke(null, new object[] { left, right });
                return true;
            }
            binaryOperator = null;
            return false;
        }

        private static bool IsClosedType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return !type.IsGenericType || !type.ContainsGenericParameters;
        }

        private static IEnumerable<MethodInfo> GetOperatorsOfType(Type type, string name, Type returnType)
        {
            if (IsClosedType(type))
            {
                return
                    from method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    where IsOperatorOfType(method, name, returnType)                
                    select method;
            }
            return Enumerable.Empty<MethodInfo>();
        }

        private static bool IsOperatorOfType(MethodInfo method, string name, Type returnType) =>
             method.IsSpecialName && method.Name == name && method.ReturnType == returnType;

        private static bool HasParameterOfType(MethodInfo method, Type parameterType)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 1)
            {
                return parameters[0].ParameterType == parameterType;
            }
            return false;
        }

        private static bool HasParametersOfType(MethodInfo method, Type parameterType0, Type parameterType1)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 2)
            {
                return parameters[0].ParameterType == parameterType0 && parameters[1].ParameterType == parameterType1;
            }
            return false;
        }

        #endregion

        #region [====== FriendlyName ======]

        /// <summary>
        /// Concatenates the names of the specified <paramref name="types"/> into a single string using the specified <paramref name="separator"/>.
        /// </summary>
        /// <param name="types">A collection of types.</param>
        /// <param name="useFullNames">Indicates whether or not the full name of all types should be used in the friendly name.</param>
        /// <param name="separator">The separator that is used to concatenate all names.</param>
        /// <returns>A single string containing all names of the specified <paramref name="types"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>
        /// </exception>
        public static string FriendlyNames(this IEnumerable<Type> types, bool useFullNames = false, string separator = null) =>
            string.Join(separator ?? ", ", types.Select(type => type.FriendlyName(useFullNames)));

        /// <summary>
        /// Returns the friendly name of a type, useful for displaying type information in consoles or debug strings.
        /// </summary>
        /// <param name="type">A type.</param>
        /// <param name="useFullNames">Indicates whether or not the full name of all types should be used in the friendly name.</param>
        /// <returns>A friendly name of a type</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static string FriendlyName(this Type type, bool useFullNames = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return new StringBuilder().AppendFriendlyNameOf(type, useFullNames).ToString();
        }

        private static StringBuilder AppendFriendlyNameOf(this StringBuilder builder, Type type, bool useFullNames)
        {
            var typeName = useFullNames ? type.FullName : type.Name;

            if (type.IsGenericType)
            {
                return builder
                    .Append(typeName.RemoveTypeParameterCount())
                    .Append('<')
                    .AppendTypeParameters(type.GetGenericArguments(), useFullNames && !type.ContainsGenericParameters)
                    .Append('>');                
            }
            return builder.Append(typeName);
        }

        internal static string RemoveTypeParameterCount(this string typeName) =>
            typeName.Substring(0, typeName.IndexOf("`"));

        private static StringBuilder AppendTypeParameters(this StringBuilder builder, Type[] typeParameters, bool useFullNames) => builder
            .AppendFriendlyNameOf(typeParameters[0], useFullNames)
            .AppendTypeParameterTail(typeParameters, useFullNames, 1);

        private static StringBuilder AppendTypeParameterTail(this StringBuilder builder, Type[] typeParameters, bool useFullNames, int index)
        {
            if (index == typeParameters.Length)
            {
                return builder;
            }
            return builder
                .Append(", ")
                .AppendFriendlyNameOf(typeParameters[index], useFullNames)
                .AppendTypeParameterTail(typeParameters, useFullNames, index + 1);
        }

        #endregion
    }
}
