using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text;

namespace RKSI.EduPractice
{
    class LocalUsersDb
    {
        public LocalUsersDb()
        {
        }

        public async void SaveLocalDb(List<Citizen> users, string path)
        {
            using (var sw = new StreamWriter(path + ".dbcnt", false, Encoding.Default))
            {
                try
                {
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                }
                catch (ArgumentException) { return; }
                XmlSerializer ser = new XmlSerializer(typeof(Citizen));
                foreach (var i in users)
                {
                    using (FileStream fs = new FileStream(path + "\\" + i.Id + "-" + i.Inn + "-" + i.Surname, FileMode.Create))
                    {
                        ser.Serialize(fs, i);
                    }
                    await sw.WriteLineAsync(i.Id + "-" + i.Inn + "-" + i.Surname);
                }
            }
        }

        public List<Citizen> LoadLocalDb(string path)
        {
            List<string> filenames = new List<string>();
            List<Citizen> users = new List<Citizen>();
            XmlSerializer ser = new XmlSerializer(typeof(Citizen));
            using (var sr = new StreamReader(path, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    filenames.Add(sr.ReadLine());
                }
            }
            foreach (var pathToFile in filenames)
            {
                string pathFs = path.Remove(path.LastIndexOf('.')) + @"\" + pathToFile;
                FileStream fs = new FileStream(pathFs, FileMode.Open);
                users.Add(ser.Deserialize(fs) as Citizen);
            }
            return users;
        }
    }
}
