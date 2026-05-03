namespace DoYouFeelLucky.Common.Models;

public class BaseResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}