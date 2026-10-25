namespace TeamPulse.Domain.Entities;

public class PulseResponse
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long QuestionId { get; set; }

    public int Rating { get; set; }

    public DateOnly AnsweredDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Question Question { get; set; } = null!;
}