namespace ClusterPoc
{
    using System;

    /// <summary>
    /// Contains approximate string matching, 
    /// A string metric for measuring the edit distance between two sequences.
    /// https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
    /// </summary>
    static class LevenshteinDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            //validations...
            if (n == 0) return m;
            if (m == 0) return n;

            // step 2
            for (int i = 0; i <= n; d[i, 0] = i++) {}
            for (int j = 0; j <= m; d[0, j] = j++){}

            // step 3
            for (int i = 1; i <= n; i++)
            {
                //step 4
                for (int j = 1; j <= m; j++)
                {
                    // step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // step 7
            return d[n, m];
        }
    }
}
