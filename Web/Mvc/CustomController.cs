﻿// 
// CustomController.cs
// 
// Author:
//   Eddy Zavaleta <eddy@mictlanix.com>
// 
// Copyright (C) 2016 Eddy Zavaleta, Mictlanix, and contributors.
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
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Mictlanix.BE.Web.Security;

namespace Mictlanix.BE.Web.Mvc {
	public abstract class CustomController : Controller {
		public const string MIME_TYPE_EXCEL_XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

		public CustomPrincipal CurrentUser {
			get {
				return User as CustomPrincipal;
			}
		}

		public FileResult ExcelFile (Stream stream, string fileName)
		{
			if (stream == null) {
				throw new ArgumentNullException ("stream");
			}

			stream.Seek (0, SeekOrigin.Begin);

			return File (stream, MIME_TYPE_EXCEL_XLSX, fileName);
		}

		public FileResult ExcelView (string viewPath, object model)
		{
			var content = RenderPartialView (viewPath, model);

			return File (Encoding.UTF8.GetBytes (content), "application/vnd.ms-excel");
		}

		public string RenderView (string viewPath, object model)
		{
			return RenderViewToStringInternal (viewPath, model, false);
		}

		public string RenderPartialView (string viewPath, object model)
		{
			return RenderViewToStringInternal (viewPath, model, true);
		}

		protected string RenderViewToStringInternal (string viewPath, object model, bool partial = false)
		{
			string result;
			var context = ControllerContext;
			ViewEngineResult viewEngineResult;

			if (partial)
				viewEngineResult = ViewEngines.Engines.FindPartialView (context, viewPath);
			else
				viewEngineResult = ViewEngines.Engines.FindView (context, viewPath, null); 

			if (viewEngineResult == null)
				throw new FileNotFoundException ("View could not be found.");

			context.Controller.ViewData.Model = model;

			using (var sw = new StringWriter ()) {
				var view = viewEngineResult.View;
				var ctx = new ViewContext (context, view, context.Controller.ViewData, context.Controller.TempData, sw);
				view.Render (ctx, sw);
				result = sw.ToString ();
			}

			return result;
		}
    }
}
