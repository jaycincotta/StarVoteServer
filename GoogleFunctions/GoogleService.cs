using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarVoteServer.GoogleFunctions
{
    public class GoogleService : ServiceAccount
    {
        public GoogleService(string spreadsheetId) : base(spreadsheetId) { }


        public async Task<string> ReadRange(string range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);
            //request.ValueRenderOption = valueRenderOption;

            var response = await request.ExecuteAsync().ConfigureAwait(false);
            return JsonConvert.SerializeObject(response.Values);
        }

        public async Task<string> WriteRange(string range, IList<IList<object>> values)
        {
            var request = BuildUpdateRequest(range, values);
            var response = await request.ExecuteAsync().ConfigureAwait(false);
            return JsonConvert.SerializeObject(response.UpdatedCells);
        }

        public async Task<GoogleSheetInfo> GetSheetInfo()
        {
            var request = _service.Spreadsheets.Get(_spreadsheetId);
            var response = await request.ExecuteAsync().ConfigureAwait(false);
            var info = new GoogleSheetInfo
            {
                Title = response.Properties.Title,
                TimeZone = response.Properties.TimeZone
            };
            foreach (var sheet in response.Sheets)
            {
                var props = sheet.Properties;
                info.Sheets.Add(new SheetInfo { Index = props.Index.GetValueOrDefault(-1), SheetId = props.SheetId.GetValueOrDefault(-1), Title = props.Title });
            }
            return info;
        }


        public async Task<string> CreateSheet()
        {
            /*
            var addSheetRequest = new AddSheetRequest
            {
                Properties = new SheetProperties
                {
                    Title = "Voters",
                    SheetId = 1,
                    GridProperties = new GridProperties { FrozenRowCount = 1}
                }
            };

            var row = new string[] { "VoterId", "Timestamp", "Email" }.ToRowData();


            var updateRequest = new UpdateCellsRequest
            {
                Range = new GridRange { SheetId = 1, StartColumnIndex = 0, StartRowIndex = 0, EndColumnIndex = 3, EndRowIndex = 1 },
                Rows = new List<RowData> { row },
                Fields = "userEnteredValue"
            };
            */

            BeginBatch();
            AddNewSheet("\u2605Voters", 1, "VoterId", "Timestamp", "Email");

            var formatRequest = new RepeatCellRequest
            {
                Range = new GridRange { SheetId = 1, StartColumnIndex = 1, EndColumnIndex = 2, StartRowIndex = 0 },
                Cell = new CellData { UserEnteredFormat = new CellFormat { NumberFormat = new NumberFormat { Type = "DATE", Pattern = "M/d/yy hh:mm" } } },
                Fields = "userEnteredFormat.numberFormat"
            };

            AddToBatch(new Request { RepeatCell = formatRequest });
            var batch = EndBatch();
            var response = await batch.ExecuteAsync().ConfigureAwait(false);
            return JsonConvert.SerializeObject(response);
            /*
            var batchRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
                {
                    new Request { AddSheet = addSheetRequest },
                    new Request { RepeatCell = formatRequest },
                    new Request { UpdateCells = updateRequest },
                }
            };

            var batchUpdateRequest = _service.Spreadsheets.BatchUpdate(batchRequest, _spreadsheetId);

            var response = await batchUpdateRequest.ExecuteAsync().ConfigureAwait(false);
            return JsonConvert.SerializeObject(response);
            */
        }

        public async Task<string> Initialize(ElectionSettings settings)
        {
            var clearDataRequest = new UpdateCellsRequest { Fields = "*", Range = new GridRange { SheetId = 0 } };

            var renameRequest = new UpdateSheetPropertiesRequest
            {
                Properties = new SheetProperties { Title = "\u2605Settings" },
                Fields = "title"
            };

            var updateRequest = new UpdateCellsRequest
            {
                Range = new GridRange { SheetId = 0, StartColumnIndex = 0, StartRowIndex = 0, EndColumnIndex = 3, EndRowIndex = 100 },
                Rows = settings.ToRowData(),
                Fields = "userEnteredValue"
            };

            var batchRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
                {
                    new Request { UpdateCells = clearDataRequest },
                    new Request { UpdateSheetProperties = renameRequest },
                    new Request { UpdateCells = updateRequest },
                }
            };

            var batchUpdateRequest = _service.Spreadsheets.BatchUpdate(batchRequest, _spreadsheetId);

            var response = await batchUpdateRequest.ExecuteAsync().ConfigureAwait(false);
            return JsonConvert.SerializeObject(response);
        }

    }
}
