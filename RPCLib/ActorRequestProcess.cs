using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class ActorRequestProcess
    {
        Service service;
        Socket handler;
        public ActorRequestProcess(Service service, Socket handler)
        {
            this.service = service;
            this.handler = handler;
        }
        public void ProcessRequest(Request request)
        {
            string method = request.nameOfMethod;
            if(method == "actorRepository.Insert")
            {
                ProcessInsert(request);
            }
            else if(method == "actorRepository.DeleteById")
            {
                ProcessDeleteById(request);
            }
            else if(method == "actorRepository.Update")
            {
                ProcessUpdate(request);
            }
            else if(method == "actorRepository.GetById")
            {
                ProcessGetById(request);
            }
            else if(method == "actorRepository.GetAll")
            {
                ProcessGetAll(request);
            }
            else if(method == "actorRepository.GetSearchPagesCount")
            {
                ProcessGetSearchPagesCount(request);
            }
            else if(method == "actorRepository.GetSearchPage")
            {
                ProcessGetSearchPage(request);
            }
        }
        private void ProcessInsert(Request request)
        {
            Actor actor = Actor.ActorParser(request.methodParams[0]);
            long returnValue = service.actorRepository.Insert(actor);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessDeleteById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            int returnValue = service.actorRepository.DeleteById(id);
            Response<int> response = new Response<int>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessUpdate(Request request)
        {
            long id = long.Parse(request.methodParams[0]);
            Actor actor = Actor.ActorParser(request.methodParams[1]);
            long returnValue = service.actorRepository.Update(id, actor);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            Actor returnValue = service.actorRepository.GetById(id);
            Response<Actor> response = new Response<Actor>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetAll(Request request)
        {
            List<Actor> returnValue = service.actorRepository.GetAll();
            Response<List<Actor>> response = new Response<List<Actor>>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetSearchPagesCount(Request request)
        {
            string toSearch = request.methodParams[0];
            int returnValue = service.actorRepository.GetSearchPagesCount(toSearch);
            Response<int> response = new Response<int>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetSearchPage(Request request)
        {
            string toSearch = request.methodParams[0];
            int page = int.Parse(request.methodParams[1]);
            List<Actor> returnValue = service.actorRepository.GetSearchPage(toSearch, page);
            Response<List<Actor>> response = new Response<List<Actor>>(){value = returnValue};
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