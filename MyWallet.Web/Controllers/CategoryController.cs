﻿using MyWallet.Common.Extensions;
using MyWallet.Data.Domain;
using MyWallet.Data.Repository;
using MyWallet.Web.ViewModels.Category;
using System.Linq;
using System.Web.Mvc;

namespace MyWallet.Web.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private UnitOfWork _unitOfWork;

        public CategoryController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public ActionResult Index()
        {
            var categories = _unitOfWork.CategoryRepository.GetByContextId(GetCurrentContextId());

            var listAllCategoriesViewModel = new ListAllCategoriesViewModel();
            foreach (var category in categories)
            {
                var categoryViewModel = new CategoryViewModel();
                categoryViewModel.Name = category.Name;
                categoryViewModel.Id = category.Id;

                listAllCategoriesViewModel.Categories.Add(categoryViewModel);
            }

            return View(listAllCategoriesViewModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category()
                {
                    Name = categoryViewModel.Name,
                    ContextId = GetCurrentContextId()
                };
                _unitOfWork.CategoryRepository.Save(category);
                _unitOfWork.Commit();

                return RedirectToAction("Index");
            }
            else
            {
                SendModelStateErrors();
                return View(categoryViewModel);
            }
        }

        public ActionResult Edit(string id)
        {
            Category category = _unitOfWork.CategoryRepository.GetById(id);

            var categoryViewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
            };

            return View(categoryViewModel);
        }

        [HttpPost]
        public ActionResult Edit(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category()
                {
                    Id = categoryViewModel.Id,
                    Name = categoryViewModel.Name,
                    ContextId = GetCurrentContextId()
                };
                _unitOfWork.CategoryRepository.Save(category);
                _unitOfWork.Commit();

                return RedirectToAction("Index");
            }
            else
            {
                SendModelStateErrors();
                return View(categoryViewModel);
            }
        }

        public ActionResult Delete(string id)
        {
            var category = _unitOfWork.CategoryRepository.GetById(id);
            var viewModel = new CategoryViewModel()
            {
                Id = category.Id,
                Name = category.Name,

            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(CategoryViewModel categoryViewModel)
        {
            _unitOfWork.CategoryRepository.Delete(categoryViewModel.Id);
            _unitOfWork.Commit();

            return RedirectToAction("Index");
        }

        public JsonResult GetAllByContextId(string contextId)
        {
            var id = contextId.IsNotNullOrEmpty() ? contextId : GetCurrentContextId();

            var listCategory = _unitOfWork.CategoryRepository.GetByContextId(id);

            var json = listCategory.Select(c => new
            {
                c.Id,
                c.Name
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