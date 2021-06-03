using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class RemoteRoleRepository
    {
        Socket sender;
        public RemoteRoleRepository(Socket sender)
        {
            this.sender = sender;
        }
        public long Insert(Role role)
        {
            string[] parameters = new string[] {role.RoleConnection()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.Insert",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public long Delete(int actorId, int filmId)
        {
            string[] parameters = new string[] {actorId.ToString(), filmId.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.Delete",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public long DeleteByActorId(int actorId)
        {
            string[] parameters = new string[] {actorId.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.DeleteByActorId",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public long DeleteByFilmId(int filmId)
        {
            string[] parameters = new string[] {filmId.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.DeleteByFilmId",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public Role GetById(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.GetById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<Role> response = GetResponse<Role>();
            return response.value;
        }
        public bool IsExist(long filmId, long actorId)
        {
            string[] parameters = new string[] {filmId.ToString(), actorId.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.IsExist",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<bool> response = GetResponse<bool>();
            return response.value;
        }
        public List<Film> GetAllFilms(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.GetAllFilms",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Film>> response = GetResponse<List<Film>>();
            return response.value;
        }
        public List<Actor> GetAllActors(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.GetAllActors",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Actor>> response = GetResponse<List<Actor>>();
            return response.value;
        }
        public List<Film> GetForImage(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "roleRepository.GetForImage",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Film>> response = GetResponse<List<Film>>();
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