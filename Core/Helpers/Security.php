<?php
namespace Core\Helpers;

class Security
{
    //CSRF Protection
   public static function getCSRFToken(&$csrf_name, &$csrf_token, $expire = 7200){
     $csrf_token =  base64_encode(openssl_random_pseudo_bytes(32));
     $csrf_name = "token_" . md5($csrf_token);

       setcookie(
           $csrf_name,
           $csrf_token,
           $expire,
           $GLOBALS["tachyon_config"]["secure_cookies"]["cookie_path"],
           $GLOBALS["tachyon_config"]["secure_cookies"]["cookie_domain"],
           $GLOBALS["tachyon_config"]["secure_cookies"],
           $GLOBALS["tachyon_config"]["http_only"]
       );
   }
   public static function validateCSRFToken($csrf_name, $csrf_token){
       if(isset($_COOKIE[$csrf_name])){
           $cookie_csrf_token = $_COOKIE[$csrf_name];
           unset($_COOKIE[$csrf_name]);

           return $cookie_csrf_token == $csrf_token;
       }
       return false;

   }
}