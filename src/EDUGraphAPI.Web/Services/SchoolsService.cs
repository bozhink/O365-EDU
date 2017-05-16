/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Services.BingMaps;
    using EDUGraphAPI.Web.Models;
    using EDUGraphAPI.Web.ViewModels;
    using Microsoft.Education.Services;
    using Microsoft.Education.Services.Models;
    using Microsoft.Graph;

    /// <summary>
    /// A service class used to get education data by controllers
    /// </summary>
    public class SchoolsService : ISchoolsService
    {
        private readonly IEducationServiceClient client;
        private readonly IBingMapService mapService;
        private readonly ApplicationDbContext dbContext;

        public SchoolsService(IEducationServiceClient client, IBingMapService mapService, ApplicationDbContext dbContext)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (mapService == null)
            {
                throw new ArgumentNullException(nameof(mapService));
            }

            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            this.client = client;
            this.mapService = mapService;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Get SchoolsViewModel
        /// </summary>
        public async Task<SchoolsViewModel> GetSchoolsViewModelAsync(UserContext userContext)
        {
            var currentUser = userContext.IsStudent
                ? await this.client.GetStudentAsync() as SectionUser
                : await this.client.GetTeacherAsync() as SectionUser;

            var schools = (await this.client.GetSchoolsAsync())
                .OrderBy(i => i.Name)
                .ToArray();

            for (var i = 0; i < schools.Count(); i++)
            {
                var address = string.Format("{0}/{1}/{2}", schools[i].State, HttpUtility.HtmlEncode(schools[i].City), HttpUtility.HtmlEncode(schools[i].Address));
                if (!string.IsNullOrEmpty(schools[i].Address))
                {
                    var longitudeAndLatitude = await this.mapService.GetLongitudeAndLatitudeByAddressAsync(address);
                    if (longitudeAndLatitude.Length == 2)
                    {
                        schools[i].Latitude = longitudeAndLatitude[0];
                        schools[i].Longitude = longitudeAndLatitude[1];
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(schools[i].Zip))
                    {
                        schools[i].Address = "-";
                    }
                }
            }

            var mySchools = schools
                .Where(i => i.SchoolId == currentUser.SchoolId)
                .ToArray();

            var myFirstSchool = mySchools.FirstOrDefault();
            var grade = userContext.IsStudent ? currentUser.EducationGrade : myFirstSchool?.EducationGrade;

            var sortedSchools = mySchools.Union(schools.Except(mySchools));

            return new SchoolsViewModel(sortedSchools)
            {
                IsStudent = userContext.IsStudent,
                UserId = currentUser.UserId,
                EducationGrade = grade,
                UserDisplayName = currentUser.DisplayName,
                MySchoolId = currentUser.SchoolId,
                BingMapKey = EDUGraphAPI.Constants.Common.BingMapKey
            };
        }

        /// <summary>
        /// Get SectionsViewModel of the specified school
        /// </summary>
        public async Task<SectionsViewModel> GetSectionsViewModelAsync(UserContext userContext, string objectId, int top)
        {
            var school = await this.client.GetSchoolAsync(objectId);
            var mySections = await this.client.GetMySectionsAsync(school.SchoolId);
            mySections = mySections.OrderBy(c => c.CombinedCourseNumber).ToArray();
            var allSections = await this.client.GetAllSectionsAsync(school.SchoolId, top, null);

            return new SectionsViewModel(userContext, school, allSections, mySections);
        }

        /// <summary>
        /// Get SectionsViewModel of the specified school
        /// </summary>
        public async Task<SectionsViewModel> GetSectionsViewModelAsync(UserContext userContext, string objectId, int top, string nextLink)
        {
            var school = await this.client.GetSchoolAsync(objectId);
            var mySections = await this.client.GetMySectionsAsync(school.SchoolId);
            var allSections = await this.client.GetAllSectionsAsync(school.SchoolId, top, nextLink);

            return new SectionsViewModel(userContext.UserO365Email, school, allSections, mySections);
        }

        /// <summary>
        /// Get users, teachers and students of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolUsersAsync(string objectId, int top)
        {
            var school = await this.client.GetSchoolAsync(objectId);
            var users = await this.client.GetMembersAsync(objectId, top, null);
            var students = await this.client.GetStudentsAsync(school.SchoolId, top, null);
            var teachers = await this.client.GetTeachersAsync(school.SchoolId, top, null);

            return new SchoolUsersViewModel(school, users, students, teachers);
        }

        /// <summary>
        /// Get users of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolUsersAsync(string objectId, int top, string nextLink)
        {
            var school = await this.client.GetSchoolAsync(objectId);
            var users = await this.client.GetMembersAsync(objectId, top, nextLink);

            return new SchoolUsersViewModel(school, users, null, null);
        }

        /// <summary>
        /// Get students of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolStudentsAsync(string objectId, int top, string nextLink)
        {
            var school = await this.client.GetSchoolAsync(objectId);
            var students = await this.client.GetStudentsAsync(school.SchoolId, top, nextLink);

            return new SchoolUsersViewModel(school, null, students, null);
        }

        /// <summary>
        /// Get teachers of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolTeachersAsync(string objectId, int top, string nextLink)
        {
            var school = await this.client.GetSchoolAsync(objectId);
            var teachers = await this.client.GetTeachersAsync(school.SchoolId, top, nextLink);

            return new SchoolUsersViewModel(school, null, null, teachers);
        }

        /// <summary>
        /// Get SectionDetailsViewModel of the specified section
        /// </summary>
        public async Task<SectionDetailsViewModel> GetSectionDetailsViewModelAsync(string schoolId, string classId, IGroupRequestBuilder group)
        {
            var school = await this.client.GetSchoolAsync(schoolId);
            var section = await this.client.GetSectionAsync(classId);
            var driveRootFolder = await group.Drive.Root.Request().GetAsync();

            foreach (var user in section.Students)
            {
                var seat = dbContext.ClassroomSeatingArrangements.Where(c => c.O365UserId == user.O365UserId && c.ClassId == classId).FirstOrDefault();
                user.Position = (seat == null ? 0 : seat.Position);
            }

            return new SectionDetailsViewModel
            {
                School = school,
                Section = section,
                Conversations = await group.Conversations.Request().GetAllAsync(),
                SeeMoreConversationsUrl = string.Format(EDUGraphAPI.Constants.Common.O365GroupConversationsUrl, section.Email),
                DriveItems = await group.Drive.Root.Children.Request().GetAllAsync(),
                SeeMoreFilesUrl = driveRootFolder.WebUrl
            };
        }

        /// <summary>
        /// Get my classes
        /// </summary>
        public async Task<string[]> GetMyClassesAsync()
        {
            var myClasses = await this.client.GetMySectionsAsync();
            return myClasses
                .Select(i => i.SectionName)
                .ToArray();
        }
    }
}
