<?php
namespace Core\Cache;
use Core\Cache\Drivers\FileCacheDriver;

class CacheManager
{

    private static $instance;
    public static function getInstance($driver = null)
    {
        if(self::$instance == null)
        {
            $instance = new CacheManager($driver);
            return $instance;
        }

        return self::$instance;
    }


    private $cache_driver, $is_caching;
    public function __construct($cache_driver = null)
    {
        if($cache_driver != null)
            $this->cache_driver = $cache_driver;
        else $this->cache_driver = new FileCacheDriver();
    }



    public function startCaching($id, $force = true){
        $current_cache = null;

        if($force)
            $current_cache = $this->get($id);


        if($current_cache == null)
        {
            $this->is_caching = true;
            ob_start();
            return null;
        }
        else {
            $this->is_caching = false;
            return $current_cache;
        }

    }
    public function printAndSave($id, $ttl = 60){
        if($this->is_caching){
            $view_buffer = ob_get_contents();
            ob_end_clean();
            $this->save($id, $view_buffer,$ttl);
            echo $view_buffer ;
            return true;
        }
        else{
            $this->is_caching=false;
            return false;
        }
    }

    public function isCaching(){
        return $this->is_caching;
    }
    public function get($id){
        return   $this->cache_driver->get($id);
    }
    public function save($id,$data, $ttl = 60){
        return   $this->cache_driver->save($id, $data, $ttl);
    }
    public function delete($id){
        return   $this->cache_driver->delete($id);
    }
    public function clean(){
        return   $this->cache_driver->clean();
    }
}