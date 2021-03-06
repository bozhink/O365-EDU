﻿/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Services.Web
{
    using System.IO;
    using System.Linq;
    using System.Web;
    using EDUGraphAPI.Services.Models.Web;
    using Newtonsoft.Json;

    public class DemoHelperService : IDemoHelperService
    {
        private const string DataFilePath = "~/App_Data/demo-pages.json";

        public DemoPage GetDemoPage(string controller, string action)
        {
            var path = HttpContext.Current.Server.MapPath(DataFilePath);
            var json = File.ReadAllText(path);
            var pages = JsonConvert.DeserializeObject<DemoPage[]>(json);

            var page = pages
                .Where(i => i.Controller.IgnoreCaseEquals(controller))
                .Where(i => i.Action.IgnoreCaseEquals(action))
                .FirstOrDefault();

            if (page != null)
            {
                var sourceCodeRepositoryUrl = EDUGraphAPI.Constants.Common.SourceCodeRepositoryUrl.TrimEnd('/');
                foreach (var link in page.Links)
                {
                    link.Url = sourceCodeRepositoryUrl + link.Url;
                }
            }

            return page;
        }
    }
}
