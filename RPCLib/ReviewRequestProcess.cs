using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClassLib;
namespace RPCLib
{
    public class ReviewRequestProcess
    {
        Service service;
        Socket handler;
        public ReviewRequestProcess(Service service, Socket handler)
        {
            this.service = service;
            this.handler = handler;
        }
        public void ProcessRequest(Request request)
        {
            string method = request.nameOfMethod;
            if(method == "reviewRepository.Insert")
            {
                ProcessInsert(request);
            }
            else if(method == "reviewRepository.DeleteById")
            {
                ProcessDeleteById(request);
            }
            else if(method == "reviewRepository.DeleteByFilmId")
            {
                ProcessDeleteByFilmId(request);
            }
            else if(method == "reviewRepository.Update")
            {
                ProcessUpdate(request);
            }
            else if(method == "reviewRepository.GetById")
            {
                ProcessGetById(request);
            }
            else if(method == "reviewRepository.GetSearchPagesCount")
            {
                ProcessGetSearchPagesCount(request);
            }
            else if(method == "reviewRepository.GetSearchPage")
            {
                ProcessGetSearchPage(request);
            }
            else if(method == "reviewRepository.GetAllFilmReviews")
            {
                ProcessGetAllFilmReviews(request);
            }
            else if(method == "reviewRepository.GetAllUserReviews")
            {
                ProcessGetAllUserReviews(request);
            }
            else if(method == "reviewRepository.GetAverageFilmRating")
            {
                ProcessGetAverageFilmRating(request);
            }
        }
        private void ProcessInsert(Request request)
        {
            Review review = Review.ReviewParser(request.methodParams[0]);
            long returnValue = service.reviewRepository.Insert(review);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessDeleteById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            int returnValue = service.reviewRepository.DeleteById(id);
            Response<int> response = new Response<int>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessDeleteByFilmId(Request request)
        {
            int filmId = int.Parse(request.methodParams[0]);
            long returnValue = service.reviewRepository.DeleteByFilmId(filmId);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessUpdate(Request request)
        {
            long id = long.Parse(request.methodParams[0]);
            Review review = Review.ReviewParser(request.methodParams[1]);
            long returnValue = service.reviewRepository.Update(id, review);
            Response<long> response = new Response<long>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            Review returnValue = service.reviewRepository.GetById(id);
            Response<Review> response = new Response<Review>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetSearchPagesCount(Request request)
        {
            string toSearch = request.methodParams[0];
            int returnValue = service.reviewRepository.GetSearchPagesCount(toSearch);
            Response<int> response = new Response<int>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetSearchPage(Request request)
        {
            string toSearch = request.methodParams[0];
            int page = int.Parse(request.methodParams[1]);
            List<Review> returnValue = service.reviewRepository.GetSearchPage(toSearch, page);
            Response<List<Review>> response = new Response<List<Review>>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetAllFilmReviews(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            List<Review> returnValue = service.reviewRepository.GetAllFilmReviews(id);
            Response<List<Review>> response = new Response<List<Review>>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetAllUserReviews(Request request)
        {
            int id = int.Parse(request.methodParams[0]);
            List<Review> returnValue = service.reviewRepository.GetAllUserReviews(id);
            Response<List<Review>> response = new Response<List<Review>>(){value = returnValue};
            SendResponse(response);
        }
        private void ProcessGetAverageFilmRating(Request request)
        {
            int filmId = int.Parse(request.methodParams[0]);
            double returnValue = service.reviewRepository.GetAverageFilmRating(filmId);
            Response<double> response = new Response<double>(){value = returnValue};
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