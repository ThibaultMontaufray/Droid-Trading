namespace Droid_trading
{
    using System.Windows.Forms;

    public partial class TradeMonitor : Form
    {
        #region Attribute
        private Account _account;
        #endregion

        #region Properties
        public Account Account
        {
            get { return _account; }
            set { _account = value; }
        }
        #endregion

        #region Constructor
        public TradeMonitor()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Methods public
        #endregion

        #region Methods private
        private void Init()
        {
            _account = new Account();

            tradeReport1.Init(_account);
            accountPreview1.Account = _account;
            foreach (Market market in _account.Markets)
            {
                switch (market.Forex)
                {
                    case FOREX.EURUSD:
                        marketPreViewEURUSD.Init(market);
                        break;
                    case FOREX.GBPUSD:
                        marketPreViewGBPUSD.Init(market);
                        break;
                    case FOREX.NZDUSD:
                        marketPreViewNZDUSD.Init(market);
                        break;
                    case FOREX.USDCHF:
                        marketPreViewUSDCHF.Init(market);
                        break;
                    case FOREX.USDCAD:
                        marketPreViewUSDCAD.Init(market);
                        break;
                    case FOREX.USDJPY:
                        marketPreViewUSDJPY.Init(market);
                        break;
                    case FOREX.AUDCHF:
                        marketPreViewAUDCHF.Init(market);
                        break;
                    case FOREX.AUDJPY:
                        marketPreViewAUDJPY.Init(market);
                        break;
                    case FOREX.AUDUSD:
                        marketPreViewAUDUSD.Init(market);
                        break;
                }
            }
        }
        #endregion
    }
}
