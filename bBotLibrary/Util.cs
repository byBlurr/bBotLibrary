using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Discord.Net.Bot
{
    public class Util
    {

        /// Checks if the file exists, creates the directory if it doesn't exist.
        public static bool DoesFileExist(string directory, string filename)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            return File.Exists(Path.Combine(directory, filename));
        }
    }
}
