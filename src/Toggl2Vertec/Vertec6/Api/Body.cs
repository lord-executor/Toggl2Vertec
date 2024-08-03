using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api;

public class Body
{
    [XmlElement(Type = typeof(Query))]
    [XmlElement(Type = typeof(Create))]
    [XmlElement(Type = typeof(Update))]
    [XmlElement(Type = typeof(Delete))]
    public Request Request { get; set; }

    [XmlArrayItem(Type = typeof(ProjektPhase))]
    [XmlArrayItem(Type = typeof(OffeneLeistung))]
    [XmlArrayItem(Type = typeof(PraesenzZeit))]
    public List<Entity> QueryResponse { get; set; }
}