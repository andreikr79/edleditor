using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

namespace edleditor
{
    public class WTVRecordsList : VirtualList
    {
        private Dictionary<int, string> _items = new Dictionary<int, string>();

        public Dictionary<int, string> Items
        {
            get
            {
                return this._items;
            }
            set
            {
                this._items = value;
            }
        }

        public WTVRecordsList()
        {
            int _count = 0;
            // get all files from tvrecordspath                        
            foreach (string wtvfile in Directory.GetFiles(Settings.TVRecordsPath, "*.wtv"))
            {                
                this.Items.Add(_count, wtvfile);
                _count++;
            }
            foreach (string wtvfile in Directory.GetFiles(Settings.TVRecordsPath, "*.dvr-ms"))
            {
                this.Items.Add(_count, wtvfile);
                _count++;
            }
            Count = _count;
        }
        protected override void OnRequestItem(int index, ItemRequestCallback callback)
        {
            string wtv="";
            if (this.Items.ContainsKey(index))
            {
                wtv = this.Items[index];
            }
            callback(this, index, wtv);
        }
    }
}
