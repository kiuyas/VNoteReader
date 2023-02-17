using System.Collections.Generic;
using System.IO;

namespace VNoteReader
{
    /// <summary>
    /// エクスポータ
    /// </summary>
    class Exporter
    {
        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="analyzeResult"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<string> Execute(List<Dictionary<string, string>> analyzeResult, string fileName)
        {
            var fileNameList = new List<string>();

            var dirPath = Path.GetDirectoryName(fileName);
            var fileNamePrefix = Path.GetFileNameWithoutExtension(fileName);

            var formatText = analyzeResult.Count > 999 ? "_{0:D8}.txt" : "_{0:D3}.txt";
            if (analyzeResult.Count > 99999999) formatText = "_{0}.txt";
            var i = 0;
            foreach (var x in analyzeResult)
            {
                var fileName2 = fileNamePrefix + string.Format(formatText, i + 1);
                var filePath = Path.Combine(dirPath, fileName2);
                var text = x["TEXT"];
                File.WriteAllText(filePath, text);
                i++;

                fileNameList.Add(fileName2);
            }

            return fileNameList;
        }
    }
}
