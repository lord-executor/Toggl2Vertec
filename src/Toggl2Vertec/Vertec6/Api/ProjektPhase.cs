using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public class ProjektPhase : Entity
    {
        [XmlElement(ElementName = "code")]
        public string Code { get; set; }
    }
}
