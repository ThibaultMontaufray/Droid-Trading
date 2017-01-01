using System;

namespace Droid_trading
{
    public class ExclusionZone
    {
        #region Attribute
        private double _minValZone;
        private double _maxValZone;
        private double _minTradeZone;
        private double _maxTradeZone;
        private DateTime _lastUsed;
        private double _value;
        private double _variationMinimum;
        private double _variationMaximum;
        #endregion

        #region Properties
        public double VariationMaximum
        {
            get { return _variationMaximum; }
            set { _variationMaximum = value; }
        }
        public double VariationMinimum
        {
            get { return _variationMinimum; }
            set { _variationMinimum = value; }
        }
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public double MaxTradeZone
        {
            get { return _maxTradeZone; }
            set { _maxTradeZone = value; }
        }
        public double MinTradeZone
        {
            get { return _minTradeZone; }
            set { _minTradeZone = value; }
        }
        public double MaxValZone
        {
            get { return _maxValZone; }
            set { _maxValZone = value; }
        }
        public double MinValZone
        {
            get { return _minValZone; }
            set { _minValZone = value; }
        }
        public DateTime LastUsed
        {
            get { return _lastUsed; }
            set { _lastUsed = value; }
        }
        #endregion

        #region Constructor
        public ExclusionZone(double min, double max, double varMin, double varMax)
        {
            _minValZone = min;
            _maxValZone = max;
            _variationMinimum = varMin;
            _variationMaximum = varMax;
            _minTradeZone = _minValZone - _variationMaximum;
            _maxTradeZone = _maxValZone + _variationMaximum;
            _value = _minValZone + ((_maxValZone - _minValZone) / 2);
            _lastUsed = DateTime.Now;
        }
        public ExclusionZone(ExclusionZone ez)
        {
            _minValZone = ez.MinValZone;
            _maxValZone = ez.MaxValZone;
            _variationMinimum = ez.VariationMinimum;
            _variationMaximum = ez.VariationMaximum;
            _minTradeZone = _minValZone - (_maxValZone - _minValZone);
            _maxTradeZone = _maxValZone + (_maxValZone - _minValZone);
            _value = _minValZone + ((_maxValZone - _minValZone) / 2);
            _lastUsed = DateTime.Now;
        }
        #endregion

        #region Methods public
        public bool IsInZone(double value)
        {
            bool ret = (value < _maxValZone && value > _minValZone);
            if (!ret) Console.WriteLine(string.Format("Out of the zone {0} <= {1} or {0} >= {2}", value, _minValZone, _maxValZone));
            else Console.WriteLine(string.Format("In the navy :) {0} < {1} < {2}", _minValZone, value, _maxValZone));
            return ret;
        }
        public double? GetCloserValue(double value)
        {
            if (value <= _minValZone) return _minValZone;
            if (value >= _maxValZone) return _maxValZone;
            if (_maxValZone - value < value - _minValZone) return _maxValZone;
            if (_maxValZone - value > value - _minValZone) return _minValZone;
            return null;
        }
        #endregion
    }
}