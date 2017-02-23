<?php
define("ROOT_DIR", dirname(__FILE__));

include "Config/Init.inc.php";

/*
var_dump(\Models\User::sql("SELECT * FROM :table WHERE Username=? OR Id=?", ["arsslen", 1]));
try{

  /*  $router = \Core\Controllers\RouteController::getInstance();
// match current request url
    $match = $router->match();

// call closure or throw 404 status
    if( $match && is_callable( $match['target'] ) ) {
        call_user_func_array( $match['target'], $match['params'] );
    } else {
        // no route was matched
        header( $_SERVER["SERVER_PROTOCOL"] . ' 404 Not Found');
    }
}
catch (Exception $ex){
    \Core\ErrorHandler::logException($ex);
    \Core\Controllers\BaseController::viewError($ex);
}

*/