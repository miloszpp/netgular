using System;
using System.Collections.Generic;

namespace Netgular.Examples.WebApi
{
    public class Book
    {
        public string Title
        {
            get;
            set;
        }

        public int PageCount
        {
            get;
            set;
        }

        public IEnumerable<string> Pages { get; set; }
    }
}
