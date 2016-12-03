using Library.Domain.Models;
using Library.Repository;
using Library.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Services
{
    public class UserService : IUserService
    {
        private readonly ILibraryRepository _libraryRepository;

        // summary
        // Initializes a new instance of the UserService class.
        // Parameters:
        //   libraryRepository:
        //   Instance of class which implements ILibraryRepository
        public UserService(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public bool Login(User user)
        {
            // summary
            // loggining user
            return _libraryRepository.Login(user);
        }

        public bool RegistrationNewUser(User user)
        {
            // summary
            // registration new user in repository
            return _libraryRepository.RegistrationNewUser(user);
        }
    }
}
