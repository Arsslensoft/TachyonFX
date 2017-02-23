<?php
namespace Core\ORM;

class EntityRelationship
{

    public static $RelationShipMapperCache = array();
    public $TargetFieldName;
    public $PrimaryEntityField;
    public $ForeignEntityKey;
    public $ForeignEntity;
    public $IsOneToOne;

    /**
     * EntityRelationshipMapper constructor.
     * @param $foreign_entity Foreign key class name
     * @param $pkey Primary entity foreign key field name
     * @param string $fkey  Foreign entity primary key field name
     * @param bool $oto  1 to 1 relationship, otherwise 1 to *
     */
    public function __construct($foreign_entity, $fkey = "Id",$target_field_name = null, $oto = false)
    {
        $this->TargetFieldName = $target_field_name != null ? $target_field_name : $foreign_entity;
        $this->ForeignEntityKey = $fkey;
        $this->ForeignEntity = $foreign_entity;
        $this->IsOneToOne = $oto;
    }

    /**
     * Binds two entities, lookup for foreign keys, link the tables
     * Example :
     * User(Id, PictureId) and Picture(Id, Image); => 1 to 1 => fkey = Id, pkey=PictureId
     * primary_instance = [Id = 1, PictureId=2]
     * The function will modify $primary_instance and return it like this
     * primary_instance = [Id = 1, PictureId=2, Picture=[ Id = 2, Image="Hi.jpg"] ]
     * @param $primary_instance Parent entity instance
     * @return mixed
     */
    public function bind($primary_instance)
    {
        $fentity = $this->ForeignEntity;
        $pkey = $this->PrimaryEntityField->Name;
        $fkey = $this->ForeignEntityKey;
        $target_field_name =    $this->TargetFieldName ;

        // infinite lookup fix
        if(isset(self::$RelationShipMapperCache[$fentity]) &&  isset(self::$RelationShipMapperCache[$fentity][$this->IsOneToOne]) && isset(self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$primary_instance->$pkey]))
        {
            $primary_instance->$target_field_name =  self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$primary_instance->$pkey];
            return $primary_instance;
        }

        self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$primary_instance->$pkey] = false;

        if($this->IsOneToOne)
            $primary_instance->$target_field_name = $fentity::retrieveByFieldWithBinding($fkey, $primary_instance->$pkey, EmeraldObjectRelationalModel::FETCH_ONE);
        else $primary_instance->$target_field_name = $fentity::retrieveByFieldWithBinding($fkey, $primary_instance->$pkey, EmeraldObjectRelationalModel::FETCH_MANY);

        self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$primary_instance->$pkey] =  $primary_instance->$target_field_name;

        return $primary_instance;

    }

    /**
     * Binds two entities, lookup for foreign keys, link the tables
     * Example :
     * User(Id, PictureId) and Picture(Id, Image); => 1 to 1 => fkey = Id, pkey=PictureId
     * primary_instance = [Id = 1, PictureId=2]
     * The function will modify $primary_instance and return it like this
     * primary_instance = [Id = 1, PictureId=2, Picture=[ Id = 2, Image="Hi.jpg"] ]
     * @param $primary_instance Parent entity instance
     * @return mixed
     */
    public function bindSingle($primary_instance)
    {
        $fentity = $this->ForeignEntity;
        $pkey = $this->PrimaryEntityField->Name;
        $fkey = $this->ForeignEntityKey;
        $target_field_name =    $this->TargetFieldName ;

        // infinite lookup fix
        if(isset(self::$RelationShipMapperCache[$fentity]) &&  isset(self::$RelationShipMapperCache[$fentity][$this->IsOneToOne]) && isset(self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$primary_instance->$pkey]))
        {
            $primary_instance->$target_field_name =  self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$primary_instance->$pkey];
            return $primary_instance;
        }

        self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$primary_instance->$pkey] = false;

        if($this->IsOneToOne)
            $primary_instance->$target_field_name = $fentity::retrieveByField($fkey, $primary_instance->$pkey, EmeraldObjectRelationalModel::FETCH_ONE);
        else $primary_instance->$target_field_name = $fentity::retrieveByField($fkey, $primary_instance->$pkey, EmeraldObjectRelationalModel::FETCH_MANY);

        self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$primary_instance->$pkey] =  $primary_instance->$target_field_name;

        return $primary_instance;

    }

    /**
     * Binds two entities, lookup for foreign values using the query, link the tables
     * Example :
     * User(Id) and Message(Id, UserId); => 1 to * => fkey = UserId, pkey=Id
     * primary_instance = [Id = 1]
     * The function will modify $primary_instance and return it like this
     * primary_instance = [Id = 1, Messages=[ [Id = 2, UserId=1], [Id = 5, UserId=1] ] ]
     * @param $primary_instance
     * @param $query
     * @return mixed
     */
    public function bindWithQuery($primary_instance, $query)
    {
        $fentity = $this->ForeignEntity;
        $target_field_name =    $this->TargetFieldName ;

        // infinite lookup fix
        if(isset(self::$RelationShipMapperCache[$fentity]) &&  isset(self::$RelationShipMapperCache[$fentity][$this->IsOneToOne]) && isset(self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$query]))
        {
            $primary_instance->$target_field_name =  self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$query];
            return $primary_instance;
        }

        self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$query] = false;


        if($this->IsOneToOne)
            $primary_instance->$target_field_name = $fentity::sqlWithBinding($query, EmeraldObjectRelationalModel::FETCH_ONE);
        else $primary_instance->$target_field_name = $fentity::sqlWithBinding($query, EmeraldObjectRelationalModel::FETCH_MANY);

        self::$RelationShipMapperCache[$fentity][$this->IsOneToOne][$query] =  $primary_instance->$target_field_name;

        return $primary_instance;
    }
}