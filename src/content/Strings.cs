using System.Collections.Generic;

namespace VcCoop.src.content
{
    internal class Strings
    {
        /// <summary>
        /// List of the strings
        /// </summary>
        private static Dictionary<int, string> stringList = new Dictionary<int, string>();

        /// <summary>
        /// Load the strings into dictionary list
        /// </summary>
        public static void Load()
        {
            // start indexing
            int index = 1;

            // MISSION STRINGS
            stringList.Add(index++, "Mission failed, try it again!");
            stringList.Add(index++, "Mission failed! This is your last cahnce..");
            stringList.Add(index++, "Last attempt failed, switching to nextmap");
            stringList.Add(index++, "Starting the mission attempt...");
        }

        /// <summary>
        /// Gets strings by index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>String</returns>
        public static string Get(int index)
        {
            if (index < stringList.Count)
            {
                return stringList[index];
            }

            return null;
        }
    }
}
