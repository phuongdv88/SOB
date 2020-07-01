using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Utils
{
    public class UIHelper
    {
        public static readonly Assembly CurrentActionAssembly = Assembly.GetExecutingAssembly();
        public static string EncriptMd5(string str)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(str);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
        public static string FormatCurrency(string currencyCode, object amount, bool unit, bool hideNull = false)
        {
            string amountReturn = string.Empty;

            if (amount != null)
            {
                decimal _amount = Utility.ConvertToDecimal(amount);
                if (hideNull && _amount <= 0) { return amountReturn; }
                CultureInfo culture = (from c in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                       let r = new RegionInfo(c.LCID)
                                       where r != null
                                       && r.ISOCurrencySymbol.ToUpper() == currencyCode.ToUpper()
                                       select c).FirstOrDefault();
                if (culture == null)
                {
                    // fall back to current culture if none is found
                    // you could throw an exception here if that's not supposed to happen
                    culture = CultureInfo.CurrentCulture;

                }
                culture = (CultureInfo)culture.Clone();
                culture.NumberFormat.CurrencySymbol = currencyCode;
                culture.NumberFormat.CurrencyPositivePattern = culture.NumberFormat.CurrencyPositivePattern == 0 ? 2 : 3;
                var cnp = culture.NumberFormat.CurrencyNegativePattern;
                switch (cnp)
                {
                    case 0: cnp = 14; break;
                    case 1: cnp = 9; break;
                    case 2: cnp = 12; break;
                    case 3: cnp = 11; break;
                    case 4: cnp = 15; break;
                    case 5: cnp = 8; break;
                    case 6: cnp = 13; break;
                    case 7: cnp = 10; break;
                }
                culture.NumberFormat.CurrencyNegativePattern = cnp;
                if (unit)
                {
                    amountReturn = _amount.ToString("C" + ((_amount % 1) == 0 ? "0" : "2"), culture).Replace("VND", "đ");
                }
                else
                {
                    amountReturn = _amount.ToString("C" + ((_amount % 1) == 0 ? "0" : "2"), culture).Replace("VND", "");

                }

            }
            return amountReturn;
        }

        public static bool IsValiFolder(string folder)
        {
            const string str = "CON,PRN,AUX,CLOCK$,NUL,COM1,COM2,COM3,COM4,COM5,COM6,COM7,COM8,COM9,LPT1,LPT2,LPT3,LPT4,LPT5,LPT6,LPT7,LPT8,LPT9";
            var folders = str.Split(',');
            int pos = Array.IndexOf(folders, folder);
            if (pos > -1)
            {
                return false;
            }
            return true;
        }
        public string ToText(string input)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(@"<html><body>" + input + "</body></html>");
            return doc.DocumentNode.SelectSingleNode("//body").InnerText;
        }
        public static string Md5HasCode(string plainText)
        {
            //create new instance of md5
            MD5 md5 = MD5.Create();

            //convert the input text to array of bytes
            byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(plainText));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }
        public static string TrimByWord(object title, int numberOfWords = 20, string afterFix = "...")
        {
            if (numberOfWords < 0) numberOfWords = 10;
            var result = title.ToString();
            var resultArray = result.Split(new Char[] { ' ' });

            if (resultArray.Length > numberOfWords)
            {
                List<string> Slice = new List<string>(resultArray).GetRange(0, numberOfWords);
                result = string.Join(" ", Slice.ToArray()) + afterFix;
            }
            return result;
        }
        public static string TrimByWord(object title, int numberOfWords = 20, string splitChar = "", string afterFix = "...")
        {
            if (numberOfWords < 0) numberOfWords = 10;
            var result = title.ToString();
            var resultArray = result.Split(splitChar.ToCharArray());

            if (resultArray.Length > numberOfWords)
            {
                List<string> Slice = new List<string>(resultArray).GetRange(0, numberOfWords);
                result = string.Join(splitChar, Slice.ToArray()) + afterFix;
            }
            return result;
        }
        public static string GetLongDate(object dt)
        {
            if (dt == null) return "";

            var dateTime = Convert.ToDateTime(dt);
            if (dateTime == DateTime.MinValue)
            {
                return "-";
            }
            var dw = string.Empty;
            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    dw = "Thứ Hai";
                    break;
                case DayOfWeek.Tuesday:
                    dw = "Thứ Ba";
                    break;
                case DayOfWeek.Wednesday:
                    dw = "Thứ Tư";
                    break;
                case DayOfWeek.Thursday:
                    dw = "Thứ Năm";
                    break;
                case DayOfWeek.Friday:
                    dw = "Thứ Sáu";
                    break;
                case DayOfWeek.Saturday:
                    dw = "Thứ Bảy";
                    break;
                case DayOfWeek.Sunday:
                    dw = "Chủ Nhật";
                    break;
            }

            return String.Format("{0}, {1}", dw, GetFullDate(dt));
        }
        public static string EventProcess(DateTime start, DateTime stop)
        {
            string htm = "";
            try
            {
                if (start > DateTime.Now)
                {
                    htm = "<a href=\"/su-kien\" class=\"btn btn-green btn-block\">Sắp diễn ra</a>";
                }
                if (start <= DateTime.Now && stop >= DateTime.Now)
                {
                    htm = "<a href=\"/su-kien\" class=\"btn btn-primary btn-block\">Đang diễn ra</a>";
                }
                if (stop <= DateTime.Now)
                {
                    htm = "<a href=\"/su-kien\" class=\"btn btn-default btn-block\">Đã diễn ra</a>";
                }

            }
            catch (Exception)
            {

                return "n/a";
            }
            return htm;
        }
        public static string GetTetxStatusShop(object status)
        {
            var dw = string.Empty;
            var data = status.ToString();
            if (!string.IsNullOrEmpty(data))
            {

                switch (data)
                {
                    case "1":
                        dw = "<i data=\"publish\" class=\"fa fa-check-circle publish\"></i><em>Đang hoạt động</em>";

                        break;
                    case "2":
                        dw = "<i data=\"unpublish\" class=\"fa fa-check-circle unpublish\"></i><em>Tạm ngừng</em>";
                        break;
                    case "3":
                        dw = "<i class=\"fa fa-check-circle close\"></i><em>Đóng cửa</em>";
                        break;

                }
            }

            return dw;
        }
        public static string GetTetxStatusProduct(object status)
        {
            var dw = string.Empty;
            var data = status.ToString();
            if (!string.IsNullOrEmpty(data))
            {

                switch (data)
                {
                    case "1":
                        dw = "<i data=\"publish\" class=\"fa fa-check-circle publish\"></i><em>Hiển thị</em>";

                        break;
                    case "2":
                        dw = "<i data=\"unpublish\" class=\"fa fa-check-circle unpublish\"></i><em>Ẩn</em>";
                        break;


                }
            }

            return dw;
        }
        public static string FomatMoney(object obj)
        {
            string data = string.Empty;

            if (!string.IsNullOrEmpty(obj.ToString()))
            {
                CultureInfo cultureInfo = new CultureInfo("vi-VN");
                data = String.Format(cultureInfo, "{0:C}", obj);
            }
            else
            {
                data = "0.0 đ";
            }
            return data;
        }
      
        public static string GetFullDate(object dt)
        {
            if (dt == null) return "";
            var dateTime = Convert.ToDateTime(dt);
            return string.Format("{0}/{1}/{2} {3}:{4}", dateTime.Day, dateTime.Month, dateTime.Year, dateTime.Hour,
                dateTime.Minute > 10 ? dateTime.Minute + "" : ("0" + dateTime.Minute));
        }
        public static string GetDateTime(object dt)
        {
            if (dt == null) return "";
            var dateTime = Convert.ToDateTime(dt);
            return string.Format("{0}/{1}/{2} {3}:{4}", dateTime.Day, dateTime.Month, dateTime.Year, dateTime.Hour,
                dateTime.Minute > 10 ? dateTime.Minute + "" : ("0" + dateTime.Minute));
        }
        public static string GetOnlyDate(object dt, string symbol = null)
        {
            var dateTime = Convert.ToDateTime(dt);
            if (symbol == null)
            {
                return string.Format("{0}/{1}/{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            }
            else
            {
                var frameFormat = "{0}" + symbol + "{1}" + symbol + "{2}";
                return string.Format(frameFormat, dateTime.Day, dateTime.Month, dateTime.Year);
            }
        }
        public static string GetOnlyDateFormarMonthYear(object dt, string symbol = null)
        {
            string str = string.Empty;
            var dateTime = Convert.ToDateTime(dt);
            if (symbol == null && (DateTime.Compare(DateTime.MinValue, dateTime) != 0))

            {
                str = string.Format("{0}/{1}", dateTime.Month, dateTime.Year);
            }
            else
            {


                str = "Đến nay";
            }
            return str;
        }
        public static string GetOnlyDateWithAddZero(object dt, string symbol = null)
        {
            var dateTime = Convert.ToDateTime(dt);
            if (symbol == null)
            {
                return string.Format("{0}/{1}/{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            }
            else
            {
                var frameFormat = "{0}" + symbol + "{1}" + symbol + "{2}";
                return string.Format(frameFormat, dateTime.Day < 10 ? "0" + dateTime.Day.ToString() : dateTime.Day.ToString(), dateTime.Month < 10 ? "0" + dateTime.Month.ToString() : dateTime.Month.ToString(), dateTime.Year);
            }
        }
        public static string GetOnlyTime(object dt, bool addZeroToFist = false)
        {
            var dateTime = Convert.ToDateTime(dt);
            if (!addZeroToFist)
            {
                return string.Format("{0}:{1}", dateTime.Hour, dateTime.Minute);
            }
            else
            {
                return string.Format("{0}:{1}", dateTime.Hour < 10 ? "0" + dateTime.Hour.ToString() : dateTime.Hour.ToString(), dateTime.Minute < 10 ? "0" + dateTime.Minute.ToString() : dateTime.Minute.ToString());
            }
        }
        public static string GetTicks(object dt, bool isReturnNow = false)
        {
            if (dt == null)
            {
                if (isReturnNow)
                {
                    dt = DateTime.Now;
                }
                else
                {
                    return "0";
                }
            }
            var dateTime = Convert.ToDateTime(dt);
            if (dateTime == DateTime.MinValue)
            {
                if (isReturnNow)
                {
                    dateTime = DateTime.Now;
                }
                else
                {
                    return "0";
                }
            }
            return dateTime.Ticks.ToString();
        }

        public static bool CompareDateNow(DateTime strDate)
        {
            if (DateDiff("mi", DateTime.Now, strDate) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static double DateDiff(string datepart, DateTime startDate, DateTime endDate)
        {
            var diff = endDate.Subtract(startDate);
            double result = 0;
            switch (datepart)
            {
                case "mi":
                    result = diff.TotalMinutes / 1000 * 60;
                    break;
                case "h":
                    result = diff.TotalHours / 1000 * 60 * 60;
                    break;
                case "d":
                    result = diff.TotalDays / 1000 * 60 * 60 * 24;
                    break;
                case "m":
                    result = diff.TotalDays / 1000 * 60 * 60 * 24 * 30;
                    break;
            }
            return result;
        }

        public static string NumberFormat(double num, double decimalNum, bool bolLeadingZero, bool bolParens, bool bolCommas)
        {

            if (num == 0) return "0";

            var tmpNum = num;
            var iSign = num < 0 ? -1 : 1;

            tmpNum *= Math.Pow(10, decimalNum);
            tmpNum = Math.Round(Math.Abs(tmpNum));
            tmpNum /= Math.Pow(10, decimalNum);
            tmpNum *= iSign;

            var tmpNumStr = tmpNum.ToString();

            if (!bolLeadingZero && num < 1 && num > -1 && num != 0)
                if (num > 0)
                    tmpNumStr = tmpNumStr.Substring(1, tmpNumStr.Length);
                else
                    tmpNumStr = "-" + tmpNumStr.Substring(2, tmpNumStr.Length);

            if (bolCommas && (num >= 1000 || num <= -1000))
            {
                var iStart = tmpNumStr.IndexOf(".");
                if (iStart < 0)
                    iStart = tmpNumStr.Length;
                else
                {
                    tmpNumStr = tmpNumStr.Replace(".", ",");
                }

                iStart -= 3;
                while (iStart >= 1)
                {
                    tmpNumStr = tmpNumStr.Substring(0, iStart) + "." + tmpNumStr.Substring(iStart, tmpNumStr.Length);
                    iStart -= 3;
                }
            }
            if (bolParens && num < 0)
                tmpNumStr = "(" + tmpNumStr.Substring(1, tmpNumStr.Length) + ")";
            return tmpNumStr;
        }

        public static string ToTipsyTitle(object title)
        {
            return title == null ? "" : title.ToString().Replace("\"", "\'");
        }
       
        #region Host Thumb, Static...

        
        #endregion

        #region util for compare eval data
        public static bool IsTrue(object evalData)
        {
            try
            {
                return bool.Parse(evalData.ToString());
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool IsEvalString(object evalData, string compareTo = "")
        {
            try
            {
                if (evalData == null)
                {
                    return false;
                }
                else
                {
                    if (evalData.ToString().ToLower().Trim() == compareTo)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        //public static string BuildRealImageUrl(object url, string noImageUrl = "")
        //{
        //    if (url != null && (url + "") != "")
        //        return CmsChannelConfiguration.GetAppSetting("FILE_MANAGER_HTTPDOWNLOAD") + url;
        //    else
        //    {
        //        if (noImageUrl == "")
        //            return CmsChannelConfiguration.GetAppSetting("Domain") + "/images/avatar1.gif";
        //        else
        //            return CmsChannelConfiguration.GetAppSetting(Constants.HOST_BOOKMARK) + noImageUrl;
        //    }
        //}
        //public static string FormatThumb(int width, int height, object url, string noImageUrl = "")
        //{
        //    var hostThumbFormat = CmsChannelConfiguration.GetAppSetting(Constants.HOST_CMS_THUMB_FORMAT);

        //    if (url == null || url.ToString() == "" || url.ToString().Length < 5)
        //    {
        //        if (noImageUrl == "")
        //            url = CmsChannelConfiguration.GetAppSetting("Domain") + "Statics/images/reapair-logo-black.png";
        //        else
        //            url = CmsChannelConfiguration.GetAppSetting("Domain") + noImageUrl;
        //    }
        //    if (url.ToString().StartsWith("http://") || url.ToString().StartsWith("https://"))
        //    {
        //        return url.ToString();
        //    }
        //    else
        //    {
        //        return string.Format(hostThumbFormat, width, height, url);
        //    }
        //}
        public static string GetTime(object dt)
        {
            var dateTime = Convert.ToDateTime(dt);
            var dw = string.Empty;

            var hour = dateTime.Hour < 10 ? "0" + dateTime.Hour.ToString() : dateTime.Hour.ToString();
            var min = dateTime.Minute < 10 ? "0" + dateTime.Minute.ToString() : dateTime.Minute.ToString();

            return String.Format("{0}:{1}", hour, min);
        }
        public static string ConvertToTimeAgo(object dt)
        {
            var dateTime = Convert.ToDateTime(dt);
            if (DateTime.Now.AddDays(-1) > dateTime)
            {
                return GetFullDate(dt) + ":nottimeago";
            }
            else
            {
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
            }
        }

        public static string GetTime(DateTime dt)
        {
            string strTime = string.Empty;

            var curentDate = DateTime.Now;
            var dif = curentDate.Subtract(dt);

            if (dif.TotalSeconds < 60)
                strTime = "Vài giây trước";
            else if (dif.TotalSeconds < 3600)
                strTime = Math.Floor(dif.TotalMinutes) + " phút trước";
            else if (dif.TotalSeconds < 3600 * 24)
                strTime = Math.Floor(dif.TotalHours) + " giờ trước";
            else
            {
                var yy = dt.Year;
                var mm = dt.Month;
                var dd = dt.Day;
                var hr = dt.Hour;
                var mi = dt.Minute;
                var ss = dt.Second;

                strTime = "";
                strTime += dd < 10 ? ('0' + dd.ToString()) : dd.ToString();
                strTime += "/" + (mm < 10 ? ('0' + mm.ToString()) : mm.ToString());
                strTime += "/" + (yy < 10 ? ('0' + yy.ToString()) : yy.ToString());
                strTime += " " + (hr < 10 ? ('0' + hr.ToString()) : hr.ToString());
                strTime += ":" + (mi < 10 ? ('0' + mi.ToString()) : mi.ToString());
            }

            return strTime;
        }

        public static string ConvertToDateTimePicker(object datetime, bool isReturnNow = false)
        {
            if (datetime == null)
            {
                if (isReturnNow)
                {
                    datetime = DateTime.Now;
                }
                else
                {
                    datetime = DateTime.MinValue;
                }
            }
            DateTime dt = DateTime.Parse(datetime.ToString());
            if (dt == DateTime.MinValue)
            {
                if (isReturnNow)
                {
                    dt = DateTime.Now;
                }
                else
                {
                    return "";
                }
            }

            return dt.ToString("dd/MM/yyyy HH:mm");
        }
        public static string ConvertToDatePicker(object datetime, bool isReturnNow = false)
        {
            if (datetime == null)
            {
                if (isReturnNow)
                {
                    datetime = DateTime.Now;
                }
                else
                {
                    datetime = DateTime.MinValue;
                }
            }
            DateTime dt = DateTime.Parse(datetime.ToString());
            if (dt == DateTime.MinValue)
            {
                if (isReturnNow)
                {
                    dt = DateTime.Now;
                }
                else
                {
                    return "";
                }
            }

            return dt.ToString("dd/MM/yyyy");
        }
        public static string ConvertToDateMonthYear(object datetime, bool isReturnNow = false)
        {
            if (datetime == null)
            {
                if (isReturnNow)
                {
                    datetime = DateTime.Now;
                }
                else
                {
                    datetime = DateTime.MinValue;
                }
            }
            DateTime dt = DateTime.Parse(datetime.ToString());
            if (dt == DateTime.MinValue)
            {
                if (isReturnNow)
                {
                    dt = DateTime.Now;
                }
                else
                {
                    return "";
                }
            }

            return dt.ToString("MM/yyyy");
        }

     
        public static int CheckAllowTimeAgo(object dt)
        {
            var dateTime = Convert.ToDateTime(dt);
            return (DateTime.Now.AddDays(-1) - dateTime) > TimeSpan.Zero ? 1 : 0;
        }

        public static string EscapeTitle(object title)
        {
            return title.ToString().Replace("'", "&apos;").Replace("\"", "&quot;");
        }

        public static string FormatNumber(object number)
        {
            return string.Format("{0:n0}", number);
        }
        public static string FormatMobile(object mobile)
        {
            if (mobile == null)
            {
                return "---";
            }
            else
            {
                if (string.IsNullOrEmpty(mobile.ToString()))
                {
                    return "---";
                }
                else
                {
                    return mobile.ToString();
                }
            }
        }
        public static string FormatSex(object sex)
        {
            if (sex == null)
            {
                return "---";
            }
            else
            {
                if (string.IsNullOrEmpty(sex.ToString()))
                {
                    return "---";
                }
                else
                {
                    if (int.Parse(sex.ToString()) == 0)
                    {
                        return "Nữ";
                    }
                    else
                    {
                        return "Nam";
                    }
                }
            }
        }

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string FormatFileSize(Int64 value)
        {
            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            if (mag > 0)
                return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
            return "n/a";
        }
        public static string FormatFileSize(Double value)
        {
            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            if (mag > 0)
                return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
            return "n/a";
        }

      
        #region converData

        public static int BoolToInt(object val)
        {
            if (val == null) return 0;
            if (Utility.ConvertToBoolean(val)) return 1;
            return 0;
        }

        #endregion

        public static string ReplaceQuote(object content)
        {
            if (content != null)
            {
                return content.ToString().Replace("'", "&apos;");
            }
            else
            {
                return "";
            }
        }
        public static string ReFormatPhoneNumber(object phoneNumber)
        {
            if (phoneNumber == null)
            {
                return "";
            }
            else
            {
                return (phoneNumber + "").Replace(" ", "").Replace("+84", "0").Replace(".", "");
            }
        }

        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex).Replace("&amp;", ";").Replace("&nbsp;", " ");
        }

        public static string GeneratorColorAvatar(string chr)
        {
            if (string.IsNullOrEmpty(chr.ToString()))
            {
                return "btn-default";
            }
            chr = (chr + "").ToCharArray()[0].ToString().ToUpper();
            if (chr == "A" || chr == "C")
            {
                return "btn-primary";
            }
            if (chr == "B")
            {
                return "btn-angry";
            }
            if (chr == "G" || chr == "H" || chr == "K" || chr == "Y")
            {
                return "btn-success";
            }
            if (chr == "M" || chr == "N" || chr == "L" || chr == "P")
            {
                return "btn-info";
            }
            if (chr == "Q" || chr == "S" || chr == "U")
            {
                return "btn-warning";
            }
            if (chr == "T")
            {
                return "btn-infidelity";
            }
            if (chr == "V")
            {
                return "btn-happy";
            }
            if (chr == "X" || chr == "O" || chr == "D")
            {
                return "btn-danger";
            }
            return "btn-default";
        }
        public static string GeneratorColorAvatar(object _chr)
        {
            if (string.IsNullOrEmpty(_chr.ToString()))
            {
                return "btn-default";
            }

            var chr = _chr.ToString().ToCharArray()[0].ToString().ToUpper();

            if (chr == "A" || chr == "C")
            {
                return "btn-primary";
            }
            if (chr == "B")
            {
                return "btn-angry";
            }
            if (chr == "G" || chr == "H" || chr == "K" || chr == "Y")
            {
                return "btn-success";
            }
            if (chr == "M" || chr == "N" || chr == "L" || chr == "P")
            {
                return "btn-info";
            }
            if (chr == "Q" || chr == "S" || chr == "U")
            {
                return "btn-warning";
            }
            if (chr == "T")
            {
                return "btn-infidelity";
            }
            if (chr == "V")
            {
                return "btn-happy";
            }
            if (chr == "X" || chr == "O" || chr == "D")
            {
                return "btn-danger";
            }
            return "btn-default";
        }


        

        public static string RemoveScriptTagInHtml(object inputHtml)
        {
            return Regex.Replace(inputHtml == null ? "" : inputHtml.ToString(), "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }


        public static string GetPageNumber(int pageSize, int totalRows)
        {
            return Math.Ceiling((double)totalRows / (double)pageSize) + "";
        }

      

    }
    public static class Utils
    {
        public static Int32 ToInt32Return0<T>(this T input)
        {
            int output = 0;
            if (null != input)
            {
                try
                {
                    output = Convert.ToInt32(input);
                }
                catch (Exception)
                {
                    return output;
                }
            }

            return output;
        }

    }
}