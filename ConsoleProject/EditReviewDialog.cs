namespace ConsoleProject
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
    }
}