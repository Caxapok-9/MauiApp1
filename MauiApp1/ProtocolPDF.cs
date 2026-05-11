using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MauiApp1
{
    public class ProtocolPDF(DatabaseService db, ProtocolInfo info) : IDocument
    {
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(0.5f, Unit.Centimetre);
                page.PageColor(QuestPDF.Infrastructure.Color.FromRGB(255,255,255));
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Calibri"));

                page.Header().Element(ComposeHeader);
            });
        }

        public void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
        {
            container.Row(row =>
            {
                row.AutoItem().Text("ПРОТОКОЛ").Bold().FontSize(20);
                row.RelativeItem().AlignCenter().Text($"Соревнования по волейболу на первенство города Твери\nМежду командами А «{NameTeamHome}» и Б «{NameTeamGuest}»");
                row.AutoItem().AlignRight().Text(DateTime.Now.ToString());
            });
        }

        public void ComposeContent(QuestPDF.Infrastructure.IContainer container)
        {

        }

        public void ComposeFooter(QuestPDF.Infrastructure.IContainer container)
        {

        }
    }
}
