using LearningPlatform.Application.Common.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LearningPlatform.Infrastructure.Certificates;

public class QuestPdfCertificateGenerator : ICertificatePdfGenerator
{
    static QuestPdfCertificateGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generate(CertificatePdfData data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontFamily("Arial"));

                page.Content()
                    .Border(2)
                    .BorderColor(Colors.Blue.Darken2)
                    .Padding(30)
                    .Column(column =>
                    {
                        column.Spacing(15);

                        column.Item().AlignCenter().Text("Certificate of Completion")
                            .FontSize(32).Bold().FontColor(Colors.Blue.Darken3);

                        column.Item().AlignCenter().Text("This certifies that").FontSize(14);

                        column.Item().AlignCenter().Text(data.StudentName)
                            .FontSize(26).Bold();

                        column.Item().AlignCenter().Text("has successfully completed the course").FontSize(14);

                        column.Item().AlignCenter().Text(data.CourseName)
                            .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);

                        column.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().AlignCenter().Text("Instructor").FontSize(10).FontColor(Colors.Grey.Darken1);
                                c.Item().AlignCenter().Text(data.InstructorName).FontSize(14).Bold();
                            });

                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().AlignCenter().Text("Issue Date").FontSize(10).FontColor(Colors.Grey.Darken1);
                                c.Item().AlignCenter().Text(data.IssuedAt.ToString("yyyy-MM-dd")).FontSize(14).Bold();
                            });
                        });

                        column.Item().PaddingTop(20).AlignCenter()
                            .Text($"Certificate Number: {data.CertificateNumber}")
                            .FontSize(11).FontColor(Colors.Grey.Darken2);

                        column.Item().AlignCenter()
                            .Text("Verify this certificate using the certificate number above.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                    });
            });
        });

        return document.GeneratePdf();
    }
}
