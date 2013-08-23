using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.BizTalk.Services;
using Newtonsoft.Json;

namespace JsonMsgInspectors
{
    public class XmlToJsonConverter : IMessageInspector
    {
        [PipelineProperty(Name = "TypeName")]
        public string TypeName { get; set; }

        public Task Execute(IMessage message, IMessageInspectorContext context)
        {
            context.Tracer.TraceEvent(TraceEventType.Information,
                                     String.Format("XmlToJsonConverter - TypeName: {0}", TypeName));
            return Task.Factory.StartNew(() =>
            {
                message.Data = GetJsonStream(message.Data);
                message.Data.Position = 0;
                message.ContentType = new ContentType("application/json");
                context.Tracer.TraceEvent(TraceEventType.Information,
                                        String.Format("XmlToJsonConverter -  xml to json conversion completed."));
            });
        }

        public Stream GetJsonStream(Stream msgStream)
        {
            Stream originalStream = msgStream;
            Type myClassType = Type.GetType(TypeName);
            object reqObj = FromXml(originalStream, myClassType);
            string jsonText = JsonConvert.SerializeObject(reqObj, myClassType, Formatting.None,
                                                          new JsonSerializerSettings());
            byte[] outBytes = Encoding.ASCII.GetBytes(jsonText);

            var memStream = new MemoryStream();
            memStream.Write(outBytes, 0, outBytes.Length);
            memStream.Position = 0;

            return memStream;
        }

        //Creates an object from an XML string.
        public static object FromXml(Stream xml, Type objType)
        {
            var ser = new XmlSerializer(objType);
            return ser.Deserialize(xml);
        }
    }
}