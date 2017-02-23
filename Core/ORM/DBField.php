<?php
namespace Core\ORM;


class DBField{
    public $Nullable;
    public $DefaultValue;
    public $SQLType;
    public $Name;
    public $IsPrimaryKey;
    public $Length;
    public $Relationship;


    public function __construct($name, $type,$length, $primary = false, $nullable = false, $default = null, $relationship = null)
    {

        $this->Name = $name;
        $this->SQLType = new SQLDataTypes($type);
        $this->Length = $length;
        $this->DefaultValue = $default;
        $this->IsPrimaryKey = $primary;
        $this->Nullable = $nullable;

        $this->Relationship = $relationship;
        if( $this->Relationship != null){
            $this->Relationship->PrimaryEntityField = $this;
        }

    }
    public function getSqlDefinition()
    {

        if($this->Length != null)
            return sprintf("`%s` %s(%s) %s", $this->Name, $this->SQLType->getSqlCode(),
                $this->Length,
                ($this->Nullable ? "NULL" : ($this->DefaultValue != null ? "DEFAULT '".$this->DefaultValue."'" : "NOT NULL")).($this->IsPrimaryKey ? " PRIMARY KEY AUTO_INCREMENT":"") );
        else   return sprintf("`%s` %s %s", $this->Name, $this->SQLType->getSqlCode(), ($this->Nullable ? "NULL" : ($this->DefaultValue != null ? "DEFAULT '".$this->DefaultValue."'" : "NOT NULL")).($this->IsPrimaryKey ? " PRIMARY KEY AUTO_INCREMENT":"") );

    }
    public function getRelationshipQuery(){

        if($this->Relationship != null && $this->Relationship->IsOneToOne){

            $target_class = $this->Relationship->ForeignEntity;
            $sql = " ADD FOREIGN KEY (".$this->Name.")  REFERENCES `".$target_class::getTableName()."`(".$this->Relationship->ForeignEntityKey.")";
            return $sql;
        }
        return null;
    }

}