using Google.Apis.Sheets.v4.Data;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarVoteServer
{
    public static class ExtensionMethods
    {
        const string dateFormat = "MM/d/yy H:mm";

        public static IList<IList<object>> ToList(this ElectionSettings settings)
        {
            var list = new List<IList<object>>
            {
                new List<object> { "Election", settings.Election },
                new List<object> { "StartTime", settings.StartTime.ToString(dateFormat) },
                new List<object> { "EndTime", settings.EndTime.ToString(dateFormat) },
                new List<object> { "FaqUrl", settings.FaqUrl },
                new List<object> { "SupportEmail", settings.SupportEmail },
                new List<object> { "AuditEmail", settings.AuditEmail },
                new List<object> { "PrivateResults", settings.PrivateResults },
                new List<object> { "AnonymousVoters", settings.AnonymousVoters },
                new List<object> { "VerifiedVoters", settings.VerifiedVoters },
                new List<object> { "BallotUpdates", settings.BallotUpdates }
            };
            return list;
        }

        public static IList<RowData> ToRowData(this ElectionSettings settings)
        {
            var rows = new List<RowData> {
                CreateRow("Election".ToCellData(), settings.Election.ToCellData()),
                CreateRow("StartTime".ToCellData(), settings.StartTime.ToCellData()),
                CreateRow("EndTime".ToCellData(), settings.EndTime.ToCellData()),
                CreateRow("FaqUrl".ToCellData(), settings.FaqUrl.ToCellData()),
                CreateRow("SupportEmail".ToCellData(), settings.SupportEmail.ToCellData()),
                CreateRow("AuditEmail".ToCellData(), settings.AuditEmail.ToCellData()),
                CreateRow("PrivateResults".ToCellData(), settings.PrivateResults.ToCellData()),
                CreateRow("AnonymousVoters".ToCellData(), settings.AnonymousVoters.ToCellData()),
                CreateRow("VerifiedVoters".ToCellData(), settings.VerifiedVoters.ToCellData()),
                CreateRow("BallotUpdates".ToCellData(), settings.BallotUpdates.ToCellData()),
            };

            return rows;
        }

        private static RowData CreateRow( params CellData[] cells)
        {
            return new RowData { Values = cells };
        }
        private static CellData ToCellData(this string value)
        {
            return new CellData { UserEnteredValue = value.ToExtendedValue() };
        }

        private static CellData ToCellData(this DateTime value)
        {
            return new CellData { UserEnteredValue = value.ToString(dateFormat).ToExtendedValue() };
        }

        private static CellData ToCellData(this bool value)
        {
            return new CellData { UserEnteredValue = value.ToExtendedValue() };
        }

        private static ExtendedValue ToExtendedValue(this string value)
        {
            return new ExtendedValue { StringValue = value };
        }

        private static ExtendedValue ToExtendedValue(this bool value)
        {
            return new ExtendedValue { BoolValue = value };
        }

    }
}
