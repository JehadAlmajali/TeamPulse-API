namespace TeamPulse.Application.DTOs;

public class TrendReportDto
{
    public DateOnly Date { get; set; }

    public int ResponseCount { get; set; }

    public double AverageRating { get; set; }
}