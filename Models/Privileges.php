<?php


namespace Models;


use Core\Enum;

class Privileges extends Enum
{

    const __default = self::None;
    const None = 0;
}