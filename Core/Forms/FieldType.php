<?php

namespace Core\Forms;
use Core\Enum;

class FieldType extends Enum {
    const __default = self::Number;
    /**
     * Number type
     */
    const Number = 0;
    /**
     * String text
     */
    const Text = 1;
    /**
     * String encoded in base 64
     */
    const TextBase64 = 2;
    /**
     * Email string
     */
    const Email = 3;
    /**
     * Password string
     */
    const Password = 4;
    /**
     * Long text encoded in base64
     */
    const HtmlContent = 5;
    /**
     * A relation field (int by default) must be linked via EntityBinding class
     */
    const EntityList = 6;

    /**
     * Enumeration field
     */
    const Enumeration = 7;

    const DateTime = 8;

    const Date = 9;

    const Boolean = 10;

    const File = 11;

    const Time = 12;

    const Label = 13;

    const StringEnumeration = 14;
}