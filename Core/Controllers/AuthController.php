<?php
namespace Core\Controllers;

use Core\ORM\DataMappingManager;
use Core\ORM\EmeraldObjectRelationalModel;
use Exception;
use Models\User;
use Models\UserSession;


class AuthController extends  ModelBaseController {


    private static $instance;
    private static $user;
    public static function getInstance()
    {
        if(self::$instance == null)
        {
            $instance = new AuthController();
            return $instance;
        }

        return self::$instance;
    }

    public static function getCurrentUser()
    {
        $auth =  self::getInstance();
        if($auth->logged_in && (self::$user == null || self::$user->Id != $auth->current_userid ) )
            self::$user = $auth->getUser($auth->current_userid);

        return self::$user;
    }

    private $sessionName;

    /**
     * @var bool $logged_in Current Logged in state of the user
     */
    public $logged_in = false;

    /**
     * @var int $current_userid Current Connected User Id
     */
    public $current_userid;


    /**
     * Object construct verifies that a session has been started and that a MySQL connection can be established.
     * It takes no parameters.
     *
     * @exception    Exception    If a session id can't be returned.
     */

    public function __construct()
    {
        $this->sessionName = $GLOBALS["tachyon_config"]["session_name"];
        $sessionId = session_id();
        if (strlen($sessionId) == 0)
            throw new Exception("No session has been started.\n<br />Please add `session_start();` initially in your file before any output.");

        DataMappingManager::initializeMapper();

        $this->_validateUser();
    }


    /**
     * Logout a user
     */
    public function logoutUser()
    {
        if (isset($_SESSION[$this->sessionName]))
            unset($_SESSION[$this->sessionName]);
        session_regenerate_id ();
        $this->logged_in = false;
    }

    /**
     * Login a user
     * @param string $username Username string
     * @param string $password Password string
     * @param string $ip Ip address of the user
     * @return null
     * @throws Exception
     */
    public function loginUser($username, $password, $ip)
    {
        $sql = "SELECT * FROM users WHERE Username=? LIMIT 1";
        $user = User::sql($sql, [$username], EmeraldObjectRelationalModel::FETCH_ONE);

        if ($user != null && password_verify($password, $user->Password)) {

            session_regenerate_id ();
            $_SESSION[$this->sessionName]["Id"] = $user->Id;
            $_SESSION[$this->sessionName]["user"] = $user->Username;
            $this->current_userid = intval($_SESSION[$this->sessionName]["Id"]);

            $this->addOrUpdateSession( $user->Id, $ip);

            $this->logged_in = true;
            return $user->Id;

        }
        else return null;
    }

    /**
     * Creates a user session
     * @param $user_id
     * @param $ip
     */
    public function addOrUpdateSession($user_id, $ip)
    {
        $usession = UserSession::retrieveByField("UserId",$user_id, EmeraldObjectRelationalModel::FETCH_ONE);
        $new_token = hash('sha256',"IP=".$_SERVER['REMOTE_ADDR'].",UA=". $_SERVER['HTTP_USER_AGENT']);

        if($usession == null) {
            $usession = new UserSession();
            $usession->UserId = $user_id;
            $usession->LastAccessDate = date("Y-m-d H:i:s", time());
            $usession->LastAccessIP = $ip;
            $usession->SessionToken = $new_token;
            $usession->UserAgent = $_SERVER['HTTP_USER_AGENT'];
            $usession->save();
        }
        else {
            $usession =  UserSession::retrieveByPK($usession->Id);
            $usession->SessionToken = $new_token;
            $usession->LastAccessDate = date("Y-m-d H:i:s", time());
            $usession->LastAccessIP = $ip;
            $usession->UserAgent = $_SERVER['HTTP_USER_AGENT'];
            $usession->save();
        }
        $_SESSION[$this->sessionName]["Token"] = $usession->SessionToken;





    }

    /**
     * Checks if a user session is valid
     * @return bool|void
     */
    public function validateUserSession()
    {
        $new_token = hash('sha256',"IP=".$_SERVER['REMOTE_ADDR'].",UA=". $_SERVER['HTTP_USER_AGENT']);
        if(!isset($_SESSION[$this->sessionName]["Token"]))
            return false;
        $session_token = $_SESSION[$this->sessionName]["Token"];
        $usession = UserSession::sql("SELECT * FROM user_sessions WHERE UserId=? AND SessionToken=?", [$this->current_userid, $session_token],EmeraldObjectRelationalModel::FETCH_ONE);

        if($usession == null)
            return false;
        else {
            // if not match there is a potential session hijaking
            return $usession->SessionToken == $new_token;
        }
    }

    /**
     * Gets a user using a given userId
     * @param int $userId User Id
     * @return User
     */
    public function getUser($userId = null)
    {

        if ($userId == null)
            $userId = $_SESSION[$this->sessionName]["Id"];

        $user = User::retrieveByPK($userId);
        return $user;

    }

    public function getLockedUser()
    {

        if(isset($_SESSION[$this->sessionName]["LOCKED_USER"]))
            return $_SESSION[$this->sessionName]["LOCKED_USER"];
        else return -1;
    }

    /**
     * Checks if the current connected user is a valid user
     * @return bool
     * @throws Exception
     */
    private function _validateUser()
    {
        if (!isset($_SESSION[$this->sessionName]["Id"]))
            return;

        if (!$this->_validateUserId())
            return;


        $this->current_userid = intval($_SESSION[$this->sessionName]["Id"]);
        $this->logged_in =  $this->validateUserSession();
    }

    /**
     * Checks if the current connected user is a valid user
     * @return bool
     * @throws Exception
     */
    private function _validateUserId()
    {
        $userId = $_SESSION[$this->sessionName]["Id"];


        $sql = "SELECT * FROM users WHERE Id=? LIMIT 1";

        $users = User::sql($sql,[$userId]);

        if (count($users))
            return true;

        $this->logoutUser();

        return false;
    }


}

?>