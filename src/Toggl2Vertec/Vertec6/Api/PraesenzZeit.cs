using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public class PraesenzZeit : Entity
    {
        [XmlElement(ElementName = "bearbeiter")]
        public ObjRef Bearbeiter { get; set; }

        [XmlElement(ElementName = "datum")]
        public string Datum { get; set; }

        [XmlElement(ElementName = "von")]
        public string Von { get; set; }

        [XmlElement(ElementName = "bis")]
        public string Bis { get; set; }
    }
}
