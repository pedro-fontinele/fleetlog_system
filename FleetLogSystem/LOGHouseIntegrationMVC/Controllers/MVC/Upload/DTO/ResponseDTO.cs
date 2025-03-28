using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Controllers
{
    public class ResponseDTO
    {

        public ResponseDTO(bool success, string? message, NfeProc nFeProc = null, int receiptNoteId = 0)
        {
            Success = success;
            Message = message;
            NFeProc = nFeProc;
            ReceiptNoteId = receiptNoteId;
        }


        public bool Success { get; set; }
        public string? Message { get; set; }
        public NfeProc NFeProc { get; set; }
        public int ReceiptNoteId { get; set; }

    }
}
