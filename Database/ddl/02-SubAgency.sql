CREATE TABLE `SubAgency` (
  `SubAgencyID` varchar(767) NOT NULL,
  `AgencyID` varchar(767) NOT NULL,
  `DisplayOrder` int(11) NOT NULL,
  `OPMCode` text,
  `SubAgencyName` text NOT NULL,
  `TreasuryCode` text,
  PRIMARY KEY (`SubAgencyID`),
  KEY `IX_SubAgency_AgencyID` (`AgencyID`),
  CONSTRAINT `FK_SubAgency_Agency_AgencyID` FOREIGN KEY (`AgencyID`) REFERENCES `Agency` (`AgencyID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;