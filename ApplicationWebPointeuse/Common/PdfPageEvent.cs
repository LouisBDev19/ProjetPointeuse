using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ApplicationWebPointeuse.Common
{
    public class PdfPageEvent : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD);
            Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL);

            PdfPTable headerTable = new PdfPTable(2);
            headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell cell = new PdfPCell(new Phrase("Fiche d'émargement", headerFont));
            cell.Border = Rectangle.NO_BORDER;
            headerTable.AddCell(cell);

            cell = new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), headerFont));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = Rectangle.NO_BORDER;
            headerTable.AddCell(cell);

            headerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - document.TopMargin + headerTable.TotalHeight, writer.DirectContent);

            PdfPTable footerTable = new PdfPTable(1);
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            footerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            cell = new PdfPCell(new Phrase("Page " + writer.PageNumber, footerFont));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = Rectangle.NO_BORDER;
            footerTable.AddCell(cell);

            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - footerTable.TotalHeight, writer.DirectContent);
        }
    }

}
