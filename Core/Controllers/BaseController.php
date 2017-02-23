<?php
namespace Core\Controllers;
use Core\Cache\CacheManager;

class BaseController
{
        public  $RouteController,
                $AccessController,
                $CacheManager;

        public function __construct()
        {
            $this->RouteController = RouteController::getInstance();
            $this->AccessController = AccessController::getInstance();
            $this->CacheManager = CacheManager::getInstance();
        }

        public function checkAuth(){
            $auth = AuthController::getInstance();
            if(!$auth->logged_in){
                $_SESSION["redirect"] = $_SERVER['REQUEST_URI'];
                header("Location: ".$GLOBALS["home_url"]."login");
            }

        }
        public function view($view_name, $parameters = null, $cache = null ){
            $view_name = str_replace(".","/", $view_name);

            // define default variables
            $auth = AuthController::getInstance();
            $user  = AuthController::getCurrentUser();

                // define variables
            if(isset($parameters))
                foreach ($parameters as $parameter => $value)
                    $$parameter = $value;

            $cache_enabled = false;
            $cache_id = null;
            $cache_ttl = 60;
            if($cache != null && is_callable( $cache ))
                $cache_enabled = $cache($cache_id, $cache_ttl);



            if($cache_enabled){


               $cached_buffer = $this->CacheManager->startCaching($cache_id);

                if($cached_buffer != null)
                    echo $cached_buffer;
                else
                    include ROOT_DIR."/Views/$view_name.pxt.php";

            if($cached_buffer == null && $this->CacheManager->isCaching())
                $this->CacheManager->printAndSave($cache_id, $cache_ttl);


            }
        }

        public function viewClean($view_name, $parameters = null, $cache = null){
        $view_name = str_replace(".","/", $view_name);


        // define variables
        if(isset($parameters))
            foreach ($parameters as $parameter => $value)
                $$parameter = $value;


            $cache_enabled = false;
            $cache_id = null;
            $cache_ttl = 60;
            if($cache != null && is_callable( $cache ))
                $cache_enabled = $cache($cache_id, $cache_ttl);



            if($cache_enabled){


                $cached_buffer = $this->CacheManager->startCaching($cache_id);

                if($cached_buffer != null)
                    echo $cached_buffer;
                else
                    include ROOT_DIR."/Views/$view_name.pxt.php";

                if($cached_buffer == null && $this->CacheManager->isCaching())
                    $this->CacheManager->printAndSave($cache_id, $cache_ttl);


            }

    }

        public static function viewError($ex){
            $exception = $ex;
            include ROOT_DIR."/Views/error.pxt.php";
        }
}