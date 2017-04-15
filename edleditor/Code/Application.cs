using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;

namespace edleditor
{

    public class Application : ModelItem
    {
        private AddInHost host;
        private HistoryOrientedPageSession session;

        private ArrayListDataSet _segmentslist = new ArrayListDataSet();

        private object _selecteditem;
        private bool _listchanged = false;

        public bool ListChanged
        {
            get
            {
                return _listchanged;
            }
        }

        public string strPlayPosition
        {
            get
            {
                TimeSpan _playpos = host.MediaCenterEnvironment.MediaExperience.Transport.Position;
                int _milliseconds = _playpos.Milliseconds / 10;
                return _playpos.Hours.ToString("00")+":"+_playpos.Minutes.ToString("00")+":"+_playpos.Seconds.ToString("00")+"."+_milliseconds.ToString("00");
            }
        }

        public string strPlayDuration
        {
            get
            {
                string _playdur = (string)host.MediaCenterEnvironment.MediaExperience.MediaMetadata["Duration"];
                return _playdur.Substring(0, 11);
            }
        }

        public object SelectedItem
        {
            get
            {
                return _selecteditem;
            }
            set
            {
                _selecteditem = value;
                FirePropertyChanged("SelectedItem");
            }
        }

        public ArrayListDataSet SegmentsList
        {
            get
            {
                return _segmentslist;
            }
            set
            {
                _segmentslist = value;
            }
        }

        public TimeSpan PlayDuration
        {
            get
            {
                TimeSpan _playduration;
                try
                {
                    _playduration = TimeSpan.Parse((string)host.MediaCenterEnvironment.MediaExperience.MediaMetadata["Duration"]);
                }
                catch
                {
                    _playduration = new TimeSpan(0);
                }
                return _playduration;
            }
        }

        public Application()
            : this(null, null)
        {
        }

        public Application(HistoryOrientedPageSession session, AddInHost host)
        {
            this.session = session;
            this.host = host;
        }

        public MediaCenterEnvironment MediaCenterEnvironment
        {
            get
            {
                if (host == null) return null;
                return host.MediaCenterEnvironment;
            }
        }

        public void GoToMenu()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            //properties.Add("WTVRecordsList", new WTVRecordsList());

            if (session != null)
            {
                session.GoToPage("resx://edleditor/edleditor.Resources/Menu", properties);
            }
            else
            {
                Debug.WriteLine("GoToMenu");
            }
        }

        public void AddNewSegment()
        {
            SegmentData _cutsegment = new SegmentData();
            _segmentslist.Add((object)_cutsegment);
            _listchanged = true;
        }

        public void ClearSegments()
        {
            if (_segmentslist.Count > 0)
            {
                if (Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog("Are you shure you want to clear cut list?", "Cut List not empty", (DialogButtons)12, 0, true) == DialogResult.Yes)
                {
                    _segmentslist.Clear();
                    _listchanged = true;
                }
            }
        }

        public void CancelEditing()
        {
            if ((_segmentslist.Count > 0) && (ListChanged))
            {
                if (Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog("Are you shure you want to cancel?", "Cut List not empty", (DialogButtons)12, 0, true) == DialogResult.Yes)
                {
                    if (session != null)
                    {
                        session.BackPage();
                    }
                }
            }
            else
            {
                if (session != null)
                {
                    session.BackPage();
                }
            }
        }

        public bool LoadEDLFile(string VideoPath)
        {
            // try to found edl file
            try
            {
                string EDLFileName = Path.ChangeExtension(VideoPath, ".edl");
                if (File.Exists(EDLFileName))
                {
                    if (Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog("For this video EDL file found. Do you want to load it?", "Found EDL file", (DialogButtons)12, 0, true) == DialogResult.Yes)
                    {
                        // try to load file
                        using (StreamReader sr = new StreamReader(EDLFileName))
                        {
                            String line;
                            char[] charSeparator = new char[] { ' ' };
                            _segmentslist.Clear();
                            while ((line = sr.ReadLine()) != null)
                            {
                                // parse edl line from file                            
                                string[] parseline = line.Split(charSeparator, 3, StringSplitOptions.RemoveEmptyEntries);
                                //try convert to timespan
                                SegmentData _cutsegment = new SegmentData();
                                _cutsegment.strStartPosition = parseline[0];
                                _cutsegment.strEndPosition = parseline[1];
                                _cutsegment.EDLAction = 0;
                                _segmentslist.Add((object)_cutsegment);
                            }
                        }
                        return true;
                    }                    
                }
            }
            catch
            {
            }
            return false;
        }

        public void SaveEDLFile()
        {
            string EDLFileName = Path.ChangeExtension(SelectedItem.ToString(), ".edl");
            if (File.Exists(EDLFileName))
            {
                if (Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog("For this video EDL file found. Do you want to replace it?", "Found EDL file", (DialogButtons)6, 0, true) == DialogResult.Cancel)
                {
                    return;
                }
            }
            try
            {
                using (StreamWriter sw = new StreamWriter(EDLFileName))
                {
                    string edlline;
                    foreach (SegmentData _segment in _segmentslist)
                    {                        
                        edlline = _segment.StartPosition.TotalSeconds.ToString("0.00", CultureInfo.InvariantCulture)+" "+_segment.EndPosition.TotalSeconds.ToString("0.00",CultureInfo.InvariantCulture)+" "+_segment.EDLAction.ToString();
                        sw.WriteLine(edlline);
                    }
                }
                _segmentslist.Clear();
                _listchanged = false;
                if (session != null)
                {
                    session.BackPage();
                }
            }
            catch
            {
            }
        }

        public void FileSelected()
        {
            _listchanged = false;
            _segmentslist.Clear();
            LoadEDLFile(SelectedItem.ToString());
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["VideoPath"] = SelectedItem.ToString();
            host.MediaCenterEnvironment.PlayMedia(MediaType.Video, SelectedItem, false);            
            session.GoToPage("resx://edleditor/edleditor.Resources/VideoEditor", properties);
        }

        public void ChangePosition(long changepos, string timeprefix)
        {
            TimeSpan changetime;
            switch (timeprefix)
            {                
                case "t":
                    changetime = TimeSpan.FromTicks(changepos);
                    break;
                case "m":
                    changetime = TimeSpan.FromMinutes((double)changepos);
                    break;
                case "ms":
                    changetime = TimeSpan.FromMilliseconds((double)changepos);
                    break;
                case "s":
                    changetime = TimeSpan.FromSeconds((double)changepos);
                    break;
                case "h":
                    changetime = TimeSpan.FromHours((double)changepos);
                    break;
                default:
                    changetime = TimeSpan.FromTicks(changepos);
                    break;
            }
            try
            {
                host.MediaCenterEnvironment.MediaExperience.Transport.Position += changetime;
            }
            catch
            {
            }
        }

        public void MoveUpCutListItem(int indx)
        {
            if (indx > 0)
            {
                _segmentslist.Move(indx, indx - 1);
            }
        }

        public void MoveDownCutListItem(int indx)
        {
            if (indx < _segmentslist.Count - 1)
            {
                _segmentslist.Move(indx, indx + 1);
            }
        }

        public void DeleteCutListItem(int indx)
        {
            _segmentslist.RemoveAt(indx);
            _listchanged = true;
        }
    }
}