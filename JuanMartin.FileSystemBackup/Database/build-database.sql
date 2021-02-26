USE backup;
# --------------------------------------------------------
# Host:                         127.0.0.1
# Database:                     backup
# Server version:               5.1.52-community
# Server OS:                    Win64
# HeidiSQL version:             5.0.0.3272
# Date/time:                    2010-11-24 20:09:18
# --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
# Dumping database structure for backup
CREATE DATABASE IF NOT EXISTS backup /*!40100 DEFAULT CHARACTER SET latin1 */;
USE backup;


# Dumping structure for table backup.tblbackupjobdefinition
CREATE TABLE IF NOT EXISTS tblbackupjobdefinition (
  JobId BIGINT NOT NULL,
  FolderId BIGINT NOT NULL,
  KEY idxDefinition_JobId_FolderId (JobId,FolderId)
) ENGINE=MyISAM;

# Dumping data for table backup.tblbackupjobdefinition: 0 rows
DELETE FROM tblbackupjobdefinition;
/*!40000 ALTER TABLE tblbackupjobdefinition DISABLE KEYS */;
/*!40000 ALTER TABLE tblbackupjobdefinition ENABLE KEYS */;


# Dumping structure for table backup.tblbackupjobfiles
CREATE TABLE IF NOT EXISTS tblbackupjobfiles (
  JobId BIGINT NOT NULL,
  FileId BIGINT NOT NULL,
  BackupDtm DATETIME NOT NULL,
  IsNew BIT(1) NOT NULL
) ENGINE=MyISAM;

# Dumping data for table backup.tblbackupjobfiles: 0 rows
DELETE FROM tblbackupjobfiles;
/*!40000 ALTER TABLE tblbackupjobfiles DISABLE KEYS */;
/*!40000 ALTER TABLE tblbackupjobfiles ENABLE KEYS */;


# Dumping structure for table backup.tblbackupjobs
CREATE TABLE IF NOT EXISTS tblbackupjobs (
  JobId BIGINT NOT NULL AUTO_INCREMENT,
  Name CHAR(30) NOT NULL,
  Target CHAR(100) NOT NULL,
  PRIMARY KEY (JobId)
) ENGINE=MyISAM;

# Dumping data for table backup.tblbackupjobs: 0 rows
DELETE FROM tblbackupjobs;
/*!40000 ALTER TABLE tblbackupjobs DISABLE KEYS */;
/*!40000 ALTER TABLE tblbackupjobs ENABLE KEYS */;

# Dumping structure for table backup.tblbackupjobs
CREATE TABLE IF NOT EXISTS tblbackupjobexecution (
  JodId BIGINT NOT NULL,
  StartDtm DATETIME NOT NULL,
  EndDtm DATETIME NOT NULL,
  Status CHAR(10) NOT NULL
) ENGINE=MyISAM;

# Dumping data for table backup.tblbackupjobs: 0 rows
DELETE FROM tblbackupjobs;
/*!40000 ALTER TABLE tblbackupjobs DISABLE KEYS */;
/*!40000 ALTER TABLE tblbackupjobs ENABLE KEYS */;

# Dumping structure for table backup.tblfiles
CREATE TABLE IF NOT EXISTS tblfiles (
  FileId BIGINT NOT NULL AUTO_INCREMENT,
  FolderId BIGINT NOT NULL,
  Name CHAR(100) NOT NULL,
  Size FLOAT DEFAULT NULL,
  ModifiedDtm DATETIME NOT NULL,
  PRIMARY KEY (FileId)
) ENGINE=MyISAM;

# Dumping data for table backup.tblfiles: 0 rows
DELETE FROM tblfiles;
/*!40000 ALTER TABLE tblfiles DISABLE KEYS */;
/*!40000 ALTER TABLE tblfiles ENABLE KEYS */;


# Dumping structure for table backup.tblfolders
CREATE TABLE IF NOT EXISTS tblfolders (
  FolderId BIGINT NOT NULL AUTO_INCREMENT,
  Name CHAR(200) NOT NULL,
  Path TEXT NOT NULL,
  PRIMARY KEY (FolderId)
) ENGINE=MyISAM;

# Dumping data for table backup.tblfolders: 0 rows
DELETE FROM tblfolders;
/*!40000 ALTER TABLE tblfolders DISABLE KEYS */;
/*!40000 ALTER TABLE tblfolders ENABLE KEYS */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
