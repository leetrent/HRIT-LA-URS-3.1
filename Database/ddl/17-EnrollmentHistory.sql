CREATE TABLE `EnrollmentHistory` (
  `EnrollmentHistoryID` int(11) NOT NULL AUTO_INCREMENT,
  `ProgramEnrollmentID` int(11) NOT NULL,
  `StatusTransitionID` int(11) NOT NULL,
  `ActorUserId` varchar(767) NOT NULL,
  `ActorRemarks` text,
  `DateCreated` datetime NOT NULL,
  PRIMARY KEY (`EnrollmentHistoryID`),
  KEY `IX_EnrollmentHistory_ActorUserId` (`ActorUserId`),
  KEY `IX_EnrollmentHistory_ProgramEnrollmentID` (`ProgramEnrollmentID`),
  KEY `IX_EnrollmentHistory_StatusTransitionID` (`StatusTransitionID`),
  CONSTRAINT `FK_EnrollmentHistory_AspNetUsers_ActorUserId` FOREIGN KEY (`ActorUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnrollmentHistory_ProgramEnrollment_ProgramEnrollmentID` FOREIGN KEY (`ProgramEnrollmentID`) REFERENCES `ProgramEnrollment` (`ProgramEnrollmentID`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnrollmentHistory_StatusTransition_StatusTransitionID` FOREIGN KEY (`StatusTransitionID`) REFERENCES `StatusTransition` (`StatusTransitionID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;