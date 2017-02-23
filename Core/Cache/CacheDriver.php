<?php
namespace Core\Cache;

abstract class CacheDriver
{
   public  abstract function get($id);
   public  abstract function save($id, $data,  $ttl = 60);
   public  abstract function delete($id);
   public abstract function clean();
}