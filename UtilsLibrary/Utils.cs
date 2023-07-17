using System.Dynamic;

namespace JiraSchedulingConnectAppService.Common
{
    public class Utils
    {
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ExtractUpperDigitLetter(string source)
        {
            source = FirstLetterToUpper(source);
            string result = string.Concat(source.Where(c => char.IsUpper(c) || char.IsDigit(c)));

            return result;
        }

        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);
            return str.ToUpper();
        }

        public static bool IsUpperFirstLetter(string str)
        {
            if (str == null)
                return false;
            return char.IsUpper(str[0]);
        }

        public static void AddPropertyToExpando(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public static class MyQuery<T>
        {
            public static (IQueryable<T>, int, int, int) Paging(IQueryable<T> query, int pageNumber)
            {
                pageNumber = PageIndexNormalize(pageNumber);
                int totalPage = 0;

                int totalRecord = query.Count();
                totalPage = (int)Math.Ceiling((decimal)totalRecord / Const.PAGING.NUMBER_RECORD_PAGE);

                IQueryable<T> queryResult = query.Skip(Const.PAGING.NUMBER_RECORD_PAGE * (pageNumber - 1))
                    .Take(Const.PAGING.NUMBER_RECORD_PAGE);

                return (queryResult, totalPage, pageNumber, totalRecord);
            }
        }

        public static int PageIndexNormalize(int page)
        {
            if (page <= 0)
            {
                page = 1;
            }
            return page;
        }

        public static double? GetDaysBeetween2Dates(DateTime? start, DateTime? end)
        {
            if (start == null || end == null)
            {
                return 0;
            }
            return end?.Subtract((DateTime)start).TotalDays;
        }
    }
}
