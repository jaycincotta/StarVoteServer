using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarVoteServer.GoogleFunctions
{
    public class ServiceAccount : ServiceAccountBase
    {
        private readonly string _spreadsheetId;
        public ServiceAccount(string spreadsheetId)
        {
            _spreadsheetId = spreadsheetId;
        }


        private SpreadsheetsResource.ValuesResource.UpdateRequest UpdateRequest(string range, IList<IList<object>> values)
        {
            ValueRange body = new ValueRange
            {
                Values = values
            };
            SpreadsheetsResource.ValuesResource.UpdateRequest request = _service.Spreadsheets.Values.Update(body, _spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            return request;
        }

        public async Task<string> ReadRange(string range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);
            //request.ValueRenderOption = valueRenderOption;

            var response = await request.ExecuteAsync().ConfigureAwait(false);
            return JsonConvert.SerializeObject(response.Values);
        }

        public async Task<string> WriteRange(string range, IList<IList<object>> values)
        {
            var request = UpdateRequest(range, values);
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

        public async Task<string> Initialize(ElectionSettings settings)
        {
            var clearDataRequest = new UpdateCellsRequest { Fields = "*", Range = new GridRange { SheetId = 0 } };

            var renameRequest = new UpdateSheetPropertiesRequest
            {
                Properties = new SheetProperties { Title = "\u2605" },
                Fields = "title"
            };

            var updateRequest = new Google.Apis.Sheets.v4.Data.UpdateCellsRequest();
            updateRequest.Range = new GridRange { SheetId = 0, StartColumnIndex = 0, StartRowIndex = 0, EndColumnIndex = 3, EndRowIndex = 100 };
            updateRequest.Rows = settings.ToRowData();
            updateRequest.Fields = "userEnteredValue";

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

        /*
        public async Task<string> CreateSettings(string spreadsheetId, string sheetName, ElectionSettings settings)
        {
            ValueRange body = new ValueRange
            {
                Values = values
            };

            var addSheetRequest = new AddSheetRequest
            {
                Properties = new SheetProperties
                {
                    Title = sheetName
                }
            };
            addSheetRequest.Properties.s

            var updateCellsRequest = new UpdateCellsRequest
            {
                Start = new GridCoordinate().
                Rows = settings.ToRowData()
            };

            var batchRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
            {
                new Request { AddSheet = addSheetRequest },
                new Request { UpdateCells = updateCellsRequest }
            }
            };

            var batchUpdateRequest =
                _service.Spreadsheets.BatchUpdate(batchRequest, spreadsheetId);

            var response = await batchUpdateRequest.ExecuteAsync().ConfigureAwait(false);
            return JsonConvert.SerializeObject(response.Replies);
        }
        */
    }
}
