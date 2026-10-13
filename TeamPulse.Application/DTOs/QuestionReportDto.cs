namespace TeamPulse.Application.DTOs;

public class QuestionReportDto
{
    public long QuestionId { get; set; }

    public string Question { get; set; } = string.Empty;

    public int ResponseCount { get; set; }

    public double AverageRating { get; set; }
}