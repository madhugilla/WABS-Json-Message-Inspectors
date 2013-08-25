using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.BizTalk.Services;
using Newtonsoft.Json;

namespace JsonMsgInspectors
{
    public class JsonToXmlConverter : IMessageInspector
    {
        [PipelineProperty(Name = "RootNode")]
        public string RootNode { get; set; }

        [PipelineProperty(Name = "Namespace")]
        public string Namespace { get; set; }

        [PipelineProperty(Name = "Prefix")]
        public string Prefix { get; set; }

        private const string ApplicationXml = "application/xml";
        private const string ApplicationJson = "application/json";

        public Task Execute(IMessage message, IMessageInspectorContext context)
        {
            context.Tracer.TraceEvent(TraceEventType.Information,
                                      String.Format("JsonToXmlConverter - MessageType: {0}", message.ContentType));
          
            return Task.Factory.StartNew(() =>
                {
                    message.Data = GetXmlStream(message.Data, message.ContentType);
                    message.Data.Position = 0;
                    message.ContentType = new ContentType(ApplicationXml);
                    context.Tracer.TraceEvent(TraceEventType.Information, "JsonToXmlConverter - json to xml conversion done.");
                });
        }


        private Stream GetXmlStream(Stream msgStream, ContentType contentType)
        {
            Stream originalStream = msgStream;
            string json = null;

            if (contentType.ToString().Contains(ApplicationJson))
            {
                using (var reader = new StreamReader(originalStream))
                {
                    json = reader.ReadToEnd();
                }
            }
                // When Bridges are chained, WABS converts the message into a base64 string and sends it to the next bridge and sets the message content type to application/xml
                // the following c
            else if (contentType.ToString().Contains(ApplicationXml))
            {
                //<Binary>ew0KICAicGVyc29uIjogew0KICAgICJuYW1lIjogIm5lbiIsDQogICAgInVybCI6ICJuZW5AMTIzNCINCiAgfQ0KfQ==</Binary>
                var parser = new XmlDocument();
                parser.Load(msgStream);

                string jsonInnerXml = parser.FirstChild.InnerXml;
                byte[] data = Convert.FromBase64String(jsonInnerXml);
                json = Encoding.ASCII.GetString(data);

            }

            //// http://www.modhul.com/2013/04/30/restfully-getting-json-formatted-data-with-biztalk-2013/
             
            var xmlDoc = new XmlDocument();
            XmlNode jsonNode = JsonConvert.DeserializeXmlNode(json, "RootNode");
            XmlNode rootNode = xmlDoc.CreateNode(XmlNodeType.Element, Prefix, RootNode, Namespace);
            var selectSingleNode = jsonNode.SelectSingleNode("RootNode");
            if (selectSingleNode != null)
                rootNode.InnerXml = selectSingleNode.InnerXml;
            xmlDoc.AppendChild(rootNode);
            string innerXml = xmlDoc.InnerXml;

            byte[] output = Encoding.ASCII.GetBytes(innerXml);
            var memoryStream = new MemoryStream();
            memoryStream.Write(output, 0, output.Length);
            memoryStream.Position = 0;

            return memoryStream;
        }

    }
}