using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class RemoteUserRepository
    {
        Socket sender;
        public RemoteUserRepository(Socket sender)
        {
            this.sender = sender;
        }
        public long Insert(User user)
        {
            string[] parameters = new string[] {user.UserConnection()};
            Request request = new Request()
            {
                nameOfMethod = "userRepository.Insert",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public int DeleteById(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "userRepository.DeleteById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public long Update(long id, User user)
        {
            string[] parameters = new string[] {id.ToString(), user.UserConnection()};
            Request request = new Request()
            {
                nameOfMethod = "userRepository.Update",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public User GetById(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "userRepository.GetById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<User> response = GetResponse<User>();
            return response.value;
        }
        public User GetByUsername(string username)
        {
            string[] parameters = new string[] {username};
            Request request = new Request()
            {
                nameOfMethod = "userRepository.GetByUsername",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<User> response = GetResponse<User>();
            return response.value;
        }
        private void SendRequest(Request request)
        {
            string xmlRequest = ServerSerializers.SerializeRequest(request);
            byte[] msg = Encoding.UTF8.GetBytes(xmlRequest);
            sender.Send(msg);
        }
        private Response<T> GetResponse<T>()
        {
            byte[] bytes = new byte[1024];
            string xmlResponse = "";
            while (true)
            {
                int bytesRec = sender.Receive(bytes);
                xmlResponse += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                if (xmlResponse.IndexOf("</response>") > -1)
                {
                    break;
                }
            }
            Response<T> response = ServerSerializers.DeserializeResponse<T>(xmlResponse);
            return response;
        }
    }
}