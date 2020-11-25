CREATE TABLE `urs`.`EmailToken` (
  `UserId` varchar(767) NOT NULL,
  `TokenValue` varchar(3072) NOT NULL,
  `CreatedOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  KEY `emailtoken_fk1` (`UserId`),
  CONSTRAINT `emailtoken_fk1` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1