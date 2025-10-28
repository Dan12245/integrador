using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels
{
   public class InvoiceDocument : IDocument
    {
        public InvoiceModel Model { get; }
        public byte[] ChartWeekData { get; }
        public byte[] ChartMonthData { get; }
        public byte[] ChartYearData { get; }
        public InvoiceDocument(InvoiceModel model, byte[] chartWeekData = null,
            byte[] chartMonthData = null, byte[] chartYearData = null)
        {
            Model = model;
            ChartWeekData = chartWeekData;
            ChartMonthData = chartMonthData;
            ChartYearData = chartYearData;
        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.Header().PaddingBottom(20).Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();

                    });
                });
        }
        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Text($"Water consumption report #{Model.InvoiceNumber}")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                        
                    column.Item().Text(text =>
                    {
                        text.Span("Date: ").SemiBold();
                        text.Span($"{Model.IssueDate:d}");
                    });
                });
            });
        }
        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Item()
                    .Layers(layers =>
                    {
                        //Background image
                        layers.Layer().AlignBottom().Image(System.IO.Path.Combine(AppContext.BaseDirectory, "Images", "LogoOpacidad15.png"));

                        //Table + charts
                        layers.PrimaryLayer()
                        .Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Row(r =>
                                {
                                    r.RelativeItem().Component(new AddressComponent("Building information", Model.SellerAddress));
                                });

                                left.Item().TranslateY(15).Element(ComposeTable);

                                var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);
                                left.Item().TranslateY(20).TranslateX(50).Text($"Grand total: {totalPrice}").Bold().FontSize(12);

                            });

                            row.ConstantItem(240).TranslateY(130).Column(right =>
                            {
                                right.Spacing(5);

                            if (ChartWeekData != null && ChartWeekData.Length > 0)
                            {
                                right.Item()
                                     .Height(160)
                                     .Image(ChartWeekData);
                            }

                            if (ChartMonthData != null && ChartMonthData.Length > 0)
                            {
                                right.Item()
                                     .Height(160)
                                     .Image(ChartMonthData);
                            }

                            if (ChartYearData != null && ChartYearData.Length > 0)
                            {
                                right.Item()
                                        .Height(160)
                                        .Image(ChartYearData);
                            }
                            });
                         });
                    });
            });
        }

        void ComposeTable(IContainer container)
        {
            container.AlignLeft().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(80);
                    columns.ConstantColumn(80);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Day");
                    header.Cell().Element(CellStyle).AlignRight().Text("Consumption (litters)");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in Model.Items)
                {
                    table.Cell().Element(CellStyle).Text(Model.Items.IndexOf(item) + 1);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price}");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(0);
                    }
                }
            });
        }
    }
    public class AddressComponent : IComponent // By implementing IComponent we can create reusable components
    {
        private string Title { get; }
        private Address Address { get; }

        public AddressComponent(string title, Address address)
        {
            Title = title;
            Address = address;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(3);

                column.Item().BorderBottom(1).PaddingBottom(6).Text(Title).SemiBold();
                column.Item().Text(Address.CompanyName);
                column.Item().Text(Address.Street);
                column.Item().Text($"{Address.City}, {Address.State}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.Phone);
            });
        }
    }
}
