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
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> AnalyzeFile(string filePath)
        {
            var data = File.ReadAllText(filePath);
            var lines = data.Replace("\r", "").Split('\n');
            var list = new List<Dictionary<string, string>>();
            var obj = new Dictionary<string, string>();
            var bodyMode = false;
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "BEGIN:VNOTE")
                {
                    obj = new Dictionary<string, string>();
                    bodyMode = false;
                }
                else if (lines[i] == "END:VNOTE")
                {
                    list.Add(obj);
                    obj = null;
                    bodyMode = false;
                }
                else if (lines[i].StartsWith("BODY"))
                {
                    var idx = lines[i].IndexOf(":");
                    obj["BODY"] = ChopEndEqualSymbol(lines[i].Substring(idx + 1));
                    bodyMode = true;

                    var header = lines[i].Substring(0, idx);
                    var h = header.Split(';');
                    foreach(var s in h)
                    {
                        var elems = s.Split('=');
                        if (elems.Length == 2)
                        {
                            obj[elems[0]] = elems[1];
                        }
                    }
                }
                else {
                    var idx = lines[i].IndexOf(":");
                    if (idx > -1)
                    {
                        var key = lines[i].Substring(0, idx);
                        var val = lines[i].Substring(idx + 1);
                        if (Array.IndexOf(CMDS, key) > -1)
                        {
                            obj[key] = val;
                            bodyMode = false;
                        }
                    }
                    else if (bodyMode)
                    {
                        Console.WriteLine(lines[i]);
                        obj["BODY"] += ChopEndEqualSymbol(lines[i]);
                    }
                }
            }

            for(var i = 0; i < list.Count; i++)
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
