﻿// 
// AccessPrivilege.cs
// 
// Author:
//   Eddy Zavaleta <eddy@mictlanix.org>
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
using System.ComponentModel.DataAnnotations;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;

namespace Business.Essentials.Model
{
    [ActiveRecord("access_privilege")]
    public class AccessPrivilege : ActiveRecordLinqBase<AccessPrivilege>
    {
        [PrimaryKey(PrimaryKeyType.Identity, "access_privilege_id")]
        public int Id { get; set; }

        [BelongsTo("user")]
        [Display(Name = "User", ResourceType = typeof(Resources))]
        public virtual User User { get; set; }

        [Property()]
        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "SystemObject", ResourceType = typeof(Resources))]
        public SystemObjects Object { get; set; }

        [Property]
        [Required(ErrorMessageResourceName = "Validation_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "Privileges", ResourceType = typeof(Resources))]
        public AccessRight Privileges { get; set; }

        public bool AllowCreate
        {
            get { return (Privileges & AccessRight.Create) == AccessRight.Create; }
            set { Privileges = value ? Privileges | AccessRight.Create : Privileges & ~AccessRight.Create; }
        }

        public bool AllowRead
        {
            get { return (Privileges & AccessRight.Read) == AccessRight.Read; }
            set { Privileges = value ? Privileges | AccessRight.Read : Privileges & ~AccessRight.Read; }
        }

        public bool AllowUpdate
        {
            get { return (Privileges & AccessRight.Update) == AccessRight.Update; }
            set { Privileges = value ? Privileges | AccessRight.Update : Privileges & ~AccessRight.Update; }
        }

        public bool AllowDelete
        {
            get { return (Privileges & AccessRight.Delete) == AccessRight.Delete; }
            set { Privileges = value ? Privileges | AccessRight.Delete : Privileges & ~AccessRight.Delete; }
        }
    }
}