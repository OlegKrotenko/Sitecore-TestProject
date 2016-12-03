using Library.Domain.Models;
using Library.Domain.ServicePassingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Interfaces
{
  public  interface IBookService
    {
        bool TakeBook(string userEmail, int bookId);
        void AddBook(Book book, params Author[] authors);
        void RemoveBook(int bookId);
        void ChangeBookQuantity(int bookId, int newQuantity);
        List<Book> GetAllBooks();
        List<Book> GetAllAvailableBooks();
        List<ServiceBookModel> GetBooksTakenByUser(string userEmail);
        List<History> GetAllHistories();
    }
}
