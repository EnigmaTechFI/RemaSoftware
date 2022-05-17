using UtilityServices.Dtos;

namespace UtilityServices
{
    public interface IPdfService
    {
        byte[] GeneratePdf(string pdfAsHtml);
    }
}