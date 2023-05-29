using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.PDFViewModel
{
    public class PDFViewModel
    {
        public string QRCode;
        public string Photo { get; set; }
        public string BasePathImages { get; set; }
        public SubBatch SubBatch { get; set; }
    }
}
