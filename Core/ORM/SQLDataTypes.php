<?php
namespace Core\ORM;
use Core\Enum;

class SQLDataTypes extends Enum{
    public function __construct($value)
    {
        parent::__construct($value);
    }

    const DECIMAL   = 0;
    const TINY      = 1;
    const SHORT           = 2;
    const LONG            = 3;
    const FLOAT           = 4;
    const DOUBLE          = 5;

    const TIMESTAMP       = 7;
    const LONGLONG        = 8;
    const INT24           = 9;
    const DATE            =10;
    const TIME            =11;
    const DATETIME        =12;
    const YEAR            =13;

    const BIT             =16;
    const TINY_BLOB      =249;
    const MEDIUM_BLOB    =250;
    const LONG_BLOB      =251;
    const BLOB           =252;
    const VAR_STRING     =253;
    const STRING         =254;

    const TEXT           =255;
    const MEDIUMTEXT     =256;
    const LONGTEXT       =257;


    public static $sql_equivalent = array(
        0=>'decimal',
        1=>'tinyint',
        2=>'smallint',
        3=>'int',
        4=>'float',
        5=>'double',
        7=>'timestamp',
        8=>'bigint',
        9=>'mediumint',
        10=>'date',
        11=>'time',
        12=>'datetime',
        13=>'year',
        16=>'bit',
        249=>'tinyblob',
        250=>'mediumblob',
        251=>'longblob',
        252=>'blob',
        253=>'varchar',
        254=>'char',
        255=>'text',
        256=>'mediumtext',
        257=>'longtext',

    );
    public static $php_equivalent = array(
        'decimal' => 0,
        'tinyint' => 1,
        'smallint' => 2,
        'int' => 3,
        'float' => 4,
        'double' => 5,

        'timestamp' => 7,
        'bigint' => 8,
        'mediumint' => 9,
        'date' => 10,
        'time' => 11,
        'datetime' => 12,
        'year' => 13,

        'bit' => 16,

        'tinyblob' => 249,
        'mediumblob' => 250,
        'longblob' => 251,
        'blob' => 252,
        'varchar' => 253,
        'char' => 254,
        'text' => 255,
        'mediumtext' => 256,
        'longtext' => 257,
    );


    public function getSqlCode(){
        return self::$sql_equivalent[$this->value];
    }
    public static function getSQLDataType($type){
        return new SQLDataTypes(self::$php_equivalent[$type]);
    }
}