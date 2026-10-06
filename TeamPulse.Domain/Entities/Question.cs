namespace TeamPulse.Domain.Entities;
public class Question
{
    public long Id { get; set; }

    public int DisplayOrder { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}