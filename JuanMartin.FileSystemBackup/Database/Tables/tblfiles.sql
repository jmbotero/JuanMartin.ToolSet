USE backup;

CREATE TABLE IF NOT EXISTS tblfiles (
  FileId BIGINT NOT NULL AUTO_INCREMENT,
  FolderId BIGINT NOT NULL,
  Name CHAR(100) NOT NULL,
  Size FLOAT DEFAULT NULL,
  ModifiedDtm DATETIME NOT NULL,
  CONSTRAINT idxFileId PRIMARY KEY (FileId)
) ENGINE=MyISAM;
