<?php
namespace Core\Helpers;


class UserAgent
{
    var $agent		= NULL;
    var $is_browser	= FALSE;
    var $is_robot	= FALSE;
    var $is_mobile	= FALSE;
    var $languages	= array();
    var $charsets	= array();
    var $platforms	= array();
    var $browsers	= array();
    var $mobiles	= array();
    var $robots		= array();
    var $platform	= '';
    var $browser	= '';
    var $version	= '';
    var $mobile		= '';
    var $robot		= '';


    /**
     * Constructor
     *
     * Sets the User Agent and runs the compilation routine
     *
     * @access	public
     * @return	void
     */
    public function __construct()
    {
        if (isset($_SERVER['HTTP_USER_AGENT']))
        {
            $this->agent = trim($_SERVER['HTTP_USER_AGENT']);
        }
        if ( ! is_null($this->agent))
        {
            if ($this->loadAgentFile())
            {
                $this->compileData();
            }
        }
        define('DEBUG', "User Agent Class Initialized");
    }
    /**
     * Compile the User Agent Data
     *
     * @access	private
     * @return	bool
     */
    private function loadAgentFile()
    {
        include(ROOT_DIR . DIRECTORY_SEPARATOR . 'Config/data/user_agents_config.inc.php');

        $return = FALSE;
        if (isset($platforms))
        {
            $this->platforms = $platforms;
            unset($platforms);
            $return = TRUE;
        }
        if (isset($browsers))
        {
            $this->browsers = $browsers;
            unset($browsers);
            $return = TRUE;
        }
        if (isset($mobiles))
        {
            $this->mobiles = $mobiles;
            unset($mobiles);
            $return = TRUE;
        }
        if (isset($robots))
        {
            $this->robots = $robots;
            unset($robots);
            $return = TRUE;
        }
        return $return;
    }
    /**
     * Compile the User Agent Data
     *
     * @access	private
     * @return	bool
     */
    private function compileData()
    {
        $this->setPlatform();
        foreach (array('setRobot', 'setBrowser', 'setMobile') as $function)
        {
            if ($this->$function() === TRUE)
            {
                break;
            }
        }
    }
    /**
     * Set the Platform
     *
     * @access	private
     * @return	mixed
     */
    private function setPlatform()
    {
        if (is_array($this->platforms) AND count($this->platforms) > 0)
        {
            foreach ($this->platforms as $key => $val)
            {
                if (preg_match("|".preg_quote($key)."|i", $this->agent))
                {
                    $this->platform = $val;
                    return TRUE;
                }
            }
        }
        $this->platform = 'Unknown Platform';
    }


    /**
     * Set the Browser
     *
     * @access	private
     * @return	bool
     */
    private function setBrowser()
    {
        if (is_array($this->browsers) AND count($this->browsers) > 0)
        {
            foreach ($this->browsers as $key => $val)
            {
                if (preg_match("|".preg_quote($key).".*?([0-9\.]+)|i", $this->agent, $match))
                {
                    $this->is_browser = TRUE;
                    $this->version = $match[1];
                    $this->browser = $val;
                    $this->setMobile();
                    return TRUE;
                }
            }
        }
        return FALSE;
    }

    /**
     * Set the Robot
     *
     * @access	private
     * @return	bool
     */
    private function setRobot()
    {
        if (is_array($this->robots) AND count($this->robots) > 0)
        {
            foreach ($this->robots as $key => $val)
            {
                if (preg_match("|".preg_quote($key)."|i", $this->agent))
                {
                    $this->is_robot = TRUE;
                    $this->robot = $val;
                    return TRUE;
                }
            }
        }
        return FALSE;
    }

    /**
     * Set the Mobile Device
     *
     * @access	private
     * @return	bool
     */
    private function setMobile()
    {
        if (is_array($this->mobiles) AND count($this->mobiles) > 0)
        {
            foreach ($this->mobiles as $key => $val)
            {
                if (FALSE !== (strpos(strtolower($this->agent), $key)))
                {
                    $this->is_mobile = TRUE;
                    $this->mobile = $val;
                    return TRUE;
                }
            }
        }
        return FALSE;
    }

    /**
     * Set the accepted languages
     *
     * @access	private
     * @return	void
     */
    private function setLanguages()
    {
        if ((count($this->languages) == 0) AND isset($_SERVER['HTTP_ACCEPT_LANGUAGE']) AND $_SERVER['HTTP_ACCEPT_LANGUAGE'] != '')
        {
            $languages = preg_replace('/(;q=[0-9\.]+)/i', '', strtolower(trim($_SERVER['HTTP_ACCEPT_LANGUAGE'])));
            $this->languages = explode(',', $languages);
        }
        if (count($this->languages) == 0)
        {
            $this->languages = array('Undefined');
        }
    }

