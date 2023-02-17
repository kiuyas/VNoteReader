using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VNoteReader
{
    class VNoteUtil
    {
        /// <summary>
        /// コマンド
        /// </summary>
        private static readonly string[] CMDS = {
            "BEGIN",
            "VERSION",
            "BODY",
            "LAST-MODIFIED",
            "END"
        };

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="vntFilePath">vntファイルパス</param>
        /// <returns>解析結果</returns>
        public static List<Dictionary<string, string>> AnalyzeFile(string vntFilePath)
        {
            var data = File.ReadAllText(vntFilePath);
            var lines = data.Replace("\r", "").Split('\n');
            var list = AnalyzeCore(lines);
            list = ConvertToString(list);
            return list;
        }

        /// <summary>
        /// 解析コア
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static List<Dictionary<string, string>> AnalyzeCore(string[] lines)
        {
            var result = new List<Dictionary<string, string>>();
            var obj = new Dictionary<string, string>();
            var bodyMode = false;
            var block = new StringBuilder();
            for (var i = 0; i < lines.Length; i++)
            {
                block.Append(lines[i] + "\r\n");
                if (lines[i] == "BEGIN:VNOTE")
                {
                    obj = new Dictionary<string, string>();
                    bodyMode = false;
                }
                else if (lines[i] == "END:VNOTE")
                {
                    obj["BLOCK"] = block.ToString();
                    result.Add(obj);
                    obj = null;
                    block.Clear();
                    bodyMode = false;
                }
                else if (lines[i].StartsWith("BODY"))
                {
                    var idx = lines[i].IndexOf(":");
                    obj["BODY"] = ChopEndEqualSymbol(lines[i].Substring(idx + 1));
                    bodyMode = true;

                    var header = lines[i].Substring(0, idx);
                    var h = header.Split(';');
                    foreach (var s in h)
                    {
                        var elems = s.Split('=');
                        if (elems.Length == 2)
                        {
                            obj[elems[0]] = elems[1];
                        }
                    }
                }
                else
                {
                    var cmd = ReadCommandOfLine(lines[i]);
                    if (cmd != null)
                    {
                        var key = lines[i].Substring(0, cmd.Length);
                        var val = lines[i].Substring(cmd.Length + 1);
                        obj[key] = val;
                        bodyMode = false;
                    }
                    else if (bodyMode)
                    {
                        obj["BODY"] += ChopEndEqualSymbol(lines[i]);
                    }
                }
            }

            return result;
        }

        private static string ReadCommandOfLine(string line)
        {
            string command = null;
            foreach (string cmd in CMDS)
            {
                if (line.StartsWith(cmd + ":"))
                {
                    command = cmd;
                    break;
                }
            }
            return command;
        }

        /// <summary>
        /// 末尾の＝記号を削除する
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string ChopEndEqualSymbol(string s)
        {
            if (s.EndsWith("="))
            {
                s = s.Substring(0, s.Length - 1);
            }
            return s;
        }

        /// <summary>
        /// 解析結果の各要素のデータを文字列変換する
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<Dictionary<string, string>> ConvertToString(List<Dictionary<string, string>> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                list[i]["INDEX"] = i.ToString();
                if (list[i]["BODY"] != null)
                {
                    try
                    {
                        list[i]["TEXT"] = ConvertToString(list[i]["BODY"]);
                    }
                    catch (Exception ex)
                    {
                        list[i]["TEXT"] = "エラーで解析できませんでした。詳細=" + ex.ToString();
                    }
                }
                else
                {
                    list[i]["TEXT"] = "エラーで解析できませんでした。詳細=BODYがありません。";
                }
            }

            return list;
        }

        /// <summary>
        /// 文字列に変換する
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ConvertToString(string s)
        {
            int i = 0;
            List<byte> bytes = new List<byte>();
            while (i < s.Length)
            {
                if (s[i] == '=')
                {
                    string temp = s.Substring(i + 1, 2);
                    bytes.Add((byte)Convert.ToInt32(temp, 16));
                    i += 3;
                }
                else
                {
                    bytes.Add((byte)s[i]);
                    i++;
                }
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}
