/* Copyright (c) 2013 - 2014 Jappi88 (Jappi88 at Gmail dot com)
*
* This(software Is provided) 'as-is', without any express or implied
* warranty. In no event will the authors be held liable for any damages arising from the use of this software.
*
* Permission is granted to anyone to use this software for any purpose,
* including commercial applications*, and to alter it and redistribute it
* freely, subject to the following restrictions:
*
* 1. The origin of this software must not be misrepresented; you must not
*   claim that you wrote the original software. If you use this software
*   in a product, an acknowledge in the product documentation is required.
*
* 2. Altered source versions must be plainly marked as such, and must not
*    be misrepresented as being the original software.
*
* 3. This notice may not be removed or altered from any source distribution.
*
* *Contact must be made to discuses permission and terms.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PS3FileSystem
{
    public class Ps3SaveManager
    {

        public Ps3SaveManager(string savedir)//, byte[] securefileid)
        {
            if (!Directory.Exists(savedir))
                throw new Exception("No such directory exist!");
            if (!File.Exists(savedir + "\\PARAM.PFD"))
                throw new Exception("Missing PARAM.PFD, Please load a valid directory");
            if (!File.Exists(savedir + "\\PARAM.SFO"))
                throw new Exception("Missing PARAM.SFO, Please load a valid directory");
            Param_PFD = new Param_PFD(savedir + "\\PARAM.PFD");
            Param_SFO = new PARAM_SFO(savedir + "\\PARAM.SFO");

            RootPath = savedir;
            if (File.Exists(savedir + "\\ICON0.PNG"))
            {
                //prevent file lock,reading to memory instead.
                SaveImage = Image.FromStream(new MemoryStream(File.ReadAllBytes(savedir + "\\ICON0.PNG")));
            }

            Files = (from ent in Param_PFD.Entries
                let x = new FileInfo(savedir + "\\" + ent.file_name)
                where x.Extension.ToUpper() != ".PFD" && x.Extension.ToUpper() != ".SFO"
                select new Ps3File(savedir + "\\" + ent.file_name, ent, this)).ToArray();
        }

        public string RootPath { get; }
        public Param_PFD Param_PFD { get; }
        public PARAM_SFO Param_SFO { get; private set; }
        public Ps3File[] Files { get; private set; }
        public Image SaveImage { get; private set; }

        public int DecryptAllFiles()
        {
            Console.WriteLine("Decrypting.");
            try
            {
                if (Param_PFD == null || !Directory.Exists(RootPath))
                    return -1;
                return Param_PFD.DecryptAllFiles(RootPath);
            }
            catch
            {
                return -1;
            }
        }

        public int EncryptAllFiles()
        {
            Console.WriteLine("Encrypting.");

            try
            {
                if (Param_PFD == null || !Directory.Exists(RootPath))
                    return -1;
                var x = Param_PFD.EncryptAllFiles(RootPath);
                if (x > 0)
                    return x;
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        public bool ReBuildChanges()
        {
            return Param_PFD.RebuilParamPFD(RootPath, false);
        }

        public bool ReBuildChanges(bool encryptfiles)
        {
            return Param_PFD.RebuilParamPFD(RootPath, encryptfiles);
        }

        /*
        public int LoadGameConfigFile(string filepath, SecureFileInfo[] GameConfigList)
        {
            try
            {
                var text = "";
                using (
                    var sr = new StreamReader(new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    text = sr.ReadToEnd();
                    sr.Close();
                }
                return (GameConfigList = Functions.ReadConfigFromtext(text)).Length;
            }
            catch
            {
                return -1;
            }
        }
        */

        /*
        private byte[] GetSecureFileIdFromConfigFile(string titleid, SecureFileInfo[] GameConfigList)
        {
            if (GameConfigList == null || GameConfigList.Length == 0)
                return null;
            return (from i in GameConfigList
                from s in i.GameIDs
                where s.ToLower() == titleid.ToLower()
                where i.SecureFileID != null && i.SecureFileID.Length == 32
                select i.SecureFileID.StringToByteArray()).FirstOrDefault();
        }
        */
       
        /*
        public byte[] GetSecureFileIdFromConfigList(SecureFileInfo[] GameConfigList)
        {
            if (GameConfigList == null || GameConfigList.Length == 0)
                return null;

            bool b1 = false; byte[] securefileid = null;

            foreach (var ent in GameConfigList)
            {
                b1 = ent.Name.Contains(Param_SFO.Title) || ent.GameIDs.Contains(Param_SFO.TitleID.Substring(0, 9));
                if (b1)
                {
                    Console.WriteLine("Found match in database!");
                    var s1 = ent.SecureFileID;
                    if (s1 != null)
                    {
                        Console.WriteLine(s1);
                        securefileid = Functions.StringToByteArray(s1);
                    }
                    else
                    {
                        Console.WriteLine("Game is not protected / sfid is null!");
                    }
                    break;
                }
            }

            if (!b1)
            {
                Console.WriteLine("Game is not found in database!");
            }

            return securefileid;
        }
        */

        public byte[] GetSecureFileId2(string cfg)
        {
            var s1 = GetSecureFileIdFromConfig(cfg);
            Console.WriteLine("sfid: " + s1);

            var d1 = new Dictionary<string, string> {
        {"NOTFOUND", "game is not found in " + cfg},
        {"","null sfid"},
        {"UNPROTECTED","game is not protected"}
        };

            // ^ probably overkill, but less text than for jagged array

            foreach (var i in d1)
            {
                if (s1 == i.Key)
                {
                    Console.WriteLine(i.Value);
                    return null;
                }
            }

            return Functions.StringToByteArray(s1);
        }

        public string GetSecureFileIdFromConfig(string cfg)
        {
            var sr = File.OpenText(cfg);

            string s1;

            while (sr.ReadLine() != "; -- UNPROTECTED GAMES --")
            {
            }

            while ((s1 = sr.ReadLine()) != "")
            {
                if (s1.Contains(Param_SFO.Title))
                {
                    sr.Close();
                    return "UNPROTECTED";
                }
            }

            var d1 = new Dictionary<string, string> {
                { "title", "; \"" },
                { "id", "[" },
                { "dhk", ";disc_hash_key=" },
                { "sfid", "secure_file_id:*=" }
            };

            var d2 = new Dictionary<string, string>(d1); // todo: init with null values

            while (sr.Peek() > -1)
            {
                s1 = sr.ReadLine();
                foreach (var j in d1)
                {
                    if (!s1.Contains(j.Value))
                    {
                        continue;
                    }

                    if (j.Value.Contains("=")) s1 = s1.Split('=')[1];
                    d2[j.Key] = s1;

                    if (j.Key == "sfid")
                    {
                        if (d2["title"].Contains(Param_SFO.Title) || d2["id"].Contains(Param_SFO.TitleID.Substring(0, 9)))
                        {
                            sr.Close();
                            return s1;
                        }
                        d2 = new Dictionary<string, string>(d1);
                    }
                    break;
                }
            }

            sr.Close();

            return "NOTFOUND";
        }

        /*
        public bool setsfid(byte [] securefileid)
        {
            //var securefileid = GetSecureFileIdFromConfigFile_2();

            if (securefileid != null)
            {
                Console.WriteLine("Assigned sfid!");
                Param_PFD.SecureFileID = securefileid;
                return true;
            }
            else
            {
                Console.WriteLine("null sfid not assigned!");
                return false;
            }
            
            
        }
        */

    }
}