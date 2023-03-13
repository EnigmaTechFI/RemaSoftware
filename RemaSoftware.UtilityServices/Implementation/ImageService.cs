using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NLog;
using RemaSoftware.UtilityServices.Interface;

namespace RemaSoftware.UtilityServices.Implementation
{
    public class ImageService : IImageService
    {
        private readonly IConfiguration _configuration;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ImageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SavingOrderImage(string photo, string path)
        {
            try
            {
                string source = photo;
                string base64 = source.Substring(source.IndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(base64);
                var guid = Guid.NewGuid().ToString();
                Directory.CreateDirectory(path);
                string file_path = path + guid + ".png";

                System.IO.File.WriteAllBytes(file_path, data);
                return guid + ".png";
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante il salvataggio dell'immagine sul server");
            }
            return "";
        }
    }
}
