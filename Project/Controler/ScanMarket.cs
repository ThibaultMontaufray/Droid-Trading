using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Droid_trading
{
    public delegate void EventHandlerScanMarket();
    public class ScanMarket
    {
        #region Attribute
        public event EventHandlerScanMarket PriceUpdated;

        private const string MARKETWEBSITE = @"https://www.tradingview.com/chart/?symbol=FX:"; // if someone had free riths better idea, you're welcome !

        private System.Windows.Forms.Timer _timer;
        private bool _watching = false;
        private double _lastPrice;
        private System.Windows.Forms.WebBrowser _webBrowser;
        private string _diff;
        private string _lastDiff;
        private FOREX _forex;
        private DateTime _lastValDate;
        #endregion

        #region Properties
        public FOREX Forex
        {
            get { return _forex; }
            set { _forex = value; }
        }
        public double LastPrice
        {
            get { return _lastPrice; }
            set { _lastPrice = value; }
        }
        #endregion

        #region Constructor
        public ScanMarket(FOREX f)
        {
            Init();
            _forex = f;

            ResetWebBrow();
            _lastPrice = double.NaN;
        }
        #endregion

        #region Methods public
        public void Start()
        {
            _watching = true;
            _timer.Start();
        }
        public void Stop()
        {
            _watching = false;
            _timer.Stop();
        }
        #endregion

        #region Methods private
        private void Init()
        {
            // 
            // timer
            // 
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 250;
            _timer.Tick += _timer_Tick;
        }
        private void ResetWebBrow()
        {
            _lastValDate = DateTime.Now;
            _webBrowser = new System.Windows.Forms.WebBrowser();
            _webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            _webBrowser.Name = "webBrowser1";
            _webBrowser.Size = new System.Drawing.Size(20, 384);
            _webBrowser.TabIndex = 18;
            _webBrowser.Url = new Uri(MARKETWEBSITE + _forex.ToString());
            _webBrowser.ScriptErrorsSuppressed = true;
        }
        [STAThread]
        private string GetPage()
        {
            if (_webBrowser.Url == null) { _webBrowser.Url = new Uri(MARKETWEBSITE + _forex.ToString()); }
            if (_webBrowser.Document != null && _webBrowser.Document.Body != null)
            {
                if (_lastValDate < DateTime.Now.AddMinutes(-1) && !_webBrowser.DocumentText.Contains("<span")) { _webBrowser.Refresh(); }
                return _webBrowser.Document.Body.InnerHtml;
            }
            return string.Empty;
        }
        private void GetMarketPrice(string webPage)
        {
            try
            {
                if (string.IsNullOrEmpty(webPage)) return;
                string[] tab = Regex.Split(webPage, "<span class=\"dl-header-price\">");// .Contains("title=\"Last price\"");
                if (tab.Length > 1)
                {
                    string[] tab1 = Regex.Split(tab[1], "<");
                    _diff = tab1[6].Split('>')[tab1[6].Split('>').Length - 1];
                    if (!string.IsNullOrEmpty(tab1[0]))
                    {
                        string price1 = tab1[0];
                        if (price1.Length < 6)
                        {
                            if (_lastPrice.Equals(double.NaN) || price1.Length >= _lastPrice.ToString().Length) return;
                            double missingVal;
                            if (!double.TryParse(_lastPrice.ToString().Substring(price1.Length, 1), out missingVal)) return;
                            if (_lastDiff == _diff)
                            {
                                price1 += missingVal;
                            }
                            else
                            {
                                price1 += tab1[1].ToLower().Contains("min") ? (missingVal - 1) % 10 : (missingVal + 1) % 10;
                            }
                            for (int i = 0; i < 6 - price1.Length; i++)
                            {
                                price1 += 0;
                            }
                        }
                        if (price1.Length < 6) return;

                        _lastDiff = _diff;
                        string[] tab2 = Regex.Split(tab1[2], ">");
                        string price2 = tab2[1];
                        if (double.TryParse(price1 + price2, out _lastPrice))
                        {
                            _lastValDate = DateTime.Now;
                            LogPrice();
                            if (PriceUpdated != null)
                            {
                                PriceUpdated();
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("failled to get market : " + exp.Message);
            }
        }
        private void LogPrice()
        {
            using (StreamWriter sw = File.AppendText(string.Format(@"Logs_{0}_{1}.csv", _forex, DateTime.Now.ToString("yyyyMMdd"))))
            {
                sw.WriteLine(string.Format("{0};{1}", DateTime.Now.ToString("yyyyMMdd:HHmmssfff"), _lastPrice));
            }
        }
        #endregion

        #region Event
        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            GetMarketPrice(GetPage());
            if (_watching) _timer.Start();
        }
        #endregion
    }
}
