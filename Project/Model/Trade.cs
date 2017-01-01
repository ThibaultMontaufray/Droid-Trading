
using System;

namespace Droid_trading
{
    public class Trade
    {
        #region Attribute
        private DateTime _date;
        private double _amount;
        private BINARY _binary;
        private WIN _win;
        private double _rate;
        private double _priceStart;
        private double _priceEnd;
        private double _acc;
        private FOREX _forex;
        private double _variance;
        private double _volatility;
        private double _ecarType;
        #endregion

        #region Properties
        public double Volatility
        {
            get { return _volatility; }
            set { _volatility = value; }
        }
        public double Variance
        {
            get { return _variance; }
            set { _variance = value; }
        }
        public FOREX Forex
        {
            get { return _forex; }
            set { _forex = value; }
        }
        public double Acc
        {
            get { return _acc; }
            set { _acc = value; }
        }
        public double PriceEnd
        {
            get { return _priceEnd; }
            set { _priceEnd = value; }
        }
        public double PriceStart
        {
            get { return _priceStart; }
            set { _priceStart = value; }
        }
        public WIN Win
        {
            get { return _win; }
            set { _win = value; }
        }
        public BINARY Binary
        {
            get { return _binary; }
            set { _binary = value; }
        }
        public double Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public double Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }
        public double EcarType
        {
            get { return _ecarType; }
            set { _ecarType = value; }
        }
        #endregion

        #region Constructor
        public Trade()
        {
            _win = WIN.INPROGRESS;
            _amount = 5;
            _rate = 0.7;
        }
        #endregion

        #region Methods public
        #endregion

        #region Methods private
        #endregion
    }
}
