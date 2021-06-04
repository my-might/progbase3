using System.Xml.Serialization;
using ClassLib;

namespace RPCLib
{
    [XmlRoot("response")]
    public class Response<T>
    {
       public T value;
       public bool errors;
    }
}