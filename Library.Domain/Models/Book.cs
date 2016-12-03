using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Models
{
    //
    // Summary:
    //     Represents DataBase table Books records as class Book.
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public List<Author> Authors { get; set; } = new List<Author>();

    }
}
