using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ShippingProvision.Services
{
    [Serializable()]
    public abstract class CompositeEntity
    {
        public override bool Equals(object obj)
        {
            return CompositeEquals(obj);
        }

        public override int GetHashCode()
        {
            return CompositeGetHashCode();
        }

        //REMEMBER: The ToString is added as ENTITY_ID in the audit log, Do not change the format

        public override string ToString()
        {
            return CompositeToString();
        }

        public abstract bool CompositeEquals(object obj);
        public abstract int CompositeGetHashCode();
        public abstract string CompositeToString();
        public abstract string GetName();
    }
}
