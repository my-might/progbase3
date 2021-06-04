using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class UserRequestProcess
    {
        Service service;
        Socket handler;
        public UserRequestProcess(Service service, Socket handler)
        {
            this.service = service;
            this.handler = handler;
        }
        public void ProcessRequest(Request request)
        {
            string method = request.nameOfMethod;
            if(method == "userRepository.Insert")
            {
                ProcessInsert(request);
            }
            else if(method == "userRepository.DeleteById")
            {
                ProcessDeleteById(request);
            }
            else if(method == "userRepository.Update")
            {
                ProcessUpdate(request);
            }
            else if(method == "userRepository.GetById")
            {
                ProcessGetById(request);
            }
            else if(method == "userRepository.GetByUsername")
            {
                ProcessGetByUsername(request);
            }
        }
        private void ProcessInsert(Request request)
        {
            User user = User.UserParser(request.methodParams[0]);
            long returnValue = service.userRepository.Insert(user);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessDeleteById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            int returnValue = service.userRepository.DeleteById(id);
            Response<int> response = new Response<int>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessUpdate(Request request)
        {
            long id = long.Parse(request.methodParams[0]);
            User user = User.UserParser(request.methodParams[1]);
            long returnValue = service.userRepository.Update(id, user);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            User returnValue = service.userRepository.GetById(id);
            Response<User> response = new Response<User>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetByUsername(Request request)
        {
            string username = request.methodParams[0];
            User returnValue = service.userRepository.GetByUsername(username);
            Response<User> response = new Response<User>(){value = returnValue};
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