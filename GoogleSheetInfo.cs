using System;
using System.Collections.Generic;
using System.Text;

namespace StarVoteServer
{
    public class GoogleSheetInfo
    {
        public string Title { get; set; }
        public string TimeZone { get; set; }
        public List<SheetInfo> Sheets { get; } = new List<SheetInfo>();

        // Test if this is a fresh Google Sheets document
        public bool IsNewDocument()
        {
            if (Sheets.Count != 1) return false;
            if (Sheets[0].SheetId != 0) return false;
            return true;
        }
    }

    public class SheetInfo
    {
        public int SheetId { get; set; }
        public string Title { get; set; }
        public int Index { get; set; }
    }
}
