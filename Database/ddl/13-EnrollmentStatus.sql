CREATE TABLE `EnrollmentStatus` (
  `StatusCode` varchar(767) NOT NULL,
  `StatusLabel` varchar(767) NOT NULL,
  PRIMARY KEY (`StatusCode`),
  UNIQUE KEY `IX_EnrollmentStatus_StatusLabel` (`StatusLabel`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;