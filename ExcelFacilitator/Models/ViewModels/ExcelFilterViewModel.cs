namespace ExcelFacilitator.Models.ViewModels
{
    public class ExcelFilterViewModel
    {
        public List<string> SellerTin { get; set; }
        public List<string> BuyerTin { get; set; }
        public List<string> ItemCatalogCode { get; set; }
        public List<string> SourceFile { get; set; }

        public DateTime? ContractDateFrom { get; set; }
        public DateTime? ContractDateTo { get; set; }

        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        public decimal? QuantityFrom { get; set; }
        public decimal? QuantityTo { get; set; }

        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }

        public decimal? PriceWithVatFrom { get; set; }
        public decimal? PriceWithVatTo { get; set; }

        public decimal? TotalWithVatFrom { get; set; }
        public decimal? TotalWithVatTo { get; set; }

        public int? NumberOfRows { get; set; }

        public string TopCriteria { get; set; }
    }
}
