﻿using MyWallet.Data.Domain;
using MyWallet.Data.Repository;
using MyWallet.Web.ViewModels.Import;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.Mvc;

namespace MyWallet.Web.Controllers
{
    public class ImportController : BaseController
    {
        private static NumberFormatInfo _numberFormatInfo;
        private UnitOfWork _unitOfWork;

        public ImportController()
        {
            _numberFormatInfo = new NumberFormatInfo();
            _numberFormatInfo.NumberDecimalSeparator = ",";

            _unitOfWork = new UnitOfWork();
        }

        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Upload(ImportViewModel importViewModel)
        {
            var contextId = GetCurrentContextId();

            var entries = new List<ImportViewModel>();
            using (var reader = new StreamReader(importViewModel.File.InputStream, Encoding.Default))
            {
                // Jump first line
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var array = line.Split(';');

                    var entry = new ImportViewModel();
                    entry.Date = ConvertToDate(array[0]);
                    entry.Description = array[1];
                    entry.Category = array[2].Trim();
                    entry.BankAccount = array[3];
                    entry.Value = ConvertToDecimal(array[4]);
                    entry.IsPaid = array[5] == "Pago" ? true : false;
                    entry.Observation = string.IsNullOrWhiteSpace(array[6]) ? null : array[6];

                    entries.Add(entry);
                }
            }

            var fileCategories = entries.Select(i => i.Category).Distinct();
            var existentCategories = _unitOfWork.CategoryRepository.GetByName(fileCategories, contextId);
            var fileBankAccounts = entries.Select(b => b.BankAccount).Distinct();
            var existentBankAccounts = _unitOfWork.BankAccountRepository.GetByName(fileBankAccounts, contextId);

            using (var scope = new TransactionScope())
            {

                foreach (var entry in entries)
                {
                    var existentCategory = existentCategories.FirstOrDefault(c => c.Name == entry.Category);
                    entry.CategoryId = existentCategory == null ? CreateCategoryAndReturnId(entry.Category, contextId)
                        : entry.CategoryId = existentCategory.Id;

                    var existentBankAccount = existentBankAccounts.FirstOrDefault(b => b.Name == entry.BankAccount);
                    entry.BankAccountId = existentBankAccount == null ? CreateBankAccountAndReturnId(entry.BankAccount, contextId)
                        : entry.BankAccountId = existentBankAccount.Id;

                    if (entry.Value > 0)
                    {
                        var income = new Income
                        {
                            Description = entry.Description,
                            BankAccountId = entry.BankAccountId,
                            CategoryId = entry.CategoryId,
                            ContextId = contextId,
                            CreationDate = DateTime.Now,
                            Date = entry.Date,
                            Received = entry.IsPaid,
                            Observation = entry.Observation,
                            Value = entry.Value
                        };
                        _unitOfWork.IncomeRepository.Add(income);
                    }
                    else
                    {
                        var expense = new Expense
                        {
                            Description = entry.Description,
                            BankAccountId = entry.BankAccountId,
                            CategoryId = entry.CategoryId,
                            ContextId = contextId,
                            CreationDate = DateTime.Now,
                            Date = entry.Date,
                            IsPaid = entry.IsPaid,
                            Observation = entry.Observation,
                            Value = -entry.Value
                        };
                        _unitOfWork.ExpenseRepository.Add(expense);
                    }
                }

                _unitOfWork.Commit();

                scope.Complete();
            }

            return RedirectToAction("Index", "Expense");
        }

        private int CreateBankAccountAndReturnId(string bankAccountName, int contextId)
        {
            var bankAccount = new BankAccount
            {
                Name = bankAccountName,
                ContextId = contextId,
                CreationDate = DateTime.Now
            };
            _unitOfWork.BankAccountRepository.Add(bankAccount);
            _unitOfWork.Commit();

            return bankAccount.Id;
        }

        private int CreateCategoryAndReturnId(string categoryName, int contextId)
        {
            var category = new Category
            {
                Name = categoryName,
                ContextId = contextId
            };

            _unitOfWork.CategoryRepository.Add(category);
            _unitOfWork.Commit();

            return category.Id;
        }

        private decimal ConvertToDecimal(string number)
        {
            return decimal.Parse(number, _numberFormatInfo);
        }

        private DateTime ConvertToDate(string date)
        {
            var numbers = date.Split('.');
            var day = int.Parse(numbers[0]);
            var month = int.Parse(numbers[1]);
            var year = int.Parse(numbers[2]);

            return new DateTime(year, month, day);
        }
    }
}