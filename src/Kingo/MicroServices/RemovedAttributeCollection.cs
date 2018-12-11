using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class RemovedAttributeCollection : IRemovedAttributeCollection
    {
        #region [====== State ======]

        private abstract class State : IRemovedAttributeCollection
        {
            public abstract bool Contains(Type attributeType);

            public abstract State Remove<TAttribute>() where TAttribute : Attribute, IMicroProcessorFilter;

            public abstract State RemoveAllAttributes();
        }

        #endregion

        #region [====== NoTypesState ======]

        private sealed class NoTypesState : State
        {
            public override bool Contains(Type attributeType) =>
                false;

            public override State Remove<TAttribute>() =>
                new SomeTypesState(typeof(TAttribute));

            public override State RemoveAllAttributes() =>
                new AllTypesState();
        }

        #endregion

        #region [====== SomeTypesState ======]

        private sealed class SomeTypesState : State
        {
            private readonly List<Type> _attributeTypes;

            public SomeTypesState(Type attributeType)
            {
                _attributeTypes = new List<Type>()
                {
                    attributeType
                };
            }

            public override bool Contains(Type attributeType) =>
                _attributeTypes.Contains(attributeType);

            public override State Remove<TAttribute>() =>
                Remove(typeof(TAttribute));

            private State Remove(Type attributeType)
            {
                if (!_attributeTypes.Contains(attributeType))
                {
                    _attributeTypes.Add(attributeType);
                }
                return this;
            }

            public override State RemoveAllAttributes() =>
                new AllTypesState();
        }

        #endregion

        #region [====== AllTypesState ======]

        private sealed class AllTypesState : State
        {
            public override bool Contains(Type attributeType) =>
                true;

            public override State Remove<TAttribute>() =>
                this;

            public override State RemoveAllAttributes() =>
                this;
        }

        #endregion

        private State _state;

        public RemovedAttributeCollection()
        {
            _state = new NoTypesState();
        }

        public bool Contains(Type attributeType) =>
            _state.Contains(attributeType);

        public void Remove<TAttribute>() where TAttribute : Attribute, IMicroProcessorFilter =>
            _state = _state.Remove<TAttribute>();

        public void RemoveAllAttributes() =>
            _state = _state.RemoveAllAttributes();
    }
}
