using System.IO;
using UtilityServices.Dtos;

namespace UtilityServices
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateOrderPdf(OrderDto order)
        {
            IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
            
            var pdf = Renderer.RenderHtmlAsPdf(GetOrderAsHtml(order)).SaveAs("C:\\Users\\zaron\\Desktop\\html-string.pdf");
            var pdfBytes = pdf.BinaryData;
            //File.Delete();
            return pdfBytes;
        }

        private string GetOrderAsHtml(OrderDto order)
        {
            return $@"
                <html>
                    <head>
                        <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@5.1.1/dist/css/bootstrap.min.css' integrity='sha384-F3w7mX95PdgyTmZZMECAngseQB83DfGTowi0iMjiWaeVhAn4FJkqJByhZMI3AhiU' crossorigin='anonymous'>
                    </head>
                    <body>
                        <div class='row'>
                            <div class='col-12'>
                                <table class='table table-bordered' width='100%' cellspacing='0'>
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th>SKU</th>
                                            <th>Descrizione</th>
                                            <th># Pezzi</th>
                                            <th>Prz. Unitario</th>
                                            <th>Prz. Totale</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><img src='C:\\Users\zaron\Pictures\Zeta.jpg' style='width: 40%; height: auto'></td>
                                            <td>{order.SKU}</td>
                                            <td>{order.Description}</td>
                                            <td>&euro; {order.Price_Uni.ToString("0.00")}</td>
                                            <td>&euro; {order.Price_Tot.ToString("0.00")}</td>
                                            <td></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </body>
                    <script src='https://cdn.jsdelivr.net/npm/bootstrap@5.1.1/dist/js/bootstrap.min.js' integrity='sha384-skAcpIdS7UcVUC05LJ9Dxay8AXcDYfBJqt1CJ85S/CFujBsIzCIv+l9liuYLaMQ/' crossorigin='anonymous'></script>
                </html>
            ";
        }
    }
}