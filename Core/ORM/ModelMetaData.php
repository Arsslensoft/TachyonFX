<?php
namespace Core\ORM;


class ModelMetaData{
    public $Columns;

    public function __construct($fields)
    {
        $this->Columns = $fields;
    }

    public function getFieldByName($field_name)
    {
        foreach ($this->Columns as $column) {
            if($column->Name == $field_name)
                return $column;
        }
        return null;
    }
    public function  getSQLDefinition()
    {
        $sql ="";
        $j = 0;
        for ($i = 0; $i < count($this->Columns); $i++)
        {

            if($this->Columns[$i]->Name == null)
                continue;

            if($j == 0)
                $sql .= $this->Columns[$i]->getSqlDefinition();
            else $sql .= ", ".$this->Columns[$i]->getSqlDefinition();
            $j++;
        }

        return $sql;
    }
    public function  getRelationshipsSQLDefinition($table)
    {
        $sql ="ALTER TABLE `".$table."` ";
        $j = 0;
        for ($i = 0; $i < count($this->Columns); $i++)
        {
            if($this->Columns[$i]->Name == null)
                continue;
            $query = $this->Columns[$i]->getRelationshipQuery();
            if($query != null){
                if($j == 0)
                    $sql .= $query;
                else $sql .= ", ".$query;
                $j++;
            }

        }

        return $j > 0 ? $sql : "";
    }


}