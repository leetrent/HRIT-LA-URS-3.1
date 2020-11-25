CREATE TABLE `ProgramApprover` (
  `LMSProgramID` int(11) NOT NULL,
  `ApproverUserId` varchar(767) NOT NULL,
  `EmailNotify` tinyint(1) NOT NULL,
  PRIMARY KEY (`LMSProgramID`,`ApproverUserId`),
  KEY `IX_ProgramApprover_ApproverUserId` (`ApproverUserId`),
  CONSTRAINT `FK_ProgramApprover_AspNetUsers_ApproverUserId` FOREIGN KEY (`ApproverUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProgramApprover_LMSProgram_LMSProgramID` FOREIGN KEY (`LMSProgramID`) REFERENCES `LMSProgram` (`LMSProgramID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;