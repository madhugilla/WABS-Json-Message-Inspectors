Windows Azure BizTalk Services,  JSON to XML and XML to JSON Message inspectors
================================================================================================================

Json to Xml conversion:  This class implements the IMessageInspector class and exposes three properties root node, namespace and the prefix to use for new xml message, the Execute method of the interface uses Newtonsoft Json library to convert the incoming json to xml message.

Xml to Json conversion:  Similar to the previous class we implement the IMessageInspector and expose a property called type name, this is the class name that we will use to convert the xml to json using the method JsonConvert.SerializeObject of the Newtonsoft Json library.

For more details visit my blog :http://blog.appliedis.com/2013/08/28/extending-windows-azure-biztalk-services-using-message-inspectors/

