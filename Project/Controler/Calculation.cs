namespace Droid_trading
{
    using Aspose.OCR;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class Calculation
    {
        #region Enum
        public enum TREND
        {
            STABLE,
            UP,
            DOWN
        }
        #endregion

        #region Struct
        public struct Price
        {
            public DateTime Date;
            public double Value;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        public struct MarketConstant
        {
            public int ECHANTILLONNAGE; // number of value before starting trading        
            public int DELAYSUPRESSCAN; // history range in minute to calculate support and resistances
            public int DELAYVARIANCE; // delay to calculate variance in minute
            public int DELAYTREND; // delay for trend mid in second
            public double MAXFRESHDELAY; // behalf this value, the data is too old to be treated
            public int DELAY; // delay to take a new value from the broker in ms
            public int TRIGERACCELERATIONMIN; // acceleration minimal mandatory to take a position
            public int TRIGERACCELERATIONMAX; // acceleration maximal mandatory to take a position
            public int TRIGERCOUNT; // nb of value to check the acceleration
            public int DELAYSUMMIT;
            public int SUMMITMINCOUNTVAL; // nb of values in different range to have a summit. avoid if you fot no data during a long period
            public int MINECHANTILLONCOUNT; // nb of value during the last 3 minutes. if too small there is a pb so no trade !
            public int TRAININGLIMIT;
            public int SUPRESNBSUMMIT;
            public int SUPRESDELAYSTABILITY; // delay for stabilisation of the support / resistance in second
            public int TRENDDELAY; // trend in seconds
            public int COUNBIGCHANGES; // count how many times we got a big value if upper that counter then it's a normal value
        }
        #endregion

        #region Attribute
        private const int TRAININGLIMIT = 500000;
        private const int SWRESTORE = 9;
        private MarketConstant _marketConstant;

        private double _minVal; // minimum of the grap to avoid absurd data
        private double _maxVal; // maximum of the grap to avoid absurd data
        private double _maxChange; // maximum changment between 2 values
        private double _exclusionRange; // zone where data no change so much
        private double _summitRange; // to detect a summit 
        private double _exclusionMin; // minimum value to reach to take a trade
        private double _exclusionMax; // maximum value to reach to take a trade
        private double _exclusionHeight; // minimum side of the exclusion zone

        private List<Price> _prices;
        private Queue<Price> _basket;
        private List<Price> _superTrend;
        private Queue<Price> _superTrendGraph;
        private Queue<KeyValuePair<DateTime, double>> _tradeUp;
        private Queue<KeyValuePair<DateTime, double>> _tradeDown;

        private List<Price> _summit;
        private Queue<Price> _summitQueue;
        private List<ExclusionZone> _exclusions;
        private ExclusionZone _lastExclusion;
        private ExclusionZone _support;
        private ExclusionZone _resistance;
        private double _currentAcceleration;
        private TREND _trend;
        private bool _debug = false;
        private double _lastVal = double.MinValue;
        private double _lastSummit = double.MinValue;
        private double _currentVal = double.MinValue;
        private DateTime _lastSummitDate = DateTime.MinValue;
        private DateTime _lastTradeDate = DateTime.MinValue;
        private System.Timers.Timer _timer;
        private DateTime _lastPriceDate;
        private DateTime _lastChangeResistance = DateTime.MaxValue;
        private DateTime _lastChangeSupport = DateTime.MaxValue;
        private int _trainingLine = 0;
        private List<Price> _trainingPrice;
        private bool _startProcessRequested = false;
        private Market _market;
        private double _volatilityHisto = 0.011;
        private double _varianceWeight = 5000;
        private double _volatilityImpl;
        private DateTime _lastVarianceDate;
        private bool _marketReady = false;
        private double _tooBigChangeVal = double.NaN;
        private double _tooBigChangeCount = 0;
        private DateTime _tooBigChangeDate = DateTime.MinValue;
        private double _variance;
        private double _ecarttype;

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        #endregion

        #region Properties
        public MarketConstant MarketConstants
        {
            get { return _marketConstant; }
            set { _marketConstant = value; }
        }
        public double VolatilityImpl
        {
            get { return _volatilityImpl; }
            set { _volatilityImpl = value; }
        }
        public double VolatilityHisto
        {
            get { return _volatilityHisto; }
            set { _volatilityHisto = value; }
        }
        public TREND Trend
        {
            get { return _trend; }
            set { _trend = value; }
        }
        public double CurrentAcceleration
        {
            get { return _currentAcceleration; }
            set { _currentAcceleration = value; }
        }
        public double Support
        {
            get { return _support.MinValZone; }
        }
        public double Resistance
        {
            get { return _support.MaxValZone; }
        }
        public ExclusionZone LastExclusion
        {
            get { return _lastExclusion; }
            set { _lastExclusion = value; }
        }
        public List<ExclusionZone> Exclusions
        {
            get { return _exclusions; }
            set { _exclusions = value; }
        }
        public List<Price> Prices
        {
            get { return _prices; }
            set { _prices = value; }
        }
        public List<Price> Summit
        {
            get { return _summit; }
            set { _summit = value; }
        }
        public Queue<Price> SummitQueue
        {
            get { return _summitQueue; }
            set { _summitQueue = value; }
        }
        public Queue<Price> Basket
        {
            get { return _basket; }
            set { _basket = value; }
        }
        public Queue<Price> SuperTrend
        {
            get { return _superTrendGraph; }
            set { _superTrendGraph = value; }
        }
        public Queue<KeyValuePair<DateTime, double>> TradeDown
        {
            get { return _tradeDown; }
            set { _tradeDown = value; }
        }
        public Queue<KeyValuePair<DateTime, double>> TradeUp
        {
            get { return _tradeUp; }
            set { _tradeUp = value; }
        }
        public double LastSummit
        {
            get { return _lastSummit; }
        }
        public DateTime LastPriceDate
        {
            get { return _lastPriceDate; }
        }
        public Market Market
        {
            get { return _market; }
            set { _market = value; }
        }
        #endregion

        #region Constructor
        public Calculation(Market market, MarketConstant? constants = null)
        {
            _market = market;
            if (constants == null)
            {
                GetMarketConstants();
            }
            else
            {
                _marketConstant = (MarketConstant)constants;
            }
            Init();
        }
        #endregion

        #region Methods public
        public void OpenMarket()
        {
            _startProcessRequested = true;
            _market.Scanner.Start();
            _timer.Start();
        }
        public void CloseMarket()
        {
            _startProcessRequested = false;
            _market.Scanner.Stop();
            _timer.Stop();
        }
        public void SetDebug(bool debug)
        {
            _debug = debug;
            Init();
        }
        #endregion

        #region Methods private
        private void GetMarketConstants()
        {
            _marketConstant = new MarketConstant();
            var dbResult = Droid_database.MySqlAdapter.ExecuteReader(string.Format("select echantillonnage, delaysupresscan, delayvariance, delaytrend, maxfreshdelay, delay, trigeraccmin, trigeraccmax, trigercount, delaysummit, summitmincountval, minechantilloncount, supresnbsummit, delaysupresstability, delaytrend, countbigchanges from s_trading where market = '{0}'", _market.Forex));
            if (dbResult.Count > 0)
            {
                _marketConstant.ECHANTILLONNAGE = int.Parse(dbResult[0][0]);
                _marketConstant.DELAYSUPRESSCAN = int.Parse(dbResult[0][1]);
                _marketConstant.DELAYVARIANCE = int.Parse(dbResult[0][2]);
                _marketConstant.DELAYTREND = int.Parse(dbResult[0][3]);
                _marketConstant.MAXFRESHDELAY = double.Parse(dbResult[0][4]);
                _marketConstant.DELAY = int.Parse(dbResult[0][5]);
                _marketConstant.TRIGERACCELERATIONMIN = int.Parse(dbResult[0][6]);
                _marketConstant.TRIGERACCELERATIONMAX = int.Parse(dbResult[0][7]);
                _marketConstant.TRIGERCOUNT = int.Parse(dbResult[0][8]);
                _marketConstant.DELAYSUMMIT = int.Parse(dbResult[0][9]);
                _marketConstant.SUMMITMINCOUNTVAL = int.Parse(dbResult[0][10]);
                _marketConstant.MINECHANTILLONCOUNT = int.Parse(dbResult[0][11]);
                _marketConstant.SUPRESNBSUMMIT = int.Parse(dbResult[0][12]);
                _marketConstant.SUPRESDELAYSTABILITY = int.Parse(dbResult[0][13]);
                _marketConstant.TRENDDELAY = int.Parse(dbResult[0][14]);
                _marketConstant.COUNBIGCHANGES = int.Parse(dbResult[0][15]);
            }
            else
            {
                _marketConstant.ECHANTILLONNAGE = 2;
                _marketConstant.DELAYSUPRESSCAN = 5;
                _marketConstant.DELAYVARIANCE = 1;
                _marketConstant.DELAYTREND = 5;
                _marketConstant.MAXFRESHDELAY = -60;
                _marketConstant.DELAY = 1;
                _marketConstant.TRIGERACCELERATIONMIN = 10;
                _marketConstant.TRIGERACCELERATIONMAX = 25;
                _marketConstant.TRIGERCOUNT = 3;
                _marketConstant.DELAYSUMMIT = 30;
                _marketConstant.SUMMITMINCOUNTVAL = 10;
                _marketConstant.MINECHANTILLONCOUNT = 200;
                _marketConstant.SUPRESNBSUMMIT = 11;
                _marketConstant.SUPRESDELAYSTABILITY = 90;
                _marketConstant.TRENDDELAY = 2;
                _marketConstant.COUNBIGCHANGES = 500;
            }
        }
        private void Init()
        {
            _trend = TREND.STABLE;
            _currentAcceleration = 0;
            _lastVarianceDate = DateTime.MinValue;

            _basket = new Queue<Price>();
            _summitQueue = new Queue<Price>();
            _superTrendGraph = new Queue<Price>();
            _tradeUp = new Queue<KeyValuePair<DateTime, double>>();
            _tradeDown = new Queue<KeyValuePair<DateTime, double>>();

            _prices = new List<Price>();
            _summit = new List<Price>();
            _superTrend = new List<Price>();
            _exclusions = new List<ExclusionZone>();

            GetMarketDetails();
            ResetSupRes();
            InitTimer();

            if (_debug)
            {
                LoadTrainingFile();
            }
        }
        private void InitTimer()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = _marketConstant.DELAY;
            _timer.Elapsed += _timer_Elapsed;
        }
        private void ProcessPrice()
        {
            try
            {
                bool newPrice = false;
                Price? p = _debug ? GetTrainingPrice() : GetPrice();
                if (p != null) newPrice = FilterPrice((Price)p);

                _marketReady = GetMarketDetails();

                if (_marketReady)
                {
                    if (newPrice)
                    {
                        GetSeuil();
                        DetectTrade();
                        ResolveTrade();
                        TrendGraph();
                        GetVariance();
                    }
                    GetVolatility();
                    GetSummit();
                    GetTrend();
                    GetAcceleration();
                }
                CleanMemory();
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error in the main calcul process : " + exp.Message);
            }
        }

        private Price? GetPrice()
        {
            double price = Market.Scanner.LastPrice;
            if (price.Equals(double.NaN))
            {
                return null;
            }
            else
            {
                return new Price() { Value = price, Date = DateTime.Now };
                //return FilterPrice(price, DateTime.Now); ;
            }
        }
        private Price? GetTrainingPrice()
        {
            try
            {
                _trainingLine++;
                if (_trainingPrice.Count < _trainingLine) return null;
                else return new Price() { Value = _trainingPrice[_trainingLine - 1].Value, Date = _trainingPrice[_trainingLine - 1].Date };
                //else return FilterPrice(_trainingPrice[_trainingLine - 1].Value, _trainingPrice[_trainingLine - 1].date);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            return null;
        }
        private void GetSeuil()
        {
            try
            {
                GetSupport();
                GetResistance();

                if (_resistance.MaxTradeZone - _support.MinTradeZone < _exclusionHeight)
                {
                    ResetSupRes();
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while calculating seuils : " + exp.Message);
            }
        }
        private void GetSupport()
        {
            try
            {
                List<Price> summits = _summit.Where(p => p.Date > _lastPriceDate.AddMinutes(-_marketConstant.DELAYSUPRESSCAN)).ToList();
                if (summits.Count > _marketConstant.SUPRESNBSUMMIT)
                {
                    if (_support.Value.Equals(Double.NaN) || summits.Min(s => s.Value) < _support.MinValZone || summits.Min(s => s.Value) > _support.MaxValZone)
                    {
                        _support = new ExclusionZone(summits.Min(s => s.Value), summits.Max(s => s.Value), _exclusionMin, _exclusionMax);
                        _lastChangeSupport = _lastPriceDate;
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while calculating the support : " + exp.Message);
            }
        }
        private void GetResistance()
        {
            try
            {
                List<Price> summits = _summit.Where(p => p.Date > _lastPriceDate.AddMinutes(-_marketConstant.DELAYSUPRESSCAN)).ToList();
                if (summits.Count > _marketConstant.SUPRESNBSUMMIT)
                {
                    if (_resistance.Value.Equals(Double.NaN) || summits.Max(s => s.Value) < _resistance.MinValZone || summits.Max(s => s.Value) > _resistance.MaxValZone)
                    {
                        _resistance = new ExclusionZone(summits.Min(s => s.Value), summits.Max(s => s.Value), _exclusionMin, _exclusionMax);
                        _lastChangeResistance = _lastPriceDate;
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while calculating the resistance : " + exp.Message);
            }
        }
        private bool GetSummit()
        {
            try
            {
                if (_lastPriceDate.Equals(DateTime.MinValue)) return false;
                DateTime t1 = _lastPriceDate.AddSeconds(-_marketConstant.DELAYSUMMIT);
                DateTime t2 = _lastPriceDate.AddSeconds(-_marketConstant.DELAYSUMMIT * 2 / 3);
                DateTime t3 = _lastPriceDate.AddSeconds(-_marketConstant.DELAYSUMMIT / 3);
                DateTime t4 = _lastPriceDate.AddSeconds(0);

                List<Price> l1 = _prices.Where(p => p.Date > t1 && p.Date < t2).ToList();
                List<Price> l2 = _prices.Where(p => p.Date > t2 && p.Date < t3).ToList();
                List<Price> l3 = _prices.Where(p => p.Date > t3 && p.Date < t4).ToList();

                Price pic = new Price();
                bool picFound = false;

                if (l1.Count > _marketConstant.SUMMITMINCOUNTVAL && l2.Count > _marketConstant.SUMMITMINCOUNTVAL && l3.Count > _marketConstant.SUMMITMINCOUNTVAL)
                {
                    double a = l1.Sum(p => p.Value) / l1.Count;
                    double b = l2.Sum(p => p.Value) / l2.Count;
                    double c = l3.Sum(p => p.Value) / l3.Count;

                    if ((a - b > _summitRange) && (c - b > _summitRange))
                    {
                        pic = l2.Where(p => p.Date > t1 && p.Date < t4).OrderBy(p => p.Value).ToList()[0];
                        _lastSummit = pic.Value;
                        _lastSummitDate = pic.Date;
                        _summit.Add(pic);
                        picFound = true;
                        //Console.WriteLine("SUMMIT DOWN : " + pic.Value + "min:" + _prices.Where(p => p.date > t1 && p.date < t4).OrderBy(p => p.Value).ToList()[0].Value + "max:" + _prices.Where(p => p.date > t1 && p.date < t4).OrderByDescending(p => p.Value).ToList()[0].Value);
                    }
                    else if ((b - a > _summitRange) && (b - c > _summitRange))
                    {
                        pic = l2.Where(p => p.Date > t1 && p.Date < t4).OrderByDescending(p => p.Value).ToList()[0];
                        _lastSummit = pic.Value;
                        _lastSummitDate = pic.Date;
                        _summit.Add(pic);
                        picFound = true;
                        //Console.WriteLine("SUMMIT UP : " + pic.Value + "min:"+ _prices.Where(p => p.date > t1 && p.date < t4).OrderBy(p => p.Value).ToList()[0].Value + "max:"+ _prices.Where(p => p.date > t1 && p.date < t4).OrderByDescending(p => p.Value).ToList()[0].Value);
                    }
                    //Console.WriteLine(string.Format("{0}  |  (a-b):{1} (c-b):{2} (b-a):{3} (b-c):{4} ", _lastPriceDate.ToString("HH:mm:ss"), a-b, c-b, b-a, b-c));

                    if (picFound)
                    {
                        _summitQueue.Enqueue(pic);
                        if (_summit.Where(pr => pr.Value < pic.Value + _summitRange && pr.Value > pic.Value - _summitRange && pr.Date >= _lastPriceDate.AddMinutes(-20)).ToList().Count > _marketConstant.ECHANTILLONNAGE)
                        {
                            List<ExclusionZone> lstExc = _exclusions.Where(e => e.Value == pic.Value).ToList();
                            if (lstExc.Count == 0)
                            {
                                _lastExclusion = new ExclusionZone(pic.Value - _exclusionRange, pic.Value + _exclusionRange, _exclusionMin, _exclusionMax);
                                _lastExclusion.Value = pic.Value;
                                _exclusions.Add(_lastExclusion);
                                _exclusions = _exclusions.Where(e => e.LastUsed > _lastPriceDate.AddMinutes(-20)).ToList();
                            }
                            else
                            {
                                lstExc[0].LastUsed = _lastPriceDate;
                                _lastExclusion = lstExc[0];
                            }
                        }
                    }
                }
                return picFound;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return false;
            }
        }
        private void GetAcceleration()
        {
            try
            {
                if (_prices.Count(p => p.Date > _lastPriceDate.AddSeconds(-2)) == 0) return;
                double lastPrice = _prices[_prices.Count - 1].Value;
                double checkPrice = _prices.Where(p => p.Date > _lastPriceDate.AddSeconds(-2)).OrderBy(p => p.Date).ToList()[0].Value;
                _currentAcceleration = _volatilityHisto / (Math.Abs(checkPrice - lastPrice));
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error on acceleration calculation : " + exp.Message);
            }
        }
        private void GetTrend()
        {
            try
            {
                if (_prices.Count(p => p.Date > _lastPriceDate.AddSeconds(-_marketConstant.DELAYTREND)) == 0) return;
                double lastPrice = _prices[_prices.Count - 1].Value;
                double checkPrice = _prices.Where(p => p.Date > _lastPriceDate.AddSeconds(-_marketConstant.DELAYTREND)).OrderBy(p => p.Date).ToList()[0].Value;

                if (checkPrice - lastPrice > 0) _trend = TREND.DOWN;
                else if (checkPrice - lastPrice < 0) _trend = TREND.UP;
                else _trend = TREND.STABLE;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while calculating the trend : " + exp.Message);
            }
        }
        private void GetVolatility()
        {
            try
            {
                if (_lastPriceDate > _lastVarianceDate.AddMinutes(_marketConstant.DELAYVARIANCE))
                {
                    double histVal;
                    if (_prices.Where(p => p.Date > _lastVarianceDate.AddMinutes(_marketConstant.DELAYVARIANCE)).Count() > 0)
                    {
                        histVal = _prices.Where(p => p.Date > _lastVarianceDate).OrderBy(p => p.Date).ToList()[0].Value;
                        _volatilityImpl = Math.Abs(((_prices[_prices.Count - 1].Value - histVal)) * 100) / histVal;
                        _volatilityHisto = (_volatilityImpl + (_volatilityHisto * _varianceWeight)) / (_varianceWeight + 1);
                        _lastVarianceDate = _prices[_prices.Count - 1].Date;
                        _varianceWeight++;
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while getting volatilities " + exp.Message);
            }
        }
        private bool GetMarketDetails()
        {
            try
            {
                if (_prices.Where(p => p.Date > _lastPriceDate.AddMinutes(-10)).Count() > 500)
                {
                    double mid = _prices.Where(p => p.Date > _lastPriceDate.AddMinutes(-10)).Sum(p => p.Value) / _prices.Count(p => p.Date > _lastPriceDate.AddMinutes(-10));
                    _minVal = _prices.Where(p => p.Date > _lastPriceDate.AddMinutes(-10)).Min(p => p.Value) - (_volatilityHisto * 10);
                    _maxVal = _prices.Where(p => p.Date > _lastPriceDate.AddMinutes(-10)).Max(p => p.Value) + (_volatilityHisto * 10);
                    _maxChange = _ecarttype * 10; // Math.Sqrt(_volatilityHisto); //(_maxVal - mid > mid - _minVal) ? mid - _minVal : _maxVal - mid;
                    _exclusionRange = _volatilityHisto / 400;
                    _summitRange = _volatilityHisto / 600;
                    _exclusionMin = _volatilityHisto / 400;
                    _exclusionMax = _volatilityHisto / 300;
                    _exclusionHeight = _volatilityHisto / 80;
                    return true;
                }
                else
                {
                    return false;
                }
                //_minVal = 1.005;
                //_maxVal = 1.9;
                //_maxRange = 0.00040;
                //_exclusionRange = 0.00003;
                //_summitRange = 0.000016;
                //_exclusionMin = 0.00003;
                //_exclusionMax = 0.00004;
                //_exclusionHeight = 0.00015;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while calculating constants : " + exp.Message);
                return false;
            }
        }
        private void GetVariance()
        {
            double effectif = 0; ;
            double median = 0;

            Dictionary<double, int> vals = new Dictionary<double, int>();
            foreach (Price p in _prices.Where(p => p.Date > _lastPriceDate.AddMinutes(-10)))
            {
                effectif++;
                median += p.Value;
                if (vals.Where(v => v.Key == p.Value).Count() == 0)
                {
                    vals.Add(p.Value, 1);
                }
                else
                {
                    vals[p.Value]++;
                }
            }
            if (effectif == 0) { return; }
            median = median / effectif;

            _variance = 0;
            foreach (var item in vals)
            {
                _variance += item.Value * (item.Key - median) * (item.Key - median);
            }
            _variance = _variance / effectif;
            _ecarttype = Math.Sqrt(_variance);
        }

        private void CleanMemory()
        {
            try
            {
                if (_basket.Count > 10) { _basket.Clear(); }
                if (_summitQueue.Count > 10) { _summitQueue.Clear(); }
                if (_tradeDown.Count > 10) { _tradeDown.Clear(); }
                if (_tradeUp.Count > 10) { _tradeUp.Clear(); }
                if (_summitQueue.Count > 10) { _summitQueue.Clear(); }
                if (_superTrendGraph.Count > 10) { _superTrendGraph.Clear(); }

                if (_summit.Count > 100) { _summit.RemoveAll(x => x.Date < _lastPriceDate.AddMinutes(-10)); }
                if (_prices.Count > 100) { _prices.RemoveAll(x => x.Date < _lastPriceDate.AddMinutes(-10)); }
                if (_superTrend.Count > 100) { _superTrend.RemoveAll(x => x.Date < _lastPriceDate.AddMinutes(-10)); }
                if (_exclusions.Count > 100) { _exclusions.RemoveAll(x => x.LastUsed < _lastPriceDate.AddMinutes(-10)); }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while cleaning memory : " + exp.Message);
            }
        }
        private void TrendGraph()
        {
            try
            {
                if (_superTrend.Count == 0 && _prices.Count > 0 && ((_prices[_prices.Count - 1].Date - _prices[0].Date).TotalSeconds > 20))
                {
                    var lp = _prices.Where(p => p.Date > _lastPriceDate.AddSeconds(-20));
                    Price price = new Price() { Date = _prices[_prices.Count - 1].Date, Value = lp.Sum(p => p.Value) / lp.Count() };
                    _superTrend.Add(price);
                    _superTrendGraph.Enqueue(price);
                }
                else if (_superTrend.Count > 0 && _lastPriceDate.AddSeconds(-20) > _superTrend[_superTrend.Count - 1].Date)
                {
                    var lp = _prices.Where(p => p.Date > _lastPriceDate.AddSeconds(-20));
                    Price price = new Price() { Date = _prices[_prices.Count - 1].Date, Value = lp.Sum(p => p.Value) / lp.Count() };
                    _superTrend.Add(price);
                    _superTrendGraph.Enqueue(price);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while preparing trend graph" + exp.Message);
            }
        }
        private void LoadTrainingFile()
        {
            int year, month, day, hour, min, sec, ms;
            string line;
            double val;
            DateTime dat;
            _trainingPrice = new List<Price>();
            try
            {
                using (var sr = new StreamReader(string.Format(@"Logs_{0}_20160218.csv", _market.Forex.ToString()))) // add the path with your home folder
                {
                    while (sr.Peek() > 0)
                    {
                        line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;
                        string[] price = line.Split(';');
                        if (price.Length == 2)
                        {
                            val = double.Parse(price[1], System.Globalization.NumberStyles.AllowDecimalPoint);

                            year = int.Parse(price[0].Substring(0, 4));
                            month = int.Parse(price[0].Substring(4, 2));
                            day = int.Parse(price[0].Substring(6, 2));
                            hour = int.Parse(price[0].Substring(9, 2));
                            min = int.Parse(price[0].Substring(11, 2));
                            sec = int.Parse(price[0].Substring(13, 2));
                            ms = int.Parse(price[0].Substring(15, 3));

                            dat = new DateTime(2016, 01, 01, hour, min, sec, ms);
                            _trainingPrice.Add(new Price() { Date = dat, Value = val });
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while loading the training file : " + exp.Message);
            }
        }
        private bool FilterPrice(Price p)
        {
            try
            {
                _currentVal = p.Value;
                if (_lastVal == double.MinValue || !_marketReady || _tooBigChangeCount > _marketConstant.COUNBIGCHANGES || Math.Abs(_currentVal - _lastVal) <= _maxChange || _prices[_prices.Count - 1].Date < _lastPriceDate.AddSeconds(-3))
                {
                    _tooBigChangeCount = 0;
                    _tooBigChangeVal = double.NaN;
                    if (!_marketReady || (_currentVal < _maxVal && _currentVal > _minVal))
                    {
                        _lastPriceDate = p.Date;
                        //Logger.LogValue(_currentVal, _market.Forex.ToString());
                        _basket.Enqueue(p);
                        _prices.Add(p);
                    }
                    else
                    {
                        Console.WriteLine("Unprobably value : " + _currentVal);
                    }
                    _lastVal = _currentVal;
                    return true;
                }
                else
                {
                    Console.WriteLine(_market.Forex + " Too big change : " + _lastVal + " -> " + _currentVal + " [" + _maxChange + "]");
                    if (_currentVal != 0)
                    {
                        if ((_currentVal == _tooBigChangeVal) && _tooBigChangeDate > _lastPriceDate.AddSeconds(-1)) { _tooBigChangeCount++; }
                        _tooBigChangeVal = _currentVal;
                        _tooBigChangeDate = _lastPriceDate;
                    }
                    return false;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while filtering input data : " + exp.Message);
                return false;
            }
        }
        private void ResetSupRes()
        {
            try
            {
                _support = new ExclusionZone(double.NaN, double.NaN, _exclusionMin, _exclusionMax);
                _resistance = new ExclusionZone(double.NaN, double.NaN, _exclusionMin, _exclusionMax);
                _lastChangeResistance = DateTime.MaxValue;
                _lastChangeSupport = DateTime.MaxValue;
            }
            catch (Exception exp)
            {
                Console.WriteLine("error while reset Support and Resistances values : " + exp.Message);
            }
        }
        private bool CheckCoherence()
        {
            try
            {
                if (_resistance.MaxValZone.Equals(double.NaN) || _resistance.MaxValZone.Equals(double.NaN)) return false;
                if ((_prices.Count(p => p.Date > _lastPriceDate.AddMinutes(-3)) < _marketConstant.MINECHANTILLONCOUNT) ||
                    (_resistance.MaxValZone - _support.MinValZone < _exclusionRange))
                {
                    ResetSupRes();
                    return false;
                }
                return true;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while checking coherence : " + exp.Message);
                return false;
            }
        }
        private void ResolveTrade()
        {
            bool changed = _market.Trades.Where(t => t.Win == WIN.INPROGRESS).ToList().Count > 0;
            if (changed)
            {
                foreach (Trade trade in _market.Trades.Where(t => t.Win == WIN.INPROGRESS))
                {
                    if (trade.Date <= _lastPriceDate.AddSeconds(_marketConstant.MAXFRESHDELAY))
                    {
                        trade.PriceEnd = _currentVal;
                        if (trade.PriceStart < _currentVal && trade.Binary == BINARY.UP) trade.Win = WIN.YES;
                        else if (trade.PriceStart > _currentVal && trade.Binary == BINARY.UP) trade.Win = WIN.NO;
                        else if (trade.PriceStart < _currentVal && trade.Binary == BINARY.DOWN) trade.Win = WIN.NO;
                        else if (trade.PriceStart > _currentVal && trade.Binary == BINARY.DOWN) trade.Win = WIN.YES;

                        if (trade.Win != WIN.INPROGRESS)
                        {
                            try
                            {
                                Droid_database.MySqlAdapter.ExecuteQuery(string.Format(@"INSERT INTO 'mpartobid01'.'t_trade'
                                ('date','montant','position','status','taux','valeur_depart','valeur_fin','accélération','variance','ecart_type','volatilite','marche')VALUES
                                ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11})",
                                    trade.Date, trade.Amount, trade.Binary.ToString(), trade.Win, trade.Rate, trade.PriceStart, trade.PriceEnd, trade.Acc, trade.Variance, trade.EcarType, trade.Volatility, trade.Forex));
                            }
                            catch (Exception exp)
                            {
                                Console.WriteLine("Cannot insert trade in database : " + exp.Message);
                            }
                            using (StreamWriter sw = File.AppendText(string.Format(@"Resume_{0}.csv", DateTime.Now.ToString("yyyyMMdd"))))
                            {
                                sw.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6}", trade.Date, trade.Win, trade.Volatility.ToString("F6"), trade.Variance.ToString("F6"), trade.Acc.ToString("F6"), trade.PriceStart, trade.PriceEnd));
                            }
                        }
                    }
                }
                _market.Account.UpdateAccount();
                //Console.WriteLine("Current solde : " + _market.Account.CurrentSolde);
            }
        }

        private void DetectTrade()
        {
            int delayTrade = 70 + (_market.LostCount * _market.LostCount * 10);

            if (CheckCoherence() && _market.Account.CurrentSolde > 0
                && _lastTradeDate < _lastPriceDate.AddSeconds(-delayTrade)
                && _lastSummitDate > _lastPriceDate.AddMinutes(-5))
            {
                if (_currentVal < _support.MinValZone
                    && _currentVal > _support.MinTradeZone
                    && _currentVal < (_summit[_summit.Count - 1].Value + _summit[Summit.Count - 2].Value + _summit[Summit.Count - 3].Value + _summit[Summit.Count - 4].Value) / 4
                    && _lastChangeSupport < _lastPriceDate.AddSeconds(-_marketConstant.SUPRESDELAYSTABILITY)
                    && _trend == TREND.DOWN
                    && _currentAcceleration > _volatilityHisto * 10000
                    && _volatilityImpl < _volatilityHisto
                    && _volatilityImpl > (_volatilityHisto / 4))
                {
                    Console.WriteLine("ACCELERATION : " + _currentAcceleration);
                    _tradeDown.Enqueue(new KeyValuePair<DateTime, double>(_lastPriceDate, _currentVal));
                    _lastTradeDate = _lastPriceDate;
                    _market.Trades.Add(new Trade() { Variance = _volatilityHisto, Volatility = _volatilityImpl, Forex = _market.Forex, Acc = _currentAcceleration, Date = _lastPriceDate, Binary = BINARY.DOWN, PriceStart = _currentVal, Amount = _market.GetTradeAmount() });
                    ResetSupRes();
                }
                else if (_currentVal > _resistance.MaxValZone
                    && _currentVal < _resistance.MaxTradeZone
                    && _currentVal > (_summit[_summit.Count - 1].Value + _summit[Summit.Count - 2].Value + _summit[Summit.Count - 3].Value + _summit[Summit.Count - 4].Value) / 4
                    && _lastChangeResistance < _lastPriceDate.AddSeconds(-_marketConstant.SUPRESDELAYSTABILITY)
                    && _trend == TREND.UP
                    && _currentAcceleration > _volatilityHisto * 10000
                    && _volatilityImpl < _volatilityHisto
                    && _volatilityImpl > (_volatilityHisto / 4))
                {
                    Console.WriteLine("ACCELERATION : " + _currentAcceleration);
                    _tradeUp.Enqueue(new KeyValuePair<DateTime, double>(_lastPriceDate, _currentVal));
                    _lastTradeDate = _lastPriceDate;
                    _market.Trades.Add(new Trade() { Variance = _volatilityHisto, Volatility = _volatilityImpl, Forex = _market.Forex, Acc = _currentAcceleration, Date = _lastPriceDate, Binary = BINARY.UP, PriceStart = _currentVal, Amount = _market.GetTradeAmount() });
                    ResetSupRes();
                }
            }
        }
        #endregion

        #region Event
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _timer.Stop();
                ProcessPrice();
                if (_startProcessRequested) _timer.Start();
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error on timer : " + exp.Message);
            }
        }
        #endregion
    }
}
