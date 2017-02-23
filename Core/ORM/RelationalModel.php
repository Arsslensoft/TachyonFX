<?php
namespace Core\ORM;
use \ReflectionClass;
/**
 * Class RelationalModel, defines a model class which supports relational operations (binding, lookup)
 */
class RelationalModel extends EmeraldObjectRelationalModel {


    protected static $MetaData;

    /**
     * Get all records with binding.
     *
     * @access public
     * @return array
     */
    public static function allWithBinding ()
    {
        return self::sqlWithBinding("SELECT * FROM :database.:table");
    }
    /**
     * Retrieve a record by its primary key (PK) and perform a full ER binding.
     *
     * @access public
     * @param integer $pk
     * @return object
     */
    public static function retrieveByPKWithBinding ($pk)
    {
        if (!is_numeric($pk))
            throw new \InvalidArgumentException('The PK must be an integer.');

        $class_name = get_called_class();
        $reflectionObj = new ReflectionClass($class_name);

        $entity = $reflectionObj->newInstanceArgs(array($pk, EmeraldObjectRelationalModel::LOAD_BY_PK));
        $erms = $class_name::getEntityRelationships();
        if($erms != null)
        {
            foreach ($erms as $erm) {
                $entity = $erm->bind($entity);
            }
        }

        return $entity;
    }
    /**
     * Execute an SQL statement & get all records as hydrated objects.
     *
     * @access public
     * @param string $sql
     * @param integer $return
     * @return mixed
     */
    public static function sqlWithBinding ($sql, $query_values = null, $return = EmeraldObjectRelationalModel::FETCH_MANY)
    {
        $ret = self::sql($sql, $query_values, $return);
        $class_name = get_called_class();
        $erms = $class_name::getEntityRelationships();

        if($erms != null)
        {
            if($return == EmeraldObjectRelationalModel::FETCH_ONE){
                foreach ($erms as $erm) {
                    if($erm  != null && $ret != null) {
                        $ret = $erm->bind($ret);

                    }
                }
            }
            else  if($return == EmeraldObjectRelationalModel::FETCH_MANY) {

                for($i = 0; $i < count($ret); $i++){
                    foreach ($erms as $erm) {
                        if($erm  != null && isset($ret[$i]))
                            $ret[$i] = $erm->bind($ret[$i] );
                    }
                }
            }


        }
        return $ret;

    }
    /**
     * Retrieve a record by a particular column name, and performs ER binding.
     *
     * @access public
     * @static
     * @param string $field
     * @param mixed $value
     * @param integer $return
     * @return mixed
     */
    public static function retrieveByFieldWithBinding ($field, $value, $return = EmeraldObjectRelationalModel::FETCH_MANY)
    {  echo "  $field";
        if (!is_string($field))
            throw new \InvalidArgumentException('The field name must be a string.');

        // build our query
        $operator = (strpos($value, '%') === false) ? '=' : 'LIKE';

        $sql = sprintf("SELECT * FROM :database.:table WHERE %s %s '%s'", $field, $operator, $value);

        if ($return === EmeraldObjectRelationalModel::FETCH_ONE)
            $sql .= ' LIMIT 0,1';

        // fetch our records
        return self::sqlWithBinding($sql, $return);
    }

