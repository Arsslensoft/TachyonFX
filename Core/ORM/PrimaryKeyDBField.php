<?php
/**
 * Created by PhpStorm.
 * User: Arsslen
 * Date: 2/22/2017
 * Time: 7:39 PM
 */

namespace Core\ORM;


class PrimaryKeyDBField extends DBField
{
  public function __construct($name = "Id", $type = SQLDataTypes::LONG, $length = 11)
  {
      parent::__construct($name, $type, $length, true, false, null, null);
  }
}