CREATE or REPLACE VIEW `URS`.`vEXT_LMS_Users` AS
select 
  case when (
	(SELECT MAX(pgmenrolla.DateExpired) 
	FROM URS.ProgramEnrollment pgmenrolla
	WHERE 
		pgmenrolla.StudentUserId = pgmenroll.StudentUserId
        -- and
        -- pgmenrolla.DateExpired = pgmenroll.DateExpired
		
    ) < now()) 
  then 'Inactive' 
  else 'Active' 
  end AS `STATUS`
 ,lower(`usr`.`UserName`) AS `USERID`
 ,lower(`usr`.`UserName`) AS `USERNAME`
 ,`usr`.`FirstName` AS `FIRSTNAME`
 ,`usr`.`LastName` AS `LASTNAME`
 ,left(`usr`.`MiddleName`,1) AS `MI`
 ,NULL AS `GENDER`
 ,lower(`usr`.`Email`) AS `EMAIL`
 ,'NO_MANAGER' AS `MANAGER`
 ,'NO_HR' AS `HR`
 ,NULL AS `DIVISION`
 ,`usr`.`SubAgencyID` AS `DEPARTMENT`
 ,NULL AS `LOCATION`
 ,NULL AS `JOBCODE`
 ,'US/Eastern' AS `TIMEZONE`
 ,NULL AS `HIREDATE`
 ,NULL AS `EMPID`
 ,`usr`.`JobTitle` AS `TITLE`
 ,`usr`.`PhoneNumber` AS `BIZ_PHONE`
 ,NULL AS `FAX`
 ,NULL AS `ADDR1`
 ,NULL AS `ADDR2`
 ,NULL AS `CITY`
 ,NULL AS `STATE`
 ,NULL AS `ZIP`
 ,NULL AS `COUNTRY`
 ,NULL AS `REVIEW_FREQ`
 ,NULL AS `LAST_REVIEW_DATE`
 ,`usr`.`JobTitle` AS `CUSTOM01`
 ,NULL AS `CUSTOM02`
 ,'PWD' AS `CUSTOM03`
 ,'Contractor' AS `CUSTOM04`
 ,NULL AS `CUSTOM05`
 ,NULL AS `CUSTOM06`
 ,NULL AS `CUSTOM07`
 ,NULL AS `CUSTOM08`
 , (SELECT MAX(pgmenrollb.DateExpired) 
   FROM URS.ProgramEnrollment pgmenrollb
   WHERE pgmenrollb.StudentUserID = pgmenroll.StudentUserId)
   AS CUSTOM09
 ,NULL AS `CUSTOM10`
 ,`usr`.`Id` AS `CUSTOM11`
 ,`usr`.`AgencyID` AS `CUSTOM12`
 ,NULL AS `CUSTOM13`
 ,NULL AS `CUSTOM14`
 ,group_concat(`pgm`.`ShortName` separator ';') AS `CUSTOM15`
 ,'en_US' AS `DEFAULT_LOCALE`
 ,'PWD' AS `LOGIN_METHOD`
 from 
 (
 (
 `URS`.`AspNetUsers` `usr` 
 join `URS`.`ProgramEnrollment` `pgmenroll` on ((`pgmenroll`.`StudentUserId` = `usr`.`Id`) and `pgmenroll`.`StatusCode` IN ('APPROVED','REVOKED')
 )
 ) 
 join `URS`.`LMSProgram` `pgm` on((`pgm`.`LMSProgramID` = `pgmenroll`.`LMSProgramID`
 )
 )
 ) 
 where 
 (
  (SELECT MAX(pgmenrollc.DateExpired) 
   FROM URS.ProgramEnrollment pgmenrollc
   WHERE pgmenrollc.StudentUserID = pgmenroll.StudentUserId 
     and
     pgmenroll.StatusCode IN ('APPROVED','REVOKED')) > (curdate() - interval 5 day
 )
 ) 
 group by `usr`.`Id`
 ;

