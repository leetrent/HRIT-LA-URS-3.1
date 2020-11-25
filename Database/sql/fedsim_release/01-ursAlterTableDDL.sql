ALTER TABLE urs.lmsprogram 
ADD COLUMN ExpiryDays INT(11) UNSIGNED DEFAULT 180
AFTER CommonInbox;

ALTER TABLE urs.enrollmentstatus 
ADD COLUMN DisplayOrder TINYINT(3) UNSIGNED NOT NULL DEFAULT 0
AFTER StatusLabel;

ALTER TABLE urs.programenrollment 
ADD COLUMN DateExpired DATETIME DEFAULT NULL
AFTER DateLastUpdated;