using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLog;
using RemaSoftware.Domain.Services.Impl;
using RemaSoftware.UtilityServices.Interface;

namespace RemaSoftware.UtilityServices.Implementation
{
    public class ImageService : IImageService
    {
        private readonly IConfiguration _configuration;
        private readonly OrderImageBlobService _orderBlobService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ImageService(IConfiguration configuration, OrderImageBlobService orderBlobService)
        {
            _configuration = configuration;
            _orderBlobService = orderBlobService;
        }

        public async Task<string> SavingOrderImage(string photo)
        {
            try
            {
                string source = photo;
                string base64 = source.Substring(source.IndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(base64);
                MemoryStream stream = new MemoryStream(data);
                var newFileName = $"{Guid.NewGuid().ToString().Replace("-", String.Empty)}.png";
                await _orderBlobService.UploadFromStreamBlobAsync(stream,
                    $"/order/{newFileName}");
                return newFileName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante il salvataggio dell'immagine sul server");
                throw ex;
            }
        }
    }
}
