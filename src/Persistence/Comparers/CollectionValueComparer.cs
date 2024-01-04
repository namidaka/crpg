using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Crpg.Persistence.Comparers;

public class CollectionValueComparer<T> : ValueComparer<IList<T>>
{
    public CollectionValueComparer()
        : base((c1, c2) => c1!.SequenceEqual(c2!),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())), c => (IList<T>)c.ToHashSet())
    {
    }
}
