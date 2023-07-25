using Microsoft.EntityFrameworkCore;
using MuaVatTu.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MuaVatTu.Common
{
    public class TreeItem<T>
    {
        public T Item { get; set; }
        public List<TreeItem<T>> Children { get; set; }
    }

    public static class ExtensionMethods
    {
        public static T SetPropertyValue<T>(this T obj, string propertyName, object propertyValue)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName)) return obj;

            var objectType = obj.GetType();

            var propertyDetail = objectType.GetProperty(propertyName);
            if (propertyDetail != null && propertyDetail.CanWrite)
            {
                var propertyType = propertyDetail.PropertyType;

                // Check for nullable types
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    if (propertyValue == null || string.IsNullOrWhiteSpace(propertyValue.ToString()))
                    {
                        propertyDetail.SetValue(obj, null);
                        return obj;
                    }

                propertyValue = Convert.ChangeType(propertyValue, propertyType);
                propertyDetail.SetValue(obj, propertyValue);
            }

            return obj;
        }

        public static object GetPropValue(this object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }

        /// <summary>
        ///     FireAndForget
        /// </summary>
        /// <param name="task"></param>
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception)
            {
                // log errors
            }
        }

        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var ub = new UriBuilder(uri);

            // this code block is taken from httpValueCollection.ToString() method
            // and modified so it encodes strings with HttpUtility.UrlEncode
            if (httpValueCollection.Count == 0)
            {
                ub.Query = string.Empty;
            }
            else
            {
                var sb = new StringBuilder();

                for (var i = 0; i < httpValueCollection.Count; i++)
                {
                    var text = httpValueCollection.GetKey(i);
                    {
                        text = HttpUtility.UrlEncode(text);

                        var val = text != null ? text + "=" : string.Empty;
                        var vals = httpValueCollection.GetValues(i);

                        if (sb.Length > 0)
                            sb.Append('&');

                        if (vals == null || vals.Length == 0)
                        {
                            sb.Append(val);
                        }
                        else
                        {
                            if (vals.Length == 1)
                            {
                                sb.Append(val);
                                sb.Append(HttpUtility.UrlEncode(vals[0]));
                            }
                            else
                            {
                                for (var j = 0; j < vals.Length; j++)
                                {
                                    if (j > 0)
                                        sb.Append('&');

                                    sb.Append(val);
                                    sb.Append(HttpUtility.UrlEncode(vals[j]));
                                }
                            }
                        }
                    }
                }

                ub.Query = sb.ToString();
            }

            return ub.Uri;
        }

        #region EF order by name

        private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName,
            bool descending, bool anotherLevel)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var property = Expression.PropertyOrField(param, propertyName);
            var sort = Expression.Lambda(property, param);

            var call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, false);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, false);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, true);
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, true);
        }

        #endregion EF order by name

        #region EF get page

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> dbset, string listSortStr)
        {
            var dataSet = dbset.AsQueryable();
            var orderedDataSet = dataSet as IOrderedQueryable<T>;
            var hasOder = false;
            var listSort = listSortStr.Split(',').ToList();
            for (var i = 0; i < listSort.Count; i++)
            {
                var sortItem = listSort[i];
                if (!string.IsNullOrEmpty(sortItem))
                {
                    sortItem = sortItem.Trim();
                    var sortType = sortItem[0];
                    var propName = sortItem.Substring(1);

                    if (typeof(T).HasProperty(propName))
                    {
                        if (sortType == '+')
                        {
                            orderedDataSet = i == 0
                                ? (orderedDataSet ?? throw new InvalidOperationException()).OrderBy(propName)
                                : (orderedDataSet ?? throw new InvalidOperationException()).ThenBy(propName);
                            hasOder = true;
                        }

                        if (sortType == '-')
                            if (sortType == '-')
                            {
                                orderedDataSet = i == 0
                                    ? (orderedDataSet ?? throw new InvalidOperationException()).OrderByDescending(
                                        propName)
                                    : (orderedDataSet ?? throw new InvalidOperationException()).ThenByDescending(
                                        propName);
                                hasOder = true;
                            }
                    }
                }
            }

            if (hasOder) dataSet = orderedDataSet;
            return dataSet;
        }

        public static async Task<Pagination<T>> GetPageAsync<T>( this IQueryable<T> dbSet, PaginationRequest query)
            where T : class
        {
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {
                dbSet = dbSet.ApplySorting(query.Sort);
                var totals = await dbSet.CountAsync();

                if (query.Size == -1)
                    query.Size = totals;
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                var items = await dbSet.Skip(excludedRows).Take(query.Size.Value).ToListAsync();
                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count,
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var totals = await dbSet.CountAsync();
                var items = await dbSet.ToListAsync();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }

        public static Pagination<T> GetPage<T>(this IQueryable<T> dbset, PaginationRequest query) where T : class
        {
            var dataSet = dbset.AsQueryable().AsNoTracking();
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {
                
                var totals = dataSet.Count();
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                var items = dataSet.Skip(excludedRows).Take(query.Size.Value).ToList();
                items.RemoveAt(items.Count - 1);
                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count,
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var totals = dataSet.Count();
                var items = dataSet.ToList();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }

        #endregion EF get page

        #region Linq Stuff

        public enum ExpressionOption
        {
            Equal,
            NotEqual,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string propertyName,
            object propertyValue, ExpressionOption type)
        {
            var item = Expression.Parameter(typeof(T), "item");
            var prop = Expression.Property(item, propertyName);
            var arrExpr = Expression.Constant(propertyValue);
            BinaryExpression method;
            switch (type)
            {
                case ExpressionOption.Equal:
                    method = Expression.Equal(prop, arrExpr);
                    break;

                case ExpressionOption.NotEqual:
                    method = Expression.NotEqual(prop, arrExpr);
                    break;

                case ExpressionOption.GreaterThan:
                    method = Expression.GreaterThan(prop, arrExpr);
                    break;

                case ExpressionOption.LessThan:
                    method = Expression.LessThan(prop, arrExpr);
                    break;

                case ExpressionOption.GreaterThanOrEqual:
                    method = Expression.GreaterThanOrEqual(prop, arrExpr);
                    break;

                case ExpressionOption.LessThanOrEqual:
                    method = Expression.LessThanOrEqual(prop, arrExpr);
                    break;

                default:
                    method = Expression.Equal(prop, arrExpr);
                    break;
            }

            var lambda = Expression.Lambda<Func<T, bool>>(method, item);

            return source.Where(lambda);
        }

        public static IQueryable<T> WhereContains<T>(this IQueryable<T> source, string propertyName,
            List<Guid> propertyValue)
        {
            var item = Expression.Parameter(typeof(T), "item");
            var prop = Expression.Property(item, propertyName);
            var arrExpr = Expression.Constant(propertyValue);
            var containsMethod = typeof(ICollection<Guid>).GetMethod("Contains");
            var method = Expression.Call(arrExpr, containsMethod ?? throw new InvalidOperationException(), prop);
            var lambda = Expression.Lambda<Func<T, bool>>(method, item);

            return source.Where(lambda);
        }

        #endregion Linq Stuff

        #region Encrypt and Decrypt (extension method)

        public static string EncryptString(this string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg != null)
                    using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                    {
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(text);
                            }

                            var iv = aesAlg.IV;

                            var decryptedContent = msEncrypt.ToArray();

                            var result = new byte[iv.Length + decryptedContent.Length];

                            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                            return Convert.ToBase64String(result);
                        }
                    }

                return null;
            }
        }

        public static string DecryptString(this string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg != null)
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }

                        return result;
                    }

                return null;
            }
        }

        #endregion Encrypt and Decrypt (extension method)

        #region String

       /* public static string ToUniqueName(this string source)
        {
            var result = Utils.RemoveVietnameseSign(source);
            return result.Replace(' ', '-').ToLower();
        }*/

        public static bool IsEmail(this string email)
        {
            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }

        public static bool IsNumeric(this string inputString)
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion String

        #region DateTime

        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        #endregion DateTime
    }
}