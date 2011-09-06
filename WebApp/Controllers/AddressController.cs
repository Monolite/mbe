﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Essentials.Model;

namespace Business.Essentials.WebApp.Controllers
{
    public class AddressController : Controller
    {
        //
        // GET: /Address/

        public ViewResult Index()
        {
            var qry = from x in Address.Queryable
                      select x;

            return View(qry.ToList());
        }

        //
        // GET: /Address/Details/5

        public ViewResult Details(int id)
        {
            Address address = Address.Find(id);
            return View(address);
        }

        //
        // GET: /Address/Create

        public ActionResult CreateForSupplier(int id)
        {
            ViewBag.OwnerId = id;
            ViewBag.OwnerType = "Suppliers";
            return View("Create");
        }

        public ActionResult CreateForCustomer(int id)
        {
            ViewBag.OwnerId = id;
            ViewBag.OwnerType = "Customers";
            return View("Create");
        }
        //
        // POST: /Address/Create

        [HttpPost]
        public ActionResult Create(Address address)
        {
            if (ModelState.IsValid)
            {
                int owner = int.Parse(Request.Params["OwnerId"]);
                string type = Request.Params["OwnerType"];

                if (type == "Suppliers")
                {
                    var supplier = Supplier.Find(owner);
                    address.Suppliers.Add(supplier);
                }

                if (type == "Customers")
                {
                    var customer = Customer.Find(owner);
                    address.Customers.Add(customer);
                }

                address.Create();

                return RedirectToAction("Details", type, new { id = owner });
            }

            return View(address);
        }

        //
        // GET: /Address/Edit/5

        public ActionResult Edit(int id)
        {
            Address address = Address.Find(id);
            return View(address);
        }

        //
        // POST: /Address/Edit/5

        [HttpPost]
        public ActionResult Edit(Address address)
        {
            if (ModelState.IsValid)
            {
                address.Save();
                return RedirectToAction("Index");
            }
            return View(address);
        }

        //
        // GET: /Address/Delete/5

        public ActionResult Delete(int id)
        {
            Address address = Address.Find(id);
            return View(address);
        }

        //
        // POST: /Address/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Address address = Address.Find(id);
            address.Delete();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}