<?php

namespace Core\Forms;
use Core\Controllers\AccessController;
use Core\Utils;


class Form
{

    public $Fields, $Action, $EncType, $Method;
    public $XSSClean,  $Validate, $Rules;


    public function __construct($fields, $action, $enctype = null, $method = "POST")
    {
        $this->XSSClean = true;
        $this->Validate = true;

        $this->Method = $method;
        $this->Fields = $fields;
        if($enctype == null){
            foreach ($this->Fields as $field) {
                if($field->FieldType == FieldType::File)
                    $enctype = "multipart/form-data";
            }
        }
        $this->EncType = $enctype;
        $this->Action = $action;

        // validation rules
        $this->Rules = array();
        foreach ($this->Fields as $field) {
            if($field->ValidationRules != null)
                $this->Rules[$field->Name]=  $field->ValidationRules;
        }

    }

    public function renderStart(){
        echo "<form action='".$this->Action."' method='".$this->Method."' ". ($this->EncType == null ? ">" : "enctype='".$this->EncType."'>");
    }
    public function renderEnd() {
        echo $this->generateCSRF();
        echo "</form>";
    }
    public function renderFields($access_privileges,$entity, $layer = 0,$form_errors = null){
        // render form fields
        foreach ($this->Fields as $field) {
            if(!AccessController::getInstance()->canPerfomAction($access_privileges, $field->AccessPrivileges) || ($layer != $field->Layer) )
                continue;

            $name = $field->Name;
            $value = $entity != null && isset($entity->$name) ? $entity->$name : null;
            $field->render( $value, $form_errors);
        }
    }
    public function render($access_privileges,$entity, $layer = 0,$form_errors = null){
        echo "<form action='".$this->Action."' method='".$this->Method."' ". ($this->EncType == null ? ">" : "enctype='".$this->EncType."'>");

        // render form fields
        foreach ($this->Fields as $field) {
            if(!AccessController::getInstance()->canPerfomAction($access_privileges, $field->AccessPrivileges)  || ($layer != $field->Layer) )
                continue;

            $name = $field->Name;
            $value = isset($entity->$name) ? $entity->$name : null;
            $field->render( $value, $form_errors);
        }

        echo $this->generateCSRF();
        echo "</form>";
    }

    public function filterInputAndRules($access_privileges, &$input, &$rules)
    {
        foreach ($this->Fields as $field) {
            if(!AccessController::getInstance()->canPerfomAction($access_privileges, $field->AccessPrivileges) )
            {
                if(isset($input[$field->Name]))
                    unset($input[$field->Name]);

                if(isset($rules[$field->Name]))
                    unset($rules[$field->Name]);


            } else if ($field->FieldType == FieldType::HtmlContent || $field->FieldType == FieldType::TextBase64)
                $input[$field->Name] = Utils::encodeContent( $input[$field->Name] );
        }
    }
    public function validate($access_privileges, $input,&$form_errors, &$output){
        $filtered = $input;
        // clean
        if($this->XSSClean)
            $filtered = FormValidator::xss_clean($filtered);

        // eliminate additional fields
        $rules = $this->Rules;
        $this->filterInputAndRules($access_privileges, $filtered, $rules);


        $output = $filtered;
        // validate
        if($this->Validate)
        {
            $form_errors = FormValidator::is_valid_array($filtered, $rules);
            return ($form_errors === true);
        }
        return true;

    }


    public function validateCSRF($input){
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
    public function generateCSRF(){
        $token_value = bin2hex(openssl_random_pseudo_bytes(32));
        $token_name =  md5($token_value);

        setcookie($token_name, $token_value, 3600 * 24);

        $string = "<input type=\"hidden\" name=\"csrf_token\" value=\"" . $token_name . "\"";

        $string .= ">";

        $string .= "<input type=\"hidden\" name=\"" . $token_name . "\" value=\"" . $token_value . "\"";
        $string .= ">";

        return $string;
    }

}