﻿using System;
using System.Text;

namespace Kingo.BuildingBlocks
{
    internal abstract class StringTemplateComponent
    {
        internal abstract StringTemplateComponent NextComponent
        {
            get;
        }

        internal abstract StringTemplateComponent AttachLast(StringTemplateComponent nextComponent);

        internal abstract int CountLiterals();

        internal abstract StringTemplateComponent Format(string identifier, object argument, IFormatProvider formatProvider);

        internal string ToString(StringBuilder value)
        {
            value.Append(ToString());

            if (NextComponent == null)
            {
                return value.ToString();
            }
            return NextComponent.ToString(value);
        }
    }
}
