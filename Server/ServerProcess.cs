using ClassLib;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RPCLib;
namespace Server
{
    public class ServerProcess
    {
        Service service;
        public ServerProcess(Service service)
        {
            this.service = service;
        }
        public void RunServer()
        {
            IPAddress ipAddress = IPAddress.Loopback;
            int port = 3000;
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
            try
            {
                listener.Bind(endPoint);
                listener.Listen();
                while(true)
                {
                    Console.WriteLine($"Waiting for a connection on port {port}");
                    Socket handler = listener.Accept();
                    Thread newClientThread = new Thread(ProcessClient);
                    newClientThread.Start(handler);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected exception: {ex.Message}");
            }
        }
        private void ProcessClient(object obj)
        {
            Socket handler = (Socket)obj;
            Console.WriteLine($"Connected {handler.RemoteEndPoint}. Waiting for a message...");
            FilmRequestProcess processFilm = new FilmRequestProcess(service, handler);
            ActorRequestProcess processActor = new ActorRequestProcess(service, handler);
            ReviewRequestProcess processReview = new ReviewRequestProcess(service, handler);
            UserRequestProcess processUser = new UserRequestProcess(service, handler);
            RoleRequestProcess processRole = new RoleRequestProcess(service, handler);

            try
            {
                while(true)
                {
                    byte[] bytes = new byte[1024];
                    string xmlRequest = "";
                    while(true)
                    {
                        int receivedBytes = handler.Receive(bytes);
                        xmlRequest += Encoding.UTF8.GetString(bytes, 0, receivedBytes);
                        if(xmlRequest.IndexOf("</request>") > -1)
                        {
                            break;
                        }
                    }
                    Request request = ServerSerializers.DeserializeRequest(xmlRequest);
                    Console.WriteLine($"Request from {handler.RemoteEndPoint}: {request.nameOfMethod}");
                    try
                    {
                        if(request.nameOfMethod.StartsWith("filmRepository."))
                        {
                            processFilm.ProcessRequest(request);
                        }
                        else if(request.nameOfMethod.StartsWith("actorRepository."))
                        {
                            processActor.ProcessRequest(request);
                        }
                        else if(request.nameOfMethod.StartsWith("reviewRepository."))
                        {
                            processReview.ProcessRequest(request);
                        }
                        else if(request.nameOfMethod.StartsWith("userRepository."))
                        {
                            processUser.ProcessRequest(request);
                        }
                        else if(request.nameOfMethod.StartsWith("roleRepository."))
                        {
                            processRole.ProcessRequest(request);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Error: Cannot connect to database.");
                        handler.Disconnect(false);
                        Console.WriteLine($"Client {handler.RemoteEndPoint} was disconnected.");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected exception: {ex.Message}");
                Console.WriteLine($"Client {handler.RemoteEndPoint} was disconnected.");
            }
        }
    }
}