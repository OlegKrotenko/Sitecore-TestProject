﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.UI.Models
{
    public class EmailModel
    {
        public string Subject { get; set; }

        public string From { get; set; }
        public string FromPassword { get; set; }
        public string To { get; set; }

        public string Body { get; set; }
    }
}