namespace Bootstrap.Avalonia.Toolkit.Helpers;

public static class StringHelpers
{
    /// <summary>
    /// Converts a hyphenated span (e.g., "primary-dark") to a prefixed PascalCase string (e.g., "BsPrimaryDark").
    /// Optimized for zero-allocation parsing using stackalloc.
    /// </summary>
    public static string ToBsPascalCase(this ReadOnlySpan<char> source)
    {
        if (source.IsEmpty) return "Bs";

        // Pre-allocate a buffer on the stack (Bs + source length)
        // Stackalloc is lightning fast and bypasses the Garbage Collector.
        Span<char> destination = stackalloc char[2 + source.Length];
        
        destination[0] = 'B';
        destination[1] = 's';

        int destIdx = 2;
        bool nextUpper = true;

        foreach (char c in source)
        {
            if (c == '-')
            {
                nextUpper = true;
                continue;
            }

            // Apply PascalCase logic
            destination[destIdx++] = nextUpper ? char.ToUpperInvariant(c) : c;
            nextUpper = false;
        }

        // Slice the buffer to the actual used length and create the final string
        return new string(destination[..destIdx]);
    }
}