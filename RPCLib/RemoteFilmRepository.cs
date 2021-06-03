using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class RemoteFilmRepository
    {
        Socket sender;
        public RemoteFilmRepository(Socket sender)
        {
            this.sender = sender;
        }
        public long Insert(Film film)
        {
            string[] parameters = new string[] {film.FilmConnection()};
            Request request = new Request()
            {
                nameOfMethod = "filmRepository.Insert",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public long InsertImported(Film film)
        {
            string[] parameters = new string[] {film.FilmConnection()};
            Request request = new Request()
            {
                nameOfMethod = "filmRepository.InsertImported",
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
                nameOfMethod = "filmRepository.DeleteById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public long Update(long id, Film film)
        {
            string[] parameters = new string[] {id.ToString(), film.FilmConnection()};
            Request request = new Request()
            {
                nameOfMethod = "filmRepository.Update",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public Film GetById(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "filmRepository.GetById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<Film> response = GetResponse<Film>();
            return response.value;
        }
        public List<Film> GetAll()
        {
            string[] parameters = new string[] {""};
            Request request = new Request()
            {
                nameOfMethod = "filmRepository.GetAll",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Film>> response = GetResponse<List<Film>>();
            return response.value;
        }
        public int GetSearchPagesCount(string searchTitle)
        {
            string[] parameters = new string[] {searchTitle};
            Request request = new Request()
            {
                nameOfMethod = "filmRepository.GetSearchPagesCount",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public List<Film> GetSearchPage(string searchTitle, int page)
        {
            string[] parameters = new string[] {searchTitle, page.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "filmRepository.GetSearchPage",
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