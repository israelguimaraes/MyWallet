﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWallet.Web.Controllers
{
    [Authorize]
    public class BankAccountController : BaseController
    {
        // GET: BankAccount
        public ActionResult Create()
        {
            return View();
        }
    }
}