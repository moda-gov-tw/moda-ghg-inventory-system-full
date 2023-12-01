-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主機： 127.0.0.1
-- 產生時間： 2023-12-01 07:40:44
-- 伺服器版本： 10.4.28-MariaDB
-- PHP 版本： 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 資料庫： `moda1`
--

-- --------------------------------------------------------

--
-- 資料表結構 `basicdatas`
--

CREATE TABLE `basicdatas` (
  `BasicId` int(11) NOT NULL,
  `OrganName` varchar(50) DEFAULT NULL COMMENT '機關名稱',
  `OrganNumber` varchar(50) DEFAULT NULL COMMENT '機關代號',
  `ContactPerson` varchar(30) DEFAULT NULL COMMENT '聯絡人名稱',
  `ContactPhone` varchar(30) DEFAULT NULL COMMENT '聯絡人電話',
  `ContactEmail` varchar(100) DEFAULT NULL COMMENT '連絡人信箱',
  `StartTime` datetime NOT NULL COMMENT '開始時間',
  `EndTime` datetime NOT NULL COMMENT '結束時間',
  `Account` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `basicdata_factoryaddress`
--

CREATE TABLE `basicdata_factoryaddress` (
  `Id` int(11) NOT NULL,
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號',
  `Name` varchar(50) DEFAULT NULL COMMENT '名稱',
  `Address` varchar(100) DEFAULT NULL COMMENT '地址',
  `WherePlace` varchar(50) DEFAULT NULL COMMENT '盤查哪裡',
  `ShiftMethod` varchar(30) DEFAULT NULL COMMENT '輪班方式',
  `sort` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `coefficient`
--

CREATE TABLE `coefficient` (
  `Id` int(11) NOT NULL,
  `EmissionSource` varchar(50) DEFAULT NULL COMMENT '排放源別',
  `Name` varchar(100) DEFAULT NULL COMMENT '名稱',
  `WasteMethod` varchar(50) DEFAULT NULL COMMENT '處理方法',
  `GreenhouseGases` varchar(50) DEFAULT NULL,
  `Unit` varchar(30) DEFAULT NULL COMMENT '單位',
  `Category` varchar(30) DEFAULT NULL COMMENT '範疇別',
  `CO2Coefficient` decimal(18,6) DEFAULT NULL,
  `CO2Unit` varchar(30) DEFAULT NULL,
  `CH4Coefficient` decimal(18,6) DEFAULT NULL,
  `CH4Unit` varchar(30) DEFAULT NULL,
  `CH4GWP` decimal(18,6) DEFAULT NULL,
  `N2OCoefficient` decimal(18,6) DEFAULT NULL,
  `N2OUnit` varchar(30) DEFAULT NULL,
  `N2OGWP` decimal(18,6) DEFAULT NULL,
  `HFCsGWP` decimal(18,6) DEFAULT NULL,
  `PFCsGWP` decimal(18,6) DEFAULT NULL,
  `SF6GWP` decimal(18,6) DEFAULT NULL,
  `Type` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 傾印資料表的資料 `coefficient`
--

INSERT INTO `coefficient` (`Id`, `EmissionSource`, `Name`, `WasteMethod`, `GreenhouseGases`, `Unit`, `Category`, `CO2Coefficient`, `CO2Unit`, `CH4Coefficient`, `CH4Unit`, `CH4GWP`, `N2OCoefficient`, `N2OUnit`, `N2OGWP`, `HFCsGWP`, `PFCsGWP`, `SF6GWP`, `Type`) VALUES
(1, '111', '電力', NULL, NULL, '25', '114', 0.495000, '2', 0.000000, NULL, NULL, 0.000000, '2', NULL, NULL, NULL, NULL, 'EnergyName'),
(2, '107', '天然氣', NULL, NULL, '22', '113', 1.879036, 'KgCO2/14', 0.000033, NULL, NULL, 0.000003, 'KgN2O/14', NULL, NULL, NULL, NULL, 'EnergyName'),
(3, '107', '液化石油氣', NULL, NULL, '14', '113', 1.752881, NULL, 0.000028, NULL, NULL, 0.000003, NULL, NULL, NULL, NULL, NULL, 'EnergyName'),
(4, '107', '汽油', NULL, NULL, '14', '113', 2.263133, NULL, 0.000098, NULL, NULL, 0.000020, NULL, NULL, NULL, NULL, NULL, 'EnergyName'),
(5, '107', '柴油', NULL, NULL, '14', '113', 2.606032, NULL, 0.000106, NULL, 27.900000, 0.000021, NULL, 273.000000, NULL, NULL, NULL, 'EnergyName'),
(6, '109', '車用汽油', NULL, NULL, '14', '113', 2.263133, NULL, 0.000816, NULL, 27.900000, 0.000261, NULL, 273.000000, NULL, NULL, NULL, 'EnergyName'),
(7, '109', '車用柴油', NULL, NULL, '14', '113', 2.606032, NULL, 0.000137, NULL, NULL, 0.000137, NULL, NULL, NULL, NULL, NULL, 'EnergyName'),
(11, '112', '自來水', NULL, NULL, '19', '116', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'ResourceName'),
(158, '110', 'HFC-23/R-23三氟甲烷，CHF3', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 14800.000000, NULL, NULL, 'RefrigerantName'),
(159, '110', 'HFC-32/R-32二氟甲烷，CH2F2', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 771.000000, NULL, NULL, 'RefrigerantName'),
(160, '110', 'HFC-41一氟甲烷，CH3F', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 92.000000, NULL, NULL, 'RefrigerantName'),
(161, '110', 'HFC-125/R-125，1,1,1,2,2-五氟乙烷，C2HF5', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 3500.000000, NULL, NULL, 'RefrigerantName'),
(162, '110', 'HFC-134，1,1,2,2-四氟乙烷，C2H2F4', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1100.000000, NULL, NULL, 'RefrigerantName'),
(163, '110', 'HFC-134a/R-134a，1,1,1,2-四氟乙烷，C2H2F4', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1430.000000, NULL, NULL, 'RefrigerantName'),
(164, '110', 'HFC-143，1,1,2-三氟乙烷，CHF2CH2F', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 353.000000, NULL, NULL, 'RefrigerantName'),
(165, '110', 'HFC-143a/R-143a，1,1,1-三氟乙烷，C2H3F3', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 4470.000000, NULL, NULL, 'RefrigerantName'),
(166, '110', 'HFC-152，1,2-二氟乙烷，CH2FCH2F', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 53.000000, NULL, NULL, 'RefrigerantName'),
(167, '110', 'HFC-152a/R-152a，1,1-二氟乙烷，C2H4F2', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 124.000000, NULL, NULL, 'RefrigerantName'),
(168, '110', 'HFC-161，一氟乙烷，CH3CH2F', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 12.000000, NULL, NULL, 'RefrigerantName'),
(169, '110', 'HFC-227ea，1,1,1,2,3,3,3-七氟丙烷，CF3CHFCF3', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 3220.000000, NULL, NULL, 'RefrigerantName'),
(170, '110', 'HFC-236cb，1,1,1,2,2,3-六氟丙烷，CH2FCF2CF3', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1340.000000, NULL, NULL, 'RefrigerantName'),
(171, '110', 'HFC-236ea，1,1,1,2,3,3-六氟丙烷，CHF2CHFCF3', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1370.000000, NULL, NULL, 'RefrigerantName'),
(172, '110', 'HFC-236fa，1,1,1,3,3,3-六氟丙烷，C3H2F6', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 9810.000000, NULL, NULL, 'RefrigerantName'),
(173, '110', 'HFC-245ca，1,1,2,2,3-五氟丙烷，CH2FCF2CHF2', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 693.000000, NULL, NULL, 'RefrigerantName'),
(174, '110', 'HFC-245fa，1,1,1,3,3-五氟丙烷，CHF2CH2CF3', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1030.000000, NULL, NULL, 'RefrigerantName'),
(175, '110', 'HFC-365mfc，1,1,1,3,3-五氟丁烷，CF3CH2CF2CH3', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 794.000000, NULL, NULL, 'RefrigerantName'),
(176, '110', 'HFC-43-10mee，1,1,1,2,2,3,4,5,5,5-十氟戊烷，CF3CHFCHFCF2CF3', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1640.000000, NULL, NULL, 'RefrigerantName'),
(177, '110', 'PFC-14，四氟化碳，CF4', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 7390.000000, NULL, 'RefrigerantName'),
(178, '110', 'PFC-116，六氟乙烷，C2F6', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 12200.000000, NULL, 'RefrigerantName'),
(179, '110', 'PFC-218 ，C3F8，全氟丙烷', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 8830.000000, NULL, 'RefrigerantName'),
(180, '110', 'PFC-318 。c-C4F8，八氟環丁烷', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 10300.000000, NULL, 'RefrigerantName'),
(181, '110', 'PFC-4-1-12，C5F12 (n-C5F12)，全氟戊烷', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 9160.000000, NULL, 'RefrigerantName'),
(182, '110', 'PFC-5-1-14 ，C6F14 (n-C6F14)，全氟己烷', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 9300.000000, NULL, 'RefrigerantName'),
(183, '110', 'R-401A，HCFC-22/HFC-152a/HCFC-124 (53.0/13.0/34.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(184, '110', 'R-401B，HCFC-22/HFC-152a/HCFC-124 (61.0/11.0/28.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(185, '110', 'R-401C，HCFC-22/HFC-152a/HCFC-124 (33.0/15.0/52.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(186, '110', 'R-402A，HFC-125/HC-290/HCFC-22 (60.0/2.0/38.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(187, '110', 'R-402B，HFC-125/HC-290/HCFC-22 (38.0/2.0/60.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(188, '110', 'R-403A，HC-290/HCFC-22/PFC-218 (5.0/75.0/20.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(189, '110', 'R-403B，HC-290/HCFC-22/PFC-218 (5.0/56.0/39.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(190, '110', 'R-404A，HFC-125/HFC-143a/HFC-134a (44.0/52.0/4.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(191, '110', 'R-405A，HCFC-22/ HFC-152a/ HCFC-142b/PFC-318 (45.0/7.0/5.5/42.5)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(192, '110', 'R-406A，HCFC-22/HC-600a/HCFC-142b (55.0/14.0/41.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(193, '110', 'R-407A，HFC-32/HFC-125/HFC-134a (20.0/40.0/40.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(194, '110', 'R-407B，HFC-32/HFC-125/HFC-134a (10.0/70.0/20.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(195, '110', 'R-407C，HFC-32/HFC-125/HFC-134a (23.0/25.0/52.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(196, '110', 'R-407D，HFC-32/HFC-125/HFC-134a (15.0/15.0/70.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(197, '110', 'R-407E，HFC-32/HFC-125/HFC-134a (25.0/15.0/60.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(198, '110', 'R-408A，HFC-125/HFC-143a/HCFC-22 (7.0/46.0/47.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(199, '110', 'R-409A，HCFC-22/HCFC-124/HCFC-142b (60.0/25.0/15.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(200, '110', 'R-409B，HCFC-22/HCFC-124/HCFC-142b (65.0/25.0/10.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(201, '110', 'R-410A，HFC-32/HFC-125 (50.0/50.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 2256.000000, NULL, NULL, 'RefrigerantName'),
(202, '110', 'R-410B，HFC-32/HFC-125 (45.0/55.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(203, '110', 'R-411A，HC-1270/HCFC-22/HFC-152a (1.5/87.5/11.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(204, '110', 'R-411B，HC-1270/HCFC-22/HFC-152a (3.0/94.0/3.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(205, '110', 'R-411C，HC-1270/HCFC-22/HFC-152a (3.0/95.5/1.5)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(206, '110', 'R-412A，HCFC-22/PFC-218/HCFC-142b (70.0/5.0/25.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(207, '110', 'R-413A，PFC-218/HFC-134a/HC-600a (9.0/88.0/3.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(208, '110', 'R-414A，HCFC-22/HCFC-124/HC-600a/HCFC-142b (51.0/28.5/4.0/16.5)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(209, '110', 'R-414B，HCFC-22/HCFC-124/HC-600a/HCFC-142b (50.0/39.0/1.5/9.5)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(210, '110', 'R-415A，HCFC-22/HFC-152a (82.0/18.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(211, '110', 'R-415B，HCFC-22/HFC-152a (25.0/75.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(212, '110', 'R-416A，HFC-134a/HCFC-124/HC-600 (59.0/39.5/1.5)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(213, '110', 'R-417A，HFC-125/HFC-134a/HC-600 (46.6/50.0/3.4)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(214, '110', 'R-418A，HC-290/HCFC-22/HFC-152a (1.5/96.0/2.5)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(215, '110', 'R-419A，HFC-125/HFC-134a/HE-E170 (77.0/19.0/4.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(216, '110', 'R-420A，HFC-134a/HCFC-142b (88.0/12.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(217, '110', 'R-421A，HFC-125/HFC-134a (58.0/42.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(218, '110', 'R-421B，HFC-125/HFC-134a (85.0/15.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(219, '110', 'R-422A，HFC-125/HFC-134a/HC-600a (85.1/11.5/3.4)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(220, '110', 'R-422B，HFC-125/HFC-134a/HC-600a (55.0/42.0/3.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(221, '110', 'R-422C，HFC-125/HFC-134a/HC-600a (82.0/15.0/3.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(222, '110', 'R-500，CFC-12/HFC-152a (73.8/26.2)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(223, '110', 'R-501，HCFC-22/CFC-12 (75.0/25.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(224, '110', 'R-502，HCFC-22/CFC-115 (48.8/51.2)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(225, '110', 'R-503，HFC-23/CFC-13 (40.1/59.9)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(226, '110', 'R-504，HFC-32/CFC-115 (48.2/51.8)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(227, '110', 'R-505，CFC-12/HCFC-31 (78.0/22.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(228, '110', 'R-506，CFC-31/CFC-114 (55.1/44.9)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(229, '110', 'R-507A，HFC-125/HFC-143a (50.0/50.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(230, '110', 'R-508A，HFC-23/PFC-116 (39.0/61.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(231, '110', 'R-508B，HFC-23/PFC-116 (46.0/54.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(232, '110', 'R-509A，HCFC-22/PFC-218 (44.0/56.0)', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'RefrigerantName'),
(233, '110', 'CO2滅火器', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'FireEquipmentName'),
(234, '110', 'BC型滅火器', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'FireEquipmentName'),
(235, '110', 'KBC型滅火器', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'FireEquipmentName'),
(236, '110', 'FM200', NULL, NULL, '10', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 3220.000000, NULL, NULL, 'FireEquipmentName'),
(237, '110', 'SF6氣體斷路器', NULL, NULL, '25', '113', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 22800.000000, 'FireEquipmentName'),
(238, '112', '一般廢棄物(生活垃圾)', '焚化', NULL, '10', '116', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'dumptreatment_outsourcing'),
(239, '112', '一般廢棄物(生活垃圾)', '掩埋', NULL, '10', '116', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'dumptreatment_outsourcing'),
(240, '112', '事業廢棄物', '焚化', NULL, '10', '116', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'dumptreatment_outsourcing'),
(241, '107', '事業廢棄物', '掩埋', NULL, '10', '116', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'dumptreatment_outsourcing'),
(242, '107', '事業廢棄物', '回收', NULL, '10', '116', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'dumptreatment_outsourcing'),
(243, '107', '資源回收物', '回收', NULL, '10', '116', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'dumptreatment_outsourcing'),
(244, '107', '陸運(km)', NULL, NULL, '11', '113', 1.000000, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'Transportation'),
(245, '107', '海運(km)', NULL, NULL, '11', '113', 1.000000, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'Transportation'),
(246, '107', '空運(km)', NULL, NULL, '11', '113', 1.000000, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'Transportation');

-- --------------------------------------------------------

--
-- 資料表結構 `dumptreatment_outsourcing`
--

CREATE TABLE `dumptreatment_outsourcing` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號',
  `FactoryName` varchar(100) DEFAULT NULL COMMENT '廠址',
  `WasteId` int(11) DEFAULT NULL,
  `WasteName` varchar(100) DEFAULT NULL COMMENT '空水廢名稱',
  `WasteMethod` varchar(50) DEFAULT NULL COMMENT '空水廢處理方法',
  `CleanerName` varchar(100) DEFAULT NULL COMMENT '清運商名稱',
  `FinalAddress` varchar(200) DEFAULT NULL COMMENT '最終處理地址',
  `Remark` varchar(100) DEFAULT NULL COMMENT '備註',
  `BeforeTotal` decimal(18,4) DEFAULT NULL COMMENT '總量',
  `BeforeUnit` varchar(30) DEFAULT NULL COMMENT '單位',
  `ConvertNum` decimal(18,4) DEFAULT NULL COMMENT '單位轉換輛',
  `AfertTotal` decimal(18,4) DEFAULT NULL COMMENT '總量',
  `AfertUnit` varchar(30) DEFAULT NULL COMMENT '單位',
  `DistributeRatio` decimal(18,4) DEFAULT NULL COMMENT '分配比率'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `d_datasources`
--

CREATE TABLE `d_datasources` (
  `Id` int(11) NOT NULL,
  `BindId` int(11) NOT NULL,
  `BindWhere` varchar(50) NOT NULL,
  `management` varchar(50) DEFAULT NULL COMMENT '管理單位',
  `Principal` varchar(50) DEFAULT NULL COMMENT '負責人員',
  `Datasource` varchar(50) DEFAULT NULL COMMENT '數據來源',
  `EvidenceFile` varchar(50) DEFAULT NULL COMMENT '佐證資料'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `d_intervalusetotal`
--

CREATE TABLE `d_intervalusetotal` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `BindId` int(11) NOT NULL COMMENT '綁定表id',
  `Num` varchar(50) DEFAULT NULL COMMENT '數值',
  `BindWhere` varchar(50) DEFAULT NULL COMMENT '綁定哪個表',
  `Type` varchar(20) NOT NULL COMMENT '類型',
  `ArraySort` int(11) NOT NULL COMMENT '陣列取值用'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `d_transportation`
--

CREATE TABLE `d_transportation` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `BindId` int(11) NOT NULL COMMENT '綁定表Id',
  `BindWhere` varchar(50) NOT NULL COMMENT '綁定哪個表',
  `Method` varchar(50) DEFAULT NULL COMMENT '運輸方式',
  `StartLocation` varchar(100) DEFAULT NULL COMMENT '起點',
  `EndLocation` varchar(100) DEFAULT NULL COMMENT '終點',
  `Car` varchar(11) DEFAULT NULL COMMENT '車種',
  `Tonnes` decimal(11,2) DEFAULT NULL COMMENT '噸數',
  `Fuel` varchar(50) DEFAULT NULL COMMENT '燃料',
  `Land` decimal(11,2) DEFAULT NULL COMMENT '陸運',
  `Sea` decimal(11,2) DEFAULT NULL COMMENT '海運',
  `Air` decimal(11,2) DEFAULT NULL COMMENT '空運'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `energyuse`
--

CREATE TABLE `energyuse` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號',
  `FactoryName` varchar(100) DEFAULT NULL COMMENT '廠址',
  `SupplierId` int(11) DEFAULT NULL,
  `EnergyName` varchar(20) DEFAULT NULL COMMENT '能源名稱',
  `EquipmentName` varchar(100) DEFAULT NULL COMMENT '設備名稱',
  `EquipmentLocation` varchar(100) DEFAULT NULL COMMENT '設備位置',
  `SupplierName` varchar(200) DEFAULT NULL COMMENT '供應商名稱',
  `SupplierAddress` varchar(200) DEFAULT NULL COMMENT '供應商地址',
  `Remark` varchar(100) DEFAULT NULL COMMENT '備註',
  `BeforeTotal` decimal(18,4) DEFAULT NULL COMMENT '轉換前總量',
  `BeforeUnit` varchar(30) DEFAULT NULL COMMENT '轉換前單位',
  `ConvertNum` decimal(18,4) DEFAULT NULL COMMENT '單位轉換之值',
  `AfertTotal` decimal(18,4) DEFAULT NULL COMMENT '轉換後總量',
  `AfertUnit` varchar(30) DEFAULT NULL COMMENT '轉換後單位',
  `DistributeRatio` decimal(18,4) DEFAULT NULL COMMENT '分配比率',
  `Account` varchar(200) DEFAULT NULL,
  `EditTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `evidencefilemanage`
--

CREATE TABLE `evidencefilemanage` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號',
  `ItemId` int(11) NOT NULL COMMENT '哪筆資料',
  `FileName` varchar(200) NOT NULL COMMENT '檔案名稱',
  `WhereForm` varchar(50) NOT NULL COMMENT '哪個表的檔案',
  `Time` datetime NOT NULL COMMENT '時間'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `fireequipment`
--

CREATE TABLE `fireequipment` (
  `Id` int(11) NOT NULL COMMENT '主鍵	',
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號	',
  `FactoryName` varchar(100) DEFAULT NULL COMMENT '廠址	',
  `EquipmentName` varchar(100) DEFAULT NULL COMMENT '設備名稱',
  `GHGType` varchar(50) DEFAULT NULL COMMENT '溫室氣體種類',
  `Remark` varchar(100) DEFAULT NULL COMMENT '備註',
  `BeforeTotal` decimal(18,4) DEFAULT NULL COMMENT '轉換前總量',
  `BeforeUnit` varchar(30) DEFAULT NULL COMMENT '轉換前單位',
  `ConvertNum` decimal(18,4) DEFAULT NULL COMMENT '單位轉換之值',
  `AfertTotal` decimal(18,4) DEFAULT NULL COMMENT '轉換後總量',
  `AfertUnit` varchar(30) DEFAULT NULL COMMENT '轉換後單位',
  `DistributeRatio` decimal(18,4) DEFAULT NULL COMMENT '分配比率'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `log`
--

CREATE TABLE `log` (
  `Id` int(11) NOT NULL,
  `WhereFunction` varchar(100) DEFAULT NULL COMMENT '哪個方法發生錯誤',
  `Exception` mediumtext DEFAULT NULL COMMENT '錯誤內容',
  `DateTime` datetime DEFAULT NULL COMMENT '時間'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `login`
--

CREATE TABLE `login` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `Account` varchar(200) NOT NULL COMMENT '帳號',
  `State` varchar(2) NOT NULL COMMENT '狀態',
  `LoginDate` datetime NOT NULL COMMENT '時間',
  `LoginFailures` int(20) NOT NULL COMMENT '登入失敗次數'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `member`
--

CREATE TABLE `member` (
  `MemberId` int(11) NOT NULL COMMENT '主鍵',
  `Account` varchar(200) NOT NULL COMMENT '帳號',
  `Passwd` varchar(50) DEFAULT NULL COMMENT '密碼',
  `CompanyName` varchar(100) DEFAULT NULL COMMENT '公司名稱',
  `Department` varchar(100) DEFAULT NULL COMMENT '部門',
  `OfficeLocation` varchar(50) DEFAULT NULL COMMENT '科別',
  `Position` varchar(50) DEFAULT NULL COMMENT '職稱',
  `Name` varchar(50) DEFAULT NULL COMMENT '名稱',
  `permissions` varchar(10) DEFAULT NULL COMMENT '權限',
  `Addr` varchar(200) DEFAULT NULL COMMENT '聯絡地址',
  `Tel` varchar(20) DEFAULT NULL COMMENT '聯絡電話',
  `Email` varchar(50) DEFAULT NULL COMMENT '電子信箱',
  `UserType` varchar(10) DEFAULT NULL COMMENT '用戶類型',
  `LoginType` varchar(20) NOT NULL COMMENT '登入類型'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `organize`
--

CREATE TABLE `organize` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `BasicId` int(11) NOT NULL,
  `Inventory` varchar(50) NOT NULL COMMENT '盤查表',
  `StartTime` datetime DEFAULT NULL COMMENT '盤查開始時間',
  `EndTime` datetime DEFAULT NULL COMMENT '盤查結束時間',
  `ContactName` varchar(50) DEFAULT NULL COMMENT '聯絡窗口',
  `UpdateTime` datetime DEFAULT NULL COMMENT '更新時間',
  `Status` varchar(50) DEFAULT NULL COMMENT '狀態',
  `Account` varchar(200) DEFAULT NULL COMMENT '帳號綁定',
  `EditLog` mediumtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `passwordhistory`
--

CREATE TABLE `passwordhistory` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `Account` varchar(200) NOT NULL COMMENT '帳號',
  `Password` varchar(50) NOT NULL COMMENT '密碼',
  `CreateTime` datetime NOT NULL COMMENT '創建時間',
  `Email` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `refrigerant_have`
--

CREATE TABLE `refrigerant_have` (
  `Id` int(11) NOT NULL,
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號',
  `FactoryName` varchar(100) DEFAULT NULL COMMENT '廠址',
  `EquipmentName` varchar(100) DEFAULT NULL COMMENT '設備名稱',
  `EquipmentType` varchar(30) DEFAULT NULL COMMENT '設備類型',
  `RefrigerantType` varchar(50) DEFAULT NULL COMMENT '冷媒種類',
  `Management` varchar(50) DEFAULT NULL COMMENT '管理單位',
  `Principal` varchar(50) DEFAULT NULL COMMENT '負責人員',
  `Datasource` varchar(50) DEFAULT NULL COMMENT '數據來源',
  `Remark` varchar(100) DEFAULT NULL COMMENT '備註',
  `Total` decimal(18,6) DEFAULT NULL COMMENT '總量',
  `Unit` varchar(30) DEFAULT NULL COMMENT '單位',
  `DistributeRatio` decimal(18,4) DEFAULT NULL COMMENT '分配比率'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `refrigerant_none`
--

CREATE TABLE `refrigerant_none` (
  `Id` int(11) NOT NULL,
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號',
  `FactoryName` varchar(100) DEFAULT NULL COMMENT '廠址',
  `EquipmentName` varchar(100) DEFAULT NULL COMMENT '設備名稱',
  `EquipmentType` varchar(30) DEFAULT NULL COMMENT '設備類型',
  `EquipmentLocation` varchar(100) DEFAULT NULL COMMENT '設備位置',
  `RefrigerantType` varchar(50) DEFAULT NULL COMMENT '冷媒種類',
  `MachineQuantity` int(11) DEFAULT NULL COMMENT '機台數量',
  `Manufacturers` varchar(100) DEFAULT NULL COMMENT '廠商or廠牌',
  `Management` varchar(50) DEFAULT NULL COMMENT '管理單位',
  `Principal` varchar(50) DEFAULT NULL COMMENT '負責人員',
  `Remark` varchar(100) DEFAULT NULL COMMENT '備註',
  `RefrigerantWeight` decimal(18,6) DEFAULT NULL COMMENT '冷媒重量or容量',
  `Unit` varchar(30) DEFAULT NULL COMMENT '單位',
  `DistributeRatio` decimal(18,4) DEFAULT NULL COMMENT '分配比率'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `resourceuse`
--

CREATE TABLE `resourceuse` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號',
  `FactoryName` varchar(100) DEFAULT NULL COMMENT '廠址',
  `EnergyName` varchar(20) DEFAULT NULL COMMENT '能源名稱	',
  `EquipmentName` varchar(100) DEFAULT NULL COMMENT '設備名稱	',
  `EquipmentLocation` varchar(100) DEFAULT NULL COMMENT '設備位置	',
  `SupplierId` int(11) DEFAULT NULL,
  `SupplierName` varchar(200) DEFAULT NULL COMMENT '供應商名稱	',
  `SupplierAddress` varchar(200) DEFAULT NULL COMMENT '供應商地址	',
  `Remark` varchar(100) DEFAULT NULL COMMENT '備註	',
  `BeforeTotal` decimal(18,4) DEFAULT NULL COMMENT '總量	',
  `BeforeUnit` varchar(30) DEFAULT NULL COMMENT '單位	',
  `ConvertNum` decimal(18,4) DEFAULT NULL COMMENT '單位轉換量	',
  `AfertTotal` decimal(18,4) DEFAULT NULL COMMENT '總量	',
  `AfertUnit` varchar(30) DEFAULT NULL COMMENT '單位	',
  `DistributeRatio` decimal(18,4) DEFAULT NULL COMMENT '分配比率'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `selectdata`
--

CREATE TABLE `selectdata` (
  `Id` int(11) NOT NULL,
  `Code` varchar(10) NOT NULL COMMENT '編號',
  `Type` varchar(50) DEFAULT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `Sort` int(11) NOT NULL COMMENT '排序'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- 傾印資料表的資料 `selectdata`
--

INSERT INTO `selectdata` (`Id`, `Code`, `Type`, `Name`, `Sort`) VALUES
(1, '1', 'Unit', '毫米(mm)', 16),
(2, '2', 'Unit', '公分(cm)', 18),
(3, '3', 'Unit', '公尺(m)', 20),
(4, '4', 'Unit', '公里(km)', 22),
(5, '5', 'Unit', '海浬(nm)', 24),
(6, '6', 'Unit', '英寸(in)', 26),
(7, '7', 'Unit', '碼(yard)', 28),
(8, '8', 'Unit', '毫克(mg)', 3),
(9, '9', 'Unit', '公克(g)', 4),
(10, '10', 'Unit', '公斤(kg)', 6),
(11, '11', 'Unit', '公噸(mt)', 8),
(12, '12', 'Unit', '英磅(lb)', 9),
(13, '13', 'Unit', '毫升(ml)', 10),
(14, '14', 'Unit', '公升(L)', 12),
(15, '15', 'Unit', '公秉(kl)', 14),
(16, '16', 'Unit', '平方毫米(mm2)', 100),
(17, '17', 'Unit', '平方公分(cm2)', 100),
(18, '18', 'Unit', '平方公尺(m2)', 100),
(19, '19', 'Unit', '平方公里(km2)', 100),
(20, '20', 'Unit', '立方毫米(mm3)', 100),
(21, '21', 'Unit', '立方公分(cm3)', 100),
(22, '22', 'Unit', '立方公尺(m3)', 100),
(23, '23', 'Unit', '立方公里(km3)', 100),
(24, '24', 'Unit', '百萬焦耳(MJ)', 100),
(25, '25', 'Unit', '度(kwh)', 2),
(26, '26', 'Unit', '延人公里(pkm)', 100),
(27, '27', 'Unit', '延噸公里(tkm)', 100),
(28, '28', 'Unit', 'g CO2e', 100),
(29, '29', 'Unit', 'kg CO2e', 100),
(30, '30', 'Unit', '每平方米‧每小時', 100),
(31, '31', 'Unit', '每人‧每小時', 100),
(32, '32', 'Unit', '每人', 100),
(33, '33', 'Unit', '每人次', 100),
(34, '34', 'Unit', '每房-每天', 100),
(35, '35', 'Unit', '片', 100),
(36, '36', 'Unit', '顆', 100),
(37, '37', 'Unit', '個', 100),
(38, '38', 'Unit', '條', 100),
(39, '39', 'Unit', '卷', 100),
(40, '40', 'Unit', '瓶', 100),
(41, '41', 'Unit', '桶', 100),
(42, '42', 'Unit', '盒', 100),
(43, '43', 'Unit', '包', 100),
(44, '44', 'Unit', '罐', 100),
(45, '45', 'Unit', '台', 100),
(46, '46', 'Unit', '雙', 100),
(47, '47', 'Unit', '張', 100),
(48, '48', 'Verify', '無', 0),
(49, '49', 'Verify', '環保署關鍵性審查', 0),
(50, '50', 'Verify', '香港商英國標準協會太平洋有限公司台灣分公司(BSI)', 0),
(51, '51', 'Verify', '台灣德國萊因技術監護股份有限公(TUV)', 0),
(52, '52', 'Verify', '台灣衛理國際品保驗證股份有限公司(BV)', 0),
(53, '53', 'Verify', '台灣檢驗科技股份有限公司(SGS)', 0),
(54, '54', 'Verify', '英商勞氏檢驗股份有限公司台灣分公司(IRQA)', 0),
(55, '55', 'Verify', '優力國際安全認證有限公司(UL)', 0),
(56, '56', 'Verify', '艾法諾國際股份有限公司(AFNOR)', 0),
(57, '57', 'Verify', '立恩威國際驗證股份有限公司(DNV GL)', 0),
(58, '58', 'Transportation', '陸運', 0),
(59, '59', 'Transportation', '海運(含陸運)', 0),
(60, '60', 'Transportation', '空運(含陸運)', 0),
(61, '61', 'Transportation', '管線', 0),
(63, '62', 'Transportation', '其他', 0),
(64, '63', 'Car', '大貨車', 0),
(65, '64', 'Car', '小貨車', 0),
(66, '65', 'EnergyType', '外購能源', 2),
(67, '66', 'EnergyType', '燃料', 4),
(68, '67', 'DistributeAccordance', '產量', 2),
(69, '68', 'DistributeAccordance', '工時', 4),
(70, '69', 'DistributeAccordance', '採購量', 6),
(71, '70', 'DistributeAccordance', '自訂', 8),
(72, '71', 'EquipmentType', '家用冷凍/冷藏裝備', 2),
(73, '72', 'EquipmentType', '獨立商用冷凍/冷藏裝備', 4),
(74, '73', 'EquipmentType', '中/大型冷凍/冷藏裝備交通用冷凍/冷藏裝備', 6),
(76, '75', 'EquipmentType', '冰水機', 10),
(77, '76', 'EquipmentType', '住宅及商業建築冷氣機(含熱泵)', 12),
(78, '77', 'EquipmentType', '移動式空氣清靜機', 14),
(79, '78', 'FactoryShift', '單班', 2),
(80, '79', 'FactoryShift', '2班', 4),
(81, '80', 'FactoryShift', '3班', 6),
(82, '81', 'DealWithMethod', '厭氧處理', 2),
(83, '82', 'DealWithMethod', '好氧處理', 4),
(84, '83', 'EnergyName', '天然氣', 2),
(85, '84', 'EnergyName', '液化石油氣', 4),
(86, '85', 'EnergyName', '汽油', 6),
(87, '89', 'EnergyName', '柴油', 8),
(88, '86', 'EnergyName', '車用汽油', 10),
(89, '87', 'EnergyName', '車用柴油', 12),
(90, '88', 'EnergyName', '電力', 0),
(91, '90', 'ResourceName', '水', 2),
(92, '91', 'Refrigerant', 'R11', 0),
(93, '92', 'Refrigerant', 'R12', 2),
(94, '93', 'Refrigerant', 'R22', 4),
(95, '94', 'Refrigerant', 'R32', 6),
(96, '95', 'FireEquipment', 'CO2滅火器(5kg)', 2),
(97, '96', 'FireEquipment', 'CO2滅火器(24kg)', 4),
(98, '97', 'FireEquipment', 'ABC型乾粉滅火器(2kg)', 6),
(99, '98', 'FireEquipment', 'ABC型乾粉滅火器(4kg)', 8),
(100, '99', 'FireEquipment', 'ABC型乾粉滅火器(8kg)', 10),
(101, '100', 'FireEquipment', 'ABC型乾粉滅火器(35kg)', 12),
(102, '101', 'WasteMethod', '焚化', 2),
(103, '102', 'WasteMethod', '掩埋', 4),
(104, '103', 'WasteMethod', '回收', 6),
(105, '104', 'WasteName', '一般廢棄物(生活垃圾)', 2),
(106, '105', 'WasteName', '事業廢棄物', 4),
(107, '106', 'WasteName', '資源回收物(只能選回收)', 6),
(108, '107', 'EmissionSource', '固定排放	', 2),
(109, '108', 'EmissionSource', '製程排放', 4),
(110, '109', 'EmissionSource', '移動排放', 6),
(111, '110', 'EmissionSource', '逸散排放', 8),
(112, '111', 'EmissionSource', '能源間接排放', 10),
(113, '112', 'EmissionSource', '其他間接排放', 12),
(114, '113', 'Category', '類別1', 2),
(115, '114', 'Category', '類別2', 4),
(116, '115', 'Category', '類別3', 6),
(117, '116', 'Category', '類別4', 8),
(118, '117', 'Unit', '千度(mwh)', 2),
(119, '118', 'GHGType', 'CO2', 2),
(120, '119', 'GHGType', 'CH4', 4),
(121, '120', 'GHGType', 'N2O', 6),
(122, '121', 'GHGType', 'HFCs', 8),
(123, '122', 'GHGType', 'PFCs', 10),
(124, '123', 'GHGType', 'SF6', 12),
(125, '124', 'GHGType', 'NF3	', 14);

-- --------------------------------------------------------

--
-- 資料表結構 `suppliermanage`
--

CREATE TABLE `suppliermanage` (
  `Id` int(11) NOT NULL,
  `Account` varchar(200) DEFAULT NULL COMMENT '帳號',
  `SupplierName` varchar(100) DEFAULT NULL COMMENT '供應商名稱',
  `SupplierAddress` varchar(200) DEFAULT NULL COMMENT '供應商地址'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 資料表結構 `verify_code`
--

CREATE TABLE `verify_code` (
  `Id` int(11) NOT NULL COMMENT '主鍵',
  `Number` varchar(50) NOT NULL COMMENT '驗證號碼',
  `VerifyCode` varchar(50) NOT NULL COMMENT '驗證碼',
  `ExpireTime` datetime DEFAULT NULL COMMENT '過期時間',
  `IsVerify` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否已驗證',
  `VerifyTime` datetime DEFAULT NULL COMMENT '驗證時間',
  `CreateTime` datetime DEFAULT NULL COMMENT '創建時間'
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci COMMENT='驗證表';

-- --------------------------------------------------------

--
-- 資料表結構 `workinghours`
--

CREATE TABLE `workinghours` (
  `Id` int(11) NOT NULL,
  `BasicId` int(11) NOT NULL COMMENT '基本資料編號',
  `FactoryId` int(11) NOT NULL COMMENT '工廠id',
  `Item` varchar(30) DEFAULT NULL COMMENT '項目',
  `TotalWorkHour` decimal(18,2) DEFAULT NULL COMMENT '總工時',
  `management` varchar(50) DEFAULT NULL COMMENT '管理部門',
  `Principal` varchar(50) DEFAULT NULL COMMENT '負責人員',
  `Datasource` varchar(50) DEFAULT NULL COMMENT '數據來源',
  `DistributeRatio` decimal(18,4) DEFAULT NULL COMMENT '分配方式'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 已傾印資料表的索引
--

--
-- 資料表索引 `basicdatas`
--
ALTER TABLE `basicdatas`
  ADD PRIMARY KEY (`BasicId`);

--
-- 資料表索引 `basicdata_factoryaddress`
--
ALTER TABLE `basicdata_factoryaddress`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `coefficient`
--
ALTER TABLE `coefficient`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `dumptreatment_outsourcing`
--
ALTER TABLE `dumptreatment_outsourcing`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `d_datasources`
--
ALTER TABLE `d_datasources`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `d_intervalusetotal`
--
ALTER TABLE `d_intervalusetotal`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `d_transportation`
--
ALTER TABLE `d_transportation`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `energyuse`
--
ALTER TABLE `energyuse`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `evidencefilemanage`
--
ALTER TABLE `evidencefilemanage`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `fireequipment`
--
ALTER TABLE `fireequipment`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `log`
--
ALTER TABLE `log`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `login`
--
ALTER TABLE `login`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `member`
--
ALTER TABLE `member`
  ADD PRIMARY KEY (`MemberId`);

--
-- 資料表索引 `organize`
--
ALTER TABLE `organize`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `passwordhistory`
--
ALTER TABLE `passwordhistory`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `refrigerant_have`
--
ALTER TABLE `refrigerant_have`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `refrigerant_none`
--
ALTER TABLE `refrigerant_none`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `resourceuse`
--
ALTER TABLE `resourceuse`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `selectdata`
--
ALTER TABLE `selectdata`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `suppliermanage`
--
ALTER TABLE `suppliermanage`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `verify_code`
--
ALTER TABLE `verify_code`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `workinghours`
--
ALTER TABLE `workinghours`
  ADD PRIMARY KEY (`Id`);

--
-- 在傾印的資料表使用自動遞增(AUTO_INCREMENT)
--

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `basicdatas`
--
ALTER TABLE `basicdatas`
  MODIFY `BasicId` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `basicdata_factoryaddress`
--
ALTER TABLE `basicdata_factoryaddress`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `coefficient`
--
ALTER TABLE `coefficient`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=247;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `dumptreatment_outsourcing`
--
ALTER TABLE `dumptreatment_outsourcing`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `d_datasources`
--
ALTER TABLE `d_datasources`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `d_intervalusetotal`
--
ALTER TABLE `d_intervalusetotal`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `d_transportation`
--
ALTER TABLE `d_transportation`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `energyuse`
--
ALTER TABLE `energyuse`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `evidencefilemanage`
--
ALTER TABLE `evidencefilemanage`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `fireequipment`
--
ALTER TABLE `fireequipment`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵	';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `log`
--
ALTER TABLE `log`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `login`
--
ALTER TABLE `login`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `member`
--
ALTER TABLE `member`
  MODIFY `MemberId` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `organize`
--
ALTER TABLE `organize`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `passwordhistory`
--
ALTER TABLE `passwordhistory`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `refrigerant_have`
--
ALTER TABLE `refrigerant_have`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `refrigerant_none`
--
ALTER TABLE `refrigerant_none`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `resourceuse`
--
ALTER TABLE `resourceuse`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `selectdata`
--
ALTER TABLE `selectdata`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=126;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `suppliermanage`
--
ALTER TABLE `suppliermanage`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `verify_code`
--
ALTER TABLE `verify_code`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主鍵';

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `workinghours`
--
ALTER TABLE `workinghours`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
