CREATE TABLE `LMSProgram` (
  `LMSProgramID` int(11) NOT NULL AUTO_INCREMENT,
  `ShortName` text NOT NULL,
  `LongName` text NOT NULL,
  `CommonInbox` text,
  PRIMARY KEY (`LMSProgramID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;