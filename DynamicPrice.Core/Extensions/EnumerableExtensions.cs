namespace DynamicPrice.Core.Extensions;

public static class EnumerableExtensions
{
	public static T GetRandomItem<T>(this IReadOnlyList<T> list)
	{
		if (list.Count == 0)
			throw new InvalidOperationException("Collection is empty");

		return list[Random.Shared.Next(list.Count)];
	}
}
