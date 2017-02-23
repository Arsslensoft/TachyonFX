<?php
namespace Core\Mail;
use \Exception;
/**
 * Mailer exception handler
 * @package Mailer
 */
class MailerException extends Exception
{
    /**
     * Prettify error message output
     * @return string
     */
    public function errorMessage()
    {
        $errorMsg = '<strong>' . $this->getMessage() . "</strong><br />\n";
        return $errorMsg;
    }

}