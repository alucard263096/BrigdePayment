CREATE DATABASE  IF NOT EXISTS `alucard263096_bridgepayment` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `alucard263096_bridgepayment`;
-- MySQL dump 10.13  Distrib 5.7.9, for Win64 (x86_64)
--
-- Host: localhost    Database: alucard263096_bridgepayment
-- ------------------------------------------------------
-- Server version	5.7.10-enterprise-commercial-advanced-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `tb_notify_log`
--

DROP TABLE IF EXISTS `tb_notify_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tb_notify_log` (
  `id` int(11) NOT NULL,
  `created_date` datetime DEFAULT NULL,
  `created_user` int(11) DEFAULT NULL,
  `updated_date` datetime DEFAULT NULL,
  `updated_user` int(11) DEFAULT NULL,
  `notify_time` datetime DEFAULT NULL COMMENT 'notify_time.',
  `log_request` varchar(4000) DEFAULT NULL COMMENT 'log_request.',
  `log_url` varchar(4000) DEFAULT NULL COMMENT 'log_url.',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tb_notify_log`
--

LOCK TABLES `tb_notify_log` WRITE;
/*!40000 ALTER TABLE `tb_notify_log` DISABLE KEYS */;
INSERT INTO `tb_notify_log` VALUES (1,'2017-05-08 16:31:24',-2,'2017-05-08 16:31:24',-2,'2017-05-08 16:31:21','[]',NULL),(2,'2017-05-08 16:33:04',-2,'2017-05-08 16:33:04',-2,'2017-05-08 16:32:37','[]',NULL);
/*!40000 ALTER TABLE `tb_notify_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tb_payment`
--

DROP TABLE IF EXISTS `tb_payment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tb_payment` (
  `id` int(11) NOT NULL,
  `created_date` datetime DEFAULT NULL,
  `created_user` int(11) DEFAULT NULL,
  `updated_date` datetime DEFAULT NULL,
  `updated_user` int(11) DEFAULT NULL,
  `orderno` varchar(255) DEFAULT NULL COMMENT 'orderno.',
  `time_end` varchar(255) DEFAULT NULL COMMENT 'time_end.',
  `openid` varchar(255) DEFAULT NULL COMMENT 'openid.',
  `total_fee` decimal(12,2) DEFAULT NULL COMMENT 'total_fee.',
  `trade_type` varchar(255) DEFAULT NULL COMMENT 'trade_type.',
  `transaction_id` varchar(255) DEFAULT NULL COMMENT 'transaction_id.',
  `result_code` varchar(255) DEFAULT NULL COMMENT 'result_code.',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tb_payment`
--

LOCK TABLES `tb_payment` WRITE;
/*!40000 ALTER TABLE `tb_payment` DISABLE KEYS */;
/*!40000 ALTER TABLE `tb_payment` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-05-08 17:10:22