    public static function getEntityRelationshipDependencies()
    {
        $class_name = get_called_class();
        $erms = $class_name::getEntityRelationships();
        $dependencies  = array();
        if($erms != null)
        {
            foreach ($erms as $erm) {

                if($erm->PrimaryEntityField->Name == $class_name::getTablePk()){
                    array_push($dependencies, $erm);
                }

            }
        }
        return $dependencies;
    }
    public function calculateDependencies()
    {
        $dependencies = self::getEntityRelationshipDependencies();
        $dep_infos = array();
        foreach ($dependencies as $dependency) {

            $fentity = $dependency->ForeignEntity;
            $pkey = $dependency->PrimaryEntityField->Name;
            $fkey = $dependency->ForeignEntityKey;

            $dependency_instances = $fentity::retrieveByField($fkey, $this->$pkey);
            $child_dep_infos = array();
            foreach ($dependency_instances as $dependency_instance) {
                $child =   $dependency_instance->calculateDependencies();
                array_push($child_dep_infos, $child);
            }
            array_push($dep_infos, new DependencyInformations($fentity, $dependency_instances, $child_dep_infos));
        }
        return $dep_infos;
    }
    public function renderDependencies()
    {
        $dependencies = self::getEntityRelationshipDependencies();
        $dep_infos = array();
        echo "<ul>";
        foreach ($dependencies as $dependency) {

            $fentity = $dependency->ForeignEntity;
            $pkey = $dependency->PrimaryEntityField->Name;
            $fkey = $dependency->ForeignEntityKey;
            $dependency_instances = $fentity::retrieveByField($fkey, $this->$pkey);



            if(count($dependency_instances) > 0) {
                echo "<li>";
                echo $fentity. " [".$fkey."= ".$this->$pkey;
                echo ", ".count($dependency_instances)." record(s)]";
                echo "<ul>";
                foreach ($dependency_instances as $dependency_instance) {

                    $dependency_instance->renderDependencies();

                }
                echo "</ul>";
                echo "</li>";
            }


        }
        echo "</ul>";
        return $dep_infos;
    }
    public function removeDependencies()
    {
        $dependencies = self::getEntityRelationshipDependencies();
        $class_name  = get_called_class();
        foreach ($dependencies as $dependency) {

            $fentity = $dependency->ForeignEntity;
            $pkey = $dependency->PrimaryEntityField->Name;
            $fkey = $dependency->ForeignEntityKey;

            $dependency_instances = $fentity::retrieveByField($fkey, $this->$pkey);
            if(count($dependency_instances) > 0) {
                foreach ($dependency_instances as $dependency_instance) {
                    // remove childrens
                    $dependency_instance->removeDependencies();
                    // remove me
                    $fentity::deleteByField($fkey, $this->$pkey);
                }
            }
            else{
                // remove me
                $fentity::deleteByField($fkey, $this->$pkey);
            }

        }

    }


    public function __call($name, $args)
    {
        $class = get_called_class();

        if (substr($name, 0, 3) == 'get')
        {

            // prepend field name to args
            $field =  substr($name, 3);
            array_unshift($args, $field);

            return call_user_func_array(array($this, 'bindEntity'), $args);
        }

        throw new \Exception(sprintf('There is no static method named "%s" in the class "%s".', $name, $class));
    }
    public static function getMetaData()
    {
        return self::$MetaData;
    }

    public function bindEntity($field)
    {

        $class_name = get_called_class();

        $erms = $class_name::getEntityRelationships();

        if($erms != null)
        {
            foreach ($erms as $erm) {
                if($erm->TargetFieldName == $field)
                {

                    if(isset($this->$field))
                        return $this->$field;
                    else
                        return $erm->bindSingle($this)->$field;
                }

            }
        }
        return null;
    }


    /**
     * Gets all entity relationship mappings defined in a class
     * @return array
     */
    public static function getEntityRelationships(){
        $rels = array();
        $class_name = get_called_class();

        $meta = $class_name::getMetaData();

        foreach ($meta->Columns as $column) {
            if($column != null && $column->Relationship != null)
                array_push($rels, $column->Relationship);
        }

        return count($rels) > 0 ? $rels : null;
    }


    public static function createTable()
    {
        $class_name = get_called_class();

        $sql_query = "CREATE TABLE IF NOT EXISTS `".self::getTableName()."` (%s";
        $columns_sql = $class_name::getMetaData()->getSQLDefinition();
        $constraints_sql = $class_name::getMetaData()->getRelationshipsSQLDefinition(self::getTableName());
        $sql_query = sprintf($sql_query, $columns_sql);

        // add relationships

        // execute query
        $sql_query .= ');';

        self::sql($sql_query,null, EmeraldObjectRelationalModel::FETCH_NONE);
        // add relationships
        if(strlen($constraints_sql) > 0)
            self::sql($constraints_sql,null, EmeraldObjectRelationalModel::FETCH_NONE);



    }
}