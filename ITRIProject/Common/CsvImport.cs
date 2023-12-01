using ITRIProject.Controllers;
using System;
using System.Data;

namespace ITRIProject.Common
{
    public class CsvImport : CommonController
    {

        public static void CheckCsvImports(string filePath)
        {
            List<string> errorList = new List<string>();
            int lineCount = 2;//記錄行數
            using (var reader = new StreamReader(filePath))
            {
                string[] headers = reader.ReadLine().Split(',');

                while (!reader.EndOfStream)
                {

                    // 讀取每一行
                    var line = reader.ReadLine();

                    string[] fields = line.Split(',');

                    if (headers.Count() > fields.Count())
                    {
                        errorList.Add($"第{lineCount}行逗號數量有少");

                    }
                    else if (headers.Count() < fields.Count())
                    {
                        errorList.Add($"第{lineCount}行逗號數量有多");

                    }

                    lineCount++;
                }

            }
            if(errorList.Count > 0)
            {
                throw new Exception(string.Join("\n", errorList));
            }

        }


        public static DataTable CsvImports(string filePath)
        {

            DataTable dataTable = new DataTable();
            using (var reader = new StreamReader(filePath))
            {
                // 讀取 CSV 標題行，作為 DataTable 的欄位名稱
                string[] headers = reader.ReadLine().Split(',');

                // 將標題行設置為 DataTable 的欄位
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                // 讀取每一行資料，並將資料加入 DataTable
                while (!reader.EndOfStream)
                {
                    string[] fields = reader.ReadLine().Split(',');

                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        dataRow[i] = fields[i];
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

                return dataTable;
            
        }
    }
}
