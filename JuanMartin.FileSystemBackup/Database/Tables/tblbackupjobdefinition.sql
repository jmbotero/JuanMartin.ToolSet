USE backup;

CREATE TABLE IF NOT EXISTS tblbackupjobdefinition (
  JobId BIGINT NOT NULL,
  FolderId BIGINT NOT NULL,
  CONSTRAINT idxJobId_FolderId PRIMARY KEY (JobId,FolderId)
) ENGINE=MyISAM;
