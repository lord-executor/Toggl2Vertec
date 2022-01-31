using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Toggl2Vertec.Vertec6.Api
{
    public class Selection
    {
        [XmlElement("ocl")]
        public string Ocl { get; set; }

        [XmlElement("sqlwhere")]
        public string SqlWhere { get; set; }
    }
}
