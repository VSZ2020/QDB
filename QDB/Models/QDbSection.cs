using QDB.Controllers;
using QDB.Database.Configurations;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Models
{
    public class QDbSection
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public string Header { get; set; } = string.Empty;

        //public QDbChapter? Chapter { get; set; }
        public static QDbSection AddGeneralSection()
        {
            return new QDbSection() { Id = QDatabaseConfig.AllCategoriesId, ChapterId = QDatabaseConfig.AllCategoriesId, Header = "Все подразделы" };
        }
        public static List<QDbSection> AddServiceFields()
        {
            return new List<QDbSection>() {
                new QDbSection(){Id = Configuration.ServiceFieldId_Add, ChapterId = -1, Header = "<Add section>"}/*,
                new QDbSection(){Id = Configuration.ServiceFieldId_EditAll, ChapterId = -1, Header = "<Edit sections>"}*/
            };
        }
        
    }
}
