using System.IO;
using System.Xml.Serialization;
using ClassLib;
namespace RPCLib
{
    public static class ServerSerializers
    {
        public static string SerializeRequest(Request request)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Request));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, request);
            writer.Close();
            return writer.ToString();
        }
        public static Request DeserializeRequest(string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Request));
            StringReader reader = new StringReader(xml);
            Request value = (Request)ser.Deserialize(reader);
            reader.Close();
            return value;
        }
        public static string SerializeResponse<T>(Response<T> response)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Response<T>));
            StringWriter writer = new StringWriter();
            ser.Serialize(writer, response);
            writer.Close();
            return writer.ToString();
        }
        public static Response<T> DeserializeResponse<T>(string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Response<T>));
            StringReader reader = new StringReader(xml);
            Response<T> value = (Response<T>)ser.Deserialize(reader);
            reader.Close();
            return value;
        }
    }
}