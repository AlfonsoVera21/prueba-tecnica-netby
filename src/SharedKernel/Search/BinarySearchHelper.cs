namespace SharedKernel.Search;

public static class BinarySearchHelper
{
    public static (int Start, int End) FindPrefixRange<T>(IReadOnlyList<T> items, Func<T, string> selector, string prefix)
    {
        if (items.Count == 0)
        {
            return (-1, -1);
        }

        var upperPrefix = prefix.ToUpperInvariant();

        int first = FindBoundary(items, selector, upperPrefix, true);
        if (first == -1)
        {
            return (-1, -1);
        }

        int last = FindBoundary(items, selector, upperPrefix, false);
        return (first, last);
    }

    private static int FindBoundary<T>(IReadOnlyList<T> items, Func<T, string> selector, string prefix, bool searchFirst)
    {
        int left = 0;
        int right = items.Count - 1;
        int result = -1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            var candidate = selector(items[mid]).ToUpperInvariant();

            if (candidate.StartsWith(prefix, StringComparison.Ordinal))
            {
                result = mid;
                if (searchFirst)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
            else if (string.Compare(candidate, prefix, StringComparison.Ordinal) < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return result;
    }
}
