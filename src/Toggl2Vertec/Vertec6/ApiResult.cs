using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6
{
    public class ApiResult
    {
        private readonly string _content;

        public ApiResult(string content)
        {
            _content = content;
        }

        public IEnumerable<T> GetResults<T>()
        {
            var serializer = new XmlSerializer(typeof(Envelope));
            var envelope = (Envelope)serializer.Deserialize(new StringReader(_content));

            return envelope.Body.QueryResponse.OfType<T>();
        }

        public XmlDocument GetDocument()
        {
            var doc = new XmlDocument();
            doc.LoadXml(_content);

            return doc;
        }
    }
}
