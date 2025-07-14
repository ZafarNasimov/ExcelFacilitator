namespace ExcelFacilitator.Models.Domain
{
    public class ExcelRecord
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string ContractNo { get; set; }
        public DateTime ContractDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string SellerTin { get; set; }
        public string SellerName { get; set; }
        public string BuyerTin { get; set; }
        public string CommitentTin { get; set; }
        public string CommitentName { get; set; }
        public string CommitentVatNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCatalogCode { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceWithVat { get; set; }
        public decimal TotalWithVat { get; set; }
        public DateTime DocumentDate { get; set; }
        public string SourceFile { get; set; }
    }
}
