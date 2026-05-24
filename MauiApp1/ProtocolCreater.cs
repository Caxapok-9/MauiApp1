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
        public static async Task<bool> CreatePDF(DatabaseService _db, Dictionary<string, byte[]> Signs)
        {
            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

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

                                image.SetFixedPosition(rectangle.GetLeft() + 8, rectangle.GetBottom() + 2);

                                Document layout = new Document(doc);

                                layout.Add(image);
                            }
                        }
                        else
                        {
                            field.SetFont(item.Value.Font);

                            field.SetFontSize(item.Value.Size);

                            field.SetJustification(item.Value.Align);

                            field.SetValue(item.Value.Text);
                        }
                    }
                }

                form.FlattenFields();

                doc.Close();

                outputStream.Position = 0;

                var res = await FileSaver.Default.SaveAsync($"Протокол матча {TeamHome.Name} - {TeamGuest.Name} от ({DateTime.Now.ToString("dd.MM.yyyy")}).pdf", outputStream, CancellationToken.None);

                if (res.IsSuccessful)
                {
                    await App.Current.MainPage.DisplayAlert("Информация", "Успешно сформирован PDF", "OK");

                    return true;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Информация", "Сохранение PDF в \"Загрузки\"", "OK");

                    try
                    {
#if ANDROID
                        string downloadPath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;

                        string fullPath = System.IO.Path.Combine(downloadPath, $"Протокол матча {TeamHome.Name} - {TeamGuest.Name} от ({DateTime.Now.ToString("dd.MM.yyyy")}).pdf");

                        using (var fileStream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                        {
                            await outputStream.CopyToAsync(fileStream);
                        }
#endif              
                        await App.Current.MainPage.DisplayAlert("Информация", "PDF успешно сохранён в \"Загрузки\"", "OK");

                        return true;
                    }
                    catch (Exception ex)
                    {
                        await App.Current.MainPage.DisplayAlert("Информация", ex.Message, "OK");

                        outputStream.Close();

                        return false;
                    }
                }
            }
            catch(Exception ex) 
            {
                await App.Current.MainPage.DisplayAlert("Информация", ex.Message, "OK");

                outputStream.Close();

                return false;
            }
        }
    }
}
