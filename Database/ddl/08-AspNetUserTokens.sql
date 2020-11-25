CREATE TABLE `AspNetUserTokens` (
  `UserId` varchar(767) NOT NULL,
  `LoginProvider` varchar(767) NOT NULL,
  `Name` varchar(767) NOT NULL,
  `Value` text,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;