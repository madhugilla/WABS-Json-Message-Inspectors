using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using Microsoft.BizTalk.Services;

namespace JsonMsgInspectors.Test
{
    public class TestMessage : IMessage
    {
        public void Promote(string propertyName, object propertyValue)
        {
            throw new NotImplementedException();
        }

        public object GetPromotedProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, object>> GetPromotedProperties()
        {
            throw new NotImplementedException();
        }

        public Stream Data { get; set; }
        public ContentType ContentType { get; set; }
    }
}