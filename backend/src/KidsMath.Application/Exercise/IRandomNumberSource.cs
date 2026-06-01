namespace KidsMath.Application.Exercise;

public interface IRandomNumberSource
{
    int Next(int minInclusive, int maxExclusive);
    int Next(int maxExclusive);
}

public sealed class RandomNumberSource : IRandomNumberSource
{
    private readonly Random _random;

    public RandomNumberSource(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : Random.Shared;
    }

    public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);

    public int Next(int maxExclusive) => _random.Next(maxExclusive);
}
