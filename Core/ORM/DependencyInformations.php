<?php
namespace Core\ORM;


class DependencyInformations
{
    public $Entity;
    public $Count;
    public $Values;
    public $ChildDependencies;

    public function __construct($entity, $values, $children)
    {
        $this->Entity = $entity;
        $this->Values = $values;
        $this->ChildDependencies = $children;
        $this->Count = count($children);
    }
}