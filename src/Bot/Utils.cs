using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Bot
{
    public class Utils
    {
        public static void WriteJObjectToFile(ref JObject obj, string path, string Comment = "")
        {
            using (var w = GetWriter(path, Comment))
                obj.WriteTo(w);
        }

        public static JsonTextWriter GetWriter(string path, string Comment = "")
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                JsonTextWriter writer = new JsonTextWriter(file);
                writer.WriteComment(Comment);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.IndentChar = ' ';
                return writer;
            }
        }
    }
}
