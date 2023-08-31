using System.Collections.Generic;
using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public class Delete : Request
    {
        [XmlElement("objref")]
        public List<long> Items { get; set; }
    }
}
