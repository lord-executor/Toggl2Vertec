using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public class OffeneLeistung : Entity
    {
        [XmlElement(ElementName = "bearbeiter")]
        public ObjRef Bearbeiter { get; set; }

        [XmlElement(ElementName = "projekt")]
        public ObjRef Projekt { get; set; }

        [XmlElement(ElementName = "phase")]
        public ObjRef Phase { get; set; }

        [XmlElement(ElementName = "minutenInt")]
        public int MinutenInt { get; set; }

        [XmlElement(ElementName = "datum")]
        public string Datum { get; set; }

        [XmlElement(ElementName = "text")]
        public string Text { get; set; }
    }
}