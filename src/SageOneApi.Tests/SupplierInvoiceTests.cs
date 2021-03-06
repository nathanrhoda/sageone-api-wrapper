﻿using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SageOneApi.Models;
using SageOneApi.Requests;

namespace SageOneApi.Tests
{
    [TestClass]
    public class SupplierInvoiceTests : TestBase
    {
        private SupplierInvoiceRequest supplierInvoiceRequest;

        [TestInitialize]
        public void LocalInit()
        {
            supplierInvoiceRequest = Api.SupplierInvoiceRequest; // Api is declared on TestBase
        }

        [TestMethod]
        public void GetAllSupplierInvoices()
        {
            var invoices = supplierInvoiceRequest.Get(true, true);
        }

        [TestMethod]
        public void GetSupplierInvoice()
        {
            int invoiceId = 0;
            var invoice = supplierInvoiceRequest.Get(invoiceId);
        }

        [TestMethod]
        public void CreateSupplierInvoice()
        {
            var supplierId = 0;
            var itemId = 0;
            var taxTypeId = 0;

            SupplierInvoice po = new SupplierInvoice();
            var supplier = Api.SupplierRequest.Get(supplierId);

            po.SupplierId = supplierId;
            po.SupplierName = supplier.Name;
            po.Date = DateTime.Now;
            po.DueDate = DateTime.Now.AddDays(7);

            po.Lines = new List<CommercialDocumentLine>();
            var line1 = new CommercialDocumentLine
            {
                SelectionId = itemId, // This must be an item or account id
                TaxTypeId = taxTypeId, // Use TaxTypeRequest to get list of Tax Types
                LineType = 0, // 0=Item/1=Account -> See Enums.CommercialDocumentLineType
                Quantity = 1,
                UnitPriceExclusive = 390,
                UnitPriceInclusive = 390,
                DiscountPercentage = 0
            };

            po.Lines.Add(line1);

            var newSupplierInvoice = supplierInvoiceRequest.Save(po);
            Assert.IsTrue(supplierInvoiceRequest.StatusCode == HttpStatusCode.Created);
        }

        [TestMethod]
        public void CalculateAndSaveSupplierInvoice()
        {
            int invoiceId = 0;
            var invoice = supplierInvoiceRequest.Get(invoiceId);
            foreach (var line in invoice.Lines)
            {
                line.Quantity = line.Quantity + 1;
            }
            var calculatedInvoice = supplierInvoiceRequest.Calculate(invoice);
            var updatedInvoice = supplierInvoiceRequest.Save(calculatedInvoice);
        }

        [TestMethod]
        public void EmailSupplierInvoice()
        {
            int invoiceId = 112897987;
            var mailRequest = new EmailRequest
            {
                ID = invoiceId, // This must be the id of the supplier invoice
                EmailAddress = "you@email.com",
                CCAddress = "",
                BCCAddress = "",
                Subject = "Your Supplier Invoice",
                Message = "Your supplier invoice is attached"
            };
            supplierInvoiceRequest.Email(mailRequest);
        }
    }
}