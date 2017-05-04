/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EDUGraphAPI.Constants;

    public class EducationRegisterViewModel : RegisterViewModel
    {
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Favorite color")]
        public string FavoriteColor { get; set; }

        public List<ColorEntity> FavoriteColors { get; set; }
    }
}
