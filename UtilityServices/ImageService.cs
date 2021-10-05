using System;
using System.Collections.Generic;
using System.Text;
using NLog;


namespace UtilityServices
{
    public class ImageService : IImageService
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public string SavingOrderImage(string photo, string path)
        {
            try
            {
                string source = photo;
                string base64 = source.Substring(source.IndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(base64);
                var guid = Guid.NewGuid().ToString();

                string file_path = path + "/ImmaginiOrdini/" + guid + ".png";

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
