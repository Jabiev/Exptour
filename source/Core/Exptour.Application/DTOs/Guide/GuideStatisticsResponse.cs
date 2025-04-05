namespace Exptour.Application.DTOs.Guide;

public record GuideStatisticsResponse(int TotalCount, int TotalGuideInvolvements, Percentage Percentage);
public record Percentage(decimal Male, decimal Female, decimal MaleGuideAssignment, decimal FemaleGuideAssignment, decimal EnglishSpeaking, decimal Polyglot);
