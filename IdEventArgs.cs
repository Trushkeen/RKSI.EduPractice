using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKSI.EduPractice
{
    public class IdEventArgs
    {
        public IdEventArgs() { }
        public IdEventArgs(int ctzId, int persId, int docId) {
            AvailableCtzId = ctzId;
            AvailablePersId = persId;
            AvailableDocId = docId;
        }
        public int AvailableCtzId { get; set; }
        public int AvailablePersId { get; set; }
        public int AvailableDocId { get; set; }
    }
}
