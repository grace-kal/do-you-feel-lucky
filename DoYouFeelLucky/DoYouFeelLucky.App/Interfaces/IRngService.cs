namespace DoYouFeelLucky.App.Interfaces;

public interface IRngService
{
    double Roll();
    decimal GetRandomDecimal(decimal min, decimal max);
}