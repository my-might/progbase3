using System.Xml.Serialization;
using ClassLib;

namespace RPCLib
{
    [XmlRoot("request")]
    public class Request
    {
        public string nameOfMethod;       
        public string[] methodParams;
    }
}