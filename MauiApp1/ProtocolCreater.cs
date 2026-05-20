using CommunityToolkit.Maui.Storage;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class ProtocolCreater
    {
        public static async Task CreatePDF(DatabaseService _db, Dictionary<string, byte[]> Signs)
        {
            ProtocolInfo info = new ProtocolInfo(_db);

            var dict = await info.GetDataDictionary();

            MemoryStream outputStream = new MemoryStream();

            try
            { 
                var streamTemplate = await FileSystem.OpenAppPackageFileAsync("protokol.pdf");

                PdfWriter writer = new PdfWriter(outputStream);

                writer.SetCloseStream(false);

                PdfReader reader = new PdfReader(streamTemplate);

                iText.Kernel.Pdf.PdfDocument doc = new iText.Kernel.Pdf.PdfDocument(reader, writer);

                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);

                foreach (var item in dict)
                {
                    var field = form.GetField(item.Key);

                    if (field != null && item.Value != null)
                    {
                        if (item.Key.Contains("Sign"))
                        {
                            if(Signs[item.Key] != null)
                            {
                                ImageData data = ImageDataFactory.Create(Signs[item.Key]);

                                iText.Layout.Element.Image image = new iText.Layout.Element.Image(data);

                                image.SetWidth(22f);

                                image.SetHeight(16f);

                                Rectangle rectangle = field.GetWidgets().FirstOrDefault().GetRectangle().ToRectangle();

                                image.SetFixedPosition(rectangle.GetLeft() + 4, rectangle.GetBottom() + 4);

                                Document layout = new Document(doc);

                                layout.Add(image);
                            }
                        }
                        else
                        {
                            field.SetFont(item.Value.Font);

                            field.SetFontSize(item.Value.Size);

                            field.SetJustification(item.Value.Align);

                            field.SetValue(string.IsNullOrWhiteSpace(item.Value.Text) ? string.Empty : item.Value.Text);
                        }
                    }
                }

                form.FlattenFields();

                doc.Close();

                outputStream.Position = 0;

                var res = await FileSaver.Default.SaveAsync("VolleyProtocol.pdf", outputStream, CancellationToken.None);

                if (res.IsSuccessful)
                {
                    await App.Current.MainPage.DisplayAlert("Информация", "Успешно сформирован PDF", "OK");
                }
                else
                {
                    // сохранение БД

                    await App.Current.MainPage.DisplayAlert("Информация", "Ошибка формирования PDF", "OK");
                }
            }
            catch(Exception e) 
            {
                // сохранение БД

                await App.Current.MainPage.DisplayAlert("Информация", e.Message, "OK");

                outputStream.Close();
            }
        }
    }
}
