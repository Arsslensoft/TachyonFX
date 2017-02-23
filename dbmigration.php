<?php
define("ROOT_DIR", dirname(__FILE__));
include "Config/Init.inc.php";

\Models\User::createTable();
\Models\UserSession::createTable();

echo "Database exported";
