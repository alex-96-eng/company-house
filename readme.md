# Company House

## Prerequisites 
- dotnet cli
- docker 

## Start PostgreSQL server
```console
docker compose up --build
```
- PGAdmin: http://localhost:5050/
- Username: hello@example.com
- Password: changeme

## Destroying
```console
docker compose down --volumes --remove-orphans
```


### Nuget Packages
```console
dotnet add package CsvHelper
dotnet add package Npgsql
dotnet add package Microsoft.Extensions.Configuration
```

## Usage
> Navigate to a new terminal and run
```console
cd CompanyHouse
```

# Load Company Data
```
dotnet run company-data-dump data/test_data
```

## Snapshots 

There are APIs that exists for this data, but their rate limits I think its best to warehouse the data.

1. The Free Company Data Product is a downloadable data snapshot containing basic company data of live 
   companies on the register. See: [Company data product](https://download.companieshouse.gov.uk/en_output.html)
2. The Accounts Data Product is a free downloadable ZIP file, which contains the individual data files (instance documents) 
   of company accounts filed electronically. See: [Accounts data product](https://download.companieshouse.gov.uk/en_accountsdata.html)
3. The People with significant control (PSC) snapshot is a downloadable data snapshot containing the full list 
   of PSC's provided to Companies House. See: [People with significant control (PSC) snapshot](https://download.companieshouse.gov.uk/en_pscdata.html)

