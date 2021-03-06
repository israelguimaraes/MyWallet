﻿using MyWallet.Data.Domain;
using MyWallet.Web.ViewModels.BankAccount;
using System;
using System.Linq;
using System.Web.Mvc;
using MyWallet.Data.Repository;
using MyWallet.Common.Extensions;

namespace MyWallet.Web.Controllers
{
    [Authorize]
    public class BankAccountController : BaseController
    {
        private UnitOfWork _unitOfWork;

        public BankAccountController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public ActionResult Index()
        {
            var bankAccounts = _unitOfWork.BankAccountRepository.GetByContextId(GetCurrentContextId());
            var listViewModel = new ListAllBankAccountsViewModel();
            foreach (var bankAccount in bankAccounts)
            {
                var viewModel = new BankAccountViewModel()
                {
                    Id = bankAccount.Id,
                    Name = bankAccount.Name,
                    OpeningBalance = bankAccount.OpeningBalance
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

                _unitOfWork.BankAccountRepository.Save(bankAccount);
                _unitOfWork.Commit();

                return RedirectToAction("Index");
            }
            else
            {
                SendModelStateErrors();
                return View(bankAccountViewModel);
            }
        }

        public ActionResult Edit(string id)
        {
            var bankAccount = _unitOfWork.BankAccountRepository.GetById(id);

            var viewModel = new BankAccountViewModel();
            viewModel.Id = bankAccount.Id;
            viewModel.Name = bankAccount.Name;
            viewModel.OpeningBalance = bankAccount.OpeningBalance;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(BankAccountViewModel bankAccountViewModel)
        {
            if (ModelState.IsValid)
            {
                var bankAccountUpdate = _unitOfWork.BankAccountRepository.GetById(bankAccountViewModel.Id);

                bankAccountUpdate.Name = bankAccountViewModel.Name;
                bankAccountUpdate.OpeningBalance = bankAccountViewModel.OpeningBalance.Value;

                _unitOfWork.BankAccountRepository.Save(bankAccountUpdate);
                _unitOfWork.Commit();

                return RedirectToAction("Index");
            }
            else
            {
                SendModelStateErrors();
                return View(bankAccountViewModel);
            }
        }

        public ActionResult Delete(string id)
        {
            var bankAccount = _unitOfWork.BankAccountRepository.GetById(id);
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
            _unitOfWork.BankAccountRepository.Delete(bankAccountViewModel.Id);
            _unitOfWork.Commit();

            return RedirectToAction("Index");
        }

        public JsonResult GetAllByContextId(string contextId)
        {
            var id = contextId.IsNotNullOrEmpty() ? contextId : GetCurrentContextId();

            var listBankAccount = _unitOfWork.BankAccountRepository.GetByContextId(id);

            var json = listBankAccount.Select(b => new
            {
                b.Id,
                b.Name
            });

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}