    /**
     * Set the accepted character sets
     *
     * @access	private
     * @return	void
     */
    private function setCharsets()
    {
        if ((count($this->charsets) == 0) AND isset($_SERVER['HTTP_ACCEPT_CHARSET']) AND $_SERVER['HTTP_ACCEPT_CHARSET'] != '')
        {
            $charsets = preg_replace('/(;q=.+)/i', '', strtolower(trim($_SERVER['HTTP_ACCEPT_CHARSET'])));
            $this->charsets = explode(',', $charsets);
        }
        if (count($this->charsets) == 0)
        {
            $this->charsets = array('Undefined');
        }
    }

    /**
     * Is Browser
     *
     * @access	public
     * @return	bool
     */
    public function isBrowser($key = NULL)
    {
        if ( ! $this->is_browser)
        {
            return FALSE;
        }
        // No need to be specific, it's a browser
        if ($key === NULL)
        {
            return TRUE;
        }
        // Check for a specific browser
        return array_key_exists($key, $this->browsers) AND $this->browser === $this->browsers[$key];
    }

    /**
     * Is Robot
     *
     * @access	public
     * @return	bool
     */
    public function isRobot($key = NULL)
    {
        if ( ! $this->is_robot)
        {
            return FALSE;
        }
        // No need to be specific, it's a robot
        if ($key === NULL)
        {
            return TRUE;
        }
        // Check for a specific robot
        return array_key_exists($key, $this->robots) AND $this->robot === $this->robots[$key];
    }

    /**
     * Is Mobile
     *
     * @access	public
     * @return	bool
     */
    public function isMobile($key = NULL)
    {
        if ( ! $this->is_mobile)
        {
            return FALSE;
        }
        // No need to be specific, it's a mobile
        if ($key === NULL)
        {
            return TRUE;
        }
        // Check for a specific robot
        return array_key_exists($key, $this->mobiles) AND $this->mobile === $this->mobiles[$key];
    }


    /**
     * Is this a referral from another site?
     *
     * @access	public
     * @return	bool
     */
    public function isReferral()
    {
        if ( ! isset($_SERVER['HTTP_REFERER']) OR $_SERVER['HTTP_REFERER'] == '')
        {
            return FALSE;
        }
        return TRUE;
    }


    /**
     * Agent String
     *
     * @access	public
     * @return	string
     */
    public function getAgentString()
    {
        return $this->agent;
    }


    /**
     * Get Platform
     *
     * @access	public
     * @return	string
     */
    public function getPlatform()
    {
        return $this->platform;
    }

    /**
     * Get Browser Name
     *
     * @access	public
     * @return	string
     */
    public function getBrowser()
    {
        return $this->browser;
    }


    /**
     * Get the Browser Version
     *
     * @access	public
     * @return	string
     */
    public function getVersion()
    {
        return $this->version;
    }

    /**
     * Get The Robot Name
     *
     * @access	public
     * @return	string
     */
    public function getRobot()
    {
        return $this->robot;
    }

    /**
     * Get the Mobile Device
     *
     * @access	public
     * @return	string
     */
    public function getMobile()
    {
        return $this->mobile;
    }
    /**
     * Get the referrer
     *
     * @access	public
     * @return	bool
     */
    public function getReferrer()
    {
        return ( ! isset($_SERVER['HTTP_REFERER']) OR $_SERVER['HTTP_REFERER'] == '') ? '' : trim($_SERVER['HTTP_REFERER']);
    }
    /**
     * Get the accepted languages
     *
     * @access	public
     * @return	array
     */
    public function getLanguages()
    {
        if (count($this->languages) == 0)
        {
            $this->setLanguages();
        }
        return $this->languages;
    }
    /**
     * Get the accepted Character Sets
     *
     * @access	public
     * @return	array
     */
    public function getCharsets()
    {
        if (count($this->charsets) == 0)
        {
            $this->setCharsets();
        }
        return $this->charsets;
    }
    /**
     * Test for a particular language
     *
     * @access	public
     * @return	bool
     */
    public function acceptLanguage($lang = 'en')
    {
        return (in_array(strtolower($lang), $this->languages(), TRUE));
    }
    /**
     * Test for a particular character set
     *
     * @access	public
     * @return	bool
     */
    public function acceptCharset($charset = 'utf-8')
    {
        return (in_array(strtolower($charset), $this->charsets(), TRUE));
    }
}