using iText.Forms;
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
        public static iText.Kernel.Pdf.PdfDocument CreatePDF(ProtocolInfo info, out string error)
        {
            try
            {
                iText.Kernel.Pdf.PdfDocument doc;

                using (PdfReader reader = new PdfReader())
                {
                    doc = new iText.Kernel.Pdf.PdfDocument(reader);

                    PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);

                    foreach(var item in info.GetDataDictionary())
                    {
                        var field = form.GetField(item.Key);

                        if(field != null)
                        {
                            field.SetValue(item.Value);
                        }
                    }

                    form.FlattenFields();
                }

                error = null;

                return doc;
            }
            catch(Exception e) 
            {
                error = e.Message;

                return null;
            }
        }
    }
}
