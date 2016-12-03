using Library.Domain.Models;
using Library.Services;
using Library.Services.Interfaces;
using Library.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Library.UI.Controllers
{

    public class LibraryController : Controller
    {
        readonly private IBookService _bookService;
        readonly private IAuthorService _authorService;

        public LibraryController(IBookService bookService,
           IAuthorService authorService)
        {
            _bookService = bookService;
            _authorService = authorService;
        }
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                List<Book> all = _bookService.GetAllBooks().ToList();
                ModelState.Clear();
                List<Author> authorsList = _authorService.GetAllAuthros().ToList();
                //TempData["authros"] = authorsList;
                return View(all);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpGet]
        public PartialViewResult Histories()
        {
            return PartialView("_Histories", _bookService.GetAllHistories().ToArray());
        }
        [Authorize]
        [HttpGet]
        public PartialViewResult AddNewBook()
        {
            return PartialView("_AddNewBook", _authorService.GetAllAuthros());
        }
        [Authorize]
        [HttpGet]
        public PartialViewResult Delete(int? id)
        {
            if (id == null)
            {
                return PartialView("_LibraryBooks", _bookService.GetAllBooks().ToList());
            }
            else
            {
                _bookService.RemoveBook((int)id);
                ModelState.Clear();
                return PartialView("_LibraryBooks", _bookService.GetAllBooks().ToList());
            }
        }
        [Authorize]
        [HttpGet]
        public PartialViewResult SetQuantity(int? id, int quantity)
        {
            if (id == null || quantity < 0)
            {
                return PartialView("_LibraryBooks", _bookService.GetAllBooks().ToList());
            }
            else
            {

                _bookService.ChangeBookQuantity((int)id, (int)quantity);
                return PartialView("_LibraryBooks", _bookService.GetAllBooks().ToList());
            }
        }
        [Authorize]
        [HttpPost]
        public JsonResult AddNewBook(Book book, string[] authorsArray)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<Author> authorArray = new List<Author>();
                    for (int i = 0; i < authorsArray.Length; i++)
                    {
                        authorArray.Add(new Author() { Name = authorsArray[i] });
                    }
                    _bookService.AddBook(book, authorArray.ToArray());
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "Library");
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    throw new Exception("Model isn't valid");
                }

            }
            catch (Exception ex)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Error", "Library", ex.Message);
                return Json(new { Url = redirectUrl });
            }

        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> TakeBook(Book book)
        {
            try
            {
                _bookService.TakeBook(User.Identity.Name, book.Id);
                var result = await SendMailNotification(book);
                if (!result)
                {
                    throw new Exception("Can't send mail");
                }
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "Library");
                return Json(new { Url = redirectUrl });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }

        }

        private async Task<bool> SendMailNotification(Book book)
        {
            var fromMail = ConfigurationManager.AppSettings["ownerMail"];
            var fromPassword = ConfigurationManager.AppSettings["ownerPassword"];
            var model = new EmailModel()
            {
                Body = $"You took the {book.Title} books in our library",
                From = fromMail,
                FromPassword = fromPassword,
                To = fromMail,//User.Identity.Name,
                Subject = "About books"
            };
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(model.From);
                mail.To.Add(new MailAddress(model.From));
                mail.Subject = model.Subject;
                mail.Body = model.Body;

                using (SmtpClient client = new SmtpClient())
                {
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(model.From.Split('@')[0], model.FromPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await client.SendMailAsync(mail);
                }
                return true;
            }
            return false;
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Error(string message)
        {
            var error = new ErrorViewModel() { Message = message };
            return View(error);
        }
    }
}