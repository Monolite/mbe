﻿// 
// BarcodesController.cs
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Mictlanix.BE.Web.Utils;
using Mictlanix.BE.Web.Mvc;

namespace Mictlanix.BE.Web.Controllers.Mvc {
	public class BarcodesController : CustomController {
		[AllowAnonymous]
		public ActionResult Code128 (string id)
		{
			var ms = new MemoryStream (4 * 1024);
			var img = Code128Rendering.MakeBarcodeImage (id, 2, false);

			img.Save (ms, ImageFormat.Png);
			ms.Seek (0, SeekOrigin.Begin);
			var result = new FileStreamResult (ms, "image/png");
			result.FileDownloadName = string.Format ("{0}.png", id);

			return result;
		}

		[AllowAnonymous]
		public ActionResult QRCode (string id)
		{
			return QRCodeAction (id);
		}

		internal static ActionResult QRCodeAction (string id)
		{
			try {
				var encoder = new QrEncoder ();
				var ms = new MemoryStream (4 * 1024);
				var render = new GraphicsRenderer (new FixedCodeSize (300, QuietZoneModules.Zero));
				QrCode code;

				if (!encoder.TryEncode (id, out code))
					return null;

				render.WriteToStream (code.Matrix, ImageFormat.Png, ms, new Point (300, 300));

				ms.Seek (0, SeekOrigin.Begin);
				var result = new FileStreamResult (ms, "image/png");
				result.FileDownloadName = string.Format ("{0}.png", id);

				return result;
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);
				return null;
			}
		}
	}
}
