using System.IO;
using PdfSharp;

namespace UtilityServices
{
    public class PdfService : IPdfService
    {
        public byte[] GeneratePdf(string pdfAsHtml)
        {
            var pdfDoc = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(pdfAsHtml, PageSize.A4);

            byte[] bytes = null;
            using (MemoryStream stream = new MemoryStream())
            {
                pdfDoc.Save(stream, true);
                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}