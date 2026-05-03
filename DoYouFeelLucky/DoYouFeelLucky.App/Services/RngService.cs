using DoYouFeelLucky.App.Interfaces;

namespace DoYouFeelLucky.App.Services;

public class RngService : IRngService
{
    private readonly Random _random = new();

    public double Roll() => _random.NextDouble();

    public decimal GetRandomDecimal(decimal min, decimal max) =>
        min + (decimal)_random.NextDouble() * (max - min);
}