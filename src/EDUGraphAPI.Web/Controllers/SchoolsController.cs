/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Utils;
    using EDUGraphAPI.Web.Infrastructure;
    using EDUGraphAPI.Web.Models;
    using EDUGraphAPI.Web.Services;
    using EDUGraphAPI.Web.ViewModels;
    using Microsoft.Education.Services.Models;

    [HandleAdalException, EduAuthorize]
    public class SchoolsController : Controller
    {
        private readonly ISchoolsServiceFactory schoolsServiceFactory;
        private IApplicationService applicationService;
        private ApplicationDbContext db;

        public SchoolsController(ISchoolsServiceFactory schoolsServiceFactory, IApplicationService applicationService, ApplicationDbContext db)
        {
            this.schoolsServiceFactory = schoolsServiceFactory ?? throw new ArgumentNullException(nameof(schoolsServiceFactory));
            this.applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // GET: /Schools/Index
        public async Task<ActionResult> Index()
        {
            var userContext = await this.applicationService.GetUserContextAsync();
            if (!userContext.AreAccountsLinked)
            {
                return this.View(new SchoolsViewModel { AreAccountsLinked = false, IsLocalAccount = userContext.IsLocalAccount });
            }

            var schoolsService = await this.schoolsServiceFactory.GetSchoolsServiceAsync(this.db);
            var model = await schoolsService.GetSchoolsViewModelAsync(userContext);
            model.AreAccountsLinked = userContext.AreAccountsLinked;

            return this.View(model);
        }

        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Classes
        public async Task<ActionResult> Classes(string schoolId)
        {
            var userContext = await this.applicationService.GetUserContextAsync();
            var schoolsService = await this.schoolsServiceFactory.GetSchoolsServiceAsync(this.db);
            var model = await schoolsService.GetSectionsViewModelAsync(userContext, schoolId, 12);
            return this.View(model);
        }

        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Classes/Next
        public async Task<JsonResult> ClassesNext(string schoolId, string nextLink)
        {
            var userContext = await this.applicationService.GetUserContextAsync();
            var schoolsService = await this.schoolsServiceFactory.GetSchoolsServiceAsync(this.db);
            var model = await schoolsService.GetSectionsViewModelAsync(userContext, schoolId, 12, nextLink);
            var sections = new List<Section>(model.Sections.Value);
            sections.AddRange(model.MySections);
            foreach (var section in sections)
            {
                if (!string.IsNullOrEmpty(section.TermStartDate))
                {
                    section.TermStartDate = Convert.ToDateTime(section.TermStartDate).ToString("yyyy-MM-ddTHH:mm:ss");
                }

                if (!string.IsNullOrEmpty(section.TermEndDate))
                {
                    section.TermEndDate = Convert.ToDateTime(section.TermEndDate).ToString("yyyy-MM-ddTHH:mm:ss");
                }
            }

            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Users
        public async Task<ActionResult> Users(string schoolId)
        {
            var schoolsService = await this.schoolsServiceFactory.GetSchoolsServiceAsync(this.db);
            var model = await schoolsService.GetSchoolUsersAsync(schoolId, 12);
            return this.View(model);
        }

        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Users/Next
        public async Task<JsonResult> UsersNext(string schoolId, string nextLink)
        {
            var schoolsService = await this.schoolsServiceFactory.GetSchoolsServiceAsync(this.db);
            var model = await schoolsService.GetSchoolUsersAsync(schoolId, 12, nextLink);
            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Students/Next
        public async Task<JsonResult> StudentsNext(string schoolId, string nextLink)
        {
            var schoolsService = await this.schoolsServiceFactory.GetSchoolsServiceAsync(this.db);
            var model = await schoolsService.GetSchoolStudentsAsync(schoolId, 12, nextLink);
            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Teachers/Next
        public async Task<JsonResult> TeachersNext(string schoolId, string nextLink)
        {
            var schoolsService = await this.schoolsServiceFactory.GetSchoolsServiceAsync(this.db);
            var model = await schoolsService.GetSchoolTeachersAsync(schoolId, 12, nextLink);
            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Classes/6510F0FC-53B3-4D9B-9742-84C9C8FA2BE4
        public async Task<ActionResult> ClassDetails(string schoolId, string sectionId)
        {
            var userContext = await this.applicationService.GetUserContextAsync();

            var graphServiceClient = await AuthenticationHelper.GetGraphServiceClientAsync();
            var group = graphServiceClient.Groups[sectionId];

            var schoolsService = await this.schoolsServiceFactory.GetSchoolsServiceAsync(this.db);
            var model = await schoolsService.GetSectionDetailsViewModelAsync(schoolId, sectionId, group);
            model.IsStudent = userContext.IsStudent;
            model.O365UserId = userContext.User.O365UserId;
            model.MyFavoriteColor = userContext.User.FavoriteColor;

            return this.View(model);
        }

        // POST: /Schools/SaveSeatingArrangements
        [HttpPost]
        public async Task<JsonResult> SaveSeatingArrangements(List<SeatingViewModel> seatingArrangements)
        {
            await this.applicationService.SaveSeatingArrangements(seatingArrangements);
            return this.Json(string.Empty);
        }
    }
}
