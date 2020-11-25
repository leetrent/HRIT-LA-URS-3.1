-- SET SQL_SAFE_UPDATES = 0;

-- lmsprogram
INSERT INTO urs.lmsprogram (ShortName, LongName, CommonInbox)
VALUES ('FEDSIM', 'FEDSIM Learning Academy Outreach Program', 'fedsim.university@gsa.gov');

-- enrollmentstatus
UPDATE urs.enrollmentstatus SET DisplayOrder = 1 WHERE StatusCode = 'PENDING';
UPDATE urs.enrollmentstatus SET DisplayOrder = 2 WHERE StatusCode = 'APPROVED';
UPDATE urs.enrollmentstatus SET DisplayOrder = 3 WHERE StatusCode = 'DENIED';
UPDATE urs.enrollmentstatus SET DisplayOrder = 4 WHERE StatusCode = 'REVOKED';
UPDATE urs.enrollmentstatus SET DisplayOrder = 5 WHERE StatusCode = 'WITHDRAWN';

INSERT INTO urs.enrollmentstatus (StatusCode, StatusLabel, DisplayOrder)
VALUES ('EXPIRATION_PENDING', 'Expiration Pending', 6);

INSERT INTO urs.enrollmentstatus (StatusCode, StatusLabel, DisplayOrder)
VALUES ('EXPIRED', 'Expired', 7);

-- programenrollment
UPDATE urs.programenrollment
   SET datelastupdated = datecreated;

UPDATE urs.programenrollment 
   SET dateexpired = date_add(datelastupdated, INTERVAL 180 DAY)
 WHERE StatusCode = 'APPROVED';
