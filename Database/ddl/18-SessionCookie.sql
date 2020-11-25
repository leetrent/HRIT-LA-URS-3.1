USE URS;

CREATE TABLE `SessionCookie` (
  `UserName` varchar(256) NOT NULL,
  `CookieName` varchar(256) NOT NULL,
  `CookieValue` varchar(3072) NOT NULL,
  `LastAccessedOn` datetime NOT NULL,
  `CreatedOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`UserName`,`CookieName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;