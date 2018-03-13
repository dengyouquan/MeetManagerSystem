using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PageBar
    {
        /// <summary>
        /// 产生GetPageBar
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static string GetPageBar(int pageIndex, int pageCount)
        {
            if (pageCount == 1) return string.Empty;
            int start = pageIndex - 5;
            if (start < 1)
            {
                start = 1;
            }
            int end = start + 9;
            if (end > pageCount)
            {
                end = pageCount;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = start; i <= end; i++)
            {
                if (i == pageIndex)
                {
                    sb.Append(i);
                }
                else
                {
                    sb.Append(string.Format("<a href='?pageIndex={0}'>{0}</a>",i));
                }
            }
            return sb.ToString();
        }
        public static string GetPageBar(int pageIndex, int pageCount,string id)
        {
            if (pageCount == 1) return string.Empty;
            int start = pageIndex - 5;
            if (start < 1)
            {
                start = 1;
            }
            int end = start + 9;
            if (end > pageCount)
            {
                end = pageCount;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = start; i <= end; i++)
            {
                if (i == pageIndex)
                {
                    sb.Append(i);
                }
                else
                {
                    sb.Append(string.Format("<a href='?pageIndex={0}&id={1}'>{0}</a>", i,id));
                }
            }
            return sb.ToString();
        }

        public static string GetPageBar(int pageIndex, int pageCount, string id,string searchString)
        {
            if (pageCount == 1) return string.Empty;
            int start = pageIndex - 5;
            if (start < 1)
            {
                start = 1;
            }
            int end = start + 9;
            if (end > pageCount)
            {
                end = pageCount;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = start; i <= end; i++)
            {
                if (i == pageIndex)
                {
                    sb.Append(i);
                }
                else
                {
                    sb.Append(string.Format("<a href='?pageIndex={0}&id={1}&searchString={2}'>{0}</a>", i, id, searchString));
                }
            }
            return sb.ToString();
        }
    }
}
