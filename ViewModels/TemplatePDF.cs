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
        public InvoiceDocument(InvoiceModel model)
        {
            Model = model;
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


                    page.Header().Element(ComposeHeader);
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
                row.ConstantItem(65).Height(65)
                .Image(System.IO.Path.Combine(AppContext.BaseDirectory, "Images", "Logo.png"));
            });
        }
        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Item()
                    .Layers(layers =>
                    {
                        layers.Layer().AlignBottom().Image(System.IO.Path.Combine(AppContext.BaseDirectory, "Images", "Logo.png"));

                        layers.PrimaryLayer()
                        .Column(innerColumn =>
                        {
                            innerColumn.Spacing(31);

                            innerColumn.Item().Row(row =>
                            {
                                row.RelativeItem().Component(new AddressComponent("Building information", Model.SellerAddress)); //Add user address
                                row.ConstantItem(250);
                            });

                            innerColumn.Item().Element(ComposeTable);

                            var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);
                            innerColumn.Item().AlignRight().Text($"Grand total: {totalPrice}$").FontSize(14);

                            if (!string.IsNullOrWhiteSpace(Model.Comments))
                                innerColumn.Item().PaddingTop(35).Element(ComposeComments);
                        });
                    });


            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Product");
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit price");
                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in Model.Items)
                {
                    table.Cell().Element(CellStyle).Text(Model.Items.IndexOf(item) + 1);
                    table.Cell().Element(CellStyle).Text(item.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price}$");
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price * item.Quantity}$");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }

        void ComposeComments(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Spacing(6);
                column.Item().Text("Comments").FontSize(14);
                column.Item().Text(Model.Comments);
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
