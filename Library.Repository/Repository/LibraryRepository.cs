using Library.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.ServicePassingModels;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Library.Repository
{
    // summary
    // Defines implementation of the ILibraryRepository contract.
    //
    public class LibraryRepository : ILibraryRepository
    {
        // summary
        // Defines DataBase stored procedures and functions as ENUM.
        //
        enum DataBaseCommands
        {
            sp_AddBook,
            sp_ChangeBookQuantity,
            sp_DeleteBook,
            sp_RegisterNewUser,
            fc_ShowAllBooks,
            fc_BookAuthors,
            fc_ShowAvailableBooks,
            fc_ShowHistory,
            fc_UserBooksTaken,
            fc_GetAllAuhtors,
            fc_LoginValidation,
            sp_TakeBook
        };
        // summary
        // Defines connectionString for DataBase connection.
        //
        private readonly string sqlConnection;
        public LibraryRepository()
        {

            sqlConnection = ConfigurationManager.ConnectionStrings["LibraryConnectionString"].ConnectionString;
        }

        //
        // Summary:
        //     Adds new book to DataBase
        //
        // Parameters:
        //   book:
        //     An object of a Book class.
        //   authors:
        //      Array of authors of this book.
        public void AddBook(Book book, params Author[] authors)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                try
                {                    
                    con.Open();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("id");
                    dt.Columns.Add("name");
                    if (authors.Length == 1 && authors[0].Name == "")
                    {
                        authors[0].Name = "Unknown Author";
                    }
                    for (int i = 0; i < authors.Length; i++)
                    {
                        dt.Rows.Add(authors[i].Id, authors[i].Name);
                    }
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_AddBook.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@title", book.Title));
                    cmd.Parameters.Add(new SqlParameter("@quantity", book.Quantity));
                    SqlParameter tvparam = cmd.Parameters.AddWithValue("@List", dt);
                    tvparam.SqlDbType = SqlDbType.Structured;
                    int count = cmd.ExecuteNonQuery();
                    Console.WriteLine(count);
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        //
        // Summary:
        //    Changes the quantity of book that exists
        //
        // Parameters:
        //   bookId:
        //   Id of book.
        //   newQuantity:
        //     new value of book quantity.
        public void ChangeBookQuantity(int bookId, int newQuantity)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_ChangeBookQuantity.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(new SqlParameter[] { new SqlParameter("@id", bookId), new SqlParameter("@newQuantity", newQuantity) });
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        //
        // Summary:
        //   Returns list of all available books from Library DataBase
        //
        public List<Book> GetAllAvailableBooks()
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                List<Book> books = new List<Book>();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_ShowAvailableBooks.ToString()}()";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        books.Add(new Book() { Id = (int)reader[0], Title = (string)reader[1], Quantity = (int)reader[2] });
                    }
                    return books;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        //
        // Summary:
        //   Returns list of all books from Library DataBase
        //
        public List<Book> GetAllBooks()
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                List<Book> books = new List<Book>();
                try
                {
                    //fc_BookAuthors
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_ShowAllBooks.ToString()}()";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        books.Add(new Book() { Id = (int)reader[0], Title = (string)reader[1], Quantity = (int)reader[2] });
                        using (SqlConnection authorsCon = new SqlConnection(sqlConnection))
                        {
                            authorsCon.Open();

                            SqlCommand authorsCmd = new SqlCommand();
                            authorsCmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_BookAuthors.ToString()}(@bookId)";
                            authorsCmd.Parameters.Add(new SqlParameter("@bookId", books[books.Count - 1].Id));
                            authorsCmd.CommandType = CommandType.Text;
                            authorsCmd.Connection = authorsCon;
                            SqlDataReader authorsReader = authorsCmd.ExecuteReader();
                            while (authorsReader.Read())
                            {
                                books[books.Count - 1].Authors.Add(new Author() { Id = (int)authorsReader[0], Name = (string)authorsReader[1] });
                            }
                        }

                    }
                    return books;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        //
        // Summary:
        //   Returns list of all user's operations from Library DataBase
        //
        public List<History> GetAllHistories()
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                List<History> histories = new List<History>();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_ShowHistory.ToString()}()";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        histories.Add(new History() { Id = (int)reader[0], BookTitle = (string)reader[1], UserEmail = (string)reader[2], PicDate = (DateTime)reader[3] });
                    }
                    return histories;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        //
        // Summary:
        //   Returns list of books taken by user from Library DataBase
        //
        // Parameters:
        //   userEmail:
        //   email that allows to get list of book taken by current userEmail
        public List<ServiceBookModel> GetBooksTakenByUser(string userEmail)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                List<ServiceBookModel> books = new List<ServiceBookModel>();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_UserBooksTaken.ToString()}(@userEmail)";
                    //SqlParameter param = new SqlParameter();
                    ////param.ParameterName = "@userEmail";
                    ////param.Value = userEmail;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@userEmail", userEmail));
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        books.Add(new ServiceBookModel() { Id = (int)reader[0], Title = (string)reader[1], PicDate = (DateTime)reader[2] });
                    }
                    return books;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        //
        // Summary:
        //   Returns true if user registration was success or false if it wasn't
        //
        // Parameters:
        //   user:
        //   represents new user for registration in Library DataBase 
        public bool RegistrationNewUser(User user)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                try
                {
                    if (!EmailValidation.IsValidEmail(user.Email))
                    {
                        throw new Exception("Your email isn't valid");
                    }
                    con.Open();
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_RegisterNewUser.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@email", user.Email));
                    int row = cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        //
        // Summary:
        //   Remove book from Library DataBase
        //
        // Parameters:
        //   bookId:
        //   represents id of deleting book
        public void RemoveBook(int bookId)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_DeleteBook.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id", bookId));
                    int row = cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        //
        // Summary:
        //   Take book by user from Library DataBase 
        //
        // Parameters:
        //   userEmail:
        //   email that will be recorded in Library DataBase when the user takes a book
        //   bookId:
        //   id of book that will be taken by user
        public bool TakeBook(string userEmail, int bookId)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_TakeBook.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@email", userEmail));
                    cmd.Parameters.Add(new SqlParameter("@bookId", bookId));
                    int row = cmd.ExecuteNonQuery();
                    if (row > 0)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            return false;

        }
        //
        // Summary:
        //   Returns list of all authors from Library DataBase
        //
        public List<Author> GetAllAuthros()
        {

            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                List<Author> authrorsList = new List<Author>();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_GetAllAuhtors.ToString()}()";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        authrorsList.Add(new Author() { Id = (int)reader[0], Name = (string)reader[1] });
                    }
                    return authrorsList;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //
        // Summary:
        //   Allows user to login on site
        //
        // Parameters:
        //   user:
        //   object of User class that contains data for logining
        public bool Login(User user)
        {
            if (!EmailValidation.IsValidEmail(user.Email))
            {
                throw new Exception("Your email isn't valid");
            }
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select dbo.{DataBaseCommands.fc_LoginValidation.ToString()}(@email)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@email", user.Email));
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[0] == null)
                        {
                            return false;
                        }
                        return true;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return false;
            }
        }
        //
        // Summary:
        //   Class for email validation when new user registers
        //
        static class EmailValidation
        {
            //
            // Summary:
            //   Check new users email
            //
            // Parameters:
            //   email:
            //   new user email that will be checked
            public static bool IsValidEmail(string email)
            {
                Regex emailValidation = new Regex(@"\w{2,15}@\w{1,5}\.\w{1,4}");
                return emailValidation.IsMatch(email);
            }
        }

    }
}
