namespace CompanyHouse.Entities
{
    public class BaseData
    {
        public int Id { get; set; }
        public string CompanyNumber { get; set; }
        public string? CompanyName { get; set; }
    }
    public class CompanyData : BaseData
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostTown { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string CompanyCategory { get; set; }
        public string CompanyStatus { get; set; }
        public string CountryOfOrigin { get; set; }
        public string DissolutionDate { get; set; }
        public string IncorporationDate { get; set; }
        public int? AccountRefDay { get; set; }
        public int? AccountRefMonth { get; set; }
        public string AccountNextDueDate { get; set; }
        public string AccountLastMadeUpdate { get; set; }
        public string AccountCategory { get; set; }
        public string ReturnsNextDueDate { get; set; }
        public string ReturnsLastMadeUpdate { get; set; }
        public int? MortgagesNumMortCharges { get; set; }
        public int? MortgagesNumMortOutstanding { get; set; }
        public int? MortgagesNumMortPartSatisfied { get; set; }
        public int? NumMortSatisfied { get; set; }
        public int? NumGenPartners { get; set; }
        public int? NumLimPartners { get; set; }
        public string Uri { get; set; }
        public string ConfStmtNextDueDate { get; set; }
        public string ConfStmtLastMadeUpdate { get; set; }

        public string DateOfBirthYear { get; set; }

        public string CountryOfResidence { get; set; }
        
        public string CeasedOn { get; set; }
        
        public string Premises { get; set; }

        public string Locality { get; set; }
    }
    public class PSCData : BaseData
    {
        public string AddressLine1 { get; set; }
        public string Country { get; set; }
        public string Locality { get; set; }
        public string PostalCode { get; set; }
        public string Premises { get; set; }
        public DateTime? CeasedOn { get; set; }
        public string CountryOfResidence { get; set; }
        public int? DobMonth { get; set; }
        public int? DobYear { get; set; }
        public string Etag { get; set; }
        public string Kind { get; set; }
        public string LinkSelf { get; set; }
        public string Name { get; set; }
        public string Forename { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public string Nationality { get; set; }
        public string NaturesOfControl { get; set; }
        public DateTime? NotifiedOn { get; set; }
    }
    public class FinancialData : BaseData
    {
        public bool? IsDormant { get; set; }

        public bool AsAgent { get; set; }
        public DateTime? ReportingPeriodStart { get; set; }
        public DateTime? ReportingPeriodEnd { get; set; }
        public DateTime? BalanceSheetDate { get; set; }
        public string EntityTradingStatus { get; set; }
        public string DirectorSigning { get; set; }
        public string DescriptionPrincipalActivities { get; set; }
        
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}