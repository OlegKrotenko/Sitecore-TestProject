using Library.Repository;
using Library.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Models;

namespace Library.Services.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ILibraryRepository _libraryRepository;

        // summary
        // Initializes a new instance of the AuthorService class.
        // Parameters:
        //   libraryRepository:
        //   Instance of class which implements ILibraryRepository
        public AuthorService(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }


        public List<Author> GetAllAuthros()
        {
            // summary
            // returns all authros from repository
            return _libraryRepository.GetAllAuthros();
        }
    }
}
