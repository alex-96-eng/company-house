DROP TABLE IF EXISTS company_data;
CREATE TABLE company_data (
    companynumber VARCHAR(20) PRIMARY KEY, 
    -- No idea if these are nullable. Will fix up later.
    companyname VARCHAR(255),
    addressline1 VARCHAR(255),
    addressline2 VARCHAR(255),
    posttown VARCHAR(255),
    county VARCHAR(255),
    country VARCHAR(255),
    postcode VARCHAR(20),
    companycategory VARCHAR(255),
    companystatus VARCHAR(50),
    countryoforigin VARCHAR(255),
    dissolutiondate TEXT,
    incorporationdate TEXT,
    accountrefday INTEGER,
    accountrefmonth INTEGER,
    accountnextduedate TEXT,
    accountlastmadeupdate TEXT,
    accountcategory VARCHAR(255),
    returnsnextduedate TEXT,
    returnslastmadeupdate TEXT,
    mortgagesnummortcharges INTEGER,
    mortgagesnummortoutstanding INTEGER,
    mortgagesnummortpartsatisfied INTEGER,
    nummortsatisfied INTEGER,
    numgenpartners INTEGER,
    numlimpartners INTEGER,
    uri TEXT,
    confstmtnextduedate TEXT,
    confstmtlastmadeupdate TEXT
);

CREATE UNIQUE INDEX idx_companynumber ON company_data (companynumber);
