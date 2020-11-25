CREATE TABLE `EventType` (
  `EventTypeCode` varchar(767) NOT NULL,
  `EventTypeLabel` varchar(767) NOT NULL,
  PRIMARY KEY (`EventTypeCode`),
  UNIQUE KEY `IX_EventType_EventTypeLabel` (`EventTypeLabel`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;