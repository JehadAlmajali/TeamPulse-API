namespace TeamPulse.Application.DTOs;

public class CategoryReportDto
{
    public string Category { get; set; } = string.Empty;

    public int ResponseCount { get; set; }

    public double AverageRating { get; set; }
}