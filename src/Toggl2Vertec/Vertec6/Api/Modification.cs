using System.Collections.Generic;
using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public abstract class Modification : Request
    {
        [XmlElement(Type = typeof(ProjektPhase))]
        [XmlElement(Type = typeof(OffeneLeistung))]
        [XmlElement(Type = typeof(PraesenzZeit))]
        public List<Entity> Entities { get; set; } = new List<Entity>();
    }
}
