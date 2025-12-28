public static class HashingFunctions
{
    public static ulong Fnv1a64(string input)
    {
        const ulong offset = 14695981039346656037UL;
        const ulong prime = 1099511628211UL;

        ulong hash = offset;
        for (int i = 0; i < input.Length; i++)
        {
            hash ^= input[i];
            hash *= prime;
        }
        return hash;
    }

}
