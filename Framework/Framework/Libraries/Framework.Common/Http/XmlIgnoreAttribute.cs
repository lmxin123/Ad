using System;

namespace Framework.Common.Http
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class XmlIgnore : Attribute
    {
        public XmlIgnore() { }
    }
}
