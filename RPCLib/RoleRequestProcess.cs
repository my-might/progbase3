using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class RoleRequestProcess
    {
        Service service;
        Socket handler;
        public RoleRequestProcess(Service service, Socket handler)
        {
            this.service = service;
            this.handler = handler;
        }
        public void ProcessRequest(Request request)
        {
            string method = request.nameOfMethod;
            if(method == "roleRepository.Insert")
            {
                ProcessInsert(request);
            }
            else if(method == "roleRepository.Delete")
            {
                ProcessDelete(request);
            }
            else if(method == "roleRepository.DeleteByActorId")
            {
                ProcessDeleteByActorId(request);
            }
            else if(method == "roleRepository.DeleteByFilmId")
            {
                ProcessDeleteByFilmId(request);
            }
            else if(method == "roleRepository.GetById")
            {
                ProcessGetById(request);
            }
            else if(method == "roleRepository.IsExist")
            {
                ProcessIsExist(request);
            }
            else if(method == "roleRepository.GetAllFilms")
            {
                ProcessGetAllFilms(request);
            }
            else if(method == "roleRepository.GetAllActors")
            {
                ProcessGetAllActors(request);
            }
            else if(method == "roleRepository.GetForImage")
            {
                ProcessGetForImage(request);
            }
        }
        private void ProcessInsert(Request request)
        {
            Role role = Role.RoleParser(request.methodParams[0]);
            long returnValue = service.roleRepository.Insert(role);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessDelete(Request request)
        {
            int actorId = int.Parse(request.methodParams[0]);
            int filmId = int.Parse(request.methodParams[1]);
            long returnValue = service.roleRepository.Delete(actorId, filmId);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessDeleteByActorId(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            long returnValue = service.roleRepository.DeleteByActorId(id);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessDeleteByFilmId(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            long returnValue = service.roleRepository.DeleteByFilmId(id);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            Role returnValue = service.roleRepository.GetById(id);
            Response<Role> response = new Response<Role>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessIsExist(Request request)
        {
            long filmId = long.Parse(request.methodParams[0]);
            long actorId = long.Parse(request.methodParams[1]);
            bool returnValue = service.roleRepository.IsExist(filmId, actorId);
            Response<bool> response = new Response<bool>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetAllFilms(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            List<Film> returnValue = service.roleRepository.GetAllFilms(id);
            Response<List<Film>> response = new Response<List<Film>>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetAllActors(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            List<Actor> returnValue = service.roleRepository.GetAllActors(id);
            Response<List<Actor>> response = new Response<List<Actor>>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetForImage(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            List<Film> returnValue = service.roleRepository.GetForImage(id);
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