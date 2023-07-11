﻿namespace JiraSchedulingConnectAppService.Common
{
    public class Utils
    {
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
    }
}
