<?php

namespace Core\ORM;


class NullableDBField extends DBField
{
   public function __construct($name, $type, $length)
   {
       parent::__construct($name, $type, $length, false, true, null, null);
   }

}