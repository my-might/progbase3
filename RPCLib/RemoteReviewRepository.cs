using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class RemoreReviewRepository
    {
        Socket sender;
        public RemoreReviewRepository(Socket sender)
        {
            this.sender = sender;
        }
        public long Insert(Review review)
        {
            string[] parameters = new string[] {review.ReviewConnection()};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.Insert",
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
                nameOfMethod = "reviewRepository.DeleteById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public long DeleteByFilmId(int filmId)
        {
            string[] parameters = new string[] {filmId.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.DeleteByFilmId",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public long Update(long id, Review review)
        {
            string[] parameters = new string[] {id.ToString(), review.ReviewConnection()};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.Update",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public Review GetById(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.GetById",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<Review> response = GetResponse<Review>();
            return response.value;
        }
        public int GetSearchPagesCount(string searchOpinion)
        {
            string[] parameters = new string[] {searchOpinion};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.GetSearchPagesCount",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public List<Review> GetSearchPage(string searchOpinion, int page)
        {
            string[] parameters = new string[] {searchOpinion, page.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.GetSearchPage",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Review>> response = GetResponse<List<Review>>();
            return response.value;
        }
        public List<Review> GetAllFilmReviews(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.GetAllFilmReviews",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Review>> response = GetResponse<List<Review>>();
            return response.value;
        }
        public List<Review> GetAllUserReviews(int id)
        {
            string[] parameters = new string[] {id.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.GetAllUserReviews",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<List<Review>> response = GetResponse<List<Review>>();
            return response.value;
        }
        public double GetAverageFilmRating(int filmId)
        {
            string[] parameters = new string[] {filmId.ToString()};
            Request request = new Request()
            {
                nameOfMethod = "reviewRepository.GetAverageFilmRating",
                methodParams = parameters,
            };
            SendRequest(request);
            Response<double> response = GetResponse<double>();
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