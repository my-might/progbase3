using Microsoft.Data.Sqlite;
namespace ClassLib
{
    public class Service
    {
        public FilmRepository filmRepository;
        public ActorRepository actorRepository;
        public ReviewRepository reviewRepository;
        public UserRepository userRepository;
        public RoleRepository roleRepository;
        public Service(SqliteConnection connection)
        {
            this.filmRepository = new FilmRepository(connection);
            this.actorRepository = new ActorRepository(connection);
            this.reviewRepository = new ReviewRepository(connection);
            this.userRepository = new UserRepository(connection);
            this.roleRepository = new RoleRepository(connection);
        }
    }
}