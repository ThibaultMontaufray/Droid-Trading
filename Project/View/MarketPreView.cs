namespace Droid_trading
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Forms.DataVisualization.Charting;

    public partial class MarketPreView : UserControl
    {
        #region Attribute
        private System.ComponentModel.IContainer components = null;

        private const int DELAY = 10;
        private const int MAX_POINT = 1000000;

        private double _lastVal = double.NaN;
        private bool _decrease = false;
        private bool _increase = false;
        private bool _scanning = false;

        private Market _market;
        private System.Windows.Forms.Timer _timer;
        private ImageList imageList;
        private Chart _chartBasket;
        private Panel panel1;
        private Label labelMarket;
        private Label labelPrice;
        private Label labelNbTrade;
        private Label labelRate;
        private Label labelSolde;
        private Button buttonStopStart;
        #endregion

        #region Properties
        public Market Market
        {
            get { return _market; }
            set { _market = value; }
        }
        #endregion

        #region Constructor
        public MarketPreView()
        {
            InitializeComponent();
        }
        ~MarketPreView()
        {
            StopScann();
        }
        #endregion

        #region Methods public
        public void Init(Market market)
        {
            _market = market;
            _chartBasket.Series["BasketCollection"].Color = Color.FromArgb(130, 255, 224, 192);
            labelMarket.Text = _market.Forex.ToString();
            InitTimer();
            _chartBasket.ChartAreas[0].AxisX.Minimum = DateTime.Now.AddMinutes(-15).ToOADate();
            _chartBasket.ChartAreas[0].AxisX.Maximum = DateTime.Now.ToOADate();
            _chartBasket.ChartAreas[0].AxisY.Minimum = 0;
            _chartBasket.ChartAreas[0].AxisY.Maximum = 10;
        }
        public void StartScann()
        {
            _market.Calcul.OpenMarket();
            _timer.Start();
        }
        public void StopScann()
        {
            _timer.Stop();
            _market.Calcul.CloseMarket();
        }
        #endregion

        #region Methods protected        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Methods private
        private void InitTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = DELAY;
            _timer.Tick += _timer_Tick;
        }
        private void Process()
        {
            UpdateSupportResistance();
            UpdateMarketGraph();
            UpdateSummit();
            UpdateTrend();
            UpdateTradeGraph();
            UpdateDetails();
        }
        private void UpdateDetails()
        {
            _market.UpdateAccount();

            int tradeWin = _market.Trades.Count(t => t.Win == WIN.YES); ;
            int tradeCount = _market.Trades.Count(t => t.Win != WIN.INPROGRESS);
            if (_market.Calcul.Prices.Count > 0) labelPrice.Text = _market.Calcul.Prices[_market.Calcul.Prices.Count - 1].Value.ToString();
            labelRate.Text = tradeCount == 0 ? "N/A" : ((tradeWin * 100 / tradeCount).ToString() + " %");
            labelNbTrade.Text = tradeCount.ToString();
            //labelSolde.Text = _market.Calcul.VolatilityImpl.ToString("F6");
            labelSolde.Text = _market.Solde.ToString("F2");
        }
        private void UpdateTradeGraph()
        {
            _chartBasket.Series["TradeDown"].Points.Where(p => p.XValue < _market.Calcul.LastPriceDate.AddMinutes(-20).ToOADate()).ToList().Clear();
            _chartBasket.Series["TradeUp"].Points.Where(p => p.XValue < _market.Calcul.LastPriceDate.AddMinutes(-20).ToOADate()).ToList().Clear();
            while (_market.Calcul.TradeDown.Count > 0)
            {
                var trade = _market.Calcul.TradeDown.Dequeue();
                _chartBasket.Series["TradeDown"].Points.AddXY(trade.Key.ToOADate(), trade.Value);
            }
            while (_market.Calcul.TradeUp.Count > 0)
            {
                var trade = _market.Calcul.TradeUp.Dequeue();
                _chartBasket.Series["TradeUp"].Points.AddXY(trade.Key.ToOADate(), trade.Value);
            }
        }
        private void UpdateSupportResistance()
        {
            try
            {
                if (_market.Calcul.Support != double.NaN) _chartBasket.Series["Support"].Points.AddXY(_market.Calcul.LastPriceDate.ToOADate(), _market.Calcul.Support);
                if (_market.Calcul.Resistance != double.NaN) _chartBasket.Series["Resistance"].Points.AddXY(_market.Calcul.LastPriceDate.ToOADate(), _market.Calcul.Resistance);
            }
            catch (Exception exp)
            {
                Console.WriteLine("support/ resistance graph drawing exception : " + exp.Message);
            }
        }
        private void UpdateSummit()
        {
            Calculation.Price price;
            while (_market.Calcul.SummitQueue.Count > 0)
            {
                price = _market.Calcul.SummitQueue.Dequeue();
                _chartBasket.Series["BasketCollectionSummit"].Points.AddXY(price.Date.ToOADate(), price.Value);
                _chartBasket.Series["BasketCollectionSummit"].Points.Where(p => p.XValue < _market.Calcul.LastPriceDate.AddMinutes(-20).ToOADate()).ToList().Clear();
            }
        }
        private void UpdateTrend()
        {
            Calculation.Price price;
            while (_market.Calcul.SuperTrend.Count > 0)
            {
                price = _market.Calcul.SuperTrend.Dequeue();
                _chartBasket.Series["Trend"].Points.AddXY(price.Date.ToOADate(), price.Value);
                _chartBasket.Series["Trend"].Points.Where(p => p.XValue < _market.Calcul.LastPriceDate.AddMinutes(-20).ToOADate()).ToList().Clear();
            }
        }
        private void UpdateMarketGraph()
        {
            Calculation.Price price;
            while (_market.Calcul.Basket.Count > 0)
            {
                try
                {
                    price = _market.Calcul.Basket.Dequeue();

                    _chartBasket.Series["BasketCollection"].Points.AddXY(price.Date.ToOADate(), price.Value);
                    _chartBasket.Series["BasketCollectionLine"].Points.AddXY(price.Date.ToOADate(), price.Value);
                    _chartBasket.Series["BasketCollection"].Points.Where(p => p.XValue < _market.Calcul.LastPriceDate.AddMinutes(-20).ToOADate()).ToList().Clear();
                    _chartBasket.Series["BasketCollectionLine"].Points.Where(p => p.XValue < _market.Calcul.LastPriceDate.AddMinutes(-20).ToOADate()).ToList().Clear();

                    _chartBasket.ChartAreas[0].AxisX.Minimum = _market.Calcul.LastPriceDate.AddMinutes(-20).ToOADate();
                    _chartBasket.ChartAreas[0].AxisX.Maximum = _market.Calcul.LastPriceDate.ToOADate();

                    double min = _chartBasket.Series["BasketCollection"].Points.FindMinByValue().YValues[0];
                    double max = _chartBasket.Series["BasketCollection"].Points.FindMaxByValue().YValues[0];
                    double marge = (max - min) / 10;
                    if (marge == 0) marge = min;
                    _chartBasket.ChartAreas[0].AxisY.Minimum = min - marge;
                    _chartBasket.ChartAreas[0].AxisY.Maximum = max + marge;

                    _lastVal = price.Value;
                    _decrease = _lastVal > price.Value;
                    _increase = _lastVal < price.Value;

                    _chartBasket.ChartAreas[0].AxisY.Minimum = price.Value - _market.Calcul.VolatilityHisto;
                    _chartBasket.ChartAreas[0].AxisY.Maximum = price.Value + _market.Calcul.VolatilityHisto;
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                }
            }
            foreach (var serie in _chartBasket.Series)
            {
                try
                {
                    if (_market.Calcul.LastPriceDate != DateTime.MinValue) serie.Points.Where(panel1 => panel1.XValue < _market.Calcul.LastPriceDate.AddMinutes(-20).ToOADate()).ToList().Clear();
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                    throw;
                }
            }
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarketPreView));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this._chartBasket = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelSolde = new System.Windows.Forms.Label();
            this.labelNbTrade = new System.Windows.Forms.Label();
            this.labelRate = new System.Windows.Forms.Label();
            this.labelPrice = new System.Windows.Forms.Label();
            this.labelMarket = new System.Windows.Forms.Label();
            this.buttonStopStart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._chartBasket)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "off");
            this.imageList.Images.SetKeyName(1, "on");
            // 
            // _chartBasket
            // 
            this._chartBasket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(73)))), ((int)(((byte)(73)))));
            this._chartBasket.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(73)))), ((int)(((byte)(73)))));
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisX.LabelStyle.Format = "HH:mm";
            chartArea1.AxisX.LineColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisX2.LabelStyle.ForeColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisX2.LabelStyle.Format = "HH:mm";
            chartArea1.AxisX2.LineColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisX2.MajorGrid.LineColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisX2.TitleForeColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisY.LabelStyle.Format = "HH:mm";
            chartArea1.AxisY.LineColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisY2.LabelStyle.ForeColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisY2.LabelStyle.Format = "HH:mm";
            chartArea1.AxisY2.LineColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisY2.MajorGrid.LineColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.AxisY2.TitleForeColor = System.Drawing.Color.DarkGoldenrod;
            chartArea1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(73)))), ((int)(((byte)(73)))));
            chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(73)))), ((int)(((byte)(73)))));
            chartArea1.InnerPlotPosition.Auto = false;
            chartArea1.InnerPlotPosition.Height = 100F;
            chartArea1.InnerPlotPosition.Width = 100F;
            chartArea1.IsSameFontSizeForAllAxes = true;
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 100F;
            chartArea1.Position.Width = 100F;
            this._chartBasket.ChartAreas.Add(chartArea1);
            this._chartBasket.Dock = System.Windows.Forms.DockStyle.Fill;
            this._chartBasket.Location = new System.Drawing.Point(0, 0);
            this._chartBasket.Name = "_chartBasket";
            this._chartBasket.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.EarthTones;
            series1.BackImageWrapMode = System.Windows.Forms.DataVisualization.Charting.ChartImageWrapMode.TileFlipY;
            series1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDot;
            series1.BorderWidth = 0;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            series1.CustomProperties = "LabelStyle=Bottom";
            series1.LabelForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series1.Name = "BasketCollection";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series1.YValuesPerPoint = 6;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series2.CustomProperties = "LabelStyle=Bottom";
            series2.Name = "BasketCollectionLine";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series3.Color = System.Drawing.Color.Yellow;
            series3.CustomProperties = "LabelStyle=Bottom";
            series3.Name = "BasketCollectionSummit";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series3.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series4.Color = System.Drawing.Color.Lime;
            series4.CustomProperties = "LabelStyle=Bottom";
            series4.Name = "TradeUp";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series4.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series5.Color = System.Drawing.Color.Red;
            series5.CustomProperties = "LabelStyle=Bottom";
            series5.Name = "TradeDown";
            series5.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series5.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Color = System.Drawing.Color.Lime;
            series6.CustomProperties = "LabelStyle=Bottom";
            series6.MarkerSize = 1;
            series6.Name = "Support";
            series6.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series6.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Color = System.Drawing.Color.Red;
            series7.CustomProperties = "LabelStyle=Bottom";
            series7.MarkerSize = 1;
            series7.Name = "Resistance";
            series7.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series7.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            series8.CustomProperties = "LabelStyle=Bottom";
            series8.MarkerBorderWidth = 2;
            series8.MarkerStep = 2;
            series8.Name = "Trend";
            series8.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series8.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this._chartBasket.Series.Add(series1);
            this._chartBasket.Series.Add(series2);
            this._chartBasket.Series.Add(series3);
            this._chartBasket.Series.Add(series4);
            this._chartBasket.Series.Add(series5);
            this._chartBasket.Series.Add(series6);
            this._chartBasket.Series.Add(series7);
            this._chartBasket.Series.Add(series8);
            this._chartBasket.Size = new System.Drawing.Size(304, 98);
            this._chartBasket.TabIndex = 3;
            this._chartBasket.Text = "chart1";
            this._chartBasket.DoubleClick += new System.EventHandler(this._chartBasket_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.panel1.Controls.Add(this.labelSolde);
            this.panel1.Controls.Add(this.labelNbTrade);
            this.panel1.Controls.Add(this.labelRate);
            this.panel1.Controls.Add(this.labelPrice);
            this.panel1.Controls.Add(this.labelMarket);
            this.panel1.Controls.Add(this.buttonStopStart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(304, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(69, 98);
            this.panel1.TabIndex = 4;
            // 
            // labelSolde
            // 
            this.labelSolde.BackColor = System.Drawing.Color.Transparent;
            this.labelSolde.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSolde.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSolde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.labelSolde.Location = new System.Drawing.Point(0, 80);
            this.labelSolde.Margin = new System.Windows.Forms.Padding(3);
            this.labelSolde.Name = "labelSolde";
            this.labelSolde.Size = new System.Drawing.Size(69, 16);
            this.labelSolde.TabIndex = 5;
            this.labelSolde.Text = "---";
            this.labelSolde.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelNbTrade
            // 
            this.labelNbTrade.BackColor = System.Drawing.Color.Transparent;
            this.labelNbTrade.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelNbTrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNbTrade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.labelNbTrade.Location = new System.Drawing.Point(0, 64);
            this.labelNbTrade.Margin = new System.Windows.Forms.Padding(3);
            this.labelNbTrade.Name = "labelNbTrade";
            this.labelNbTrade.Size = new System.Drawing.Size(69, 16);
            this.labelNbTrade.TabIndex = 4;
            this.labelNbTrade.Text = "---";
            this.labelNbTrade.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelRate
            // 
            this.labelRate.BackColor = System.Drawing.Color.Transparent;
            this.labelRate.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.labelRate.Location = new System.Drawing.Point(0, 48);
            this.labelRate.Margin = new System.Windows.Forms.Padding(3);
            this.labelRate.Name = "labelRate";
            this.labelRate.Size = new System.Drawing.Size(69, 16);
            this.labelRate.TabIndex = 3;
            this.labelRate.Text = "---";
            this.labelRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelPrice
            // 
            this.labelPrice.BackColor = System.Drawing.Color.Transparent;
            this.labelPrice.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.labelPrice.Location = new System.Drawing.Point(0, 32);
            this.labelPrice.Margin = new System.Windows.Forms.Padding(3);
            this.labelPrice.Name = "labelPrice";
            this.labelPrice.Size = new System.Drawing.Size(69, 16);
            this.labelPrice.TabIndex = 2;
            this.labelPrice.Text = "---";
            this.labelPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMarket
            // 
            this.labelMarket.BackColor = System.Drawing.Color.Transparent;
            this.labelMarket.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelMarket.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMarket.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.labelMarket.Location = new System.Drawing.Point(0, 16);
            this.labelMarket.Margin = new System.Windows.Forms.Padding(3);
            this.labelMarket.Name = "labelMarket";
            this.labelMarket.Size = new System.Drawing.Size(69, 16);
            this.labelMarket.TabIndex = 1;
            this.labelMarket.Text = "---";
            this.labelMarket.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonStopStart
            // 
            this.buttonStopStart.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonStopStart.BackgroundImage")));
            this.buttonStopStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonStopStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonStopStart.FlatAppearance.BorderSize = 0;
            this.buttonStopStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStopStart.Location = new System.Drawing.Point(0, 0);
            this.buttonStopStart.Name = "buttonStopStart";
            this.buttonStopStart.Size = new System.Drawing.Size(69, 16);
            this.buttonStopStart.TabIndex = 0;
            this.buttonStopStart.UseVisualStyleBackColor = true;
            this.buttonStopStart.Click += new System.EventHandler(this.buttonStopStart_Click);
            // 
            // MarketPreView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this._chartBasket);
            this.Controls.Add(this.panel1);
            this.Name = "MarketPreView";
            this.Size = new System.Drawing.Size(373, 98);
            ((System.ComponentModel.ISupportInitialize)(this._chartBasket)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Event
        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            Process();
            _timer.Start();
        }
        private void buttonStopStart_Click(object sender, EventArgs e)
        {
            _scanning = !_scanning;
            if (_scanning)
            {
                buttonStopStart.BackgroundImage = imageList.Images[imageList.Images.IndexOfKey("on")];
                StartScann();
            }
            else
            {
                buttonStopStart.BackgroundImage = imageList.Images[imageList.Images.IndexOfKey("off")];
                StopScann();
            }
        }
        private void _chartBasket_DoubleClick(object sender, EventArgs e)
        {
            Chart chart = sender as Chart;
            _chartBasket.DoubleClick -= new System.EventHandler(this._chartBasket_DoubleClick);
            this.Controls.Remove(chart);
            Form f = new Form();
            f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            f.StartPosition = FormStartPosition.Manual;
            f.Width = 388;
            f.Height = 283;
            f.Top = this.Parent.Top + 73;
            f.Left = this.Parent.Left + 392;
            f.Controls.Add(chart);
            chart.Dock = DockStyle.Fill;
            f.ShowDialog();
            f.Controls.Remove(chart);
            this.Controls.Add(chart);
            _chartBasket.DoubleClick += new System.EventHandler(this._chartBasket_DoubleClick);
        }
        #endregion
    }
}
