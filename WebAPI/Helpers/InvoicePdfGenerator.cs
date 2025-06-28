using WebAPI.Models;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace WebAPI.Helpers
{
    public static class InvoicePdfGenerator
    {
        public static byte[] Generate(CustomerInvoice invoice)
        {
            var companyInfo = new
            {
                CompanyName = "Invoicika Inc.",
                Address = "456 Business Road, Metropolis",
                Email = "info@invoicika.com",
                PhoneNumbers = new[] { "555-1010", "555-1020", "555-3030" }
            };

            var customerInfo = new
            {
                Name = invoice.Customer.Name,
                Address = invoice.Customer.Address,
                Email = invoice.Customer.Email,
                Phone = invoice.Customer.PhoneNumber
            };

            var invoiceDetails = new
            {
                InvoiceNumber = invoice.CustomerInvoiceId.ToString(),
                InvoiceDate = invoice.InvoiceDate.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture),
                InvoiceStatus = invoice.Status,
                InvoiceType = invoice.InvoiceType
            };

            var vatPercentage = invoice.VAT.Percentage;
            var subTotal = invoice.SubTotalAmount;
            var vat = subTotal * (invoice.VatAmount / 100);
            var total = invoice.TotalAmount;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Row(row =>
                    {
                        row.ConstantItem(20).Image("wwwroot/uploads/invoicika.png").FitArea();
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text(companyInfo.CompanyName).Bold().FontSize(18).AlignLeft();
                        });
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(columnLeft =>
                            {
                                columnLeft.Item().Text("From").Bold().FontSize(10).FontColor(Colors.Blue.Darken2);
                                columnLeft.Item().Text($"{companyInfo.CompanyName}").Bold().FontSize(12);
                                columnLeft.Item().Text($"{companyInfo.Address}");
                                columnLeft.Item().Text($"{companyInfo.Email}");
                                foreach (var phone in companyInfo.PhoneNumbers)
                                {
                                    columnLeft.Item().Text($"{phone}");
                                }
                            });

                            row.RelativeItem().Column(columnRight =>
                            {
                                columnRight.Item().Text("To").Bold().FontSize(10).FontColor(Colors.Blue.Darken2);
                                columnRight.Item().Text($"{customerInfo.Name}").Bold().FontSize(12);
                                columnRight.Item().Text($"{customerInfo.Address}");
                                columnRight.Item().Text($"{customerInfo.Email}");
                                columnRight.Item().Text($"{customerInfo.Phone}");
                            });

                            row.RelativeItem().Column(columnRight =>
                            {
                                columnRight.Item().Text("Invoice Number").Bold().FontSize(10).FontColor(Colors.Blue.Darken2);
                                columnRight.Item().PaddingBottom(5).Text($"{invoiceDetails.InvoiceNumber}").Bold().FontSize(9);
                                columnRight.Item().Text($"{invoiceDetails.InvoiceDate}");
                            });
                        });

                        column.Item().PaddingTop(5).PaddingBottom(15).LineHorizontal(1).LineColor(Colors.Blue.Medium);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn(180);
                                columns.RelativeColumn(40);
                                columns.RelativeColumn(60);
                                columns.RelativeColumn(70);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Item Name");
                                header.Cell().Element(CellStyle).Text("Description");
                                header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                                header.Cell().Element(CellStyle).AlignRight().Text("Unit Price");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.Bold()).PaddingTop(10).Background(Colors.Blue.Lighten2);
                                }
                            });

                            foreach (var line in invoice.CustomerInvoiceLines)
                            {
                                table.Cell().Element(CellStyle).Text(line.Item.Name);
                                table.Cell().Element(CellStyle).Text(line.Item.Description);
                                table.Cell().Element(CellStyle).AlignRight().Text(line.Quantity.ToString("N0", CultureInfo.InvariantCulture));
                                table.Cell().Element(CellStyle).AlignRight().Text($"{line.Price.ToString("F2", CultureInfo.InvariantCulture)}$");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{(line.Price * line.Quantity).ToString("F2", CultureInfo.InvariantCulture)}$");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Blue.Lighten2).PaddingVertical(3);
                                }
                            }
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn(180);
                                columns.RelativeColumn(40);
                                columns.RelativeColumn(60);
                                columns.RelativeColumn(70);
                            });

                            table.Cell().ColumnSpan(4).Element(LabelCellStyle).Text("Subtotal");
                            table.Cell().Element(ValueCellStyle).AlignRight().Text($"{subTotal.ToString("F2", CultureInfo.InvariantCulture)}$");

                            table.Cell().ColumnSpan(4).Element(LabelCellStyle).Text($"VAT ({vatPercentage}%)");
                            table.Cell().Element(ValueCellStyle).AlignRight().Text($"{vat.ToString("F2", CultureInfo.InvariantCulture)}$");

                            table.Cell().ColumnSpan(4).Element(LabelCellStyle).Text("Grand Total").FontColor(Colors.Blue.Darken2).Bold().FontSize(12);
                            table.Cell().Element(ValueCellStyle).AlignRight().Text($"{total.ToString("F2", CultureInfo.InvariantCulture)}$").FontColor(Colors.Blue.Darken2).Bold().FontSize(12);

                            static IContainer LabelCellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).AlignRight();
                            }

                            static IContainer ValueCellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.Bold()).PaddingVertical(5);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Column(column =>
                    {
                        column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        column.Spacing(2);
                        column.Item().Text("This computer-generated document is valid without signature.").AlignCenter();
                    });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return stream.ToArray();
            }
        }
    }
}