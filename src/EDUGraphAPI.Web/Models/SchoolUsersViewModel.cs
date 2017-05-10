/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Models
{
    using Microsoft.Education.Services.Models;

    public class SchoolUsersViewModel
    {
        public SchoolUsersViewModel(School school, ArrayResult<SectionUser> users, ArrayResult<SectionUser> students, ArrayResult<SectionUser> teachers)
        {
            this.School = school;
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
