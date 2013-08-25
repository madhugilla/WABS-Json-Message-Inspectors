using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Services;
using Microsoft.BizTalk.Services.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonMsgInspectors.Test
{
    [TestClass()]
    public class XmlToJsonConverterTests
    {
        [TestMethod()]
        public void XmlToJsonExecuteTest()
        {
            var xmlToJsonConverter = new XmlToJsonConverter
                {
                    TypeName = "Root"
                };

            var testMessage = new TestMessage
                {
                    ContentType = new ContentType("application/xml"),
                    Data = GetXmlStream()
                };


            IMessageInspectorContext context = new StubIMessageInspectorContext
            {
                TracerGet = () => new StubITracer(),
            };

            Task task = xmlToJsonConverter.Execute(testMessage, context);
            task.Wait();
            Assert.IsTrue(task.IsCompleted);


            var rdr = new StreamReader(testMessage.Data);
            string outMsg = rdr.ReadToEnd();
            JObject json = JObject.Parse(outMsg);
            Assert.IsNotNull(json);
            JToken po = json.SelectToken("person");

            Assert.IsTrue(String.CompareOrdinal(po.SelectToken("name").Value<string>(), "Demo2") == 0);
        }

        private static Stream GetXmlStream()
        {
            byte[] output = Encoding.ASCII.GetBytes("<ns0:Root xmlns:ns0='http://JsonBridges.PO'><person><name>Demo2</name><url>someurl.com</url></person></ns0:Root>");
            var memoryStream = new MemoryStream();
            memoryStream.Write(output, 0, output.Length);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
