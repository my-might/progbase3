using System.Net;
using System.Net.Sockets;
namespace RPCLib
{
    public class RemoteService
    {
        public RemoteFilmRepository filmRepository;
        public RemoteActorRepository actorRepository;
        public RemoreReviewRepository reviewRepository;
        public RemoteUserRepository userRepository;
        public RemoteRoleRepository roleRepository;
        public bool TryConnect()
        {
            IPAddress ipAddress = IPAddress.Loopback;
            int port = 3000;
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            try
            {
                sender.Connect(remoteEP);
            }
            catch
            {
                return false;
            }
            filmRepository = new RemoteFilmRepository(sender);
            actorRepository = new RemoteActorRepository(sender);
            reviewRepository = new RemoreReviewRepository(sender);
            userRepository = new RemoteUserRepository(sender);
            roleRepository = new RemoteRoleRepository(sender);
            return true;
        }
    }
}