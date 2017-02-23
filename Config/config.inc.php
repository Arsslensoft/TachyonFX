<?php

$GLOBALS["tachyon_config"] = array();

	/**
	* Configuration file
	*
	* This should be the only file you need to edit in, regarding the original script.
	* Please provide your MySQL login information below.
	*/
$GLOBALS["tachyon_config"]["mysql_hostname"] = "localhost";
$GLOBALS["tachyon_config"]["mysql_username"] = "root";
$GLOBALS["tachyon_config"]["mysql_password"] = "";
$GLOBALS["tachyon_config"]["mysql_database"] = "gmcp";
$GLOBALS["tachyon_config"]["platform_directory"] = "/gmc/gmcp/";
$GLOBALS["tachyon_config"]["platform_host"] = "localhost";
$GLOBALS["tachyon_config"]["session_name"] = "TACHYON";
$GLOBALS["tachyon_config"]["data_directory"] = "/gmc/gmcp/data/";
$GLOBALS["tachyon_config"]["protocol"] = "http";
$GLOBALS["tachyon_config"]["data_url"] = $GLOBALS["tachyon_config"]["protocol"]."://".$GLOBALS["tachyon_config"]["platform_host"] .$GLOBALS["tachyon_config"]["data_directory"];
$GLOBALS["tachyon_config"]["home_url"] = $GLOBALS["tachyon_config"]["protocol"]."://".$GLOBALS["tachyon_config"]["platform_host"].$GLOBALS["tachyon_config"]["platform_directory"];


$GLOBALS["tachyon_config"]["log"] = true;
$GLOBALS["tachyon_config"]["exception_page"] = true;

// cookies config
$GLOBALS["tachyon_config"]["http_only"] = true;
$GLOBALS["tachyon_config"]["secure_cookies"] = true;
$GLOBALS["tachyon_config"]["cookie_path"] = "/tachyon/";
$GLOBALS["tachyon_config"]["cookie_domain"] = "localhost";
// caching
$GLOBALS["tachyon_config"]["cache"] = true;
$GLOBALS["tachyon_config"]["cache_dir"] = $GLOBALS["tachyon_config"]["data_directory"]."/cache/";

// Helpers
