<?php

namespace Core;
/**
 * Class ParameterDataType, defines external input data type
 */
class ParameterDataType extends Enum{
    const __default = self::String;
    const Integer = 0;
    const String = 1;
    const Real = 2;
    const Boolean = 3;
    public static function getName($val){
        if($val == ParameterDataType::Integer)
            return "integer";
        else if($val == ParameterDataType::Real)
            return "float";
        else if($val == ParameterDataType::Boolean)
            return "boolean";
        else return "string";
    }

}

class Utils
{
    /**
     * Checks wether a value is valid
     * @param $val
     * @param $data_type
     * @return bool
     */
    public static function isValidDataType($val, $data_type)
    {
        if($data_type == ParameterDataType::Integer)
            return    is_numeric($val) && is_int($val + 0) && intval($val) > 0;
        else if($data_type == ParameterDataType::Real)
            return    is_numeric($val) && is_real($val + 0);
        else if($data_type == ParameterDataType::Boolean)
        {
            $v  = strtolower($val);
            return $v == "true" || $v == "false";
        }
        else return true;

    }

    public static function checkActionRequirementsFrontEnd($action,$parameters)
    {
        if(!isset($_GET["action"]))
        {
            header("Location: error?code=400&message=".self::encodeContent("The parameter action was not set in the GET variable"));
            exit;
        }
        else if($_GET["action"] != $action)
            return false;
        else {
            if($parameters == null)
                return true;


            foreach ($parameters as $action_param => $param_info) {
                if ($param_info[0] && !isset($_GET[$action_param])) {
                    header("Location:  error?code=400&message=" . self::encodeContent("The parameter $action_param was not set in the GET variable"));
                    exit;
                } else if (!$param_info[0]  && !isset($_POST[$action_param])) {
                    header("Location:  error?code=400&message=" . self::encodeContent("The parameter $action_param was not set in the POST variable"));
                    exit;

                }

                if ($param_info[0] && !self::isValidDataType($_GET[$action_param]  ,$param_info[1])) {
                    header("Location:  error?code=400&message=" . self::encodeContent("The parameter $action_param is not a valid ".ParameterDataType::getName($param_info[1])));
                    exit;
                } else if (!$param_info[0]  &&!self::isValidDataType($_POST[$action_param]  ,$param_info[1])) {
                    header("Location:  error?code=400&message=" . self::encodeContent("The parameter $action_param is not a valid ".ParameterDataType::getName($param_info[1])));
                    exit;

                }
            }

            return true;
        }
        return false;
    }
    public static function checkActionRequirements($action,$parameters)
    {
        if(!isset($_GET["action"]))
        {
            header("Location:  error/?code=400&message=".self::encodeContent("The parameter action was not set in the GET variable"));
            exit;
        }
        else if($_GET["action"] != $action)
            return false;
        else {
            if($parameters == null)
                return true;


            foreach ($parameters as $action_param => $param_info) {
                if ($param_info[0] && !isset($_GET[$action_param])) {
                    header("Location: error/?code=400&message=" . self::encodeContent("The parameter $action_param was not set in the GET variable"));
                    exit;
                } else if (!$param_info[0]  && !isset($_POST[$action_param])) {
                    header("Location: error/?code=400&message=" . self::encodeContent("The parameter $action_param was not set in the POST variable"));
                    exit;

                }

                if ($param_info[0] && !self::isValidDataType($_GET[$action_param]  ,$param_info[1])) {
                    header("Location: error/?code=400&message=" . self::encodeContent("The parameter $action_param is not a valid ".ParameterDataType::getName($param_info[1])));
                    exit;
                } else if (!$param_info[0]  &&!self::isValidDataType($_POST[$action_param]  ,$param_info[1])) {
                    header("Location: error/?code=400&message=" . self::encodeContent("The parameter $action_param is not a valid ".ParameterDataType::getName($param_info[1])));
                    exit;

                }
            }

            return true;
        }
        return false;
    }
    public static function checkPageRequirements($parameters)
    {
        if($parameters == null)
            return true;

        foreach ($parameters as $action_param => $param_info) {
            if ($param_info[0] && !isset($_GET[$action_param])) {
                header("Location: error?code=400&message=" . self::encodeContent("The parameter $action_param was not set in the GET variable"));
                exit;
            } else if (!$param_info[0]  && !isset($_POST[$action_param])) {
                header("Location: error?code=400&message=" . self::encodeContent("The parameter $action_param was not set in the POST variable"));
                exit;

            }

            if ($param_info[0] && !self::isValidDataType($_GET[$action_param]  ,$param_info[1])) {
                header("Location: error?code=400&message=" . self::encodeContent("The parameter $action_param is not a valid ".ParameterDataType::getName($param_info[1])));
                exit;
            } else if (!$param_info[0]  &&!self::isValidDataType($_POST[$action_param]  ,$param_info[1])) {
                header("Location: error?code=400&message=" . self::encodeContent("The parameter $action_param is not a valid ".ParameterDataType::getName($param_info[1])));
                exit;

            }
        }

        return true;

    }
    public static function validateCSRF($input){
        if(isset($input["csrf_token"])){
            $csrf_token_name = $input["csrf_token"];
            if(isset($input[$csrf_token_name]) && isset($_COOKIE[$csrf_token_name]))
            {
                $cookie_token = $_COOKIE[$csrf_token_name];
                $input_token = $input[$csrf_token_name];
                unset($_COOKIE[$csrf_token_name]);
                unset($input[$csrf_token_name]);

                // match tokens
                return $input_token == $cookie_token;
            }
            else return false;
        }
        else return false;
    }
    public static function generateCSRF(){
        $token_value = bin2hex(openssl_random_pseudo_bytes(32));
        $token_name =  md5($token_value);

        setcookie($token_name, $token_value, 3600 * 24);

        $string = "<input type=\"hidden\" name=\"csrf_token\" value=\"" . $token_name . "\"";

        $string .= ">";

        $string .= "<input type=\"hidden\" name=\"" . $token_name . "\" value=\"" . $token_value . "\"";
        $string .= ">";

        return $string;
    }
    /**
     * Rebuilds the query string after removing the "view" parameter from GET variable
     * @return string
     */
    public static function getQueryString()
    {
        $OGET = $_GET;
        unset($OGET["view"]);
        return http_build_query($OGET);
    }

    /**
     * Encodes a string in base64
     * @param string $content Content
     * @return string
     */
    public static function encodeContent($content)
    {
        return  base64_encode ( $content);
    }

    /**
     * Decodes a base64 string
     * @param string $content Base64 string
     * @return string
     */
    public static function decodeContent($content)
    {
        return  base64_decode ( $content);
    }
}