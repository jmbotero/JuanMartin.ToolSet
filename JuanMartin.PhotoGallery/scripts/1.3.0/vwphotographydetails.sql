-- Dumping structure for view gallery.vwphotographydetails
DROP VIEW IF EXISTS `vwphotographydetails`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `vwphotographydetails`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographydetails` AS select `p`.`id` AS `Id`,`p`.`filename` AS `Filename`,`loc`.`_reference` AS `Location`,`p`.`_path` AS `Path`,`p`.`title` AS `Title`,`p`.`archive` AS `Archive`,`vw`.`Taglist` AS `Tags` from ((`tblphotography` `p` left join `tbllocation` `loc` on((`loc`.`id` = `p`.`location_id`))) left join `vwphotographytags` `vw` on((`vw`.`photography_id` = `p`.`id`)));
