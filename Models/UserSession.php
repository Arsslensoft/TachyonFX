<?php

namespace Models;
use Core\ORM\ModelMetaData;
use Core\ORM\DBField;
use Core\ORM\PrimaryKeyDBField;
use Core\ORM\RelationalModel;
use Core\ORM\SQLDataTypes;
use Core\ORM\EntityRelationship;

/**
 * Logged in User Session
 */
class UserSession extends RelationalModel {
    protected static  $table = 'user_sessions';

    public static function getMetaData()
    {
        return new ModelMetaData(array(
            new PrimaryKeyDBField(),
            new DBField("UserId", SQLDataTypes::LONG,11, false, false,null, new EntityRelationship("User") ),
            new DBField("UserAgent", SQLDataTypes::TEXT,null ),
            new DBField("SessionToken", SQLDataTypes::VAR_STRING,128 ),
            new DBField("LastAccessIP", SQLDataTypes::VAR_STRING,30 ),
            new DBField("LastAccessDate", SQLDataTypes::DATETIME,null )

        ));
    }

}