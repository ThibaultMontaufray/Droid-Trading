namespace Droid_trading
{
    using System;
    using System.IO;

    public static class Logger
    {
        #region Attribute
        #endregion

        #region Properties
        #endregion

        #region Methods public
        public static void LogValue(double value)
        {
            using (StreamWriter sw = File.AppendText(string.Format(@"Logs_{0}.csv", DateTime.Now.ToString("yyyyMMdd"))))
            {
                sw.WriteLine(string.Format("{0};{1}", DateTime.Now.ToString("HHmmssfff"), value));
            }
        }
        #endregion

        #region Methods private
        #endregion
    }
}