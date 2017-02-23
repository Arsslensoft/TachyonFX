<?php
namespace Core\Forms;


class FormEntityBinding
{
    /**
     * Reflection class name of the target entity
     * @var
     */
    public $ClassName;

    /**
     * EntityBinding constructor.
     * @param $classname The reflection class name
     */
    public function __construct($classname)
    {
        $this->ClassName=$classname;
    }

    public function getAll($filter = null){
        return null;
    }
    public function getByPrimaryKey($pk){
        return null;
    }

}