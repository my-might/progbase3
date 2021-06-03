using System;
using System.Security.Cryptography;
using System.Text;
namespace ClassLib
{
    public static class Authentication
    {
        private static UserRepository repo;
        public static void SetRepository(UserRepository repo1)
        {
            repo = repo1;
        }
        public static int Registration(User user)
        {
            User exist = repo.GetByUsername(user.username);
            if(exist != null)
            {
                throw new Exception("User with this username is already exist");
            }
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, user.password);
            user.password = hash;
            user.isModerator = 0;
            user.registrationDate = DateTime.Now;
            int userId = (int)repo.Insert(user);
            return userId;
        }
        public static User Login(string username, string password)
        {
            User exist = repo.GetByUsername(username);
            if(exist == null)
            {
                throw new Exception("User with this username doesn`t exist");
            }
            SHA256 sha256Hash = SHA256.Create();
            if(!VerifyHash(sha256Hash, password, exist.password))
            {
                throw new Exception("Incorrect passsword");
            }
            return exist;
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            var hashOfInput = GetHash(hashAlgorithm, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}