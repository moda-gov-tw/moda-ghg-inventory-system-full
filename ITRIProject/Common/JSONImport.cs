using ITRIProject.Controllers;
using System;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITRIProject.Common
{
    public class JSONImport : CommonController
    {
        //Disable
        public static void CheckJSONImports(string filePath)
        {
            
        }


        public static JArray? JSONImports(string filePath)
        {
            JArray? serData = new JArray();
            using (var reader = new StreamReader(filePath))
            {
                string jsonData = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(jsonData))
                serData = JsonConvert.DeserializeObject<JArray>(jsonData);
            }

            return serData;
        }
    }
}
