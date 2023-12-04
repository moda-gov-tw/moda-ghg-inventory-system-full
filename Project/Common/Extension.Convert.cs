using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common
{
    public static partial class Extensions
    {
        #region 轉換為long
        /// <summary>
        /// 將object轉換為long，若轉換失敗，則返回0。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ParseToLong(this object obj)
        {
            try
            {
                return long.Parse(obj.ToString());
            }
            catch
            {
                return 0L;
            }
        }

        /// <summary>
        /// 將object轉換為long，若轉換失敗，則返回指定值。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ParseToLong(this string str, long defaultValue)
        {
            try
            {
                return long.Parse(str);
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region 轉換為int
        /// <summary>
        /// 將object轉換為int，若轉換失敗，則返回0。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int? ParseToInt(this object str)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch
            {
                return null; // 在 catch 區塊內回傳 null
            }
        }

        /// <summary>
        /// 將object轉換為int，若轉換失敗，則返回指定值。不拋出異常。 
        /// null返回預設值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int? ParseToInt(this object str, int defaultValue)
        {
            if (str == null || string.IsNullOrEmpty(str.ToString()))
            {
                return null;
            }
            try
            {
                if (str.ToString().Contains(".") || Convert.ToInt32(str) < 0)
                {
                    return defaultValue;
                }
                return Convert.ToInt32(str);
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region 轉換為short
        /// <summary>
        /// 將object轉換為short，若轉換失敗，則返回0。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static short ParseToShort(this object obj)
        {
            try
            {
                return short.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 將object轉換為short，若轉換失敗，則返回指定值。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static short ParseToShort(this object str, short defaultValue)
        {
            try
            {
                return short.Parse(str.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region 轉換為demical
        /// <summary>
        /// 將object轉換為demical，若轉換失敗，則返回指定值。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal? ParseToDecimal(this object str, decimal defaultValue)
        {
            try
            {
                if (str == null || string.IsNullOrEmpty(str.ToString()))
                {
                    return null;
                }
                else
                {
                    return decimal.Parse(str.ToString());
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 將object轉換為demical，若轉換失敗，則返回0。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseToDecimal(this object str)
        {
            try
            {

                return decimal.Parse(str.ToString());
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region 轉化為bool
        /// <summary>
        /// 將object轉換為bool，若轉換失敗，則返回false。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ParseToBool(this object str)
        {
            try
            {
                return bool.Parse(str.ToString());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 將object轉換為bool，若轉換失敗，則返回指定值。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ParseToBool(this object str, bool result)
        {
            try
            {
                return bool.Parse(str.ToString());
            }
            catch
            {
                return result;
            }
        }
        #endregion

        #region 轉換為float
        /// <summary>
        /// 將object轉換為float，若轉換失敗，則返回0。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ParseToFloat(this object str)
        {
            try
            {
                return float.Parse(str.ToString());
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 將object轉換為float，若轉換失敗，則返回指定值。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ParseToFloat(this object str, float result)
        {
            try
            {
                return float.Parse(str.ToString());
            }
            catch
            {
                return result;
            }
        }
        #endregion

        #region 轉換為Guid
        /// <summary>
        /// 將string轉換為Guid，若轉換失敗，則返回Guid.Empty。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ParseToGuid(this string str)
        {
            try
            {
                return new Guid(str);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        #endregion


        #region 民國年分
        public static string ParseToTWDate(this object obj, string defaultValue)
        {
            try
            {
                if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                {
                    return string.Empty;
                }
                else
                {
                    string format = "yyy/M/d";
                    DateTime result;
                    CultureInfo culture = new CultureInfo("zh-TW");
                    culture.DateTimeFormat.Calendar = new TaiwanCalendar();

                    if (DateTime.TryParseExact(obj.ToString(), format, culture, DateTimeStyles.None, out result))
                    {
                        return obj.ToString();
                    }
                    else
                    {
                        return defaultValue;

                    }
                }
            }
            catch
            {
                return defaultValue;
            }


        } 
        #endregion

        #region 轉換為DateTime
        /// <summary>
        /// 將obj轉換為DateTime，若轉換失敗，則返回日期最小值。不拋出異常。  
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ParseToDateTime(this object obj)
        {
            try
            {
                if (obj == null)
                {
                    return DateTime.MinValue;
                }
                string str = obj.ToString();
                if (string.IsNullOrWhiteSpace(str))
                {
                    return DateTime.MinValue;
                }
                if (str.Contains("-") || str.Contains("/"))
                {
                    return DateTime.Parse(str);
                }
                else
                {
                    int length = str.Length;
                    switch (length)
                    {
                        case 4:
                            return DateTime.ParseExact(str, "yyyy", System.Globalization.CultureInfo.CurrentCulture);
                        case 6:
                            return DateTime.ParseExact(str, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                        case 8:
                            return DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        case 10:
                            return DateTime.ParseExact(str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
                        case 12:
                            return DateTime.ParseExact(str, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
                        case 14:
                            return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                        default:
                            return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    }
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// 將string轉換為DateTime，若轉換失敗，則返回日期最小值。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ParseToDateTime(this string str)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return DateTime.MinValue;
                }
                if (str.Contains("-") || str.Contains("/"))
                {
                    return DateTime.Parse(str);
                }
                else
                {
                    int length = str.Length;
                    switch (length)
                    {
                        case 4:
                            return DateTime.ParseExact(str, "yyyy", System.Globalization.CultureInfo.CurrentCulture);
                        case 6:
                            return DateTime.ParseExact(str, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                        case 8:
                            return DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        case 10:
                            return DateTime.ParseExact(str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
                        case 12:
                            return DateTime.ParseExact(str, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
                        case 14:
                            return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                        default:
                            return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    }
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 將string轉換為DateTime，若轉換失敗，則返回預設值。  
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ParseToDateTime(this string str, DateTime? defaultValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return defaultValue.GetValueOrDefault();
                }
                if (str.Contains("-") || str.Contains("/"))
                {
                    return DateTime.Parse(str);
                }
                else
                {
                    int length = str.Length;
                    switch (length)
                    {
                        case 4:
                            return DateTime.ParseExact(str, "yyyy", System.Globalization.CultureInfo.CurrentCulture);
                        case 6:
                            return DateTime.ParseExact(str, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                        case 8:
                            return DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        case 10:
                            return DateTime.ParseExact(str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
                        case 12:
                            return DateTime.ParseExact(str, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
                        case 14:
                            return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                        default:
                            return DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    }
                }
            }
            catch
            {
                return defaultValue.GetValueOrDefault();
            }
        }
        #endregion

        #region 轉換為string
        /// <summary>
        /// 將object轉換為string，若轉換失敗，則返回""。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ParseToString(this object obj)
        {
            try
            {
                if (obj == null)
                {
                    return string.Empty;
                }
                else
                {
                    return obj.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string ParseToStrings<T>(this object obj)
        {
            try
            {
                var list = obj as IEnumerable<T>;
                if (list != null)
                {
                    return string.Join(",", list);
                }
                else
                {
                    return obj.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }

        }
        #endregion

        #region 轉換為double
        /// <summary>
        /// 將object轉換為double，若轉換失敗，則返回0。不拋出異常。  
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ParseToDouble(this object obj)
        {
            try
            {
                return double.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 將object轉換為double，若轉換失敗，則返回指定值。不拋出異常。  
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ParseToDouble(this object str, double defaultValue)
        {
            try
            {
                return double.Parse(str.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region 強制轉換類型
        /// <summary>
        /// 強制轉換類型
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> CastSuper<TResult>(this IEnumerable source)
        {
            foreach (object item in source)
            {
                yield return (TResult)Convert.ChangeType(item, typeof(TResult));
            }
        }
        #endregion
        #region DataTable

        /// <summary>
        /// DataTable轉換成List dynamic
        /// </summary>
        /// <param name="table"></param>
        /// <param name="FilterField">字段過濾，FilterField 為空時,返回DataTable中的全部數</param>
        /// <returns></returns>
        public static List<dynamic> ToDynamicList(DataTable table, params string[] FilterField)
        {
            var modelList = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                dynamic model = new ExpandoObject();
                var dict = (IDictionary<string, object>)model;
                foreach (DataColumn column in table.Columns)
                {
                    if (FilterField.Length != 0)
                    {
                        if (FilterField.Contains(column.ColumnName))
                        {
                            dict[column.ColumnName] = row[column];
                        }
                    }
                    else
                    {
                        dict[column.ColumnName] = row[column];
                    }
                }
                modelList.Add(model);
            }
            return modelList;
        }

        #endregion

        #region 對象屬性
        public static bool HasProperty<T>(object obj, string propertyName)
        {
            return obj != null && obj.GetType().GetProperties().Any(p => p.Name.Equals(propertyName));
        }
        /// <summary>
        ///     取對象屬性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">支援“.”分隔的多級屬性取值。</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(object obj, string propertyName)
        {
            var strs = propertyName.Split('.');

            PropertyInfo property = null;
            object value = obj;

            for (var i = 0; i < strs.Length; i++)
            {
                property = value.GetType().GetProperty(strs[i]);
                value = property.GetValue(value, null);
            }
            return value;
        }
        /// <summary>
        ///     設定對象屬性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">propertyName1.propertyName2.propertyName3</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetPropertyValue<T>(object obj, string propertyName, object value)
        {
            var strs = propertyName.Split('.');

            PropertyInfo property = null;
            object target = obj;

            for (var i = 0; i < strs.Length; i++)
            {
                property = target.GetType().GetProperty(strs[i]);
                if (i < strs.Length - 1)
                    target = property.GetValue(target, null);
            }

            var flag = false; //設定成功標記
            if (property != null && property.CanWrite)
            {
                if (false == property.PropertyType.IsGenericType) //非泛型
                {
                    if (property.PropertyType.IsEnum)
                    {
                        property.SetValue(target, Convert.ChangeType(value, typeof(int)));
                        flag = true;
                    }
                    else if (value.ToString() != property.PropertyType.ToString())
                    {
                        //property.SetValue(target, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, property.PropertyType), null);
                        property.SetValue(target,
                            value == null ? null : Convert.ChangeType(value, property.PropertyType),
                            null);
                        flag = true;
                    }
                }
                else //泛型Nullable<>
                {
                    var genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        //property.SetValue(target, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)), null);
                        property.SetValue(target,
                            value == null
                                ? null
                                : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)),
                            null);
                        flag = true;
                    }
                }
            }

            return flag;
        }
        #endregion
    }
}
