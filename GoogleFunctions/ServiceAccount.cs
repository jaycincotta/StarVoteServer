using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarVoteServer.GoogleFunctions
{
    /// <summary>
    /// ServiceAccount provides convenient wrappers for Google API methods suitable for
    /// implementing the higher-level actions defined in GoogleService
    /// </summary>
    public class ServiceAccount : ServiceAccountBase
    {
        protected readonly string _spreadsheetId;
        private List<Request> _requests;

        public ServiceAccount(string spreadsheetId)
        {
            _spreadsheetId = spreadsheetId;
        }

        protected void BeginBatch()
        {
            _requests = new List<Request>();
        }

        protected void AddToBatch(Request request)
        {
            _requests.Add(request);
        }

        protected SpreadsheetsResource.BatchUpdateRequest EndBatch()
        {
            var batchRequest = new BatchUpdateSpreadsheetRequest { Requests = _requests };
            var batchUpdateRequest = _service.Spreadsheets.BatchUpdate(batchRequest, _spreadsheetId);
            return batchUpdateRequest;
        }

        protected void AddNewSheet(string title, int? sheetId, params string[] headings)
        {
            var addSheetRequest = new AddSheetRequest
            {
                Properties = new SheetProperties
                {
                    Title = title,
                    SheetId = sheetId,
                    GridProperties = new GridProperties { FrozenRowCount = 1 }
                }
            };

            var row = headings.ToRowData();
            var updateRequest = new UpdateCellsRequest
            {
                Range = new GridRange { SheetId = 1, StartColumnIndex = 0, StartRowIndex = 0, EndColumnIndex = headings.Length, EndRowIndex = 1 },
                Rows = new List<RowData> { row },
                Fields = "userEnteredValue"
            };

            _requests.Add(new Request { AddSheet = addSheetRequest });
            _requests.Add(new Request { UpdateCells = updateRequest });
        }
        protected SpreadsheetsResource.ValuesResource.UpdateRequest BuildUpdateRequest(string range, IList<IList<object>> values)
        {
            ValueRange body = new ValueRange
            {
                Values = values
            };
            SpreadsheetsResource.ValuesResource.UpdateRequest request = _service.Spreadsheets.Values.Update(body, _spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            return request;
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
