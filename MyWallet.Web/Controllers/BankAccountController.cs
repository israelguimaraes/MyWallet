﻿using MyWallet.Data.Domain;
using MyWallet.Service;
using MyWallet.Web.ViewModels.BankAccount;
using System;
using System.Linq;
using System.Web.Mvc;


namespace MyWallet.Web.Controllers
{
    [Authorize]
    public class BankAccountController : BaseController
    {
        private BankAccountService _bankAccountService;

        public BankAccountController()
        {
            _bankAccountService = new BankAccountService();
        }


        public ActionResult Index()
        {
            var list = _bankAccountService.GetAll();
            var listViewModel = new ListAllBankAccountsViewModel();
            foreach (var bankAccount in list)
            {
                var viewModel = new BankAccountViewModel()
                {
                    Id = bankAccount.Id,
                    Name = bankAccount.Name
                };

                listViewModel.BankAccounts.Add(viewModel);
            }

            return View(listViewModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(BankAccountViewModel bankAccountViewModel)
        {
            if (ModelState.IsValid)
            {
                var bankAccount = new BankAccount();
                bankAccount.Name = bankAccountViewModel.Name;
                bankAccount.OpeningBalance = bankAccountViewModel.OpeningBalance.Value;
                bankAccount.ContextId = GetCurrentContextId();
                bankAccount.CreationDate = DateTime.Now;

                _bankAccountService.Add(bankAccount);

                return RedirectToAction("Index");
            }
            else
            {
                SendModelStateErrors();
                return View(bankAccountViewModel);
            }
        }

        public ActionResult Edit(int id)
        {
            var bankAccount = _bankAccountService.GetById(id);
            var viewModel = new BankAccountViewModel();
            viewModel.Id = bankAccount.Id;
            viewModel.Name = bankAccount.Name;
            viewModel.OpeningBalance = bankAccount.OpeningBalance;
            viewModel.ContextId = bankAccount.ContextId;
            viewModel.CreationDate = bankAccount.CreationDate;

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Edit(BankAccountViewModel bankAccountViewModel)
        {
            if (ModelState.IsValid)
            {
                var bankAccount = new BankAccount();
                bankAccount.Id = bankAccountViewModel.Id;
                bankAccount.Name = bankAccountViewModel.Name;
                bankAccount.OpeningBalance = bankAccountViewModel.OpeningBalance.Value;
                bankAccount.ContextId = bankAccountViewModel.ContextId;
                bankAccount.CreationDate = bankAccountViewModel.CreationDate;

                _bankAccountService.Update(bankAccount);

                return RedirectToAction("Index");
            }
            else
            {
                SendModelStateErrors();
                return View(bankAccountViewModel);
            }

        }

        public ActionResult Delete(int id)
        {
            var bankAccount = _bankAccountService.GetById(id);
            var viewModel = new BankAccountViewModel()
            {
                Id = bankAccount.Id,
                Name = bankAccount.Name,
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(BankAccountViewModel bankAccountViewModel)
        {
            var bankAccount = new BankAccount()
            {
                Id = bankAccountViewModel.Id,
            };
            _bankAccountService.Delete(bankAccount);

            return RedirectToAction("Index");
        }

        public JsonResult GetAllByContextId(int? contextId)
        {
            var id = contextId.HasValue ? contextId.Value : GetCurrentContextId();

            var listBankAccount = _bankAccountService.GetByContextId(id);

            var json = listBankAccount.Select(b => new 
            { 
                b.Id,
                b.Name
            });

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}