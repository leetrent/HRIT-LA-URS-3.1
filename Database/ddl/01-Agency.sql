CREATE TABLE `Agency` (
  `AgencyID` varchar(767) NOT NULL,
  `AgencyName` text NOT NULL,
  `DisplayOrder` int(11) NOT NULL,
  `OPMCode` text,
  `TreasuryCode` text,
  PRIMARY KEY (`AgencyID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;