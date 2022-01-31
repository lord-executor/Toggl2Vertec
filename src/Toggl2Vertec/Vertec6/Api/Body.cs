using System.Collections.Generic;
using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public class Body
    {
        public Query Query { get; set; }

        [XmlArrayItem(Type = typeof(ProjektPhase))]
        [XmlArrayItem(Type = typeof(OffeneLeistung))]
        public List<Entity> QueryResponse { get; set; }
    }
}
