using System;
using CsvHelper.Configuration.Attributes;


public abstract class Row {

    [Name("CompanyNumber")]
    public string CompanyNumber { get; set; }

}