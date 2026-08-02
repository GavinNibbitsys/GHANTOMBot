using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace GHANTOM.Core;

/// <summary>
/// Pops up an image in a WinForms window on its own STA thread. Used for the
/// "I'm the better artist" Mona Lisa gag when MS Paint is detected.
/// </summary>
public static class ImagePopup
{
    public static void Show(string imagePath)
    {
        var thread = new Thread(() =>
        {
            var form = new Form();
            var pictureBox = new PictureBox
            {
                Image = System.Drawing.Image.FromFile(imagePath),
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };

            form.Controls.Add(pictureBox);
            form.Width = 800;
            form.Height = 600;
            form.Text = "Hello :)";

            Application.Run(form);
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }

    public static void DownloadAndShow(string url)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "gbuddy_img.jpg");

        using (var client = new WebClient())
        {
            client.Headers.Add("User-Agent", "Mozilla/5.0");
            client.DownloadFile(url, tempPath);
        }

        Show(tempPath);
    }
}
