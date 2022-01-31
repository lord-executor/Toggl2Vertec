using System.Collections.Generic;
using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public class Query
    {
        public Selection Selection { get; set; }

        [XmlArray(ElementName = "Resultdef")]
        [XmlArrayItem(ElementName = "member")]
        public List<string> Members { get; set; }
    }
}
