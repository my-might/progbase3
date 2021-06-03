using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class RemoteActorRepository
    {
        Socket sender;
        public RemoteActorRepository(Socket sender)
        {
            this.sender = sender;
        }
        public long Insert(Actor actor)
        {
            string[] parameters = new string[] {actor.ActorConnection()};
            Request request = new Request()
            {
                nameOfMethod = "actorRepository.Insert",
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
                nameOfMethod = "actorRepository.DeleteById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public long Update(long id, Actor actor)
        {
            string[] parameters = new string[] {id.ToString(), actor.ActorConnection()};
            Request request = new Request()
            {
                nameOfMethod = "actorRepository.Update",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public Actor GetById(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "actorRepository.GetById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<Actor> response = GetResponse<Actor>();
            return response.value;
        }
        public List<Actor> GetAll()
        {
            string[] parameters = new string[] {""};
            Request request = new Request()
            {
                nameOfMethod = "actorRepository.GetAll",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Actor>> response = GetResponse<List<Actor>>();
            return response.value;
        }
        public int GetSearchPagesCount(string searchFullname)
        {
            string[] parameters = new string[] {searchFullname};
            Request request = new Request()
            {
                nameOfMethod = "actorRepository.GetSearchPagesCount",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public List<Actor> GetSearchPage(string searchFullname, int page)
        {
            string[] parameters = new string[] {searchFullname, page.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "actorRepository.GetSearchPage",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Actor>> response = GetResponse<List<Actor>>();
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