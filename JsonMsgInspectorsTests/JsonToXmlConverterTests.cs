using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.BizTalk.Services;
using Microsoft.BizTalk.Services.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonMsgInspectors.Test
{
    [TestClass]
    public class JsonToXmlConverterTests
    {
        [TestMethod]
        public void JsonToXmlExecuteTest()
        {
            var jsonToXmlConverter = new JsonToXmlConverter
                {
                    Namespace = "http://test",
                    Prefix = "ns0",
                    RootNode = "somenode"
                };

            IMessageInspectorContext context = new StubIMessageInspectorContext
                {
                    TracerGet = () => new StubITracer(),
                };


            var testMessage = new TestMessage
                {
                    ContentType = new ContentType("application/json"),
                    Data = GetJsonStream()
                };

            Task task = jsonToXmlConverter.Execute(testMessage, context);
            task.Wait();
            Assert.IsTrue(task.IsCompleted);

            var doc = new XmlDocument();
            doc.Load(testMessage.Data);

            Assert.IsTrue(doc.DocumentElement != null && doc.DocumentElement.Name == "ns0:somenode");
        }

        private static Stream GetJsonStream()
        {
            byte[] output = Encoding.ASCII.GetBytes("{'person': {'name': 'Demo1','url': 'somesite.com'}}");
            var memoryStream = new MemoryStream();
            memoryStream.Write(output, 0, output.Length);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}