using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Droid_trading
{
    public class Market
    {
        #region Attribute
        private FOREX _forex;
        private ScanMarket _scanner;
        private Calculation _calcul;
        private Account _account;
        private List<Trade> _trades;
        private int _lostCount;
        private double _solde;
        #endregion

        #region Properties
        public List<Trade> Trades
        {
            get { return _trades; }
            set { _trades = value; }
        }
        public Account Account
        {
            get { return _account; }
            set { _account = value; }
        }
        public Calculation Calcul
        {
            get { return _calcul; }
            set { _calcul = value; }
        }
        public FOREX Forex
        {
            get { return _forex; }
            set { _forex = value; }
        }
        public ScanMarket Scanner
        {
            get { return _scanner; }
            set { _scanner = value; }
        }
        public int LostCount
        {
            get { return _lostCount; }
            set { _lostCount = value; }
        }
        public double Solde
        {
            get { return _solde; }
            set { _solde = value; }
        }
        #endregion

        #region Constructor
        public Market(FOREX forex, Account account)
        {
            _solde = 0;
            _lostCount = 0;
            _account = account;
            _forex = forex;
            _trades = new List<Trade>();
            _calcul = new Calculation(this);
            _scanner = new ScanMarket(_forex);
        }
        #endregion

        #region Methods public
        public void UpdateAccount()
        {
            _lostCount = 0;
            _solde = 0;
            foreach (Trade trade in _trades.OrderBy(t => t.Date))
            {
                if (trade.Win == WIN.NO)
                {
                    _solde -= trade.Amount;
                    _lostCount++;
                }
                else if (trade.Win == WIN.YES)
                {
                    _solde += trade.Amount * trade.Rate;
                    _lostCount = 0;
                }
            }
        }
        public int GetTradeAmount()
        {
            switch (_lostCount)
            {
                case 0:
                    return 5;
                case 1:
                    return _account.CurrentSolde - 10 > 0 ? 10 : 5;
                case 2:
                    return _account.CurrentSolde - 25 > 0 ? 25 : 5;
                case 3:
                    return _account.CurrentSolde - 50 > 0 ? 50 : 5;
                case 4:
                    return _account.CurrentSolde - 100 > 50 ? 100 : 5;
                case 5:
                    return _account.CurrentSolde - 250 > 100 ? 250 : 5;
                case 6:
                    return _account.CurrentSolde - 500 > 500 ? 500 : 5;
                default:
                    return 5;
            }
        }
        #endregion

        #region Methods private
        #endregion
    }
}
