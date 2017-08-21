using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vasily;

namespace Demo
{
    [Table("Student")]
    public class Test : IVasily
    {
        public static string TString;
        [PrimaryKey]
        public int ID { get; set; }
        public int? Age { get; set; }
        public string Name { get; set; }

        //[Column("Map Descrption")]
        //public string Dest { get; set; }


        public string Address { get; set; }

        [Ignore]
        public string Address1 { get; set; }
        [NoRepeate]
        public string Address2 { get; set; }

        public void SetAddress(string i)
        {
            Address = i;
        }

        public DateTime? CreateTime { get; set; }

    }
}
