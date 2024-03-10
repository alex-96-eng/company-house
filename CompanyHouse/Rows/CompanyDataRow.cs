namespace CompanyHouse.Rows
{
    using System;
    using System.Globalization;
    using CsvHelper.Configuration.Attributes;

    public class CompanyDataRow: Row
    {
        [Name("CompanyName")]
        public string? CompanyName { get; set; }
        
        [Name("RegAddress.AddressLine1")]
        public string? AddressLine1 { get; set; }
        
        [Name("RegAddress.AddressLine2")]
        public string? AddressLine2 { get; set; }
        
        [Name("RegAddress.PostTown")]
        public string? PostTown { get; set; }
        
        [Name("RegAddress.County")]
        public string? County { get; set; }
        
        [Name("RegAddress.Country")]
        public string? Country { get; set; }
        
        [Name("RegAddress.PostCode")]
        public string? PostCode { get; set; }
        
        [Name("CompanyCategory")]
        public string? CompanyCategory { get; set; }
        
        [Name("CompanyStatus")]
        public string? CompanyStatus { get; set; }
        
        [Name("CountryOfOrigin")]
        public string? CountryOfOrigin { get; set; }
        
        [Name("DissolutionDate")]
        [DateTimeStyles(DateTimeStyles.None)]
        [Format("dd/MM/yyyy")]
        public string? DissolutionDate { get; set; }
        
        [Name("IncorporationDate")]
        [DateTimeStyles(DateTimeStyles.None)]
        [Format("dd/MM/yyyy")]
        public string? IncorporationDate { get; set; }
        
        [Name("Accounts.AccountRefDay")]
        public int? AccountRefDay { get; set; }
        
        [Name("Accounts.AccountRefMonth")]
        public int? AccountRefMonth { get; set; }
        
        [Name("Accounts.NextDueDate")]
        [DateTimeStyles(DateTimeStyles.None)]
        [Format("dd/MM/yyyy")]
        public string? AccountNextDueDate { get; set; }
        
        [Name("Accounts.LastMadeUpDate")]
        [DateTimeStyles(DateTimeStyles.None)]
        [Format("dd/MM/yyyy")]
        public string? AccountLastMadeUpDate{ get; set; }
        
        [Name("Accounts.AccountCategory")]
        public string? AccountCategory { get; set; }
        
        [Name("Returns.NextDueDate")]
        [DateTimeStyles(DateTimeStyles.None)]
        [Format("dd/MM/yyyy")]
        public string? ReturnsNextDueDate { get; set; }
        
        [Name("Returns.LastMadeUpDate")]
        [DateTimeStyles(DateTimeStyles.None)]
        [Format("dd/MM/yyyy")]
        public string? ReturnsLastMadeUpDate { get; set; }
        
        [Name("Mortgages.NumMortCharges")]
        public int? MortgagesNumMortCharges { get; set; }
        
        [Name("Mortgages.NumMortOutstanding")]
        public int? MortgagesNumMortOutstanding { get; set; }
        
        [Name("Mortgages.NumMortPartSatisfied")]
        public int? MortgagesNumMortPartSatisfied { get; set; }
        
        [Name("Mortgages.NumMortSatisfied")]
        public int? NumMortSatisfied { get; set; }
        
        [Name("LimitedPartnerships.NumGenPartners")]
        public int? NumGenPartners { get; set; }
        
        [Name("LimitedPartnerships.NumLimPartners")]
        public int? NumLimPartners { get; set; }
        
        [Name("URI")]
        public Uri? Uri { get; set; }
        
        [Name("ConfStmtNextDueDate")]
        [DateTimeStyles(DateTimeStyles.None)]
        [Format("dd/MM/yyyy")]
        public string? ConfStmtNextDueDate { get; set; }
        
        [Name("ConfStmtLastMadeUpDate")]
        [DateTimeStyles(DateTimeStyles.None)]
        [Format("dd/MM/yyyy")]
        public string? ConfStmtLastMadeUpDate { get; set; }
    
        /* public CompanyDataRow()
        {
            SICCodes = new List<string>();
            PreviousCompanyNames = new List<PreviousCompanyName>();
        } */
    }
}