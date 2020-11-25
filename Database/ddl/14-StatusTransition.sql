CREATE TABLE `StatusTransition` (
  `StatusTransitionID` int(11) NOT NULL AUTO_INCREMENT,
  `FromStatusCode` varchar(767) NOT NULL,
  `ToStatusCode` varchar(767) NOT NULL,
  `TransitionCode` text NOT NULL,
  `TransitionLabel` text NOT NULL,
  PRIMARY KEY (`StatusTransitionID`),
  UNIQUE KEY `IX_StatusTransition_FromStatusCode_ToStatusCode` (`FromStatusCode`,`ToStatusCode`),
  KEY `IX_StatusTransition_ToStatusCode` (`ToStatusCode`),
  CONSTRAINT `FK_StatusTransition_EnrollmentStatus_FromStatusCode` FOREIGN KEY (`FromStatusCode`) REFERENCES `EnrollmentStatus` (`StatusCode`) ON DELETE CASCADE,
  CONSTRAINT `FK_StatusTransition_EnrollmentStatus_ToStatusCode` FOREIGN KEY (`ToStatusCode`) REFERENCES `EnrollmentStatus` (`StatusCode`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;