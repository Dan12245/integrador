using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Helpers;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo.ViewModels
{
    public class InvoiceDocumentDataSource // This class generates random data for the invoice PDF document. This will query a database in the future
    {
        private static Random Random = new Random();

        public InvoiceModel GetInvoiceDetails()
        {
            var items = Enumerable
                .Range(1, 30)
                .Select(i => GenerateRandomOrderItem())
                .ToList();

            return new InvoiceModel
            {
                InvoiceNumber = Random.Next(1_000, 10_000),
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now + TimeSpan.FromDays(14),

                SellerAddress = GenerateRandomAddress(),
                CustomerAddress = GenerateRandomAddress(),

                Items = items,
                Comments = Placeholders.Paragraph()
            };
        }

        private static OrderItem GenerateRandomOrderItem()
        {
            return new OrderItem
            {
                Name = Placeholders.Label(),
                Price = (decimal)Math.Round(Random.NextDouble() * 100, 2),
                Quantity = Random.Next(1, 10)
            };
        }

        private static Address GenerateRandomAddress()
        {
            return new Address
            {
                CompanyName = Placeholders.Name(),
                Street = Placeholders.Label(),
                City = Placeholders.Label(),
                State = Placeholders.Label(),
                Email = Placeholders.Email(),
                Phone = Placeholders.PhoneNumber()
            };
        }
    }


}
