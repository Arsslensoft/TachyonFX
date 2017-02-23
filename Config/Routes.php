<?php
$router = new Core\Controllers\RouteController();


$router->map( 'GET|POST', $GLOBALS["tachyon_config"]["platform_directory"].'login', array(\Core\Controllers\AuthController::getInstance(),"index") );
$router->map( 'GET', $GLOBALS["tachyon_config"]["platform_directory"].'logout', array(\Core\Controllers\AuthController::getInstance(),"logout") );
?>