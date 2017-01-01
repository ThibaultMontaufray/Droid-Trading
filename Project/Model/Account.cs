
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Droid_trading
{
    public delegate void AccountEventHandler(object obj);
    public class Account
    {
        #region Attribute
        public event AccountEventHandler Changed;

        private double _currentSolde;
        private double _initSolde;
        private List<Market> _markets;
        #endregion

        #region Properties
        public double InitSolde
        {
            get { return _initSolde; }
            set { _initSolde = value; }
        }
        public double CurrentSolde
        {
            get { return _currentSolde; }
            set { _currentSolde = value; }
        }
        public List<Market> Markets
        {
            get { return _markets; }
            set { _markets = value; }
        }
        #endregion

        #region Constructor
        public Account()
        {
            _initSolde = 300;
            _markets = new List<Market>();
            _markets.Add(new Market(FOREX.EURUSD, this));
            _markets.Add(new Market(FOREX.GBPUSD, this));
            _markets.Add(new Market(FOREX.NZDUSD, this));
            _markets.Add(new Market(FOREX.USDCHF, this));
            _markets.Add(new Market(FOREX.USDCAD, this));
            _markets.Add(new Market(FOREX.USDJPY, this));
            _markets.Add(new Market(FOREX.AUDCHF, this));
            _markets.Add(new Market(FOREX.AUDJPY, this));
            _markets.Add(new Market(FOREX.AUDUSD, this));
            UpdateAccount();
        }
        #endregion

        #region Methods public
        public void UpdateAccount()
        {
            double solde = _initSolde;
            foreach (Market m in _markets)
            {
                foreach (Trade trade in m.Trades.OrderBy(t => t.Date))
                {
                    if (trade.Win == WIN.NO)
                    {
                        solde -= trade.Amount;
                    }
                    else if (trade.Win == WIN.YES)
                    {
                        solde += trade.Amount * trade.Rate;
                    }
                }
            }
            _currentSolde = solde;
            if (Changed != null) Changed(solde);
        }
        public void OpenMarkets()
        {
            foreach (Market market in _markets)
            {
                market.Calcul.OpenMarket();
            }
        }
        public void CloseMarkets()
        {
            foreach (Market market in _markets)
            {
                market.Calcul.CloseMarket();
            }
        }
        public string GetReport()
        {
            string soldeTrade;
            List<string[]> table;
            StringBuilder report = new StringBuilder();
            DateTime date = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day);

            report.AppendLine("Bonjour,");
            report.AppendLine("");
            report.AppendLine("Voici le rapport de trading de la dernière heure passé.");

            report.AppendLine("");
            report.AppendLine(string.Format("Solde courrant : {0}", _currentSolde));

            foreach (Market market in _markets)
            {
                table = Droid_database.MySqlAdapter.ExecuteReader(string.Format("select marche, position, taux, montant, status from t_trade where date > '{0}' and marche = '{1}'", date, market.Forex));
                if (table.Count > 0)
                {
                    report.AppendLine("<table>");
                    report.AppendLine("<tr><th>Marché</th><th>Position prices</th><th>Taux</th><th>Solde</th></tr>");
                    foreach (string[] row in table)
                    {
                        soldeTrade = row[3].ToString().ToUpper().Equals("WIN") ? ((double.Parse(row[3]) * double.Parse(row[2])) + double.Parse(row[3])).ToString() : "-" + row[3].ToString();
                        report.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", row[0], row[1], row[2], soldeTrade));
                    }
                    report.AppendLine("</table>");
                    report.AppendLine("");
                }
            }

            report.AppendLine("");
            report.AppendLine("Cordialement,");
            report.AppendLine("");
            report.AppendLine("______________");
            report.AppendLine("TOBI Assistant");
            return report.ToString();
        }
        #endregion

        #region Methods private
        #endregion
    }
}
