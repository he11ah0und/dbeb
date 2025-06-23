namespace dshared.Utils;

public static class StringUtils
{
    public static string? Truncate(this string input, int maxLength)
    {
        return input?[..Math.Min(input.Length, maxLength)];
    }
}