﻿using System;

namespace CacheExplorer.Extensions
{
  public static class StringExtensions
  {
    public static int Similarity(this string source, string target)
    {
      var n = source.Length;
      var m = target.Length;
      var d = new int[n + 1, m + 1];

      // Step 1
      if (n == 0)
      {
        return m;
      }

      if (m == 0)
      {
        return n;
      }

      // Step 2
      for (var i = 0; i <= n; d[i, 0] = i++)
      {
        // ??
      }

      for (var j = 0; j <= m; d[0, j] = j++)
      {
        // ??
      }

      // Step 3
      for (var i = 1; i <= n; i++)
      {
        //Step 4
        for (var j = 1; j <= m; j++)
        {
          // Step 5
          var cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

          // Step 6
          d[i, j] = Math.Min(
              Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
              d[i - 1, j - 1] + cost);
        }
      }
      // Step 7
      return d[n, m];
    }
  }
}
