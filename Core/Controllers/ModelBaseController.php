<?php
namespace Core\Controllers;


class ModelBaseController extends BaseController
{

    private function folder_exist($folder)
    {
        // Get canonicalized absolute pathname
        $path = realpath($folder);

        // If it exist, check if it's a directory
        return ($path !== false AND is_dir($path)) ? $path : false;
    }
    public function moveUploadedFile($uploaded_file,$path, $file)
    {
        if(!$this->folder_exist($path))
            mkdir($path,0777, true);

        return move_uploaded_file($uploaded_file, $path.$file);

    }
}