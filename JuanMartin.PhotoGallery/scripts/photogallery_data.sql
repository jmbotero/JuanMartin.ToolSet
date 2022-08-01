SET foreign_key_checks = 0;

REPLACE INTO `tblaudit` (`id`, `user_id`, `event_dtm`, `message`) VALUES
	(6, 1, '2022-06-30 16:35:28', 'Add tag \'bear\' for (2)'),
	(7, 1, '2022-07-01 10:50:37', 'Add tag \'bear\' for (5)'),
	(8, 1, '2022-07-01 11:03:04', 'Add tag \'bear\' for (17)'),
	(9, -1, '2022-07-01 16:21:28', 'Search for (bear,city) returned 5 results.'),
	(10, 1, '2022-07-01 16:23:01', 'Add tag \'bear\' for (14)'),
	(11, 1, '2022-07-01 16:33:31', 'Add tag \'bear\' for (15)'),
	(12, 1, '2022-07-01 16:34:01', 'Add tag \'city\' for (9)'),
	(13, 1, '2022-07-01 16:34:21', 'Search for (bear,city) returned 8 results.'),
	(14, 1, '2022-07-01 16:34:53', 'Add tag \'city\' for (6)'),
	(15, 1, '2022-07-01 16:35:04', 'Add tag \'tree\' for (6)'),
	(16, 1, '2022-07-01 16:35:29', 'Add tag \'bear\' for (8)'),
	(17, 1, '2022-07-01 16:35:43', 'Search for (bear,city) returned 8 results.'),
	(18, 1, '2022-07-01 16:36:53', 'Search for (bear,city) returned 8 results.'),
	(19, 1, '2022-07-03 11:30:31', 'Add tag \'bear\' for (9)'),
	(20, 1, '2022-07-03 11:30:48', 'Set rank = 6 for (9)'),
	(21, 1, '2022-07-03 11:32:09', 'Set rank = 10 for (5)'),
	(22, 1, '2022-07-03 11:32:51', 'Add tag \'tree\' for (7)'),
	(23, -1, '2022-07-04 11:36:09', 'Search for (bear,city) returned 8 results.'),
	(24, -1, '2022-07-04 11:38:40', 'Search for (bear,city) returned 8 results.'),
	(25, -1, '2022-07-05 09:36:49', 'Search for (bear,city) returned 8 results.'),
	(26, 1, '2022-07-05 09:52:05', 'Add tag \'city\' for (18)'),
	(27, 1, '2022-07-05 09:53:05', 'Add tag \'bear\' for (11)'),
	(28, 1, '2022-07-05 09:53:28', 'Search for (bear,city) returned 8 results.'),
	(29, 1, '2022-07-05 16:37:44', 'Search for (bear,city) returned 2 results.'),
	(30, 1, '2022-07-06 14:44:57', 'Set rank = 4 for (14)'),
	(31, 1, '2022-07-06 14:46:09', 'Search for (bear,city) returned 13 results.'),
	(32, -1, '2022-07-06 15:26:39', 'Search for (bear,city) returned 13 results.'),
	(33, -1, '2022-07-06 15:27:55', 'Search for (bear,city) returned 13 results.'),
	(34, -1, '2022-07-07 10:28:31', 'Search for (bear,city) returned 13 results.'),
	(35, -1, '2022-07-07 10:36:58', 'Search for (bear,city) returned 13 results.'),
	(36, -1, '2022-07-07 11:31:07', 'Search for (bear,city) returned 13 results.'),
	(37, -1, '2022-07-07 16:03:33', 'Search for (bear,city) returned 13 results.'),
	(38, 1, '2022-07-07 16:15:50', 'Set rank = 10 for (6)'),
	(39, 1, '2022-07-07 16:17:07', 'Set rank = 8 for (15)'),
	(40, 1, '2022-07-07 16:17:48', 'Search for (tree) returned 3 results.'),
	(41, 1, '2022-07-07 16:18:12', 'Set rank = 8 for (7)'),
	(42, 1, '2022-07-07 16:22:20', 'Search for (tree) returned 3 results.'),
	(43, -1, '2022-07-07 16:36:35', 'Search for (tree) returned 3 results.'),
	(44, 1, '2022-07-08 11:09:27', 'Set rank = 7 for (18)'),
	(45, -1, '2022-07-08 11:22:44', 'Search for (tree) returned 3 results.'),
	(46, 2, '2022-07-08 11:24:54', 'Set rank = 6 for (6)'),
	(47, 2, '2022-07-08 11:25:47', 'Set rank = 6 for (6)'),
	(48, 2, '2022-07-08 11:26:02', 'Set rank = 2 for (6)'),
	(49, -1, '2022-07-09 15:11:58', 'Search for (tree) returned 3 results.'),
	(50, -1, '2022-07-11 08:40:22', 'Search for (tree) returned 3 results.'),
	(51, 1, '2022-07-11 08:41:54', 'User logged in, started session (71).'),
	(52, 1, '2022-07-11 08:42:42', 'Set rank = 5 for (6)');


