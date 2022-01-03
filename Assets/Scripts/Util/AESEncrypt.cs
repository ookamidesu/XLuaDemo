/*
* @Author: name
* @LastEditors: name
* @Description:
* @Date: ${YEAR}-${MONTH}-${DAY} ${TIME}
* @Modify:
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace XLuaDemo
{
    public class AESEncrypt
    {
        private static string AESHead = "OOKAMI";

        private static string encrptyKey = "ML4qZD75gdS8aLc4";


        /// <summary>
        /// 文件加密，传入文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="EncrptyKey"></param>
        public static void AESFileEncrypt(string path)
        {
            if (!File.Exists(path))
                return;

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    if (fs != null)
                    {
                        //读取字节头，判断是否已经加密过了
                        byte[] headBuff = new byte[6];
                        fs.Read(headBuff, 0, headBuff.Length);
                        string headTag = Encoding.UTF8.GetString(headBuff);
                        if (headTag == AESHead)
                        {
#if UNITY_EDITOR
                            Debug.Log(path + "已经加密过了！");
#endif
                            return;
                        }

                        //加密并且写入字节头
                        fs.Seek(0, SeekOrigin.Begin);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.SetLength(0);
                        byte[] headBuffer = Encoding.UTF8.GetBytes(AESHead);
                        fs.Write(headBuffer, 0, headBuffer.Length);
                        byte[] EncBuffer = Encrypt(buffer);
                        fs.Write(EncBuffer, 0, EncBuffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 文件解密，传入文件路径（会改动加密文件，不适合运行时）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="EncrptyKey"></param>
        public static void AESFileDecrypt(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    if (fs != null)
                    {
                        byte[] headBuff = new byte[10];
                        fs.Read(headBuff, 0, headBuff.Length);
                        string headTag = Encoding.UTF8.GetString(headBuff);
                        if (headTag == AESHead)
                        {
                            byte[] buffer = new byte[fs.Length - headBuff.Length];
                            fs.Read(buffer, 0, Convert.ToInt32(fs.Length - headBuff.Length));
                            fs.Seek(0, SeekOrigin.Begin);
                            fs.SetLength(0);
                            byte[] DecBuffer = Decrypt(buffer);
                            fs.Write(DecBuffer, 0, DecBuffer.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        /// <summary>
        /// 文件界面，传入文件路径，返回字节
        /// </summary>
        /// <returns></returns>
        public static byte[] AESFileByteDecryptAB(string path)
        {
            byte[] DecBuffer = null;
            try
            {
                byte[] allBytes = GetBytesForStreamingAssets(path);
                byte[] headBuff = new byte[6];
                byte[] contentBuff = new byte[allBytes.Length - 6];
                Array.Copy(allBytes, headBuff, 6);
                Array.Copy(allBytes, 6, contentBuff, 0, allBytes.Length - 6);

                string headTag = Encoding.UTF8.GetString(headBuff);
                if (headTag == AESHead)
                {
                    DecBuffer = Decrypt(contentBuff);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return DecBuffer;
        }

        /// <summary>
        ///读取StreamingAssets中的文件 
        /// </summary>
        /// <param name="path">StreamingAssets下的文件路径</param>
        /// <returns>读取到的字符串</returns>
        public static byte[] GetBytesForStreamingAssets(string path)
        {
            byte[] bytes = null;
#if NORMAL_PC || STEAM_PC || WEGAME_PC || UNITY_EDITOR
            bytes = ReadFileByWindows(path);
#elif ANDROID_MOBILE
             bytes = ReadFileByAndroid(path);
#endif
            if (bytes == null)
            {
                Debug.LogError("读取文件出错");
            }

            return bytes;
        }

        private static byte[] ReadFileByWindows(string path)
        {
            /*byte[] bytes = null;
            WWW www = new WWW(path);
            while (!www.isDone) { }

            bytes = www.bytes;

            return bytes;*/
            
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                return buffer;
            }
        }
        
#if ANDROID_MOBILE
        private static byte[] ReadFileByAndroid(string path)
        {
            //return AndroidSDK.AndroidSDKManager.Instance.LoadFileFromStreamingAssets(path);
            byte[] bytes = null;
            WWW www = new WWW(path);
            while (!www.isDone) { }
            bytes = www.bytes;

            return bytes;

        }
#endif
      

        /// <summary>
        /// AES 加密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
        /// </summary>
        /// <param name="EncryptString">待加密密文</param>
        public static string Encrypt(string EncryptString)
        {
            return Convert.ToBase64String(Encrypt(Encoding.Default.GetBytes(EncryptString)));
        }

        /// <summary>
        /// AES 加密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
        /// </summary>
        /// <param name="EncryptString">待加密密文</param>
        /// <param name="EncryptKey">加密密钥</param>
        public static byte[] Encrypt(byte[] EncryptByte)
        {
            if (EncryptByte.Length == 0)
            {
                throw (new Exception("明文不得为空"));
            }

            if (string.IsNullOrEmpty(encrptyKey))
            {
                throw (new Exception("密钥不得为空"));
            }

            byte[] m_strEncrypt;
            byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
            byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
            Rijndael m_AESProvider = Rijndael.Create();
            try
            {
                MemoryStream m_stream = new MemoryStream();
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(encrptyKey, m_salt);
                ICryptoTransform transform = m_AESProvider.CreateEncryptor(pdb.GetBytes(32), m_btIV);
                CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
                m_csstream.Write(EncryptByte, 0, EncryptByte.Length);
                m_csstream.FlushFinalBlock();
                m_strEncrypt = m_stream.ToArray();
                m_stream.Close();
                m_stream.Dispose();
                m_csstream.Close();
                m_csstream.Dispose();
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                m_AESProvider.Clear();
            }

            return m_strEncrypt;
        }


        /// <summary>
        /// AES 解密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
        /// </summary>
        /// <param name="decryptString">待解密密文</param>
        public static string Decrypt(string decryptString)
        {
            return Convert.ToBase64String(Decrypt(Encoding.Default.GetBytes(decryptString)));
        }

        /// <summary>
        /// AES 解密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
        /// </summary>
        /// <param name="DecryptString">待解密密文</param>
        public static byte[] Decrypt(byte[] decryptByte)
        {
            if (decryptByte.Length == 0)
            {
                throw (new Exception("密文不得为空"));
            }

            if (string.IsNullOrEmpty(encrptyKey))
            {
                throw (new Exception("密钥不得为空"));
            }

            byte[] m_strDecrypt;
            byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
            byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
            Rijndael m_AESProvider = Rijndael.Create();
            try
            {
                MemoryStream m_stream = new MemoryStream();
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(encrptyKey, m_salt);
                ICryptoTransform transform = m_AESProvider.CreateDecryptor(pdb.GetBytes(32), m_btIV);
                CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
                m_csstream.Write(decryptByte, 0, decryptByte.Length);
                m_csstream.FlushFinalBlock();
                m_strDecrypt = m_stream.ToArray();
                m_stream.Close();
                m_stream.Dispose();
                m_csstream.Close();
                m_csstream.Dispose();
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                m_AESProvider.Clear();
            }

            return m_strDecrypt;
        }
    }
}