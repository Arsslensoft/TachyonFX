<?php

namespace Core\Forms;
use Core\Utils;

class FormField {
    public $ValidationRules;
    public $Name;
    public $DisplayName;
    public $FieldType;
    public $AdditionalAttributes;
    public $StyleClass;
    public $AccessPrivileges;
    public $Layer;
    public $BoundEntity;

    public function __construct($name, $type, $privileges = 0, $display,$rules = null, $be = null,$layer = 0, $attributes = null, $class_name = null)
    {
        $this->Name = $name;
        $this->FieldType = $type;
        $this->DisplayName = $display;
        $this->ValidationRules = $rules;
        $this->AdditionalAttributes = $attributes;
        $this->StyleClass = $class_name != null ? $class_name : "form-control";
        $this->AccessPrivileges = $privileges;
        $this->Layer = $layer;
        $this->BoundEntity=$be;
    }

    public function renderElementByType($default_value){

        if($this->FieldType == FieldType::Text)
            echo   "<input type=\"text\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\" id=\"".$this->Name."\" placeholder=\"Enter ".$this->DisplayName."\" value=\"".$default_value."\" />";
        else if($this->FieldType == FieldType::Password)
            echo   "<input type=\"password\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\" id=\"".$this->Name."\" placeholder=\"Enter ".$this->DisplayName."\" />";
        else if($this->FieldType == FieldType::Number)
            echo   "<input type=\"number\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\" id=\"".$this->Name."\" placeholder=\"Enter ".$this->DisplayName."\"  value=\"".$default_value."\" />";
        else if($this->FieldType == FieldType::Boolean)
            echo   "<input type=\"checkbox\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass." flat-red\" id=\"".$this->Name."\"  ".($default_value == true ?"checked":"")."  />";
        else if($this->FieldType == FieldType::DateTime)
            echo   "<input type=\"datetime-local\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\" id=\"".$this->Name."\" placeholder=\"Enter ".$this->DisplayName."\"  value=\"".$default_value."\"  />";
        else if($this->FieldType == FieldType::Date)
            echo   "<input type=\"date\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\" id=\"".$this->Name."\" placeholder=\"Enter ".$this->DisplayName."\"  value=\"".$default_value."\" />";
        else if($this->FieldType == FieldType::File)
            echo   "<input type=\"file\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\" id=\"".$this->Name."\" placeholder=\"Enter ".$this->DisplayName."\"  />";
        else if($this->FieldType == FieldType::Time)
            echo   "<input type=\"time\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\" id=\"".$this->Name."\" placeholder=\"Enter ".$this->DisplayName."\"  value=\"".$default_value."\" />";

        else if($this->FieldType == FieldType::Email)
            echo   "<input type=\"email\" name=\"".$this->Name."\"  ".$this->AdditionalAttributes."  class=\"".$this->StyleClass."\" id=\"".$this->Name."\" placeholder=\"Enter ".$this->DisplayName."\" value=\"".$default_value."\" />";
        else if($this->FieldType == FieldType::EntityList && $this->BoundEntity != null)
        {

            echo "<select id=\"".$this->Name."\" name=\"".$this->Name."\"  ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\">";
            $entity_key = $this->BoundEntity->Key;
            $entity_name = $this->BoundEntity->Name;
            $entities = $this->BoundEntity->getAll();

            foreach ($entities as $el) {
                echo "<option value=\"".$el->$entity_key."\" ".($el->$entity_key == $default_value ? "selected " : " ").">".$el->$entity_name."</option>";
            }

            echo  "</select>";
        }
        else if($this->FieldType == FieldType::Enumeration && $this->BoundEntity != null)
        {

            echo "<select id=\"".$this->Name."\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\">";

            foreach ($this->BoundEntity as $el => $value) {
                echo "<option value=\"".$el."\" ".($el == $default_value ? "selected " : " ").">".$value."</option>";
            }

            echo  "</select>";
        }
        else if($this->FieldType == FieldType::StringEnumeration && $this->BoundEntity != null)
        {

            echo "<select id=\"".$this->Name."\" name=\"".$this->Name."\" ".$this->AdditionalAttributes." class=\"".$this->StyleClass."\">";

            foreach ($this->BoundEntity as $el => $value) {
                echo "<option value=\"".$value."\" ".($value == $default_value ? "selected " : " ").">".$value."</option>";
            }

            echo  "</select>";
        }
        else if($this->FieldType == FieldType::HtmlContent)
            echo " <textarea id=\"".$this->Name."\"  ".$this->AdditionalAttributes." name=\"".$this->Name."\" class=\"".$this->StyleClass." htmleditor\" rows=\"3\">".Utils::decodeContent($default_value)."</textarea>";
        else if($this->FieldType == FieldType::TextBase64)
            echo " <textarea id=\"".$this->Name."\" ".$this->AdditionalAttributes."  name=\"".$this->Name."\" class=\"".$this->StyleClass."\" rows=\"3\">".Utils::decodeContent($default_value)."</textarea>";
        else if($this->FieldType == FieldType::Label)
            echo "<label ".$this->AdditionalAttributes." for=\"".$this->Name."\">".$this->DisplayName.": ".$default_value."</label>";
    }
    public function render($value = null, $form_errors = null){
        echo "   <div class=\"form-group".(isset($form_errors[$this->Name]) ?" has-error" : "")."\" >";
        if($this->FieldType != FieldType::Label)
            echo "<label for=\"".$this->Name."\">".(isset($form_errors[$this->Name]) ? "<i class=\"fa fa-times-circle-o\"></i>  " : "").$this->DisplayName."</label>";
        // render element
        $this->renderElementByType($value);

        if(isset($form_errors[$this->Name]))
        {
            echo "<span class=\"help-block\">".$form_errors[$this->Name]."</span>";
        }
        echo "</div>";
    }


}
