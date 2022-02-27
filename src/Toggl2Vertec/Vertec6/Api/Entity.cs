using System;
using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public abstract class Entity
    {
        public bool ObjIdSpecified => ObjId.HasValue;

        [XmlElement(ElementName = "objid")]
        public long? ObjId { get; set; }

        public bool ObjRefSpecified => ObjRef.HasValue;

        [XmlElement(ElementName = "objref")]
        public long? ObjRef { get; set; }

        public void IdToRef()
        {
            if (!ObjId.HasValue)
            {
                throw new InvalidOperationException("Cannot convert to Ref because it has no Id");
            }

            ObjRef = ObjId.Value;
            ObjId = null;
        }
    }
}
