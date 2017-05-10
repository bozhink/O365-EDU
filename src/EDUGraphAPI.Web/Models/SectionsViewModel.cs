/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using EDUGraphAPI.Web.Models;
    using Microsoft.Education.Services.Models;

    public class SectionsViewModel
    {
        public SectionsViewModel(string userEmail, School school, ArrayResult<Section> sections, IEnumerable<Section> mySections)
        {
            this.UserEmail = userEmail;
            this.School = school;
            this.Sections = sections;
            this.MySections = mySections.ToList();
        }

        public SectionsViewModel(UserContext userContext, School school, ArrayResult<Section> sections, IEnumerable<Section> mySections)
        {
            this.UserEmail = userContext.UserO365Email;
            this.School = school;
            this.Sections = sections;
            this.MySections = mySections.ToList();
            this.UserContext = userContext;
        }

        public string UserEmail { get; set; }

        public School School { get; set; }

        public ArrayResult<Section> Sections { get; set; }

        public List<Section> MySections { get; set; }

        public UserContext UserContext { get; set; }

        public bool IsMy(Section section)
        {
            return this.MySections != null && this.MySections.Any(c => c.Email == section.Email);
        }
    }
}
