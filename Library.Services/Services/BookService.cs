using Library.Repository;
using Library.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Models;
using Library.Domain.ServicePassingModels;

namespace Library.Services.Services
{
    public class BookService : IBookService
    {
        private readonly ILibraryRepository _libraryRepository;

        // summary
        // Initializes a new instance of the BookService class.
        // Parameters:
        //   libraryRepository:
        //   Instance of class which implements ILibraryRepository
        public BookService(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public void AddBook(Book book, params Author[] authors)
        {
            // summary
            // adding new book to repository
            _libraryRepository.AddBook(book, authors);
        }

        public void ChangeBookQuantity(int bookId, int newQuantity)
        {
            // summary
            // change book quantity in repository
            _libraryRepository.ChangeBookQuantity(bookId, newQuantity);
        }

        public List<Book> GetAllAvailableBooks()
        {
            // summary
            // returns all authros from repository
            return _libraryRepository.GetAllAvailableBooks();
        }

        public List<Book> GetAllBooks()
        {
            // summary
            // returns all books from repository
            return _libraryRepository.GetAllBooks();
        }

        public List<History> GetAllHistories()
        {
            // summary
            // returns all operation history from repository
            return _libraryRepository.GetAllHistories();
        }
        public List<ServiceBookModel> GetBooksTakenByUser(string userEmail)
        {
            // summary
            // returns list of books taken by user
            return _libraryRepository.GetBooksTakenByUser(userEmail);
        }
        public void RemoveBook(int bookId)
        {
            // summary
            // remove book from repository
            _libraryRepository.RemoveBook(bookId);
        }
        public bool TakeBook(string userEmail, int bookId)
        {
            // summary
            // allows to take book by user
            return _libraryRepository.TakeBook(userEmail, bookId);
        }
    }
}
