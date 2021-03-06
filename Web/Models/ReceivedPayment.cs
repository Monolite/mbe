﻿// 
// DateRange.cs
// 
// Author:
//   Eddy Zavaleta <eddy@mictlanix.com>
//   Eduardo Nieto <enieto@mictlanix.com>
//
// Copyright (C) 2011-2013 Eddy Zavaleta, Mictlanix, and contributors.
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
using System.ComponentModel.DataAnnotations;
using Mictlanix.BE.Model;

namespace Mictlanix.BE.Web.Models {
	public class ReceivedPayment
	{
		public ReceivedPayment()
		{
		}
		
		[DataType(DataType.Date)]
		[Display(Name = "Date", ResourceType = typeof(Resources))]
		public DateTime Date { get; set; }
		
		[Display(Name = "SalesOrder", ResourceType = typeof(Resources))]
		public int? SalesOrder { get; set; }
		
		[Display(Name = "Serial", ResourceType = typeof(Resources))]
		public int? Serial { get; set; }
		
		[Display(Name = "Customer", ResourceType = typeof(Resources))]
		public Customer Customer { get; set; }
		
		[Display(Name = "PaymentMethod", ResourceType = typeof(Resources))]
		public PaymentMethod Method { get; set; }
		
		[Display(Name = "Amount", ResourceType = typeof(Resources))]
		public decimal Amount { get; set; }
	}
}