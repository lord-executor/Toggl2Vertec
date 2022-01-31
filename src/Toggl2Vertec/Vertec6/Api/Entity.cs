using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public class Entity
    {
        [XmlElement(ElementName = "objid")]
        public long ObjId { get; set; }
    }
}
