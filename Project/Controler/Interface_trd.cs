using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using Tools4Libraries;
using System.Net.Mail;

namespace Droid_trading
{
    /// <summary>
    /// Interface for Tobi Assistant application : take care, some french word here to allow Tobi to speak with natural langage.
    /// </summary>            
    public class Interface_trd : GPInterface
    {
        #region Attributes
        private static Account _account;
        #endregion

        #region Properties
        public Account Account
        {
            get { return _account; }
            set { _account = value; }
        }
        #endregion

        #region Constructor
        public Interface_trd()
        {
            Init();
        }
        #endregion

        #region Action
        public static bool ACTION_130_demarrer_trading()
        {
            try
            {
                if (_account == null)
                {
                    _account = new Account();
                }
                _account.OpenMarkets();
                return true;
            }
            catch (Exception exp)
            {
                throw new Exception("Cannot launch the trading : " + exp.Message);
            }
        }
        public static bool ACTION_131_arreter_trading()
        {
            try
            {
                if (_account == null)
                {
                    _account = new Account();
                }
                _account.CloseMarkets();
                return true;
            }
            catch (Exception exp)
            {
                throw new Exception("Cannot stop the trading : " + exp.Message);
            }
        }
        public static bool ACTION_132_envoyer_rapport()
        {
            try
            {
                string report = _account.GetReport();
                if (_account == null)
                {
                    _account = new Account();
                }
                Droid_communication.Interface_com.ACTION_130_envoyer_mail(
                    "Daily trading",
                    new List<MailAddress>() {
                        new MailAddress("toto@totomail.to") // add this in database project
                    },
                    report
                    );
                return true;
            }
            catch (Exception exp)
            {
                throw new Exception("Cannot send the reporting : " + exp.Message);
            }
        }
        #endregion

        #region Methods Public
        public override bool Open(object fileName)
        {
            return false;
        }
        public override void Print()
        {

        }
        public override void Close()
        {

        }
        public override bool Save()
        {   return false;
        }
        public override void ZoomIn()
        {

        }
        public override void ZoomOut()
        {

        }
        public override void Copy()
        {

        }
        public override void Cut()
        {

        }
        public override void Paste()
        {

        }
        public override void Resize()
        {
            //if (panelTB != null) { panelTB.Refresh(); }
            //if (panelGV != null) { panelGV.Refresh(); }
        }
        //public override void GlobalAction(object sender, EventArgs e)
        //{
        //    ToolBarEventArgs tbea = e as ToolBarEventArgs;
        //    string action = tbea.EventText;
        //    switch (action.ToLower())
        //    {
        //        case "trade":
        //            break;
        //    }
        //}
        #endregion

        #region Methods Launcher
        private void LaunchStartTrading()
        {
            _account.OpenMarkets();
        }
        private void LaunchStopTrading()
        {
            _account.CloseMarkets();
        }
        #endregion

        #region Methods	private
        private void Init()
        {
            _account = new Account();
        }
        #endregion

        #region Event
        #endregion
    }
}
