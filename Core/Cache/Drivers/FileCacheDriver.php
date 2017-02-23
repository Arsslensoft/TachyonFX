<?php

namespace Core\Cache\Drivers;
use Core\Cache\CacheDriver;

class FileCacheDriver extends CacheDriver
{
    private $cache_extension = ".tcache";

    private function writeFile($path, $data, $mode = 'wb')
    {
        if ( ! $fp = @fopen($path, $mode))
        {
            return FALSE;
        }

        flock($fp, LOCK_EX);

        for ($result = $written = 0, $length = strlen($data); $written < $length; $written += $result)
        {
            if (($result = fwrite($fp, substr($data, $written))) === FALSE)
            {
                break;
            }
        }

        flock($fp, LOCK_UN);
        fclose($fp);

        return is_int($result);
    }

    public   function get($id){
        $cache_file = $GLOBALS["tachyon_config"]["cache_dir"] .$id.$this->cache_extension;
            if(file_exists($cache_file))
            {
                $cache_data = unserialize(file_get_contents($cache_file));

                if (is_array($cache_data))
                {

                    if ( ! isset($cache_data['ttl'], $cache_data['time']))
                        return null;

                   if(time() >= ($cache_data['time'] + $cache_data['ttl']))
                       return  $cache_data['data'];
                   else return null;


                }

                return null;
            }
            else return null;
    }

    public   function save($id, $data, $ttl = 60){
        $cache_contents = array(
            'time'		=> time(),
            'ttl'		=> $ttl,
            'data'		=> $data
        );


        $cache_file = $GLOBALS["tachyon_config"]["cache_dir"] .$id.$this->cache_extension;
        if ($this->writeFile($cache_file, serialize($cache_contents)))
        {
            chmod($cache_file, 0640);
            return TRUE;
        }

        return FALSE;

    }
    public   function delete($id){
        $cache_file = $GLOBALS["tachyon_config"]["cache_dir"] .$id.$this->cache_extension;
        return is_file($cache_file) ? unlink($cache_file) : FALSE;
    }
    public  function clean(){

        $files = glob($GLOBALS["tachyon_config"]["cache_dir"] . "/*".$this->cache_extension);

        //interate thorugh the files and folders
        foreach($files as $file){
            if(is_file($file) )
                unlink($file);
            else return false;

        }
        return true;
    }
}