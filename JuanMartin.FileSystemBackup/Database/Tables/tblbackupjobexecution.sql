USE backup;

CREATE TABLE IF NOT EXISTS tblbackupjobexecution (
  JobId BIGINT NOT NULL,
  StartDtm DATETIME NOT NULL,
  EndDtm DATETIME NOT NULL,
  Status CHAR(10) NOT NULL
) ENGINE=MyISAM;
