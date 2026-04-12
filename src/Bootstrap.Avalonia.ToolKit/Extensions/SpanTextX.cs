namespace Bootstrap.Avalonia.ToolKit.Extensions;

/// <summary>
/// Static Extension Provider for ReadOnlySpan Data Transformation.
/// </summary>
public static class SpanTextX
{
    /// <summary>
    /// Performs a Buffer Transformation on a Character Sequence to generate a PascalCase Identifier prefixed with 'Bs'.
    /// </summary>
    /// <param name="segment">The ReadOnlySpan Source Buffer containing the Raw Character Data.</param>
    /// <returns>A Serialized String Instance of the Processed Buffer.</returns>
    public static string ToBsPascalCase(this ReadOnlySpan<char> segment)
    {
        // Guard Clause: Return Default Prefix if the Span Length is Zero.
        if (segment.IsEmpty) return "Bs";

        // Stack Allocation: Pre-calculating Memory Requirements for the Intermediate Buffer.
        // Length = Source Length + Offset for the 'Bs' Constant.
        Span<char> intermediateBuffer = stackalloc char[segment.Length + 2];
        intermediateBuffer[0] = 'B';
        intermediateBuffer[1] = 's';

        var cursor = 2;
        var toggleUpper = true;

        // Iterating through the Source Memory Segment.
        for (var index = 0; index < segment.Length; index++)
        {
            var charValue = segment[index];

            // Delimiter Check: Triggering the Uppercase State Machine logic.
            if (charValue == '-')
            {
                toggleUpper = true;
                continue;
            }

            // Bitwise/Invariant Mutation based on the Toggle State.
            intermediateBuffer[cursor++] = toggleUpper
                ? char.ToUpperInvariant(charValue)
                : charValue;

            toggleUpper = false;
        }

        // Heap Allocation: Slicing the Buffer to the Write-Cursor and Casting to String.
        return new string(intermediateBuffer[..cursor]);
    }
}