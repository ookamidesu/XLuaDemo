using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Util 
{
    public static byte[] GetFileBytes(string inFile)
    {
        List<int> data = null;
        //data.ElementAt()
        try
        {
            if (string.IsNullOrEmpty(inFile))
            {
                return null;
            }
            

            if (!File.Exists(inFile))
            {
                return null;
            }
            return File.ReadAllBytes(inFile);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeReadAllBytes failed! path = {0} with err = {1}", inFile, ex.Message));
            return null;
        }
    }
}
