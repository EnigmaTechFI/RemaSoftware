using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.PDFViewModel
{
    public class PDFViewModel
    {
        public string QRCode;
        public Ddt_In DdtIn { get; set; }
        public string Photo { get; set; }
        public string BasePathImages { get; set; }
    }
}
