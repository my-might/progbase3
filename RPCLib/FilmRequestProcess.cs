using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class FilmRequestProcess
    {
        Service service;
        Socket handler;
        public FilmRequestProcess(Service service, Socket handler)
        {
            this.service = service;
            this.handler = handler;
        }
        public void ProcessRequest(Request request)
        {
            string method = request.nameOfMethod;
            if(method == "filmRepository.Insert")
            {
                ProcessInsert(request);
            }
            else if(method == "filmRepository.InsertImported")
            {
                ProcessInsertImported(request);
            }
            else if(method == "filmRepository.DeleteById")
            {
                ProcessDeleteById(request);
            }
            else if(method == "filmRepository.Update")
            {
                ProcessUpdate(request);
            }
            else if(method == "filmRepository.GetById")
            {
                ProcessGetById(request);
            }
            else if(method == "filmRepository.GetAll")
            {
                ProcessGetAll(request);
            }
            else if(method == "filmRepository.GetSearchPagesCount")
            {
                ProcessGetSearchPagesCount(request);
            }
            else if(method == "filmRepository.GetSearchPage")
            {
                ProcessGetSearchPage(request);
            }
        }
        private void ProcessInsert(Request request)
        {
            Film film = Film.FilmParser(request.methodParams[0]);
            long returnValue = service.filmRepository.Insert(film);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessInsertImported(Request request)
        {
            Film film = Film.FilmParser(request.methodParams[0]);
            long returnValue = service.filmRepository.InsertImported(film);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessDeleteById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            int returnValue = service.filmRepository.DeleteById(id);
            Response<int> response = new Response<int>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessUpdate(Request request)
        {
            long id = long.Parse(request.methodParams[0]);
            Film film = Film.FilmParser(request.methodParams[1]);
            long returnValue = service.filmRepository.Update(id, film);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            Film returnValue = service.filmRepository.GetById(id);
            Response<Film> response = new Response<Film>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetAll(Request request)
        {
            List<Film> returnValue = service.filmRepository.GetAll();
            Response<List<Film>> response = new Response<List<Film>>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetSearchPagesCount(Request request)
        {
            string toSearch = request.methodParams[0];
            int returnValue = service.filmRepository.GetSearchPagesCount(toSearch);
            Response<int> response = new Response<int>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetSearchPage(Request request)
        {
            string toSearch = request.methodParams[0];
            int page = int.Parse(request.methodParams[1]);
            List<Film> returnValue = service.filmRepository.GetSearchPage(toSearch, page);
            Response<List<Film>> response = new Response<List<Film>>(){value = returnValue};
            SendResponse(response);
        }
        private void SendResponse<T>(Response<T> response)
        {
            string xmlResponse = ServerSerializers.SerializeResponse(response);
            byte[] message = Encoding.UTF8.GetBytes(xmlResponse);
            handler.Send(message);
            Console.WriteLine($"Response was sent to {handler.RemoteEndPoint}");
        }
    }
}