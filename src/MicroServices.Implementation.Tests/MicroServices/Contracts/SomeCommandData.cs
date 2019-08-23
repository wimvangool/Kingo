using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.Contracts
{
    [Serializable]
    public sealed class SomeCommandData : ValidatableObject
    {        
        [Required]
        public object PropertyA
        {
            get;
            set;
        }

        [ChildMember(ErrorMessage = "{0} contains invalid items.")]
        public List<SomeCommandData> PropertyB
        {
            get;
            set;
        }       
        
        [ChildMember]
        public SomeCommandCollection PropertyC
        {
            get;
            set;
        }

        [ChildMember(ErrorMessage = "{0} contains invalid items.")]
        public Dictionary<string, SomeCommandData> PropertyD
        {
            get;
            set;
        }

        [ChildMember]
        public SomeCommandDictionary PropertyE
        {
            get;
            set;
        }
    }
}
