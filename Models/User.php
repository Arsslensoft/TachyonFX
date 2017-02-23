<?php

namespace Models;
use Core\ORM\ModelMetaData;
use Core\ORM\DBField;
use Core\ORM\PrimaryKeyDBField;
use Core\ORM\RelationalModel;
use Core\ORM\SQLDataTypes;
use Core\ORM\EntityRelationship;

class User extends RelationalModel {
    protected  static  $table= "users";

    public static function getMetaData()
    {
        return new ModelMetaData(array(
            new PrimaryKeyDBField(),
            new DBField("Username", SQLDataTypes::VAR_STRING,64 ),
            new DBField("Password", SQLDataTypes::VAR_STRING,60 ),
            new DBField("Email", SQLDataTypes::VAR_STRING,100 ),
            new DBField("Fullname", SQLDataTypes::VAR_STRING,128 ),
            new DBField("Tel", SQLDataTypes::VAR_STRING,20 ),
            new DBField("Address", SQLDataTypes::VAR_STRING,150 ),
            new DBField("Role", SQLDataTypes::LONG,1 ),
            new DBField("ProfilePicture", SQLDataTypes::VAR_STRING,255,false,false,"default.jpg"),
            new DBField(null, SQLDataTypes::LONG,11, false, false,null, new EntityRelationship("UserPosition", "UserId", "UserPosition",true) ),
        ));
    }
}

