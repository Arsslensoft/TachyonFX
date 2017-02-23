<?php
// include Controllers
include(ROOT_DIR . "/Core/Autoloader.php");
spl_autoload_register ( 'loadDependency' );

include (ROOT_DIR."/Config/config.inc.php");


include(ROOT_DIR . "/Core/Controllers/BaseController.php");
include(ROOT_DIR . "/Core/Controllers/ModelBaseController.php");
include(ROOT_DIR . "/Core/Controllers/AccessController.php");
/*
 * // include Core
include ($path."/../Core/Utils.php");
include ($path."/../Core/ErrorHandler.php");
// include Config


// include Routes
include ($path."/Routes.php");
*/



?>