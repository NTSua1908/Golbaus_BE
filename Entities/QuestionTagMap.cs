namespace Golbaus_BE.Entities
{
	public class QuestionTagMap
	{
        public Guid QuestionId { get; set; }
        public Guid TagId { get; set; }
        public virtual Question Question { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
