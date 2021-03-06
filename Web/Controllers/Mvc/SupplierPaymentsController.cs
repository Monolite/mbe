﻿// 
// SupplierPaymentsController.cs
// 
// Author:
//   Eddy Zavaleta <eddy@mictlanix.com>
//   Eduardo Nieto <enieto@mictlanix.com>
// 
// Copyright (C) 2011-2016 Eddy Zavaleta, Mictlanix, and contributors.
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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.ActiveRecord;
using Mictlanix.BE.Model;
using Mictlanix.BE.Web.Models;
using Mictlanix.BE.Web.Mvc;
using Mictlanix.BE.Web.Helpers;

namespace Mictlanix.BE.Web.Controllers.Mvc {
	[Authorize]
	public class SupplierPaymentsController : CustomController {
		//
		// GET: /SupplierPayment/

		public ActionResult Index ()
		{
			var qry = from x in SupplierPayment.Queryable
				  orderby x.Id descending
				  select x;

			Search<SupplierPayment> search = new Search<SupplierPayment> ();
			search.Limit = WebConfig.PageSize;
			search.Results = qry.Skip (search.Offset).Take (search.Limit).ToList ();
			search.Total = qry.Count ();

			return View (search);
		}

		// POST: /SupplierPayment/

		[HttpPost]
		public ActionResult Index (Search<SupplierPayment> search)
		{
			if (ModelState.IsValid) {
				search = GetSupplierPayments (search);
			}

			if (Request.IsAjaxRequest ()) {
				return PartialView ("_Index", search);
			} else {
				return View (search);
			}
		}

		Search<SupplierPayment> GetSupplierPayments (Search<SupplierPayment> search)
		{
			if (search.Pattern == null) {
				var qry = from x in SupplierPayment.Queryable
					  orderby x.Id descending
					  select x;

				search.Total = qry.Count ();
				search.Results = qry.Skip (search.Offset).Take (search.Limit).ToList ();
			} else {
				var qry = from x in SupplierPayment.Queryable
					  where x.Supplier.Name.Contains (search.Pattern)
					  orderby x.Id descending
					  select x;

				search.Total = qry.Count ();
				search.Results = qry.Skip (search.Offset).Take (search.Limit).ToList ();
			}

			return search;
		}

		//
		// GET: /SupplierPayment/Details/5

		public ActionResult Details (int id)
		{
			var item = SupplierPayment.Find (id);
			return View (item);
		}

		//
		// GET: /SupplierPayment/Create

		public ActionResult NewPay ()
		{
			return View (new SupplierPayment ());
		}

		//
		// POST: /SupplierPayment/Create

		[HttpPost]
		public ActionResult NewPay (SupplierPayment item)
		{
			if (!ModelState.IsValid)
				return View (item);

			item.Supplier = Supplier.Find (item.SupplierId);
			item.Date = DateTime.Now;
			item.Creator = CurrentUser.Employee;

			using (var scope = new TransactionScope ()) {
				item.CreateAndFlush ();
			}

			return RedirectToAction ("Index");
		}

		//
		// GET: /SupplierPayment/Delete/5

		public ActionResult Delete (int id)
		{
			return View ();
		}

		//
		// POST: /SupplierPayment/Delete/5

		[HttpPost]
		public ActionResult Delete (int id, FormCollection collection)
		{
			try {
				// TODO: Add delete logic here
				return RedirectToAction ("Index");
			} catch {
				return View ();
			}
		}
	}
}
