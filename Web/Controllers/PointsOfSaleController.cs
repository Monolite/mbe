﻿// 
// PointsOfSaleController.cs
// 
// Author:
//   Eddy Zavaleta <eddy@mictlanix.org>
//   Eduardo Nieto <enieto@mictlanix.org>
// 
// Copyright (C) 2011 Eddy Zavaleta, Mictlanix, and contributors.
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.ActiveRecord;
using Mictlanix.BE.Model;

namespace Mictlanix.BE.Web.Controllers
{
    public class PointsOfSaleController : Controller
    {
        //
        // GET: /PointSale/

        public ViewResult Index ()
        {
            var qry = from x in PointOfSale.Queryable
                      select x;

            return View (qry.ToList ());
        }

        //
        // GET: /PointSale/Details/5

        public ViewResult Details (int id)
        {
            var item = PointOfSale.Find (id);
            return View (item);
        }

        //
        // GET: /PointSale/Create

        public ActionResult Create ()
        {
            return View ();
        }

        //
        // POST: /PointSale/Create

        [HttpPost]
        public ActionResult Create (PointOfSale item)
        {
            if (!ModelState.IsValid)
            	return View (item);
            
            item.Store = Store.Find (item.StoreId);
            
			using (var scope = new TransactionScope ()) {
            	item.CreateAndFlush ();
			}
			
            return RedirectToAction ("Index");
        }

        //
        // GET: /Warehouses/Edit/5

        public ActionResult Edit (int id)
        {
            var item = PointOfSale.Find (id);
            return View (item);
        }

        //
        // POST: /PointSale/Edit/5

        [HttpPost]
        public ActionResult Edit (PointOfSale item)
        {
            if (!ModelState.IsValid)
            	return View (item);
            
			var entity = PointOfSale.Find (item.Id);

			entity.Code = item.Code;
			entity.Name = item.Name;
			entity.Comment = item.Comment;

			using (var scope = new TransactionScope ()) {
				entity.UpdateAndFlush ();
			}

            return RedirectToAction ("Index");
        }

        //
        // GET: /PointSale/Delete/5

        public ActionResult Delete (int id)
        {
            return View (PointOfSale.Find (id));
        }

        //
        // POST: /Warehouses/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed (int id)
		{
			PointOfSale item = PointOfSale.Find (id);

			using (var scope = new TransactionScope ()) {
            	item.DeleteAndFlush ();
			}

			return RedirectToAction ("Index");
		}
		
		public JsonResult GetSuggestions (int store, string pattern)
		{
			var qry = from x in PointOfSale.Queryable
                      where x.Store.Id == store &&
							(x.Code.Contains (pattern) ||
					 		 x.Name.Contains (pattern))
                      select new { id = x.Id, name = x.Name };

			return Json (qry.Take (15).ToList (), JsonRequestBehavior.AllowGet);
		}
    }
}