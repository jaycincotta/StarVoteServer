using Google.Apis.Sheets.v4.Data;
using Microsoft.VisualBasic;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace StarVoteServer
{
    public static class ExtensionMethods
    {
        const string dateFormat = "M/d/yy H:mm";

        static CellData GetCellData(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "".ToCellData();
            }
            return value.ToCellData();
        }
       static  CellData GetCellData(bool? value)
       {
            if (!value.HasValue) {
                return "".ToCellData();
            }
            return value.Value.ToCellData();
       }

        static CellData GetCellData(DateTime? value)
        {
            return value.ToCellData();

        }

        public static IList<RowData> ToRowData(this ElectionSettings settings)
        {
            var rows = new List<RowData> {
                CreateRow("StartTime".ToCellData(), GetCellData(settings.StartTime)),
                CreateRow("EndTime".ToCellData(), GetCellData(settings.EndTime)),
                CreateRow("FaqUrl".ToCellData(), GetCellData(settings.InfoUrl)),
                CreateRow("AdminEmail".ToCellData(), GetCellData(settings.AdminEmail)),
                CreateRow("SupportEmail".ToCellData(), GetCellData(settings.SupportEmail)),
                CreateRow("AuditEmail".ToCellData(), GetCellData(settings.AuditEmail)),
                CreateRow("EmailVerification".ToCellData(), GetCellData(settings.EmailVerification)),
                CreateRow("VoterAuthorization".ToCellData(), GetCellData(settings.VoterAuthorization)),
                CreateRow("BallotUpdates".ToCellData(), GetCellData(settings.BallotUpdates)),
                CreateRow("PublicResults".ToCellData(), GetCellData(settings.PublicResults)),
            };

            return rows;
        }

        public static RowData ToRowData(this string[] array)
        {
            var cells = new List<CellData>();
            foreach (var element in array) {
                cells.Add(element.ToCellData());
            }
            var rowData = new RowData { Values = cells };
            return rowData;
        }

        public static T[] Slice<T>(this T[] source, int start, int? end = null)
        {
            var endIndex = end.HasValue ? end.Value : source.Length;

            // Handles negative ends.
            if (endIndex < 0)
            {
                endIndex = source.Length + endIndex;
            }
            int len = endIndex - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }

        private static RowData CreateRow( params CellData[] cells)
        {
            return new RowData { Values = cells };
        }
        private static CellData ToCellData(this string value)
        {
            return new CellData { UserEnteredValue = value.ToExtendedValue() };
        }

        private static CellData ToCellData(this DateTime? value)
        {
            return value.HasValue ? value.Value.ToCellData() : "".ToCellData();
        }

        private static CellData ToCellData(this DateTime value)
        {
            var googleDate = (int) value.Subtract(new DateTime(1899, 12, 30, 0, 0, 0)).TotalDays;
            var hour = value.Hour;
            var minute = value.Minute;
            var googleTime = (minute + hour*60) / 1440.0;
            var timestamp = googleDate + googleTime;

            return new CellData
            {
                UserEnteredFormat = new CellFormat
                {
                    NumberFormat = new NumberFormat
                    {
                        Type = "DATE",
                        Pattern = dateFormat
                    },
                },
                UserEnteredValue = new ExtendedValue { NumberValue = timestamp }
            };
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
