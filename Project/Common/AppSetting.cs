using Microsoft.Extensions.Configuration;

namespace Project.Common
{
    public class AppSetting
    {
        public static IConfiguration Configuration { get; }
            static AppSetting()
            {
                Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .Build();
            }

            /// <summary>
            /// 獲取指定Key的值
            /// </summary>
            /// <param name="key">Key(多級中間用:分割)</param>
            /// <returns></returns>
            public static string GetValue(string key)
            {
                return Configuration[key];
            }

            /// <summary>
            /// 獲取指定Key的值，不存在或類型轉換錯誤會拋出異常
            /// </summary>
            /// <param name="key">Key(多級中間用:分割)</param>
            /// <returns></returns>
            public static T GetValue<T>(string key)
            {
                return Configuration.GetValue<T>(key);
            }

            /// <summary>
            /// 獲取指定Key的值，不存在或類型轉換錯誤會返回預設值
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key">Key(多級中間用:分割)</param>
            /// <param name="objectValue"></param>
            /// <returns></returns>
            public static T GetValue<T>(string key, T objectValue)
            {
                return Configuration.GetValue<T>(key, objectValue);
            }

            /// <summary>
            /// 獲取指定Key下鍵值對集合
            /// </summary>
            /// <param name="key">Key(多級中間用:分割)</param>
            /// <returns></returns>
            //public static IEnumerable<KeyValuePair<string, string>> GetSection(string key)
            //{
            //    var section = Configuration.GetSection(key).AsEnumerable().Where(w => w.Value != null);
            //    return section;
            //}
        }
    
}
