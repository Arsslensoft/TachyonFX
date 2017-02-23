<?php
namespace Core\Controllers;


class AccessController
{

    private static $instance;
    public static function getInstance()
    {
        if(self::$instance == null)
        {
            $instance = new AccessController();
            return $instance;
        }

        return self::$instance;
    }


    public $RoleFeatureList;
    public $PermissionList;
    public function __construct()
    {
        $this->RoleFeatureList = array(

        );

        $this->PermissionList = array(

        );
    }


    public function canPerfomAction($privilges, $requested_privileges)
    {
        return true;
    }
    public function isAccessGrantedForViewAction($user, $view, $action)
    {

        $view_action = $view."/".$action;
        if(isset($this->PermissionList[$view_action]))
        {
            $requested_features = $this->PermissionList[$view_action];


            if(isset($this->RoleFeatureList[$user->Role]))
                return ($this->RoleFeatureList[$user->Role] & $requested_features) == $requested_features;
            else return false;
        }
        else return false;
    }
}