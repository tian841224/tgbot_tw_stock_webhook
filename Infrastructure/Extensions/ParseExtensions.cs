namespace TGBot_TW_Stock_Webhook.Infrastructure.Extensions
{
    public static class ParseExtensions
    {
        /// <summary>
        /// 將字串轉換為長整數（移除千分位符號）
        /// </summary>
        public static long ParseToLong(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            return long.TryParse(value.Replace(",", ""), out long result) ? result : 0;
        }

        /// <summary>
        /// 將字串轉換為小數
        /// </summary>
        public static decimal ParseToDecimal(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            return decimal.TryParse(value, out decimal result) ? result : 0;
        }

        /// <summary>
        /// 從 HTML 內容提取漲跌符號
        /// </summary>
        public static string ExtractUpDownSign(this string? htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
                return "";
            if (htmlContent.Contains("color:red"))
                return "+";
            else if (htmlContent.Contains("color:green"))
                return "-";
            return "";
        }

        /// <summary>
        /// 將字串轉換為整數（移除千分位符號）
        /// </summary>
        public static int ParseToInt(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            return int.TryParse(value.Replace(",", ""), out int result) ? result : 0;
        }

        /// <summary>
        /// 將民國年日期轉換為 DateTime
        /// </summary>
        /// <param name="taiwanDate">民國年日期 (格式: 113/11/01)</param>
        /// <returns>DateTime 格式日期</returns>
        public static DateTime ParseTaiwanDate(this string taiwanDate)
        {
            try
            {
                // 分割年月日
                string[] dateParts = taiwanDate.Split('/');
                if (dateParts.Length != 3)
                {
                    throw new FormatException($"Invalid date format: {taiwanDate}");
                }

                // 轉換民國年為西元年
                int year = int.Parse(dateParts[0]) + 1911;
                int month = int.Parse(dateParts[1]);
                int day = int.Parse(dateParts[2]);

                return new DateTime(year, month, day);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error converting Taiwan date: {taiwanDate}", ex);
            }
        }
    }
}
