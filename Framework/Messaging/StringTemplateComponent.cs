using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceComponents
{
    internal abstract class StringTemplateComponent
    {
        internal abstract StringTemplateComponent NextComponent
        {
            get;
        }

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
