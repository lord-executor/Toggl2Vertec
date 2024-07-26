using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api;

public class ObjRef
{
    [XmlElement(ElementName = "objref")]
    public long Target { get; set; }

    public static implicit operator ObjRef(long target)
    {
        return new ObjRef { Target = target };
    }
}