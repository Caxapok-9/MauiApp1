using CommunityToolkit.Maui.Storage;
using iText.Bouncycastle;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Crypto;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class ProtocolCreater
    {
        private static TaskCompletionSource<bool> _CompletedTask = new TaskCompletionSource<bool>();

        public static async Task CreatePDF(DatabaseService _db, Dictionary<string, byte[]> Signs, TaskCompletionSource<bool> CompletedTask)
        {     
            _CompletedTask = CompletedTask;

            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            ProtocolInfo info = new ProtocolInfo(_db);

            var dict = await info.GetDataDictionary();

            MemoryStream outputStream = new MemoryStream();

            MemoryStream streamSign = new MemoryStream();

            var streamTemplate = await FileSystem.OpenAppPackageFileAsync("protokol.pdf");

            string password = await App.Current.MainPage.DisplayPromptAsync("Безопасность", "Введите пин-код", "Ок", "Отмена");

            if (password == null || string.IsNullOrWhiteSpace(password))
            {
                _CompletedTask.SetResult(false);
                return;
            }

            try
            {
                iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(outputStream);

                writer.SetCloseStream(false);

                iText.Kernel.Pdf.PdfReader reader = new iText.Kernel.Pdf.PdfReader(streamTemplate);

                iText.Kernel.Pdf.PdfDocument doc = new iText.Kernel.Pdf.PdfDocument(reader, writer);

                iText.Forms.PdfAcroForm form = iText.Forms.PdfAcroForm.GetAcroForm(doc, true);

                await Setting.GetFonts();

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

                                image.SetHeight(14f);

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

                await SignPdfContractAsync(_db, outputStream, password);
            }
            catch(Exception ex) 
            {
                await App.Current.MainPage.DisplayAlert("Информация", ex.Message, "OK");

                _CompletedTask.SetResult(false);

                return;
            }
        }

        private static async Task SignPdfContractAsync(DatabaseService _db, MemoryStream generatedPdfStream, string Password)
        {
            Team TeamHome = await _db.GetTeamHomeAsync();

            Team TeamGuest = await _db.GetTeamGuestAsync();

            using Stream pfxStream = await FileSystem.OpenAppPackageFileAsync("VolleyApp.pfx");

            Pkcs12Store pkcs12Store = new Pkcs12StoreBuilder().Build();

            try
            {
                pkcs12Store.Load(pfxStream, Password.ToCharArray());
            }
            catch
            {
                await App.Current.MainPage.DisplayAlert("Информация", "Пароль неверный!", "OK");

                _CompletedTask.SetResult(false);

                return;
            }

            string alias = null;

            foreach (string currentAlias in pkcs12Store.Aliases)
            {
                if (pkcs12Store.IsKeyEntry(currentAlias))
                {
                    alias = currentAlias;
                    break;
                }
            }
         
            IX509Certificate[] chain = pkcs12Store.GetCertificateChain(alias).Select(x => new X509CertificateBC(x.Certificate)).ToArray();

            StampingProperties properties = new StampingProperties();

            ReaderProperties readerProperties = new ReaderProperties();

            PdfReader reader = new PdfReader(generatedPdfStream, readerProperties);

            MemoryStream memoryStream = new MemoryStream();

            PdfSigner signer = new PdfSigner(reader, memoryStream, properties);

            signer.GetDocument().SetCloseWriter(false);

            var pk = pkcs12Store.GetKey(alias).Key;

            IPrivateKey privateKey = new PrivateKeyBC(pk);

            IExternalSignature signature = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);

            signer.SignDetached(signature, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);

            byte[] bytes = memoryStream.ToArray();

            var res = await FileSaver.Default.SaveAsync($"Протокол матча {TeamHome.Name} - {TeamGuest.Name} от ({DateTime.Now.ToString("dd.MM.yyyy")}).pdf", new MemoryStream(bytes), CancellationToken.None);

            if (res.IsSuccessful)
            {
                await App.Current.MainPage.DisplayAlert("Информация", "Успешно сформирован PDF", "OK");

                _CompletedTask.SetResult(true);

                return;
            }
            else
            {
                try
                {
#if ANDROID
                            await App.Current.MainPage.DisplayAlert("Информация", "Сохранение PDF в \"Загрузки\"", "OK");

                            string downloadPath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;

                            string fullPath = System.IO.Path.Combine(downloadPath, $"Протокол матча {TeamHome.Name} - {TeamGuest.Name} от ({DateTime.Now.ToString("dd.MM.yyyy")}).pdf");

                            using (var fileStream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                            {
                                await new MemoryStream(bytes).CopyToAsync(fileStream);
                            }

                            await App.Current.MainPage.DisplayAlert("Информация", "PDF успешно сохранён в \"Загрузки\"", "OK");

                            _CompletedTask.SetResult(true);

                            memoryStream.Close();

                            generatedPdfStream.Close();

                            return;
#endif
                    await App.Current.MainPage.DisplayAlert("Информация", "Файл не сохранён", "OK");

                    _CompletedTask.SetResult(false);

                    return;
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Информация", ex.Message, "OK");

                    _CompletedTask.SetResult(false);

                    return;
                }
            }
        }
    }
    
}
