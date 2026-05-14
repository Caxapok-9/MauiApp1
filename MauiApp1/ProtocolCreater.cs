using CommunityToolkit.Maui.Storage;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class ProtocolCreater
    {
        public static async Task CreatePDF(Dictionary<string, string> info)
        {
            try
            {
                using var fontStream = await FileSystem.OpenAppPackageFileAsync("CALIBRI.TTF");

                using var fontMs = new MemoryStream();

                await fontStream.CopyToAsync(fontMs);

                byte[] fontBytes = fontMs.ToArray();

                PdfFont myCyrillicFont = PdfFontFactory.CreateFont(fontBytes, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

                MemoryStream outputStream = new MemoryStream();

                var streamTemplate = await FileSystem.OpenAppPackageFileAsync("protokol.pdf");

                PdfWriter writer = new PdfWriter(outputStream);

                writer.SetCloseStream(false);

                PdfReader reader = new PdfReader(streamTemplate);

                iText.Kernel.Pdf.PdfDocument doc = new iText.Kernel.Pdf.PdfDocument(reader, writer);

                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);

                foreach (var item in info)
                {
                    var field = form.GetField(item.Key);

                    if (field != null)
                    {
                        field.SetFont(myCyrillicFont);
                        field.SetFontSize(12);
                        field.SetJustification(iText.Layout.Properties.TextAlignment.CENTER);
                        field.SetValue(item.Value);
                    }
                }

                form.FlattenFields();

                doc.Close();

                outputStream.Position = 0; 

                var result = await FileSaver.Default.SaveAsync("VolleyProtocol.pdf", outputStream, CancellationToken.None);

                outputStream.Close();

                if(result.IsSuccessful)
                {
                    await App.Current.MainPage.DisplayAlert("Информация", "Успешно сформирован PDF", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Информация", "Ошибка формирования PDF", "OK");
                }
            }
            catch(Exception e) 
            {
                await App.Current.MainPage.DisplayAlert("Информация", e.Message, "OK");
            }
        }
    }
}
