
namespace Droid_trading
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    public partial class TradeReport : UserControl
    {
        #region Attribute
        private Account _account;
        private Timer _timer;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public TradeReport()
        {
            InitializeComponent();
            InitTimer();
        }
        #endregion

        #region Methods public
        public void Init(Account acc)
        {
            _account = acc;
        }
        public void UpdateTrades()
        {
            try
            {
                dataGridView1.Rows.Clear();
                List<Trade> lt = new List<Trade>();
                if (_account != null)
                {
                    foreach (Market m in _account.Markets)
                    {
                        lt.AddRange(m.Trades);
                    }
                    foreach (Trade trade in lt.OrderByDescending(t => t.Date))
                    {
                        dataGridView1.Rows.Add();
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnBinary.Index].Value = trade.Binary == BINARY.UP ? imageList.Images[imageList.Images.IndexOfKey("up.png")] : imageList.Images[imageList.Images.IndexOfKey("down.png")];
                        if (trade.Win == WIN.INPROGRESS) dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnWin.Index].Value = imageList.Images[imageList.Images.IndexOfKey("pending.png")];
                        if (trade.Win == WIN.YES) dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnWin.Index].Value = imageList.Images[imageList.Images.IndexOfKey("tick.png")];
                        if (trade.Win == WIN.NO) dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnWin.Index].Value = imageList.Images[imageList.Images.IndexOfKey("cross.png")];
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnAmount.Index].Value = trade.Amount;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnDate.Index].Value = trade.Date.ToString("dd/MM/yyy HH:mm:ss");
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnPriceStart.Index].Value = trade.PriceStart;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnPriceEnd.Index].Value = trade.PriceEnd;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnForex.Index].Value = trade.Forex;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColumnAcc.Index].Value = trade.Acc.ToString("F6");
                        if (dataGridView1.Rows.Count > 11) break;
                    }
                }
            }
            catch (System.Exception exp)
            {
                System.Console.WriteLine(exp.Message);
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
        #endregion

        #region Event
        private void _timer_Tick(object sender, System.EventArgs e)
        {
            _timer.Stop();
            UpdateTrades();
            _timer.Start();
        }
        #endregion
    }
}