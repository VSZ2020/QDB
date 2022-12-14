using QDB.Controllers;
using QDB.Database.Configurations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Models
{
    public class QDbChapter
    {
        public int Id { get; set; }
        public string Header { get; set; } = String.Empty;
        //public List<QDbSection> Sections { get; set; }
        public static QDbChapter AddGeneralChapter()
        {
            return new QDbChapter() { Id = QDatabaseConfig.AllCategoriesId, Header = "Все разделы" };
        }
        public static List<QDbChapter> AddServiceFields()
        {
            return new List<QDbChapter>() { 
                new QDbChapter(){Id = Configuration.ServiceFieldId_Add, Header = "<Add chapter>"},
                new QDbChapter(){Id = Configuration.ServiceFieldId_EditAll, Header = "<Edit chapters>"}
            };
        }
    }
}
