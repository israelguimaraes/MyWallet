﻿using MyWallet.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWallet.Data.Repository
{
    public class BankAccountRepository
    {
        public void Add(BankAccount bankAccount)
        {
            using (var context = new MyWalletDBContext())
            {
                context.BankAccount.Add(bankAccount);
                context.SaveChanges();

            }
        }

        public void Update(BankAccount bankAccount)
        {
            using (var context = new MyWalletDBContext())
            {
                context.Entry(bankAccount).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void Delete(BankAccount bankAccount)
        {
            using (var context = new MyWalletDBContext())
            {
                context.Entry(bankAccount).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public IEnumerable<BankAccount> GetByContextId(int contextId)
        {
            using (var context = new MyWalletDBContext())
            {
                return context.BankAccount.Where(b => b.ContextId == contextId).ToList();
            }
        }

        public BankAccount GetById(int id)
        {
            using (var context = new MyWalletDBContext())
            {
                return context.BankAccount.Find(id);
            }
        }

        public IEnumerable<BankAccount> GetAll()
        {
            using (var context = new MyWalletDBContext())
            {
                return context.BankAccount.ToList();
            }
        }
    }
}
