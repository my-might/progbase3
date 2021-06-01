using System;
using System.Security.Cryptography;
namespace ConsoleProject
{
    public class Authentication
    {
        private UserRepository repo;
        public Authentication(UserRepository repo)
        {
            this.repo = repo;
        }
        public void Registration(User user)
        {
            User exist = repo.GetByUsername(user.username);
            if(exist != null)
            {
                throw new Exception("User with this username is already exist");
            }
            SHA256 sha256Hash = SHA256.Create();
            
        }
    }
}