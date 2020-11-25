CREATE TABLE `EventLog` (
  `EventLogID` int(11) NOT NULL AUTO_INCREMENT,
  `EventTypeCode` varchar(767) NOT NULL,
  `UserCreatedID` varchar(767) NOT NULL,
  `UserCreatedName` text NOT NULL,
  `DataValues` text,
  `DateTimeCreated` datetime NOT NULL,
  PRIMARY KEY (`EventLogID`),
  KEY `IX_EventLog_EventTypeCode` (`EventTypeCode`),
  KEY `IX_EventLog_UserCreatedID` (`UserCreatedID`),
  CONSTRAINT `FK_EventLog_AspNetUsers_UserCreatedID` FOREIGN KEY (`UserCreatedID`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EventLog_EventType_EventTypeCode` FOREIGN KEY (`EventTypeCode`) REFERENCES `EventType` (`EventTypeCode`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;