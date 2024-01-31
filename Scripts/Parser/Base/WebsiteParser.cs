using System.Collections.Generic;
using System;

namespace SkullMp3Player.Scripts.Parser.Base
{
    class WebsiteParser
    {
        private static readonly Random _random = new();

        public static List<int> GetAllSubstringsIndexes(string mainString, string subString, int startIndex = 0)
        {
            List<int> result = new();
            int index = mainString.IndexOf(subString, startIndex);
            while (index > -1) {
                result.Add(index);
                index = mainString.IndexOf(subString, index + subString.Length);
            }

            return result;
        }

        public static Tuple<List<int>, List<int>> GetIndexes(string mainString, List<int> searchFromIndexes, string startIndexSubstring, string endIndexSubstring)
        {
            List<int> start = new();
            List<int> end = new();

            foreach (int index in searchFromIndexes) {
                int startIndex = mainString.IndexOf(startIndexSubstring, index);
                int endIndex = mainString.IndexOf(endIndexSubstring, startIndex);
                if (startIndex == -1 || endIndex == -1) {
                    break;
                }

                start.Add(startIndex);
                end.Add(endIndex);
            }

            return Tuple.Create(start, end);
        }

        public static List<string> GetSubstringsByIndexes(string mainString, List<int> startIndexes, List<int> endIndexes, int additionalStartIndex = 0)
        {
            List<string> result = new();
            for (int i = 0; i < startIndexes.Count; i++) {
                string substring = mainString.Substring(startIndexes[i] + additionalStartIndex, endIndexes[i] - startIndexes[i] - additionalStartIndex);
                substring = substring.Trim();
                result.Add(substring);
            }

            return result;
        }

        public static int GetRandomNumber(int begin = 1, int end = 30000)
        {
            return _random.Next(begin, end);
        }
    }
}