REPLACE INTO `tblphotography` (`id`, `_source`, `_path`, `title`, `filename`, `location_id`) VALUES
	(1, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001 - Copy (2).jpg', NULL),
	(2, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001 - Copy (3).jpg', NULL),
	(3, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001 - Copy (4).jpg', NULL),
	(4, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001 - Copy (5).jpg', NULL),
	(5, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001 - Copy (6).jpg', NULL),
	(6, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001 - Copy (7).jpg', NULL),
	(7, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001 - Copy (8).jpg', NULL),
	(8, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001 - Copy.jpg', NULL),
	(9, 0, '~\\photos\\negatives', '\'\'', '35mmNegatives01_0001.jpg', NULL),
	(10, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001 - Copy (2).jpg', NULL),
	(11, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001 - Copy (3).jpg', NULL),
	(12, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001 - Copy (4).jpg', NULL),
	(13, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001 - Copy (5).jpg', NULL),
	(14, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001 - Copy (6).jpg', NULL),
	(15, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001 - Copy (7).jpg', NULL),
	(16, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001 - Copy (8).jpg', NULL),
	(17, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001 - Copy.jpg', NULL),
	(18, 1, '~\\photos\\slide', '\'\'', 'Slide Memories 01_0001.jpg', NULL);


REPLACE INTO `tblphotographytags` (`photography_id`, `tag_id`) VALUES
	(1, 3),
	(1, 4),
	(3, 15),
	(2, 3),
	(5, 3),
	(17, 3),
	(14, 3),
	(15, 3),
	(9, 15),
	(6, 15),
	(6, 4),
	(8, 3),
	(9, 3),
	(7, 4),
	(18, 15),
	(11, 3);

REPLACE INTO `tblranking` (`id`, `user_id`, `photography_id`, `_rank`) VALUES
	(4, 1, 1, 8),
	(5, 2, 1, 2),
	(6, 1, 14, 4),
	(7, 1, 9, 6),
	(8, 1, 5, 10),
	(9, 1, 6, 5),
	(10, 1, 15, 8),
	(11, 1, 7, 8),
	(12, 1, 18, 7),
	(13, 2, 6, 2);


REPLACE INTO `tblsession` (`id`, `user_id`, `start_dtm`, `end_dtm`) VALUES
	(27, 4, '2022-06-10 16:53:38', NULL),
	(28, 1, '2022-06-15 14:58:01', NULL),
	(29, 1, '2022-06-15 16:17:43', '2022-06-15 16:19:02'),
	(30, 1, '2022-06-15 16:21:13', '2022-06-15 16:24:53'),
	(31, 1, '2022-06-15 16:26:46', NULL),
	(32, 1, '2022-06-20 07:32:23', NULL),
	(33, 1, '2022-06-20 10:39:49', NULL),
	(34, 1, '2022-06-20 10:48:35', NULL),
	(35, 1, '2022-06-20 10:56:50', NULL),
	(36, 1, '2022-06-20 11:15:15', NULL),
	(37, 1, '2022-06-20 15:03:10', '2022-06-20 15:04:16'),
	(38, 1, '2022-06-20 16:09:10', NULL),
	(39, 1, '2022-06-20 16:12:20', NULL),
	(40, 1, '2022-06-20 16:32:56', NULL),
	(41, 1, '2022-06-20 16:34:59', NULL),
	(42, 1, '2022-06-21 10:44:09', NULL),
	(43, 1, '2022-06-22 16:41:01', NULL),
	(44, 1, '2022-06-25 15:35:46', NULL),
	(45, 1, '2022-06-26 10:30:30', '2022-06-26 10:31:14'),
	(46, 1, '2022-06-26 11:02:48', '2022-06-26 11:03:11'),
	(47, 1, '2022-06-30 09:30:02', NULL),
	(48, 1, '2022-06-30 16:03:13', NULL),
	(49, 1, '2022-06-30 16:17:54', NULL),
	(50, 1, '2022-06-30 16:23:25', NULL),
	(51, 1, '2022-06-30 16:35:20', NULL),
	(52, 1, '2022-06-30 16:44:22', NULL),
	(53, 1, '2022-07-01 11:01:35', NULL),
	(54, 1, '2022-07-01 16:22:03', NULL),
	(55, 1, '2022-07-01 16:32:55', NULL),
	(56, 1, '2022-07-02 13:05:33', NULL),
	(57, 1, '2022-07-02 17:10:53', '2022-07-02 17:11:18'),
	(58, 1, '2022-07-02 17:11:53', NULL),
	(59, 1, '2022-07-03 11:29:29', NULL),
	(60, 1, '2022-07-03 12:34:45', NULL),
	(61, 1, '2022-07-03 15:59:40', '2022-07-03 16:01:22'),
	(62, 1, '2022-07-03 16:02:04', '2022-07-03 16:07:01'),
	(63, 1, '2022-07-03 16:08:12', NULL),
	(64, 1, '2022-07-04 11:31:44', '2022-07-04 11:34:05'),
	(65, 1, '2022-07-04 11:34:51', '2022-07-04 11:35:17'),
	(66, 1, '2022-07-05 09:51:30', NULL),
	(67, 1, '2022-07-06 14:25:28', NULL),
	(68, 1, '2022-07-07 16:15:28', NULL),
	(69, 1, '2022-07-08 11:09:06', NULL),
	(70, 2, '2022-07-08 11:24:30', NULL),
	(71, 1, '2022-07-11 08:41:54', NULL);


REPLACE INTO `tblstate` (`remote_host`, `user_id`, `redirect_controller`, `redirect_action`, `redirect_route_id`, `redirect_routevalues`, `event_dtm`) VALUES
	('::1', 1, 'Gallery', 'Detail', 6, '?fistId=0&lastId=7&searchQuery=tree&pageId=1', '2022-07-11 08:42:42'),
	('::1', 2, 'Gallery', 'Index', 1, '?pageId=2&firstId=0&lastId=18&searchQuery=', '2022-07-08 11:29:09');


REPLACE INTO `tbltag` (`id`, `word`) VALUES
	(3, 'bear'),
	(4, 'tree'),
	(15, 'city');


REPLACE INTO `tbluser` (`id`, `login`, `_password`, `email`) VALUES
	(1, 'juan', 'dc681250f7549ba735dcf6b5d13685c3', 'jbotero@hotmail.com'),
	(2, 'juanm', 'dc681250f7549ba735dcf6b5d13685c3', 'jbotero@hotmail.com');

SET foreign_key_checks = 1;