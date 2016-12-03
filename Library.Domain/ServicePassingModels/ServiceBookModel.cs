using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.ServicePassingModels
{
    //
    // Summary:
    //     Represents the class for passing data from service and repository.
    //      Contains information about Book title and when it was taken
    //
    public class ServiceBookModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PicDate { get; set; }
    }
}
