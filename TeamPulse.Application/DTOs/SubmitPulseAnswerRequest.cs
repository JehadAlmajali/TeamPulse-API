namespace TeamPulse.Application.DTOs;

public class SubmitPulseAnswerRequest
{
    public long QuestionId { get; set; }
    public int Rating { get; set; }
}