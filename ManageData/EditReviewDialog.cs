using ClassLib;
namespace ManageData
{
    public class EditReviewDialog : CreateReviewDialog
    {
        public EditReviewDialog()
        {
            this.Title = "Edit review";
        }
        public void SetReview(Review review)
        {
            this.inputOpinion.Text = review.opinion;
            this.inputRating.Text = review.rating.ToString();
            this.inputFilmId.Text = review.filmId.ToString();
        }
        public Review GetReviewEdit()
        {
            return new Review(){opinion = inputOpinion.Text.ToString(),
                            rating = int.Parse(inputRating.Text.ToString()),
                            filmId = int.Parse(inputFilmId.Text.ToString())};
        }
    }
}