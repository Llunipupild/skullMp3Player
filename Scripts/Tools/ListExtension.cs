using System.Collections.Generic;

namespace SkullMp3Player.Scripts.Tools
{
    static class ListExtension
    {
        public static bool IsNullOrEmpty<T>(this List<T>? list)
        {
            return list == null || list.Count == 0;
        }
    }
}
