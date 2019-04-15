using System;
using System.Collections.Generic;
using System.Linq;

namespace RKSI.EduPractice
{
    [Serializable]
    public class Citizen : IComparable
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronym { get; set; }
        public string FullName => Surname + " " + Name + " " + Patronym;

        public int PId { get; set; }
        public uint Cypher { get; set; }
        public uint Inn { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string ShortDate => Date.ToShortDateString();

        public int DId { get; set; }
        public string DocumentName { get; set; }
        public uint DocumentSerial { get; set; }
        public string DocumentWhereIssued { get; set; }
        public DateTime DocumentDateIssued { get; set; }
        public string DocumentShortDate => DocumentDateIssued.ToShortDateString();

        public override string ToString()
        {
            return $"{Id}. {Name} {Surname} {Patronym}";
        }

        public static IdEventArgs GetFreeId(List<Citizen> users)
        {
            List<int> ctzIds = new List<int>();
            List<int> persIds = new List<int>();
            List<int> docIds = new List<int>();
            foreach (var i in users)
            {
                ctzIds.Add((int)i.Id);
                persIds.Add(i.PId);
                docIds.Add(i.DId);
            }
            return new IdEventArgs(ctzIds.Max() + 1, persIds.Max() + 1, docIds.Max() + 1);
        }

        public int CompareTo(object obj)
        {
            if (obj is Citizen ctz)
            {
                if (Id < ctz.Id) return -1;
                if (Id == ctz.Id) return 0;
                if (Id > ctz.Id) return 1;
            }
            return 0;
        }
    }
}
