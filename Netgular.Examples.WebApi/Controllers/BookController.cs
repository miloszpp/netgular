using System;

using System.Web.Http;
using System.Collections;
using System.Collections.Generic;

namespace Netgular.Examples.WebApi
{
    [RoutePrefix("Books")]
    public class BookController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IEnumerable<Book> GetBooks()
        {
            return new Book[] {
                new Book { Title = "Book A", PageCount = 100 }
            };
        }
    }
}
