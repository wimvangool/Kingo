using System;
using System.Text;

namespace Kingo.Messaging.Validation
{
    internal abstract class MemberNameComponentStack
    {
        protected string InstanceName =>
             ToInstanceName(InstanceType);

        internal abstract Type InstanceType
        {
            get;
        }

        internal abstract string Top
        {
            get;
        }

        internal virtual MemberNameComponentStack Push(Identifier identifier) =>
             new IdentifierComponent(InstanceType, identifier, this);

        internal virtual MemberNameComponentStack Push(IndexList indexList) =>
             new IndexListComponent(InstanceType, indexList, this);

        internal abstract bool Pop(out MemberNameComponentStack memberName);        

        public override string ToString() =>
             ToString(false);

        public string ToString(bool displayName)
        {
            var fullName = new StringBuilder();

            WriteTo(fullName, displayName);

            return fullName.ToString();
        }

        internal abstract void WriteTo(StringBuilder fullName, bool displayName); 
       
        private static string ToInstanceName(Type instanceType)
        {
            var instanceName = new StringBuilder();

            WriteInstanceName(instanceType, instanceName);

            return instanceName.ToString();
        }

        private static void WriteInstanceName(Type instanceType, StringBuilder instanceName)
        {
            if (instanceType.IsGenericType)
            {                
                var instanceNameWithGenericTypeInfo = instanceType.Name;
                var quoteIndex = instanceNameWithGenericTypeInfo.IndexOf('`');                

                instanceName.Append(instanceNameWithGenericTypeInfo.Substring(0, quoteIndex));
                instanceName.Append('<');

                WriteGenericArgumentsOf(instanceType, instanceName);

                instanceName.Append('>');
            }
            else
            {
                instanceName.Append(instanceType.Name);
            }
        }

        private static void WriteGenericArgumentsOf(Type instanceType, StringBuilder instanceName)
        {
            var arguments = instanceType.GetGenericArguments();

            WriteInstanceName(arguments[0], instanceName);

            for (int index = 1; index < arguments.Length; index++)
            {
                instanceName.Append(", ");

                WriteInstanceName(arguments[index], instanceName);
            }
        }
    }
}
