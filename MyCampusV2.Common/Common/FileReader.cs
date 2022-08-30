using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyCampusV2.Common
{
    public class FileReader
    {
        public string ReadText(string path)
        {
            string text = "";
            FileReader fileReader = new FileReader();


            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }

            return text;
        }
    }
}
