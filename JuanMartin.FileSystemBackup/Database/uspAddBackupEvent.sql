DELIMITER $$

DROP PROCEDURE IF EXISTS uspAddBackupEvent;

CREATE PROCEDURE uspAddBackupEvent(IN JobId BIGINT, IN NewStatus CHAR(10), IN EventDtm CHAR(25))
BEGIN

	UPDATE tblbackupjobexecution job
		SET job.EndDtm = STR_TO_DATE(EventDtm,'%m/%d/%Y %h:%i:%s %p'), job.Status = NewStatus
	WHERE job.JobId = JobId AND NewStatus!='INPROGRESS';

	IF NewStatus = 'INPROGRESS' THEN
		UPDATE tblbackupjobexecution job
			SET job.StartDtm = STR_TO_DATE(EventDtm,'%m/%d/%Y %h:%i:%s %p'), job.Status = NewStatus
		WHERE job.JobId = JobId;
	END IF;
		
		-- Add a new event 
	IF ROW_COUNT() = 0 THEN 
		INSERT INTO tblbackupjobexecution (JobId, StartDtm, EndDtm, Status) VALUES(JobId, STR_TO_DATE(EventDtm,'%m/%d/%Y %h:%i:%s %p'), STR_TO_DATE(EventDtm,'%m/%d/%Y %h:%i:%s %p'), NewStatus);
	END IF;
END $$

DELIMITER ;
