using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;

namespace edleditor
{
    public class SegmentData : ModelItem
    {
        private TimeSpan _startposition;
        private TimeSpan _endposition;
        private int _edlaction;

        public SegmentData()
        {
            _startposition = new TimeSpan(0);
            _endposition = new TimeSpan(0);
            _edlaction = 0;
        }

        public TimeSpan StartPosition
        {
            get
            {
                return _startposition;
            }
            set
            {
                _startposition = value;
                FirePropertyChanged("StartPosition");
                FirePropertyChanged("strStartPosition");
            }
        }

        public TimeSpan EndPosition
        {
            get
            {
                return _endposition;
            }
            set
            {
                _endposition = value;
                FirePropertyChanged("EndPosition");
                FirePropertyChanged("strEndPosition");
            }
        }

        public string strStartPosition
        {
            get
            {                
                int _milliseconds = _startposition.Milliseconds / 10;
                return _startposition.Hours.ToString("00") + ":" + _startposition.Minutes.ToString("00") + ":" + _startposition.Seconds.ToString("00") + "." + _milliseconds.ToString("00");
            }
            set
            {
                try
                {
                    string _str = value;
                    if (_str.Contains(":"))
                    {
                        _startposition = TimeSpan.Parse(_str);
                    }
                    else
                    {
                        double parseseconds = double.Parse(_str, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                        _startposition = TimeSpan.FromSeconds(parseseconds);
                    }
                }
                catch
                {
                    _startposition = TimeSpan.Zero;
                }
                FirePropertyChanged("strStartPosition");
                FirePropertyChanged("StartPosition");
            }
        }

        public string strEndPosition
        {
            get
            {
                int _milliseconds = _endposition.Milliseconds / 10;
                return _endposition.Hours.ToString("00") + ":" + _endposition.Minutes.ToString("00") + ":" + _endposition.Seconds.ToString("00") + "." + _milliseconds.ToString("00");
            }
            set
            {
                try
                {
                    string _str = value;
                    if (_str.Contains(":"))
                    {
                        _endposition = TimeSpan.Parse(_str);
                    }
                    else
                    {
                        double parseseconds = double.Parse(_str, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                        _endposition = TimeSpan.FromSeconds(parseseconds);
                    }
                }
                catch
                {
                    _endposition = TimeSpan.Zero;
                }
                FirePropertyChanged("strEndPosition");
                FirePropertyChanged("EndPosition");
            }
        }

        public int EDLAction
        {
            get
            {
                return _edlaction;
            }
            set
            {
                _edlaction = value;
                FirePropertyChanged("EDLAction");
            }
        }
    }
}
