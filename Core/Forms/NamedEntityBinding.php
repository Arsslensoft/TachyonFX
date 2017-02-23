<?php
namespace Core\Forms;

/**
 * Class NamedEntityBinding, binds two entities using a Relational mapping
 */
class NamedEntityBinding extends FormEntityBinding  {
    /**
     * Reflection class name of the target entity
     * @var
     */
    public $ClassName;
    /**
     * Primary Key field in the target entity
     * @var
     */
    public $Key;
    /**
     * The description field name (example : Name, Description...)
     * @var
     */
    public $Name;


    /**
     * NamedEntityBinding constructor.
     * @param $name The name field
     * @param $key The primary key field
     * @param $classname The reflection class name
     */
    public function __construct($name, $key, $classname)
    {
        parent::__construct($classname);
        $this->Name=$name;
        $this->Key=$key;
    }

    public function getAll($filter = null){
        $model = $this->ClassName;
        if($filter == null)
            return $model::all();
        else return $model::sql($filter);
    }
    public function getByPrimaryKey($pk){
        $model = $this->ClassName;
        return $model::retrieveByPK($pk);
    }
}