//22.06.2016
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace Anvil
{
    public class Web
    {
        public static Image LoadImage(string URL)
        {
            WebRequest request = WebRequest.Create(URL);

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    return Bitmap.FromStream(stream);
                }
            }
        }

        public static void LoadImageAsync(string URL, Action<Image> Done)
        {
            string Path = Application.StartupPath + @"\temp";

            WebClient webClient = new WebClient();

            webClient.DownloadFileCompleted += delegate {
                try
                {
                    Image Im = Image.FromFile(Path);
                    Done(Im);
                }
                catch (Exception e)
                {

                }
            };

            webClient.DownloadFileAsync(new Uri(URL), Path);
        }
    }
}
