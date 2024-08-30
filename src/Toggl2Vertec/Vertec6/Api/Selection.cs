using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api;

public class Selection
{
    [XmlElement("ocl")]
    public string Ocl { get; set; }

    [XmlElement("sqlwhere")]
    public string SqlWhere { get; set; }
    
    [XmlElement("objref")]
    public long Objref { get; set; }
    
}
