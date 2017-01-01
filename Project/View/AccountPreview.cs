
namespace Droid_trading.View
{
    using System.Linq;
    using System.Windows.Forms;

    public partial class AccountPreview : UserControl
    {
        #region Attribute
        private Account _account;
        private Timer _timer;
        #endregion

        #region Properties
        public Account Account
        {
            get { return _account; }
            set { _account = value; }
        }
        #endregion

        #region Constructor
        public AccountPreview()
        {
            InitializeComponent();
            InitTimer();
        }
        #endregion

        #region Methods public
        public void UpdateAll()
        {
            int countWin = 0;
            int countTrade = 0;
            if (_account != null)
            {
                foreach (Market market in _account.Markets)
                {
                    countWin += market.Trades.Count(t => t.Win == WIN.YES);
                    countTrade += market.Trades.Count(t => t.Win != WIN.INPROGRESS);
                }

                labelSolde.Text = string.Format("Solde : {0}", _account.CurrentSolde);
                labelTrades.Text = string.Format("Trades : {0}", countTrade);
                labelRate.Text = string.Format("Rate : {0} %", (countTrade == 0) ? "N/A" : ((countWin * 100) / countTrade).ToString());
            }
        }
        #endregion

        #region Methods private
        private void InitTimer()
        {
            _timer = new Timer();
            _timer.Interval = 500;
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, System.EventArgs e)
        {
            _timer.Stop();
            UpdateAll();
            _timer.Start();
        }
        #endregion
    }
}
