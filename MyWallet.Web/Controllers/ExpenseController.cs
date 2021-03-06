﻿using MyWallet.Data.Domain;
using MyWallet.Data.Repository;
using MyWallet.Web.ViewModels.Expense;
using System;
using System.Net;
using System.Web.Mvc;

namespace MyWallet.Web.Controllers
{
    [Authorize]
    public class ExpenseController : BaseController
    {
        private UnitOfWork _unitOfWork;

        public ExpenseController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public ActionResult Index()
        {
            var contextId = GetCurrentContextId();

            var expenseList = _unitOfWork.ExpenseRepository.GetAllByContextId(contextId);

            var viewModelList = new ListAllExpensesViewModel();
            viewModelList.Currency = _unitOfWork.CurrencyTypeRepository.GetCurrencySymbolByContextId(contextId);

            foreach (var item in expenseList)
            {
                var expense = new ExpenseViewModel()
                {
                    Id = item.Id,
                    Description = item.Description,
                    CategoryId = item.CategoryId,
                    IsPaid = item.IsPaid,
                    BankAccountId = item.BankAccountId,
                    Value = item.Value,
                    Date = item.Date,
                    Observation = item.Observation,
                    BankAccount = item.BankAccount.Name,
                    Category = item.Category.Name
                };

                viewModelList.Expenses.Add(expense);
            }
            return View(viewModelList);
        }

        [HttpPost]
        public HttpStatusCodeResult Create(ExpenseViewModel expenseViewModel)
        {
            if (ModelState.IsValid)
            {
                var expense = new Expense();
                expense.BankAccountId = expenseViewModel.BankAccountId;
                expense.CategoryId = expenseViewModel.CategoryId;
                expense.CreationDate = DateTime.Now;
                expense.Date = expenseViewModel.Date.Value;
                expense.Description = expenseViewModel.Description;
                expense.IsPaid = expenseViewModel.IsPaid;
                expense.Observation = expenseViewModel.Observation;
                expense.Value = expenseViewModel.Value.Value;
                expense.ContextId = GetCurrentContextId();

                _unitOfWork.ExpenseRepository.Save(expense);
                _unitOfWork.Commit();

                return new HttpStatusCodeResult(HttpStatusCode.Created);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public PartialViewResult GetExpenseById(string id)
        {
            var expense = _unitOfWork.ExpenseRepository.GetById(id);
            var expenseViewModel = new ExpenseViewModel
            {
                Id = expense.Id,
                Description = expense.Description,
                Value = expense.Value,
                Date = expense.Date,
                IsPaid = expense.IsPaid,
                BankAccountId = expense.BankAccountId,
                CategoryId = expense.CategoryId
            };

            var bankAccounts = _unitOfWork.BankAccountRepository.GetByContextId(expense.ContextId);
            foreach (var item in bankAccounts)
            {
                expenseViewModel.SelectListBankAccount.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            var categories = _unitOfWork.CategoryRepository.GetByContextId(expense.ContextId);
            foreach (var item in categories)
            {
                expenseViewModel.SelectListCategory.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            return PartialView("PartialView/_ExpenseEditFields", expenseViewModel);
        }

        [HttpPost]
        public HttpStatusCodeResult Edit(ExpenseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var expense = _unitOfWork.ExpenseRepository.GetById(viewModel.Id);
                expense.Date = viewModel.Date.Value;
                expense.Description = viewModel.Description;
                expense.CategoryId = viewModel.CategoryId;
                expense.BankAccountId = viewModel.BankAccountId;
                expense.Value = viewModel.Value.Value;
                expense.IsPaid = viewModel.IsPaid;

                _unitOfWork.ExpenseRepository.Save(expense);
                _unitOfWork.Commit();

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpStatusCodeResult Delete(string id)
        {
            _unitOfWork.ExpenseRepository.Delete(id);
            _unitOfWork.Commit();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public PartialViewResult GetExpenses()
        {
            var contextId = GetCurrentContextId();

            var expenseList = _unitOfWork.ExpenseRepository.GetAllByContextId(contextId);

            var viewModelList = new ListAllExpensesViewModel();
            viewModelList.Currency = "€";

            foreach (var item in expenseList)
            {
                var expense = new ExpenseViewModel()
                {
                    Id = item.Id,
                    Description = item.Description,
                    CategoryId = item.CategoryId,
                    IsPaid = item.IsPaid,
                    BankAccountId = item.BankAccountId,
                    Value = item.Value,
                    Date = item.Date,
                    Observation = item.Observation,
                    BankAccount = item.BankAccount.Name,
                    Category = item.Category.Name
                };

                viewModelList.Expenses.Add(expense);
            }
            return PartialView("~/Views/Expense/PartialView/_ExpensesList.cshtml", viewModelList);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}