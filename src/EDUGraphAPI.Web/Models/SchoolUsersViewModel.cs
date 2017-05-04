﻿/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

using Microsoft.Education;
using Microsoft.Education.Data;
using Microsoft.Education.Data.Models;

namespace EDUGraphAPI.Web.Models
{
    public class SchoolUsersViewModel
    {
        public SchoolUsersViewModel(School School, ArrayResult<SectionUser> users, ArrayResult<SectionUser> students, ArrayResult<SectionUser> teachers)
        {
            this.School = School;
            this.Users = users;
            this.Students = students;
            this.Teachers = teachers;
        }

        public School School { get; set; }
        public ArrayResult<SectionUser> Users { get; set; }
        public ArrayResult<SectionUser> Students { get; set; }
        public ArrayResult<SectionUser> Teachers { get; set; }
    }
}
